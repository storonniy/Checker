using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using static Checker.Settings.ControlObjectSettings;
using System.ComponentModel;
using System.Linq;
using Checker.DataBase;
using Checker.Devices;
using Checker.Logging;
using Checker.Steps;
using Checker.Device;
using Checker.DeviceInterface;
using Checker.Settings;

namespace Checker
{
    public partial class Form1 : Form
    {
        #region Глобальные переменные

        static Dictionary<TreeNode, Steps.Step> treeviewNodeStep = new Dictionary<TreeNode, Steps.Step>();
        static Dictionary<Steps.Step, TreeNode> treeviewStepNode = new Dictionary<Steps.Step, TreeNode>();
        static Dictionary<DeviceNames, Label> deviceLabelDictionary = new Dictionary<DeviceNames, Label>();
        static StepsInfo stepsInfo;
        static DeviceInit DeviceHandler;
        ControlObjectSettings.Settings settings = new ControlObjectSettings.Settings();

        static Form1 form;
        static Log log;

        Thread mainThread = new Thread(some);//stepsDictionary
        static Queue<Step> queue = new Queue<Step>();
        static Form1 EventSend;
        static bool checkingResult = true;
        static bool isCheckingStarted = false;
        static bool isCheckingInterrupted = false;
        static bool isCheckingPaused = false;

        static bool isStepByStepMode = false;

        private static void some()
        {
            while (true)
            {
                DoNextStep();
            }
        }

        private static void DoNextStep()
        {
            Step step = null;
            lock (queue)
            {
                if (queue.Count != 0 && isCheckingStarted)
                {
                    step = queue.Dequeue();
                    Thread.Sleep(10);
                }
            }
            if (step != null)
            {
                if (step.ShowStep)
                {
                    var node = treeviewStepNode[step];
                    form.HighlightTreeNode(node, Color.Blue);
                }
                var stepResult = DoStep(step);
                if (step.Argument == "")
                {
                    MessageBox.Show($"Шаг {step.Description}: Аргумент пустой: {step.Argument}");
                }
                if (step.ShowStep)
                {
                    ShowStepResult(step, stepResult);
                }
            }
            else if (isCheckingStarted)
            {
                isCheckingStarted = false;
                var result = checkingResult ? "ОК исправен." : "ОК неисправен";
                if (isCheckingInterrupted)
                {
                    result = $"Проверка прервана, результаты проверки записаны в файл.";
                }
                else
                {
                    result = $"Проверка завершена, результаты проверки записаны в файл. {result}";
                }
                log.Send(result);
                MessageBox.Show(result);
                form.ChangeStartButtonState();
                form.ChangeButton(form.buttonCheckingPause, "Пауза");
                form.CleanTreeView();
                form.BlockControls(false);
            }
            else
            {
                Thread.Sleep(42);
            }
        }
        #endregion

        #region Конструктор Form1
        public Form1(ControlObjectSettings.Settings settings)
        {
            form = this;
            InitializeComponent();
            EventSend = this;
            this.settings = settings;
            ShowSettings();
            InitialActions();       
            this.Text = stepsInfo.ProgramName;
            buttonCheckingPause.Enabled = false;
            mainThread.Start();
        }

        #endregion

        #region Показать режимы и настройки

        private void ShowSettings()
        {
            textBoxComment.Text = (settings.Comment != "") ? settings.Comment : "Не указано";
            textBoxFactoryNumber.Text = settings.FactoryNumber.ToString();
            textBoxOperatorName.Text = (settings.OperatorName != "") ? settings.OperatorName : "Не указано";
        }

        private void SetVoltageSupplyModes()
        {
            foreach (var modeName in stepsInfo.VoltageSupplyModesDictionary.Keys)
            {
                comboBoxVoltageSupply.Items.Add(modeName);
            }
            var selectedItemNumber = 0;
            if (comboBoxVoltageSupply.Items.Count > 1)
            {
                selectedItemNumber = 1;
            }
            comboBoxVoltageSupply.SelectedItem = comboBoxVoltageSupply.Items[selectedItemNumber];
        }

        private void ShowCheckingModes()
        {
            foreach (var modeName in stepsInfo.ModesDictionary.Keys)
            {
                comboBoxCheckingMode.Items.Add(modeName);
            }
            var selectedItemNumber = 0;
            if (comboBoxCheckingMode.Items.Count > 1)
            {
                selectedItemNumber = 1;
            }
            comboBoxCheckingMode.SelectedItem = comboBoxCheckingMode.Items[selectedItemNumber];
        }

        private void SelectCheckingMode()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            FillTreeView(treeOfChecking, stepsInfo.ModesDictionary[modeName]);
            if (modeName == "Полная проверка")
            {
                SetVoltageSupplyMode();
                comboBoxVoltageSupply.Enabled = true;
            }
            else
            {
                comboBoxVoltageSupply.Enabled = false;
            }
            if (modeName == "Режим самопроверки")
            {
                labelAttention.ForeColor = Color.Red;
                labelAttention.Text = "Объект контроля должен быть отстыкован!";
            }
            else
            {
                labelAttention.Text = "";
            }
        }

        private void SetVoltageSupplyMode()
        {
            var modeName = comboBoxVoltageSupply.SelectedItem.ToString();
            FillTreeView(treeOfChecking, stepsInfo.VoltageSupplyModesDictionary[modeName]);
        }

        #endregion

        #region Устройства
        private void InitDevices()
        {
            DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
            foreach (var device in stepsInfo.DeviceList)
            {
                if (device.Status == DeviceStatus.OK)
                {
                    break;
                }
            }
            InitDevices();
        }

/*        private void ShowDevicesOnForm()
        {
            var deviceList = stepsInfo.DeviceList;
            foreach (var device in deviceList)
            {
                if (device.Name.ToString() == "None")
                    continue;
                var labelDevice = new Label()
                {
                    Text = device.Name.ToString(),
                    Location = new Point(20, 30 + 25 * groupBoxDevices.Controls.Count)
                };
                groupBoxDevices.Controls.Add(labelDevice);
                deviceLabelDictionary.Add(device.Name, labelDevice);
            }
        }

        private void UpdateDevicesOnForm()
        {
            foreach (var device in stepsInfo.DeviceList)
            {
                if (device.Name.ToString() == "None")
                    continue;
                var deviceLabel = deviceLabelDictionary[device.Name];
                if (device.SerialPort.IsOpen)
                {
                    deviceLabel.ForeColor = Color.Green;
                }
                else
                {
                    deviceLabel.ForeColor = Color.Red;
                }
            }
        }*/

        #endregion

        #region Инициализация 

        private void InitialActions(string pathToDataBase)
        {
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", pathToDataBase);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathToDataBase;
            var dbReader = new DbReader(connectionString);
            var dataSet = dbReader.GetDataSet();
            stepsInfo = Steps.Step.GetStepsInfo(dataSet);
            SetVoltageSupplyModes();
            ShowCheckingModes();
            //ReplaceVoltageSupplyInStepsDictionary();
            try
            {
                DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitialActions()
        {
            var connectionString = "NS03.xlsx;";
            InitialActions(connectionString);
        }

        #endregion

        #region Логирование

        private void CreateLog()
        {
            log = new Log(settings);
            log.Send($"Время начала проверки: {DateTime.Now}\n");
            log.Send($"Имя оператора: {settings.OperatorName}\n");
            log.Send($"Комментарий: {settings.Comment}\n");
            log.Send($"Заводской номер: {settings.FactoryNumber}\n");
            var modeName = GetModeName();
            log.Send($"Режим: {modeName}\r\n");
        }

        private string GetModeName()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            if (modeName == "Полная проверка")
            {
                modeName = comboBoxVoltageSupply.SelectedItem.ToString();
            }
            return modeName;
        }

        #endregion

        #region Проверка

        private void EnQueueCheckingSteps(string modeName)//(Dictionary<string, List<Step>> stepsDictionary)
        {
            var stepsDictionary = (modeName.Contains("ОУ") || modeName.Contains("НУ")) ? stepsInfo.VoltageSupplyModesDictionary[modeName] : stepsInfo.ModesDictionary[modeName];
            foreach (var step in stepsDictionary.Keys.SelectMany(tableName => stepsDictionary[tableName]))
            {
                lock (queue)
                {
                    queue.Enqueue(step);
                }
            }
        }

        private static void ShowStepResult(Steps.Step step, DeviceResult deviceResult)
        {
            var node = treeviewStepNode[step];
            form.HighlightTreeNode(node, Color.Blue);
            var indexOf = node.Text.IndexOf(' ');
            var stepNumber = int.Parse(node.Text.Substring(0, indexOf));
            var result = $"Шаг {stepNumber}: {step.Description}\r\n{deviceResult.Description}\r\n\r\n";
            log.Send(result);
            log.Send(DateTime.Now.ToString());
            form.AddSubTreeNode(node, deviceResult.Description);
            var color = Color.Black;
            switch (deviceResult.State)
            {
                case DeviceStatus.OK:
                    color = Color.Green;
                    break;
                case DeviceStatus.ERROR:
                case DeviceStatus.NOT_CONNECTED:
                    color = Color.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            form.HighlightTreeNode(node, color);
        }

        private static void ShowErrorDialog(string description)
        {
            var dialogResult = MessageBox.Show(description, "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (dialogResult == DialogResult.Yes)
            {
                isCheckingInterrupted = true;
                isCheckingStarted = false;
                //form.AddSubTreeNode(node, "Аварийная остановка проверки");               
                MessageBox.Show("Проверка остановлена.");
                form.AbortChecking();
                form.ChangeStartButtonState();
            }
            else if (dialogResult == DialogResult.No)
            {
                isCheckingStarted = true;
                form.ChangeControlState(form.groupBoxManualStep, true);
                //form.ChangeCheckingState(false);
            }
        }

        private static DeviceResult DoStep(Steps.Step step)
        {
            var stepParser = new StepParser(DeviceHandler, step);
            var deviceResult = stepParser.DoStep();
            if (deviceResult.State == DeviceStatus.ERROR || deviceResult.State == DeviceStatus.NOT_CONNECTED)
            {             
                checkingResult = false;
                if (!form.checkBoxIgnoreErrors.Checked)
                {
                    isCheckingStarted = false;
                    var description = $"В ходе проверки произошла ошибка:\r\nШаг: {step.Description}\r\nРезультат шага: {deviceResult.Description}\r\nОстановить проверку?";
                    ShowErrorDialog(description);
                }
            }
            if (deviceResult.State == DeviceStatus.NOT_CONNECTED)
            {
                /*
                DeviceHandler.CloseDevicesSerialPort(stepsInfo.DeviceList);
                form.UpdateDevicesOnForm();
                DeviceHandler = new DeviceInit(stepsInfo.DeviceList);
                form.UpdateDevicesOnForm();
                */
            }
            return deviceResult;
        }

        #endregion

        #region Выборочная проверка

        private static List<Steps.Step> GetSelectedSteps()
        {
            var stepList = new List<Steps.Step>();
            foreach (var node in treeviewNodeStep.Keys)
            {
                if (node.Checked)
                {
                    var step = treeviewNodeStep[node];
                    stepList.Add(step);
                }
            }
            return stepList;
        }

        private void DoSelectedSteps(List<Steps.Step> stepList)
        {
            CreateLog();
            log.Send("Выполнение выбранных оператором шагов проверки: \r\n");
            foreach (var step in stepList)
            {
                lock (queue)
                {
                    queue.Enqueue(step);
                }
            }
            while (checkBoxCycle.Checked)
            {
                foreach (var step in stepList)
                {
                    lock (queue)
                    {
                        queue.Enqueue(step);
                    }
                }
            }
            BlockControls(false);
        }

        private void DoStepList(List<Steps.Step> stepList)
        {
            foreach (var step in stepList)
            {
                lock (queue)
                {
                    queue.Enqueue(step);
                }
            }
        }

        #endregion

        #region Управление потоком проверки

        private void AbortChecking()
        {         
            isCheckingStarted = false;           
            lock (queue)
            {
                queue.Clear();         
                foreach (var step in stepsInfo.EmergencyStepList)
                {
                    queue.Enqueue(step);
                }
                isCheckingStarted = true;
            }
            IDeviceInterface.ClearCoefficientDictionary();
            IDeviceInterface.ClearValuesDictionary();
            Thread.Sleep(3000);
            CleanTreeView();
//BlockControls(false);
        }

        private void ChangeStartButtonState()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeStartButtonState(); });
                return;
            }
            //isCheckingStarted = !isCheckingStarted;
            var buttonText = isCheckingStarted ? "Стоп" : "Старт";
            buttonCheckingStart.Text = buttonText;
        }

        private void ChangeButtonPauseResume()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeButtonPauseResume(); });
                return;
            }
            var buttonText = isCheckingStarted ? "Пауза" : "Продолжить";
            buttonCheckingPause.Text = buttonText;
        }

        private void ChangeButton(Button button, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeButton(button, text); });
                return;
            }
            button.Text = text;
        }

        private void ChangeCheckingState(bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeCheckingState(state); });
                return;
            }
            isCheckingStarted = state;
            var buttonText = isCheckingStarted ? "Пауза" : "Продолжить";
            buttonCheckingPause.Text = buttonText;
        }

        #endregion

        #region TreeNodes

        private static void FillTreeView(TreeView treeView, Dictionary<string, List<Steps.Step>> stepDictionary)
        {
            treeviewNodeStep.Clear();
            treeviewStepNode.Clear();
            treeView.Nodes.Clear();
            treeView.BeginUpdate();
            var nodesCount = 0;
            foreach (var tableName in stepDictionary.Keys)
            {
                var treeNode = new TreeNode(tableName);
                treeView.Nodes.Add(treeNode);
                foreach (var step in stepDictionary[tableName])
                {
                    if (step.ShowStep)
                    {
                        nodesCount++;
                        try
                        {
                            var nodeName = $"{nodesCount} {step.Description}";
                            var stepNode = new TreeNode(nodeName);
                            treeviewNodeStep.Add(stepNode, step);
                            //stepNode.Checked = true;
                            treeviewStepNode.Add(step, stepNode);
                            treeNode.Nodes.Add(stepNode);
                            treeNode.Expand();
                        }
                        catch (ArgumentException ex)
                        {
                            MessageBox.Show("ArgumentException " + ex.Message);
                        }
                    }
                }

            }
            treeView.EndUpdate();
        }

        private void HighlightTreeNode(TreeNode treeNode, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { HighlightTreeNode(treeNode, color); });
                return;
            }
            treeNode.EnsureVisible();
            treeNode.ForeColor = color;
        }

        private void AddSubTreeNode(TreeNode parentTreeNode, string stepResult)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { AddSubTreeNode(parentTreeNode, stepResult); });
                return;
            }
            parentTreeNode.Nodes.Add(stepResult);
            //parentTreeNode.Expand();
        }

        #endregion

        #region Блокировка и очистка элементов формы

        private void BlockControls(bool state)
        {
            ChangeControlState(buttonCheckingStop, state);
            ChangeControlState(buttonCheckingPause, state);
            ChangeControlState(buttonCheckingStart, !state);
            ChangeControlState(buttonOpenDataBase, !state);
            ChangeControlState(groupBoxPreferences, !state);
            ChangeControlState(groupBoxManualStep, !state);
        }

        private void CleanAll()
        {
            treeOfChecking.Nodes.Clear();
            comboBoxCheckingMode.Items.Clear();
            comboBoxVoltageSupply.Items.Clear();
            treeviewNodeStep.Clear();
            treeviewStepNode.Clear();
        }
        private void CleanTreeView()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { CleanTreeView(); });
                return;
            }
            treeOfChecking.Nodes.Clear();
            SelectCheckingMode();
        }
        private void ChangeControlState(Control control, bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeControlState(control, state); });
                return;
            }
            control.Enabled = state;
        }

        #endregion     

        #region Методы элементов управления

        private void StartStopChecking()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { StartStopChecking(); });
                return;
            }
            if (isCheckingStarted || isCheckingPaused)
            {
                isCheckingStarted = false;
                isCheckingInterrupted = true;
                AbortChecking();
            }
            else
            {
                IDeviceInterface.ClearCoefficientDictionary();
                IDeviceInterface.ClearValuesDictionary();
                checkingResult = true;
                isCheckingInterrupted = false;
                CreateLog();
                var modeName = GetModeName();
                EnQueueCheckingSteps(modeName);
                isCheckingStarted = true;
                BlockControls(isCheckingStarted);
            }
            ChangeStartButtonState();
        }

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            checkingResult = true;
            isCheckingInterrupted = false;
            CreateLog();
            var modeName = GetModeName();
            EnQueueCheckingSteps(modeName);
            isCheckingStarted = true;
            BlockControls(isCheckingStarted);
            //StartStopChecking();
        }
        private void buttonCheckingStop_Click(object sender, EventArgs e)
        {
            ChangeControlState(buttonCheckingStop, false);
            isCheckingStarted = false;
            isCheckingInterrupted = true;
            AbortChecking();
        }

        private void buttonOpenDataBase_Click(object sender, EventArgs e)
        {
            //DoStepList(stepsInfo.EmergencyStepList);
            OpenFileDialog openBinFileDialog = new OpenFileDialog();
            openBinFileDialog.Filter = "Файлы *.xls* | *xls*";//"Файлы *.accdb | *.accdb | Файлы *.mdb | *.mdb"; "Файлы *.*db | *.*db"
            if (openBinFileDialog.ShowDialog() == DialogResult.OK)
            {
                CleanAll();
                InitialActions(openBinFileDialog.FileName);
            }
        }
        private void buttonCheckingPause_Click(object sender, EventArgs e)
        {
            isCheckingStarted = !isCheckingStarted;
            ChangeButtonPauseResume();
            ChangeControlState(buttonStep, !isCheckingStarted);
        }

        private void comboBoxCheckingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectCheckingMode();
        }

        private void comboBoxVoltageSupply_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ReplaceVoltageSupplyInStepsDictionary();
            SetVoltageSupplyMode();
        }

        private void treeOfChecking_AfterSelect(object sender, TreeViewEventArgs e)
        {

/*            if (treeviewNodeStep.ContainsKey(e.Node) && mainThread.ThreadState != ThreadState.Running)
            {
                var thisStep = treeviewNodeStep[e.Node];
                var node = treeviewStepNode[thisStep];
                DoStep(thisStep);
            }*/

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var stepList = GetSelectedSteps();
            BlockControls(true);
            DoSelectedSteps(stepList);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AbortChecking();
            CleanTreeView();
        }

        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Закомментированные методы
        /*
        private void ReplaceVoltageSupplyInStepsDictionary()
        {
            var stepsDictionary = stepsInfo.StepsDictionary;
            foreach (var tableName in stepsDictionary.Keys)
            {
                foreach (var step in stepsDictionary[tableName])
                {
                    if (tableName == "'Установка напряжения питания'" && step.Command == "SetVoltage" && step.Device.Contains("power") || step.Device.Contains("Power"))
                    {
                        var voltage = stepsInfo.VoltageSupplyDictionary[comboBoxVoltageSupply.SelectedItem.ToString()];
                        step.Argument = voltage.ToString();
                        step.Description = $"Установка напряжения питания {step.Argument} на ИП1";
                    }
                }
            }
            stepsInfo.StepsDictionary = stepsDictionary;
        }
        */
        #endregion

        #region Обработка закрытия формы
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (queue.Count != 0)
            {
                AbortChecking();
                Thread.Sleep(3000);
            }
            if (isCheckingStarted)
            {
                isCheckingInterrupted = true;
                var result = $"Проверка прервана, результаты проверки записаны в файл.";
                log.Send(result);
            }
            Die();
            Application.Exit();
            mainThread.Abort();
        }

        private static void Die()
        {
            if (DeviceHandler == null)
                return;
            DeviceHandler.Devices
                .Values
                .Where(dev => dev != null)
                .ToList()
                .ForEach(device => device.Die());
        }

        #endregion

        private void buttonStep_Click(object sender, EventArgs e)
        {
            Steps.Step step = null;
            lock (queue)
            {
                if (queue.Count != 0)
                {
                    step = queue.Dequeue();
                    Thread.Sleep(10);
                }
            }
            if (step != null)
            {
                if (step.ShowStep)
                {
                    var node = treeviewStepNode[step];
                    form.HighlightTreeNode(node, Color.Blue);
                }
                var stepResult = DoStep(step);
                if (step.Argument == "")
                {
                    MessageBox.Show($"Шаг {step.Description}: Аргумент пустой: {step.Argument}");
                }
                if (step.ShowStep)
                {
                    ShowStepResult(step, stepResult);
                }
            }
        }

        private void textBoxComment_TextChanged(object sender, EventArgs e)
        {
            settings.Comment = textBoxComment.Text;
        }

        private void textBoxFactoryNumber_TextChanged(object sender, EventArgs e)
        {
            settings.FactoryNumber = textBoxFactoryNumber.Text;
        }

        private void textBoxOperatorName_TextChanged(object sender, EventArgs e)
        {
            settings.OperatorName = textBoxOperatorName.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isCheckingInterrupted = true;
            AbortChecking();
        }

        private void buttonShowRelays_Click(object sender, EventArgs e)
        {
            var stepGetMkRelays = new Step(DeviceNames.MK, DeviceCommands.GetClosedRelayNames);
            var stepGetSimulatorRelays = new Step(DeviceNames.MK, DeviceCommands.GetClosedRelayNames);
            try
            {
                var relays = DeviceHandler.Devices[DeviceNames.MK].DoCommand(stepGetMkRelays).Description;
                relays += DeviceHandler.Devices[DeviceNames.Simulator].DoCommand(stepGetSimulatorRelays).Description;
                ShowRelays(relays);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Как минимум одно устройство не подключено");
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Как минимум одно устройство не подключено");
            }
        }

        private void ShowRelays(string relays)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ShowRelays(relays); });
                return;
            }
            labelRelays.Text = relays;
        }
    }
}
