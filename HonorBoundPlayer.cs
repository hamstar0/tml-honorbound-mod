using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace HonorBound {
	public class HonorBoundPlayer : ModPlayer {
		private ISet<string> BegunWorldIds;

		public bool HasEnteredWorld { get; private set; }



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

				mymod.UI.ResetOptions();

				this.HasEnteredWorld = true;
				this.OnEnterWorldIfSynced();
			}
		}

		public void OnEnterWorldIfSynced() {
			if( !this.HasEnteredWorld ) { return; }

			var mymod = (HonorBoundMod)this.mod;
			var modworld = mymod.GetModWorld<HonorBoundWorld>();

			if( modworld.Logic.IsHonorBound || modworld.Logic.IsDishonorable ) {
				if( Main.netMode == 0 ) {   // Single
					modworld.Logic.BeginGameModeForLocalPlayer( mymod );
				} else if( Main.netMode == 1 && modworld.HasCorrectID ) {   // Client
					modworld.Logic.BeginGameModeForLocalPlayer( mymod );
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
				tags.Set( "begun_world_id_" + i, guid );
				i++;
			}
			return tags;
		}


		////////////////

		internal bool HasBegunCurrentWorld() {
			var modworld = this.mod.GetModWorld<HonorBoundWorld>();
			if( !modworld.HasCorrectID ) { throw new Exception("Cannot check if game is running for player without world loaded."); }
			if( !this.HasEnteredWorld ) { throw new Exception( "Cannot check if game is running for player if player hasn't joined game." ); }
			
			return this.BegunWorldIds.Contains( modworld.ID );
		}

		internal void Begin() {
			var modworld = this.mod.GetModWorld<HonorBoundWorld>();
			if( !modworld.HasCorrectID ) { throw new Exception( "Cannot begin game for player without world loaded." ); }
			if( !this.HasEnteredWorld ) { throw new Exception( "Cannot begin game if player hasn't joined game." ); }

			this.BegunWorldIds.Add( modworld.ID );
		}
	}
}
