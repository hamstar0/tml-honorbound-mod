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
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );

			inj_mod.Config.Data.CraftableLifeCrystal = false;
			liv_mod.Config.Data.CraftableExtraLives = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );

			inj_mod.Config.Data.CraftableLifeCrystal = true;
			liv_mod.Config.Data.CraftableExtraLives = true;
		}
	}
}
