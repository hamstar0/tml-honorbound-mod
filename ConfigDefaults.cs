using HamstarHelpers.Utilities.Config;
using System;


namespace HonorBound {
	public class HonorBoundConfigData : ConfigurationDataBase {
		public static readonly Version ConfigVersion = new Version( 1, 0, 2 );
		public static readonly string ConfigFileName = "Honor Bound Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public bool Enabled = true;

		public int DEBUGMODE = 0;   // 1: Info; 2: Reset



		////////////////

		public bool UpdateToLatestVersion() {
			var new_config = new HonorBoundConfigData();
			var vers_since = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( vers_since >= HonorBoundConfigData.ConfigVersion ) {
				return false;
			}
			
			this.VersionSinceUpdate = HonorBoundConfigData.ConfigVersion.ToString();

			return true;
		}

		////////////////

		public string _OLD_SETTINGS_BELOW = "";
	}
}
