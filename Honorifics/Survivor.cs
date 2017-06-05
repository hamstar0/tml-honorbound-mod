using Lives;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class SurvivorHonorificEntry : HonorificEntry {
		public SurvivorHonorificEntry() {
			var liv_default = new Lives.ConfigurationData();

			this.Name = "Survivor";
			this.Descriptions = new string[] {
				liv_default.InitialLives+" starting lives only (otherwise 10)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var liv_player = Main.LocalPlayer.GetModPlayer<LivesPlayer>( liv_mod );
			var liv_default = new Lives.ConfigurationData();

			liv_mod.Config.Data.InitialLives = liv_default.InitialLives;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var liv_player = Main.LocalPlayer.GetModPlayer<LivesPlayer>( liv_mod );

			liv_mod.Config.Data.InitialLives = 10;
		}

		
		public override void BegunWorldOn( HonorBoundLogic logic ) {
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var liv_player = Main.LocalPlayer.GetModPlayer<LivesPlayer>( liv_mod );

			liv_player.AddLives( liv_mod.Config.Data.InitialLives - liv_player.Lives );
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var liv_player = Main.LocalPlayer.GetModPlayer<LivesPlayer>( liv_mod );

			liv_player.AddLives( liv_mod.Config.Data.InitialLives - liv_player.Lives );
		}
	}
}
