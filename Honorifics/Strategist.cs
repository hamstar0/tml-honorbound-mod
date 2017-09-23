using TheLunatic;


namespace HonorBound.Honorifics {
	class StrategistHonorificEntry : HonorificEntry {
		public StrategistHonorificEntry() {
			var lun_default = new LunaticConfigData();
			float wof_time = lun_default.WallOfFleshMultiplier * ((float)lun_default.HalfDaysRecoveredPerMask / 2f);
			float time = lun_default.DaysUntil;

			this.Name = "Strategist";
			this.Descriptions = new string[] {
				"Lunatic gives 1/2 (+"+(wof_time/2f).ToString("N1")+" days) added time for Wall of Flesh (otherwise +"+wof_time.ToString("N1")+").",
				"Lunatic gives 1/2 ("+(time/2f).ToString("N0")+") warmup time (otherwise "+time.ToString("N0")+" days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var config = LunaticConfigData.GetCurrent();
			var lun_default = new LunaticConfigData();

			config.WallOfFleshMultiplier = lun_default.WallOfFleshMultiplier / 2f;
			config.DaysUntil = lun_default.DaysUntil / 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var config = LunaticConfigData.GetCurrent();
			var lun_default = new LunaticConfigData();

			config.WallOfFleshMultiplier = lun_default.WallOfFleshMultiplier;
			config.DaysUntil = lun_default.DaysUntil;
		}
	}
}
