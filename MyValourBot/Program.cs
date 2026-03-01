using Valour.Sdk.Client;
using Valour.Sdk.Models;
using Valour.Shared.Models;

var client = new ValourClient("https://api.valour.gg/");
client.SetupHttpClient();

// Initialize the bot user
var loginResult = await client.InitializeUser("bot-8786d48e-a08b-43d6-8d0a-7c9a09423e3a");
if (!loginResult.Success)
{
    Console.WriteLine($"Login failed: {loginResult.Message}");
    return;
}

Console.WriteLine($"Bot online: {client.Me.Name}");

// Join a specific planet first
await client.PlanetService.JoinPlanetAsync(42439954653511681, "k2tz9c4i");

// Connect to all planets and channels (BotService helper)
await client.BotService.JoinAllChannelsAsync();

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
    if (client.Cache.Channels.TryGet(message.ChannelId, out var channel))
    {
        await channel.SendMessageAsync(reply);
    }
};

Console.WriteLine("Listening for messages...");
await Task.Delay(Timeout.Infinite);
