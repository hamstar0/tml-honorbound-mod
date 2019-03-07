using LosingIsFun;


namespace HonorBound.Honorifics {
	class HonorableHonorificEntry : HonorificEntry {
		public HonorableHonorificEntry() {
			var lifConfig = LosingIsFunAPI.GetModSettings();
			var lifDefault = new LosingIsFunConfigData();
			int evacTime = lifDefault.EvacWarpChargeDurationFrames;

			this.Name = "Honorable";
			this.Descriptions = new string[] {
				"Recall/mirror warp requires " +(evacTime/60)+"s warmup delay.",
				"Nurse heals add a stacking debuff."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lifConfig = LosingIsFunAPI.GetModSettings();
			var lifDefault = new LosingIsFunConfigData();

			lifConfig.EvacWarpChargeDurationFrames = lifDefault.EvacWarpChargeDurationFrames;
			lifConfig.SorenessDurationSeconds = lifDefault.SorenessDurationSeconds;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lifConfig = LosingIsFunAPI.GetModSettings();
			var lifDefault = new LosingIsFunConfigData();

			lifConfig.EvacWarpChargeDurationFrames = -1;
			lifConfig.SorenessDurationSeconds = lifDefault.SorenessDurationSeconds;
		}
	}
}
