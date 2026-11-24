using System.Text.Json.Serialization;

namespace ClaudeConsole;

public sealed class AnthropicMessageRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; } = 1024;

    [JsonPropertyName("messages")]
    public required AnthropicMessage[] Messages { get; init; }
}

public sealed class AnthropicMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = "user";

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}

public sealed class AnthropicMessageResponse
{
    [JsonPropertyName("content")]
    public AnthropicContentBlock[] Content { get; init; } = [];

    [JsonPropertyName("error")]
    public AnthropicError? Error { get; init; }
}

public sealed class AnthropicContentBlock
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public sealed class AnthropicError
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
