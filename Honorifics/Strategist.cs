using TheLunatic;


namespace HonorBound.Honorifics {
	class StrategistHonorificEntry : HonorificEntry {
		public StrategistHonorificEntry() {
			var lunDefault = new LunaticConfigData();
			float wofTime = lunDefault.WallOfFleshMultiplier * ((float)lunDefault.HalfDaysRecoveredPerMask / 2f);
			float time = lunDefault.DaysUntil;

			this.Name = "Strategist";
			this.Descriptions = new string[] {
				"Lunatic gives 1/2 (+"+(wofTime/2f).ToString("N1")+" days) added time for Wall of Flesh (otherwise +"+wofTime.ToString("N1")+").",
				"Lunatic gives 1/2 ("+(time/2f).ToString("N0")+") warmup time (otherwise "+time.ToString("N0")+" days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var config = TheLunaticAPI.GetModSettings();
			var lunDefault = new LunaticConfigData();

			config.WallOfFleshMultiplier = lunDefault.WallOfFleshMultiplier / 2f;
			config.DaysUntil = lunDefault.DaysUntil / 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var config = TheLunaticAPI.GetModSettings();
			var lunDefault = new LunaticConfigData();

			config.WallOfFleshMultiplier = lunDefault.WallOfFleshMultiplier;
			config.DaysUntil = lunDefault.DaysUntil;
		}
	}
}
