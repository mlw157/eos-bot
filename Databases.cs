using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using Discord.Commands;
using Steamworks;
using Steamworks.Data;
using Discord;
using Okolni;
using Okolni.Source.Query;
using System.Data;

namespace eos_bot
{   
    class Databases
    {

        public static async Task Connect()
        {
          

            string connStr = ""; //add your database containing the servers here, use server_info.mysql

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "SELECT * FROM serversInfo";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);               
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public static async Task<List<string>> GetServerName(string requestedServer)
        {

            
            List<string> serverList = new List<string>();

            

            string connStr = "server=localhost;user=root;database=servers;port=3306;password=password";

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {

                conn.Open();


                
                var cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (requestedServer[0] == 's' | requestedServer[0] == 'S')
                {
                    
                    cmd.CommandText = "SELECT serverName FROM serversInfo WHERE serverName LIKE CONCAT('%', @server, '%') LIMIT 2";
                }
                else
                {
                    cmd.CommandText = "SELECT serverName FROM serversInfo WHERE serverName LIKE CONCAT('%', @server, '%')  AND NOT serverName LIKE '%Tribes%' LIMIT 2";
                }
                
                cmd.Parameters.Add("@server", MySql.Data.MySqlClient.MySqlDbType.VarChar).Value = requestedServer;


                MySqlDataReader rdr = cmd.ExecuteReader();




                while (rdr.Read())
                {
                    string server1 = rdr.GetString(0);
                    serverList.Add(server1);

                }
                    if (rdr.NextResult())
                    {
                        while (rdr.Read())
                        {
                            string server2 = rdr.GetString(0);
                            serverList.Add(server2);

                        }
                    }

                
                rdr.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            return serverList;
        }

        public static async Task<string> GetServerIP(string serverName)
        {




            string serverIp = "";
            string connStr = "server=localhost;user=root;database=servers;port=3306;password=password";

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {

                conn.Open();



                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT serverIp FROM serversInfo WHERE serverName = @server";
                cmd.Parameters.Add("@server", MySql.Data.MySqlClient.MySqlDbType.VarChar).Value = serverName;


                MySqlDataReader rdr = cmd.ExecuteReader();




                while (rdr.Read())
                {
                    serverIp = rdr.GetString(0);
                   
                }
               
                


                rdr.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            return serverIp;
        }

    }
       

}

