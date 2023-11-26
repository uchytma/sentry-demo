using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace service_commissions.Controllers;

[ApiController]
public class Commissions : Controller
{
    [HttpGet("api/commissions/compute")]
    public async Task<IActionResult> Compute()
    {
        // update transaction name
        var transactionName = $"service-commissions.http.{nameof(Commissions)}.{nameof(Compute)}";
        SentrySdk.ConfigureScope(scope => scope.Transaction.Name = transactionName);
        
        // 5s async wait
        await Task.Delay(1000);
        var result = new { Id = Guid.NewGuid() };
        return Ok(result);
    }
    
    [HttpGet("api/commissions/commit")]
    public async Task<IActionResult> Commit(string id)
    {
        // update transaction name
        var transactionName = $"service-commissions.http.{nameof(Commissions)}.{nameof(Commit)}";
        SentrySdk.ConfigureScope(scope => scope.Transaction.Name = transactionName);
        
        // 5s async wait
        await Task.Delay(1000);
        var result = new { Commited = 30 };
        return Ok(result);
    }
}