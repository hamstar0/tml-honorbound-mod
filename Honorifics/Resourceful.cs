using Durability;
using Terraria.ModLoader;


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
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );

			dur_mod.Config.Data.CanRepair = false;
			dur_mod.Config.Data.CanRepairBroken = false;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );

			dur_mod.Config.Data.CanRepair = true;
			dur_mod.Config.Data.CanRepairBroken = true;
		}
	}
}
