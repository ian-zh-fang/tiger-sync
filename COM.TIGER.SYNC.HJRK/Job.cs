using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.TIGER.SYNC.HJRK
{
    public class Job : TaskJob.Job<Model>
    {
        public override void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            Model.tick = 0;
            Console.WriteLine("人员基本信息数据同步开始 ...");
            base.Executed(dbFrom, dbTarget, dataFromCmdString);
        }

        protected override void Executed(Model t, DAO.DataHandler dbTarget)
        {
            Console.WriteLine(t.ToString());
            try
            {
                string sexname = null;
                t.SexID = GetSex(t.XB, dbTarget, out sexname);
                t.Sex = sexname;
                t.AddrID = CheckAddress(t.ZZ, dbTarget);
                t.HRelationID = CheckParam(dbTarget, t.YHZGX, GetCode(), 30);

                var obj = dbTarget.ExecuteScalar(t.ExistCmd());
                if (obj == null)
                {
                    dbTarget.ExecuteNonQuery(t.InsertCmd());
                    return;
                }
                dbTarget.ExecuteNonQuery(t.UpdateCmd(obj));
            }
            catch (Exception e) { Console.WriteLine("错误：{0}", e.Message); }
        }
    }
}
