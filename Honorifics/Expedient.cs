using Terraria.ModLoader;
using TheLunatic;


namespace HonorBound.Honorifics {
	class ExpedientHonorificEntry : HonorificEntry {
		public ExpedientHonorificEntry() {
			var lunDefault = new LunaticConfig();

			this.Name = "Expedient";
			this.Descriptions = new string[] {
				"Lunatic gives only +1 day apocalypse delay for each mask (otherwise +"+(lunDefault.HalfDaysRecoveredPerMask/2f)+" days)."
			};
		}


		public override void LoadOn( HonorBoundLogic logic ) {
			var lunConfig = ModContent.GetInstance<LunaticConfig>();

			lunConfig.HalfDaysRecoveredPerMask = 2;
		}

		public override void LoadOff( HonorBoundLogic logic ) {
			var lunConfig = ModContent.GetInstance<LunaticConfig>();
			var lunDefault = new LunaticConfig();

			lunConfig.HalfDaysRecoveredPerMask = lunDefault.HalfDaysRecoveredPerMask;
		}
	}
}
