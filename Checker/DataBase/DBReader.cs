using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace Checker.DataBase
{
    public class DbReader
    {
        private readonly OleDbConnection connection;
        private DbReader (OleDbConnection connection)
        {
            this.connection = connection;
            this.connection.Open();
        }

        public DbReader(string connectionString) : this(new OleDbConnection(connectionString))
        {

        }

        private List<string> GetTableNamesList()
        {
            var dataTable = connection.GetSchema("Tables");
            return (from DataRow row in dataTable.Rows select (string) row[2] into tableName where !tableName.Contains("MSys") && !tableName.Contains("~") select tableName).ToList();
        }

        public DataSet GetDataSet()
        {
            var dataSet = new DataSet();
            foreach (var tableName in GetTableNamesList())
            {
                var selectCommandText = $"SELECT * FROM [{tableName}]";
                var dataAdapter = new OleDbDataAdapter(selectCommandText, connection);
                dataAdapter.Fill(dataSet, tableName.Replace("$", ""));
            }
            return dataSet;
        }

        ~DbReader() // деструктор
        {
            //this.connection.Close();
        }
    }
}
