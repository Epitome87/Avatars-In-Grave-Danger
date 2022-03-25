#region File Description
//-----------------------------------------------------------------------------
// GameOverArcadeModeScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Menu Screen that occurs upon Game Over.
     /// 
     /// This Screen alerts the Player that it is Game Over and gives
     /// him a quick summary of his statistics, such as Enemies Killed,
     /// Enemies Escaped, and his final Score.
     /// 
     /// From this Screen the Player can share his Score with his friends 
     /// or return to the Main Menu.
     /// </remarks>
     public class GameOverArcadeModeScreen : MenuScreen
     {
          #region Fields

          GameResourceTexture2D skyTexture;

          GameResourceTexture2D gradientTexture;

          Player losingPlayer;

          TextObject gameOverText;

          float totalScore = 0;

          Wave wave;



          Vector3 avatarRenderPosition = new Vector3(0f, 2.5f, -2f);
          Vector3 cameraRenderPosition = Vector3.Zero;


          int totalEnemiesKilled;
          int totalEnemiesEscaped;
          int totalWaveReached;


          SpriteFont sf = TextManager.Fonts[(int)FontType.HudFont].SpriteFont;

          float fontScale = 0.75f;


          float fontHeight;
          float fontWidth;
          int width;
          int height;
          int xPos;
          int yPos;


          Rectangle totalRect;
          Rectangle waveHeaderRectangle;
          Rectangle waveCenterRectangle;
          Rectangle waveFooterRectangle;


          string resultString;
          string waveString;
          string killString;
          string escapeString;
          string scoreString;
          string scoreValueString;
          string scoreAndValueString;


          float line1;
          float line2;
          float line3;

          #endregion


          #region Initialization

          public GameOverArcadeModeScreen(Player theLosingPlayer, Wave _wave, int finalScore)
               : base("You Have Become Infected")
          {
               wave = _wave;

               this.MenuTitleFontScale = 1.0f;

               //AudioManager.PushMusic("GretaSting");

               losingPlayer = theLosingPlayer;

               losingPlayer.UnequipWeapon();

               losingPlayer.Avatar.Rotation = new Vector3(0f, 0f, 0f);
               losingPlayer.Avatar.Scale = 2.0f;

               // Set up the cool blue lighting!
               losingPlayer.Avatar.LightDirection = new Vector3(1, 1, 0);
               losingPlayer.Avatar.LightColor = new Vector3(0, 0, 10);
               losingPlayer.Avatar.AmbientLightColor = Color.CornflowerBlue.ToVector3();

               //losingPlayer.IsAlive = true;
               losingPlayer.Avatar.PlayAnimation(AnimationType.ZombieWalk, true);

               losingPlayer.Position = new Vector3(0f);


               totalScore = finalScore + _wave.Score;

               gameOverText = new TextObject("Kills: " + _wave.WaveManager.EnemiesKilled.ToString() + "\nWave " + (_wave.WaveNumber + 1).ToString() +
                    "\nTotal Score\n" + totalScore.ToString(), new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y * 0.75f));
               gameOverText.FontType = FontType.TitleFont;
               gameOverText.IsCenter = true;
               gameOverText.Scale = 1.25f * 0.35f;
               gameOverText.Origin = gameOverText.Font.MeasureString(gameOverText.Text) / 2;
               gameOverText.Color = Color.Gold;
               gameOverText.AddTextEffect(new TypingEffect(3000f, gameOverText.Text));

               TransitionOnTime = TimeSpan.FromSeconds(2.5f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               MenuEntry tellFriendMenuEntry = new MenuEntry("Share Score", "Show off your score to friends!");
               MenuEntry quitMenuEntry = new MenuEntry("Main Menu", "Return to Main Menu.");

               tellFriendMenuEntry.DescriptionPosition = new Vector2(tellFriendMenuEntry.DescriptionPosition.X, tellFriendMenuEntry.DescriptionPosition.Y);
               quitMenuEntry.DescriptionPosition = new Vector2(quitMenuEntry.DescriptionPosition.X, quitMenuEntry.DescriptionPosition.Y);

               tellFriendMenuEntry.Position = new Vector2(tellFriendMenuEntry.Position.X, tellFriendMenuEntry.DescriptionPosition.Y - 300f);
               quitMenuEntry.Position = new Vector2(quitMenuEntry.Position.X, quitMenuEntry.DescriptionPosition.Y - 280f);

               tellFriendMenuEntry.Selected += TellFriendMenuEntrySelected;
               quitMenuEntry.Selected += QuitMenuEntrySelected;

               tellFriendMenuEntry.BorderColor = Color.Gold * (50f / 255f);
               quitMenuEntry.BorderColor = Color.Gold * (50f / 255f);

               tellFriendMenuEntry.SelectedColor = Color.Gold;
               quitMenuEntry.SelectedColor = Color.Gold;

               MenuEntries.Add(tellFriendMenuEntry);
               MenuEntries.Add(quitMenuEntry);

               this.MenuTitleColor = Color.DarkOrange;

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.ShowGradientBorder = false;
                    entry.ShowBorder = true;

                    entry.BorderColor = Color.Black * (0.5f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.35f;

                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.34f;
                    entry.DescriptionColor = Color.DarkOrange;
               }





               totalEnemiesKilled = this.wave.WaveManager.EnemiesKilled;
               totalEnemiesEscaped = this.wave.WaveManager.EnemiesEscaped;
               totalWaveReached = this.wave.WaveManager.Round;



               fontHeight = (fontScale * sf.MeasureString("").Y) + (25 / 2);
               fontWidth = (fontScale * sf.MeasureString("Enemy Types:   - Beserkers").X) + 50f;

               width = (int)fontWidth;
               height = 125 + (30);

               // Y Value we center on.
               yPos = (EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top + 100) + (height / 2) + (30 / 2);
               xPos = (int)(EngineCore.ScreenCenter.X);


               totalRect = new Rectangle(xPos, yPos, width, height + 30);
               waveHeaderRectangle = new Rectangle(xPos, yPos - (height / 2) - 1, width, 30);
               waveCenterRectangle = new Rectangle(xPos, yPos, width, height - 30);
               waveFooterRectangle = new Rectangle(xPos, yPos + (height / 2), width, 30);


               resultString = "Results";
               waveString = "Waves Survived:";
               killString = "Zombies Stopped:";
               escapeString = "Zombies Escaped:";
               scoreString = "Final Score: ";
               scoreValueString = totalScore.ToString("N0");
               scoreAndValueString = scoreString + scoreValueString;


               line1 = sf.MeasureString(waveString).X * fontScale;
               line2 = sf.MeasureString(killString).X * fontScale;
               line3 = sf.MeasureString(escapeString).X * fontScale;




               if (!Guide.IsTrialMode)
               {
                    // New on 12-03-2011
                    if (AvatarZombieGame.HighScoreSaveData == null)
                         return;


                    // Enter the score into the Leaderboard.
                    Leaderboards.TopScoreEntry leaderboardEntry = new Leaderboards.TopScoreEntry(AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.Gamertag, (int)totalScore);
                    AvatarZombieGame.HighScoreSaveData.AddEntry(0, leaderboardEntry, AvatarZombieGame.OnlineSyncManager);

                    leaderboardEntry = new Leaderboards.TopScoreEntry("i Epitome i", 25000);
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
          }

          public override void LoadContent()
          {
               base.LoadContent();

               skyTexture = ResourceManager.LoadTexture(@"Textures\Backgrounds\Graveyard_Background");

               gradientTexture = ResourceManager.LoadTexture(@"Textures\Gradients\Gradient_BlackToWhite1");
          }

          public override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion


          #region Menu Events

          /// <summary>
          /// Event handler for when the Play Game menu entry is selected.
          /// </summary>
          void TellFriendMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (losingPlayer == null)
                    return;

               if (losingPlayer.GamerInformation.Gamer == null)
                    return;

               /*
               if (losingPlayer.GamerInformation.Gamer.IsGuest ||
                   !AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsSignedInToLive)
                    return;
               */




               // Why does this pop up only every other click?!?!
               if (AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsGuest || !AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsSignedInToLive)
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Score Sharing Disabled",
                         "You cannot share scores with friends. To share scores with friends, please make sure you are signed into Live, on a non-Guest account, and have the appropriate Privileges.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }




               string waveString = (AvatarZombieGame.AwardData.CurrentWave + 1).ToString();

               if (AvatarZombieGame.AwardData.CurrentWave <= 0)
                    waveString = "1";

               string scoreString = (totalScore).ToString();
               string difficultyString = ZombieGameSettings.Difficulty.ToString();

               Guide.ShowComposeMessage(losingPlayer.GamerInformation.PlayerIndex,
                    "I progressed to Wave #" + waveString + " on " + difficultyString +
                    " with a score of " + scoreString + " points on 'Avatars In Grave Danger'! Think you can beat me? Visit Game Marketplace - Games & Demos - Indie Games & purchase the game to try!", null);
          }

          /// <summary>
          /// Event handler for when the Play Game menu entry is selected.
          /// </summary>
          void QuitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               //losingPlayer.IsAlive = true;

               AvatarZombieGame.AwardData.CurrentWave = 0;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               // Do not let the Player back out of this screen.
          }

          /// <summary>
          /// Event handler for when the user selects Accept on the "are you sure
          /// you want to exit" message box.
          /// </summary>
          void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               //losingPlayer.IsAlive = true;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               if (losingPlayer != null)
                    losingPlayer.Avatar.Update(gameTime);

               gameOverText.Update(gameTime);

               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               MySpriteBatch.Begin();
               MySpriteBatch.Draw(skyTexture.Texture2D, new Rectangle(0, 0, EngineCore.GraphicsDevice.Viewport.Width, EngineCore.GraphicsDevice.Viewport.Height), Color.White);
               MySpriteBatch.End();

               if (losingPlayer != null)
               {
                    losingPlayer.Avatar.DrawToScreen(gameTime, avatarRenderPosition, cameraRenderPosition);
               }

               MySpriteBatch.Begin();

               DrawOverallStatistics(gameTime);

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion


          #region Draw Overall Statistics

          private void DrawOverallStatistics(GameTime gameTime)
          {
               totalRect = new Rectangle(xPos, yPos, width, height + 30);
               waveHeaderRectangle = new Rectangle(xPos, yPos - (height / 2) - 1, width, 30);
               waveCenterRectangle = new Rectangle(xPos, yPos, width, height - 30);
               waveFooterRectangle = new Rectangle(xPos, yPos + (height / 2), width, 30);


               // Make the menu slide into place during transitions, using a
               // power curve to make things look more interesting (this makes
               // the movement slow down as it nears the end).
               float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

               totalRect = new Rectangle(totalRect.X - (int)(transitionOffset * 512), yPos, width, height + 30);
               waveHeaderRectangle = new Rectangle(waveHeaderRectangle.X - (int)(transitionOffset * 512), yPos - (height / 2) - 1, width, 30);
               waveCenterRectangle = new Rectangle(waveCenterRectangle.X - (int)(transitionOffset * 512), yPos, width, height - 30);
               waveFooterRectangle = new Rectangle(waveFooterRectangle.X - (int)(transitionOffset * 512), yPos + (height / 2), width, 30);


               MySpriteBatch.DrawCentered(blankTexture.Texture2D, waveHeaderRectangle, Color.White * 0.5f * (TransitionAlpha / 255f));
               MySpriteBatch.DrawCentered(gradientTexture.Texture2D, waveCenterRectangle, Color.Black * 0.5f * (TransitionAlpha / 255f));
               MySpriteBatch.DrawCentered(blankTexture.Texture2D, waveFooterRectangle, Color.White * 0.5f * (TransitionAlpha / 255f));


               TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, resultString,
                    new Vector2(waveHeaderRectangle.X, waveHeaderRectangle.Y), Color.White * (TransitionAlpha / 255f));

               GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, totalRect, 4, Color.White * (TransitionAlpha / 255f));

               Color white = Color.White * (TransitionAlpha / 255f);


               float aWidth = (fontScale * sf.MeasureString(escapeString).X);
               float aBoxX = waveCenterRectangle.X - (width / 2) + aWidth + 50;

               // Right aligned on left side.
               TextManager.Draw(AlignmentType.Right, sf, waveString, new Vector2(aBoxX, waveHeaderRectangle.Y + 25), white, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(AlignmentType.Right, sf, killString, new Vector2(aBoxX, waveHeaderRectangle.Y + 50), white, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(AlignmentType.Right, sf, escapeString, new Vector2(aBoxX, waveHeaderRectangle.Y + 75), white, 0f, Vector2.Zero, fontScale);


               Color green = Color.SpringGreen * (TransitionAlpha / 255f);


               float myWidth = (fontScale * sf.MeasureString("- Beserkers").X);
               float thisBoxX = waveCenterRectangle.X + (width / 2) - myWidth + 50;

               // Left Aligned on right side.
               TextManager.Draw(sf, totalWaveReached.ToString(), new Vector2(thisBoxX, waveHeaderRectangle.Y + 25), green, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(sf, totalEnemiesKilled.ToString(), new Vector2(thisBoxX, waveHeaderRectangle.Y + 50), green, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(sf, totalEnemiesEscaped.ToString(), new Vector2(thisBoxX, waveHeaderRectangle.Y + 75), green, 0f, Vector2.Zero, fontScale);


               TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, scoreAndValueString,
                    new Vector2(waveFooterRectangle.X, waveFooterRectangle.Y), Color.White);
          }

          #endregion
     }
}