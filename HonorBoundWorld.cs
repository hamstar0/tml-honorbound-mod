using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace HonorBound {
	class HonorBoundWorld : ModWorld {
		public string ID { get; private set; }
		public bool HasCorrectID { get; private set; }  // Workaround for tml bug?

		public HonorBoundLogic Logic;



		public override void Initialize() {
			this.ID = Guid.NewGuid().ToString( "D" );
			this.HasCorrectID = false;
			this.Logic = new HonorBoundLogic( (HonorBoundMod)this.mod, false, false, new HashSet<string>() );
		}


		public override void Load( TagCompound tag ) {
			if( tag.ContainsKey("is_honor_bound") ) {
				string id = tag.GetString( "world_id" );
				bool is_honor_bound = tag.GetBool( "is_honor_bound" );
				bool has_no_honor = tag.GetBool( "has_no_honor" );
				ISet<string> honorifics = new HashSet<string>();

				foreach( string honorific in HonorBoundLogic.Honorifics.Keys ) {
					if( tag.GetBool("has_" + honorific) ) {
						honorifics.Add( honorific );
					}
				}

				if( id != "" ) {
					this.HasCorrectID = true;
					this.ID = id;
				}
				this.Logic = new HonorBoundLogic( (HonorBoundMod)this.mod, is_honor_bound, has_no_honor, honorifics );
			}
		}

		public override TagCompound Save() {
			var tags = new TagCompound {
				{ "world_id", this.ID },
				{ "is_honor_bound", this.Logic.IsHonorBound},
				{ "has_no_honor", this.Logic.IsDishonorable}
			};
			foreach( string honorific in this.Logic.CurrentActiveHonorifics ) {
				tags.Set( "has_" + honorific, true );
			}
			return tags;
		}


		public override void NetSend( BinaryWriter writer ) {
			writer.Write( (bool)this.HasCorrectID );
			writer.Write( (string)this.ID );

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

			bool is_correct_id = reader.ReadBoolean();
			string id = reader.ReadString();
			bool is_honor_bound = reader.ReadBoolean();
			bool has_no_honor = reader.ReadBoolean();
			int count = reader.ReadInt32();

			for( int i=0; i<count; i++ ) {
				honorifics.Add( reader.ReadString() );
			}

			if( is_correct_id ) {
				this.ID = id;
				this.HasCorrectID = is_correct_id;
				this.Logic = new HonorBoundLogic( mymod, is_honor_bound, has_no_honor, honorifics );

				modplayer.OnEnterWorldIfSynced();
			}
		}
	}
}
