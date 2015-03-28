using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.TIGER.SYNC.JCJ_JJDB
{
    /// <summary>
    /// 报警信息只做增量处理
    /// </summary>
    public class Job : TaskJob.Job<Model>
    {
        public override void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            Model.tick = 0;
            Console.WriteLine("三台合一平台数据同步开始 ...");
            var maxnum = GetMax(dbTarget);

            if(!string.IsNullOrWhiteSpace(maxnum))
            {
                if(dataFromCmdString.Contains(" where "))
                    dataFromCmdString = string.Format("{0} and JJDBH > {1}", dataFromCmdString, maxnum);
                else
                    dataFromCmdString = string.Format("{0} where JJDBH > {1}", dataFromCmdString, maxnum);
            }

            base.Executed(dbFrom, dbTarget, dataFromCmdString);
        }

        protected override void Executed(Model t, DAO.DataHandler dbTarget)
        {
            try
            {
                Console.WriteLine(t.ToString());

                string adminname = null;
                t.AdminID = CheckAdministrative(dbTarget, out adminname, t.GXDWDM);

                string typename = null;
                t.TypeID = CheckParam(dbTarget, t.BJFSDM, out typename);
                t.TypeName = typename;

                dbTarget.ExecuteNonQuery(t.InsertCmd());
            }
            catch (Exception e) { Console.WriteLine("错误：{0}", e.Message); }
        }

        private string GetMax(DAO.DataHandler db)
        {
            return string.Format("{0}", db.ExecuteScalar(GetMaxCmd()));
        }

        public static string GetMaxCmd()
        {
            return string.Format("select max(Num) from {0}", "Pgis_JCJ_JJDB");
        }

        private int CheckParam(DAO.DataHandler db, string code, out string name)
        {
            switch (code)
            {
                case "01":
                    name = "110 报警";
                    break;
                case "02":
                    name = "122 报警";
                    break;
                case "03":
                    name = "119 报警";
                    break;
                default:
                    name = "其它";
                    break;
            }

            return CheckParam(db, name, GetCode(), 5);
        }
    }
}
