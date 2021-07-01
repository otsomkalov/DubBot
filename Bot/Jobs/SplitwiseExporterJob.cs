using System.Threading.Tasks;
using Quartz;

namespace Bot.Jobs
{
    [DisallowConcurrentExecution]
    public class SplitwiseExporterJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
