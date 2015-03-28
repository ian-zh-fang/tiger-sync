using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.TIGER.SYNC.WGRSQ
{
    public class Job : TaskJob.Job<Model>
    {
        public override void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            Model.tick = 0;
            Console.WriteLine("入境人员数据同步开始 ...");
            base.Executed(dbFrom, dbTarget, dataFromCmdString);
        }

        protected override void Executed(Model t, DAO.DataHandler dbTarget)
        {
            Console.WriteLine(t.ToString());
            try
            {
                //校验国籍
                t.CountryID = CheckParam(dbTarget, t.GJDQ, null, 22);

                //校验人员ID
                t.PoID = GetPopulationID(t.ZJHM, dbTarget);
                dbTarget.ExecuteNonQuery(t.UpdateRJCmd(t.PoID));

                var obj = dbTarget.ExecuteScalar(t.ExistCmd());
                if (obj == null)
                {
                    dbTarget.ExecuteNonQuery(t.InsertCmd());
                    return;
                }

                dbTarget.ExecuteNonQuery(t.updateCmd(obj));
            }
            catch (Exception e) { Console.WriteLine("错误：{0}", e.Message); }
        }   
     
        
    }
}
