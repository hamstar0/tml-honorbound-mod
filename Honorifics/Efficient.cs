using Durability;


namespace HonorBound.Honorifics {
	class EfficientHonorificEntry : HonorificEntry {
		public EfficientHonorificEntry() {
			this.Name = "Efficient";
			this.Descriptions = new string[] {
				"Items have 1/2 normal durability.",
				"Repairs drain max durability."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var dur_config = DurabilityAPI.GetModSettings();
			var dur_default = new DurabilityConfigData();

			dur_config.GeneralWearAndTearMultiplier = dur_default.GeneralWearAndTearMultiplier;
			dur_config.MaxDurabilityLostPerRepair = dur_default.MaxDurabilityLostPerRepair;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var dur_config = DurabilityAPI.GetModSettings();
			var dur_default = new DurabilityConfigData();

			dur_config.GeneralWearAndTearMultiplier = dur_default.GeneralWearAndTearMultiplier * 2;
			dur_config.MaxDurabilityLostPerRepair = 0;
		}
	}
}
