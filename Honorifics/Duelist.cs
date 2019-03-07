using HamstarHelpers.Helpers.DebugHelpers;
using Stamina;
using Terraria;


namespace HonorBound.Honorifics {
	class DuelistHonorificEntry : HonorificEntry {
		public DuelistHonorificEntry() {
			var staDefault = new StaminaConfigData();

			this.Name = "Duelist";
			this.Descriptions = new string[] {
				"Items use drains stamina.",
				"Only " + staDefault.InitialStamina + " starting stamina (originally " + (staDefault.InitialStamina * 2) + ")."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var staConfig = StaminaAPI.GetModSettings();
			var staDefault = new StaminaConfigData();

			staConfig.ItemUseRate = staDefault.ItemUseRate;
			staConfig.InitialStamina = staDefault.InitialStamina;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var staConfig = StaminaAPI.GetModSettings();
			var staDefault = new StaminaConfigData();

			staConfig.ItemUseRate = 0;
			staConfig.InitialStamina = staDefault.InitialStamina * 2;
		}

		public override void BegunWorldOn( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var staConfig = StaminaAPI.GetModSettings();
				var staDefault = new StaminaConfigData();
				
				StaminaAPI.AddStamina( Main.LocalPlayer, staDefault.InitialStamina - StaminaAPI.GetStamina( Main.LocalPlayer ) );
			}
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var staDefault = new StaminaConfigData();

				StaminaAPI.AddStamina( Main.LocalPlayer, ( staDefault.InitialStamina * 2) - StaminaAPI.GetStamina( Main.LocalPlayer ) );
			}
		}
	}
}
