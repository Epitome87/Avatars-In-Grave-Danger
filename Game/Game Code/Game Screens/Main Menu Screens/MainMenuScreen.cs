#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
using PixelEngine.Graphics;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// The Main Menu screen is the first thing displayed when the game starts up.
     /// It displays the title's picture, as well as a number of menu entries:
     /// Play Game, Options, View Credits, View Awards, View Unlockables, Exit.
     /// </remarks>
     public class MainMenuScreen : MenuScreen
     {
          CinematicEvent moveToDoorEvent;

          public static bool isMovingToDoor = false;
          public static bool hasMovedToDoor = false;

          #region Fields

          // Create our menu entries.
          MenuEntry playGameMenuEntry = new MenuEntry("Single\nPlayer", "Blast through waves of zombies.\nEarn money to upgrade your arsenal."); //"Play A Solo Game of\nAvatars In Grave Danger!"); 
          MenuEntry optionsMenuEntry = new MenuEntry("Help &\nOptions", "Get Help &\nCustomize Your Experience.");
          MenuEntry extrasMenuEntry = new MenuEntry("Extra &\nGoodies", "View Your Awards, Watch The Credits,\nOr Tell A Friend About This Game!");
          MenuEntry scoreEntry = new MenuEntry("Global\nHighscores", "Compare Your Scores To\nOther Players Online!");
          MenuEntry exitMenuEntry = new MenuEntry("Exit\nGame", "Exit Back to the Dashboard.");

          // Menu entry exclusive to Trial Mode.
          MenuEntry purchaseGameMenuEntry;

          PlayerBackgroundScreen playerScreen;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor fills in the menu contents.
          /// </summary>
          public MainMenuScreen()
               : base("Avatars In Grave Danger")//"    Avatars In\nGrave Danger")
          {

               this.numberOfColumns = 2;
               this.SpacingBetweenColumns = 500;

               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);


               List<CameraEffect> effectList = new List<CameraEffect>();

               effectList.Add(new PointEffect(1.0f, new Vector3(0f, 2f, 50f)));
               effectList.Add(new MoveEffect(2.5f, new Vector3(0f, 2f, 35f)));

               moveToDoorEvent = new CinematicEvent(effectList, false);


               AchievementManager.IsUnlockNow = true;

               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               // Hook up menu event handlers.
               playGameMenuEntry.Selected += SinglePlayerMenuEntrySelected;
               optionsMenuEntry.Selected += HelpAndOptionsMenuEntrySelected;
               extrasMenuEntry.Selected += ExtrasMenuEntrySelected;
               scoreEntry.Selected += ScoreMenuEntrySelected;
               exitMenuEntry.Selected += ExitMenuEntrySelected;

               // Add entries to the menu.
               MenuEntries.Add(playGameMenuEntry);
               MenuEntries.Add(optionsMenuEntry);
               MenuEntries.Add(extrasMenuEntry);
               //MenuEntries.Add(scoreEntry);
               MenuEntries.Add(exitMenuEntry);

               float menuScale = 1f;
               float additionalSpacing = 35f;// 0;// 20f;

               if (Guide.IsTrialMode)
               {
                    purchaseGameMenuEntry = new MenuEntry("Purchase\nGame!", "Buy the FULL Version of Avatars In Grave Danger");
                    purchaseGameMenuEntry.Selected += PurchaseGameEntrySelected;
                    MenuEntries.Add(purchaseGameMenuEntry);
               }

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.Position = new Vector2(entry.Position.X - 250, entry.Position.Y + 150);
                    entry.AdditionalVerticalSpacing = additionalSpacing;
                    entry.menuEntryBorderScale = new Vector2(1.0f, menuScale);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;

                    entry.ShowGradientBorder = true;
                    entry.ShowBorder = false;

                    entry.menuEntryBorderSize = new Vector2(200, 200);
                    entry.useCustomBorderSize = true;
                    entry.FontScale = 0.75f;
                    entry.ShowIcon = false;

                    entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                    entry.SelectedBorderColor = Color.Red;// Color.DarkOrange * (255f / 255f);

                    entry.DescriptionColor = Color.Red;// Color.DarkOrange;

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.35f;

                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale = 0.34f;
               }

               this.MenuTitleColor = Color.Red;// Color.DarkOrange;

               exitMenuEntry.IsPulsating = true;


               playerScreen = new PlayerBackgroundScreen();
               ScreenManager.AddScreen(playerScreen, AvatarZombieGame.ControllingPlayer);

               playerScreen.playerPosition = new Vector3(0f, 0f, 0f);//(0f, 1.25f, -1.5f);
               playerScreen.border = new Rectangle(640 - 125, 150, 250, 400);
               playerScreen.borderScale = 1.75f * 2f;// 0.75f;
               playerScreen.playerScale = 1.75f;//0.75f;
               playerScreen.ShowBorder = false;

               playerScreen.player.Avatar.PlayAnimation(AvatarAnimationPreset.MaleIdleLookAround, true);

               playerScreen.ShowGamerTag = false;


               // Reset this here?!
               AvatarZombieGame.AwardData.TotalUpgrades = 0;
               AvatarZombieGame.AwardData.CurrentWave = 0;
               AvatarZombieGame.AwardData.ConsecutivePerfectAccuracy = 0;
          }

          public override void LoadContent()
          {
               base.LoadContent();
          }

          public override void UnloadContent()
          {
               /* This was making the game crash when exiting back to dashboard.
                * My theory is, we are removing a screen within the foreach () screen.Unload loop,
                * causing an index change error.
               // New as of 12-5-2011
               if (playerScreen != null)
               {
                    ScreenManager.RemoveScreen(playerScreen);
                    playerScreen.UnloadContent();
                    playerScreen = null;
               }
               */
               base.UnloadContent();
          }

          #endregion


          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Single Player menu entry is selected.
          /// </summary>
          void SinglePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new DifficultySelectionScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Multiplayer menu entry is selected.
          /// </summary>
          void MultiplayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new MultiplayerMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Live menu entry is selected.
          /// </summary>
          void LiveMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               CreateOrFindSession(NetworkSessionType.SystemLink, e.PlayerIndex);
          }

          /// <summary>
          /// Helper method shared by the Live and System Link menu event handlers.
          /// </summary>
          void CreateOrFindSession(NetworkSessionType sessionType,
                                   PlayerIndex playerIndex)
          {
               // First, we need to make sure a suitable gamer profile is signed in.
               ProfileSignInScreen profileSignIn = new ProfileSignInScreen(sessionType);

               // Hook up an event so once the ProfileSignInScreen is happy,
               // it will activate the CreateOrFindSessionScreen.
               profileSignIn.ProfileSignedIn += delegate
               {
                    GameScreen createOrFind = new CreateOrFindSessionScreen(sessionType);

                    ScreenManager.AddScreen(createOrFind, playerIndex);
               };

               // Activate the ProfileSignInScreen.
               ScreenManager.AddScreen(profileSignIn, playerIndex);
          }
          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          void HelpAndOptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               //throw new Exception("We broke the game!!");

               ScreenManager.AddScreen(new HelpAndOptionsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Extras menu entry is selected.
          /// </summary>
          void ExtrasMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new ExtrasMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Purchase Game menu entry is selected.
          /// This is only present in Trial Mode.
          /// </summary>
          void PurchaseGameEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (!AvatarZombieGame.CurrentPlayer.GamerInformation.CanBuyGame())
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Purchasing Game Disabled",
                         "You cannot purchase the Full version of Avatars In Grave Danger. To purchase the Full version, please make sure you are signed into Live, on a non-Guest account, and have the appropriate Privileges.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               // Do Purchase Guide Stuff
               if (Guide.IsTrialMode)
               {
                    Guide.ShowMarketplace(AvatarZombieGame.CurrentPlayer.GamerInformation.PlayerIndex);

                    // Set SaveEnabled to true?
               }
          }

          void ScoreMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
                    return;

               ScreenManager.AddScreen(new GlobalHighScores(), e.PlayerIndex);
          }

          /// <summary>
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               const string message = "Are you sure you want to exit the game?";

               MessageBoxWithKeyboardScreen confirmExitMessageBox = new MessageBoxWithKeyboardScreen(message);

               confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

               ScreenManager.AddScreen(confirmExitMessageBox, e.PlayerIndex);
          }

          /// <summary>
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               // New as of 12-5-2011
               if (playerScreen != null)
               {
                    ScreenManager.RemoveScreen(playerScreen);
                    playerScreen.UnloadContent();
                    playerScreen = null;
               }

               // Leave this screen, without transitioning.
               ScreenManager.RemoveScreen(this);

               // Return to the Start Screen.
               ScreenManager.AddScreen(new StartScreen(), playerIndex);
          }

          /// <summary>
          /// Event handler for when the user selects Accept on the "are you sure
          /// you want to exit" message box.
          /// </summary>
          void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
               {
                    //ScreenManager.AddScreen(new BuyScreen(), e.PlayerIndex);
                    // Add this for now
                    ScreenManager.Game.Exit();
               }

               else
               {
#if XBOX
                    // Set the request flag
                    if (StorageManager.SaveRequested == false)
                    {
                         StorageManager.SaveRequested = true;
                    }
#endif
               }

          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               //if (IsActive)
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (isMovingToDoor)
               {
                    moveToDoorEvent.Update(gameTime);

                    if (moveToDoorEvent.HasFinished)
                    {
                         hasMovedToDoor = true;
                    }
               }


               #region Handle Menu Differences When In Trial / Guest or Non-Live Profiles

               // Handle the appearance of Purchase Game Menu Entry.
               if (!Guide.IsTrialMode)
               {
                    if (purchaseGameMenuEntry != null)
                    {
                         if (MenuEntries.Contains(purchaseGameMenuEntry))
                         {
                              MenuEntries.RemoveAt(MenuEntries.Count - 1);
                              SelectedMenuEntry = 0;

                              AvatarZombieGame.GamePurchased = true;
                              AvatarZombieGame.AwardData.GamePurchased = true;
                         }
                    }

                    scoreEntry.UnselectedColor = Color.White;
                    scoreEntry.SelectedColor = Color.White;
                    scoreEntry.Description = "Compare Your Scores To\nOther Players Online!";
               }

               else if (Guide.IsTrialMode)
               {
                    scoreEntry.UnselectedColor = Color.Gray;
                    scoreEntry.SelectedColor = Color.Gray;
                    scoreEntry.Description = "Not Available in Trial Mode.";
               }

               #endregion

               #region Handle Pending Save Request

               // If a save is pending, save as soon as the storage device is chosen
               if ((StorageManager.SaveRequested))
               {
                    StorageDevice device = StorageManager.Device;

                    if (device != null && device.IsConnected)
                    {
                         AvatarZombieGame.SaveGame(device);
                    }

                    // Reset the request flag
                    StorageManager.SaveRequested = false;

                    //ScreenManager.Game.Exit();

                    AvatarZombieGame.OnlineSyncManager.stop(delegate() { ScreenManager.Game.Exit(); },
                         true);
               }

               #endregion
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion
     }
}
