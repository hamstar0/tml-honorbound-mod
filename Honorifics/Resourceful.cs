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
			var durConfig = DurabilityAPI.GetModSettings();

			durConfig.CanRepair = false;
			durConfig.CanRepairBroken = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var durConfig = DurabilityAPI.GetModSettings();

			durConfig.CanRepair = true;
			durConfig.CanRepairBroken = true;
		}
	}
}
