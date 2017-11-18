using Capitalism;
using Durability;
using HamstarHelpers.DebugHelpers;
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
	abstract class HonorificEntry {
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



	class HonorBoundLogic {
		public static readonly IDictionary<string, HonorificEntry> Honorifics = new Dictionary<string, HonorificEntry>();

		public IDictionary<string, bool> HonorificAllowed = new Dictionary<string, bool>();
		

		////////////////

		static HonorBoundLogic() {
			bool up_to_date = HonorBoundLogic.GetVersionIncompatibilityMessages().Count == 0;
			if( !up_to_date ) { return; }

			try {
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
			} catch( Exception e ) {
				DebugHelpers.Log( e.ToString() );
				throw e;
			}
		}

		private static void DefineHonorific( HonorificEntry entry ) {
			HonorBoundLogic.Honorifics[entry.Name] = entry;
		}

		////////////////

		public static IList<string> GetVersionIncompatibilityMessages() {
			var list = new List<string>();
			var dur_ver = new Version( 2, 5 );
			var inj_ver = new Version( 1, 10 );
			var liv_ver = new Version( 1, 6 );
			var sta_ver = new Version( 1, 5 );
			var cap_ver = new Version( 1, 4 );
			var lun_ver = new Version( 1, 3 );
			var lif_ver = new Version( 1, 2 );
			Version cur_dur_ver = ModLoader.GetMod("Durability").Version;
			Version cur_inj_ver = ModLoader.GetMod( "Injury" ).Version;
			Version cur_liv_ver = ModLoader.GetMod( "Lives" ).Version;
			Version cur_sta_ver = ModLoader.GetMod( "Stamina" ).Version;
			Version cur_cap_ver = ModLoader.GetMod( "Capitalism" ).Version;
			Version cur_lun_ver = ModLoader.GetMod( "TheLunatic" ).Version;
			Version cur_lif_ver = ModLoader.GetMod( "LosingIsFun" ).Version;

			if( cur_dur_ver.Major != dur_ver.Major || cur_dur_ver.Minor != dur_ver.Minor ) {
				list.Add( "Honor Bound requires Durability "+cur_dur_ver.ToString()+" to be at least version " + dur_ver.ToString() );// + " or newer." );
			}
			if( cur_inj_ver.Major != inj_ver.Major || cur_inj_ver.Minor != inj_ver.Minor ) {
				list.Add( "Honor Bound requires Injury " + cur_inj_ver.ToString() + " to be at least version " + inj_ver.ToString() );// + " or newer." );
			}
			if( cur_liv_ver.Major != liv_ver.Major || cur_liv_ver.Minor != liv_ver.Minor ) {
				list.Add( "Honor Bound requires Lives " + cur_liv_ver.ToString() + " to be at least version " + liv_ver.ToString() );// + " or newer." );
			}
			if( cur_sta_ver.Major != sta_ver.Major || cur_sta_ver.Minor != sta_ver.Minor ) {
				list.Add( "Honor Bound requires Stamina " + cur_sta_ver.ToString() + " to be at least version " + sta_ver.ToString() );// + " or newer." );
			}
			if( cur_cap_ver.Major != cap_ver.Major || cur_cap_ver.Minor != cap_ver.Minor ) {
				list.Add( "Honor Bound requires Capitalism " + cur_cap_ver.ToString() + " to be at least version " + cap_ver.ToString() );// + " or newer." );
			}
			if( cur_lun_ver.Major != lun_ver.Major || cur_lun_ver.Minor != lun_ver.Minor ) {
				list.Add( "Honor Bound requires The Lunatic " + cur_lun_ver.ToString() + " to be at least version " + lun_ver.ToString() );// + " or newer." );
			}
			if( cur_lif_ver.Major != lif_ver.Major || cur_lif_ver.Minor != lif_ver.Minor ) {
				list.Add( "Honor Bound requires Losing Is Fun " + cur_lif_ver.ToString() + " to be at least version " + lif_ver.ToString() );// + " or newer." );
			}

			return list;
		}



		////////////////

		public ISet<string> CurrentActiveHonorifics = new HashSet<string>();
		public bool IsHonorBound { get; private set; }
		public bool IsDishonorable { get; private set; }
		public bool NotPlayingLunatic { get; private set; }

		public bool IsGameModeBegun { get; private set; }



		internal HonorBoundLogic( HonorBoundMod mymod, bool is_honor_bound, bool has_no_honor, ISet<string> honorifics ) {
			bool mods_up_to_date = HonorBoundLogic.GetVersionIncompatibilityMessages().Count == 0;
			
			foreach( var kv in HonorBoundLogic.Honorifics ) {
				this.HonorificAllowed[ kv.Key ] = true;
			}

			if( !mods_up_to_date || mymod.IsDebugReset() ) {
				this.IsHonorBound = false;
				this.IsDishonorable = false;
				this.CurrentActiveHonorifics = new HashSet<string>();
			} else {
				this.IsHonorBound = is_honor_bound;
				this.IsDishonorable = has_no_honor;
				this.CurrentActiveHonorifics = honorifics;

				if( mymod.NeedsUpdateBecauseNewModVersion ) {
					mymod.NeedsUpdateBecauseNewModVersion = false;

					if( this.CurrentActiveHonorifics.Remove( "Prudent" ) ) {
						this.CurrentActiveHonorifics.Add( "Cautious" );
					}
					if( this.CurrentActiveHonorifics.Remove( "Frugal" ) ) {
						this.CurrentActiveHonorifics.Add( "Efficient" );
					}
				}

				this.RefreshAllowedHonorifics();
			}

			this.IsGameModeBegun = false;
		}


		////////////////
		
		public void RefreshAllowedHonorifics() {
			if( TheLunaticAPI.HasCurrentGameEnded() && !this.NotPlayingLunatic ) {
				this.NotPlayingLunatic = true;
				HonorBoundLogic.Honorifics[ "Expedient" ].NotAllowed( this );
				HonorBoundLogic.Honorifics[ "Strategist" ].NotAllowed( this );
				HonorBoundLogic.Honorifics[ "Completionist" ].NotAllowed( this );
			}
		}


		////////////////

		internal void EnableMods( bool enable ) {
			DurabilityAPI.GetModSettings().Enabled = enable;
			InjuryAPI.GetModSettings().Enabled = enable;
			LivesAPI.GetModSettings().Enabled = enable;
			StaminaAPI.GetModSettings().Enabled = enable;
			CapitalismAPI.GetModSettings().Enabled = enable;
			TheLunaticAPI.GetModSettings().Enabled = enable;
			LosingIsFunAPI.GetModSettings().Enabled = enable;
		}



		////////////////
		
		internal void BeginGameModeForLocalPlayer( HonorBoundMod mymod ) {
			if( this.IsGameModeBegun ) { throw new Exception( "BeginGameModeForLocalPlayer() - Already begun." ); }

			if( this.IsHonorBound ) {
				this.ApplyHonorifics( mymod );
			} else if( this.IsDishonorable ) {
				this.ApplyDishonorability( mymod );
			}
			this.IsGameModeBegun = true;
		}

		internal void ForHonor() {
			if( this.IsGameModeBegun ) { throw new Exception( "ForHonor() - Already begun." ); }
			this.IsHonorBound = true;
		}

		internal void NoHonor() {
			if( this.IsGameModeBegun ) { throw new Exception( "NoHonor() - Already begun." ); }
			this.IsDishonorable = true;
		}

		////////////////

		private void ApplyHonorifics( HonorBoundMod mymod ) {
			var skipped = HonorBoundLogic.Honorifics.Keys.Where( x => !this.CurrentActiveHonorifics.Contains( x ) );

			this.EnableMods( true );

			foreach( string honorific in this.CurrentActiveHonorifics ) {
				HonorBoundLogic.Honorifics[honorific].LoadOn( this );
			}
			foreach( string honorific in skipped ) {
				HonorBoundLogic.Honorifics[honorific].LoadOff( this );
			}

			foreach( string honorific in this.CurrentActiveHonorifics ) {
				HonorBoundLogic.Honorifics[honorific].PostLoadOn( this );
			}
			foreach( string honorific in skipped ) {
				HonorBoundLogic.Honorifics[honorific].PostLoadOff( this );
			}

			if( Main.netMode != 2 ) {   // Not server
				var player = Main.LocalPlayer;
				var modplayer = player.GetModPlayer<MyPlayer>( mymod );

				if( !modplayer.HasBegunCurrentWorld() ) {
					foreach( string honorific in this.CurrentActiveHonorifics ) {
						HonorBoundLogic.Honorifics[honorific].BegunWorldOn( this );
					}
					foreach( string honorific in skipped ) {
						HonorBoundLogic.Honorifics[honorific].BegunWorldOff( this );
					}

					modplayer.Begin();
				}
			}

			this.AnnounceHonorifics();
		}

		private void ApplyDishonorability( HonorBoundMod mymod ) {
			this.EnableMods( false );

			if( Main.netMode != 2 ) {   // Not server
				var player = Main.LocalPlayer;
				var modplayer = player.GetModPlayer<MyPlayer>( mymod );

				if( !modplayer.HasBegunCurrentWorld() ) {
					modplayer.Begin();
				}
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
