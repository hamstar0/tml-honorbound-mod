using TheLunatic;


namespace HonorBound.Honorifics {
	class CompletionistHonorificEntry : HonorificEntry {
		public CompletionistHonorificEntry() {
			this.Name = "Completionist";
			this.Descriptions = new string[] {
				"Lunatic requires every boss mask (not just Moon Lord's)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var config = TheLunaticAPI.GetModSettings();

			config.MoonLordMaskWins = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var config = TheLunaticAPI.GetModSettings();

			config.MoonLordMaskWins = true;
		}
	}
}
