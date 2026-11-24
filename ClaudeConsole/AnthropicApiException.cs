namespace ClaudeConsole;

public sealed class AnthropicApiException : Exception
{
    public AnthropicApiException(string message)
        : base(message)
    {
    }
}
