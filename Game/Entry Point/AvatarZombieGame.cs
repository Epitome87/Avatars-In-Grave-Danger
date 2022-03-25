#region File Description
//-----------------------------------------------------------------------------
// AvatarTypingGame.cs
// Copyright (C) Matt McGrath. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// Avatar Typing game. Action typing game for the 360.
     /// Most of the game logic is found within the ScreenManager component.
     /// </remarks>
     public class AvatarZombieGame : EngineCore
     {
          #region Fields

          // A global reference to the Arcade Level.
          public static ArcadeLevel CurrentArcadeLevel = null;





          // A global reference to the Player.
          public static Player CurrentPlayer = null;

          // A global reference to Save-Game data.
          public static SaveGameData SaveGameData = new SaveGameData(); // Converted to Class, so added = new SaveGameData()

          // A global reference to Achievement data.
          public static AchievementData AwardData = new AchievementData();  // Converted to Class, so added = new AchievementData()

          // Global Achievement-Related Variables.
          public static bool GamePurchased = false;
          public static bool CreditsWatched = false;




          public static bool SeizureModeEnabled = false;//true;

          /// <summary>
          /// A GraphicsInfo object must be created and passed into the
          /// EngineCore Constructor, in order to determine how the
          /// PixelEngine will be Initialized.
          /// </summary>
          private static GraphicsInfo gi = new GraphicsInfo
          {
               ScreenWidth = (int)(1280),
               ScreenHeight = (int)(720),
               PreferMultiSampling = true,
               IsFullScreen = true
          };

          #endregion


          #region Save Highscore Shit

          public static Stream GlobalScoreStream;
          public static Leaderboards.TopScoreListContainer HighScoreSaveData;

          public static void ResetHighScores(StorageDevice device)
          {
               // Open a storage container.
               StorageContainer container = StorageManager.OpenContainer(device, "Avatars In Grave Danger - HighScores");

               // Get the path of the save game.
               string filename = "highScores.sav";

               // Check to see whether the save exists.
               if (!container.FileExists(filename))
               {
                    // Notify the user there is no save.
                    container.CreateFile(filename);

                    AvatarZombieGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 25000);
               }

               // Open the file.
               else
               {
                    // Open the file, creating it if necessary.
                    using (Stream stream = container.OpenFile(filename, FileMode.Truncate, FileAccess.ReadWrite))
                    {
                         BinaryReader reader = new BinaryReader(stream);

                         AvatarZombieGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 25000);

                         reader.Close();
                    }
               }

               // Dispose the container, to commit changes.
               container.Dispose();




               // Enter the score into the Leaderboard.
               Leaderboards.TopScoreEntry leaderboardEntry = new Leaderboards.TopScoreEntry("i Epitome i", 25000);
               AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

               leaderboardEntry = new Leaderboards.TopScoreEntry("Pixel Pysche", 15000);
               AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

               leaderboardEntry = new Leaderboards.TopScoreEntry("Frank West", 10000);
               AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

               leaderboardEntry = new Leaderboards.TopScoreEntry("Chuck Greene", 7500);
               AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

               leaderboardEntry = new Leaderboards.TopScoreEntry("Jill  V", 5000);
               AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

               leaderboardEntry = new Leaderboards.TopScoreEntry("Some Noob", 1000);
               AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

               // Save the high scores.
               AvatarZombieGame.SaveHighScores(PixelEngine.Storage.StorageManager.Device);
          }

          public static void SaveHighScores(StorageDevice device)
          {
               // Show "Now Saving" animation?
               ScreenManager.AddPopupScreen(new PixelEngine.SavePopUpScreen("! Saving Scores"), EngineCore.ControllingPlayer.Value);

               if (device != null && device.IsConnected)
               {
                    // Open a storage container.
                    StorageContainer container = StorageManager.OpenContainer(device, "Avatars In Grave Danger - HighScores");

                    // Get the path of the save game.
                    string filename = "highScores.sav";


                    // Open the file, creating it if necessary.
                    using (Stream stream = container.OpenFile(filename, FileMode.Create))
                    {
                         BinaryWriter writer = new BinaryWriter(stream);

                         AvatarZombieGame.HighScoreSaveData.Save(writer);

                         GlobalScoreStream = stream;

                         writer.Close();
                    }

                    // Dispose the container, to commit changes.
                    container.Dispose();
               }
          }

          public static void LoadHighScores(StorageDevice device)
          {
               // Should be in a different spot?
               //AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 10);

               // NEW
               // Open a storage container.
               StorageContainer container = StorageManager.OpenContainer(device, "Avatars In Grave Danger - HighScores");

               // Get the path of the save game.
               string filename = "highScores.sav";

               // Check to see whether the save exists.
               if (!container.FileExists(filename))
               {
                    // Notify the user there is no save.
                    container.CreateFile(filename);

                    AvatarZombieGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 25000);//10);
               }

               // Open the file.
               else
               {
                    // Open the file, creating it if necessary.
                    using (Stream stream = container.OpenFile(filename, FileMode.Open, FileAccess.Read))//filename, FileMode.Create))
                    {
                         BinaryReader reader = new BinaryReader(stream);

                         if (stream.Length <= 0)
                         {
                              //   AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 10);
                         }

                         else
                         {
                              AvatarZombieGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(reader);
                         }

                         reader.Close();
                    }
               }

               // Dispose the container, to commit changes.
               container.Dispose();
          }

          #endregion


          #region SaveGame and LoadGame - Temporary Locations

          public static void SaveGame(StorageDevice device)
          {
               SaveGameData data = new SaveGameData();

               data = AvatarZombieGame.SaveGameData;
               data.IsUnlockedAchievement = AchievementManager.IsUnlockedList;
               data.AwardData = AvatarZombieGame.AwardData;
               data.Difficulty = (int)ZombieGameSettings.Difficulty;

               data.VibrationEnabled = ZombieGameSettings.VibrationEnabled;

               data.SoundVolume = ZombieGameSettings.SoundVolume;
               data.MusicVolume = ZombieGameSettings.MusicVolume;

               AudioManager.SoundAudioCategory.SetVolume(MathHelper.Clamp(ZombieGameSettings.SoundVolume / 10f, 0.0f, 1.0f));
               AudioManager.MusicAudioCategory.SetVolume(MathHelper.Clamp(ZombieGameSettings.MusicVolume / 10f, 0.0f, 1.0f));

               if (device != null && device.IsConnected)
               {
                    StorageManager.DoSaveGame(device, data);
               }
          }

          public static bool LoadGame(StorageDevice device)
          {
               bool saveExisted = false;

               saveExisted = StorageManager.DoLoadGame(device);

               AvatarZombieGame.SaveGameData = StorageManager.SaveData;
               AvatarZombieGame.AwardData = AvatarZombieGame.SaveGameData.AwardData;

               // Test for achievement fix
               AvatarZombieGame.AwardData.CurrentWave = 0;
               // End Test for achievement fix


               // Grab the User's Achievements.
               if (AvatarZombieGame.SaveGameData.IsUnlockedAchievement != null)
               {
                    int i = 0;

                    foreach (bool isUnlocked in AvatarZombieGame.SaveGameData.IsUnlockedAchievement)
                    {
                         AchievementManager.Achievements[i++].IsUnlocked = isUnlocked;
                    }
               }

               // If a previous save does not exist...
               if (!saveExisted)
               {
                    // We need to default various Settings.
                    AvatarZombieGame.SaveGameData.VibrationEnabled = true;
                    AvatarZombieGame.SaveGameData.SoundVolume = 10;
                    AvatarZombieGame.SaveGameData.MusicVolume = 10;
               }

               // Grab the User's Settings.
               ZombieGameSettings.VibrationEnabled = AvatarZombieGame.SaveGameData.VibrationEnabled;
               ZombieGameSettings.Difficulty = (Difficulty)AvatarZombieGame.SaveGameData.Difficulty;
               ZombieGameSettings.SoundVolume = AvatarZombieGame.SaveGameData.SoundVolume;
               ZombieGameSettings.MusicVolume = AvatarZombieGame.SaveGameData.MusicVolume;

               return saveExisted;
          }

          #endregion


          #region Constructor

          /// <summary>
          /// The main Game constructor.
          /// </summary>
          public AvatarZombieGame(GraphicsInfo graphics)
               : base(graphics)
          {
               Content.RootDirectory = "Content";

               #region Debug: Safe Area
               // Draw Safe Area if in Debug && Xbox
#if XBOX && DEBUG
              /*
               PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay safeAreaOverlay;
               safeAreaOverlay = new PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay(this);
               Components.Add(safeAreaOverlay);
               safeAreaOverlay.Visible = true;
              */

               //Components.Add(new PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay(this));
               //PixelEngine.DebugUtilities.FpsCounter.Initialize(this);

#endif
               #endregion

               // Set presence mode to "At Menu".
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode = GamerPresenceMode.AtMenu;
               }

               // Simulate Trial Mode.
               //Guide.SimulateTrialMode = true;

               SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
               SignedInGamer.SignedOut += new System.EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
          }

          #endregion


          public static Leaderboards.OnlineDataSyncManager OnlineSyncManager;


          #region Initialize

          protected override void Initialize()
          {
               // Create and add the Screen Manager component.
               ScreenManager.Initialize(this);

               // Add a GamerServiceComponent to list.
               Components.Add(new GamerServicesComponent(this));

               // Create and add the Audio Manager.
               AudioManager.Initialize(this, @"Content\MyGameAudio.xgs", @"Content\Wave Bank.xwb", @"Content\Sound Bank.xsb");

               // Create and add the Award Manager.
               AchievementManager.Initialize(this);

               // Create and add the Text Handler.
               TextManager.Initialize(this);

               // Create and add the Camera Manager.
               CameraManager.Initialize(this);

               // Create and add the Content Manager.
               ResourceManager.Initialize(this);

               AvatarManager.Initialize(this);

               //this.IsFixedTimeStep = false;

               OnlineSyncManager = new Leaderboards.OnlineDataSyncManager(0, this);
               Components.Add(OnlineSyncManager);



               AchievementList.InitializeAchievements();

               



               //Components.Add(new MessageDisplayComponent(this));

               // Listen for invite notification events.
               //NetworkSession.InviteAccepted += (sender, e) => NetworkSessionComponent.InviteAccepted(ScreenManager.GetInstance, e);



               AudioManager.SoundAudioCategory.SetVolume(MathHelper.Clamp(ZombieGameSettings.SoundVolume / 10f, 0.0f, 1.0f));
               AudioManager.MusicAudioCategory.SetVolume(MathHelper.Clamp(ZombieGameSettings.MusicVolume / 10f, 0.0f, 1.0f));


               base.Initialize();


               Guide.NotificationPosition = NotificationPosition.TopCenter;

               // Activate the first screen.
               PixelEngine.Screen.ScreenManager.AddScreen(new SplashScreen(), null);
          }

          #endregion


          #region Draw

          /// <summary>
          /// This is called when the game should draw itself.
          /// </summary>
          protected override void Draw(GameTime gameTime)
          {
               //TitleSafeRenderingComponent.GetInstance().PrepareForTitleSafeZoomDraw();


               // The real drawing happens inside the EngineCore,
               // which calls upon game components such as ScreenManager.
               base.Draw(gameTime);

               if (ShowSignedOutMessageBox)
               {                                    
                    if (!Guide.IsVisible)
                    {
                         // New 12-03-2011 for crash.
                         if (!ControllingPlayer.HasValue)
                         {
                              ShowSignedOutMessageBox = false;
                         }

                         else
                         {
                              Guide.BeginShowMessageBox(ControllingPlayer.Value,
                                        "Sign-In Status Changed",
                                        "You have returned to the Start Screen because the active player profile has signed out. Please ensure you do not sign out of the active profile while playing.",
                                        new string[] { "Continue" }, 0, MessageBoxIcon.None, SignInCompleteCallback, null);
                         }
                    }
               }
          }

          #endregion


          #region Entry Point

          /// <summary>
          /// The main entry point for the application.
          /// </summary>
          static void Main(string[] args)
          {
               try
               {
                    using (AvatarZombieGame game = new AvatarZombieGame(gi))
                    {
                         game.Run();
                    }
               }
               
               
               catch (Exception e)
               {
                    using (CrashDebugGame game = new CrashDebugGame(e))
                    {
                         game.Run();
                    }
               }
               
               
          }

          #endregion


          #region SignedInGamer Events

          /// <summary>
          /// Event hooked to when a gamer Signs In.
          /// Note: This also is triggered automatically upon game-start.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
          {
               if (EngineCore.ControllingPlayer.HasValue)
                    return;

               AvatarZombieGame.CurrentPlayer = new Player(e.Gamer.PlayerIndex);

               AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer = e.Gamer;

               AvatarZombieGame.SaveGameData.PlayerName = e.Gamer.Gamertag;
          }

          public bool ShowSignedOutMessageBox = false;

          /// <summary>
          /// Event hooked to when a gamer Signs Out.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
          {
               // Check to see if the Gamer that Signed-Out is our Current Player.
               if (AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer == e.Gamer)
               {
                    GameplayBackgroundScreen.isUpdate = true;

                    // New Testing
                    AnimatedLoadingScreen.RemoveAllButGameplayScreen();
                    AvatarZombieGame.CurrentPlayer = null;



                    // NEW FOR LEADERBOARDS

                    if (AvatarZombieGame.OnlineSyncManager != null)
                         AvatarZombieGame.OnlineSyncManager.stop(null, false);

                    // END NEW FOR LEADERBOARDS


                    ScreenManager.AddScreen(new StartScreen(), e.Gamer.PlayerIndex);

                    ShowSignedOutMessageBox = true;
               }
          }

          void SignInCompleteCallback(IAsyncResult r)
          {
               int? button = Guide.EndShowMessageBox(r);

               ShowSignedOutMessageBox = false;
          }

          #endregion
     }
}
