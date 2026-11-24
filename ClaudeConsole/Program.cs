using ClaudeConsole;
using System.Net.Http.Headers;

try
{
    var options = AnthropicOptions.Load();

    using var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://api.anthropic.com"),
        Timeout = TimeSpan.FromSeconds(100)
    };
    httpClient.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    using var cancellation = new CancellationTokenSource();
    Console.CancelKeyPress += (_, eventArgs) =>
    {
        eventArgs.Cancel = true;
        cancellation.Cancel();
    };

    var anthropicClient = new AnthropicClient(httpClient, options);

    Console.WriteLine("Claude console (type 'exit' or 'quit' to stop)");

    while (!cancellation.IsCancellationRequested)
    {
        Console.Write("> ");
        var prompt = await Console.In.ReadLineAsync(cancellation.Token);

        if (prompt is null)
        {
            break;
        }

        if (prompt.Equals("exit", StringComparison.OrdinalIgnoreCase)
            || prompt.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            continue;
        }

        try
        {
            var response = await anthropicClient.SendMessageAsync(
                prompt,
                cancellation.Token);

            Console.WriteLine(response);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            break;
        }
        catch (AnthropicApiException exception)
        {
            Console.Error.WriteLine($"Anthropic API error: {exception.Message}");
        }
        catch (HttpRequestException exception)
        {
            Console.Error.WriteLine($"Network error: {exception.Message}");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Unexpected error: {exception.Message}");
        }
    }
}
catch (InvalidOperationException exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

return 0;
