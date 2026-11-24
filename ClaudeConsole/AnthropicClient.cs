using System.Text;
using System.Text.Json;

namespace ClaudeConsole;

public sealed class AnthropicClient
{
    private const string ApiVersion = "2023-06-01";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient;
    private readonly AnthropicOptions _options;

    public AnthropicClient(HttpClient httpClient, AnthropicOptions options)
    {
        _httpClient = httpClient;
        _options = options;

        _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", ApiVersion);
    }

    public async Task<string> SendMessageAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var requestBody = new AnthropicMessageRequest
        {
            Model = _options.Model,
            Messages =
            [
                new AnthropicMessage
                {
                    Content = prompt
                }
            ]
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/v1/messages")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var responseJson = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var apiError = TryReadError(responseJson);
            var detail = apiError is null
                ? "The API returned an unsuccessful response."
                : $"{apiError.Type ?? "API error"}: {apiError.Message}";

            throw new AnthropicApiException(
                $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase}). {detail}");
        }

        try
        {
            var message = JsonSerializer.Deserialize<AnthropicMessageResponse>(
                responseJson,
                JsonOptions);

            var text = message?.Content
                .Where(block => block.Type == "text")
                .Select(block => block.Text)
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new AnthropicApiException(
                    "The API response did not contain text content.");
            }

            return text;
        }
        catch (JsonException exception)
        {
            throw new AnthropicApiException(
                $"The API returned invalid JSON: {exception.Message}");
        }
    }

    private static AnthropicError? TryReadError(string responseJson)
    {
        try
        {
            var response = JsonSerializer.Deserialize<AnthropicMessageResponse>(
                responseJson,
                JsonOptions);
            return response?.Error;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
