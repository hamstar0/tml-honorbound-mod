using Durability;
using Terraria.ModLoader;


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
			var durConfig = ModLoader.GetMod( "Durability" ).GetConfig<DurabilityConfig>();
			var durDefault = new DurabilityConfig();

			durConfig.GeneralWearAndTearMultiplier = durDefault.GeneralWearAndTearMultiplier;
			durConfig.MaxDurabilityLostPerRepair = durDefault.MaxDurabilityLostPerRepair;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var durConfig = ModLoader.GetMod( "Durability" ).GetConfig<DurabilityConfig>();
			var durDefault = new DurabilityConfig();

			durConfig.GeneralWearAndTearMultiplier = durDefault.GeneralWearAndTearMultiplier * 2;
			durConfig.MaxDurabilityLostPerRepair = 0;
		}
	}
}
