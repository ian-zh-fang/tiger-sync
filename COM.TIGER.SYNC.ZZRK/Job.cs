using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.TIGER.SYNC.ZZRK
{
    public class Job : TaskJob.Job<Model>
    {
        public override void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            Model.tick = 0;
            Console.WriteLine("暂住人员数据同步开始 ...");
            base.Executed(dbFrom, dbTarget, dataFromCmdString);
        }

        protected override void Executed(Model t, DAO.DataHandler dbTarget)
        {
            Console.WriteLine(t.ToString());

            //首先获取指定的人员信息
            if (!string.IsNullOrWhiteSpace(t.GMSFHM))
            {
                var pop = dbTarget.ExecuteScalar(t.GetPopIDCmd());
                if (pop != null)
                {
                    t.PopID = int.Parse(string.Format("{0}", pop));
                    //更改人员信息
                    dbTarget.ExecuteNonQuery(t.UpdateZZCmd(t.PopID));
                }
            }

            try
            {
                var obj = dbTarget.ExecuteScalar(t.ExistCmd());
                if (obj == null)
                {
                    //直接添加数据
                    dbTarget.ExecuteNonQuery(t.InsertCmd());
                    return;
                }

                dbTarget.ExecuteNonQuery(t.UpdateCmd(obj));
            }
            catch (Exception e) { Console.WriteLine("错误：{0}", e.Message); }
        }
    }
}
