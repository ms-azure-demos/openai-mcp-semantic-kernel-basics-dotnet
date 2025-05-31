using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
namespace MCPHost
{
    class Program
    {
        async static Task Main(string[] args)
        {
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Task<Kernel>>(async (sp) =>
                    {
                        var builder = Kernel.CreateBuilder();
                        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
                        {
                            Name = "puppeteer",
                            Command = "npx",
                            Arguments = ["-y", "@modelcontextprotocol/server-puppeteer"],
                        });
                        var puppeteerMcpClient = await McpClientFactory.CreateAsync(clientTransport); // Replace with your MCP server URL
                        //var tools = await puppeteerMcpClient.ListToolsAsync();
                        //foreach (var tool in tools)
                        //{
                        //    Console.WriteLine($"{tool.Name}: {tool.Description}");
                        //}
                        var puppeteerMcpClientTools = await puppeteerMcpClient.ListToolsAsync();
                        
                        var azureConfig = context.Configuration.GetSection("AzureOpenAI");

                        builder.AddAzureOpenAIChatCompletion(
                            deploymentName: azureConfig["DeploymentName"],
                            endpoint: azureConfig["Endpoint"],
                            apiKey: azureConfig["ApiKey"]);
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                        builder
                        .Plugins
                        .AddFromFunctions("puppeteer", puppeteerMcpClientTools
                                                        .Select(tool => tool.AsKernelFunction()));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                        return builder.Build();
                    });
                    services.AddHostedService<UserInputService>();
                })
                .Build()
                .Run();
        }
    }
}
