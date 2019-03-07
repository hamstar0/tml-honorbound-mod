using HamstarHelpers.Components.Config;
using System;


namespace HonorBound {
	public class HonorBoundConfigData : ConfigurationDataBase {
		public static readonly string ConfigFileName = "Honor Bound Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public bool DebugModeInfo = false;
		public bool DebugModeReset = false;

		public bool Enabled = true;



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
	}
}
