using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class EnrollmentWorker(IServiceScopeFactory scopeFactory)
{
    public async Task processBatch()
    {
        using var scope = scopeFactory.CreateScope();
        {
            var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
        }
    }
}