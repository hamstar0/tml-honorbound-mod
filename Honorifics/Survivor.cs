using HamstarHelpers.Helpers.DebugHelpers;
using Lives;
using Terraria;


namespace HonorBound.Honorifics {
	class SurvivorHonorificEntry : HonorificEntry {
		public SurvivorHonorificEntry() {
			var livDefault = new LivesConfigData();

			this.Name = "Survivor";
			this.Descriptions = new string[] {
				livDefault.InitialLives+" starting lives only (otherwise 10)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var livConfig = LivesAPI.GetModSettings();
			var livDefault = new LivesConfigData();

			livConfig.InitialLives = livDefault.InitialLives;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var livConfig = LivesAPI.GetModSettings();

			livConfig.InitialLives = 10;
		}

		
		public override void BegunWorldOn( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var livConfig = LivesAPI.GetModSettings();
				int baseLives = LivesAPI.GetLives( Main.LocalPlayer );
				
				LivesAPI.AddLives( Main.LocalPlayer, livConfig.InitialLives - baseLives );
			}
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var livConfig = LivesAPI.GetModSettings();
				int baseLives = LivesAPI.GetLives( Main.LocalPlayer );

				LivesAPI.AddLives( Main.LocalPlayer, livConfig.InitialLives - baseLives );
			}
		}
	}
}
