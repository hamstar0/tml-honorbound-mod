using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace HonorBound {
	public class HonorBoundConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;

		////////////////

		[DefaultValue( false )]
		public bool DebugModeInfo = false;
		[DefaultValue( false )]
		public bool DebugModeReset = false;

		[DefaultValue( true )]
		public bool Enabled = true;
	}
}
