using TheLunatic;


namespace HonorBound.Honorifics {
	class ExpedientHonorificEntry : HonorificEntry {
		public ExpedientHonorificEntry() {
			var lun_default = new LunaticConfigData();

			this.Name = "Expedient";
			this.Descriptions = new string[] {
				"Lunatic gives only +1 day apocalypse delay for each mask (otherwise +"+(lun_default.HalfDaysRecoveredPerMask/2f)+" days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var config = LunaticConfigData.GetCurrent();

			config.HalfDaysRecoveredPerMask = 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var config = LunaticConfigData.GetCurrent();
			var lun_default = new LunaticConfigData();

			config.HalfDaysRecoveredPerMask = lun_default.HalfDaysRecoveredPerMask;
		}
	}
}
