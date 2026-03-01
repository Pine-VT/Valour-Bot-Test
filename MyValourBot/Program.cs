using Valour.Sdk.Client;
using Valour.Sdk.Models;
using Valour.Shared.Models;
using TwitchBot;
using DotNetEnv;

Env.Load();

var client = new ValourClient("https://api.valour.gg/");
client.SetupHttpClient();

// Initialize the bot user
var token = Environment.GetEnvironmentVariable("TOKEN");
if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("TOKEN environment variable not set.");
    return;
}

var loginResult = await client.InitializeUser(token);
if (!loginResult.Success)
{
    Console.WriteLine($"Login failed: {loginResult.Message}");
    return;
}

Console.WriteLine($"Bot online: {client.Me.Name}");

// Join a specific planet first
await client.PlanetService.JoinPlanetAsync(42439954653511681, "k2tz9c4i");


//Dictionaries
var channelCache = new Dictionary<long, Channel>();
var InitializedPlanets = new HashSet<long>(); 

// Connect to all planets and channels
await Utils.InitializePlanetsAsync(client, channelCache, InitializedPlanets);

client.PlanetService.JoinedPlanetsUpdated += async () =>
{
    await Utils.InitializePlanetsAsync(client, channelCache, InitializedPlanets);
};

// Listen for messages globally
client.MessageService.MessageReceived += async (message) =>
{
    // Ignore our own messages
    if (message.AuthorUserId == client.Me.Id)
        return;

    // Only respond to messages starting with "!echo "
    if (message.Content is null || !message.Content.StartsWith("!echo "))
        return;

    var reply = message.Content.Substring(6);

    // Get the channel and send the reply
    if (channelCache.TryGetValue(message.ChannelId, out var channel))
    {
        await channel.SendMessageAsync(reply);
    }
};

Console.WriteLine("Listening for messages...");
await Task.Delay(Timeout.Infinite);