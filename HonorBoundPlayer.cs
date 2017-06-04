using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace HonorBound {
	public class HonorBoundPlayer : ModPlayer {
		public bool HasEnteredWorld { get; private set; }



		public override void Initialize() {
			this.HasEnteredWorld = false;
		}

		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );

			var myclone = (HonorBoundPlayer)clone;
			myclone.HasEnteredWorld = this.HasEnteredWorld;
		}


		public override void OnEnterWorld( Player player ) {
			var mymod = (HonorBoundMod)this.mod;

			if( player.whoAmI == this.player.whoAmI ) { // Current player
				/*if( Main.netMode != 2 ) { // Not server
					if( !mymod.Config.LoadFile() ) {
						mymod.Config.SaveFile();
					}
				}

				if( Main.netMode == 1 ) { // Client
					HonorBoundNetProtocol.RequestSettingsWithClient( mymod, player );
				}*/

				foreach( string err in HonorBoundLogic.GetVersionIncompatibilityMessages() ) {
					Main.NewText( err, Color.Gray );
				}

				this.HasEnteredWorld = true;
			}
		}

		////////////////

		/*public override void Load( TagCompound tags ) {
			//if( tags.ContainsKey("lives") ) {
			//}
		}

		public override TagCompound Save() {
			var tags = new TagCompound { };
			return tags;
		}*/
	}
}
