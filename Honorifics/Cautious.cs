using Injury;
using Lives;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class CautiousHonorificEntry : HonorificEntry {
		public CautiousHonorificEntry() {
			this.Name = "Cautious";
			this.Descriptions = new string[] {
				"Life Crystals need evil biome boss drops to craft.",
				"Cracked Life Crystals require more Vitae.",
				"1-Ups need voodoo dolls to craft."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var inj_config = InjuryAPI.GetModSettings();
			var liv_config = LivesAPI.GetModSettings();
			var inj_default = new InjuryConfigData();

			inj_config.LifeCrystalNeedsEvilBossDrops = true;
			inj_config.VitaePerCrackedLifeCrystal = inj_default.VitaePerCrackedLifeCrystal;
			liv_config.ExtraLifeVoodoo = true;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var inj_config = InjuryAPI.GetModSettings();
			var liv_config = LivesAPI.GetModSettings();
			var inj_default = new InjuryConfigData();

			inj_config.LifeCrystalNeedsEvilBossDrops = false;
			inj_config.VitaePerCrackedLifeCrystal = 2;
			liv_config.ExtraLifeVoodoo = false;
		}
	}
}
