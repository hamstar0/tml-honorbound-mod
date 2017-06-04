using Capitalism;
using Durability;
using HamstarHelpers.DisplayHelpers;
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
	public class HonorBoundLogic {
		public static bool AllModsUpToDate { get; private set; }
		public static readonly IDictionary<string, string[]> Honorifics = new Dictionary<string, string[]>();
		public static readonly IDictionary<string, bool> HonorificAllowed = new Dictionary<string, bool>();
		private static readonly IDictionary<string, Action<HonorBoundLogic>> HonorificAdd = new Dictionary<string, Action<HonorBoundLogic>>();
		private static readonly IDictionary<string, Action<HonorBoundLogic>> HonorificSkip = new Dictionary<string, Action<HonorBoundLogic>>();
		
		public ISet<string> CurrentActiveHonorifics = new HashSet<string>();
		public bool IsHonorBound { get; private set; }
		public bool HasNoHonor { get; private set; }




		public static IList<string> GetVersionIncompatibilityMessages() {
			var list = new List<string>();
			var dur_ver = new Version( 2, 3, 1 );
			var inj_ver = new Version( 1, 9, 4 );
			var liv_ver = new Version( 1, 5, 3 );
			var sta_ver = new Version( 1, 4, 10 );
			var cap_ver = new Version( 1, 3, 2 );
			var lun_ver = new Version( 1, 2, 4 );
			var lif_ver = new Version( 1, 0, 3 );

			if( DurabilityMod.ConfigVersion < dur_ver ) {
				list.Add( "Honor Bound requires Durability to be version " + dur_ver.ToString() + " or newer." );
			}
			if( InjuryMod.ConfigVersion < inj_ver ) {
				list.Add( "Honor Bound requires Injury to be version " + inj_ver.ToString() + " or newer." );
			}
			if( LivesMod.ConfigVersion < liv_ver ) {
				list.Add( "Honor Bound requires Lives to be version " + liv_ver.ToString() + " or newer." );
			}
			if( StaminaMod.ConfigVersion < sta_ver ) {
				list.Add( "Honor Bound requires Stamina to be version " + sta_ver.ToString() + " or newer." );
			}
			if( CapitalismMod.ConfigVersion < cap_ver ) {
				list.Add( "Honor Bound requires Capitalism to be version " + cap_ver.ToString() + " or newer." );
			}
			if( TheLunaticMod.ConfigVersion < lun_ver ) {
				list.Add( "Honor Bound requires The Lunatic to be version " + lun_ver.ToString() + " or newer." );
			}
			if( LosingIsFunMod.ConfigVersion < lif_ver ) {
				list.Add( "Honor Bound requires Losing Is Fun to be version " + lif_ver.ToString() + " or newer." );
			}

			return list;
		}


		static HonorBoundLogic() {
			HonorBoundLogic.AllModsUpToDate = HonorBoundLogic.GetVersionIncompatibilityMessages().Count == 0;
			if( !HonorBoundLogic.AllModsUpToDate ) { return; }

			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var cap_mod = (CapitalismMod)ModLoader.GetMod( "Capitalism" );
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var dur_default = new Durability.ConfigurationData();
			var inj_default = new Injury.ConfigurationData();
			var liv_default = new Lives.ConfigurationData();
			var sta_default = new Stamina.ConfigurationData();
			var cap_default = new Capitalism.ConfigurationData();
			var lun_default = new TheLunatic.ConfigurationData();
			var lif_default = new LosingIsFun.ConfigurationData();


			HonorBoundLogic.DefineHonorific( "Duelist", new string[] {
					"Items use drains stamina.",
					"Only " + sta_default.InitialStamina + " starting stamina (originally " + (sta_default.InitialStamina * 2) + ")."
				}, delegate ( HonorBoundLogic logic ) {
					sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
					var sta_player = Main.LocalPlayer.GetModPlayer<StaminaPlayer>( sta_mod );

					sta_mod.Config.Data.ItemUseRate = sta_default.ItemUseRate;
					sta_mod.Config.Data.InitialStamina = sta_default.InitialStamina;
					if( !logic.HasBegin() ) {
						sta_player.AddStamina( sta_default.InitialStamina - sta_player.GetStamina() );
					}
				}, delegate ( HonorBoundLogic logic ) {
					sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
					var sta_player = Main.LocalPlayer.GetModPlayer<StaminaPlayer>( sta_mod );

					sta_mod.Config.Data.ItemUseRate = 0;
					sta_mod.Config.Data.InitialStamina = 200;
					if( !logic.HasBegin() ) {
						sta_player.AddStamina( (sta_default.InitialStamina * 2) - sta_player.GetStamina() );
					}
				} );

			HonorBoundLogic.DefineHonorific( "Prudent", new string[] {
					"Life Crystals need evil biome boss drops to craft.",
					"Cracked Life Crystals require more Broken Hearts.",
					"1-Ups need voodoo dolls to craft."
				}, delegate ( HonorBoundLogic logic ) {
					inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
					liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );

					inj_mod.Config.Data.LifeCrystalNeedsEvilBossDrops = true;
					inj_mod.Config.Data.BrokenHeartsPerCrackedLifeCrystal = inj_default.BrokenHeartsPerCrackedLifeCrystal;
					liv_mod.Config.Data.ExtraLifeVoodoo = true;
				}, delegate ( HonorBoundLogic logic ) {
					inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
					liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );

					inj_mod.Config.Data.LifeCrystalNeedsEvilBossDrops = false;
					inj_mod.Config.Data.BrokenHeartsPerCrackedLifeCrystal = 2;
					liv_mod.Config.Data.ExtraLifeVoodoo = false;
				} );

			HonorBoundLogic.DefineHonorific( "Brave", new string[] {
					"No craftable Life Crystals or 1-Ups.",
					"Overriding."
				}, delegate ( HonorBoundLogic logic ) {
					inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
					liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );

					inj_mod.Config.Data.CraftableLifeCrystal = false;
					liv_mod.Config.Data.CraftableExtraLives = false;
				}, delegate ( HonorBoundLogic logic ) {
					inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
					liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );

					inj_mod.Config.Data.CraftableLifeCrystal = true;
					liv_mod.Config.Data.CraftableExtraLives = true;
				} );

			HonorBoundLogic.DefineHonorific( "Survivor", new string[] {
					liv_default.InitialLives+" starting lives only (originally 10)."
				}, delegate ( HonorBoundLogic logic ) {
					liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
					var liv_player = Main.LocalPlayer.GetModPlayer<LivesPlayer>( liv_mod );

					liv_mod.Config.Data.InitialLives = liv_default.InitialLives;
					if( !logic.HasBegin() ) {
						liv_player.AddLives( liv_mod.Config.Data.InitialLives - liv_player.Lives );
					}
				}, delegate ( HonorBoundLogic logic ) {
					liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
					var liv_player = Main.LocalPlayer.GetModPlayer<LivesPlayer>( liv_mod );

					liv_mod.Config.Data.InitialLives = 10;
					if( !logic.HasBegin() ) {
						liv_player.AddLives( liv_mod.Config.Data.InitialLives - liv_player.Lives );
					}
				} );

			int evac_time = lif_default.EvacWarpChargeDurationFrames;
			HonorBoundLogic.DefineHonorific( "Honorable", new string[] {
					"Recall/mirror warp requires " +(evac_time/60)+"s warmup delay.",
					"Nurse heals add a stacking debuff."
				}, delegate ( HonorBoundLogic logic ) {
					lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );

					lif_mod.Config.Data.EvacWarpChargeDurationFrames = lif_default.EvacWarpChargeDurationFrames;
					lif_mod.Config.Data.SorenessDurationSeconds = lif_default.SorenessDurationSeconds;
				}, delegate ( HonorBoundLogic logic ) {
					lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );

					lif_mod.Config.Data.EvacWarpChargeDurationFrames = -1;
					lif_mod.Config.Data.SorenessDurationSeconds = lif_default.SorenessDurationSeconds;
				} );

			int bleed_time = inj_mod.Config.Data.DurationOfBleedingHeart;
			HonorBoundLogic.DefineHonorific( "Nimble", new string[] {
					"Broken Hearts last only "+(bleed_time/(3*60)) +"s before fading (originally "+(bleed_time/60)+ "s).",
					"3x more likely to lose max health from damage."
				}, delegate ( HonorBoundLogic logic ) {
					inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );

					inj_mod.Config.Data.DurationOfBleedingHeart = inj_default.DurationOfBleedingHeart / 3;
					inj_mod.Config.Data.HarmBufferCapacityBeforeReceivingInjury = inj_default.HarmBufferCapacityBeforeReceivingInjury / 3f;
				}, delegate ( HonorBoundLogic logic ) {
					inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );

					inj_mod.Config.Data.DurationOfBleedingHeart = inj_default.DurationOfBleedingHeart;
					inj_mod.Config.Data.HarmBufferCapacityBeforeReceivingInjury = inj_default.HarmBufferCapacityBeforeReceivingInjury;
				} );

			HonorBoundLogic.DefineHonorific( "Frugal", new string[] {
					"Items have 1/2 normal durability.",
					"Repairs drain max durability."
				}, delegate ( HonorBoundLogic logic ) {
					dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );

					dur_mod.Config.Data.GeneralWearAndTearMultiplier = dur_default.GeneralWearAndTearMultiplier * 2;
					dur_mod.Config.Data.MaxDurabilityLostPerRepair = dur_default.MaxDurabilityLostPerRepair;
				}, delegate ( HonorBoundLogic logic ) {
					dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );

					dur_mod.Config.Data.GeneralWearAndTearMultiplier = dur_default.GeneralWearAndTearMultiplier;
					dur_mod.Config.Data.MaxDurabilityLostPerRepair = 0;
				} );

			HonorBoundLogic.DefineHonorific( "Resourceful", new string[] {
					"No durability repairs.",
					"Overriding."
				}, delegate ( HonorBoundLogic logic ) {
					dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );

					dur_mod.Config.Data.CanRepair = false;
				}, delegate ( HonorBoundLogic logic ) {
					dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );

					dur_mod.Config.Data.CanRepair = dur_default.CanRepair;
				} );

			HonorBoundLogic.DefineHonorific( "Generous", new string[] {
					"NPCs must not live crowded or high above ground."
				}, delegate ( HonorBoundLogic logic ) {
					lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );

					lif_mod.Config.Data.MinimumRatioTownNPCSolidBlocks = lif_default.MinimumRatioTownNPCSolidBlocks;
					lif_mod.Config.Data.MinimumTownNpcTileSpacing = lif_default.MinimumTownNpcTileSpacing;
				}, delegate ( HonorBoundLogic logic ) {
					lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );

					lif_mod.Config.Data.MinimumRatioTownNPCSolidBlocks = 0;
					lif_mod.Config.Data.MinimumTownNpcTileSpacing = 0;
				} );

			HonorBoundLogic.DefineHonorific( "Expedient", new string[] {
					"Lunatic gives +1 day apocalypse delay for each mask (originally 2.5 days)."
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

					lun_mod.Config.Data.HalfDaysRecoveredPerMask = 2;
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

					lun_mod.Config.Data.HalfDaysRecoveredPerMask = lun_default.HalfDaysRecoveredPerMask;
				} );

			HonorBoundLogic.DefineHonorific( "Strategist", new string[] {
					"Lunatic gives 1/2 added time for Wall of Flesh.",
					"Lunatic gives 1/2 warmup time (originally 9 days)."
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
					int mul = logic.CurrentActiveHonorifics.Contains("Procrastinator") ? 5 : 1;

					lun_mod.Config.Data.WallOfFleshMultiplier = (lun_default.WallOfFleshMultiplier * (float)mul) / 2f;
					lun_mod.Config.Data.DaysUntil = (lun_default.DaysUntil * mul) / 2;
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
					int mul = logic.CurrentActiveHonorifics.Contains( "Procrastinator" ) ? 5 : 1;
					
					lun_mod.Config.Data.WallOfFleshMultiplier = lun_default.WallOfFleshMultiplier * (float)mul;
					lun_mod.Config.Data.DaysUntil = lun_default.DaysUntil * mul;
				} );

			HonorBoundLogic.DefineHonorific( "Completionist", new string[] {
					"Lunatic requires every boss mask (not just Moon Lord's)."
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

					lun_mod.Config.Data.MoonLordMaskWins = false;
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

					lun_mod.Config.Data.MoonLordMaskWins = true;
				} );

			HonorBoundLogic.DefineHonorific( "Procrastinator", new string[] {
					"Lunatic gives 5x warmup time.",
					"Lunatic gives 5x mask time."
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

					lun_mod.Config.Data.DaysUntil = lun_mod.Config.Data.DaysUntil / 5;
					lun_mod.Config.Data.HalfDaysRecoveredPerMask = lun_mod.Config.Data.HalfDaysRecoveredPerMask / 5;
				}, delegate ( HonorBoundLogic logic ) {
					lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );

					lun_mod.Config.Data.DaysUntil = lun_mod.Config.Data.DaysUntil * 5;
					lun_mod.Config.Data.HalfDaysRecoveredPerMask = lun_mod.Config.Data.HalfDaysRecoveredPerMask * 5;
				} );
		}

		
		private static void DefineHonorific( string name, string[] changes, Action<HonorBoundLogic> apply, Action<HonorBoundLogic> skip ) {
			HonorBoundLogic.Honorifics[ name ] = changes;
			HonorBoundLogic.HonorificAllowed[ name ] = true;
			HonorBoundLogic.HonorificAdd[ name ] = apply;
			HonorBoundLogic.HonorificSkip[ name ] = skip;
		}


		////////////////

		internal HonorBoundLogic() {
			this.IsHonorBound = false;
			this.HasNoHonor = false;
		}

		internal void Load( HonorBoundMod mymod, bool is_honor_bound, bool has_no_honor, ISet<string> honorifics ) {
			if( this.HasBegin() ) {
				throw new Exception("Invalid attempt to reset honorifics.");
			}
			
			if( (mymod.Config.Data.DEBUGMODE & 2) == 0 ) {
				this.IsHonorBound = is_honor_bound;
				this.HasNoHonor = has_no_honor;
				this.CurrentActiveHonorifics = honorifics;

				if( HonorBoundLogic.AllModsUpToDate ) {
					if( is_honor_bound ) {
						this.ForHonor( mymod );
					} else if( has_no_honor ) {
						this.NoHonor( mymod );
					}
				}
			}
		}


		////////////////
		
		public void RefreshAllowedHonorifics() {
			bool has_changed = false;
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lun_world = lun_mod.GetModWorld<TheLunaticWorld>();

			if( lun_world.GameLogic.HasGameEnded ) {
				HonorBoundLogic.HonorificAllowed["Expedient"] = false;
				HonorBoundLogic.HonorificAllowed["Strategist"] = false;
				HonorBoundLogic.HonorificAllowed["Completionist"] = false;
				has_changed = true;
			}

			if( has_changed ) {
				foreach( string honorific in this.CurrentActiveHonorifics.ToArray() ) {
					if( !HonorBoundLogic.HonorificAllowed[honorific] ) {
						this.CurrentActiveHonorifics.Remove( honorific );
					}
				}
			}
		}


		////////////////

		public bool HasBegin() {
			return this.IsHonorBound || this.HasNoHonor;
		}

		internal bool ForHonor( HonorBoundMod mymod ) {
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var cap_mod = (CapitalismMod)ModLoader.GetMod( "Capitalism" );
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );
			var dur_default = new Durability.ConfigurationData();
			var inj_default = new Injury.ConfigurationData();
			var liv_default = new Lives.ConfigurationData();
			var sta_default = new Stamina.ConfigurationData();
			var cap_default = new Capitalism.ConfigurationData();
			var lun_default = new TheLunatic.ConfigurationData();
			var lif_default = new LosingIsFun.ConfigurationData();

			dur_mod.Config.Data.Enabled = true;
			inj_mod.Config.Data.Enabled = true;
			liv_mod.Config.Data.Enabled = true;
			sta_mod.Config.Data.Enabled = true;
			cap_mod.Config.Data.Enabled = true;
			lun_mod.Config.Data.Enabled = true;
			lif_mod.Config.Data.Enabled = true;

			foreach( string honorific in this.CurrentActiveHonorifics ) {
				HonorBoundLogic.HonorificAdd[ honorific ]( this );
			}
			foreach( string honorific in HonorBoundLogic.Honorifics.Keys.Where( x => !this.CurrentActiveHonorifics.Contains(x) ) ) {
				HonorBoundLogic.HonorificSkip[ honorific ]( this );
			}

			this.IsHonorBound = true;

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

			return true;
		}

		internal bool NoHonor( HonorBoundMod mymod ) {
			var dur_mod = (DurabilityMod)ModLoader.GetMod( "Durability" );
			var inj_mod = (InjuryMod)ModLoader.GetMod( "Injury" );
			var liv_mod = (LivesMod)ModLoader.GetMod( "Lives" );
			var sta_mod = (StaminaMod)ModLoader.GetMod( "Stamina" );
			var cap_mod = (CapitalismMod)ModLoader.GetMod( "Capitalism" );
			var lun_mod = (TheLunaticMod)ModLoader.GetMod( "TheLunatic" );
			var lif_mod = (LosingIsFunMod)ModLoader.GetMod( "LosingIsFun" );

			dur_mod.Config.Data.Enabled = false;
			inj_mod.Config.Data.Enabled = false;
			liv_mod.Config.Data.Enabled = false;
			sta_mod.Config.Data.Enabled = false;
			cap_mod.Config.Data.Enabled = false;
			lun_mod.Config.Data.Enabled = false;
			lif_mod.Config.Data.Enabled = false;

			this.HasNoHonor = true;

			SimpleMessage.PostMessage( "No honor codes in effect. Vanilla only.", "", 10 * 60 );
			//Main.PlaySound( SoundID.NPCDeath20.WithVolume( 0.5f ) );	// Pig Squeal
			//Main.PlaySound( SoundID.NPCDeath59.WithVolume( 0.5f ) );    // Cultist cry
			Main.PlaySound( SoundID.Item16.WithVolume( 0.5f ) );    // Fart

			return true;
		}
	}
}
