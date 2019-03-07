using HamstarHelpers.Components.UI.Elements;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.HudHelpers;
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



		////////////////

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

		public void UpdateBackend( GameTime gameTime ) {
			if( !this.HasBeenSetup ) { return; }
			this.Backend.Update( gameTime );
		}

		protected override void DrawSelf( SpriteBatch sb ) {
			if( this.MainPanel.ContainsPoint( Main.MouseScreen ) ) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}


		////////////////

		public override void OnInitialize() {
			var mymod = (HonorBoundMod)ModLoader.GetMod( "HonorBound" );
			var myworld = mymod.GetModWorld<HonorBoundWorld>();

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
				var uiOption = new UICheckbox( honorific, String.Join("\n", kv.Value.Descriptions) );

				uiOption.Top.Set( top, 0f );
				uiOption.SetText( "    "+honorific, 0.8f, false );
				uiOption.TextColor = Color.Gray;

				uiOption.OnSelectedChanged += delegate () {
					mymod = (HonorBoundMod)ModLoader.GetMod( "HonorBound" );
					myworld = mymod.GetModWorld<HonorBoundWorld>();
					
					if( uiOption.Selected ) {
						myworld.Logic.CurrentActiveHonorifics.Add( honorific );
					} else {
						myworld.Logic.CurrentActiveHonorifics.Remove( honorific );
					}
				};

				this.Options[ honorific ] = uiOption;
				this.MainPanel.Append( uiOption );
			}
			
			top += 20f;
			var honorWarnText = new UIText( "Warning: Settings are permanent for your\nworld when honor bound.\nSelect 'No Honor' to play only vanilla.", 0.7f );
			honorWarnText.Top.Set( top, 0f );
			honorWarnText.Left.Set( 0, 0f );
			honorWarnText.TextColor = Color.Yellow;
			this.MainPanel.Append( honorWarnText );

			top += 56f;
			var forHonorButton = new UITextPanel<string>( "For Honor!" );
			forHonorButton.SetPadding( 4f );
			forHonorButton.Width.Set( HonorBoundUI.PanelWidth - 24f, 0f );
			forHonorButton.Left.Set( 0f, 0 );
			forHonorButton.Top.Set( top, 0f );
			forHonorButton.OnClick += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				this.ActivateForHonor();
			};
			forHonorButton.OnMouseOver += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				forHonorButton.BackgroundColor = HonorBoundUI.ButtonBodyLitColor;
			};
			forHonorButton.OnMouseOut += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				forHonorButton.BackgroundColor = HonorBoundUI.ButtonBodyColor;
			};
			this.MainPanel.Append( forHonorButton );
			
			top += 32f;
			var noHonorButton = new UITextPanel<string>( "No Honor..." );
			noHonorButton.SetPadding( 4f );
			noHonorButton.Width.Set( HonorBoundUI.PanelWidth - 24f, 0f );
			noHonorButton.Left.Set( 0f, 0 );
			noHonorButton.Top.Set( top, 0f );
			noHonorButton.OnClick += delegate( UIMouseEvent evt, UIElement listeningElement ) {
				this.ActivateNoHonor();
			};
			noHonorButton.OnMouseOver += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				noHonorButton.BackgroundColor = HonorBoundUI.ButtonBodyLitColor;
			};
			noHonorButton.OnMouseOut += delegate ( UIMouseEvent evt, UIElement listeningElement ) {
				noHonorButton.BackgroundColor = HonorBoundUI.ButtonBodyColor;
			};
			this.MainPanel.Append( noHonorButton );
			
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

			bool hasChanged = false;

			foreach( var kv in logic.HonorificAllowed ) {
				string honorific = kv.Key;
				bool isAllowed = kv.Value;

				if( !isAllowed && this.Options.ContainsKey( honorific ) ) {
					var option = this.Options[honorific];

					this.MainPanel.RemoveChild( option );
					this.Options.Remove( honorific );
					hasChanged = true;
				}
			}

			if( hasChanged ) {
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
			Color bodyColor = HonorBoundUI.ButtonBodyColor;
			Color edgeColor = HonorBoundUI.ButtonEdgeColor;

			if( this.IsTogglerLit ) { bodyColor = HonorBoundUI.ButtonBodyLitColor; }
			this.GetTogglerDimensions( out pos, out size );
			
			HudHelpers.DrawBorderedRect( sb, bodyColor, edgeColor, pos, size, 2 );
		}
		
		public void CheckTogglerMouseInteraction() {
			bool isClick = Main.mouseLeft && Main.mouseLeftRelease;
			bool lit = false;
			Vector2 pos, size;

			this.GetTogglerDimensions( out pos, out size );
			
			if( Main.mouseX >= pos.X && Main.mouseX < (pos.X+size.X) ) {
				if( Main.mouseY >= pos.Y && Main.mouseY < (pos.Y + size.Y) ) {
					if( isClick ) {
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

		private void ActivateForHonor() {
			var mymod = HonorBoundMod.Instance;
			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			var mylogic = modworld.Logic;
			
			mylogic.ForHonor();
			mylogic.BeginGameModeForLocalPlayer();

			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.SendHonorSettingsFromClient( Main.LocalPlayer );
			}
		}

		private void ActivateNoHonor() {
			var mymod = HonorBoundMod.Instance;
			var modworld = mymod.GetModWorld<HonorBoundWorld>();
			var mylogic = modworld.Logic;
			
			mylogic.NoHonor();
			mylogic.BeginGameModeForLocalPlayer();

			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.SendHonorSettingsFromClient( Main.LocalPlayer );
			}
		}
	}
}
