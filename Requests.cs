using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace eos_bot
{
    class Requests
    {
        public static async Task<string> GetSteamName(string steamId)
        {

            var httpClient = new HttpClient();
            string getUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?steamids={steamId}&key=6833211F087334F5B31C57BC41A4A8E2";

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(getUrl);

                var content = httpResponseMessage.Content;
                var data = await content.ReadAsStringAsync();
                dynamic parsedData = JsonConvert.DeserializeObject(data);
                string playerName = parsedData.response.players[0].personaname;
                return playerName;                                

        }

        public static async Task<string> GetSteamPicture(string steamId)
        {

            var httpClient = new HttpClient();
            string getUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?steamids={steamId}&key=6833211F087334F5B31C57BC41A4A8E2";

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(getUrl);

            var content = httpResponseMessage.Content;
            var data = await content.ReadAsStringAsync();
            dynamic parsedData = JsonConvert.DeserializeObject(data);
            string playerAvatar = parsedData.response.players[0].avatarfull;
            return playerAvatar;

        }

        public static async Task<string> GetSteamProfileLink(string steamId)
        {

            var httpClient = new HttpClient();
            string getUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?steamids={steamId}&key=6833211F087334F5B31C57BC41A4A8E2";

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(getUrl);

            var content = httpResponseMessage.Content;
            var data = await content.ReadAsStringAsync();
            dynamic parsedData = JsonConvert.DeserializeObject(data);
            string profileLink = parsedData.response.players[0].profileurl;
            return profileLink;

        }


    }
}
