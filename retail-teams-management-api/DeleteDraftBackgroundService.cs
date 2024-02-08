using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Infrastructure;

namespace retail_teams_management_api
{
    public class DeleteDraftBackgroundService : IHostedService, IDisposable
    {
        private  Timer? _timer = null;
        private readonly ILogger<DeleteDraftBackgroundService> _logger;
        public IServiceProvider Services { get; }
        public DeleteDraftBackgroundService( ILogger<DeleteDraftBackgroundService> logger, IServiceProvider services)
        {
         
            _logger = logger;
            Services = services;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
           
            _timer = new Timer(DeleteDraft, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(30));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void DeleteDraft(object? state)
        {

            using (var scope = Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<BranchDataContext>();
                // now do your work
              
                List<BranchRequest> toDelete = new();
                var records = _db.BranchRequests.Where(c => c.Status == AppContants.DRAFT_REQUEST_STATUS.ToString() && c.Created_At <= DateTime.Now.AddHours(-2).ToUniversalTime()).ToList();
                foreach (var record in records)
                {
                    record.Deleted = true;
                    record.Deleted_At = DateTime.UtcNow;
                    record.Deleted_By_Id = Guid.Empty.ToString();
                    toDelete.Add(record);
                }
                _db.ChangeTracker.Clear();

                _db.BranchRequests.UpdateRange(toDelete);
                _db.SaveChanges();
            }


           
            

        }
    }
}
