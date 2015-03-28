using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM.TIGER.SYNC.SubTask
{
    public class SubTask
    {
        //程序集路径
        private readonly string _assemblyPath = null;
        //数据来源命令
        private readonly string _commandTextFrom = null;
        //程序集名称
        private readonly string _assemblyName = null;
        //目标类名称，需要包含命名空间
        private readonly string _className = null;

        public SubTask(string comTextFrom, string targetAssemblyName)
        {
            _commandTextFrom = comTextFrom;
            string[] arr = System.Configuration.ConfigurationManager.AppSettings[targetAssemblyName].Split(',');
            _assemblyName = string.Format("{0}.dll", arr[1]);
            _className = arr[0];

            _assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        public void Start(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget)
        {
            var job = LoadHandlerAssembly();
            if (job != null)
                job.Executed(dbFrom, dbTarget, _commandTextFrom);
        }

        /// <summary>
        /// 加载处理程序集
        /// </summary>
        /// <returns></returns>
        private TaskJob.IJob LoadHandlerAssembly()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(string.Format("{0}\\{1}", _assemblyPath, _assemblyName));
            TaskJob.IJob job = assembly.CreateInstance(_className, true) as TaskJob.IJob;
            return job;
        }
    }
}
