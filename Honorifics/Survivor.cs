using Lives;
using Terraria;


namespace HonorBound.Honorifics {
	class SurvivorHonorificEntry : HonorificEntry {
		public SurvivorHonorificEntry() {
			var liv_default = new LivesConfigData();

			this.Name = "Survivor";
			this.Descriptions = new string[] {
				liv_default.InitialLives+" starting lives only (otherwise 10)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var liv_config = LivesAPI.GetModSettings();
			var liv_default = new LivesConfigData();

			liv_config.InitialLives = liv_default.InitialLives;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var liv_config = LivesAPI.GetModSettings();

			liv_config.InitialLives = 10;
		}

		
		public override void BegunWorldOn( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var liv_config = LivesAPI.GetModSettings();
				int base_lives = LivesAPI.GetLives( Main.LocalPlayer );

				LivesAPI.AddLives( Main.LocalPlayer, liv_config.InitialLives - base_lives );
			}
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var liv_config = LivesAPI.GetModSettings();
				int base_lives = LivesAPI.GetLives( Main.LocalPlayer );

				LivesAPI.AddLives( Main.LocalPlayer, liv_config.InitialLives - base_lives );
			}
		}
	}
}
