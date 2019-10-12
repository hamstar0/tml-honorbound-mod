using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace HonorBound {
	class HonorBoundPlayer : ModPlayer {
		private ISet<string> BegunWorldIds;

		////////////////

		public override bool CloneNewInstances => false;

		public bool HasEnteredWorld { get; private set; }



		////////////////

		public override void Initialize() {
			this.BegunWorldIds = new HashSet<string>();
			this.HasEnteredWorld = false;
		}
		
		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );

			var myclone = (HonorBoundPlayer)clone;
			myclone.BegunWorldIds = this.BegunWorldIds;
			myclone.HasEnteredWorld = this.HasEnteredWorld;
		}

		////////////////

		public override void OnEnterWorld( Player player ) {
			if( player.whoAmI != Main.myPlayer ) { return; }
			if( this.player.whoAmI != Main.myPlayer ) { return; }

			var mymod = (HonorBoundMod)this.mod;
			
			/*if( Main.netMode != 2 ) { // Not server
				if( !mymod.Config.LoadFile() ) {
					mymod.Config.SaveFile();
				}
			}

			if( Main.netMode == 1 ) { // Client
				HonorBoundNetProtocol.RequestSettingsWithClient( mymod, player );
			}*/

			mymod.UI.ResetOptions();

			this.HasEnteredWorld = true;
			this.OnEnterWorldIfSynced();
		}

		public void OnEnterWorldIfSynced() {
			if( !this.HasEnteredWorld ) { return; }

			var mymod = (HonorBoundMod)this.mod;
			var modworld = ModContent.GetInstance<HonorBoundWorld>();

			if( !mymod.IsEnabled() ) {
				Main.NewText( "Honor Bound disabled (if unexpected, see log.txt for information).", Color.Gray );
				return;
			}

			if( modworld.Logic.IsHonorBound || modworld.Logic.IsDishonorable ) {
				if( Main.netMode == 0 ) {   // Single
					modworld.Logic.BeginGameModeForLocalPlayer();
				} else if( Main.netMode == 1 ) {   // Client
					modworld.Logic.BeginGameModeForLocalPlayer();
				}
			}
		}

		////////////////

		public override void Load( TagCompound tags ) {
			if( tags.ContainsKey( "begun_worlds_count" ) ) {
				this.BegunWorldIds = new HashSet<string>();

				int count = tags.GetInt( "begun_worlds_count" );
				for( int i = 0; i < count; i++ ) {
					this.BegunWorldIds.Add( tags.GetString( "begun_world_id_" + i ) );
				}
			}
		}

		public override TagCompound Save() {
			var tags = new TagCompound { { "begun_worlds_count", this.BegunWorldIds.Count } };
			int i = 0;

			foreach( string guid in this.BegunWorldIds ) {
				tags[ "begun_world_id_" + i ] = guid;
				i++;
			}
			return tags;
		}


		////////////////

		internal bool HasBegunCurrentWorld() {
			if( !this.HasEnteredWorld ) { throw new ModHelpersException( "Cannot check if game is running for player if player hasn't joined game." ); }
			
			return this.BegunWorldIds.Contains( WorldHelpers.GetUniqueIdForCurrentWorld( true) );
		}

		internal void Begin() {
			if( !this.HasEnteredWorld ) { throw new ModHelpersException( "Cannot begin game if player hasn't joined game." ); }

			this.BegunWorldIds.Add( WorldHelpers.GetUniqueIdForCurrentWorld(true) );
		}
	}
}
