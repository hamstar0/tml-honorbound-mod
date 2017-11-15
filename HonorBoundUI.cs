using HamstarHelpers.HudHelpers;
using HamstarHelpers.UIHelpers.Elements;
using HonorBound.NetProtocol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;


namespace HonorBound {
	class HonorBoundUI : UIState {
		public static float PanelWidth = 256f;
		public static float PanelHeight = 416f;
		public static Color ButtonEdgeColor = new Color( 96, 96, 192 );
		public static Color ButtonBodyColor = new Color( 0, 0, 160 );
		public static Color ButtonBodyLitColor = new Color( 0, 0, 255 );
		
		public static Texture2D BackgroundLogo { get; private set; }

		public UIPanel MainPanel;
		public IDictionary<string, UICheckbox> Options = new Dictionary<string, UICheckbox>();
		private UserInterface Backend;

		public bool IsOpen { get; private set; }
		public bool IsTogglerLit { get; private set; }

		private bool HasBeenSetup = false;



		public HonorBoundUI() {
			this.Backend = new UserInterface();
			this.IsOpen = false;
			this.IsTogglerLit = false;
		}

		public void PostSetupContent() {
			this.Activate();
			this.Backend.SetState( this );
			this.HasBeenSetup = true;
		}

		////////////////

		public void RecalculateBackend() {
			if( !this.HasBeenSetup ) { return; }
			this.Backend.Recalculate();
		}

		public void UpdateBackend( GameTime game_time ) {
			if( !this.HasBeenSetup ) { return; }
			this.Backend.Update( game_time );
		}

		protected override void DrawSelf( SpriteBatch sb ) {
			if( this.MainPanel.ContainsPoint( Main.MouseScreen ) ) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}


		////////////////

		public override void OnInitialize() {
			var mymod = (HonorBoundMod)ModLoader.GetMod( "HonorBound" );
			var myworld = mymod.GetModWorld<MyWorld>();

			if( HonorBoundUI.BackgroundLogo == null ) {
				HonorBoundUI.BackgroundLogo = mymod.GetTexture( "BackgroundLogo" );
			}

			float top = 0;
			
			this.MainPanel = new UIPanel();
			this.MainPanel.Left.Set( -(HonorBoundUI.PanelWidth / 2f), 0.5f );
			this.MainPanel.Top.Set( 8f - HonorBoundUI.PanelHeight, 0f );
			this.MainPanel.Width.Set( HonorBoundUI.PanelWidth, 0f );
			this.MainPanel.Height.Set( HonorBoundUI.PanelHeight, 0f );
			this.MainPanel.SetPadding( 12f );
			this.MainPanel.BackgroundColor = new Color( 0, 0, 64 );
			
			var bg = new UIImage( HonorBoundUI.BackgroundLogo );
			bg.Top.Set( 0f, 0.05f );
			bg.Left.Set( 0f, 0.4f );
			this.MainPanel.Append( bg );

			this.MainPanel.Append( new UIText( "Choose your honorifics:", 0.9f ) );
			
			foreach( var kv in HonorBoundLogic.Honorifics ) {
				top += 20f;
				
				string honorific = kv.Key;
				var ui_option = new UICheckbox( honorific, String.Join("\n", kv.Value.Descriptions) );

				ui_option.Top.Set( top, 0f );
				ui_option.SetText( "    "+honorific, 0.8f, false );
				ui_option.TextColor = Color.Gray;

				ui_option.OnSelectedChanged += delegate () {
					mymod = (HonorBoundMod)ModLoader.GetMod( "HonorBound" );
					myworld = mymod.GetModWorld<MyWorld>();
					
					if( ui_option.Selected ) {
						myworld.Logic.CurrentActiveHonorifics.Add( honorific );
					} else {
						myworld.Logic.CurrentActiveHonorifics.Remove( honorific );
					}
				};

				this.Options[ honorific ] = ui_option;
				this.MainPanel.Append( ui_option );
			}
			
			top += 20f;
			var honor_warn_text = new UIText( "Warning: Settings are permanent for your\nworld when honor bound.\nSelect 'No Honor' to play only vanilla.", 0.7f );
			honor_warn_text.Top.Set( top, 0f );
			honor_warn_text.Left.Set( 0, 0f );
			honor_warn_text.TextColor = Color.Yellow;
			this.MainPanel.Append( honor_warn_text );

			top += 56f;
			var for_honor_button = new UITextPanel<string>( "For Honor!" );
			for_honor_button.SetPadding( 4f );
			for_honor_button.Width.Set( HonorBoundUI.PanelWidth - 24f, 0f );
			for_honor_button.Left.Set( 0f, 0 );
			for_honor_button.Top.Set( top, 0f );
			for_honor_button.OnClick += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				this.ActivateForHonor( (HonorBoundMod)ModLoader.GetMod("HonorBound") );
			};
			for_honor_button.OnMouseOver += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				for_honor_button.BackgroundColor = HonorBoundUI.ButtonBodyLitColor;
			};
			for_honor_button.OnMouseOut += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				for_honor_button.BackgroundColor = HonorBoundUI.ButtonBodyColor;
			};
			this.MainPanel.Append( for_honor_button );
			
			top += 32f;
			var no_honor_button = new UITextPanel<string>( "No Honor..." );
			no_honor_button.SetPadding( 4f );
			no_honor_button.Width.Set( HonorBoundUI.PanelWidth - 24f, 0f );
			no_honor_button.Left.Set( 0f, 0 );
			no_honor_button.Top.Set( top, 0f );
			no_honor_button.OnClick += delegate( UIMouseEvent evt, UIElement listeningElement ) {
				this.ActivateNoHonor( (HonorBoundMod)ModLoader.GetMod("HonorBound") );
			};
			no_honor_button.OnMouseOver += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				no_honor_button.BackgroundColor = HonorBoundUI.ButtonBodyLitColor;
			};
			no_honor_button.OnMouseOut += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				no_honor_button.BackgroundColor = HonorBoundUI.ButtonBodyColor;
			};
			this.MainPanel.Append( no_honor_button );
			
			this.Append( this.MainPanel );
		}


		public void ResetOptions() {
			foreach( var option in this.Options.Values ) {
				option.Selected = false;
			}
		}


		////////////////

		public void RefreshAllowedOptions( HonorBoundLogic logic ) {
			if( !this.HasBeenSetup ) { return; }

			bool has_changed = false;

			foreach( var kv in logic.HonorificAllowed ) {
				string honorific = kv.Key;
				bool is_allowed = kv.Value;

				if( !is_allowed && this.Options.ContainsKey( honorific ) ) {
					var option = this.Options[honorific];

					this.MainPanel.RemoveChild( option );
					this.Options.Remove( honorific );
					has_changed = true;
				}
			}

			if( has_changed ) {
				this.Backend.Recalculate();
			}
		}

		////////////////

		public void Open() {
			this.IsOpen = true;
			this.MainPanel.Top.Set( -(HonorBoundUI.PanelHeight / 2f), 0.5f );
			this.Recalculate();
		}

		public void Close() {
			this.IsOpen = false;
			this.MainPanel.Top.Set( 8f - HonorBoundUI.PanelHeight, 0f );
			this.Recalculate();
		}


		private void GetTogglerDimensions( out Vector2 pos, out Vector2 size ) {
			size = new Vector2( HonorBoundUI.PanelWidth, 6f );
			float x = ((Main.screenWidth / 2f) - (HonorBoundUI.PanelWidth / 2f));

			if( this.IsOpen ) {
				pos = new Vector2( x, ((Main.screenHeight / 2f) - (HonorBoundUI.PanelHeight / 2f)) + 2f );
			} else {
				pos = new Vector2( x, 2f );
			}
		}

		public void DrawToggler( SpriteBatch sb ) {
			Vector2 pos, size;
			Color body_color = HonorBoundUI.ButtonBodyColor;
			Color edge_color = HonorBoundUI.ButtonEdgeColor;

			if( this.IsTogglerLit ) { body_color = HonorBoundUI.ButtonBodyLitColor; }
			this.GetTogglerDimensions( out pos, out size );
			
			HudHelpers.DrawBorderedRect( sb, body_color, edge_color, pos, size, 2 );
		}
		
		public void CheckTogglerMouseInteraction() {
			bool is_click = Main.mouseLeft && Main.mouseLeftRelease;
			bool lit = false;
			Vector2 pos, size;

			this.GetTogglerDimensions( out pos, out size );
			
			if( Main.mouseX >= pos.X && Main.mouseX < (pos.X+size.X) ) {
				if( Main.mouseY >= pos.Y && Main.mouseY < (pos.Y + size.Y) ) {
					if( is_click ) {
						if( this.IsOpen ) { this.Close(); }
						else { this.Open(); }
					} else {
						lit = true;
					}
				}
			}
			
			this.IsTogglerLit = lit;
		}


		////////////////

		private void ActivateForHonor( HonorBoundMod mymod ) {
			var modworld = mymod.GetModWorld<MyWorld>();
			var mylogic = modworld.Logic;
			
			mylogic.ForHonor();
			mylogic.BeginGameModeForLocalPlayer( mymod );

			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.SendHonorSettingsFromClient( mymod, Main.LocalPlayer );
			}
		}

		private void ActivateNoHonor( HonorBoundMod mymod ) {
			var modworld = mymod.GetModWorld<MyWorld>();
			var mylogic = modworld.Logic;
			
			mylogic.NoHonor();
			mylogic.BeginGameModeForLocalPlayer( mymod );

			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.SendHonorSettingsFromClient( mymod, Main.LocalPlayer );
			}
		}
	}
}
