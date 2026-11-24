namespace ClaudeConsole;

public sealed record AnthropicOptions(string ApiKey, string Model)
{
    public static AnthropicOptions Load()
    {
        var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "ANTHROPIC_API_KEY environment variable is not set.");
        }

        var model = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL");
        if (string.IsNullOrWhiteSpace(model))
        {
            throw new InvalidOperationException(
                "ANTHROPIC_MODEL environment variable is not set.");
        }

        return new AnthropicOptions(apiKey, model);
        //testing secon change
    }
}
