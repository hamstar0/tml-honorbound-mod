using HamstarHelpers.Components.Config;
using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DebugHelpers;
using HonorBound.NetProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace HonorBound {
	class HonorBoundMod : Mod {
		public static HonorBoundMod Instance { get; private set; }

		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-honorbound-mod"; } }

		public static string ConfigFileRelativePath {
			get { return ConfigurationDataBase.RelativePath + Path.DirectorySeparatorChar + HonorBoundConfigData.ConfigFileName; }
		}

		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new HamstarException( "Cannot reload configs outside of single player." );
			}
			if( HonorBoundMod.Instance != null ) {
				if( !HonorBoundMod.Instance.ConfigJson.LoadFile() ) {
					HonorBoundMod.Instance.ConfigJson.SaveFile();
				}
			}
		}

		public static void ResetConfigFromDefaults() {
			if( Main.netMode != 0 ) {
				throw new HamstarException( "Cannot reset to default configs outside of single player." );
			}

			var new_config = new HonorBoundConfigData();
			//new_config.SetDefaults();

			HonorBoundMod.Instance.ConfigJson.SetData( new_config );
			HonorBoundMod.Instance.ConfigJson.SaveFile();
		}


		////////////////

		public JsonConfig<HonorBoundConfigData> ConfigJson { get; private set; }
		public HonorBoundConfigData Config { get { return this.ConfigJson.Data; } }

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

			this.ConfigJson = new JsonConfig<HonorBoundConfigData>( HonorBoundConfigData.ConfigFileName, ConfigurationDataBase.RelativePath, new HonorBoundConfigData() );
		}

		////////////////

		public override void Load() {
			HonorBoundMod.Instance = this;

			this.LoadConfig();

			if( !Main.dedServ ) {
				this.UI = new HonorBoundUI();
			}
		}

		private void LoadConfig() {
			try {
				if( !this.ConfigJson.LoadFile() ) {
					this.ConfigJson.SaveFile();
				}
			} catch( Exception e ) {
				LogHelpers.Log( e.Message );
				this.ConfigJson.SaveFile();
			}

			if( this.ConfigJson.Data.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Honor Bound updated to " + this.Version.ToString() );
				this.ConfigJson.SaveFile();
			}
		}

		public override void Unload() {
			HonorBoundMod.Instance = null;
		}


		////////////////

		public override void PostSetupContent() {
			if( !Main.dedServ ) {
				this.UI.PostSetupContent();
			}
			
			this.ConfigJson.Data.Enabled = this.IsEnabled();
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
			return this.ConfigJson.Data.Enabled;
		}

		public bool IsDebugInfo() {
			return (this.ConfigJson.Data.DEBUGMODE & 1) > 0;
		}

		public bool IsDebugReset() {
			return (this.ConfigJson.Data.DEBUGMODE & 2) > 0;
		}
	}
}
