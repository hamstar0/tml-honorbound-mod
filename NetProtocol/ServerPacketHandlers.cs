using HamstarHelpers.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.NetProtocol {
	static class ServerPacketHandlers {
		public static void HandlePacket( HonorBoundMod mymod, BinaryReader reader, int player_who ) {
			NetProtocolTypes protocol = (NetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			/*case HonorBoundNetProtocolTypes.ModSettingsFromServer:
				HonorBoundNetProtocol.ReceiveSettingsWithClient( mymod, reader );
				break; */
			case NetProtocolTypes.ReceiveHonorSettingsWithServer:
				ServerPacketHandlers.ReceiveHonorSettingsRequestWithServer( mymod, reader );
				break;
			default:
				/*if( mymod.IsDebugInfoMode() ) {*/ DebugHelpers.Log( "RouteReceivedServerPackets ...? " + protocol ); //}
				break;
			}
		}



		////////////////
		// Server Senders
		////////////////

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

			packet.Write( (byte)NetProtocolTypes.ReceiveHonorSettingsWithClient );
			packet.Write( (bool)mylogic.IsHonorBound );
			packet.Write( (bool)mylogic.IsDishonorable );
			packet.Write( (int)mylogic.CurrentActiveHonorifics.Count );
			foreach( string honorific in mylogic.CurrentActiveHonorifics ) {
				packet.Write( honorific );
			}

			packet.Send( (int)player.whoAmI );

			if( (mymod.Config.Data.DEBUGMODE & 1) != 0 ) {
				ErrorLogger.Log( "SendHonorSettingsFromServer - IsHonorBound:" + mylogic.IsHonorBound +
					" IsDishonorable:" + mylogic.IsDishonorable +
					" CurrentActiveHonorifics:" + String.Join( ",", mylogic.CurrentActiveHonorifics ) );
			}
		}



		////////////////
		// Server Receivers
		////////////////

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

			if( (mymod.Config.Data.DEBUGMODE & 1) != 0 ) {
				ErrorLogger.Log( "ReceiveHonorSettingsRequestWithServer - who:" + who );
			}

			ServerPacketHandlers.SendHonorSettingsFromServer( mymod, Main.player[who] );
		}

		public static void ReceiveHonorSettingsWithServer( HonorBoundMod mymod, BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only

			int who_from = reader.ReadInt32();
			bool is_honor_bound = reader.ReadBoolean();
			bool has_no_honor = reader.ReadBoolean();
			int num_honorifics = reader.ReadInt32();

			ISet<string> honorifics = new HashSet<string>();
			for( int i = 0; i < num_honorifics; i++ ) {
				honorifics.Add( reader.ReadString() );
			}

			if( who_from < 0 || who_from >= Main.player.Length || Main.player[who_from] == null ) {
				ErrorLogger.Log( "HonorBoundNetProtocol.ReceiveHonorSettingsWithServer - Invalid player whoAmI. " + who_from );
				return;
			}

			if( (mymod.Config.Data.DEBUGMODE & 1) != 0 ) {
				ErrorLogger.Log( "ReceiveHonorSettingsWithServer - who_from: " + who_from +
					" is_honor_bound:" + is_honor_bound +
					" has_no_honor:" + has_no_honor +
					" num_honorifics: " + num_honorifics +
					" honorifics:" + String.Join( ",", honorifics ) );
			}

			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			modworld.Logic = new HonorBoundLogic( mymod, is_honor_bound, has_no_honor, honorifics );

			for( int i = 0; i < Main.player.Length; i++ ) {
				Player player = Main.player[i];
				if( player != null && player.active && i != who_from ) {
					ServerPacketHandlers.SendHonorSettingsFromServer( mymod, player );
				}
			}
		}
	}
}
