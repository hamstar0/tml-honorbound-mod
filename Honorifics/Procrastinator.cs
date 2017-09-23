using Terraria.ModLoader;
using TheLunatic;


namespace HonorBound.Honorifics {
	class ProcrastinatorHonorificEntry : HonorificEntry {
		public ProcrastinatorHonorificEntry() {
			this.Name = "Procrastinator";
			this.Descriptions = new string[] {
				"Lunatic gives 5x warmup time.",
				"Lunatic gives 5x mask time."
			};
		}


		public override void PostLoadOn( HonorBoundLogic logic ) {
			var config = LunaticConfigData.GetCurrent();

			config.DaysUntil *= 5;
			config.HalfDaysRecoveredPerMask *= 5;
		}
	}
}
