
namespace Checker
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonCheckingStart = new System.Windows.Forms.Button();
            this.buttonCheckingPause = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxOperatorName = new System.Windows.Forms.TextBox();
            this.textBoxFactoryNumber = new System.Windows.Forms.TextBox();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxPreferences = new System.Windows.Forms.GroupBox();
            this.buttonPowerOn = new System.Windows.Forms.Button();
            this.labelAttention = new System.Windows.Forms.Label();
            this.comboBoxCheckingMode = new System.Windows.Forms.ComboBox();
            this.comboBoxVoltageSupply = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonShowRelays = new System.Windows.Forms.Button();
            this.checkBoxIgnoreErrors = new System.Windows.Forms.CheckBox();
            this.labelRelays = new System.Windows.Forms.Label();
            this.groupBoxManualStep = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.treeOfChecking = new System.Windows.Forms.TreeView();
            this.groupBoxCheckingManagement = new System.Windows.Forms.GroupBox();
            this.buttonStep = new System.Windows.Forms.Button();
            this.buttonCheckingStop = new System.Windows.Forms.Button();
            this.buttonOpenDataBase = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxPreferences.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxManualStep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBoxCheckingManagement.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCheckingStart
            // 
            resources.ApplyResources(this.buttonCheckingStart, "buttonCheckingStart");
            this.buttonCheckingStart.Name = "buttonCheckingStart";
            this.buttonCheckingStart.UseVisualStyleBackColor = true;
            this.buttonCheckingStart.Click += new System.EventHandler(this.buttonCheckingStart_Click);
            // 
            // buttonCheckingPause
            // 
            resources.ApplyResources(this.buttonCheckingPause, "buttonCheckingPause");
            this.buttonCheckingPause.Name = "buttonCheckingPause";
            this.buttonCheckingPause.UseVisualStyleBackColor = true;
            this.buttonCheckingPause.Click += new System.EventHandler(this.buttonCheckingPause_Click);
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBoxPreferences);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBoxCheckingManagement);
            this.panel1.Name = "panel1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.textBoxOperatorName);
            this.groupBox1.Controls.Add(this.textBoxFactoryNumber);
            this.groupBox1.Controls.Add(this.textBoxComment);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxOperatorName
            // 
            resources.ApplyResources(this.textBoxOperatorName, "textBoxOperatorName");
            this.textBoxOperatorName.Name = "textBoxOperatorName";
            this.textBoxOperatorName.TextChanged += new System.EventHandler(this.textBoxOperatorName_TextChanged);
            // 
            // textBoxFactoryNumber
            // 
            resources.ApplyResources(this.textBoxFactoryNumber, "textBoxFactoryNumber");
            this.textBoxFactoryNumber.Name = "textBoxFactoryNumber";
            this.textBoxFactoryNumber.TextChanged += new System.EventHandler(this.textBoxFactoryNumber_TextChanged);
            // 
            // textBoxComment
            // 
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.TextChanged += new System.EventHandler(this.textBoxComment_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBoxPreferences
            // 
            this.groupBoxPreferences.Controls.Add(this.buttonPowerOn);
            this.groupBoxPreferences.Controls.Add(this.labelAttention);
            this.groupBoxPreferences.Controls.Add(this.comboBoxCheckingMode);
            this.groupBoxPreferences.Controls.Add(this.comboBoxVoltageSupply);
            this.groupBoxPreferences.Controls.Add(this.label6);
            this.groupBoxPreferences.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBoxPreferences, "groupBoxPreferences");
            this.groupBoxPreferences.Name = "groupBoxPreferences";
            this.groupBoxPreferences.TabStop = false;
            // 
            // buttonPowerOn
            // 
            resources.ApplyResources(this.buttonPowerOn, "buttonPowerOn");
            this.buttonPowerOn.Name = "buttonPowerOn";
            this.buttonPowerOn.UseVisualStyleBackColor = true;
            this.buttonPowerOn.Click += new System.EventHandler(this.button3_Click);
            // 
            // labelAttention
            // 
            resources.ApplyResources(this.labelAttention, "labelAttention");
            this.labelAttention.Name = "labelAttention";
            // 
            // comboBoxCheckingMode
            // 
            this.comboBoxCheckingMode.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCheckingMode, "comboBoxCheckingMode");
            this.comboBoxCheckingMode.Name = "comboBoxCheckingMode";
            this.comboBoxCheckingMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxCheckingMode_SelectedIndexChanged);
            // 
            // comboBoxVoltageSupply
            // 
            this.comboBoxVoltageSupply.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxVoltageSupply, "comboBoxVoltageSupply");
            this.comboBoxVoltageSupply.Name = "comboBoxVoltageSupply";
            this.comboBoxVoltageSupply.SelectedIndexChanged += new System.EventHandler(this.comboBoxVoltageSupply_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.buttonShowRelays);
            this.groupBox2.Controls.Add(this.checkBoxIgnoreErrors);
            this.groupBox2.Controls.Add(this.labelRelays);
            this.groupBox2.Controls.Add(this.groupBoxManualStep);
            this.groupBox2.Controls.Add(this.treeOfChecking);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonShowRelays
            // 
            resources.ApplyResources(this.buttonShowRelays, "buttonShowRelays");
            this.buttonShowRelays.Name = "buttonShowRelays";
            this.buttonShowRelays.UseVisualStyleBackColor = true;
            this.buttonShowRelays.Click += new System.EventHandler(this.buttonShowRelays_Click);
            // 
            // checkBoxIgnoreErrors
            // 
            resources.ApplyResources(this.checkBoxIgnoreErrors, "checkBoxIgnoreErrors");
            this.checkBoxIgnoreErrors.Name = "checkBoxIgnoreErrors";
            this.checkBoxIgnoreErrors.Tag = "Показывает неинформативные шаги проверки";
            this.checkBoxIgnoreErrors.UseVisualStyleBackColor = true;
            // 
            // labelRelays
            // 
            resources.ApplyResources(this.labelRelays, "labelRelays");
            this.labelRelays.Name = "labelRelays";
            // 
            // groupBoxManualStep
            // 
            resources.ApplyResources(this.groupBoxManualStep, "groupBoxManualStep");
            this.groupBoxManualStep.Controls.Add(this.numericUpDown1);
            this.groupBoxManualStep.Controls.Add(this.button1);
            this.groupBoxManualStep.Name = "groupBoxManualStep";
            this.groupBoxManualStep.TabStop = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeOfChecking
            // 
            this.treeOfChecking.AllowDrop = true;
            resources.ApplyResources(this.treeOfChecking, "treeOfChecking");
            this.treeOfChecking.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeOfChecking.CheckBoxes = true;
            this.treeOfChecking.Name = "treeOfChecking";
            this.treeOfChecking.ShowLines = false;
            this.treeOfChecking.ShowPlusMinus = false;
            this.treeOfChecking.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeOfChecking_AfterSelect);
            // 
            // groupBoxCheckingManagement
            // 
            this.groupBoxCheckingManagement.Controls.Add(this.buttonStep);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingStop);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonOpenDataBase);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingStart);
            this.groupBoxCheckingManagement.Controls.Add(this.buttonCheckingPause);
            resources.ApplyResources(this.groupBoxCheckingManagement, "groupBoxCheckingManagement");
            this.groupBoxCheckingManagement.Name = "groupBoxCheckingManagement";
            this.groupBoxCheckingManagement.TabStop = false;
            // 
            // buttonStep
            // 
            resources.ApplyResources(this.buttonStep, "buttonStep");
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // buttonCheckingStop
            // 
            resources.ApplyResources(this.buttonCheckingStop, "buttonCheckingStop");
            this.buttonCheckingStop.Name = "buttonCheckingStop";
            this.buttonCheckingStop.UseVisualStyleBackColor = true;
            this.buttonCheckingStop.Click += new System.EventHandler(this.buttonCheckingStop_Click);
            // 
            // buttonOpenDataBase
            // 
            resources.ApplyResources(this.buttonOpenDataBase, "buttonOpenDataBase");
            this.buttonOpenDataBase.Name = "buttonOpenDataBase";
            this.buttonOpenDataBase.UseVisualStyleBackColor = true;
            this.buttonOpenDataBase.Click += new System.EventHandler(this.buttonOpenDataBase_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxPreferences.ResumeLayout(false);
            this.groupBoxPreferences.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxManualStep.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBoxCheckingManagement.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.NumericUpDown numericUpDown1;

        #endregion
        private System.Windows.Forms.Button buttonCheckingStart;
        private System.Windows.Forms.Button buttonCheckingPause;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxCheckingManagement;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView treeOfChecking;
        private System.Windows.Forms.Button buttonOpenDataBase;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxVoltageSupply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCheckingMode;
        private System.Windows.Forms.GroupBox groupBoxPreferences;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelAttention;
        private System.Windows.Forms.GroupBox groupBoxManualStep;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxIgnoreErrors;
        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.TextBox textBoxOperatorName;
        private System.Windows.Forms.TextBox textBoxFactoryNumber;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button buttonCheckingStop;
        private System.Windows.Forms.Button buttonPowerOn;
        private System.Windows.Forms.Button buttonShowRelays;
        private System.Windows.Forms.Label labelRelays;
    }
}

