using Stamina;
using Terraria;


namespace HonorBound.Honorifics {
	class DuelistHonorificEntry : HonorificEntry {
		public DuelistHonorificEntry() {
			var sta_default = new StaminaConfigData();

			this.Name = "Duelist";
			this.Descriptions = new string[] {
				"Items use drains stamina.",
				"Only " + sta_default.InitialStamina + " starting stamina (originally " + (sta_default.InitialStamina * 2) + ")."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var sta_config = StaminaAPI.GetModSettings();
			var sta_default = new StaminaConfigData();

			sta_config.ItemUseRate = sta_default.ItemUseRate;
			sta_config.InitialStamina = sta_default.InitialStamina;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var sta_config = StaminaAPI.GetModSettings();
			var sta_default = new StaminaConfigData();

			sta_config.ItemUseRate = 0;
			sta_config.InitialStamina = sta_default.InitialStamina * 2;
		}

		public override void BegunWorldOn( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var sta_config = StaminaAPI.GetModSettings();
				var sta_default = new StaminaConfigData();

				StaminaAPI.AddStamina( Main.LocalPlayer, sta_default.InitialStamina - StaminaAPI.GetStamina( Main.LocalPlayer ) );
			}
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var sta_default = new StaminaConfigData();

				StaminaAPI.AddStamina( Main.LocalPlayer, ( sta_default.InitialStamina * 2) - StaminaAPI.GetStamina( Main.LocalPlayer ) );
			}
		}
	}
}
