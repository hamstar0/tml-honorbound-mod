using Capitalism;
using Durability;
using HamstarHelpers.Utilities.Messages;
using HonorBound.Honorifics;
using Injury;
using Lives;
using LosingIsFun;
using Stamina;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheLunatic;


namespace HonorBound {
	abstract public class HonorificEntry {
		public string Name { get; protected set; }
		public string[] Descriptions { get; protected set; }

		public virtual void BegunWorldOn( HonorBoundLogic logic ) { }
		public virtual void BegunWorldOff( HonorBoundLogic logic ) { }
		public virtual void LoadOn( HonorBoundLogic logic ) { }
		public virtual void LoadOff( HonorBoundLogic logic ) { }
		public virtual void PostLoadOn( HonorBoundLogic logic ) { }
		public virtual void PostLoadOff( HonorBoundLogic logic ) { }

		public void NotAllowed( HonorBoundLogic logic ) {
			logic.HonorificAllowed[ this.Name ] = false;
			logic.CurrentActiveHonorifics.Remove( this.Name );
		}
	}



	public class HonorBoundLogic {
		public static readonly IDictionary<string, HonorificEntry> Honorifics = new Dictionary<string, HonorificEntry>();

		public IDictionary<string, bool> HonorificAllowed = new Dictionary<string, bool>();
		

		public static IList<string> GetVersionIncompatibilityMessages() {
			var list = new List<string>();
			var dur_ver = new Version( 2, 3, 2 );
			var inj_ver = new Version( 1, 9, 5 );
			var liv_ver = new Version( 1, 5, 4 );
			var sta_ver = new Version( 1, 4, 11 );
			var cap_ver = new Version( 1, 3, 3 );
			var lun_ver = new Version( 1, 2, 5 );
			var lif_ver = new Version( 1, 0, 4 );

			if( DurabilityMod.ConfigVersion != dur_ver ) {
				list.Add( "Honor Bound requires Durability to be version " + dur_ver.ToString() );// + " or newer." );
			}
			if( InjuryMod.ConfigVersion != inj_ver ) {
				list.Add( "Honor Bound requires Injury to be version " + inj_ver.ToString() );// + " or newer." );
			}
			if( LivesMod.ConfigVersion != liv_ver ) {
				list.Add( "Honor Bound requires Lives to be version " + liv_ver.ToString() );// + " or newer." );
			}
			if( StaminaMod.ConfigVersion != sta_ver ) {
				list.Add( "Honor Bound requires Stamina to be version " + sta_ver.ToString() );// + " or newer." );
			}
			if( CapitalismMod.ConfigVersion != cap_ver ) {
				list.Add( "Honor Bound requires Capitalism to be version " + cap_ver.ToString() );// + " or newer." );
			}
			if( TheLunaticMod.ConfigVersion != lun_ver ) {
				list.Add( "Honor Bound requires The Lunatic to be version " + lun_ver.ToString() );// + " or newer." );
			}
			if( LosingIsFunMod.ConfigVersion != lif_ver ) {
				list.Add( "Honor Bound requires Losing Is Fun to be version " + lif_ver.ToString() );// + " or newer." );
			}

			return list;
		}


		static HonorBoundLogic() {
			bool up_to_date = HonorBoundLogic.GetVersionIncompatibilityMessages().Count == 0;
			if( !up_to_date ) { return; }
			
			HonorBoundLogic.DefineHonorific( new DuelistHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new CautiousHonorificEntry() );    // was 'Prudent'
			HonorBoundLogic.DefineHonorific( new BraveHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new SurvivorHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new HonorableHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new NimbleHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new EfficientHonorificEntry() );   // was 'Frugal'
			HonorBoundLogic.DefineHonorific( new ResourcefulHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new GenerousHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new ExpedientHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new StrategistHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new CompletionistHonorificEntry() );
			HonorBoundLogic.DefineHonorific( new ProcrastinatorHonorificEntry() );
		}
		
		private static void DefineHonorific( HonorificEntry entry ) {
			HonorBoundLogic.Honorifics[ entry.Name ] = entry;
		}



		////////////////

		public ISet<string> CurrentActiveHonorifics = new HashSet<string>();
		public bool IsHonorBound { get; private set; }
		public bool IsDishonorable { get; private set; }
		public bool NotPlayingLunatic { get; private set; }



		internal HonorBoundLogic( HonorBoundMod mymod, bool is_honor_bound, bool has_no_honor, ISet<string> honorifics ) {
			bool mods_up_to_date = HonorBoundLogic.GetVersionIncompatibilityMessages().Count == 0;
			
			foreach( var kv in HonorBoundLogic.Honorifics ) {
				this.HonorificAllowed[ kv.Key ] = true;
			}

			if( this.HasBegun() ) {
				throw new Exception("Invalid attempt to reset honorifics.");
			}

			if( !mods_up_to_date || (mymod.Config.Data.DEBUGMODE & 2) != 0 ) {
				this.IsHonorBound = false;
				this.IsDishonorable = false;
				this.CurrentActiveHonorifics = new HashSet<string>();
			} else {
				this.IsHonorBound = is_honor_bound;
				this.IsDishonorable = has_no_honor;
				this.CurrentActiveHonorifics = honorifics;

				if( mymod.NeedsUpdate ) {
					mymod.NeedsUpdate = false;

					if( this.CurrentActiveHonorifics.Remove( "Prudent" ) ) {
						this.CurrentActiveHonorifics.Add( "Cautious" );
					}
					if( this.CurrentActiveHonorifics.Remove( "Frugal" ) ) {
						this.CurrentActiveHonorifics.Add( "Efficient" );
					}
				}

				this.RefreshAllowedHonorifics();
				
				if( is_honor_bound ) {
					this.ForHonor( mymod );
				} else if( has_no_honor ) {
					this.NoHonor( mymod );
				}
			}
		}


		////////////////
		
		public void RefreshAllowedHonorifics() {
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lun_world = lun_mod.GetModWorld<TheLunaticWorld>();

			if( lun_world.GameLogic.HasGameEnded && !this.NotPlayingLunatic ) {
				this.NotPlayingLunatic = true;
				HonorBoundLogic.Honorifics[ "Expedient" ].NotAllowed( this );
				HonorBoundLogic.Honorifics[ "Strategist" ].NotAllowed( this );
				HonorBoundLogic.Honorifics[ "Completionist" ].NotAllowed( this );
			}
		}


		////////////////

		public bool HasBegun() {
			return this.IsHonorBound || this.IsDishonorable;
		}

		internal void EnableMods( bool enable ) {
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var cap_mod = (CapitalismMod)ModLoader.GetMod( "Capitalism" );
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );

			dur_mod.Config.Data.Enabled = enable;
			inj_mod.Config.Data.Enabled = enable;
			liv_mod.Config.Data.Enabled = enable;
			sta_mod.Config.Data.Enabled = enable;
			cap_mod.Config.Data.Enabled = enable;
			lun_mod.Config.Data.Enabled = enable;
			lif_mod.Config.Data.Enabled = enable;
		}

		internal void ForHonor( HonorBoundMod mymod ) {
			var skipped = HonorBoundLogic.Honorifics.Keys.Where( x => !this.CurrentActiveHonorifics.Contains( x ) );

			this.EnableMods( true );

			foreach( string honorific in this.CurrentActiveHonorifics ) {
				HonorBoundLogic.Honorifics[ honorific ].LoadOn( this );
			}
			foreach( string honorific in skipped ) {
				HonorBoundLogic.Honorifics[ honorific ].LoadOff( this );
			}

			this.IsHonorBound = true;
			
			foreach( string honorific in this.CurrentActiveHonorifics ) {
				HonorBoundLogic.Honorifics[honorific].PostLoadOn( this );
			}
			foreach( string honorific in skipped ) {
				HonorBoundLogic.Honorifics[honorific].PostLoadOff( this );
			}

			if( Main.netMode != 2 ) {   // Not server
				try {
					var player = Main.LocalPlayer;
					var modplayer = player.GetModPlayer<HonorBoundPlayer>( mymod );

					if( !modplayer.HasBegunCurrentWorld() ) {
						foreach( string honorific in this.CurrentActiveHonorifics ) {
							HonorBoundLogic.Honorifics[honorific].BegunWorldOn( this );
						}
						foreach( string honorific in skipped ) {
							HonorBoundLogic.Honorifics[honorific].BegunWorldOff( this );
						}

						modplayer.Begin();
					}
				} catch( Exception e ) { }
			}

			this.AnnounceHonorifics();
		}


		internal void NoHonor( HonorBoundMod mymod ) {
			this.EnableMods( false );

			this.IsDishonorable = true;

			if( Main.netMode != 2 ) {   // Not server
				try {
					var player = Main.LocalPlayer;
					var modplayer = player.GetModPlayer<HonorBoundPlayer>( mymod );

					if( !modplayer.HasBegunCurrentWorld() ) {
						modplayer.Begin();
					}
				} catch( Exception e ) { }
			}

			this.AnnounceDishonorable();
		}


		////////////////

		public void AnnounceHonorifics() {
			int i = 0;
			string honorifics_list = "";
			foreach( string honorific in this.CurrentActiveHonorifics ) {
				honorifics_list += honorific;

				if( i < this.CurrentActiveHonorifics.Count - 1 ) {
					honorifics_list += ", ";
					if( i > 0 && i % 5 == 0 ) {
						honorifics_list += '\n';
					}
				}
				i++;
			}
			SimpleMessage.PostMessage( "The following honor codes are now in effect:", honorifics_list, 15 * 60 );
			Main.PlaySound( SoundID.Item47.WithVolume( 0.5f ) );
		}

		public void AnnounceDishonorable() {
			SimpleMessage.PostMessage( "No honor codes in effect. Vanilla only.", "", 10 * 60 );
			//Main.PlaySound( SoundID.NPCDeath20.WithVolume( 0.5f ) );	// Pig Squeal
			//Main.PlaySound( SoundID.NPCDeath59.WithVolume( 0.5f ) );    // Cultist cry
			Main.PlaySound( SoundID.Item16.WithVolume( 0.5f ) );    // Fart
		}
	}
}
