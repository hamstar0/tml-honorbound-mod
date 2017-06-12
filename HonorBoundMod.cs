using HamstarHelpers.Utilities.Config;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace HonorBound {
	public class ConfigurationData {
		public string VersionSinceUpdate = "";

		public int DEBUGMODE = 0;	// 1: Info; 2: Reset
	}


	public class HonorBoundMod : Mod {
		public static readonly Version ConfigVersion = new Version( 1, 0, 2 );
		public JsonConfig<ConfigurationData> Config { get; private set; }

		public HonorBoundUI UI = null;
		private int LastSeenScreenWidth = -1;
		private int LastSeenScreenHeight = -1;

		internal bool NeedsUpdateBecauseNewModVersion = false;
		


		public HonorBoundMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			string filename = "Honor Bound Config.json";
			this.Config = new JsonConfig<ConfigurationData>( filename, "Mod Configs", new ConfigurationData() );
		}

		public override void Load() {
			if( !this.Config.LoadFile() ) {
				this.Config.SaveFile();
			} else {
				Version vers_since = this.Config.Data.VersionSinceUpdate != "" ?
					new Version( this.Config.Data.VersionSinceUpdate ) :
					new Version();

				if( vers_since < HonorBoundMod.ConfigVersion ) {
					ErrorLogger.Log( "Honor Bound config updated to " + HonorBoundMod.ConfigVersion.ToString() );
					this.NeedsUpdateBecauseNewModVersion = true;

					this.Config.Data.VersionSinceUpdate = HonorBoundMod.ConfigVersion.ToString();
					this.Config.SaveFile();
				}
			}

			if( !Main.dedServ ) {
				this.UI = new HonorBoundUI();
			}
		}

		public override void PostSetupContent() {
			if( !Main.dedServ ) {
				this.UI.PostSetupContent();
			}
		}


		////////////////

		public override void HandlePacket( BinaryReader reader, int whoAmI ) {
			HonorBoundNetProtocol.RoutePacket( this, reader );
		}

		////////////////

		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
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
	}
}
