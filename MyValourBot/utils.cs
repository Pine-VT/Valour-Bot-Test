using System.Globalization;
using System.Text.Json;
using Valour.Sdk.Models;
using Valour.Sdk.Client;

namespace TwitchBot
{
    public static class Utils
    {
        public static async Task InitializePlanetsAsync(ValourClient client, Dictionary<long, Channel> channelCache, HashSet<long> initializedPlanets)
        {
            foreach (var planet in client.PlanetService.JoinedPlanets)
            {
                if (initializedPlanets.Contains(planet.Id))
                    continue;

                Console.WriteLine($"Initializing Planet: {planet.Name}");

                await planet.EnsureReadyAsync();
                await planet.FetchInitialDataAsync();

                foreach (var channel in planet.Channels)
                {
                    channelCache[channel.Id] = channel;

                    if (channel.ChannelType == Valour.Shared.Models.ChannelTypeEnum.PlanetChat)
                    {
                        await channel.OpenWithResult("SkyBot");
                        Console.WriteLine($"Realtime opened for: {planet.Name} -> {channel.Name}");
                    }
                }

                initializedPlanets.Add(planet.Id);
            }
        }
    };
};