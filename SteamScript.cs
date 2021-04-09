extern alias steamworksnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Steamworks;


namespace eos_bot
{
	public class SteamScript
	{
		public static void Start()
		{

			
			try
			{
				Steamworks.SteamClient.Init(346110, true);
			}
			catch (System.Exception e)
			{
				// error
			}
			
		}

		public static void Stop()
		{


			Steamworks.SteamClient.Shutdown();

		}


	}
}