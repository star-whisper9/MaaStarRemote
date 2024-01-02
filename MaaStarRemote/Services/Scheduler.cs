using MaaStarRemote.Data;
using Microsoft.EntityFrameworkCore;

namespace MaaStarRemote.Services
{
    public class Scheduler : IHostedService
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Scheduler> log;
        private readonly IConfiguration _configuration;

        public Scheduler(IServiceScopeFactory scopeFactory, ILogger<Scheduler> logger, IConfiguration configuration)
        {
            log = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _timer = new Timer(UpdateTasks, null, 0, _configuration.GetSection("UpdateInterval").Get<Int32>() * 1000);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private async void UpdateTasks(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                log.LogDebug("Scheduler - 开始更新任务");
                var tasks = _context.Tasks.AsNoTracking();

                foreach (var task in tasks)
                {
                    if (task.interval == 0)
                    {
                        continue;
                    }
                    else if (task.time.AddSeconds(task.interval) < DateTime.Now)
                    {
                        task.uuid = Guid.NewGuid().ToString();
                        task.time = DateTime.Now;
                        _context.Tasks.Update(task);
                        log.LogDebug($"Scheduler - 更新了任务 {task.uuid}");
                    }
                }

                await _context.SaveChangesAsync();

            }
        }
    }
}
