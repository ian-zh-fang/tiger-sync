using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEntLib = Microsoft.Practices.EnterpriseLibrary.Data;

namespace COM.TIGER.SYNC.DAO.Oracle
{
    public class OracleHandler:DataHandler
    {
        private const string _connStrFormat = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}1521))(CONNECT_DATA=(SID={2})));User ID={3};Password={4}";

        private const string _connFormat = "Data Source={0};User Id={1};Password={2}";

        public OracleHandler(string ip, string dbName, string userid, string pwd, int port = 1521)
        {
            dbConnectionString = string.Format(_connStrFormat, ip, port, dbName, userid, pwd);
        }

        public OracleHandler(string dataSource, string userid, string pwd)
        {
            dbConnectionString = string.Format(_connFormat, dataSource, userid, pwd);
        }

        protected override DEntLib.Database CreateDB()
        {
            DEntLib.Database db = new DEntLib.Oracle.OracleDatabase(dbConnectionString);
            return db;
        }
    }
}
