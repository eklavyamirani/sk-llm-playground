using System.ComponentModel;
using Microsoft.SemanticKernel;

public class EmailPlugin
{
    [KernelFunction("send_email")]
    [Description("Send an email to the specified recipient.")]
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // TODO: @eklavya need a prompt to ask for the inputs instead of making them up.
        Console.WriteLine("Executing email plugin");

        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Email body cannot be null or empty.", nameof(body));
        }

        // Send email
        Console.WriteLine("Email sent to: " + to);

        await Task.CompletedTask;
    }
}