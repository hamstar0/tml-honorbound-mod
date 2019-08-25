using Lives;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class SurvivorHonorificEntry : HonorificEntry {
		public SurvivorHonorificEntry() {
			var livDefault = new LivesConfig();

			this.Name = "Survivor";
			this.Descriptions = new string[] {
				livDefault.InitialLives+" starting lives only (otherwise 10)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var livConfig = ModLoader.GetMod("Lives").GetConfig<LivesConfig>();
			var livDefault = new LivesConfig();

			livConfig.InitialLives = livDefault.InitialLives;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var livConfig = ModLoader.GetMod( "Lives" ).GetConfig<LivesConfig>();

			livConfig.InitialLives = 10;
		}

		
		public override void BegunWorldOn( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var livConfig = ModLoader.GetMod( "Lives" ).GetConfig<LivesConfig>();
				int baseLives = LivesAPI.GetLives( Main.LocalPlayer );
				
				LivesAPI.AddLives( Main.LocalPlayer, livConfig.InitialLives - baseLives );
			}
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			if( Main.netMode != 2 ) {
				var livConfig = ModLoader.GetMod( "Lives" ).GetConfig<LivesConfig>();
				int baseLives = LivesAPI.GetLives( Main.LocalPlayer );

				LivesAPI.AddLives( Main.LocalPlayer, livConfig.InitialLives - baseLives );
			}
		}
	}
}
