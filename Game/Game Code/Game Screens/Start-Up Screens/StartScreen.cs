#region File Description
//-----------------------------------------------------------------------------
// StartScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Audio;
using PixelEngine.CameraSystem;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
using PixelEngine.Graphics;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// The Start Screen is brought up after the Splash Screen. 
     /// This screen merely displays a request for the player to press Start.
     /// Upon pressing Start, the Player's Index is obtained, setting him as
     /// the controlling player.
     /// </remarks>
     public class StartScreen : MenuScreen
     {
          #region Fields

          TextObject startMessage;
          TextObject startMessage2;

          TextObject titleMessage;

          float elapsedTime = 0.0f;

          IAsyncResult result;

          static string pName;

          CinematicEvent triangularZoomEvent;

          bool updateCinematicEvent = true;

          string message = "";
          bool saveGameBeingRequested = false;
          bool gameSavedAlready = false;
          bool saveRequested = false;
          bool gamerSignedIn = false;
          private static bool profilePromptRead = false;


          private TextObject introductionTextObject;

          public static bool IsOnStartScreen = true;

 

          TextObject pressStartTextObject = new TextObject("Press Start To Skip", new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 50),
               FontType.HudFont, Color.Gold, 0.0f, Vector2.Zero, 1.0f, true);


          public static bool IsStartingFromAfterSkipText = false;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public StartScreen()
               : base("")
          {
               IsOnStartScreen = true;

               pressStartTextObject.IsCenter = true;
               ControllingPlayer = null;

               AudioManager.PlayMusic("Gameplay Music");

               TransitionOnTime = TimeSpan.FromSeconds(5f);
               TransitionOffTime = TimeSpan.FromSeconds(0f);

               startMessage = new TextObject("Avatars In\n", new Vector2(EngineCore.ScreenCenter.X, -150f));
               startMessage.FontType = FontType.TitleFont;
               startMessage.Scale = 1f;
               startMessage.Color = new Color(Color.CornflowerBlue.R, Color.CornflowerBlue.G, Color.CornflowerBlue.B + 100, 255);
               startMessage.Origin = startMessage.Font.MeasureString(startMessage.Text) / 2;
               startMessage.AddTextEffect(new MoveInEffect(1.5f, startMessage.Text, new Vector2(EngineCore.ScreenCenter.X, -150f), new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 150)));

               startMessage2 = new TextObject("Grave Danger", new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 25));
               startMessage2.FontType = FontType.TitleFont;
               startMessage2.Scale = 1f;
               startMessage2.Color = Color.Red;
               startMessage2.Origin = startMessage.Font.MeasureString(startMessage2.Text) / 2;
               startMessage2.AddTextEffect(new PixelEngine.Text.MoveInEffect(25.0f, startMessage2.Text, new Vector2(EngineCore.ScreenCenter.X, 1800f), new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 25)));//startMessage2.Position));


               titleMessage = new TextObject("Grave Danger", new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 25));
               titleMessage.FontType = FontType.TitleFont;
               titleMessage.Scale = 1f;
               titleMessage.Color = Color.Red;
               titleMessage.Origin = titleMessage.Font.MeasureString(titleMessage.Text) / 2;


               string introString = TextManager.WrapText("Zombies have been spotted around town. Their hungry moans resonate throughout the darkened sky. It is believed they originated from the local graveyard, where they have been seen in large numbers. Armed with weapons and wits, it is up to you to prevent more zombies from leaving the graveyard and infecting your town.\n\nFail, and everyone will be in...",
                    FontType.TitleFont, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Width * (1f / 0.4f));

               introductionTextObject = new TextObject(introString, EngineCore.ScreenCenter);
               introductionTextObject.FontType = FontType.TitleFont;
               introductionTextObject.Scale *= 0.4f;
               introductionTextObject.Color = Color.DarkOrange;// Color.LightGray;
               introductionTextObject.IsCenter = true;

               introductionTextObject.Text = "Introduction / Story Here";


               introductionTextObject.TextEffect = new MoveInEffect(25f, introductionTextObject.Text, new Vector2(EngineCore.ScreenCenter.X, 1300f), new Vector2(EngineCore.ScreenCenter.X, -400f));
          }

          public override void LoadContent()
          {
               base.LoadContent();

               // TODO: SHOULD ADD CHECK TO MAKE SURE GUIDE ISN"T ALREADY OPENNNNNNNNNNNNNNNNNNNNNNNNNNNNNN
               if (Gamer.SignedInGamers.Count < 1)
                    Guide.ShowSignIn(1, false);//true);

               //
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               //
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 1, 0);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 1.25f, 10f);
               CameraManager.ActiveCamera.ViewMatrix = Matrix.CreateLookAt(CameraManager.ActiveCamera.Position, CameraManager.ActiveCamera.LookAt, Vector3.Up);
               CameraManager.ActiveCamera.FieldOfView = 45f;



               List<CameraEffect> effectList = new List<CameraEffect>();

               effectList.Add(new PointEffect(2.5f, new Vector3(0f, 1f, 15f)));
               effectList.Add(new MoveEffect(true, 5.0f, new Vector3(0f, 10f, 0f)));
               effectList.Add(new MoveEffect(true, 5.0f, new Vector3(-10f, 10f, 0f)));
               effectList.Add(new MoveEffect(true, 5.0f, new Vector3(0f, 10f, 0f)));
               effectList.Add(new MoveEffect(true, 5.0f, new Vector3(10f, 10f, 0f)));
               effectList.Add(new MoveEffect(true, 5.0f, new Vector3(0f, 10f, 0f)));

               triangularZoomEvent = new CinematicEvent(effectList, true);
          }

          #endregion


          public override void UnloadContent()
          {
               base.UnloadContent();

               IsStartingFromAfterSkipText = true;
          }

          bool isSkippingIntroduction = false;

          #region Handle Input

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               // This prevents the user from providing input for the first 2.0 seconds.
               // This prevents him from Pressing Start right away.
               if (elapsedTime < 1.5f)
                    return;

               base.HandleInput(input, gameTime);











               PlayerIndex index;

               // Does the Player want to skip the Introduction text?
               if (input.IsNewButtonPress(Buttons.A, null, out index) || input.IsNewButtonPress(Buttons.Start, null, out index))
               {
                    // Pressing Start while on "Skip Story" prompt.
                    if (!isSkippingIntroduction)
                    {
                         AudioManager.PlayCue("ButtonPress");

                         isSkippingIntroduction = true;

                         // Have to update once or it flicckers?!?!
                         startMessage.Update(gameTime);
                         startMessage2.Update(gameTime);
                         titleMessage.Update(gameTime);

                         pressStartTextObject.Text = "Press Start To Continue";
                    }

                    // Pressing Start while on "Press Start" prompt.
                    else
                    {
                         AudioManager.PlayCue("ButtonPress");

                         updateCinematicEvent = false;

                         ControllingPlayer = index;

                         PixelEngine.EngineCore.ControllingPlayer = ControllingPlayer;

                         AvatarZombieGame.CurrentPlayer = new Player(ControllingPlayer.Value);

                         SignedInGamer gamer = Gamer.SignedInGamers[ControllingPlayer.Value];

                         if (gamer != null)
                         {
                              pName = gamer.Gamertag;

                              gamerSignedIn = true;
                         }

                         else
                         {
                              Guide.ShowSignIn(1, false);

                              gamerSignedIn = false;
                         }

                         if (gamer != null)
                         {
                              AvatarZombieGame.SaveGameData.PlayerName = pName;

#if XBOX
                              // Set the request flag
                              saveGameBeingRequested = true;
#else
                    ExitScreen();
                    ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
#endif
                         }
                    }
               }
          }

          /// <summary>
          /// Event handler for when the Start Game menu entry is selected.
          /// </summary>
          void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (!isSkippingIntroduction)
               {
                    return;
               }

               updateCinematicEvent = false;

               ControllingPlayer = e.PlayerIndex;

               PixelEngine.EngineCore.ControllingPlayer = ControllingPlayer;

               AvatarZombieGame.CurrentPlayer = new Player(e.PlayerIndex);

               SignedInGamer gamer = Gamer.SignedInGamers[ControllingPlayer.Value];

               if (gamer != null)
               {
                    pName = gamer.Gamertag;

                    gamerSignedIn = true;
               }

               else
               {
                    Guide.ShowSignIn(1, false);

                    gamerSignedIn = false;
               }

               if (gamer != null)
               {
                    AvatarZombieGame.SaveGameData.PlayerName = pName;

#if XBOX
                    // Set the request flag
                    saveGameBeingRequested = true;
#else
                    ExitScreen();
                    ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
#endif
               }
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               // Do nothing.
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);



               // Pressing Start while on "Skip Story" prompt.
               if (IsStartingFromAfterSkipText && !isSkippingIntroduction)
               {
                    isSkippingIntroduction = true;

                    // Have to update once or it flicckers?!?!
                    startMessage.Update(gameTime);
                    startMessage2.Update(gameTime);
                    titleMessage.Update(gameTime);

                    pressStartTextObject.Text = "Press Start To Continue";
               }



               pressStartTextObject.Update(gameTime);


               if (isSkippingIntroduction)
               {
                    startMessage.Update(gameTime);
                    startMessage2.Update(gameTime);

                    pressStartTextObject.Text = "Press Start To Continue";
               }

               else
               {
                    introductionTextObject.Update(gameTime);
                    startMessage2.Update(gameTime);

                    if (startMessage2.Position.Y <= EngineCore.ScreenCenter.Y - 25)
                    {
                         isSkippingIntroduction = true;
                    }
               }

               if (updateCinematicEvent)
               {
                    triangularZoomEvent.Update(gameTime);
               }

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;



               // New 12-03-2011: Removed IsActive check for text updates.
               if (IsActive)
               {
                    // If we have Pressed Start, but we are not Signed-In...
                    if (ControllingPlayer.HasValue)
                    {
                         if (Gamer.SignedInGamers[ControllingPlayer.Value] == null)//(!gamerSignedIn && ControllingPlayer.HasValue)
                         {
                              if (profilePromptRead)
                              {
                                   // Force the Player to Sign-In.

                                   // NEVERMIND! Don't force the Sign-In page to appear. NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
                                   //PromptForSignIn();
                              }

                              //profilePromptRead = false;
                              // new
                              if (Gamer.SignedInGamers[ControllingPlayer.Value] == null && !profilePromptRead)
                              {
                                   // If we reached this point, they must have not signed-in despite being prompted.
                                   int? profileRequiredPromptRead = SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Profile Required", "This game requires you to be signed into a Player Profile. Please select a Player Profile before you can continue.",
                                        new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                                   // If we reached this point, they must have read the Profile Required prompt.
                                   if (profileRequiredPromptRead.HasValue)
                                   {
                                        profilePromptRead = true;
                                   }
                              }

                              return;
                         }
                    }

                    // We don't save in Trial Mode, so just go to Main Menu.
                    if (saveGameBeingRequested && Guide.IsTrialMode)
                    {
                         SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Saving Disabled", "Since you are playing Trial Mode, your progress will not be saved during gameplay.",
                              new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);

                         AchievementManager.IsUpdating = true;

                         ScreenManager.RemoveScreen(this);
                    }


                    // Takes care of Guest-specific storage problem: Just don't let them save! 
                    else if (saveGameBeingRequested && AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsGuest)
                    {
                         SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Saving Disabled", "Since you are playing as a Guest, your progress will not be saved during gameplay.",
                                                   new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                         IsOnStartScreen = false;

                         ExitScreen();

                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                         AchievementManager.IsUpdating = true;
                    }
               }

               #region Save Process

               // If not Trial Mode, do the Save process.
               if (saveGameBeingRequested)
               {
                    message = "Saved";
                    saveGameBeingRequested = false;
               }

               if (message == "Saved" && !gameSavedAlready)
               {
                    gameSavedAlready = true;

                    if ((!Guide.IsVisible) && !saveRequested)//(StorageManager.SaveRequested == false))
                    {
                         // Added this Try Statement on 8-6-2010:
                         try
                         {
                              saveRequested = true;

                              result = StorageDevice.BeginShowSelector(ControllingPlayer.Value, null, null);
                         }
                         catch
                         {
                         }
                    }
               }

               // If a save is pending, save as soon as the storage device is chosen
               if (saveRequested && result != null)//(StorageManager.SaveRequested) && (result != null))
               {
                    if (result.IsCompleted)
                    {
                         StorageDevice device = StorageDevice.EndShowSelector(result);

                         StorageManager.Device = device;

                         StorageDevice.DeviceChanged += new EventHandler<EventArgs>(StorageManager.StorageDevice_DeviceChanged);

                         if (device != null && device.IsConnected)
                         {
                              AvatarZombieGame.LoadGame(device);

                              AvatarZombieGame.SaveGame(device);
                         }

                         // Reset the request flag
                         saveRequested = false;


                         IsOnStartScreen = false;

                         ExitScreen();





                         // LEADERBOARD CODE HERE

                         if (!Guide.IsTrialMode &&
                              AvatarZombieGame.CurrentPlayer != null &&
                              AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsSignedInToLive &&
                              AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.Privileges.AllowOnlineSessions)
                         {

                              // Should be in a different spot?
                              AvatarZombieGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 100);

                              //AvatarTypingGame.LoadHighScores(device);
                              //AvatarTypingGame.SaveHighScores(device);

                              if (AvatarZombieGame.OnlineSyncManager != null)
                                   AvatarZombieGame.OnlineSyncManager.start(AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer, AvatarZombieGame.HighScoreSaveData);
                         }
                         // END LEADERBOARD CODE


                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);

                         AchievementManager.IsUpdating = true;

                         if (!Guide.IsTrialMode)
                         {
                              AvatarZombieGame.AwardData.GamePurchased = true;
                         }
                    }
               }

               #endregion
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               MySpriteBatch.Begin();

               if (!isSkippingIntroduction)
               {
                    if (!IsStartingFromAfterSkipText)
                    {
                         introductionTextObject.Draw(gameTime);

                         startMessage2.Draw(gameTime);
                    }
               }

               else
               {
                    startMessage.Draw(gameTime);

                    titleMessage.Draw(gameTime);
               }


               pressStartTextObject.Draw(gameTime);

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion


          #region Prompt For Sign-In

          /// <summary>
          /// This forces a Profile to be Signed-In.
          /// </summary>
          private void PromptForSignIn()
          {
               if (ControllingPlayer.HasValue)
               {
                    SignedInGamer gamer = Gamer.SignedInGamers[ControllingPlayer.Value];

                    if (gamer == null)
                    {
                         gamerSignedIn = false;
                         Guide.ShowSignIn(1, false);
                    }

                    else
                    {
                         gamerSignedIn = true;
                    }
               }
          }

          #endregion
     }
}
