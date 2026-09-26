using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Q2.App.Common;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SubCommandApplicationCommandInteractionDataOption), 1)]
[JsonDerivedType(typeof(SubCommandGroupApplicationCommandInteractionDataOption), 2)]
[JsonDerivedType(typeof(StringApplicationCommandInteractionDataOption), 3)]
[JsonDerivedType(typeof(IntegerApplicationCommandInteractionDataOption), 4)]
[JsonDerivedType(typeof(UserApplicationCommandInteractionDataOption), 6)]
public abstract class ApplicationCommandInteractionDataOption(string name)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;
}

public class SubCommandApplicationCommandInteractionDataOption(
    string name,
    IList<ApplicationCommandInteractionDataOption> options) : ApplicationCommandInteractionDataOption(name)
{
    [JsonPropertyName("options")]
    public IList<ApplicationCommandInteractionDataOption> Options { get; set; } = options;
}

public class SubCommandGroupApplicationCommandInteractionDataOption(
    string name,
    IList<ApplicationCommandInteractionDataOption> options) : ApplicationCommandInteractionDataOption(name)
{
    [JsonPropertyName("options")]
    public IList<ApplicationCommandInteractionDataOption> Options { get; set; } = options;
}

public class StringApplicationCommandInteractionDataOption(
    string name,
    string value) : ApplicationCommandInteractionDataOption(name)
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = value;
}

public class IntegerApplicationCommandInteractionDataOption(
    string name,
    int value) : ApplicationCommandInteractionDataOption(name)
{
    [JsonPropertyName("value")]
    public int Value { get; set; } = value;
}

public class UserApplicationCommandInteractionDataOption(
    string name,
    string value) : ApplicationCommandInteractionDataOption(name)
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = value;
}

public class ApplicationCommandInteractionData(
    string name,
    IList<ApplicationCommandInteractionDataOption> options)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("options")]
    public IList<ApplicationCommandInteractionDataOption> Options { get; set; } = options;
}

public class MessageComponentInteractionData(string customId)
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;
}

public class GuildMember(User user)
{
    [JsonPropertyName("user")]
    public User User { get; set; } = user;
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PingInteraction), 1)]
[JsonDerivedType(typeof(ApplicationCommandInteraction), 2)]
[JsonDerivedType(typeof(MessageComponentInteraction), 3)]
public abstract class Interaction(string id, string applicationId, string token)
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;

    [JsonPropertyName("application_id")]
    public string ApplicationId { get; set; } = applicationId;

    [JsonPropertyName("token")]
    public string Token { get; set; } = token;
}

public class PingInteraction(string id, string applicationId, string token) : Interaction(id, applicationId, token) { }

public class User(string id, string username)
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;

    [JsonPropertyName("username")]
    public string Username { get; set; } = username;
}

public class ApplicationCommandInteraction(
    string id,
    string applicationId,
    string token,
    ApplicationCommandInteractionData data,
    string guildId,
    string channelId,
    GuildMember member) : Interaction(id, applicationId, token)
{
    [JsonPropertyName("data")]
    public ApplicationCommandInteractionData Data { get; set; } = data;

    [JsonPropertyName("guild_id")]
    public string GuildId { get; set; } = guildId;

    [JsonPropertyName("channel_id")]
    public string ChannelId { get; set; } = channelId;

    [JsonPropertyName("member")]
    public GuildMember Member { get; set; } = member;
}

public class MessageComponentInteraction(
    string id,
    string applicationId,
    string token,
    MessageComponentInteractionData data,
    string guildId,
    string channelId,
    GuildMember member) : Interaction(id, applicationId, token)
{
    [JsonPropertyName("data")]
    public MessageComponentInteractionData Data { get; set; } = data;

    [JsonPropertyName("guild_id")]
    public string GuildId { get; set; } = guildId;

    [JsonPropertyName("channel_id")]
    public string ChannelId { get; set; } = channelId;

    [JsonPropertyName("member")]
    public GuildMember Member { get; set; } = member;
}

[JsonSerializable(typeof(Interaction))]
[JsonSourceGenerationOptions(AllowOutOfOrderMetadataProperties = true)]
internal partial class InteractionContext : JsonSerializerContext { }

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PongInteractionResponse), 1)]
[JsonDerivedType(typeof(ChannelMessageWithSourceInteractionResponse), 4)]
[JsonDerivedType(typeof(DeferredChannelMessageWithSourceInteractionResponse), 5)]
[JsonDerivedType(typeof(DeferredUpdateMessageInteractionResponse), 6)]
public abstract class InteractionResponse { }

public class PongInteractionResponse : InteractionResponse { }

public class ChannelMessageWithSourceInteractionResponse(MessageInteractionCallback data) : InteractionResponse
{
    [JsonPropertyName("data")]
    public MessageInteractionCallback Data { get; set; } = data;
}

public class DeferredChannelMessageWithSourceInteractionResponse : InteractionResponse
{
    // TODO: Add support for ephemeral deferred responses
}

public class DeferredUpdateMessageInteractionResponse : InteractionResponse { }

[JsonSerializable(typeof(InteractionResponse))]
[JsonSourceGenerationOptions(AllowOutOfOrderMetadataProperties = true)]
internal partial class InteractionResponseContext : JsonSerializerContext { }

public class Application(string id)
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;
}

[JsonSerializable(typeof(Application))]
[JsonSourceGenerationOptions(AllowOutOfOrderMetadataProperties = true)]
internal partial class ApplicationContext : JsonSerializerContext { }

public class StringOptionChoice(string name, string value)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("value")]
    public string Value { get; set; } = value;
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SubCommandOption), 1)]
[JsonDerivedType(typeof(SubCommandGroupOption), 2)]
[JsonDerivedType(typeof(StringOption), 3)]
[JsonDerivedType(typeof(IntegerOption), 4)]
[JsonDerivedType(typeof(UserOption), 6)]
public abstract class ApplicationCommandOption(string name, string description)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;
}

public class SubCommandOption(
    string name,
    string description,
    IList<ApplicationCommandOption> options) : ApplicationCommandOption(name, description)
{
    [JsonPropertyName("options")]
    public IList<ApplicationCommandOption> Options { get; set; } = options;
}

public class SubCommandGroupOption(
    string name,
    string description,
    IList<ApplicationCommandOption> options) : ApplicationCommandOption(name, description)
{
    [JsonPropertyName("options")]
    public IList<ApplicationCommandOption> Options { get; set; } = options;
}

public class StringOption(
    string name,
    string description,
    bool required,
    IList<StringOptionChoice>? choices) : ApplicationCommandOption(name, description)
{
    [JsonPropertyName("required")]
    public bool Required { get; set; } = required;

    [JsonPropertyName("choices")]
    public IList<StringOptionChoice>? Choices { get; set; } = choices;
}

public class IntegerOption(
    string name,
    string description,
    bool required,
    int? min,
    int? max) : ApplicationCommandOption(name, description)
{
    [JsonPropertyName("required")]
    public bool Required { get; set; } = required;

    [JsonPropertyName("min_value")]
    public int? Min { get; set; } = min;

    [JsonPropertyName("max_value")]
    public int? Max { get; set; } = max;
}

public class UserOption(
    string name,
    string description,
    bool required) : ApplicationCommandOption(name, description)
{
    [JsonPropertyName("required")]
    public bool Required { get; set; } = required;
}

public enum IntegrationType
{
    Guild = 0,
}

public enum ContextType
{
    Guild = 0,
}

public class ApplicationCommand(
    string name,
    string description,
    IList<ApplicationCommandOption> options,
    ulong? defaultMemberPermissions,
    IList<IntegrationType> integrationTypes,
    IList<ContextType> contexts)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonPropertyName("options")]
    public IList<ApplicationCommandOption> Options { get; set; } = options;

    [JsonPropertyName("default_member_permissions")]
    public ulong? DefaultMemberPermissions { get; set; } = defaultMemberPermissions;

    [JsonPropertyName("integration_types")]
    public IList<IntegrationType> IntegrationTypes { get; set; } = integrationTypes;

    [JsonPropertyName("contexts")]
    public IList<ContextType> Contexts { get; set; } = contexts;
}

[JsonSerializable(typeof(ApplicationCommand))]
[JsonSerializable(typeof(List<ApplicationCommand>))]
[JsonSourceGenerationOptions(AllowOutOfOrderMetadataProperties = true)]
internal partial class ApplicationCommandContext : JsonSerializerContext { }

public enum MessageFlags
{
    Ephemeral = 1 << 6,
    IsComponentsV2 = 1 << 15,
}

public enum SeparatorComponentSpacing
{
    Small = 1,
    Large = 2,
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ActionRowComponent), 1)]
[JsonDerivedType(typeof(ButtonComponent), 2)]
[JsonDerivedType(typeof(TextDisplayComponent), 10)]
[JsonDerivedType(typeof(SeparatorComponent), 14)]
[JsonDerivedType(typeof(ContainerComponent), 17)]
public abstract class Component { }

public class ActionRowComponent(IEnumerable<Component> components) : Component
{
    [JsonPropertyName("components")]
    public IEnumerable<Component> Components { get; } = components;
}

public class ButtonComponent(string label, string customId, bool disabled) : Component
{
    [JsonPropertyName("style")]
    public int Style { get; } = 1;

    [JsonPropertyName("label")]
    public string Label { get; } = label;

    [JsonPropertyName("custom_id")]
    public string CustomId { get; } = customId;

    [JsonPropertyName("disabled")]
    public bool Disabled { get; } = disabled;
}

public class TextDisplayComponent(string content) : Component
{
    [JsonPropertyName("content")]
    public string Content { get; } = content;
}

public class SeparatorComponent(bool divider, SeparatorComponentSpacing spacing) : Component
{
    [JsonPropertyName("divider")]
    public bool Divider { get; } = divider;

    [JsonPropertyName("spacing")]
    public SeparatorComponentSpacing Spacing { get; } = spacing;
}

public class ContainerComponent(IEnumerable<Component> components) : Component
{
    [JsonPropertyName("components")]
    public IEnumerable<Component> Components { get; } = components;
}

public class MessageInteractionCallback(MessageFlags flags, IEnumerable<Component> components)
{
    [JsonPropertyName("flags")]
    public MessageFlags Flags { get; } = flags;

    [JsonPropertyName("components")]
    public IEnumerable<Component> Components { get; } = components;
}

[JsonSerializable(typeof(MessageInteractionCallback))]
[JsonSourceGenerationOptions(AllowOutOfOrderMetadataProperties = true)]
internal partial class MessageInteractionCallbackContext : JsonSerializerContext { }

public class DiscordClient : IDisposable
{
    private readonly string BotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN")
        ?? throw new InvalidOperationException("Missing 'DISCORD_BOT_TOKEN' environment varialble");

    private readonly HttpClient _httpClient;

    public DiscordClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiscordBot (https://github.com/rlss-gg/Q2, 0.0.1)");
    }

    public async Task<Application> GetCurrentApplicationAsync()
    {
        using var req = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://discord.com/api/v10/applications/@me");

        var res = await _httpClient.SendAsync(req);
        var application = await res.Content.ReadFromJsonAsync(ApplicationContext.Default.Application);

        if (application is null)
            throw new InvalidOperationException();

        return application;
    }

    public async Task BulkOverwriteGlobalApplicationCommandsAsync(string applicationId, IList<ApplicationCommand> commands)
    {
        using var req = new HttpRequestMessage(
            HttpMethod.Put,
            $"https://discord.com/api/v10/applications/{applicationId}/commands")
        {
            Content = JsonContent.Create(commands, ApplicationCommandContext.Default.ListApplicationCommand),
        };

        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    public async Task EditOriginalInteractionResponseAsync(string applicationId, string interactionToken, MessageInteractionCallback callback)
    {
        using var req = new HttpRequestMessage(
            HttpMethod.Patch,
            $"https://discord.com/api/v10/webhooks/{applicationId}/{interactionToken}/messages/@original")
        {
            Content = JsonContent.Create(callback, MessageInteractionCallbackContext.Default.MessageInteractionCallback),
        };

        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    public async Task CreateFollowUpMessageAsync(string applicationId, string interactionToken, MessageInteractionCallback callback)
    {
        using var req = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://discord.com/api/v10/webhooks/{applicationId}/{interactionToken}")
        {
            Content = JsonContent.Create(callback, MessageInteractionCallbackContext.Default.MessageInteractionCallback),
        };

        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
        }
    }
}
