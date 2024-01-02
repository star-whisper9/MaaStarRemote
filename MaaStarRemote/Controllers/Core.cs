using MaaStarRemote.Data;
using MaaStarRemote.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace MaaStarRemote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Core : ControllerBase
    {
        private readonly ILogger<Core> log;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public Core(ILogger<Core> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            log = logger;
            _context = context;
            _configuration = configuration;
        }

        // Maa接口
        // 获取任务接口
        [HttpPost("getTasks")]
        public async Task<IActionResult> ReturnTasks([FromBody] Users user)
        {
            var userInDb = await _context.Users.FindAsync(user.user, user.device);
            if (userInDb == null)
            {
                log.LogDebug("MAA - 未找到用户");
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Where(t => t.user == user.user)
                .Select(t => new { id = t.uuid, type = t.task })
                .ToListAsync();

            log.LogDebug("MAA - 获取并发送了任务列表");
            return Ok(new { tasks = tasks.Select(t => new { t.id, t.type }) });
        }

        // 回报任务完成情况端口
        [HttpPost("reportStatus")]
        public async Task<IActionResult> ReceiveStatus([FromBody] JsonElement body)
        {
            var user = body.GetProperty("user").GetString();
            var device = body.GetProperty("device").GetString();

            var userInDb = await _context.Users.FindAsync(user, device);
            if (userInDb == null)
            {
                log.LogInformation("前端 - 未找到正确用户和设备");
                return StatusCode(403, "未找到正确的用户或设备");
            }

            var task = body.GetProperty("task").GetString();
            var status = body.GetProperty("status").GetString();
            var payload = body.GetProperty("payload").GetString();
            var info=_context.Tasks.Where(t => t.uuid == task).Select(t => t.task).FirstOrDefault().ToString();

            if (task == null || status == null)
            {
                log.LogError("前端 - 回报任务完成情况时获取到空任务项");
                return BadRequest();
            }
            else
            {
                var report = new Reports
                {
                    info = info,
                    user = user,
                    device = device,
                    task = task,
                    status = status,
                    payload = payload,
                    time = DateTime.Now
                };
                _context.Reports.Add(report);
                log.LogInformation("前端 - 回报了任务完成情况");
            }
            await _context.SaveChangesAsync();
            return Ok();

        }

        // 前端调用端口
        // 增加任务端口
        [HttpPost("addTasks")]
        public async Task<IActionResult> AddTasks([FromBody] JsonElement body)
        {
            var user = body.GetProperty("user").GetString();
            var device = body.GetProperty("device").GetString();
            var tasks = body.GetProperty("tasks").EnumerateArray()
                .Select(t => new
                {
                    order = t.GetProperty("order").GetInt32(),
                    task = t.GetProperty("task").GetString()
                })
                .OrderBy(t => t.order)
                .ToList();
            var validTasks = _configuration.GetSection("ValidTasks").Get<List<string>>();

            var userInDb = await _context.Users.FindAsync(user, device);
            if (userInDb == null)
            {
                log.LogInformation("前端 - 未找到正确用户和设备");
                return StatusCode(403, "未找到正确的用户或设备");
            }
            var userTasks = _context.Tasks.Where(t => t.user == user);
            _context.Tasks.RemoveRange(userTasks);
            await _context.SaveChangesAsync();

            foreach (var task in tasks)
            {
                if (task.task == null)
                {
                    log.LogError("前端 - 新增任务时获取到空任务项");
                    continue;
                }
                else if (!validTasks.Contains(task.task))
                {
                    log.LogError("前端 - 新增任务时获取到无效任务项");
                    continue;
                }

                var uuid = Guid.NewGuid().ToString();
                var newTask = new Models.Tasks
                {
                    interval = body.GetProperty("interval").GetInt32(),
                    time = DateTime.Now,
                    uuid = uuid,
                    user = user,
                    task = task.task
                };

                _context.Tasks.Add(newTask);
                log.LogInformation("前端 - 新增了任务项");
                await _context.SaveChangesAsync();
            }
            return Ok();
        }


        [HttpPost("getReport")]
        public async Task<IActionResult> GetReport([FromBody] JsonElement body)
        {

            var user = body.GetProperty("user").GetString();
            var device = body.GetProperty("device").GetString();
            var time = DateTime.Parse(body.GetProperty("time").GetString());

            var userInDb = await _context.Users.FindAsync(user, device);
            if (userInDb == null)
            {
                log.LogInformation("前端 - 未找到正确用户和设备");
                return StatusCode(403, "未找到正确的用户或设备");
            }

            var tasksInDb = _context.Reports.Where(t => t.user == user && t.time.Date == time.Date);
            if (!tasksInDb.Any())
            {
                log.LogInformation($"{time.Date}没有找到匹配的记录");
                return NotFound("没有找到匹配的记录");
            }

            List<object> reports = new List<object>();
            foreach (var task in tasksInDb)
            {
                if (task.info != "CaptureImage" && task.info != "CaptureImageNow")
                {
                    continue;
                }
                var report = new
                {
                    status = task.status,
                    payload = task.payload
                };
                reports.Add(report);
                log.LogInformation("前端 - 获取了截图");
            }
            return Ok(reports);

        }
    }
}
