using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM.TIGER.SYNC.AJJBXX
{
    public class Job:TaskJob.Job<AJJBXX>
    {
        public override void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            AJJBXX.tick = 0;
            Console.WriteLine("案事件数据同步开始 ...");
            base.Executed(dbFrom, dbTarget, dataFromCmdString);
        }

        protected override void Executed(AJJBXX t, DAO.DataHandler dbTarget)
        {
            //校验数据是否存在，
            //已经存在，那么更新数据
            //不存在，那么添加数据

            Console.WriteLine(t.ToString());

            var rst = 0;
            var obj = dbTarget.ExecuteScalar(t.ExistCmd());
            if (obj == null)
            {
                rst = dbTarget.ExecuteNonQuery(t.InsertCmd());
                return;
            }

            rst = dbTarget.ExecuteNonQuery(t.UpdateCmd(obj));
        }
    }
}
