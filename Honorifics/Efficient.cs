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
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );
			var dur_default = new Durability.ConfigurationData();

			dur_mod.Config.Data.GeneralWearAndTearMultiplier = dur_default.GeneralWearAndTearMultiplier;
			dur_mod.Config.Data.MaxDurabilityLostPerRepair = dur_default.MaxDurabilityLostPerRepair;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );
			var dur_default = new Durability.ConfigurationData();

			dur_mod.Config.Data.GeneralWearAndTearMultiplier = dur_default.GeneralWearAndTearMultiplier * 2;
			dur_mod.Config.Data.MaxDurabilityLostPerRepair = 0;
		}
	}
}
