using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace HonorBound {
	class HonorBoundWorld : ModWorld {
		public HonorBoundLogic Logic;



		////////////////

		public override void Initialize() {
			this.Logic = new HonorBoundLogic( false, false, new HashSet<string>() );
		}


		public override void Load( TagCompound tag ) {
			if( tag.ContainsKey("is_honor_bound") ) {
				bool isHonorBound = tag.GetBool( "is_honor_bound" );
				bool hasNoHonor = tag.GetBool( "has_no_honor" );
				ISet<string> honorifics = new HashSet<string>();

				foreach( string honorific in HonorBoundLogic.Honorifics.Keys ) {
					if( tag.GetBool("has_" + honorific) ) {
						honorifics.Add( honorific );
					}
				}
				
				this.Logic = new HonorBoundLogic( isHonorBound, hasNoHonor, honorifics );
			}
		}

		public override TagCompound Save() {
			var tags = new TagCompound {
				{ "is_honor_bound", this.Logic.IsHonorBound},
				{ "has_no_honor", this.Logic.IsDishonorable}
			};
			foreach( string honorific in this.Logic.CurrentActiveHonorifics ) {
				tags[ "has_" + honorific ] = true;
			}
			return tags;
		}


		public override void NetSend( BinaryWriter writer ) {
			writer.Write( (bool)this.Logic.IsHonorBound );
			writer.Write( (bool)this.Logic.IsDishonorable );
			writer.Write( (int)this.Logic.CurrentActiveHonorifics.Count );
			foreach( string honorific in this.Logic.CurrentActiveHonorifics ) {
				writer.Write( honorific );
			}
		}

		public override void NetReceive( BinaryReader reader ) {
			var mymod = (HonorBoundMod)this.mod;
			var modplayer = Main.LocalPlayer.GetModPlayer<HonorBoundPlayer>( mymod );
			if( !modplayer.HasEnteredWorld ) { return; }

			ISet<string> honorifics = new HashSet<string>();
			
			bool isHonorBound = reader.ReadBoolean();
			bool hasNoHonor = reader.ReadBoolean();
			int count = reader.ReadInt32();

			for( int i=0; i<count; i++ ) {
				honorifics.Add( reader.ReadString() );
			}
			
			this.Logic = new HonorBoundLogic( isHonorBound, hasNoHonor, honorifics );

			modplayer.OnEnterWorldIfSynced();
		}
	}
}
