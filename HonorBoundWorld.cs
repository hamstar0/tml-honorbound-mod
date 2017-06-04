using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace HonorBound {
	internal class HonorBoundWorld : ModWorld {
		public HonorBoundLogic Logic;



		public override void Initialize() {
			this.Logic = new HonorBoundLogic();
		}


		public override void Load( TagCompound tags ) {
			if( tags.ContainsKey("is_honor_bound") ) {
				bool is_honor_bound = tags.GetBool( "is_honor_bound" );
				bool has_no_honor = tags.GetBool( "has_no_honor" );
				ISet<string> honorifics = new HashSet<string>();

				foreach( string honorific in HonorBoundLogic.Honorifics.Keys ) {
					if( tags.GetBool("has_" + honorific) ) {
						honorifics.Add( honorific );
					}
				}
				
				this.Logic.Load( (HonorBoundMod)this.mod, is_honor_bound, has_no_honor, honorifics );
			}
		}

		public override TagCompound Save() {
			var tags = new TagCompound {
				{ "is_honor_bound", this.Logic.IsHonorBound},
				{ "has_no_honor", this.Logic.HasNoHonor}
			};
			foreach( string honorific in this.Logic.CurrentActiveHonorifics ) {
				tags.Set( "has_" + honorific, true );
			}
			return tags;
		}


		public override void NetSend( BinaryWriter writer ) {
			writer.Write( (bool)this.Logic.IsHonorBound );
			writer.Write( (bool)this.Logic.HasNoHonor );
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

			bool is_honor_bound = reader.ReadBoolean();
			bool has_no_honor = reader.ReadBoolean();
			int count = reader.ReadInt32();

			for( int i=0; i<count; i++ ) {
				honorifics.Add( reader.ReadString() );
			}
			
			this.Logic.Load( mymod, is_honor_bound, has_no_honor, honorifics );
		}
	}
}
