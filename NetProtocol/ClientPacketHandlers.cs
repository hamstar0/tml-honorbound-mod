using HamstarHelpers.Helpers.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.NetProtocol {
	static class ClientPacketHandlers {
		public static void HandlePacket( BinaryReader reader ) {
			NetProtocolTypes protocol = (NetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			/*case HonorBoundNetProtocolTypes.RequestModSettingsWithClient:
				HonorBoundNetProtocol.ReceiveSettingsRequestWithServer( mymod, reader );
				break;*/
			case NetProtocolTypes.ReceiveHonorSettingsWithClient:
				ClientPacketHandlers.ReceiveHonorSettingsWithClient( reader );
				break;
			default:
				/*if( mymod.IsDebugInfoMode() ) {*/ LogHelpers.Log( "RouteReceivedClientPackets ...? "+protocol ); //}
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

		public static void SendHonorSettingsFromClient( Player player ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			var mymod = HonorBoundMod.Instance;
			var modworld = mymod.GetModWorld<HonorBoundWorld>();
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

			if( mymod.ConfigJson.Data.DebugModeInfo ) {
				LogHelpers.Alert( "IsHonorBound:" + mylogic.IsHonorBound +
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

		public static void ReceiveHonorSettingsWithClient( BinaryReader reader ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			var mymod = HonorBoundMod.Instance;
			bool isHonorBound = reader.ReadBoolean();
			bool hasNoHonor = reader.ReadBoolean();
			int numHonorifics = reader.ReadInt32();

			ISet<string> honorifics = new HashSet<string>();
			for( int i = 0; i < numHonorifics; i++ ) {
				honorifics.Add( reader.ReadString() );
			}

			if( mymod.ConfigJson.Data.DebugModeInfo ) {
				LogHelpers.Alert( "isHonorBound:" + isHonorBound +
					" hasNoHonor:" + hasNoHonor +
					" numHonorifics: " + numHonorifics +
					" honorifics:" + String.Join( ",", honorifics ) );
			}

			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			modworld.Logic = new HonorBoundLogic( isHonorBound, hasNoHonor, honorifics );
		}
	}
}
