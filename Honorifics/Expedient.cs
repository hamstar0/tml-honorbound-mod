using TheLunatic;


namespace HonorBound.Honorifics {
	class ExpedientHonorificEntry : HonorificEntry {
		public ExpedientHonorificEntry() {
			var lunDefault = new LunaticConfigData();

			this.Name = "Expedient";
			this.Descriptions = new string[] {
				"Lunatic gives only +1 day apocalypse delay for each mask (otherwise +"+(lunDefault.HalfDaysRecoveredPerMask/2f)+" days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var config = TheLunaticAPI.GetModSettings();

			config.HalfDaysRecoveredPerMask = 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var config = TheLunaticAPI.GetModSettings();
			var lunDefault = new LunaticConfigData();

			config.HalfDaysRecoveredPerMask = lunDefault.HalfDaysRecoveredPerMask;
		}
	}
}
