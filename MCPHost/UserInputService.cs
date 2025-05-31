using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Threading;
using System.Threading.Tasks;

public class UserInputService(Task<Kernel> kernelTask,ILogger<UserInputService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("Enter your prompt (or 'exit' to quit): ");
            var input = Console.ReadLine();

            if (input?.ToLower() == "exit")
                break;
            logger.LogInformation("Creating the Kernel");
            var kernel = await kernelTask;
            logger.LogInformation("Kernel created");
            // Enable automatic function calling
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            OpenAIPromptExecutionSettings executionSettings = new()
            {
                Temperature = 1,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
            };
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            var result = await kernel.InvokePromptAsync(input,new(executionSettings), cancellationToken: stoppingToken);
            var aiResponse = result.GetValue<string>();
            
            logger.LogInformation($"Azure OpenAI (Semantic Kernel): {aiResponse}");
            await Task.Delay(100, stoppingToken); // Small delay to avoid tight loop
        }
    }
}
