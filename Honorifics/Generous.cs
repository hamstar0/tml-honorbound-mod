using LosingIsFun;


namespace HonorBound.Honorifics {
	class GenerousHonorificEntry : HonorificEntry {
		public GenerousHonorificEntry() {
			this.Name = "Generous";
			this.Descriptions = new string[] {
				"NPCs must not live crowded or high above ground."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lif_config = LosingIsFunAPI.GetModSettings();
			var lif_default = new LosingIsFunConfigData();

			lif_config.MinimumRatioTownNPCSolidBlocks = lif_default.MinimumRatioTownNPCSolidBlocks;
			lif_config.MinimumTownNpcTileSpacing = lif_default.MinimumTownNpcTileSpacing;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lif_config = LosingIsFunAPI.GetModSettings();
			var lif_default = new LosingIsFunConfigData();

			lif_config.MinimumRatioTownNPCSolidBlocks = 0;
			lif_config.MinimumTownNpcTileSpacing = 0;
		}
	}
}
