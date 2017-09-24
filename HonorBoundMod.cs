using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Utilities.Config;
using HonorBound.NetProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace HonorBound {
	class HonorBoundMod : Mod {
		public JsonConfig<HonorBoundConfigData> Config { get; private set; }

		public HonorBoundUI UI = null;
		private int LastSeenScreenWidth = -1;
		private int LastSeenScreenHeight = -1;

		internal bool NeedsUpdateBecauseNewModVersion = false;


		////////////////

		public HonorBoundMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			string filename = "Honor Bound Config.json";
			this.Config = new JsonConfig<HonorBoundConfigData>( filename, "Mod Configs", new HonorBoundConfigData() );
		}
		
		public override void Load() {
			var hamhelpmod = ModLoader.GetMod( "HamstarHelpers" );
			var min_vers = new Version( 1, 1, 1 );

			if( hamhelpmod.Version < min_vers ) {
				throw new Exception( "Hamstar's Helpers must be version " + min_vers.ToString() + " or greater." );
			}

			this.LoadConfig();

			if( !Main.dedServ ) {
				this.UI = new HonorBoundUI();
			}
		}

		private void LoadConfig() {
			try {
				if( !this.Config.LoadFile() ) {
					this.Config.SaveFile();
				}
			} catch( Exception e ) {
				DebugHelpers.Log( e.Message );
				this.Config.SaveFile();
			}

			if( this.Config.Data.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Honor Bound updated to " + HonorBoundConfigData.ConfigVersion.ToString() );
				this.Config.SaveFile();
			}
		}
		

		public override void PostSetupContent() {
			if( !Main.dedServ ) {
				this.UI.PostSetupContent();
			}
			
			this.Config.Data.Enabled = this.IsEnabled() && HonorBoundLogic.GetVersionIncompatibilityMessages().Count == 0;
		}


		////////////////

		public override void HandlePacket( BinaryReader reader, int player_who ) {
			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.HandlePacket( this, reader );
			} else if( Main.netMode == 2 ) {    // Server
				ServerPacketHandlers.HandlePacket( this, reader, player_who );
			}
		}

		////////////////

		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			if( !this.IsEnabled() ) { return; }

			var my_world = this.GetModWorld<HonorBoundWorld>();
			var my_logic = my_world.Logic;
			
			if( !my_logic.IsGameModeBegun ) {
				my_logic.RefreshAllowedHonorifics();

				int idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Mouse Text" ) );
				if( idx != -1 ) {
					var interface_layer = new LegacyGameInterfaceLayer( "HonorBound: Honorific Picker",
						delegate {
							this.UI.RefreshAllowedOptions( my_logic );

							if( this.LastSeenScreenWidth != Main.screenWidth || this.LastSeenScreenHeight != Main.screenHeight ) {
								this.LastSeenScreenWidth = Main.screenWidth;
								this.LastSeenScreenHeight = Main.screenHeight;
								this.UI.RecalculateBackend();
							}

							this.UI.CheckTogglerMouseInteraction();
							this.UI.UpdateBackend( Main._drawInterfaceGameTime );

							this.UI.Draw( Main.spriteBatch );
							this.UI.DrawToggler( Main.spriteBatch );

							return true;
						} );
					layers.Insert( idx, interface_layer );
				}
			}
		}


		////////////////

		public bool IsEnabled() {
			return this.Config.Data.Enabled;
		}
	}
}
