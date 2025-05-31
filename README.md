# Demo of MCP capabilities 
Using Azure OpenAI perform series of task using mcp server

# How to run
- Replace the values in appsettings.json
- Run the following command to install the required packages:
  ```
  dotnet restore
  ```
- Compile and run the console application
# Prerequisites
- Azure OpenAI account with access to the required models
- Node.js installed on your machine

# Technologies
- Azure openai
- CSharp
- DotNet 8.0
- nugets
	- Microsoft.SemanticKernel
	- ModelContextProtocol
- Node.JS
	- npx
- MCP Server
	- @modelcontextprotocol/server-puppeteer