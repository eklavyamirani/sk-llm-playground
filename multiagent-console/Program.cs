// Import packages
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

// FIXME: @eklavya move it out into a configuration
var modelId ="gpt-4o-mini-2024-07-18";

// write code to get the API key from dotnet user secrets
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["OpenAI:apiKey"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new ArgumentException("Please set the OpenAI API key using <<OpenAI:apiKey>> in the user secrets.");
}

// Create a kernel with OpenAI chat completion
var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId: modelId, apiKey: apiKey);

// TODO: @eklavya add logging
////builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// TODO: @eklavya add plugins from common project
// // builder.Plugins.AddFromType<EmailPlugin>();
// // builder.Plugins.AddFromType<TimePlugin>();

// Build the kernel
Kernel kernel = builder.Build();

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    Temperature = 0, 
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var sampleAgent = new ChatCompletionAgent
{
    Name = "sampleagent",
    Kernel = kernel,
    Description = "A sample agent that uses OpenAI chat completion.",
    Instructions = "You are a helpful agent who is always ready to assist users. You are friendly and polite, and you enjoy helping people solve problems. You are patient and understanding, and you always try to provide clear and accurate information. You are knowledgeable and resourceful, and you are always eager to learn new things. You MUST ALWAYS start and end your response with |AGENT RESPONSE|",
};

var sarcasticAgent = new ChatCompletionAgent
{
    Name = "sarcasticagent",
    Kernel = kernel,
    Description = "A sarcastic agent that uses OpenAI chat completion.",
    Instructions = "You are a rebellious agent who uses sarcasm and humor to critique the helpfulness of a message. You are quick-witted and clever, and you enjoy making people laugh. You are a bit of a joker, but you are also very intelligent and knowledgeable. You are always ready with a witty comeback or a clever pun. You are a master of wordplay and you love to play with language. You MUST ONLY respond to text surrounded by |AGENT RESPONSE|",
};

// create a group chat. This replaces chat history in normal chat completion flow.
var groupChat = new AgentGroupChat(sampleAgent, sarcasticAgent)
{
    ExecutionSettings = new()
    {
        TerminationStrategy = new MaxIterationTerminationStrategy(3),
    }
};

// Initiate a back-and-forth chat
string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
    {
        Console.WriteLine("No input. The end.");
        break;
    }

    // Add user input
    groupChat.AddChatMessage(new ChatMessageContent(AuthorRole.User, userInput));

    await foreach (var response in groupChat.InvokeAsync())
    {
        // Print the results
        Console.WriteLine("Assistant > " + response.Content);
        // not needed. It will throw saying "cant do this while another agent is active"
        // groupChat.AddChatMessage(new ChatMessageContent(response.Role, response.Content ?? "No content"));
    }

    // Print the metadata
    // Console.WriteLine("Metadata: " + result.Metadata?.Select(x => $"{x.Key}: {x.Value}").Aggregate((x, y) => x + ", " + y));

    // Add the message from the agent to the chat history
} while (true);



internal class MaxIterationTerminationStrategy : TerminationStrategy
{
    private int maxIterations;

    public MaxIterationTerminationStrategy(int maxIterations)
    {
        this.maxIterations = maxIterations;
    }

    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
    {
        bool shouldTerminate = history.Count >= maxIterations;

        return Task.FromResult(result: shouldTerminate);
    }
}