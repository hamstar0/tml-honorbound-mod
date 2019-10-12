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
			var injConfig = ModContent.GetInstance<InjuryConfig>();
			var livConfig = ModContent.GetInstance<LivesConfig>();
			var injDefault = new InjuryConfig();

			injConfig.LifeCrystalNeedsEvilBossDrops = true;
			injConfig.VitaePerCrackedLifeCrystal = injDefault.VitaePerCrackedLifeCrystal;
			livConfig.ExtraLifeVoodoo = true;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var injConfig = ModContent.GetInstance<InjuryConfig>();
			var livConfig = ModContent.GetInstance<LivesConfig>();
			var injDefault = new InjuryConfig();

			injConfig.LifeCrystalNeedsEvilBossDrops = false;
			injConfig.VitaePerCrackedLifeCrystal = 2;
			livConfig.ExtraLifeVoodoo = false;
		}
	}
}
