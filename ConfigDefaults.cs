using HamstarHelpers.Components.Config;
using System;


namespace HonorBound {
	public class HonorBoundConfigData : ConfigurationDataBase {
		public static readonly string ConfigFileName = "Honor Bound Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public bool Enabled = true;

		public int DEBUGMODE = 0;   // 1: Info; 2: Reset



		////////////////

		public void SetDefaults() { }


		////////////////

		public bool UpdateToLatestVersion() {
			var mymod = HonorBoundMod.Instance;
			var newConfig = new HonorBoundConfigData();
			newConfig.SetDefaults();

			var versSince = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( versSince >= mymod.Version ) {
				return false;
			}

			if( this.VersionSinceUpdate == "" ) {
				this.SetDefaults();
			}

			this.VersionSinceUpdate = mymod.Version.ToString();

			return true;
		}

		////////////////

		public string _OLD_SETTINGS_BELOW = "";
	}
}
