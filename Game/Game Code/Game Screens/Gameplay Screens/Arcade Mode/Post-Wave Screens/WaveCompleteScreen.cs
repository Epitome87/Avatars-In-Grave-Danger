#region File Description
//-----------------------------------------------------------------------------
// WaveCompleteScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Avatars;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Screen containing the logic for when a Wave is Completed.
     /// Displays post-wave statistics such as number of kills, accuracy, etc.
     /// </remarks>
     public class WaveCompleteScreen : MenuScreen
     {
          int totalBonusMoney = 0;

          int bonusMoneyTime = 0;
          int bonusMoneyWave = 0;
          int bonusMoneyKills = 0;
          int bonusMoneyEscapes = 0;
          int bonusMoneyAccuracy = 0;


          #region Fields

          /// <summary>
          /// Our beloved Merchant's Avatar!
          /// </summary>
          public static Avatar merchantAvatar = new Avatar(EngineCore.Game);

          /// <summary>
          /// Boolean Flag that lets others know when this Menu Screen is being shown.
          /// </summary>
          public static bool IsScreenUp = false;

          /// <summary>
          /// This MenuScreen contains a sub-menu screen
          /// for purchasing upgrades.
          /// </summary>
          private UpgradeMenuScreen upgradeMenuScreen;

          /// <summary>
          /// The position for our various text objects.
          /// </summary>
          private Vector2 textPosition = new Vector2(EngineCore.ScreenCenter.X, 10);

          /// <summary>
          /// The number of the Wave that was just completed.
          /// </summary>
          private int waveNumber;

          /// <summary>
          /// The score the player achieved this completed Wave.
          /// </summary>
          private float score;

          /// <summary>
          /// The number of enemies killed this completed Wave.
          /// </summary>
          private float enemiesKilled;

          /// <summary>
          /// The number of shots the player fired this completed Wave.
          /// </summary>
          private float shotsFired;

          /// <summary>
          /// The number of hits that connected successfully this completed Wave.
          /// </summary>
          private float shotsConnected;

          /// <summary>
          /// The player's accuracy this completed Wave.
          /// </summary>
          private float accuracy;

          /// <summary>
          /// The number of enemies that escaped this completed Wave.
          /// </summary>
          private float escapes;

          /// <summary>
          /// The Wave Manager.
          /// </summary>
          private WaveManager waveManager;

          /// <summary>
          /// The Arcade Level we are completing Waves in.
          /// </summary>
          private ArcadeLevel theArcadeLevel;

          /// <summary>
          /// Save Game Data to save progress achieved this completed Wave.
          /// </summary>
          private SaveGameData data = new SaveGameData();

          /// <summary>
          /// A list of Text Objects we will render on this screen.
          /// </summary>
          private List<TextObject> textObjectList = new List<TextObject>();

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public WaveCompleteScreen(ArcadeLevel arcadeLevel, WaveManager _waveManager)
               : base("W A V E  " + (_waveManager.Round).ToString() + "  C L E A R E D !")
          {
               IsScreenUp = true;

               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(2.0);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);

               theArcadeLevel = arcadeLevel;
               waveManager = _waveManager;

               this.MenuTitleColor = Color.CornflowerBlue;

               MenuEntry upgradeMenuEntry = new MenuEntry("Purchase\nUpgrades", "Spend the money you have\nearned on Health & Upgrades");
               upgradeMenuEntry.Position = new Vector2(upgradeMenuEntry.Position.X, EngineCore.GraphicsDevice.Viewport.Height * 0.50f);
               upgradeMenuEntry.Selected += UpgradeMenuEntrySelected;
               MenuEntries.Add(upgradeMenuEntry);

               // Create our menu entries.
               MenuEntry continueMenuEntry = new MenuEntry("Begin\nNext Wave", "Continue to the next wave");

               // Hook up menu event handler.
               continueMenuEntry.Selected += ContinueMenuEntrySelected;

               // Set the menu entry's various properties.
               continueMenuEntry.Position = new Vector2(continueMenuEntry.Position.X, EngineCore.GraphicsDevice.Viewport.Height * 0.50f);

               // Add entries to the menu.
               MenuEntries.Add(continueMenuEntry);


               this.numberOfColumns = 1;

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.Position = new Vector2(entry.Position.X * 1.5f, 150f);
                    //new Vector2(entry.Position.X - 250f, entry.DescriptionPosition.Y - 100);
                    //new Vector2(entry.Position.X - 250f, entry.Position.Y + 10f);

                    entry.AdditionalVerticalSpacing = 60f;
                    entry.menuEntryBorderScale = new Vector2(1.0f, 1.0f);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;

                    entry.ShowGradientBorder = true;
                    entry.ShowBorder = false;

                    entry.menuEntryBorderSize = new Vector2(200, 200);
                    entry.useCustomBorderSize = true;
                    entry.FontScale = 0.75f;
                    entry.ShowIcon = false;

                    entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                    entry.SelectedBorderColor = Color.DarkOrange * (255f / 255f);
                    entry.DescriptionColor = Color.DarkOrange;
                    entry.DescriptionFontType = FontType.HudFont;
               }

               this.MenuTitleColor = Color.DarkOrange;
               this.MenuTitleFontScale *= 0.75f;

               // Retrieve the values for statistics to be shown.

               // Gather the wave number.
               waveNumber = waveManager.Round;

               // Gather the score achieved this wave.
               score = waveManager.CurrentWave.Score;

               // Gather the number of enemies killed this wave.
               enemiesKilled = waveManager.CurrentWave.EnemiesKilled;

               // Gather accuracy related stats.
               foreach (Gun gun in this.theArcadeLevel.CurrentPlayer.PlayersGuns)
               {
                    shotsFired += (int)gun.ShotsFired;
               }
               
               shotsConnected = waveManager.CurrentWave.NumberOfHits;
               accuracy = shotsConnected / shotsFired * 100;

               if (float.IsNaN(accuracy))
               {
                    accuracy = 100f;
               }

               // Gather the number of enemies that escaped.
               escapes = waveManager.CurrentWave.EnemiesEscaped;


               float time = (int)this.waveManager.CurrentWave.ElaspedWaveTime;


               int timeGoal = 15 * (waveNumber % 10);


               bonusMoneyTime = (int)((timeGoal / time) * 100);
               bonusMoneyWave = waveNumber * 50;
               bonusMoneyKills = (int)enemiesKilled * 10;
               bonusMoneyEscapes = (int)(escapes * -50f);
               bonusMoneyAccuracy = (int)((accuracy / 100f) * 250f);


               totalBonusMoney = bonusMoneyTime + bonusMoneyWave + bonusMoneyKills + bonusMoneyEscapes + bonusMoneyAccuracy;


               this.theArcadeLevel.CurrentPlayer.Money += totalBonusMoney;




               // Let our Award structure know what Wave we completed.
               AvatarZombieGame.AwardData.CurrentWave = (uint)waveNumber;

               // And what Accuracy we achieved.
               AvatarZombieGame.AwardData.Accuracy = (uint)accuracy;

               if (AvatarZombieGame.AwardData.Accuracy >= 100)
               {
                    AvatarZombieGame.AwardData.ConsecutivePerfectAccuracy++;
               }

               // Set the Save-Data info that's about to be saved.
               data.PlayerName = AvatarZombieGame.CurrentPlayer.GamerInformation.GamerTag;
               data.Level = (uint)waveNumber;
               data.HighScore = score;

               data.IsUnlockedAchievement = AchievementManager.IsUnlockedList;
               data.AwardData = AvatarZombieGame.AwardData;

               data.Difficulty = (int)ZombieGameSettings.Difficulty;

               // Let's reset shots fired here.
               this.theArcadeLevel.CurrentPlayer.Gun.ShotsFired = 0;





               merchantAvatar.LoadAvatar(CustomAvatarType.Merchant);
               merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleIdleShiftWeight, true);
               merchantAvatar.Position = new Vector3(-20f, 0f, 35f);
               merchantAvatar.Scale = 1.75f;

          }

          public override void LoadContent()
          {
               base.LoadContent();


#if XBOX
               // Set the request flag
               if (StorageManager.SaveRequested == false)
               {
                    StorageManager.SaveRequested = true;
               }
#endif
          }

          #endregion


          #region Handle Input

          void UpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               upgradeMenuScreen = new UpgradeMenuScreen(this.theArcadeLevel, this.waveManager);
               ScreenManager.AddScreen(upgradeMenuScreen, e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Continue Game menu entry is selected.
          /// </summary>
          void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               IsScreenUp = false;

               // Clear our list of text objects.
               textObjectList.Clear();

               // Remove the Player Screen.
               //ScreenManager.RemoveScreen(playerScreen);

               // And then remove this Wave Complete Screen.
               ScreenManager.RemoveScreen(this);


               // If we have not yet completed Arcade Mode...
               if (waveManager.Round <= ArcadeLevel.MAX_WAVE)
               {
                    // Load the Stage Introduction Screen via an Animated Loading Screen.
                    AnimatedLoadingScreen.Load(false, ControllingPlayer,                                       // is slow loading use to be = true
                         String.Format("Arcade Mode\nWave {0}", waveManager.Round), false,
                         new StageIntroScreen(theArcadeLevel, waveManager.CurrentWave));
               }

               // Otherwise, we've completed all the Waves, so...
               else
               {
                    // Remove all screens and add the Arcade Mode Complete Screen!
                    AnimatedLoadingScreen.RemoveAllButGameplayScreen();
                    ScreenManager.AddScreen(new ArcadeModeCompleteScreen(theArcadeLevel), ControllingPlayer);
               }
          }

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               base.HandleInput(input, gameTime);
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

               // Update all text objects.
               foreach (TextObject textObj in textObjectList)
               {
                    textObj.Update(gameTime);
               }

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
               }


               merchantAvatar.Update(gameTime);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               merchantAvatar.Draw(gameTime);

               MySpriteBatch.Begin();

               int xPos = (int)(EngineCore.ScreenCenter.X * 0.8f);

               Rectangle totalRect = new Rectangle(xPos, 500 - ((500 - 200) / 2), 600, (500 - 200) + 30);
               Rectangle rect1 = new Rectangle(xPos, 200, 600, 30);
               Rectangle rect3 = new Rectangle(xPos, 200 + ((500 - 200) / 2), 600, (500 - 200) - 30);
               Rectangle rect2 = new Rectangle(xPos, 500, 600, 30);


               // Make the menu slide into place during transitions, using a
               // power curve to make things look more interesting (this makes
               // the movement slow down as it nears the end).
               float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

               totalRect = new Rectangle(totalRect.X - (int)(transitionOffset * 512), 500 - ((500 - 200) / 2), 600, (500 - 200) + 30);
               rect1 = new Rectangle(rect1.X - (int)(transitionOffset * 512), 200, 600, 30);
               rect3 = new Rectangle(rect3.X - (int)(transitionOffset * 512), 200 + ((500 - 200) / 2), 600, (500 - 200) - 30);
               rect2 = new Rectangle(rect2.X - (int)(transitionOffset * 512), 500, 600, 30);


               MySpriteBatch.DrawCentered(blankTexture.Texture2D, rect1, Color.White * 0.5f * (TransitionAlpha / 255f));
               MySpriteBatch.DrawCentered(blankTexture.Texture2D, rect2, Color.White * 0.5f * (TransitionAlpha / 255f));
               MySpriteBatch.DrawCentered(blankTexture.Texture2D, rect3, Color.Black * 0.5f * (TransitionAlpha / 255f));

               TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Wave Results", new Vector2(rect1.X, rect1.Y), Color.White * (TransitionAlpha / 255f));

               SpriteFont sf = TextManager.Fonts[(int)FontType.HudFont].SpriteFont;

               float line1 = sf.MeasureString("Time:").X;
               float line2 = sf.MeasureString("Wave:").X;
               float line3 = sf.MeasureString("Kills:").X;
               float line4 = sf.MeasureString("Escapes:").X;
               float line5 = sf.MeasureString("Accuracy:").X;

               int timeTaken = (int)this.waveManager.CurrentWave.ElaspedWaveTime;

               GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, totalRect, 3, Color.Black * (TransitionAlpha / 255f));

               Color white = Color.White * (TransitionAlpha / 255f);

               // Right aligned on left side.
               TextManager.Draw(AlignmentType.Right, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Time:", new Vector2(rect1.X - 50f, rect1.Y + 25), white, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(AlignmentType.Right, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Wave:", new Vector2(rect1.X - 50f, rect1.Y + 75), white, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(AlignmentType.Right, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Kills:", new Vector2(rect1.X - 50f, rect1.Y + 125), white, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(AlignmentType.Right, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Escapes:", new Vector2(rect1.X - 50f, rect1.Y + 175), white, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(AlignmentType.Right, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Accuracy:", new Vector2(rect1.X - 50f, rect1.Y + 225), white, 0f, Vector2.Zero, 1.0f);


               Color gold = Color.Gold * (TransitionAlpha / 255f);

               // Left Aligned near center.
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, timeTaken.ToString(), new Vector2(rect1.X - 50f + 10f, rect1.Y + 25), gold, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, waveNumber.ToString(), new Vector2(rect1.X - 50f + 10f, rect1.Y + 75), gold, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, enemiesKilled.ToString(), new Vector2(rect1.X - 50f + 10f, rect1.Y + 125), gold, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, escapes.ToString(), new Vector2(rect1.X - 50f + 10f, rect1.Y + 175), gold, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, accuracy.ToString("#00"), new Vector2(rect1.X - 50f + 10f, rect1.Y + 225), gold, 0f, Vector2.Zero, 1.0f);


               Color green = Color.SpringGreen * (TransitionAlpha / 255f);

               // Left Aligned on right side.
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "$" + bonusMoneyTime.ToString(), new Vector2(rect3.X + 125, rect1.Y + 25), green, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "$" + bonusMoneyWave.ToString(), new Vector2(rect3.X + 125, rect1.Y + 75), green, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "$" + bonusMoneyKills.ToString(), new Vector2(rect3.X + 125, rect1.Y + 125), green, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "$" + bonusMoneyEscapes.ToString(), new Vector2(rect3.X + 125, rect1.Y + 175), green, 0f, Vector2.Zero, 1.0f);
               TextManager.Draw(TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "$" + bonusMoneyAccuracy.ToString(), new Vector2(rect3.X + 125, rect1.Y + 225), green, 0f, Vector2.Zero, 1.0f);


               TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Wave Bonus: $" + totalBonusMoney.ToString("N0"), new Vector2(rect2.X, rect2.Y), Color.White);


               MySpriteBatch.End();


               base.Draw(gameTime);
          }

          #endregion


          #region Helper Draw Merchant

          
          private static Random random = new Random();

          public static void DrawMerchant(GameTime gameTime)
          {
               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    merchantAvatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                    merchantAvatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                    merchantAvatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
               }

               merchantAvatar.Draw(gameTime);
          }
          
          #endregion
     }
}