using Injury;
using Lives;
using Terraria.ModLoader;


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
			var injConfig = ModContent.GetInstance<InjuryConfig>();
			var livConfig = ModContent.GetInstance<LivesConfig>();

			injConfig.CraftableLifeCrystal = false;
			livConfig.CraftableExtraLives = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var injConfig = ModContent.GetInstance<InjuryConfig>();
			var livConfig = ModContent.GetInstance<LivesConfig>();

			injConfig.CraftableLifeCrystal = true;
			livConfig.CraftableExtraLives = true;
		}
	}
}
