
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
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
     /// A quick congratulatory screen presented to the Player
     /// when he reaches the end of Arcade Mode.
     /// </remarks>
     public class ArcadeModeCompleteScreen : MenuScreen
     {
          #region Fields

          Player winningPlayer;

          GameResourceTexture2D skyTexture;

          TextObject gameOverText;

          float totalScore = 0;
          string scoreString = String.Empty;

          Avatar playersAvatar;

          #endregion


          #region Initialization

          public ArcadeModeCompleteScreen(ArcadeLevel completedLevel)
               : base("Congratulations!\nYou Completed All Waves!")
          {
               winningPlayer = completedLevel.CurrentPlayer;

               winningPlayer.UnequipWeapon();

               playersAvatar = new Avatar(EngineCore.Game);
               playersAvatar.LoadUserAvatar(EngineCore.ControllingPlayer.Value);

               playersAvatar.Position = new Vector3(0f, -1f, 1f);
               playersAvatar.Rotation = new Vector3(0f, 0f, 0f);
               playersAvatar.Scale = 2.5f;

               //winningPlayer.IsAlive = true;
               playersAvatar.PlayAnimation(AvatarAnimationPreset.Celebrate, true);


               totalScore = completedLevel.TotalScore;

               gameOverText = new TextObject("Final Score\n" + totalScore.ToString(), new Vector2(EngineCore.ScreenCenter.X, 250f));
               gameOverText.FontType = FontType.MenuFont;
               gameOverText.IsCenter = true;
               gameOverText.Scale = 1.75f;
               gameOverText.Origin = gameOverText.Font.MeasureString(gameOverText.Text) / 2;
               gameOverText.Color = Color.Gold;
               gameOverText.AddTextEffect(new TypingEffect(5000f, gameOverText.Text));

               TransitionOnTime = TimeSpan.FromSeconds(5.0f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               MenuEntry tellFriendMenuEntry = new MenuEntry("Tell a Friend!", "Show off your High Score!");
               MenuEntry quitMenuEntry = new MenuEntry("Main Menu", "Return to Main Menu.");

               tellFriendMenuEntry.DescriptionPosition = new Vector2(tellFriendMenuEntry.DescriptionPosition.X, tellFriendMenuEntry.DescriptionPosition.Y + 40f);
               quitMenuEntry.DescriptionPosition = new Vector2(quitMenuEntry.DescriptionPosition.X, quitMenuEntry.DescriptionPosition.Y + 40f);

               tellFriendMenuEntry.Position = new Vector2(tellFriendMenuEntry.Position.X, tellFriendMenuEntry.DescriptionPosition.Y - 260);
               quitMenuEntry.Position = new Vector2(quitMenuEntry.Position.X, quitMenuEntry.DescriptionPosition.Y - 250);

               tellFriendMenuEntry.Selected += TellFriendMenuEntrySelected;
               quitMenuEntry.Selected += QuitMenuEntrySelected;

               tellFriendMenuEntry.BorderColor = Color.Gold * (50f / 255f);
               quitMenuEntry.BorderColor = Color.Gold * (50f / 255f);

               tellFriendMenuEntry.SelectedColor = Color.Gold;
               quitMenuEntry.SelectedColor = Color.Gold;

               MenuEntries.Add(tellFriendMenuEntry);
               MenuEntries.Add(quitMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.ShowGradientBorder = false;
                    entry.ShowBorder = true;
               }
          }

          public override void LoadContent()
          {
               base.LoadContent();

               skyTexture = ResourceManager.LoadTexture(@"Textures\Backgrounds\Graveyard_Background");
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
               if (winningPlayer == null)
                    return;

               if (winningPlayer.GamerInformation.Gamer == null)
                    return;


               // Why does this pop up only every other click?!?!
               if (AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsGuest || !AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsSignedInToLive)
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Score Sharing Disabled",
                         "You cannot share scores with friends. To share scores with friends, please make sure you are signed into Live, on a non-Guest account, and have the appropriate Privileges.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }


               string waveString = (AvatarZombieGame.AwardData.CurrentWave).ToString();

               if (AvatarZombieGame.AwardData.CurrentWave <= 0)
                    waveString = "1";

               string scoreString = (totalScore).ToString();
               string difficultyString = ZombieGameSettings.Difficulty.ToString();

               Guide.ShowComposeMessage(winningPlayer.GamerInformation.PlayerIndex,
                    "I stopped the zombie invasion " + //+ difficultyString +
                    "with a score of " + scoreString + " points on 'Avatars In Grave Danger'! Think you can beat me? Visit Game Marketplace - Games & Demos - Indie Games & purchase the game to try!", null);
          }

          /// <summary>
          /// Event handler for when the Play Game menu entry is selected.
          /// </summary>
          void QuitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               //winningPlayer.IsAlive = true;

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
               //winningPlayer.IsAlive = true;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               if (playersAvatar != null)
               {
                    playersAvatar.Update(gameTime);
               }

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

               if (winningPlayer != null)
               {
                    playersAvatar.DrawToScreen(gameTime, new Vector3(0f, 0f, 0f), new Vector3(0f, -1f, 2f));
               }

               MySpriteBatch.Begin();

               gameOverText.Draw(gameTime);

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}
