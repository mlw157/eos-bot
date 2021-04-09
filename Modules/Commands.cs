extern alias steamworksnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Addons.Interactive;
using Steamworks;
using Steamworks.Data;
using Discord;
using Okolni;
using Okolni.Source.Query;
using MySql.Data.MySqlClient;
using Interactivity;
using Interactivity.Confirmation;
using System.Linq;

namespace eos_bot.Modules
{

    

    public class Commands : ModuleBase<SocketCommandContext>


    {

        string message1 = "";
        public InteractivityService Interactivity { get; set; }

            

        public async Task<string> GetServerIP(string requestedServer)
        {
            

            string serverIp = "";

            List<string> serverList = await Databases.GetServerName(requestedServer);


            if (serverList.Count == 2) //more than one server found
            {
                await ReplyAsync($"```Multiple servers found. Specify the server name or enter a number.{Environment.NewLine}{Environment.NewLine}1: {serverList[0]} {Environment.NewLine}2: {serverList[1]}```");
                var response = await Interactivity.NextMessageAsync(x => x.Author.Id == Context.User.Id);



                if (response.Value.Content == "1")
                {
                    serverIp = await Databases.GetServerIP(serverList[0]);

                }
                else if (response.Value.Content == "2")
                {
                    serverIp = await Databases.GetServerIP(serverList[1]);

                }

            }
            else
            {
                serverIp = await Databases.GetServerIP(serverList[0]); //get server ip

            }

            
            return serverIp;
        }


        [Command("scan", RunMode = RunMode.Async)]
        public async Task scan(string requestedServer)
        {
            SteamScript.Start();

            

            string IP = await GetServerIP(requestedServer); // get server IP

            if (IP == null) //if server doesn't exist
            {

                await ReplyAsync($"```Invalid Server```");

            }


            ulong serverID = default;
            string serverName = default;

            using (var list = new Steamworks.ServerList.IpList(new string[] { IP.Split(':')[0] }))
            {

                await list.RunQueryAsync();
                list.UpdateResponsive();
                foreach (var server in list.Responsive)
                {
                    if (server.QueryPort == int.Parse(IP.Split(':')[1]) && server.SteamId != 0)
                    {
                        //Console.WriteLine($"Server Address: {server.Address} Server Name: {server.Name} Server Player Count: {server.Players} Server SteamID: {server.SteamId}");


                        serverID = server.SteamId;
                        serverName = server.Name;
                    }
                }
            } //getting server ID and Name





            var serverIP = IP.Split(':')[0];
            var serverPort = IP.Split(':')[1];
            var IPuInt = BitConverter.ToUInt32(IPAddress.Parse(serverIP).GetAddressBytes(), 0);
            var portUInt = Convert.ToUInt16(serverPort);
            var ServerCSteamID = new steamworksnet.Steamworks.CSteamID(serverID);

            List<object> playerIDList = new List<object>();
            List<object> playerIDList2 = new List<object>();
            List<object> playerIDList3 = new List<object>();
            List<object> playerIDList4 = new List<object>();
            List<object> playerIDList5 = new List<object>();
            List<object> playerIDList6 = new List<object>();


            string idMessage1 = "";
            string idMessage2 = "";
            string idMessage3 = "";
            string idMessage4 = "";
            string idMessage5 = "";
            string idMessage6 = "";
          
           





            steamworksnet.Steamworks.SteamUser.AdvertiseGame(ServerCSteamID, IPuInt, portUInt);

            await Task.Delay(500);

            var playerCount = steamworksnet.Steamworks.SteamFriends.GetFriendCountFromSource(ServerCSteamID); // number of detected ids

             //discord bot message

            if (playerCount == 0)
            {
                await ReplyAsync("```Failed to scan this server. This is a issue from steam, not the bot.```");
            }

            if (playerCount > 0 && playerCount < 40)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount}```");

                for (int i = 0; i < playerCount; i++) //get steam ids of server
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList.Add(playerID);                    
                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {
                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage1 = idMessage1 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{idMessage1}```");
            }

            if (playerCount >= 40 && playerCount < 80)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount}```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < playerCount; i++) //get rest of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList2.Add(playerID);

                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {



                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage1 = idMessage1 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage2 = idMessage2 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{idMessage1}```");
                await ReplyAsync($"```{idMessage2}```");
            }

            if (playerCount >= 80 && playerCount < 120)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount}```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < 80; i++) //get 25-50 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList2.Add(playerID);

                }
                for (int i = 80; i < playerCount; i++) //get rest of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList3.Add(playerID);

                }

                foreach (var id in playerIDList) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage1 = idMessage1 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage2 = idMessage2 + $"#{id}  Name: {playerName} {Environment.NewLine}";

                }
                foreach (var id in playerIDList3) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage3 = idMessage3 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{idMessage1}```");
                await ReplyAsync($"```{idMessage2}```");
                await ReplyAsync($"```{idMessage3}```");

            }

            if (playerCount >= 120 && playerCount < 160)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount}```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < 80; i++) //get 25-50 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList2.Add(playerID);

                }
                for (int i = 80; i < 120; i++) //get 50-75 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList3.Add(playerID);

                }
                for (int i = 120; i < playerCount; i++) //get rest of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList4.Add(playerID);

                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage1 = idMessage1 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage2 = idMessage2 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList3) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage3 = idMessage3 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList4) //make discord message for the steam ids
                {


                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage4 = idMessage4 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{idMessage1}```");
                await ReplyAsync($"```{idMessage2}```");
                await ReplyAsync($"```{idMessage3}```");
                await ReplyAsync($"```{idMessage4}```");

            }

            if (playerCount >= 160 && playerCount < 200)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount}```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < 80; i++) //get 25-50 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList2.Add(playerID);

                }
                for (int i = 80; i < 120; i++) //get 50-75 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList3.Add(playerID);

                }
                for (int i = 120; i < 160; i++) //get 75-100 of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList4.Add(playerID);

                }
                for (int i = 160; i < playerCount; i++) //get 100-125 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList5.Add(playerID);

                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage1 = idMessage1 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage2 = idMessage2 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList3) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage3 = idMessage3 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList4) //make discord message for the steam ids
                {


                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage4 = idMessage4 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList5) //make discord message for the steam ids
                {


                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage5 = idMessage5 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{idMessage1}```");
                await ReplyAsync($"```{idMessage2}```");
                await ReplyAsync($"```{idMessage3}```");
                await ReplyAsync($"```{idMessage4}```");
                await ReplyAsync($"```{idMessage5}```");

            }

            if (playerCount >= 200)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount}```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < 80; i++) //get 25-50 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList2.Add(playerID);

                }
                for (int i = 80; i < 120; i++) //get 50-75 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList3.Add(playerID);

                }
                for (int i = 120; i < 160; i++) //get 75-100 of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList4.Add(playerID);

                }
                for (int i = 160; i < 200; i++) //get 100-125 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList5.Add(playerID);

                }
                for (int i = 200; i < playerCount; i++) //get rest of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetFriendFromSourceByIndex(ServerCSteamID, i);
                    playerIDList6.Add(playerID);

                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage1 = idMessage1 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage2 = idMessage2 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList3) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage3 = idMessage3 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList4) //make discord message for the steam ids
                {


                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage4 = idMessage4 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList5) //make discord message for the steam ids
                {


                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage5 = idMessage5 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList6) //make discord message for the steam ids
                {


                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    idMessage6 = idMessage6 + $"#{id}  Name: {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{idMessage1}```");
                await ReplyAsync($"```{idMessage2}```");
                await ReplyAsync($"```{idMessage3}```");
                await ReplyAsync($"```{idMessage4}```");
                await ReplyAsync($"```{idMessage5}```");
                await ReplyAsync($"```{idMessage6}```");
            }

            SteamScript.Stop();
        }

        [Command("id", RunMode = RunMode.Async)]
        public async Task ID(string steamID)
        {
            //SteamScript.Start();


            string playerName = await Requests.GetSteamName(steamID);
            string profileAvatarLink = await Requests.GetSteamPicture(steamID);
            string profileLink = await Requests.GetSteamProfileLink(steamID);
           
            EmbedBuilder playerEmbed = new EmbedBuilder();

            playerEmbed.WithTitle($"Steam ID: {steamID}");
            playerEmbed.AddField("Steam Name", playerName, false);
            playerEmbed.AddField("Profile Link", profileLink, false);
            playerEmbed.WithThumbnailUrl(profileAvatarLink);

            playerEmbed.WithColor(Discord.Color.DarkGrey);
            await ReplyAsync("", false, playerEmbed.Build());




        }

        [Command("server", RunMode = RunMode.Async)]
        public async Task Server(string requestedServer)
        {

            //SteamScript.Start();




            string IP = await GetServerIP(requestedServer); // get server IP

            if (IP == null) //if server doesn't exist
            {

                await ReplyAsync($"```Invalid Server```");

            }



            string serverIP = IP.Split(':')[0];
            int serverPort = Convert.ToInt32(IP.Split(':')[1]);

            IQueryConnection conn = new QueryConnection();

            conn.Host = serverIP;
            conn.Port = serverPort;

            conn.Connect();

            var serverInfo = conn.GetInfo(); // Get the Server info
            var players = conn.GetPlayers(); // Get the Player info
            var rules = conn.GetRules(); // Get the Rules

            string playerMessage = "";
            int playerCount = 0;






            foreach (var player in players.Players)
            {
                if (player.Name != "")
                {

                    string playerDuration = player.Duration.ToString();
                    playerDuration = playerDuration.Remove(playerDuration.Length - 8);


                    playerCount = playerCount + 1;

                    playerMessage = $"{playerMessage}[{playerDuration}]  {player.Name}{Environment.NewLine}";



                }              

            }

            await ReplyAsync($"```{serverInfo.Name}{Environment.NewLine}Map: {serverInfo.Map}{Environment.NewLine}Players: {playerCount}/{serverInfo.MaxPlayers}{Environment.NewLine}Server Address: {IP}```");
            await ReplyAsync($"``` Duration   Name{Environment.NewLine}{Environment.NewLine}{playerMessage}```");
        }

        [Command("a", RunMode = RunMode.Async)]
        public async Task A(string requestedServer)
        {

            SteamScript.Start();

            string IP = await GetServerIP(requestedServer); // get server IP

            if (IP == null) //if server doesn't exist
            {

                await ReplyAsync($"```Invalid Server```");

            }


            ulong serverID = default;
            string serverName = default;

            using (var list = new Steamworks.ServerList.IpList(new string[] { IP.Split(':')[0] }))
            {

                await list.RunQueryAsync();
                list.UpdateResponsive();
                foreach (var server in list.Responsive)
                {
                    if (server.QueryPort == int.Parse(IP.Split(':')[1]) && server.SteamId != 0)
                    {
                        //Console.WriteLine($"Server Address: {server.Address} Server Name: {server.Name} Server Player Count: {server.Players} Server SteamID: {server.SteamId}");


                        serverID = server.SteamId;
                        serverName = server.Name;
                    }
                }
            } //getting server ID and Name


            

            string serverIP = IP.Split(':')[0];
            int serverPort = Convert.ToInt32(IP.Split(':')[1]);

            var IPuInt = BitConverter.ToUInt32(IPAddress.Parse(serverIP).GetAddressBytes(), 0);
            var portUInt = Convert.ToUInt16(serverPort);
            var ServerCSteamID = new steamworksnet.Steamworks.CSteamID(serverID);

           

            List<object> playerIDList = new List<object>();
            List<object> playerIDList2 = new List<object>();
            List<object> playerIDList3 = new List<object>();

            string message1 = "";
            string message2 = "";
            string message3 = "";

            DateTime currentTime = DateTime.Now;


            steamworksnet.Steamworks.SteamUser.AdvertiseGame(ServerCSteamID, IPuInt, portUInt);

            await Task.Delay(500);

            var playerCount = steamworksnet.Steamworks.SteamFriends.GetCoplayFriendCount();

            //await Task.Delay(50000);

            

            

            if (playerCount == 0)
            {
                await ReplyAsync("```Failed to scan this server. This is a issue from steam, not the bot.```");
            }

            if (playerCount > 0 && playerCount < 40)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount} {Environment.NewLine} Duration        Steam-ID       Name  ```");

                for (int i = 0; i < playerCount; i++) //get steam ids of server
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList.Add(playerID);
                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {
                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    var eSeconds = steamworksnet.Steamworks.SteamFriends.GetFriendCoplayTime(cID);
                    DateTimeOffset oTime = DateTimeOffset.FromUnixTimeSeconds(eSeconds);
                    DateTime playerTime = oTime.DateTime;
                    TimeSpan time = currentTime - playerTime;
                    string serverTime = time.ToString();
                    serverTime = serverTime.Remove(serverTime.Length - 8);

                    message1 = message1 + $"[{serverTime}] #{id} {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{message1}```");
            }

            if (playerCount >= 40 && playerCount < 80)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount} {Environment.NewLine} Duration        Steam-ID       Name  ```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < playerCount; i++) //get rest of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList2.Add(playerID);

                }
                foreach (var id in playerIDList) //make discord message for the steam ids
                {



                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    var eSeconds = steamworksnet.Steamworks.SteamFriends.GetFriendCoplayTime(cID);
                    DateTimeOffset oTime = DateTimeOffset.FromUnixTimeSeconds(eSeconds);
                    DateTime playerTime = oTime.DateTime;
                    TimeSpan time = currentTime - playerTime;
                    string serverTime = time.ToString();
                    serverTime = serverTime.Remove(serverTime.Length - 8);
                    message1 = message1 + $"[{serverTime}] #{id} {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    var eSeconds = steamworksnet.Steamworks.SteamFriends.GetFriendCoplayTime(cID);
                    DateTimeOffset oTime = DateTimeOffset.FromUnixTimeSeconds(eSeconds);
                    DateTime playerTime = oTime.DateTime;
                    TimeSpan time = currentTime - playerTime;
                    string serverTime = time.ToString();
                    serverTime = serverTime.Remove(serverTime.Length - 8);
                    message2 = message2 + $"[{serverTime}] #{id} {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{message1}```");
                await ReplyAsync($"```{message2}```");
            }

            if (playerCount >= 80 && playerCount < 120)
            {
                await ReplyAsync($"```{serverName}``````Server IP: {IP} {Environment.NewLine}Total Detected Players: {playerCount} {Environment.NewLine}{Environment.NewLine} Duration       Steam-ID      Name  ```");

                for (int i = 0; i < 40; i++) //get first 25 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList.Add(playerID);

                }

                for (int i = 40; i < 80; i++) //get 25-50 steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList2.Add(playerID);

                }
                for (int i = 80; i < playerCount; i++) //get rest of steam ids of server 
                {
                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList3.Add(playerID);

                }

                foreach (var id in playerIDList) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    var eSeconds = steamworksnet.Steamworks.SteamFriends.GetFriendCoplayTime(cID);
                    DateTimeOffset oTime = DateTimeOffset.FromUnixTimeSeconds(eSeconds);
                    DateTime playerTime = oTime.DateTime;
                    TimeSpan time = currentTime - playerTime;
                    string serverTime = time.ToString();
                    serverTime = serverTime.Remove(serverTime.Length - 8);
                    message1 = message1 + $"[{serverTime}] #{id} {playerName} {Environment.NewLine}";
                }
                foreach (var id in playerIDList2) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    var eSeconds = steamworksnet.Steamworks.SteamFriends.GetFriendCoplayTime(cID);
                    DateTimeOffset oTime = DateTimeOffset.FromUnixTimeSeconds(eSeconds);
                    DateTime playerTime = oTime.DateTime;
                    TimeSpan time = currentTime - playerTime;
                    string serverTime = time.ToString();
                    serverTime = serverTime.Remove(serverTime.Length - 8);
                    message2 = message2 + $"[{serverTime}] #{id} {playerName} {Environment.NewLine}";

                }
                foreach (var id in playerIDList3) //make discord message for the steam ids
                {

                    string idString = id.ToString();
                    var ulongID = Convert.ToUInt64(idString);

                    var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                    string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);
                    var eSeconds = steamworksnet.Steamworks.SteamFriends.GetFriendCoplayTime(cID);
                    DateTimeOffset oTime = DateTimeOffset.FromUnixTimeSeconds(eSeconds);
                    DateTime playerTime = oTime.DateTime;
                    TimeSpan time = currentTime - playerTime;
                    string serverTime = time.ToString();
                    serverTime = serverTime.Remove(serverTime.Length - 8);
                    message3 = message3 + $"[{serverTime}] #{id} {playerName} {Environment.NewLine}";
                }
                await ReplyAsync($"```{message1}```");
                await ReplyAsync($"```{message2}```");
                await ReplyAsync($"```{message3}```");

            }





        }

        [Command("as", RunMode = RunMode.Async)]
        public async Task Accurate(string requestedServer)
        {

            SteamScript.Start();

            

            string IP = await GetServerIP(requestedServer); // get server IP

            if (IP == null) //if server doesn't exist
            {

                await ReplyAsync($"```Invalid Server```");

            }


            ulong serverID = default;
            string serverName = default;

            using (var list = new Steamworks.ServerList.IpList(new string[] { IP.Split(':')[0] }))
            {

                await list.RunQueryAsync();
                list.UpdateResponsive();
                foreach (var server in list.Responsive)
                {
                    if (server.QueryPort == int.Parse(IP.Split(':')[1]) && server.SteamId != 0)
                    {
                        //Console.WriteLine($"Server Address: {server.Address} Server Name: {server.Name} Server Player Count: {server.Players} Server SteamID: {server.SteamId}");


                        serverID = server.SteamId;
                        serverName = server.Name;
                    }
                }
            } //getting server ID and Name


            string[] botIds = { "76561198087880962", "76561198440715359", "76561198997043966", "76561198871471227", "76561198841854802"};

            List<object> playerIDList = new List<object>();
            List<object> playerIDList2 = new List<object>();
            List<string> onServerList = new List<string>();

           
            string idString = "";



            string serverIP = IP.Split(':')[0];
            int serverPort = Convert.ToInt32(IP.Split(':')[1]);

            var IPuInt = BitConverter.ToUInt32(IPAddress.Parse(serverIP).GetAddressBytes(), 0);
            var portUInt = Convert.ToUInt16(serverPort);
            var ServerCSteamID = new steamworksnet.Steamworks.CSteamID(serverID);

            steamworksnet.Steamworks.SteamUser.AdvertiseGame(ServerCSteamID, IPuInt, portUInt);

            

            await Task.Delay(1000);

            while (true)
            {
                
                

                var playerCount = steamworksnet.Steamworks.SteamFriends.GetCoplayFriendCount();

                for (int i = 0; i < playerCount; i++)
                {

                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList.Add(playerID);

                }


                await Task.Delay(10000);

                for (int i = 0; i < playerCount; i++)
                {

                    var playerID = steamworksnet.Steamworks.SteamFriends.GetCoplayFriend(i);
                    playerIDList2.Add(playerID);

                }

                Console.WriteLine("Scanning");

                var newPlayerIds = playerIDList2.Except(playerIDList).ToList();

                int newPlayerCount = newPlayerIds.Count();

             

                if (newPlayerIds.Any())
                {

                    

                    foreach (var newPlayer in newPlayerIds)
                    {

                        idString = newPlayer.ToString();

                        if (!botIds.Contains(idString) && !onServerList.Contains(idString))
                        {

                            var ulongID = Convert.ToUInt64(idString);

                            var cID = new steamworksnet.Steamworks.CSteamID(ulongID);
                            string playerName = steamworksnet.Steamworks.SteamFriends.GetFriendPersonaName(cID);

                            message1 = message1 + $"[{idString}] {playerName} {Environment.NewLine}";

                            await ReplyAsync($"```{message1}```");
                            await ReplyAsync($"```Player Joined server {serverName} {Environment.NewLine} ID: #{idString} Name: {playerName}```");
                        }


                        if (!playerIDList.Contains(idString))
                        {

                            onServerList.Add(idString);

                        }

                    }
                   

                }

                
            }           
        }


        [Command("asc", RunMode = RunMode.Async)]
        public async Task Current(string message)
        {

            

        }


    }
    

}



     
