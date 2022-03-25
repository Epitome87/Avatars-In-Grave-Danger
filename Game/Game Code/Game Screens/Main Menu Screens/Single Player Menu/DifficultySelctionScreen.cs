#region File Description
//-----------------------------------------------------------------------------
// DifficultySelectionScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine;
using PixelEngine.CameraSystem;
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
     public class DifficultySelectionScreen : MenuScreen
     {
          #region Fields

          /// <summary>
          /// Public static variable that tells us if the Player
          /// has selected a Difficulty to play on. We share this with
          /// a few other classes.
          /// </summary>
          public static bool IsDifficultySelected = false;

          /// <summary>
          /// Public static variable that tells us which Difficulty the Player
          /// has selected to play on. We share this with a few other classes.
          /// </summary>
          public static Difficulty DifficultySelected = Difficulty.Normal;


          /// <summary>
          /// A Menu Entry used to handle the selection of Normal Difficulty.
          /// </summary>
          MenuEntry normalModeMenuEntry = new MenuEntry("Normal", "Zombies are slow and weak.");

          /// <summary>
          /// A Menu Entry used to handle the selection of Hard Difficulty.
          /// </summary>
          MenuEntry hardModeMenuEntry = new MenuEntry("Hard", "Zombies are faster and\nhungrier for brains.");

          #endregion


          #region Properties

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public DifficultySelectionScreen()
               : base("Select Difficulty")
          {
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);


               TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.StartingGame;
               }

               // Set the necessary properties for the Normal Menu Entry.
               normalModeMenuEntry.Selected += NormalMenuSelected;
               normalModeMenuEntry.Position = new Vector2(normalModeMenuEntry.Position.X, EngineCore.ScreenCenter.Y - 100);
               normalModeMenuEntry.DescriptionPosition = new Vector2(normalModeMenuEntry.DescriptionPosition.X, normalModeMenuEntry.DescriptionPosition.Y);
               normalModeMenuEntry.DescriptionFontScale = 1.0f;

               // Set the necessary properties for the Hard Menu Entry.
               hardModeMenuEntry.Selected += HardMenuSelected;
               hardModeMenuEntry.Position = new Vector2(hardModeMenuEntry.Position.X, EngineCore.ScreenCenter.Y - 55);
               hardModeMenuEntry.DescriptionPosition = new Vector2(hardModeMenuEntry.DescriptionPosition.X, hardModeMenuEntry.DescriptionPosition.Y);
               hardModeMenuEntry.DescriptionFontScale = 1.0f;

               // Add the menu entries to the screen.
               MenuEntries.Add(normalModeMenuEntry);
               MenuEntries.Add(hardModeMenuEntry);

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

               if (Guide.IsTrialMode)
               {
                    // Gray out Hard Mode in Trial versions.
                    hardModeMenuEntry.Description = "Not Available In Trial Mode";
                    hardModeMenuEntry.SelectedColor = Color.Gray;
                    hardModeMenuEntry.UnselectedColor = Color.Gray;
               }

          }

          #endregion


          #region Menu Event Handlers

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void NormalMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               DifficultySelected = Difficulty.Normal;

               ZombieGameSettings.Difficulty = DifficultySelected;

               ScreenManager.AddScreen(new TutorialSelectionScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void HardMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               if (Guide.IsTrialMode)
                    return;

               DifficultySelected = Difficulty.Hard;

               ZombieGameSettings.Difficulty = DifficultySelected;

               ScreenManager.AddScreen(new TutorialSelectionScreen(), e.PlayerIndex);
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


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (TutorialSelectionScreen.TutorialScreenCompleted)
               {
                    ScreenManager.RemoveScreen(this);

                    GameplayBackgroundScreen.isUpdate = false;

                    // Reset the Tutorial Selection Screen properties.
                    TutorialSelectionScreen.TutorialScreenCompleted = false;

                    // Are we doing the Tutorial?
                    bool doTutorial = TutorialSelectionScreen.IsTutorialMode;

                    // Now reset the flag.
                    TutorialSelectionScreen.IsTutorialMode = true;

                    if (doTutorial)
                    {
                         AnimatedLoadingScreen.Load(true, EngineCore.ControllingPlayer, ((Difficulty)DifficultySelected).ToString() + "\nWave 1", true, new ArcadeWithTutorialGameplayScreen());
                    }

                    else
                    {
                         AnimatedLoadingScreen.Load(true, EngineCore.ControllingPlayer, ((Difficulty)DifficultySelected).ToString() + "\nWave 1", true, new ArcadeGameplayScreen());
                    }
               }
          }

          #endregion
     }
}