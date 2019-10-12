using LosingIsFun;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class HonorableHonorificEntry : HonorificEntry {
		public HonorableHonorificEntry() {
			var lifConfig = ModContent.GetInstance<LosingIsFunConfig>();
			var lifDefault = new LosingIsFunConfig();
			int evacTime = lifDefault.EvacWarpChargeDurationFrames;

			this.Name = "Honorable";
			this.Descriptions = new string[] {
				"Recall/mirror warp requires " +(evacTime/60)+"s warmup delay.",
				"Nurse heals add a stacking debuff."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lifConfig = ModContent.GetInstance<LosingIsFunConfig>();
			var lifDefault = new LosingIsFunConfig();

			lifConfig.EvacWarpChargeDurationFrames = lifDefault.EvacWarpChargeDurationFrames;
			lifConfig.SorenessDurationSeconds = lifDefault.SorenessDurationSeconds;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lifConfig = ModContent.GetInstance<LosingIsFunConfig>();
			var lifDefault = new LosingIsFunConfig();

			lifConfig.EvacWarpChargeDurationFrames = -1;
			lifConfig.SorenessDurationSeconds = lifDefault.SorenessDurationSeconds;
		}
	}
}
