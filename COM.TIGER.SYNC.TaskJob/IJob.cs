using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM.TIGER.SYNC.TaskJob
{
    public interface IJob
    {
        void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString);
    }
}
