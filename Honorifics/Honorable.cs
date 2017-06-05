using LosingIsFun;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class HonorableHonorificEntry : HonorificEntry {
		public HonorableHonorificEntry() {
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var lif_default = new LosingIsFun.ConfigurationData();
			int evac_time = lif_default.EvacWarpChargeDurationFrames;

			this.Name = "Honorable";
			this.Descriptions = new string[] {
				"Recall/mirror warp requires " +(evac_time/60)+"s warmup delay.",
				"Nurse heals add a stacking debuff."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var lif_default = new LosingIsFun.ConfigurationData();

			lif_mod.Config.Data.EvacWarpChargeDurationFrames = lif_default.EvacWarpChargeDurationFrames;
			lif_mod.Config.Data.SorenessDurationSeconds = lif_default.SorenessDurationSeconds;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var lif_default = new LosingIsFun.ConfigurationData();

			lif_mod.Config.Data.EvacWarpChargeDurationFrames = -1;
			lif_mod.Config.Data.SorenessDurationSeconds = lif_default.SorenessDurationSeconds;
		}
	}
}
