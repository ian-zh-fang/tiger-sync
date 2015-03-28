using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEntLib = Microsoft.Practices.EnterpriseLibrary.Data;

namespace COM.TIGER.SYNC.DAO.SqlServer
{
    public class MSSqlServerHandler:DataHandler
    {
        private const string _connStringFormat = "Server={0};database={1};user id={2};password={3};";

        public MSSqlServerHandler(string ip, string dbName, string userid, string pwd)
        {
            dbConnectionString = string.Format(_connStringFormat, ip, dbName, userid, pwd);            
        }

        protected override DEntLib.Database CreateDB()
        {
            DEntLib.Database db = new DEntLib.Sql.SqlDatabase(dbConnectionString);
            return db;
        }
    }
}
