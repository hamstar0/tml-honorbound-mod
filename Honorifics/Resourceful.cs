using Durability;


namespace HonorBound.Honorifics {
	class ResourcefulHonorificEntry : HonorificEntry {
		public ResourcefulHonorificEntry() {
			this.Name = "Resourceful";
			this.Descriptions = new string[] {
				"No durability repairs.",
				"Overriding."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var dur_config = DurabilityAPI.GetModSettings();

			dur_config.CanRepair = false;
			dur_config.CanRepairBroken = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var dur_config = DurabilityAPI.GetModSettings();

			dur_config.CanRepair = true;
			dur_config.CanRepairBroken = true;
		}
	}
}
