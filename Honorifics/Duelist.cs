using Stamina;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound.Honorifics {
	class DuelistHonorificEntry : HonorificEntry {
		public DuelistHonorificEntry() {
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var sta_default = new Stamina.ConfigurationData();

			this.Name = "Duelist";
			this.Descriptions = new string[] {
				"Items use drains stamina.",
				"Only " + sta_default.InitialStamina + " starting stamina (otherwise " + (sta_default.InitialStamina * 2) + ")."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var sta_default = new Stamina.ConfigurationData();
			var sta_player = Main.LocalPlayer.GetModPlayer<StaminaPlayer>( sta_mod );

			sta_mod.Config.Data.ItemUseRate = sta_default.ItemUseRate;
			sta_mod.Config.Data.InitialStamina = sta_default.InitialStamina;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var sta_default = new Stamina.ConfigurationData();
			var sta_player = Main.LocalPlayer.GetModPlayer<StaminaPlayer>( sta_mod );

			sta_mod.Config.Data.ItemUseRate = 0;
			sta_mod.Config.Data.InitialStamina = 200;
		}

		public override void BegunWorldOn( HonorBoundLogic logic ) {
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var sta_default = new Stamina.ConfigurationData();
			var sta_player = Main.LocalPlayer.GetModPlayer<StaminaPlayer>( sta_mod );

			sta_player.AddStamina( sta_default.InitialStamina - sta_player.GetStamina() );
		}

		public override void BegunWorldOff( HonorBoundLogic logic ) {
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var sta_default = new Stamina.ConfigurationData();
			var sta_player = Main.LocalPlayer.GetModPlayer<StaminaPlayer>( sta_mod );

			sta_player.AddStamina( (sta_default.InitialStamina * 2) - sta_player.GetStamina() );
		}
	}
}
