using HamstarHelpers.Components.Config;
using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.TmlHelpers.ModHelpers;
using HonorBound.NetProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace HonorBound {
	partial class HonorBoundMod : Mod {
		public static HonorBoundMod Instance { get; private set; }



		////////////////

		public JsonConfig<HonorBoundConfigData> ConfigJson { get; private set; }
		public HonorBoundConfigData Config => this.ConfigJson.Data;

		public HonorBoundUI UI = null;
		private int LastSeenScreenWidth = -1;
		private int LastSeenScreenHeight = -1;

		internal bool NeedsUpdateBecauseNewModVersion = false;



		////////////////

		public HonorBoundMod() {
			this.ConfigJson = new JsonConfig<HonorBoundConfigData>(
				HonorBoundConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath,
				new HonorBoundConfigData()
			);
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

		public override void HandlePacket( BinaryReader reader, int playerWho ) {
			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.HandlePacket( reader );
			} else if( Main.netMode == 2 ) {    // Server
				ServerPacketHandlers.HandlePacket( reader, playerWho );
			}
		}

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(HonorBoundAPI), args );
		}

		////////////////

		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			if( !this.IsEnabled() ) { return; }

			var myWorld = this.GetModWorld<HonorBoundWorld>();
			var myLogic = myWorld.Logic;
			
			if( !myLogic.IsGameModeBegun ) {
				myLogic.RefreshAllowedHonorifics();

				int idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Mouse Text" ) );
				if( idx != -1 ) {
					var interface_layer = new LegacyGameInterfaceLayer( "HonorBound: Honorific Picker",
						delegate {
							this.UI.RefreshAllowedOptions( myLogic );

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
	}
}
