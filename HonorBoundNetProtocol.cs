﻿using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound {
	public enum HonorBoundNetProtocolTypes : byte {
		//RequestModSettingsWithClient,
		//ModSettingsFromServer,
		RequestHonorSettingsWithClient,
		HonorSettingsFromClient,
		HonorSettingsFromServer
	}


	public class HonorBoundNetProtocol {
		public static void RoutePacket( HonorBoundMod mymod, BinaryReader reader ) {
			HonorBoundNetProtocolTypes protocol = (HonorBoundNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			/*case HonorBoundNetProtocolTypes.RequestModSettingsWithClient:
				HonorBoundNetProtocol.ReceiveSettingsRequestWithServer( mymod, reader );
				break;
			case HonorBoundNetProtocolTypes.ModSettingsFromServer:
				HonorBoundNetProtocol.ReceiveSettingsWithClient( mymod, reader );
				break;*/
			case HonorBoundNetProtocolTypes.HonorSettingsFromClient:
				HonorBoundNetProtocol.ReceiveHonorSettingsWithServer( mymod, reader );
				break;
			case HonorBoundNetProtocolTypes.HonorSettingsFromServer:
				HonorBoundNetProtocol.ReceiveHonorSettingsWithClient( mymod, reader );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////////////////////
		// Senders (client)
		////////////////////////////////

		/*public static void RequestSettingsWithClient( HonorBoundMod mymod, Player player ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)HonorBoundNetProtocolTypes.RequestModSettingsWithClient );
			packet.Write( (int)player.whoAmI );
			packet.Send();
		}*/
		
		public static void SendHonorSettingsFromClient( HonorBoundMod mymod, Player player ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			var mylogic = modworld.Logic;
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)HonorBoundNetProtocolTypes.HonorSettingsFromClient );
			packet.Write( (int)player.whoAmI );
			packet.Write( (bool)mylogic.IsHonorBound );
			packet.Write( (bool)mylogic.HasNoHonor );
			packet.Write( (int)mylogic.CurrentActiveHonorifics.Count );
			foreach( string honorific in mylogic.CurrentActiveHonorifics ) {
				packet.Write( honorific );
			}

			packet.Send( -1 );
		}

		////////////////////////////////
		// Senders (server)
		////////////////////////////////

		/*public static void SendSettingsFromServer( HonorBoundMod mymod, Player player ) {
			if( Main.netMode != 2 ) { return; } // Server only

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)HonorBoundNetProtocolTypes.ModSettingsFromServer );
			packet.Write( (string)mymod.Config.SerializeMe() );

			packet.Send( (int)player.whoAmI );
		}*/

		public static void SendHonorSettingsFromServer( HonorBoundMod mymod, Player player ) {
			if( Main.netMode != 2 ) { return; } // Server only

			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			var mylogic = modworld.Logic;
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)HonorBoundNetProtocolTypes.HonorSettingsFromServer );
			packet.Write( (bool)mylogic.IsHonorBound );
			packet.Write( (bool)mylogic.HasNoHonor );
			packet.Write( (int)mylogic.CurrentActiveHonorifics.Count );
			foreach( string honorific in mylogic.CurrentActiveHonorifics ) {
				packet.Write( honorific );
			}

			packet.Send( (int)player.whoAmI );
		}



		////////////////////////////////
		// Recipients (client)
		////////////////////////////////

		/*private static void ReceiveSettingsWithClient( HonorBoundMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			mymod.Config.DeserializeMe( reader.ReadString() );
		}*/

		public static void ReceiveHonorSettingsWithClient( HonorBoundMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { return; } // Clients only
			
			bool is_honor_bound = reader.ReadBoolean();
			bool has_no_honor = reader.ReadBoolean();
			int num_honorifics = reader.ReadInt32();

			ISet<string> honorifics = new HashSet<string>();
			for( int i = 0; i < num_honorifics; i++ ) {
				honorifics.Add( reader.ReadString() );
			}

			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			modworld.Logic.Load( mymod, is_honor_bound, has_no_honor, honorifics );
		}

		////////////////////////////////
		// Recipients (server)
		////////////////////////////////

		/*private static void ReceiveSettingsRequestWithServer( HonorBoundMod mymod, BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only

			int who = reader.ReadInt32();

			if( who < 0 || who >= Main.player.Length || Main.player[who] == null ) {
				ErrorLogger.Log( "HonorBoundNetProtocol.ReceiveSettingsRequestOnServer - Invalid player whoAmI. " + who );
				return;
			}

			HonorBoundNetProtocol.SendSettingsFromServer( mymod, Main.player[who] );
		}*/

		private static void ReceiveHonorSettingsRequestWithServer( HonorBoundMod mymod, BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only

			int who = reader.ReadInt32();

			if( who < 0 || who >= Main.player.Length || Main.player[who] == null ) {
				ErrorLogger.Log( "HonorBoundNetProtocol.ReceiveSettingsRequestOnServer - Invalid player whoAmI. " + who );
				return;
			}
			
			HonorBoundNetProtocol.SendHonorSettingsFromServer( mymod, Main.player[who] );
		}

		public static void ReceiveHonorSettingsWithServer( HonorBoundMod mymod, BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only

			int who_from = reader.ReadInt32();
			bool is_honor_bound = reader.ReadBoolean();
			bool has_no_honor = reader.ReadBoolean();
			int num_honorifics = reader.ReadInt32();

			ISet<string> honorifics = new HashSet<string>();
			for( int i=0; i<num_honorifics; i++ ) {
				honorifics.Add( reader.ReadString() );
			}

			if( who_from < 0 || who_from >= Main.player.Length || Main.player[who_from] == null ) {
				ErrorLogger.Log( "HonorBoundNetProtocol.ReceiveHonorSettingsWithServer - Invalid player whoAmI. " + who_from );
				return;
			}

			bool can_honor = true;
			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			modworld.Logic.Load( mymod, is_honor_bound, has_no_honor, honorifics );

			if( can_honor ) {
				for( int i = 0; i < Main.player.Length; i++ ) {
					if( Main.player[i] != null && Main.player[i].active && i != who_from ) {
						HonorBoundNetProtocol.SendHonorSettingsFromServer( mymod, Main.player[i] );
					}
				}
			}
		}
	}
}