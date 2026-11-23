using System.ClientModel;
using System.CommandLine;
using DotBahn.Ai;
using DotBahn.Boards;
using DotBahn.Common;
using DotBahn.Facilities;
using DotBahn.Samples.Chatbot;
using DotBahn.Stations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI;
using Spectre.Console;

return await DbChatCli.BuildRootCommand().Parse(args).InvokeAsync();

namespace DotBahn.Samples.Chatbot {
    internal static class DbChatCli {
        private const string SystemPrompt =
            "You are a helpful Deutsche Bahn assistant. Use the provided tools to answer questions " +
            "about stations, departures, arrivals, and facility status. Keep answers concise. " +
            "If a tool returns a structured error, explain the problem and ask for corrected input.";

        private static readonly Option<string?> ClientIdOption = new("--client") {
            Description = "DB API client id; falls back to the DOTBAHN_CLIENT environment variable",
        };

        private static readonly Option<string?> SecretOption = new("--secret") {
            Description = "DB API secret; falls back to the DOTBAHN_SECRET environment variable",
        };

        private static readonly Option<string> ApiKeyOption = new("--api-key") {
            Description = "Model API key; defaults to OPENAI_API_KEY or 'ollama' for local models",
            DefaultValueFactory = _ => Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "ollama",
        };

        private static readonly Option<string> EndpointOption = new("--endpoint", "-e") {
            Description = "OpenAI-compatible endpoint",
            DefaultValueFactory = _ => Environment.GetEnvironmentVariable("DOTBAHN_AI_ENDPOINT") ?? "http://localhost:11434/v1",
        };

        private static readonly Option<string> ModelOption = new("--model", "-m") {
            Description = "Model name; defaults to DOTBAHN_AI_MODEL or 'qwen3:4b'",
            DefaultValueFactory = _ => Environment.GetEnvironmentVariable("DOTBAHN_AI_MODEL") ?? "qwen3:4b",
        };

        internal static RootCommand BuildRootCommand() {
            var rootCommand = new RootCommand("Minimal local chatbot for Deutsche Bahn data") {
                ClientIdOption,
                SecretOption,
                ApiKeyOption,
                EndpointOption,
                ModelOption,
            };

            EndpointOption.Validators.Add(result => {
                var endpoint = result.GetValue(EndpointOption);
                if (!Uri.TryCreate(endpoint, UriKind.Absolute, out _)) {
                    result.AddError("--endpoint must be an absolute URI.");
                }
            });

            ModelOption.Validators.Add(result => {
                if (string.IsNullOrWhiteSpace(result.GetValue(ModelOption))) {
                    result.AddError("--model must not be empty.");
                }
            });

            rootCommand.SetAction(async (parseResult, ct) => await RunAsync(
                parseResult.GetValue(ClientIdOption),
                parseResult.GetValue(SecretOption),
                parseResult.GetValue(ApiKeyOption)!,
                parseResult.GetValue(EndpointOption)!,
                parseResult.GetValue(ModelOption)!, ct));

            return rootCommand;
        }

        private static async Task<int> RunAsync(string? clientId, string? secret, string apiKey, string endpoint, string model, CancellationToken cancellationToken) {
            if (!ResolveCredentials(clientId, secret, out var resolvedClientId, out var resolvedSecret)) {
                AnsiConsole.MarkupLine("[red]Usage:[/] provide --client/--secret, or set DOTBAHN_CLIENT / DOTBAHN_SECRET via env or CLI args");
                return 1;
            }

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            services.AddDotBahnAuthorization(opt => {
                opt.ClientId = resolvedClientId;
                opt.ApiKey = resolvedSecret;
            });

            services.AddDotBahnStations();
            services.AddDotBahnBoards();
            services.AddDotBahnFacilities();
            services.AddDotBahnAiTools();

            await using var provider = services.BuildServiceProvider();

            var openAiOptions = new OpenAIClientOptions {
                Endpoint = new Uri(endpoint)
            };
            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), openAiOptions);

            IChatClient inner = openAiClient.GetChatClient(model).AsIChatClient();
            IChatClient chat = new ChatClientBuilder(inner).UseDotBahnAiTools(provider.GetService<ILoggerFactory>()).Build();

            var chatOptions = new ChatOptions {
                Tools = provider.GetRequiredService<IReadOnlyList<AIFunction>>()
                    .Cast<AITool>()
                    .ToList()
            };

            var history = new List<ChatMessage> {
                new(ChatRole.System, SystemPrompt)
            };

            AnsiConsole.MarkupLine($"[grey]Model:[/] {Markup.Escape(model)}");
            while (true) {
                var input = AnsiConsole.Ask<string>("[blue]You[/]");
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(input, "quit", StringComparison.OrdinalIgnoreCase)) {
                    return 0;
                }

                history.Add(new ChatMessage(ChatRole.User, input));
                AnsiConsole.MarkupLine("[grey]Thinking...[/]");

                ChatResponse response;
                try {
                    response = await chat.GetResponseAsync(history, chatOptions, cancellationToken);
                } catch (OperationCanceledException) {
                    return 130;
                } catch (Exception ex) when (IsLocalEndpoint(endpoint)) {
                    AnsiConsole.MarkupLine($"[red]Local model unavailable:[/] {Markup.Escape(ex.Message)}");
                    AnsiConsole.MarkupLine($"[grey]Start Ollama with 'ollama serve' and pull the model with 'ollama pull {Markup.Escape(model)}'.[/]");
                    return 1;
                } catch (Exception ex) {
                    AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
                    return 1;
                }

                history.AddRange(response.Messages);
                AnsiConsole.MarkupLine($"[green]Assistant[/]: {Markup.Escape(response.Text)}");
            }
        }

        private static bool IsLocalEndpoint(string endpoint) =>
            endpoint.Contains("localhost", StringComparison.OrdinalIgnoreCase) ||
            endpoint.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase);

        private static bool ResolveCredentials(string? clientId, string? secret, out string resolvedClientId, out string resolvedSecret) {
            resolvedClientId = clientId ?? Environment.GetEnvironmentVariable("DOTBAHN_CLIENT") ?? string.Empty;
            resolvedSecret = secret ?? Environment.GetEnvironmentVariable("DOTBAHN_SECRET") ?? string.Empty;
            return !string.IsNullOrEmpty(resolvedClientId) && !string.IsNullOrEmpty(resolvedSecret);
        }
    }
}
