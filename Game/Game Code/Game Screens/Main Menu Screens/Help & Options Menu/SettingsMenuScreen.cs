#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Storage;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// The Options screen is brought up over the top of the Main Menu
     /// screen, and gives the user a chance to configure the game.
     /// Menu Entries include: Difficulty, Language, Vibration, Music.
     /// </remarks>
     public class SettingsMenuScreen : MenuScreen
     {
          #region Fields

          // The Menu Entries.
          private MenuEntry difficultyMenuEntry;
          private MenuEntry vibrationMenuEntry;
          private BarMenuEntry musicVolumeEntry;
          private BarMenuEntry soundVolumeEntry;
          private MenuEntry storageMenuEntry;
          private MenuEntry safeAreaMenuEntry;

          // Helper variables for displaying the Menu Entry's contents.
          //private static Difficulty currentDifficulty = ZombieGameSettings.Difficulty;

          private bool isMenuVibrating = true;
          private float _elapsedTime;

          private bool storageRequested = false;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public SettingsMenuScreen()
               : base("S E T T I N G S")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               // Create our menu entries.
               difficultyMenuEntry = new MenuEntry(String.Empty, "Recommended for beginners.\nScore Multiplier: x 0.5");
               musicVolumeEntry = new BarMenuEntry(String.Empty, "Change the volume level\nof the music.");
               soundVolumeEntry = new BarMenuEntry(String.Empty, "Change the volume level\nof sound effects.");
               storageMenuEntry = new MenuEntry(String.Empty, "Change the Storage Device used for saving.\nAlso saves current game.");
               vibrationMenuEntry = new MenuEntry(String.Empty, "Turn Vibration on & off.");
               safeAreaMenuEntry = new MenuEntry("Safe Area", "o");

               // Check to see if the Pause Menu is up, meaning we are visiting Settings from in-game.
               for (int i = 0; i < ScreenManager.GetScreens().Length; i++)
               {
                    // And if we are in the Pause Menu...
                    if (ScreenManager.GetScreens()[i].GetType().Equals(typeof(PauseMenuScreen)))
                    {
                         // Do not let the user mess with Difficulty!
                         difficultyMenuEntry.SelectedColor = Color.Gray;
                         difficultyMenuEntry.UnselectedColor = Color.Gray;
                         difficultyMenuEntry.Description = "Cannot change Difficulty while in-game.\nReturn to Main Menu to change Difficulty.";
                    }
               }


               this.TransitionOnTime = TimeSpan.FromSeconds(1.0);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0);


               //isMenuVibrating = AvatarTypingGameSettings.VibrationEnabled;


               //SetDifficulty();
               SetMenuEntryText();

               MenuEntry backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.");
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 30f);

               // Set the Bars for the Music and Sound entries to full.
               musicVolumeEntry.CurrentBar = ZombieGameSettings.MusicVolume;// startingMusicVolume;// musicVolumeEntry.NumberOfBars;
               soundVolumeEntry.CurrentBar = ZombieGameSettings.SoundVolume;// startingSoundVolume;//soundVolumeEntry.NumberOfBars;

               // Hook up menu event handlers.
               //difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
               vibrationMenuEntry.Selected += VibrationMenuEntrySelected;
               musicVolumeEntry.Selected += MusicVolumeEntrySelected;
               soundVolumeEntry.Selected += SoundVolumeEntrySelected;
               storageMenuEntry.Selected += StorageMenuEntrySelected;
               safeAreaMenuEntry.Selected += SafeAreaMenuEntrySelected;

               backMenuEntry.Selected += OnCancel;

               if (Guide.IsTrialMode)
               {
                    storageMenuEntry.SelectedColor = Color.Gray;
                    storageMenuEntry.UnselectedColor = Color.Gray;
                    storageMenuEntry.Description = "\nNot Available In Trial Mode.";
               }

               // Add entries to the menu.
               //MenuEntries.Add(difficultyMenuEntry);
               MenuEntries.Add(musicVolumeEntry);
               MenuEntries.Add(soundVolumeEntry);
               MenuEntries.Add(vibrationMenuEntry);
               MenuEntries.Add(storageMenuEntry);
               //MenuEntries.Add(safeAreaMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.menuEntryBorderSize = new Vector2(550f, 100f);
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + (EngineCore.GraphicsDevice.Viewport.Height * 0.1f));
                    entry.IsPulsating = false;
                    entry.ShowBorder = false;
                    entry.SelectedColor = entry.UnselectedColor;
                    entry.ShowGradientBorder = true;

                    entry.AdditionalVerticalSpacing = 30;
               }

               storageMenuEntry.DescriptionFontScale *= 0.80f;

               _elapsedTime = 0.0f;     
          }

          /// <summary>
          /// Fills in the latest values for the options screen menu text.
          /// </summary>
          void SetMenuEntryText()
          {
               //difficultyMenuEntry.Text = "Difficulty: " + currentDifficulty;

               string vibrationString = ZombieGameSettings.VibrationEnabled == true ? "Enabled" : "Disabled";
               vibrationMenuEntry.Text = "Vibration: " + vibrationString;

               musicVolumeEntry.Text = "Music Volume:         ";
               soundVolumeEntry.Text = "Sound Volume:         ";

               storageMenuEntry.Text = "Select Storage Device";
               storageMenuEntry.Description = "Change the Storage Device used for saving.\nAlso saves current game.";
          }

          #endregion


          #region Handle Input

          void SafeAreaMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new CustomizeableSafeAreaScreen(), EngineCore.ControllingPlayer);

          }

          bool is1080p = false;

          void VibrationMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ZombieGameSettings.VibrationEnabled = !ZombieGameSettings.VibrationEnabled;

               if (ZombieGameSettings.VibrationEnabled)
               {
                    isMenuVibrating = true;
               }

               else
               {
                    isMenuVibrating = false;
                    _elapsedTime = 0.0f;
               }

               InputState.IsVibrationEnabled = ZombieGameSettings.VibrationEnabled;

               SetMenuEntryText();



               /*
               is1080p = !is1080p;

               // We want 1080P.
               if (is1080p)
               {
                    foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    {
                         if (displayMode.Width == 1920 && displayMode.Height == 1080)
                         {
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferHeight = displayMode.Height;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferWidth = displayMode.Width;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferFormat = displayMode.Format;

                              EngineCore.GraphicsDeviceManager.PreferredBackBufferWidth = displayMode.Width;
                              EngineCore.GraphicsDeviceManager.PreferredBackBufferHeight = displayMode.Height;



                              PresentationParameters pp = EngineCore.GraphicsDevice.PresentationParameters;

                              EngineCore.GraphicsDevice.Reset(pp);
                              //EngineCore.GraphicsDeviceManager.ApplyChanges();
                         }
                    }
               }

               // We want 720P, or the next best.
               else
               {
                    foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    {
                         if (displayMode.Width == 640 && displayMode.Height == 480)
                         {
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferHeight = 720;// displayMode.Height;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferWidth = 1280;// displayMode.Width;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferFormat = displayMode.Format;
                         }

                         // 720P supported, so break!
                         if (displayMode.Width == 1280 && displayMode.Height == 720)
                         {
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferHeight = displayMode.Height;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferWidth = displayMode.Width;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferFormat = displayMode.Format;

                              EngineCore.GraphicsDeviceManager.PreferredBackBufferWidth = displayMode.Width;
                              EngineCore.GraphicsDeviceManager.PreferredBackBufferHeight = displayMode.Height;



                              PresentationParameters pp = EngineCore.GraphicsDevice.PresentationParameters;
                              EngineCore.GraphicsDevice.Reset(pp);

                              break;
                         }

                         // In case 1080P is somehow supported but 720P isn't...?!
                         if (displayMode.Width == 1920 && displayMode.Height == 1080)
                         {
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferHeight = displayMode.Height;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferWidth = displayMode.Width;
                              EngineCore.GraphicsDevice.PresentationParameters.BackBufferFormat = displayMode.Format;
                         }
                    }

                    
               }
               EngineCore.ScreenCenter =
                         new Vector2(EngineCore.GraphicsDevice.PresentationParameters.BackBufferWidth / 2, EngineCore.GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
               */
          }


          /// <summary>
          /// Event handler for when the Music Volume menu entry is selected.
          /// </summary>
          void MusicVolumeEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               AudioManager.MusicAudioCategory.SetVolume(
                    MathHelper.Clamp(musicVolumeEntry.Value, 0.0f, 1.0f));

               ZombieGameSettings.MusicVolume = musicVolumeEntry.CurrentBar;
          }

          /// <summary>
          /// Event handler for when the Sound Volume menu entry is selected.
          /// </summary>
          void SoundVolumeEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               AudioManager.SoundAudioCategory.SetVolume(
                    MathHelper.Clamp(soundVolumeEntry.Value, 0.0f, 1.0f));

               ZombieGameSettings.SoundVolume = soundVolumeEntry.CurrentBar;
          }

          IAsyncResult result;

          private void StorageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
                    return;

               bool doingStorageShit = true;

               if (doingStorageShit)
               {
                    doingStorageShit = false;

                    if ((!Guide.IsVisible) && !storageRequested)
                    {
                         try
                         {
                              result = StorageDevice.BeginShowSelector(e.PlayerIndex, null, null);

                              storageRequested = true;
                         }
                         catch
                         {
                         }
                    }
               }
          }

          /// <summary>
          /// Overridden Event handler for when the user cancels the menu.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               base.OnCancel(playerIndex);
               InputState.SetVibration(ControllingPlayer.Value, 0.0f, 0.0f, 0.0f);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
               
               // Unless we are on the Vibration entry, we do not want the controller to vibrate.
               if (SelectedMenuEntry != 2)//1)
               {
                    InputState.SetVibration(ControllingPlayer.Value, 0.0f, 0.0f, 0.0f);

                    isMenuVibrating = false;
                    _elapsedTime = 0.0f;
               }

               // We are on Vibration entry, so let's check for that hidden Award!
               else
               {
                    if (isMenuVibrating)
                    {
                         InputState.SetVibration(ControllingPlayer.Value, 0.5f, 0.5f, 0.1f);

                         _elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                         if (_elapsedTime >= 30000.0f)
                         {
                              AvatarZombieGame.AwardData.VibrationSet = true;
                         }
                    }
               }
               

               // If a save is pending, save as soon as the storage device is chosen
               if (storageRequested && result != null)
               {
                    if (result.IsCompleted)
                    {
                         StorageDevice device = StorageDevice.EndShowSelector(result);

                         StorageManager.Device = device;

                         StorageDevice.DeviceChanged += new EventHandler<EventArgs>(StorageManager.StorageDevice_DeviceChanged);

                         if (device != null && device.IsConnected)
                         {
                              AvatarZombieGame.SaveGame(device);
                         }

                         // Reset the request flag.
                         storageRequested = false;

                         storageMenuEntry.Description = "Storage selection & save complete!\nIf you weren't prompted to choose a device,\nyou don't have multiple present.";
                    }
               }
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
