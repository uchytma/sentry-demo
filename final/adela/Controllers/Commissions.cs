using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Sentry;

namespace adela.Controllers;

class ComputeResponse
{
    public string Id { get; set; }
}

class CommitResponse
{
    public int Commited { get; set; }
}


[ApiController]
public class Commissions : Controller
{
    [HttpGet("api/commissions/process")]
    public async Task<IActionResult> CommissionsProcess(string? partnerName)
    {
        // add partnerName to the transaction
        SentrySdk.ConfigureScope(scope => scope.SetTag("partnerName", partnerName ?? "unknown"));
        
        // update transaction name
        var transactionName = $"adela.http.{nameof(Commissions)}.{nameof(CommissionsProcess)}";
        SentrySdk.ConfigureScope(scope => scope.Transaction.Name = transactionName);
        
        RestClient c = new RestClient("http://localhost:5227");
        var request = new RestRequest("api/commissions/compute", Method.Get);

        request.AddHeader("sentry-trace", SentrySdk.GetTraceHeader()?.ToString() ?? "");
        var response = await c.ExecuteAsync<ComputeResponse>(request);
        if (response.Data == null) throw new ApplicationException("Compute failed");
        
        var requestCommit = new RestRequest("api/commissions/commit", Method.Get);
        requestCommit.AddQueryParameter("id", response.Data.Id);
        requestCommit.AddHeader("sentry-trace", SentrySdk.GetTraceHeader()?.ToString() ?? "");
        var responseCommit = await c.ExecuteAsync<CommitResponse>(requestCommit);
        
        var curentTransaction = SentrySdk.GetSpan()?.GetTransaction();
        if (curentTransaction == null) throw new ApplicationException("Transaction not found");
        var emailProcessingSpan = curentTransaction.StartChild("email-processing", "email-processing");
        try
        {
            // 1s async wait - simulate sending email with results
            await Task.Delay(1000);
        }
        finally
        {
            emailProcessingSpan.Finish();
        }

        return Ok(responseCommit.Data);
    }
}