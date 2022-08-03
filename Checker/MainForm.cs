using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Checker.DataBase;
using Checker.Device;
using Checker.Devices;
using Checker.Logging;
using Checker.Settings;
using Checker.Steps;

namespace Checker
{
    public partial class Form1 : Form
    {
        #region Глобальные переменные

        private static readonly Dictionary<TreeNode, Step> NodeStepDictionary = new ();

        private static readonly Dictionary<Step, TreeNode> StepNodeDictionary = new ();

        private static StepsInfo _stepsInfo;
        private static DeviceInit _deviceHandler;
        private ControlObjectSettings.Settings settings;

        private static Form1 _form;
        private static Log _log;

        private readonly Thread mainThread = new(Some);
        private static readonly Queue<Step> Queue = new();
        private static Form1 _eventSend;
        private static bool _checkingResult = true;
        private static bool _isCheckingStarted;
        private static bool _isCheckingInterrupted;
        private static bool _isStepByStepMode = false;

        private static void Some()
        {
            while (true)
            {
                DoNextStep();
            }
        }

        private static void DoNextStep()
        {
            Steps.Step step = null;
            lock (Queue)
            {
                if (Queue.Count != 0 && _isCheckingStarted)
                {
                    step = Queue.Dequeue();
                    Thread.Sleep(10);
                }
            }

            if (step != null)
            {
                if (step.ShowStep)
                {
                    var node = StepNodeDictionary[step];
                    _form.HighlightTreeNode(node, Color.Blue);
                }

                var stepResult = DoStep(step);
                if (step.ShowStep)
                {
                    ShowStepResult(step, stepResult);
                }
/*                if (!step.Command.ToString().StartsWith("Get") || stepResult.State != DeviceStatus.Error) return;
                for (var i = 0; i < 2; i++)
                {
                    stepResult = DoStep(step);
                    if (step.ShowStep)
                    {
                        ShowStepResult(step, stepResult);
                    }
                }*/
            }
            else if (_isCheckingStarted)
            {
                _isCheckingStarted = false;
                var result = _checkingResult ? "ОК исправен." : "ОК неисправен";
                result = _isCheckingInterrupted
                    ? "Проверка прервана, результаты проверки записаны в файл."
                    : $"Проверка завершена, результаты проверки записаны в файл. {result}";
                _log.Send(result);
                MessageBox.Show(result);
                _form.ChangeStartButtonState();
                _form.ChangeButton(_form.buttonCheckingPause, "Пауза");
                _form.CleanTreeView();
                _form.BlockControls(false);
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
            _form = this;
            InitializeComponent();
            _eventSend = this;
            treeOfChecking.AfterCheck += (node_AfterCheck);
            this.settings = settings;
            ShowSettings();
            InitialActions();
            Text = _stepsInfo.ProgramName;
            buttonCheckingPause.Enabled = false;
            mainThread.Start();
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion

        #region Показать режимы и настройки

        private void ShowSettings()
        {
            textBoxComment.Text = (settings.Comment != "") ? settings.Comment : "";
            textBoxFactoryNumber.Text = settings.FactoryNumber;
            textBoxOperatorName.Text = (settings.OperatorName != "") ? settings.OperatorName : "";
        }

        private void SetVoltageSupplyModes()
        {
            foreach (var modeName in _stepsInfo.VoltageSupplyModesDictionary.Keys)
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
            foreach (var modeName in _stepsInfo.ModesDictionary.Keys)
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
            FillTreeView(treeOfChecking, _stepsInfo.ModesDictionary[modeName]);
            if (modeName.ToLower().Contains("проверка"))
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
            FillTreeView(treeOfChecking, _stepsInfo.VoltageSupplyModesDictionary[modeName]);
        }

        #endregion

        #region Инициализация

        private void InitialActions(string pathToDataBase)
        {
            var connectionString =
                string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;",
                    pathToDataBase); //"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathToDataBase;
            var dbReader = new DbReader(connectionString);
            var dataSet = dbReader.GetDataSet();
            _stepsInfo = Step.GetStepsInfo(dataSet);
            SetVoltageSupplyModes();
            ShowCheckingModes();
            //ReplaceVoltageSupplyInStepsDictionary();
            try
            {
                _deviceHandler = new DeviceInit(_stepsInfo.DeviceList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitialActions()
        {
            var connectionString = settings.TableName; // "UPD.xlsx;";
            InitialActions(connectionString);
        }

        #endregion

        #region Логирование

        private void CreateLog()
        {
            _log = new Log(settings, _stepsInfo.ProtocolDirectory);
            _log.Send($"Время начала проверки: {DateTime.Now}\n");
            _log.Send($"Имя оператора: {settings.OperatorName}\n");
            _log.Send($"Комментарий: {settings.Comment}\n");
            _log.Send($"Заводской номер: {settings.FactoryNumber}\n");
            var voltageMode = GetModeName();
            _log.Send($"Режим проверки: {_regime}\n");
            _log.Send($"Напряжение питания: {voltageMode}\r\n");
        }

        private string GetModeName()
        {
            var modeName = comboBoxCheckingMode.SelectedItem.ToString();
            if (modeName.ToLower().Contains("проверка"))
            {
                modeName = comboBoxVoltageSupply.SelectedItem.ToString();
            }

            return modeName;
        }

        #endregion

        #region Проверка

        private static void EnQueueCheckingSteps(string modeName)
        {
            //var stepsDictionary = modeName.ToLower().Contains("проверка") ? _stepsInfo.VoltageSupplyModesDictionary[modeName] : _stepsInfo.ModesDictionary[modeName];
            var stepsDictionary = _stepsInfo.VoltageSupplyModesDictionary[modeName];
            foreach (var step in stepsDictionary.Keys.SelectMany(tableName => stepsDictionary[tableName]))
            {
                lock (Queue)
                {
                    Queue.Enqueue(step);
                }
            }
        }

        private static void ShowStepResult(Step step, DeviceResult deviceResult)
        {
            var node = StepNodeDictionary[step];
            _form.HighlightTreeNode(node, Color.Blue);
            var result =
                $"{step.Description}\r\n{deviceResult.Description}\r\n\r\n"; // $"Шаг {stepNumber}: {step.Description}\r\n{deviceResult.Description}\r\n\r\n";
            if (step.Command.ToString().Contains("Set") || step.Command.ToString().Contains("Get") || step.DeviceName == DeviceNames.None)
            {
                _log.Send(result);
                _log.Send(DateTime.Now.ToString(CultureInfo.InvariantCulture));
            }

            _form.AddSubTreeNode(node, deviceResult.Description);
            Color color;
            switch (deviceResult.State)
            {
                case DeviceStatus.Ok:
                    color = Color.Green;
                    break;
                case DeviceStatus.Error:
                case DeviceStatus.NotConnected:
                    color = Color.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _form.HighlightTreeNode(node, color);
        }

        private static void ShowErrorDialog(string description)
        {
            var dialogResult =
                MessageBox.Show(description, @"Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            switch (dialogResult)
            {
                case DialogResult.No:
                    _isCheckingInterrupted = true;
                    _isCheckingStarted = false;
                    MessageBox.Show(@"Проверка остановлена.");
                    _form.AbortChecking();
                    _form.ChangeStartButtonState();
                    break;
                case DialogResult.Yes:
                    _isCheckingStarted = true;
                    _form.ChangeControlState(_form.groupBoxManualStep, true);
                    //form.ChangeCheckingState(false);
                    break;
            }
        }

        private static DeviceResult DoStep(Steps.Step step)
        {
            var stepParser = new StepParser(_deviceHandler, step);
            var deviceResult = stepParser.DoStep();
            if (deviceResult.State == DeviceStatus.Error || deviceResult.State == DeviceStatus.NotConnected)
            {
                _checkingResult = false;
                if (!_form.checkBoxIgnoreErrors.Checked)
                {
                    _isCheckingStarted = false;
                    var description =
                        $"В ходе проверки произошла ошибка:\r\nШаг: {step.Description}\r\nРезультат шага: {deviceResult.Description}\r\nПродолжить проверку?";
                    ShowErrorDialog(description);
                }
            }

            if (deviceResult.State == DeviceStatus.NotConnected)
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

        private void button1_Click(object sender, EventArgs e)
        {
            var stepList = GetSelectedSteps();
            CreateLog();
            _log.Send("Выполнение выбранных оператором шагов проверки: \r\n");
            _isCheckingStarted = true;
            BlockControls(true);
            for (var i = 0; i < numericUpDown1.Value; i++)
                AddSelectedStepsToQueue(stepList);
        }

        private static List<Steps.Step> GetSelectedSteps()
        {
            return NodeStepDictionary.Keys
                .Where(node => node.Checked)
                .Select(node => NodeStepDictionary[node])
                .ToList();
        }

        private static void AddSelectedStepsToQueue(List<Step> stepList)
        {
            foreach (var step in stepList)
            {
                lock (Queue)
                {
                    Queue.Enqueue(step);
                }
            }
        }

        private static void node_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) return;
            if (e.Node.Nodes.Count > 0)
            {
                CheckChildNodes(e.Node, e.Node.Checked);
            }
        }

        private static void CheckChildNodes(TreeNode node, bool state)
        {
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = state;
            }
        }

        #endregion

        #region Управление потоком проверки

        private void AbortChecking()
        {
            _isCheckingStarted = false;
            lock (Queue)
            {
                Queue.Clear();
                foreach (var step in _stepsInfo.EmergencyStepList)
                {
                    Queue.Enqueue(step);
                }

                _isCheckingStarted = true;
            }

            IDeviceInterface.ClearCoefficientDictionary();
            IDeviceInterface.ClearValuesDictionary();
            Thread.Sleep(3000);
            CleanTreeView();
//BlockControls(false);
        }

        private void ChangeStartButtonState()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)ChangeStartButtonState);
                return;
            }

            //isCheckingStarted = !isCheckingStarted;
            var buttonText = _isCheckingStarted ? "Стоп" : "Старт";
            buttonCheckingStart.Text = buttonText;
        }

        private void ChangeButtonPauseResume()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)ChangeButtonPauseResume);
                return;
            }

            var buttonText = _isCheckingStarted ? "Пауза" : "Продолжить";
            buttonCheckingPause.Text = buttonText;
        }

        private void ChangeButton(Button button, string text)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ChangeButton(button, text); });
                return;
            }

            button.Text = text;
        }

        #endregion

        #region TreeNodes

        private static void FillTreeView(TreeView treeView, Dictionary<string, List<Steps.Step>> stepDictionary)
        {
            NodeStepDictionary.Clear();
            StepNodeDictionary.Clear();
            treeView.Nodes.Clear();
            treeView.BeginUpdate();
            var nodesCount = 0;
            foreach (var tableName in stepDictionary.Keys)
            {
                var treeNode = new TreeNode(tableName);
                treeView.Nodes.Add(treeNode);
                foreach (var step in stepDictionary[tableName].Where(step => step.ShowStep))
                {
                    nodesCount++;
                    var nodeName = step.Description;//$"{nodesCount} {step.Description}";
                    var stepNode = new TreeNode(nodeName);
                    NodeStepDictionary.Add(stepNode, step);
                    StepNodeDictionary.Add(step, stepNode);
                    treeNode.Nodes.Add(stepNode);
                    treeNode.Expand();
                }
            }

            treeView.EndUpdate();
        }

        private void HighlightTreeNode(TreeNode treeNode, Color color)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { HighlightTreeNode(treeNode, color); });
                return;
            }

            treeNode.EnsureVisible();
            treeNode.ForeColor = color;
        }

        private void AddSubTreeNode(TreeNode parentTreeNode, string stepResult)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { AddSubTreeNode(parentTreeNode, stepResult); });
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
            NodeStepDictionary.Clear();
            StepNodeDictionary.Clear();
        }

        private void CleanTreeView()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)CleanTreeView);
                return;
            }

            treeOfChecking.Nodes.Clear();
            SelectCheckingMode();
        }

        private void ChangeControlState(Control control, bool state)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ChangeControlState(control, state); });
                return;
            }

            control.Enabled = state;
        }

        #endregion

        #region Методы элементов управления

        private void buttonCheckingStart_Click(object sender, EventArgs e)
        {
            _checkingResult = true;
            _isCheckingInterrupted = false;
            CreateLog();
            var modeName = GetModeName();
            EnQueueCheckingSteps(modeName);
            _isCheckingStarted = true;
            BlockControls(_isCheckingStarted);
        }

        private void buttonCheckingStop_Click(object sender, EventArgs e)
        {
            IDeviceInterface.ClearCoefficientDictionary();
            IDeviceInterface.ClearValuesDictionary();
            ChangeControlState(buttonCheckingStop, false);
            _isCheckingStarted = false;
            _isCheckingInterrupted = true;
            AbortChecking();
        }

        private void buttonOpenDataBase_Click(object sender, EventArgs e)
        {
            //DoStepList(stepsInfo.EmergencyStepList);
            var openBinFileDialog = new OpenFileDialog();
            openBinFileDialog.Filter = @"Файлы *.xls* | *xls*";
            if (openBinFileDialog.ShowDialog() != DialogResult.OK) return;
            CleanAll();
            InitialActions(openBinFileDialog.FileName);
        }

        private void buttonCheckingPause_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void Pause()
        {
            _isCheckingStarted = !_isCheckingStarted;
            ChangeButtonPauseResume();
            ChangeControlState(buttonStep, !_isCheckingStarted);
        }

        private static string _regime;

        private void comboBoxCheckingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _regime = comboBoxCheckingMode.SelectedItem.ToString();
            SelectCheckingMode();
        }

        private void comboBoxVoltageSupply_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ReplaceVoltageSupplyInStepsDictionary();
            SetVoltageSupplyMode();
        }

        private void treeOfChecking_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // if (treeViewNodeStep.ContainsKey(e.Node) && isCheckingInterrupted)
            // {
            //     var thisStep = treeViewNodeStep[e.Node];
            //     var node = treeViewStepNode[thisStep];
            //     DoStep(thisStep);
            // }
        }

        #endregion

        #region Обработка закрытия формы

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Queue.Count != 0)
            {
                AbortChecking();
                Thread.Sleep(3000);
            }

            if (_isCheckingStarted)
            {
                _isCheckingInterrupted = true;
                const string result = "Проверка прервана, результаты проверки записаны в файл.";
                _log.Send(result);
            }

            Die();
            Application.Exit();
            mainThread.Abort();
        }

        private static void Die()
        {
            if (_deviceHandler == null)
                return;
            _deviceHandler.Devices
                .Values
                .Where(dev => dev != null)
                .ToList()
                .ForEach(device => device.Die());
        }

        #endregion

        private void buttonStep_Click(object sender, EventArgs e)
        {
            Step step = null;
            lock (Queue)
            {
                if (Queue.Count != 0)
                {
                    step = Queue.Dequeue();
                    Thread.Sleep(10);
                }
            }

            if (step == null) return;
            if (step.ShowStep)
            {
                var node = StepNodeDictionary[step];
                _form.HighlightTreeNode(node, Color.Blue);
            }

            var stepResult = DoStep(step);
            if (step.Argument == "")
            {
                MessageBox.Show($@"Шаг {step.Description}: Аргумент пустой: {step.Argument}");
            }

            if (step.ShowStep)
            {
                ShowStepResult(step, stepResult);
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
            _isCheckingInterrupted = true;
            AbortChecking();
        }

        private void buttonShowRelays_Click(object sender, EventArgs e)
        {
/*            var stepGetMkRelays = new Step(DeviceNames.MK, DeviceCommands.GetClosedRelayNames);
            var stepGetSimulatorRelays = new Step(DeviceNames.MK, DeviceCommands.GetClosedRelayNames);*/
            var stepPCI1761_1 = new Step(DeviceNames.PCI_1761_1, DeviceCommands.GetClosedRelayNames);
            var stepPCI1762_1 = new Step(DeviceNames.PCI_1762_1, DeviceCommands.GetClosedRelayNames);
            var stepPCI1762_2 = new Step(DeviceNames.PCI_1762_2, DeviceCommands.GetClosedRelayNames);
            var stepPCI1762_3 = new Step(DeviceNames.PCI_1762_3, DeviceCommands.GetClosedRelayNames);
            var stepPCI1762_5 = new Step(DeviceNames.PCI_1762_5, DeviceCommands.GetClosedRelayNames);

            var stepGetSimulatorRelays = new Step(DeviceNames.MK, DeviceCommands.GetClosedRelayNames);
            try
            {
                /*                var relays = _deviceHandler.Devices[DeviceNames.MK].DoCommand(stepGetMkRelays).Description;
                                relays += _deviceHandler.Devices[DeviceNames.Simulator].DoCommand(stepGetSimulatorRelays).Description;
                                ShowRelays(relays);*/
                var relays = _deviceHandler.Devices[DeviceNames.PCI_1761_1].DoCommand(stepPCI1761_1).Description + "\n";
                relays += _deviceHandler.Devices[DeviceNames.PCI_1762_1].DoCommand(stepPCI1762_1).Description + "\n";
                relays += _deviceHandler.Devices[DeviceNames.PCI_1762_2].DoCommand(stepPCI1762_2).Description + "\n";
                relays += _deviceHandler.Devices[DeviceNames.PCI_1762_3].DoCommand(stepPCI1762_3).Description + "\n";
                relays += _deviceHandler.Devices[DeviceNames.PCI_1762_5].DoCommand(stepPCI1762_5).Description + "\n";
                ShowRelays(relays);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show(@"Как минимум одно устройство не подключено");
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show(@"Как минимум одно устройство не подключено");
            }
        }

        private void ShowRelays(string relays)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ShowRelays(relays); });
                return;
            }
            labelRelays.Text = relays;
        }
    }
}