using HamstarHelpers.Helpers.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.NetProtocol {
	static class ServerPacketHandlers {
		public static void HandlePacket( BinaryReader reader, int playerWho ) {
			NetProtocolTypes protocol = (NetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			/*case HonorBoundNetProtocolTypes.ModSettingsFromServer:
				HonorBoundNetProtocol.ReceiveSettingsWithClient( mymod, reader );
				break; */
			case NetProtocolTypes.ReceiveHonorSettingsWithServer:
				ServerPacketHandlers.ReceiveHonorSettingsRequestWithServer( reader );
				break;
			default:
				/*if( mymod.IsDebugInfoMode() ) {*/ LogHelpers.Log( "RouteReceivedServerPackets ...? " + protocol ); //}
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

		public static void SendHonorSettingsFromServer( Player player ) {
			if( Main.netMode != 2 ) { return; } // Server only

			var mymod = HonorBoundMod.Instance;
			var modworld = ModContent.GetInstance<HonorBoundWorld>();
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

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "IsHonorBound:" + mylogic.IsHonorBound +
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

		private static void ReceiveHonorSettingsRequestWithServer( BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only

			var mymod = HonorBoundMod.Instance;
			int who = reader.ReadInt32();

			if( who < 0 || who >= Main.player.Length || Main.player[who] == null ) {
				LogHelpers.Warn( "invalid player whoAmI. " + who );
				return;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "who:" + who );
			}

			ServerPacketHandlers.SendHonorSettingsFromServer( Main.player[who] );
		}

		public static void ReceiveHonorSettingsWithServer( BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only

			var mymod = HonorBoundMod.Instance;
			int whoFrom = reader.ReadInt32();
			bool isHonorBound = reader.ReadBoolean();
			bool hasNoHonor = reader.ReadBoolean();
			int numHonorifics = reader.ReadInt32();

			ISet<string> honorifics = new HashSet<string>();
			for( int i = 0; i < numHonorifics; i++ ) {
				honorifics.Add( reader.ReadString() );
			}

			if( whoFrom < 0 || whoFrom >= Main.player.Length || Main.player[whoFrom] == null ) {
				LogHelpers.Warn( "Invalid player whoAmI. " + whoFrom );
				return;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "whoFrom: " + whoFrom +
					" isHonorBound:" + isHonorBound +
					" hasNoHonor:" + hasNoHonor +
					" numHonorifics: " + numHonorifics +
					" honorifics:" + String.Join( ",", honorifics ) );
			}

			var modworld = ModContent.GetInstance<HonorBoundWorld>();
			modworld.Logic = new HonorBoundLogic( isHonorBound, hasNoHonor, honorifics );

			for( int i = 0; i < Main.player.Length; i++ ) {
				Player player = Main.player[i];
				if( player != null && player.active && i != whoFrom ) {
					ServerPacketHandlers.SendHonorSettingsFromServer( player );
				}
			}
		}
	}
}
