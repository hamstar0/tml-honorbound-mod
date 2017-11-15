using Injury;
using Lives;


namespace HonorBound.Honorifics {
	class BraveHonorificEntry : HonorificEntry {
		public BraveHonorificEntry() {
			this.Name = "Brave";
			this.Descriptions = new string[] {
				"No craftable Life Crystals or 1-Ups.",
				"Overriding."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var inj_config = InjuryAPI.GetModSettings();
			var liv_config = LivesAPI.GetModSettings();

			inj_config.CraftableLifeCrystal = false;
			liv_config.CraftableExtraLives = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var inj_config = InjuryAPI.GetModSettings();
			var liv_config = LivesAPI.GetModSettings();

			inj_config.CraftableLifeCrystal = true;
			liv_config.CraftableExtraLives = true;
		}
	}
}
