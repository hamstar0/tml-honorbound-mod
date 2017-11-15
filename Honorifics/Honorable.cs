using LosingIsFun;


namespace HonorBound.Honorifics {
	class HonorableHonorificEntry : HonorificEntry {
		public HonorableHonorificEntry() {
			var lif_config = LosingIsFunAPI.GetModSettings();
			var lif_default = new LosingIsFunConfigData();
			int evac_time = lif_default.EvacWarpChargeDurationFrames;

			this.Name = "Honorable";
			this.Descriptions = new string[] {
				"Recall/mirror warp requires " +(evac_time/60)+"s warmup delay.",
				"Nurse heals add a stacking debuff."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lif_config = LosingIsFunAPI.GetModSettings();
			var lif_default = new LosingIsFunConfigData();

			lif_config.EvacWarpChargeDurationFrames = lif_default.EvacWarpChargeDurationFrames;
			lif_config.SorenessDurationSeconds = lif_default.SorenessDurationSeconds;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lif_config = LosingIsFunAPI.GetModSettings();
			var lif_default = new LosingIsFunConfigData();

			lif_config.EvacWarpChargeDurationFrames = -1;
			lif_config.SorenessDurationSeconds = lif_default.SorenessDurationSeconds;
		}
	}
}
