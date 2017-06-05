using LosingIsFun;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class GenerousHonorificEntry : HonorificEntry {
		public GenerousHonorificEntry() {
			this.Name = "Generous";
			this.Descriptions = new string[] {
				"NPCs must not live crowded or high above ground."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var lif_default = new LosingIsFun.ConfigurationData();

			lif_mod.Config.Data.MinimumRatioTownNPCSolidBlocks = lif_default.MinimumRatioTownNPCSolidBlocks;
			lif_mod.Config.Data.MinimumTownNpcTileSpacing = lif_default.MinimumTownNpcTileSpacing;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var lif_default = new LosingIsFun.ConfigurationData();

			lif_mod.Config.Data.MinimumRatioTownNPCSolidBlocks = 0;
			lif_mod.Config.Data.MinimumTownNpcTileSpacing = 0;
		}
	}
}
