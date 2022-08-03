
namespace Checker
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.textBoxOperatorName = new System.Windows.Forms.TextBox();
            this.textBoxFactoryNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonApplySettings = new System.Windows.Forms.Button();
            this.labelErrorFactoryNumber = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxComment
            // 
            this.textBoxComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxComment.Location = new System.Drawing.Point(187, 34);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(270, 23);
            this.textBoxComment.TabIndex = 22;
            this.textBoxComment.Text = "";
            // 
            // textBoxOperatorName
            // 
            this.textBoxOperatorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxOperatorName.Location = new System.Drawing.Point(187, 124);
            this.textBoxOperatorName.Name = "textBoxOperatorName";
            this.textBoxOperatorName.Size = new System.Drawing.Size(270, 23);
            this.textBoxOperatorName.TabIndex = 20;
            this.textBoxOperatorName.Text = "";
            // 
            // textBoxFactoryNumber
            // 
            this.textBoxFactoryNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxFactoryNumber.Location = new System.Drawing.Point(187, 78);
            this.textBoxFactoryNumber.Name = "textBoxFactoryNumber";
            this.textBoxFactoryNumber.Size = new System.Drawing.Size(270, 23);
            this.textBoxFactoryNumber.TabIndex = 19;
            this.textBoxFactoryNumber.Text = "0";
            this.textBoxFactoryNumber.TextChanged += new System.EventHandler(this.textBoxFactoryNumber_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.Location = new System.Drawing.Point(16, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Оператор";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(16, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "Заводской номер";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label2.Location = new System.Drawing.Point(16, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 16;
            this.label2.Text = "Коментарий";
            // 
            // buttonApplySettings
            // 
            this.buttonApplySettings.Location = new System.Drawing.Point(174, 185);
            this.buttonApplySettings.Name = "buttonApplySettings";
            this.buttonApplySettings.Size = new System.Drawing.Size(107, 28);
            this.buttonApplySettings.TabIndex = 15;
            this.buttonApplySettings.Text = "Запуск проверки";
            this.buttonApplySettings.UseVisualStyleBackColor = true;
            this.buttonApplySettings.Click += new System.EventHandler(this.buttonApplySettings_Click);
            // 
            // labelErrorFactoryNumber
            // 
            this.labelErrorFactoryNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelErrorFactoryNumber.AutoSize = true;
            this.labelErrorFactoryNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelErrorFactoryNumber.Location = new System.Drawing.Point(184, 104);
            this.labelErrorFactoryNumber.Name = "labelErrorFactoryNumber";
            this.labelErrorFactoryNumber.Size = new System.Drawing.Size(0, 17);
            this.labelErrorFactoryNumber.TabIndex = 25;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 225);
            this.Controls.Add(this.labelErrorFactoryNumber);
            this.Controls.Add(this.textBoxComment);
            this.Controls.Add(this.textBoxOperatorName);
            this.Controls.Add(this.textBoxFactoryNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonApplySettings);
            this.MaximumSize = new System.Drawing.Size(506, 263);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(506, 263);
            this.Name = "SettingsForm";
            this.Text = "Программа проверки прибора C-33162 УСА ШЮГИ.468151.110";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.TextBox textBoxOperatorName;
        private System.Windows.Forms.TextBox textBoxFactoryNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonApplySettings;
        private System.Windows.Forms.Label labelErrorFactoryNumber;
        //private USA.DataBaseDataSetTableAdapters.AlgorithmTableAdapter algorithmTableAdapter1;
    }
}