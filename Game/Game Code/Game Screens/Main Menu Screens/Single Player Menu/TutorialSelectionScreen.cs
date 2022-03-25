#region File Description
//-----------------------------------------------------------------------------
// StageSelectScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A MenuScreen which presents the player with the two Modes to play.
     /// Upon choosing one of the Modes, the player is taken to the corresponding
     /// Gameplay Screen.
     /// 
     /// On this MenuScreen, the player's Avatar & Gamer Information is also displayed.
     /// </remarks>
     public class TutorialSelectionScreen : MenuScreen
     {
          #region Fields

          MenuEntry tutorialMenuEntry;
          MenuEntry noTutorialMenuEntry;

          public static bool TutorialScreenCompleted = false;
          public static bool IsTutorialMode = true;

          #endregion


          #region Properties

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public TutorialSelectionScreen()
               : base("View Tutorial?")
          {
               TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.StartingGame;
               }

               // Create our menu entries.
               tutorialMenuEntry = new MenuEntry("Tutorial", "You're new and can't figure\nout basic controls on your own.\nLearn How To Play.");
               noTutorialMenuEntry = new MenuEntry("I'm Ready!", "You've played a shooter before.\nJump straight into the action!");

               // Hook up menu event handlers.
               tutorialMenuEntry.Selected += TutorialMenuSelected;
               tutorialMenuEntry.Position = new Vector2(tutorialMenuEntry.Position.X, EngineCore.ScreenCenter.Y - 100);
               tutorialMenuEntry.DescriptionPosition = new Vector2(tutorialMenuEntry.DescriptionPosition.X, tutorialMenuEntry.DescriptionPosition.Y);

               // Hook up menu event handlers.
               noTutorialMenuEntry.Selected += NoTutorialMenuSelected;
               noTutorialMenuEntry.Position = new Vector2(noTutorialMenuEntry.Position.X, EngineCore.ScreenCenter.Y - 55);
               noTutorialMenuEntry.DescriptionPosition = new Vector2(noTutorialMenuEntry.DescriptionPosition.X, noTutorialMenuEntry.DescriptionPosition.Y);


               // Add entries to the menu.
               MenuEntries.Add(tutorialMenuEntry);
               MenuEntries.Add(noTutorialMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.FontScale = 1.0f;
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
                    entry.ShowIcon = false;
                    entry.useCustomBorderSize = true;
                    entry.menuEntryBorderSize = new Vector2(300f, 50f);
                    entry.ShowBorder = false;



                    entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                    entry.SelectedBorderColor = Color.DarkOrange * (255f / 255f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.32f;

                    entry.DescriptionColor = Color.DarkOrange;
                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.32f;
               }

               this.MenuTitleColor = Color.DarkOrange;

          }

          public override void LoadContent()
          {
               base.LoadContent();
          }

          #endregion


          #region Menu Event Handlers

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void TutorialMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               IsTutorialMode = true;

               MainMenuScreen.isMovingToDoor = true;
          }

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void NoTutorialMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               IsTutorialMode = false;

               MainMenuScreen.isMovingToDoor = true;
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               base.OnCancel(playerIndex);
          }

          #endregion


          #region Handle Input

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               if (MainMenuScreen.isMovingToDoor)
                    return;

               base.HandleInput(input, gameTime);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (MainMenuScreen.isMovingToDoor)
               {
                    if (MainMenuScreen.hasMovedToDoor)
                    {
                         TutorialScreenCompleted = true;

                         MainMenuScreen.hasMovedToDoor = false;
                         MainMenuScreen.isMovingToDoor = false;

                         this.ExitScreen();
                    }
               }
          }

          #endregion


          #region Draw

          float alpha = 0f;

          public override void Draw(GameTime gameTime)
          {
               if (MainMenuScreen.isMovingToDoor)
               {
                    ScreenManager.FadeBackBufferToBlack((int)(alpha += 1.2f));
                    return;
               }

               base.Draw(gameTime);
          }

          #endregion
     }
}