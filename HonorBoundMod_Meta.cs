using HamstarHelpers.Components.Config;
using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DebugHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound {
	partial class HonorBoundMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-honorbound-mod";

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

			var newConfig = new HonorBoundConfigData();
			newConfig.SetDefaults();

			HonorBoundMod.Instance.ConfigJson.SetData( newConfig );
			HonorBoundMod.Instance.ConfigJson.SaveFile();
		}
	}
}
