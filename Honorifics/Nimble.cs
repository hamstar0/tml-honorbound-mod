using Injury;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class NimbleHonorificEntry : HonorificEntry {
		public NimbleHonorificEntry() {
			var inj_default = new Injury.ConfigurationData();
			int bleed_time = inj_default.DurationOfBleedingHeart;

			this.Name = "Nimble";
			this.Descriptions = new string[] {
				"Broken Hearts last only "+(bleed_time/(3*60)) +"s before fading (otherwise "+(bleed_time/60)+ "s).",
				"3x more likely to lose max health from damage."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var inj_default = new Injury.ConfigurationData();

			inj_mod.Config.Data.DurationOfBleedingHeart = inj_default.DurationOfBleedingHeart / 3;
			inj_mod.Config.Data.HarmBufferCapacityBeforeReceivingInjury = inj_default.HarmBufferCapacityBeforeReceivingInjury / 3f;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var inj_default = new Injury.ConfigurationData();

			inj_mod.Config.Data.DurationOfBleedingHeart = inj_default.DurationOfBleedingHeart;
			inj_mod.Config.Data.HarmBufferCapacityBeforeReceivingInjury = inj_default.HarmBufferCapacityBeforeReceivingInjury;
		}
	}
}
