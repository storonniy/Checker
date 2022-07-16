using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Checker.Settings;
using static Checker.Settings.ControlObjectSettings;


namespace Checker
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void buttonApplySettings_Click(object sender, EventArgs e)
        {
            if (CheckFactoryNumberField())
            {
                var settings = new ControlObjectSettings.Settings()
                {
                    Comment = textBoxComment.Text,
                    FactoryNumber = textBoxFactoryNumber.Text,
                    OperatorName = textBoxOperatorName.Text
                };
                var mainForm = new Form1(settings);
                this.Visible = false;
                mainForm.Show();
            }
        }


        private void textBoxFactoryNumber_TextChanged(object sender, EventArgs e)
        {
            CheckFactoryNumberField();
        }

        private bool CheckFactoryNumberField()
        {
            var factoryNumber = textBoxFactoryNumber.Text;
            labelErrorFactoryNumber.ForeColor = Color.Red;
            labelErrorFactoryNumber.Text = "Заводской номер должен быть числом";
            labelErrorFactoryNumber.Location = new Point()
            {
                X = textBoxFactoryNumber.Location.X,
                Y = textBoxFactoryNumber.Location.Y + 25
            };
            labelErrorFactoryNumber.Visible = false;
            try
            {
                int.Parse(factoryNumber);
            }
            catch (FormatException)
            {
                labelErrorFactoryNumber.Visible = true;
                return false;
            }
            return true;
        }
    }
}
