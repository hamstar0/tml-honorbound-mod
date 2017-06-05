using Terraria.ModLoader;
using TheLunatic;


namespace HonorBound.Honorifics {
	class ExpedientHonorificEntry : HonorificEntry {
		public ExpedientHonorificEntry() {
			var lun_default = new TheLunatic.ConfigurationData();

			this.Name = "Expedient";
			this.Descriptions = new string[] {
				"Lunatic gives only +1 day apocalypse delay for each mask (otherwise +"+(lun_default.HalfDaysRecoveredPerMask/2f)+" days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

			lun_mod.Config.Data.HalfDaysRecoveredPerMask = 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lun_default = new TheLunatic.ConfigurationData();

			lun_mod.Config.Data.HalfDaysRecoveredPerMask = lun_default.HalfDaysRecoveredPerMask;
		}
	}
}
