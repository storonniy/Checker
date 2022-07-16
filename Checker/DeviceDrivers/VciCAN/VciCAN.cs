using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;

namespace VciCAN
{

    //##########################################################################
    /// <summary>
    ///   This class provides the entry point for the IXXAT VCI .NET 2.0 API
    ///   demo application. 
    /// </summary>
    //##########################################################################
    public class CanConNet
    {
        public CanConNet()
        {
            SelectDevice();
            InitSocket(0);
            rxThread = new Thread(new ThreadStart(ReceiveThreadFunc));
            rxThread.Start();
        }

        public void Die()
        {
            rxThread.Abort();
        }

        ~CanConNet()
        {
            rxThread.Abort();
            FinalizeApp();
        }

        #region Member variables

        private readonly int delay = 1000;

        /// <summary>
        ///   Reference to the used VCI device.
        /// </summary>
        static IVciDevice mDevice;

        /// <summary>
        ///   Reference to the CAN controller.
        /// </summary>
        static ICanControl mCanCtl;

        /// <summary>
        ///   Reference to the CAN message communication channel.
        /// </summary>
        static ICanChannel mCanChn;

        /// <summary>
        ///   Reference to the CAN message scheduler.
        /// </summary>
        static ICanScheduler mCanSched;

        /// <summary>
        ///   Reference to the message writer of the CAN message channel.
        /// </summary>
        static ICanMessageWriter mWriter;

        /// <summary>
        ///   Reference to the message reader of the CAN message channel.
        /// </summary>
        static ICanMessageReader mReader;

        /// <summary>
        ///   Thread that handles the message reception.
        /// </summary>
        static Thread rxThread;

        /// <summary>
        ///   Quit flag for the receive thread.
        /// </summary>
        static long mMustQuit = 0;

        /// <summary>
        ///   Event that's set if at least one message was received.
        /// </summary>
        static AutoResetEvent mRxEvent;

        #endregion

        #region Device selection

        //************************************************************************
        /// <summary>
        ///   Selects the first CAN adapter.
        /// </summary>
        //************************************************************************
        public IVciDevice SelectDevice()
        {
            IVciDeviceManager deviceManager = null;
            //IVciDeviceManager deviceManager = null;
            IVciDeviceList deviceList = null;
            IEnumerator deviceEnum = null;

            try
            {
                //
                // Get device manager from VCI server
                //
                deviceManager = VciServer.Instance().DeviceManager;
                //deviceManager = VciServer.GetDeviceManager();
                //
                // Get the list of installed VCI devices
                //
                deviceList = deviceManager.GetDeviceList();
                //
                // Get enumerator for the list of devices
                //
                deviceEnum = deviceList.GetEnumerator();

                //
                // Get first device
                //
                deviceEnum.MoveNext();
                mDevice = deviceEnum.Current as IVciDevice;
                // show the device name and serial number
                /*                object serialNumberGuid = mDevice.UniqueHardwareId;
                                string serialNumberText = GetSerialNumberText(ref serialNumberGuid);*/
            }
            catch (Exception)
            {

            }
            finally
            {
                //
                // Dispose device manager ; it's no longer needed.
                //
                DisposeVciObject(deviceManager);

                //
                // Dispose device list ; it's no longer needed.
                //
                DisposeVciObject(deviceList);

                //
                // Dispose device list ; it's no longer needed.
                //
                DisposeVciObject(deviceEnum);
            }
            return mDevice;
        }

        #endregion

        #region Opening socket

        //************************************************************************
        /// <summary>
        ///   Opens the specified socket, creates a message channel, initializes
        ///   and starts the CAN controller.
        /// </summary>
        /// <param name="canNo">
        ///   Number of the CAN controller to open.
        /// </param>
        /// <returns>
        ///   A value indicating if the socket initialization succeeded or failed.
        /// </returns>
        //************************************************************************
        bool InitSocket(Byte canNo)
        {
            IBalObject bal = null;
            bool succeeded = false;

            try
            {
                //
                // Open bus access layer
                //
                bal = mDevice.OpenBusAccessLayer();

                //
                // Open a message channel for the CAN controller
                //
                mCanChn = bal.OpenSocket(canNo, typeof(ICanChannel)) as ICanChannel;

                //
                // Open the scheduler of the CAN controller
                //
                mCanSched = bal.OpenSocket(canNo, typeof(ICanScheduler)) as ICanScheduler;

                // Initialize the message channel
                mCanChn.Initialize(1024, 128, false);

                // Get a message reader object
                mReader = mCanChn.GetMessageReader();

                // Initialize message reader
                mReader.Threshold = 1;

                // Create and assign the event that's set if at least one message
                // was received.
                mRxEvent = new AutoResetEvent(false);
                mReader.AssignEvent(mRxEvent);

                // Get a message wrtier object
                mWriter = mCanChn.GetMessageWriter();

                // Initialize message writer
                mWriter.Threshold = 1;

                // Activate the message channel
                mCanChn.Activate();


                //
                // Open the CAN controller
                //
                mCanCtl = bal.OpenSocket(canNo, typeof(ICanControl)) as ICanControl;

                // Initialize the CAN controller
                mCanCtl.InitLine(CanOperatingModes.Standard |
                  CanOperatingModes.Extended |
                  CanOperatingModes.ErrFrame,
                  CanBitrate.Cia500KBit);

                // Set the acceptance filter for std identifiers
                mCanCtl.SetAccFilter(CanFilter.Std,
                                     (uint)CanAccCode.All, (uint)CanAccMask.All);

                // Set the acceptance filter for ext identifiers
                mCanCtl.SetAccFilter(CanFilter.Ext,
                                     (uint)CanAccCode.All, (uint)CanAccMask.All);

                // Start the CAN controller
                mCanCtl.StartLine();

                succeeded = true;
            }
            catch (Exception exc)
            {
                succeeded = false;
            }
            finally
            {
                //
                // Dispose bus access layer
                //
                DisposeVciObject(bal);
            }
            return succeeded;
        }

        #endregion

        #region Message transmission

        /// <summary>
        ///   Transmits a CAN message with ID 0x100.
        /// </summary
        /// 
        /// 
        public void TransmitData(byte[] message, uint ID)
        {
            IMessageFactory factory = VciServer.Instance().MsgFactory;
            ICanMessage canMsg = (ICanMessage)factory.CreateMsg(typeof(ICanMessage));
            canMsg.ExtendedFrameFormat = true;
            canMsg.TimeStamp = 0;
            canMsg.Identifier = ID;
            canMsg.FrameType = CanMsgFrameType.Data;
            canMsg.DataLength = 8;
            canMsg.SelfReceptionRequest = false;
            for (var i = 0; i < canMsg.DataLength; i++)
            {
                canMsg[i] = message[i];
            }
            mWriter.SendMessage(canMsg);
            Thread.Sleep(10);
        }

        #endregion

        #region Message reception

        //************************************************************************
        /// <summary>
        ///   This method is the works as receive thread.
        /// </summary>
        //************************************************************************

        public ICanMessage lastCanMsg { get; private set; }

        private Queue<ICanMessage> msgQueue = new Queue<ICanMessage>();

        public void ReceiveThreadFunc()
        {
            IMessageFactory factory = VciServer.Instance().MsgFactory;
            ICanMessage canMessage = (ICanMessage)factory.CreateMsg(typeof(ICanMessage));
            do
            {
                if (mRxEvent.WaitOne(100, false))
                {
                    while (mReader.ReadMessage(out canMessage))
                    {
                        if (!canMessage.RemoteTransmissionRequest)
                        {
                            lock (msgQueue)
                            {
                                msgQueue.Enqueue(canMessage);
                            }
                        }
                    }
                }
            } while (0 == mMustQuit);
        }

        public ICanMessage GetData()
        {
            lock (msgQueue)
            {
                if (msgQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    if (msgQueue.Count == 0)
                        return null;
                }
                return msgQueue.Dequeue();
            }
        }


        #endregion

        #region Utility methods


        //************************************************************************
        /// <summary>
        ///   Finalizes the application 
        /// </summary>
        //************************************************************************
        public void FinalizeApp()
        {
            //
            // Dispose all hold VCI objects.
            //

            // Dispose message reader
            DisposeVciObject(mReader);

            // Dispose message writer 
            DisposeVciObject(mWriter);

            // Dispose CAN channel
            DisposeVciObject(mCanChn);

            // Dispose CAN controller
            DisposeVciObject(mCanCtl);

            // Dispose VCI device
            DisposeVciObject(mDevice);
        }


        //************************************************************************
        /// <summary>
        ///   This method tries to dispose the specified object.
        /// </summary>
        /// <param name="obj">
        ///   Reference to the object to be disposed.
        /// </param>
        /// <remarks>
        ///   The VCI interfaces provide access to native driver resources. 
        ///   Because the .NET garbage collector is only designed to manage memory, 
        ///   but not native OS and driver resources the application itself is 
        ///   responsible to release these resources via calling 
        ///   IDisposable.Dispose() for the obects obtained from the VCI API 
        ///   when these are no longer needed. 
        ///   Otherwise native memory and resource leaks may occure.  
        /// </remarks>
        //************************************************************************
        void DisposeVciObject(object obj)
        {
            if (null != obj)
            {
                IDisposable dispose = obj as IDisposable;
                if (null != dispose)
                {
                    dispose.Dispose();
                    obj = null;
                }
            }
        }

        #endregion
    }
}

