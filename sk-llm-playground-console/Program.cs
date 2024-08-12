// Import packages
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// FIXME: @eklavya fill the config below
var modelId ="";
var apiKey = "";

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId: modelId, apiKey: apiKey);

// TODO: @eklavya add logging
////builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    Temperature = 0, 
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Create a history store the conversation
var history = new ChatHistory();

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
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? "No content");
} while (true);