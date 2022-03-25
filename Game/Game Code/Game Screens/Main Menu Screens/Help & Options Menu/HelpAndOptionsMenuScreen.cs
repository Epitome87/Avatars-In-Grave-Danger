
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Menu Screen which presents menu entries for viewing
     /// Options, Controls, and How to Play.
     /// </remarks>
     public class HelpAndOptionsMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry optionsMenuEntry;
          MenuEntry controlsMenuEntry;
          MenuEntry howToPlayMenuEntry;
          MenuEntry enemyInfoMenuEntry;
          MenuEntry backMenuEntry;

          #endregion

          
          #region Initialization

          public HelpAndOptionsMenuScreen()
               : base("H e l p  &  O p t i o n s")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ConfiguringSettings;
               }

               this.numberOfColumns = 2;

               howToPlayMenuEntry = new MenuEntry("How To Play", "Learn How To Play\nAvatars In Grave Danger.");
               controlsMenuEntry = new MenuEntry("Controls", "Learn the Controls.");
               optionsMenuEntry = new MenuEntry("Settings", "Customize Audio Options\nStorage Settings & More.");
               enemyInfoMenuEntry = new MenuEntry("Enemy Information", "Learn About the Different Types of Enemies.");
               backMenuEntry = new MenuEntry("Back", "Return to the previous menu screen.");

               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y);

               howToPlayMenuEntry.Selected += HowToPlayMenuEntrySelected;
               controlsMenuEntry.Selected += ControlsMenuEntrySelected;
               optionsMenuEntry.Selected += OptionsMenuEntrySelected;
               enemyInfoMenuEntry.Selected += EnemyInfoMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               MenuEntries.Add(howToPlayMenuEntry);
               MenuEntries.Add(controlsMenuEntry);
               MenuEntries.Add(optionsMenuEntry);
               //MenuEntries.Add(enemyInfoMenuEntry);
               MenuEntries.Add(backMenuEntry);


               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.Position = new Vector2(entry.Position.X - 250, entry.Position.Y + 150);


                    entry.AdditionalVerticalSpacing = 35;
                    entry.menuEntryBorderScale = new Vector2(1.0f, 1.0f);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;

                    entry.ShowGradientBorder = true;
                    entry.ShowBorder = false;

                    entry.menuEntryBorderSize = new Vector2(200, 200);
                    entry.useCustomBorderSize = true;
                    entry.FontScale = 0.75f;
                    entry.ShowIcon = false;

                    entry.UnselectedBorderColor = new Color(Color.CornflowerBlue.R, Color.CornflowerBlue.G, Color.CornflowerBlue.B, 125);
                    entry.SelectedBorderColor = new Color(Color.DarkOrange.R, Color.DarkOrange.G, Color.DarkOrange.B, 125);

                    entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                    entry.SelectedBorderColor = Color.DarkOrange * (255f / 255f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.32f;

                    entry.DescriptionColor = Color.DarkOrange;
                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.32f;
               }

               this.MenuTitleColor = Color.DarkOrange;

               backMenuEntry.IsPulsating = true;
          }

          #endregion


          #region Menu Events

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new HowToPlayScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void ControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new ControlsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new SettingsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void EnemyInfoMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new EnemyInfoMenuScreen(), e.PlayerIndex);
          }

          protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               base.OnCancel(playerIndex);
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the screen. This darkens down the gameplay screen
          /// that is underneath us, and then chains to the base MenuScreen.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               //ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 3 / 5);

               base.Draw(gameTime);
          }

          #endregion
     }
}