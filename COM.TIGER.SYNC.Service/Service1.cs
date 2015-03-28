using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace COM.TIGER.SYNC.Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

            //在此初始化常量信息            
        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnStop()
        {

        }

        TaskHandler.TaskHandler[] GetTasks()
        {
            var dbTask = System.Configuration.ConfigurationManager.AppSettings["dbTask"];
            if (string.IsNullOrWhiteSpace(dbTask))
                return new TaskHandler.TaskHandler[] { };

            var items = dbTask.Split(',');
            TaskHandler.TaskHandler[] tasks = new TaskHandler.TaskHandler[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                var tfg = items[i];
                tfg = System.Configuration.ConfigurationManager.AppSettings[tfg];
                tasks[i] = GetTask(tfg);
            }
            return tasks;
        }

        TaskHandler.TaskHandler GetTask(string config)
        {
            var items = config.Split(new char[] { ':', ';' });
            var connStr = items[1];
            var tables = items[3];
            var interval = items[5];

            var dbFrom = GetDB(connStr);
            var dbTarget = GetDB(System.Configuration.ConfigurationManager.AppSettings["dbTarget"]);

            TaskHandler.TaskHandler task = new TaskHandler.TaskHandler(dbFrom, dbTarget, GetInterval(interval));
            var subtasks = GetTasks(tables);
            task.AddRange(subtasks);

            return task;
        }

        DAO.DataHandler GetDB(string config)
        {
            DAO.DataHandler db = null;
            var items = config.Split(',');
            switch (items[0].ToLower())
            {
                case "sqlserver":
                    db = new DAO.SqlServer.MSSqlServerHandler(items[1], items[2], items[3], items[4]);
                    break;
                case "oracle":
                    db = new DAO.Oracle.OracleHandler(items[1], items[3], items[4], items[5], int.Parse(items[2]));
                    break;
                default:
                    db = new DAO.SqlServer.MSSqlServerHandler(items[1], items[2], items[3], items[4]);
                    break;
            }
            return db;
        }

        int GetInterval(string config)
        {
            var items = config.Split(',');
            var interval = int.Parse(items[0]);
            var unit = 1;
            switch (items[1].ToLower())
            {
                case "s":
                    unit = 1;
                    break;
                case "min":
                    unit = 60;
                    break;
                case "h":
                    unit = 60 * 60;
                    break;
                case "d":
                    unit = 60 * 60 * 24;
                    break;
                case "mon":
                    unit = 60 * 60 * 24 * 30;
                    break;
                case "y":
                    unit = 60 * 60 * 24 * 30 * 12;
                    break;
                default:
                    unit = 1;
                    break;
            }
            return interval * unit;
        }

        SubTask.SubTask[] GetTasks(string tbnames)
        {
            var items = tbnames.Split(',');
            SubTask.SubTask[] tasks = new SubTask.SubTask[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                var cfg = items[i];
                cfg = System.Configuration.ConfigurationManager.AppSettings[cfg];
                tasks[i] = GetSubTask(cfg);
            }
            return tasks;
        }

        SubTask.SubTask GetSubTask(string config)
        {
            var items = config.Split(',');
            var tbFrom = string.Format("select * from {0}", items[0]);
            return new SubTask.SubTask(tbFrom, items[1]);
        }

        private void contextMenuStrip1_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(e.ClickedItem.Name);
        }
    }
}
