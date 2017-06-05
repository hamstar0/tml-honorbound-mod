using Terraria.ModLoader;
using TheLunatic;


namespace HonorBound.Honorifics {
	class StrategistHonorificEntry : HonorificEntry {
		public StrategistHonorificEntry() {
			this.Name = "Strategist";
			this.Descriptions = new string[] {
				"Lunatic gives 1/2 added time for Wall of Flesh.",
				"Lunatic gives 1/2 warmup time (originally 9 days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lun_default = new TheLunatic.ConfigurationData();

			lun_mod.Config.Data.WallOfFleshMultiplier = lun_default.WallOfFleshMultiplier / 2f;
			lun_mod.Config.Data.DaysUntil = lun_default.DaysUntil / 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lun_default = new TheLunatic.ConfigurationData();

			lun_mod.Config.Data.WallOfFleshMultiplier = lun_default.WallOfFleshMultiplier;
			lun_mod.Config.Data.DaysUntil = lun_default.DaysUntil;
		}
	}
}
