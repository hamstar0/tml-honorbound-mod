using Injury;


namespace HonorBound.Honorifics {
	class NimbleHonorificEntry : HonorificEntry {
		public NimbleHonorificEntry() {
			var inj_default = new InjuryConfigData();
			int bleed_time = inj_default.DurationOfBleedingHeart;

			this.Name = "Nimble";
			this.Descriptions = new string[] {
				"Broken Hearts last only "+(bleed_time/(3*60)) +"s before fading (otherwise "+(bleed_time/60)+ "s).",
				"3x more likely to lose max health from damage."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var inj_config = InjuryAPI.GetModSettings();
			var inj_default = new InjuryConfigData();

			inj_config.DurationOfBleedingHeart = inj_default.DurationOfBleedingHeart / 3;
			inj_config.HarmBufferCapacityBeforeReceivingInjury = inj_default.HarmBufferCapacityBeforeReceivingInjury / 3f;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var inj_config = InjuryAPI.GetModSettings();
			var inj_default = new InjuryConfigData();

			inj_config.DurationOfBleedingHeart = inj_default.DurationOfBleedingHeart;
			inj_config.HarmBufferCapacityBeforeReceivingInjury = inj_default.HarmBufferCapacityBeforeReceivingInjury;
		}
	}
}
