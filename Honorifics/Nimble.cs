using Injury;


namespace HonorBound.Honorifics {
	class NimbleHonorificEntry : HonorificEntry {
		public NimbleHonorificEntry() {
			var injDefault = new InjuryConfigData();
			int bleedTime = injDefault.DurationOfBleedingHeart;

			this.Name = "Nimble";
			this.Descriptions = new string[] {
				"Broken Hearts last only "+(bleedTime/(3*60)) +"s before fading (otherwise "+(bleedTime/60)+ "s).",
				"3x more likely to lose max health from damage."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var injConfig = InjuryAPI.GetModSettings();
			var injDefault = new InjuryConfigData();

			injConfig.DurationOfBleedingHeart = injDefault.DurationOfBleedingHeart / 3;
			injConfig.HarmBufferCapacityBeforeReceivingInjury = injDefault.HarmBufferCapacityBeforeReceivingInjury / 3f;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var injConfig = InjuryAPI.GetModSettings();
			var injDefault = new InjuryConfigData();

			injConfig.DurationOfBleedingHeart = injDefault.DurationOfBleedingHeart;
			injConfig.HarmBufferCapacityBeforeReceivingInjury = injDefault.HarmBufferCapacityBeforeReceivingInjury;
		}
	}
}
