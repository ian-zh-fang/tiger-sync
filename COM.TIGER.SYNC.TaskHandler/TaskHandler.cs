using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace COM.TIGER.SYNC.TaskHandler
{
    public class TaskHandler
    {
        //任务执行回调函数
        public static Action Action;
        //数据来源数据库
        private readonly DAO.DataHandler _dbFrom = null;
        //数据去往数据库
        private readonly DAO.DataHandler _dbTarget = null;
        //每次执行间隔，单位是秒
        private readonly int _interval = 30;

        private List<SubTask.SubTask> _tasks = null;
        //数据处理句柄，控制任务执行
        private Quartz.IScheduler _jobScheduler = null;

        public TaskHandler(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, int interval)
        {
            _dbFrom = dbFrom;
            _dbTarget = dbTarget;
            _interval = interval;

            _tasks = new List<SubTask.SubTask>();
            Action += new Action(Execute);
        }

        public void Execute()
        {
            for (var i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                task.Start(_dbFrom, _dbTarget);
            }
        }

        public void Start()
        {
            if (_jobScheduler == null)
            {
                Quartz.Impl.StdSchedulerFactory factory = new Quartz.Impl.StdSchedulerFactory();
                _jobScheduler = factory.GetScheduler();
            }

            var job = Quartz.JobBuilder.Create<TaskJob>()
                .WithIdentity(Guid.NewGuid().ToString())
                .Build();

            var trigger = Quartz.TriggerBuilder.Create()
                .WithIdentity(Guid.NewGuid().ToString())
                .WithSimpleSchedule(t => t.WithIntervalInSeconds(_interval).RepeatForever())
                .StartNow()
                .Build();

            _jobScheduler.ScheduleJob(job, trigger);
            _jobScheduler.Start();
        }

        public void Stop()
        {
            if (_jobScheduler != null && _jobScheduler.IsStarted)
            {
                _jobScheduler.Shutdown();
                _jobScheduler.Clear();
            }
        }

        public void Add(SubTask.SubTask task)
        {
            _tasks.Add(task);
        }

        public void AddRange(IEnumerable<SubTask.SubTask> items)
        {
            _tasks.AddRange(items);
        }
    }

    public class TaskJob : Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            TaskHandler.Action();
        }
    }
}
