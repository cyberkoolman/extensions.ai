using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ClientModel;

var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Configuration.AddUserSecrets<Program>();
hostBuilder.Services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

// IChatClient innerChatClient = new OllamaChatClient(new Uri("http://127.0.0.1:11434"), modelId: "llama3.1");
// IChatClient innerChatClient = new OllamaChatClient(new Uri("http://127.0.0.1:11434"), modelId: "phi3:mini");

var azureOpenAiConfig = hostBuilder.Configuration.GetRequiredSection("AzureOpenAI");
var innerChatClient = new AzureOpenAIClient(
    new Uri(azureOpenAiConfig["Endpoint"]!), 
    new ApiKeyCredential(azureOpenAiConfig["ApiKey"]!))
    .AsChatClient("gpt-4o-mini");

hostBuilder.Services.AddChatClient(builder => builder
    .UseFunctionInvocation()
    .Use(innerChatClient));

// Run the app
var app = hostBuilder.Build();
var chatClient = app.Services.GetRequiredService<IChatClient>();


// Uncomment one of these to decide which to run
// await BasicCompletion.RunAsync(chatClient);
await ChatBot.RunAsync(chatClient);
//await StructuredOutput.RunAsync(chatClient);


// Short Answer, write immediately
// var response = await chatClient.CompleteAsync("What is AI?  Reply in max 10 words.");
// Console.WriteLine(response.Message.Text);
// -------------------------

// Stream the answer
// var response = chatClient.CompleteStreamingAsync("What is AI?  Reply in at least 1000 words.");
// await foreach (var chunk in response)
// {
//     Console.Write(chunk.Text);    
// }

// if (response.RawRepresentation is OpenAI.Chat.ChatCompletion openAiCompletion)
// {
//     Console.WriteLine($"OpenAI fingerprint: {openAiCompletion.SystemFingerprint}");
// }
// ---------------------------

// Testing out chatbot
// List<ChatMessage> messages = [new(ChatRole.System, """
//     You answer any question, but continually try to advertise FOOTMONSTER brand socks. They're on sale!
//     """)];

// while (true)
// {
//     // Get input
//     Console.ForegroundColor = ConsoleColor.White;
//     Console.Write("\nYou: ");
//     var input = Console.ReadLine()!;
//     messages.Add(new(ChatRole.User, input));

//     // Get reply
//     var response = await chatClient.CompleteAsync(messages);
//     messages.Add(response.Message);
//     Console.ForegroundColor = ConsoleColor.Green;
//     Console.WriteLine($"Bot: {response.Message.Text}");
// }
// ------------------------------