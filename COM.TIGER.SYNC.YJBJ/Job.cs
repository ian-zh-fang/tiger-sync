using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM.TIGER.SYNC.YJBJ
{
    public class Job:TaskJob.Job<YJBJ>
    {
        public override void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            YJBJ.tick = 0;
            Console.WriteLine("学校，金融一件报警数据同步开始 ...");
            var maxunm = Getmax(dbTarget);

            if (!string.IsNullOrWhiteSpace(maxunm))
            {
                if (dataFromCmdString.Contains(" where "))
                    dataFromCmdString = string.Format("{0} and AlarmID > {1}", dataFromCmdString, maxunm);
                else
                    dataFromCmdString = string.Format("{0} where AlarmID > {1}", dataFromCmdString, maxunm);
            }

            base.Executed(dbFrom, dbTarget, dataFromCmdString);
        }

        protected override void Executed(YJBJ t, DAO.DataHandler dbTarget)
        {
            try
            {
                Console.WriteLine(t.ToString());
                //首先校验
                CheckParm(t, dbTarget);

                var ret = dbTarget.ExecuteNonQuery(t.InsertCmd());
            }
            catch (Exception e) { Console.WriteLine("错误：{0}", e.Message); }
        }

        private void CheckParm(YJBJ t, DAO.DataHandler db)
        {
            var id = db.ExecuteScalar(t.GetParam());
            if (id == null)
            {
                db.ExecuteScalar(t.InsertParam());
            }
            id = db.ExecuteScalar(t.GetParam());

            if (id != null)
                t.TypeID = int.Parse(string.Format("{0}", id));
        }
        
        public string Getmax(DAO.DataHandler db)
        {
            var cmdtext = "select max(Num) from Pgis_YJBJ";
            return string.Format("{0}", db.ExecuteScalar(cmdtext));
        }
    }
}
