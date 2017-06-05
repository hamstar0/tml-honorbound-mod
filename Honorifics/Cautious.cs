using Injury;
using Lives;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class CautiousHonorificEntry : HonorificEntry {
		public CautiousHonorificEntry() {
			this.Name = "Cautious";
			this.Descriptions = new string[] {
				"Life Crystals need evil biome boss drops to craft.",
				"Cracked Life Crystals require more Broken Hearts.",
				"1-Ups need voodoo dolls to craft."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var inj_default = new Injury.ConfigurationData();

			inj_mod.Config.Data.LifeCrystalNeedsEvilBossDrops = true;
			inj_mod.Config.Data.BrokenHeartsPerCrackedLifeCrystal = inj_default.BrokenHeartsPerCrackedLifeCrystal;
			liv_mod.Config.Data.ExtraLifeVoodoo = true;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var inj_default = new Injury.ConfigurationData();

			inj_mod.Config.Data.LifeCrystalNeedsEvilBossDrops = false;
			inj_mod.Config.Data.BrokenHeartsPerCrackedLifeCrystal = 2;
			liv_mod.Config.Data.ExtraLifeVoodoo = false;
		}
	}
}
