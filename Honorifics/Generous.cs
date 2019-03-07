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
			var lifConfig = LosingIsFunAPI.GetModSettings();
			var lifDefault = new LosingIsFunConfigData();

			lifConfig.MinimumRatioTownNPCSolidBlocks = lifDefault.MinimumRatioTownNPCSolidBlocks;
			lifConfig.MinimumTownNpcTileSpacing = lifDefault.MinimumTownNpcTileSpacing;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lifConfig = LosingIsFunAPI.GetModSettings();
			var lifDefault = new LosingIsFunConfigData();

			lifConfig.MinimumRatioTownNPCSolidBlocks = 0;
			lifConfig.MinimumTownNpcTileSpacing = 0;
		}
	}
}
