using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Checker.Settings;
using System.Collections.Specialized;
using System.Configuration;

namespace Checker
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new SettingsForm());
            var tableName = ConfigurationManager.AppSettings.Get("TableName");
            var settings = new ControlObjectSettings.Settings()
            {
                Comment = "Не указан",
                FactoryNumber = "Не указан",
                OperatorName = "Не указан",
                TableName = tableName
            };
            Application.Run(new Form1(settings));
        }
    }
}
