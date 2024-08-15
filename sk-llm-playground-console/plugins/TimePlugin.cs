
using System.ComponentModel;
using Microsoft.SemanticKernel;

public class TimePlugin
{
    [KernelFunction("GetCurrentTime")]
    [Description("Get the current time.")]
    public Task<string> GetCurrentTimeAsync()
    {
        Console.WriteLine("Executing time plugin");
        
        // Get the current time
        var currentTime = DateTime.Now;
        
        // Convert currentTime to ISO datetime string
        var isoDateTimeString = currentTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        return Task.FromResult(isoDateTimeString);
    }
}