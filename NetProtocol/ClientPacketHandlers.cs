using HamstarHelpers.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.NetProtocol {
	static class ClientPacketHandlers {
		public static void HandlePacket( HonorBoundMod mymod, BinaryReader reader ) {
			NetProtocolTypes protocol = (NetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			/*case HonorBoundNetProtocolTypes.RequestModSettingsWithClient:
				HonorBoundNetProtocol.ReceiveSettingsRequestWithServer( mymod, reader );
				break;*/
			case NetProtocolTypes.ReceiveHonorSettingsWithClient:
				ClientPacketHandlers.ReceiveHonorSettingsWithClient( mymod, reader );
				break;
			default:
				/*if( mymod.IsDebugInfoMode() ) {*/ DebugHelpers.Log( "RouteReceivedClientPackets ...? "+protocol ); //}
				break;
			}
		}



		////////////////
		// Client Senders
		////////////////

		/*public static void RequestSettingsWithClient( HonorBoundMod mymod, Player player ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)HonorBoundNetProtocolTypes.RequestModSettingsWithClient );
			packet.Write( (int)player.whoAmI );
			packet.Send();
		}*/

		public static void SendHonorSettingsFromClient( HonorBoundMod mymod, Player player ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			var modworld = mymod.GetModWorld<MyWorld>();
			var mylogic = modworld.Logic;
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)NetProtocolTypes.ReceiveHonorSettingsWithServer );
			packet.Write( (int)player.whoAmI );
			packet.Write( (bool)mylogic.IsHonorBound );
			packet.Write( (bool)mylogic.IsDishonorable );
			packet.Write( (int)mylogic.CurrentActiveHonorifics.Count );
			foreach( string honorific in mylogic.CurrentActiveHonorifics ) {
				packet.Write( honorific );
			}

			packet.Send( -1 );

			if( (mymod.Config.Data.DEBUGMODE & 1) != 0 ) {
				ErrorLogger.Log( "SendHonorSettingsFromClient - IsHonorBound:" + mylogic.IsHonorBound +
					" IsDishonorable:" + mylogic.IsDishonorable +
					" CurrentActiveHonorifics:" + String.Join( ",", mylogic.CurrentActiveHonorifics ) );
			}
		}


		////////////////
		// Client Receivers
		////////////////


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

			if( (mymod.Config.Data.DEBUGMODE & 1) != 0 ) {
				ErrorLogger.Log( "ReceiveHonorSettingsWithClient - is_honor_bound:" + is_honor_bound +
					" has_no_honor:" + has_no_honor +
					" num_honorifics: " + num_honorifics +
					" honorifics:" + String.Join( ",", honorifics ) );
			}

			var modworld = mymod.GetModWorld<MyWorld>();
			modworld.Logic = new HonorBoundLogic( mymod, is_honor_bound, has_no_honor, honorifics );
		}
	}
}
