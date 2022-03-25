#region File Description
//-----------------------------------------------------------------------------
// ArcadeGameplayScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Screen for "Arcade Mode", which consists of progressively
     /// harder Waves of Enemies the player must ward off.
     /// </remarks>
     public class ArcadeGameplayScreen : MenuScreen
     {
          #region Fields

          private float elapsedTime;
          private bool updatedOnce;

          /// <summary>
          /// The Arcade Level this screen is displaying.
          /// </summary>
          private ArcadeLevel arcadeLevel;

          /// <summary>
          /// The Counter object that will handle counting down the seconds
          /// before the Wave begins.
          /// </summary>
          private Counter roundCountdown = new Counter();

          /// <summary>
          /// Whether or not we are going to introduce an Enemy to the Player.
          /// 
          /// If true, we go render text that describes a specific type of Enemy.
          /// </summary>
          private static bool introduceEnemy = true;


          /// <summary>
          /// Text Object to handle rendering of the "Wave Survived!" string that
          /// emerges after the wave has been defeated and before the Wave Complete Screen shows up.
          /// </summary>
          private TextObject waveSurvivedTextObject =
               new TextObject("Wave Survived!", EngineCore.ScreenCenter, FontType.TitleFont, Color.DarkOrange, 0.0f, Vector2.Zero, 0.75f, true);


          private bool IsFirstPersonView
          {
               get
               {
                    return (ArcadeLevel.IsFirstPerson && !WaveCompleteScreen.IsScreenUp);
               }
          }

          private bool IsThirdPersonView
          {
               get
               {
                    return (!ArcadeLevel.IsFirstPerson && !WaveCompleteScreen.IsScreenUp);
               }
          }

          #endregion


          #region Properties

          /// <summary>
          /// Whether or not we should "introduce" an Enemy.
          /// If True, an EnemyIntroductionScreen is shown before the Wave begins.
          /// This screen will describe the new Enemy type that will be appearing.
          /// </summary>
          public static bool IntroduceEnemy
          {
               get { return introduceEnemy; }
               set { introduceEnemy = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public ArcadeGameplayScreen()
               : base("")
          {
               waveSurvivedTextObject.IsCenter = true;
               waveSurvivedTextObject.TextEffect = new MoveInEffect(1.0f, waveSurvivedTextObject.Text, new Vector2(EngineCore.ScreenCenter.X, -200f), EngineCore.ScreenCenter);

               this.AlwaysDraw = true;

               AchievementManager.IsUnlockNow = false;

               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.5f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               elapsedTime = 0f;


               Stage myStage = new GraveyardStage(EngineCore.Game);
               arcadeLevel = new ArcadeLevel(EngineCore.Game, myStage);

               // NEW on 7-18 to avoid CA2000
               myStage.Dispose();
          }

          public override void LoadContent()
          {
               base.LoadContent();

               arcadeLevel.LoadContent();

               Grenade preloadNade = new Grenade(EngineCore.Game);
          }

          #endregion


          #region Handle Input

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               // Don't allow input if Countdown is Active.
               if (roundCountdown != null && roundCountdown.Active)
               {
                    return;
               }

               // Pause if "Pause" was pressed, or if controller was disconnected 
               // (assuming it was ever connected in the first place).
               if (input.IsPauseGame(ControllingPlayer) || input.GamePadWasDisconnected[(int)ControllingPlayer.Value])
               {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
               }

               // Don't allow input if this isn't the active screen.
               if (this.IsActive)
               {
                    // Let the Level handle its own input.
                    arcadeLevel.HandleInput(input, gameTime);
               }
          }

          /// <summary>
          /// THESE NEVER GET CALLED DO THEYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               const string message = "Return to Main Menu?";

               MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, "Accept", "Cancel");

               confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

               ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
          }

          /// <summary>
          /// THESE NEVER GET CALLED DO THEYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
          /// Event handler for when the user selects Accept on the "are you sure
          /// you want to exit" message box.
          /// </summary>
          void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               //openingLevel.UnloadLevelContent();

               AvatarZombieGame.AwardData.CurrentWave = 0;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               // If we are in First Person View and not on the Wave Complete Screen...
               if (IsFirstPersonView)
               {
                    // Set 1st-person camera that follows the player.
                    CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 2.5f, arcadeLevel.CurrentPlayer.Position.Z + 0.5f);


                    // With new strafe animation: If we want the hand in center.
                    CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X + 0.2f, 2.25f, arcadeLevel.CurrentPlayer.Position.Z + 0.25f);

                    // With new strafe animation: If we want the hand to the right.
                    CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X + 0.3f, 2.25f, arcadeLevel.CurrentPlayer.Position.Z + 0.375f);





                    // With new strafe animation: If we want the hand to the right.
                    CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X + 0.3f, 2.25f, arcadeLevel.CurrentPlayer.Position.Z - 1f);


                   Vector3 headScale = new Vector3();
                   Quaternion headRotation = new Quaternion();
                   Vector3 headPos = new Vector3();
                   arcadeLevel.CurrentPlayer.Avatar.BonesInWorldSpace[(int)AvatarBone.Head].Decompose(out headScale, out headRotation, out headPos);

                   CameraManager.ActiveCamera.LookAt = new Vector3(headPos.X, 0.0f, 20f);
                   CameraManager.ActiveCamera.Position = new Vector3(headPos.X + 0.15f, headPos.Y + 0.15f, headPos.Z - 0.10f);// - 0.15f);
               }

               // Otherwise, if we are in Third Person View and not on the Wave Complete Screen...
               else if (IsThirdPersonView)
               {
                    // Set 3rd-person camera that follows the player.
                    CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 3.5f, arcadeLevel.CurrentPlayer.Position.Z - 3f);


                    //CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    //CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 3.5f, arcadeLevel.CurrentPlayer.Position.Z - 5f);
               }

               // Update the Screen(s)
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (IsActive)
               {
                    #region Enemy Introduction Screen Logic
                    /*
                    if (introduceEnemy && (ZombieGameSettings.Difficulty == Difficulty.Normal))// || AvatarTypingGameSettings.Difficulty == Difficulty.Normal))
                    {
                         switch (this.openingLevel.CurrentWaveManager.Round)
                         {
                              case 1:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Slinger, "New Enemy Encountered!"), ControllingPlayer);
                                   break;

                              case 3:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Beserker, "New Enemy Encountered!"), ControllingPlayer);
                                   break;

                              case 5:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Warper, "New Enemy Encountered!"), ControllingPlayer);
                                   break;

                              case 7:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Horde, "New Enemy Encountered!"), ControllingPlayer);
                                   break;

                              case 10:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Boss, "New Enemy Encountered!"), ControllingPlayer);
                                   break;
                         }

                         introduceEnemy = false;
                    }
                    */

                    #endregion

                    elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    #region Countdown Logic

                    if (!updatedOnce)
                    {
                         arcadeLevel.Update(gameTime);

                         updatedOnce = true;

                         roundCountdown = new Counter(5, 0, "Get Ready!\n", "Stop\nThe Zombies!", "Countdown_Boom", 1);

                    }

                    if (roundCountdown != null && roundCountdown.Active)
                    {
                         return;
                    }

                    #endregion

                    // Update the actual Level logic.
                    arcadeLevel.Update(gameTime);

                    #region Check for Game Over

                    // If the Current Player is no longer Alive...
                    if (!arcadeLevel.CurrentPlayer.IsAlive)
                    {
                         // Show the Game Over Screen.
                         ScreenManager.AddScreen(new GameOverArcadeModeScreen(arcadeLevel.CurrentPlayer, arcadeLevel.CurrentWaveManager.CurrentWave,
                                   arcadeLevel.TotalScore), arcadeLevel.CurrentPlayer.GamerInformation.PlayerIndex);

                         // And Exit the Arcade Gameplay screen.
                         ScreenManager.RemoveScreen(this);

                         return;
                    }

                    #endregion

                    #region Check if Wave has been Destroyed

                    // If the Current Wave has been destroyed...
                    if (arcadeLevel.CurrentWaveManager.CurrentWave.WaveOver && arcadeLevel.CurrentPlayer.IsAlive)
                    {
                         // TODO Make the player celebrate?
                         isWaveGracePeriod = true;

                         waveOverGracePeriod -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                         if (waveOverGracePeriod <= 0)
                         {
                              waveOverGracePeriod = 3.5f;

                              // Show the Wave Complete screen.
                              arcadeLevel.WaveComplete();

                              updatedOnce = false;

                              isWaveGracePeriod = false;

                              waveSurvivedTextObject.TextEffect =
                                   new MoveInEffect(1.0f, waveSurvivedTextObject.Text, new Vector2(EngineCore.ScreenCenter.X, -200f), EngineCore.ScreenCenter);
                         }
                    }

                    #endregion

                    #region Check for Game Complete

                    if (arcadeLevel.CurrentWaveManager.IsAllWavesFinished)
                    {
                         // Show the Arcade Mode Complete Screen!
                         ScreenManager.AddScreen(new ArcadeModeCompleteScreen(arcadeLevel), ControllingPlayer);

                         // And Exit the Arcade Gameplay screen.
                         ExitScreen();
                    }

                    #endregion
               }
          }


          private bool isWaveGracePeriod = false;
          private float waveOverGracePeriod = 3.5f;

          #endregion


          #region Main Draw Method

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.SpriteBatch.GraphicsDevice.Clear(Color.Pink);

               if (!IsActive)
               {
                    arcadeLevel.DrawWithoutHUD(gameTime);
                    return;
               }

               // Have the Level call its Draw logic.
               arcadeLevel.Draw(gameTime);

               // If our Countdown isn't null, and it's in an Active state...
               if (roundCountdown != null && roundCountdown.Active)
               {
                    // Fade the Arcade Gameplay screen a little.
                    ScreenManager.FadeBackBufferToBlack(255 * 2 / 5);

                    MySpriteBatch.Begin(BlendState.AlphaBlend);

                    // Render the Countdown text.
                    roundCountdown.DisplayCountdown(gameTime);

                    DrawWaveInformation(gameTime);

                    MySpriteBatch.End();
               }



               if (isWaveGracePeriod)
               {
                    MySpriteBatch.Begin(BlendState.AlphaBlend);
                    waveSurvivedTextObject.Update(gameTime);
                    waveSurvivedTextObject.Draw(gameTime);
                    MySpriteBatch.End();
               }

               //base.Draw(gameTime); COMMENTED OUT 11-20-2011 IN ATTEMPTS TO FIX BARRIER DRAWING
          }

          private void DrawWaveInformation(GameTime gameTime)
          {
               int enemyCount = this.arcadeLevel.CurrentWaveManager.CurrentWave.EnemiesRemaining;
               int waveNum = this.arcadeLevel.CurrentWaveManager.Round;
               int enemyHealthFactor = ((this.arcadeLevel.CurrentWaveManager.CurrentWave.WaveNumber) / 10);
               float enemySpeedFactor = this.arcadeLevel.CurrentWaveManager.CurrentWave.WaveSpeedFactor;
               string enemyTypes = String.Empty;

               switch (this.arcadeLevel.CurrentWaveManager.CurrentWave.WaveNumber)
               {
                    case 0:
                         enemyTypes = "- Slingers";
                         break;

                    case 1:
                         enemyTypes = "- Slingers";
                         break;

                    case 2:
                         enemyTypes = "- Slingers\n- Beserkers";
                         break;

                    case 3:
                         enemyTypes = "- Slingers\n- Beserkers";
                         break;

                    case 4:
                         enemyTypes = "- Slingers\n- Beserkers\n- Warpers";
                         break;

                    case 5:
                         enemyTypes = "- Slingers\n- Beserkers\n- Warpers";
                         break;

                    case 6:
                         enemyTypes = "- Slingers\n- Beserkers\n- Warpers";
                         break;

                    case 7:
                         enemyTypes = "- Slingers\n - Beserkers\n- Warpers";
                         break;

                    case 8:
                         enemyTypes = "- Slingers\n- Beserkers\n- Warpers\n- Giants";
                         break;

                    default:
                         enemyTypes = "- Slingers\n- Beserkers\n- Warpers\n- Giants";
                         break;
               }

               SpriteFont sf = TextManager.Fonts[(int)FontType.HudFont].SpriteFont;
               float fontScale = 0.75f;

               float fontHeight = (fontScale * sf.MeasureString(enemyTypes).Y) + (25 / 2);
               float fontWidth = (fontScale * sf.MeasureString("Enemy Types:   - Beserkers").X) + 35f;

               int width = (int)fontWidth;// (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Width * 0.55f);
               int height = 100 + (int)fontHeight;// 225;

               // Y Value we center on.
               int yPos = EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top + (height / 2) + (30 / 2);
               int xPos = (int)(EngineCore.ScreenCenter.X);


               Rectangle totalRect = new Rectangle(xPos, yPos - (30 / 2), width, height + 30 - 30);
               Rectangle waveHeaderRectangle = new Rectangle(xPos, yPos - (height / 2) - 1, width, 30);
               Rectangle waveCenterRectangle = new Rectangle(xPos, yPos, width, height - 30);
               Rectangle waveFooterRectangle = new Rectangle(xPos, yPos + (height / 2), width, 30);


               // Make the menu slide into place during transitions, using a
               // power curve to make things look more interesting (this makes
               // the movement slow down as it nears the end).
               float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

               totalRect = new Rectangle(totalRect.X - (int)(transitionOffset * 512), yPos - (30 / 2), width, height + 30 - 30);
               waveHeaderRectangle = new Rectangle(waveHeaderRectangle.X - (int)(transitionOffset * 512), yPos - (height / 2) - 1, width, 30);
               waveCenterRectangle = new Rectangle(waveCenterRectangle.X - (int)(transitionOffset * 512), yPos, width, height - 30);
               waveFooterRectangle = new Rectangle(waveFooterRectangle.X - (int)(transitionOffset * 512), yPos + (height / 2), width, 30);


               MySpriteBatch.DrawCentered(blankTexture.Texture2D, waveHeaderRectangle, Color.White * 0.5f * (TransitionAlpha / 255f));
               MySpriteBatch.DrawCentered(blankTexture.Texture2D, waveCenterRectangle, Color.Black * 0.5f * (TransitionAlpha / 255f));
               //MySpriteBatch.DrawCentered(blankTexture.Texture2D, waveFooterRectangle, Color.White * 0.5f * (TransitionAlpha / 255f));

               TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Wave " + waveNum.ToString() + " Incoming!", new Vector2(waveHeaderRectangle.X, waveHeaderRectangle.Y), Color.White * (TransitionAlpha / 255f));


               float line1 = sf.MeasureString("Enemy Count:").X * fontScale;
               float line2 = sf.MeasureString("Enemy Health:").X * fontScale;
               float line3 = sf.MeasureString("Enemy Speed:").X * fontScale;
               float line4 = sf.MeasureString("Enemy Types:").X * fontScale;

               GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, totalRect, 3, Color.Black * (TransitionAlpha / 255f));

               Color white = Color.White * (TransitionAlpha / 255f);


               float aWidth = (fontScale * sf.MeasureString("Enemy Health:").X);
               float aBoxX = waveCenterRectangle.X - (width / 2) + aWidth;

               // Right aligned on left side.
               TextManager.Draw(AlignmentType.Right, sf, "Enemy Count:", new Vector2(aBoxX, waveHeaderRectangle.Y + 25), white, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(AlignmentType.Right, sf, "Enemy Health:", new Vector2(aBoxX, waveHeaderRectangle.Y + 50), white, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(AlignmentType.Right, sf, "Enemy Speed:", new Vector2(aBoxX, waveHeaderRectangle.Y + 75), white, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(AlignmentType.Right, sf, "Enemy Types:", new Vector2(aBoxX, waveHeaderRectangle.Y + 100), white, 0f, Vector2.Zero, fontScale);

               Color green = Color.SpringGreen * (TransitionAlpha / 255f);


               float myWidth = (fontScale * sf.MeasureString("- Beserkers").X);
               float thisBoxX = waveCenterRectangle.X + (width / 2) - myWidth;

               // Left Aligned on right side.
               TextManager.Draw(sf, enemyCount.ToString(), new Vector2(thisBoxX, waveHeaderRectangle.Y + 25), green, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(sf, "+ " + enemyHealthFactor.ToString(), new Vector2(thisBoxX, waveHeaderRectangle.Y + 50), green, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(sf, "x " + enemySpeedFactor.ToString("#0.00"), new Vector2(thisBoxX, waveHeaderRectangle.Y + 75), green, 0f, Vector2.Zero, fontScale);
               TextManager.Draw(sf, enemyTypes.ToString(), new Vector2(thisBoxX, waveHeaderRectangle.Y + 100), green, 0f, Vector2.Zero, fontScale);


               //TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "UHM", new Vector2(waveFooterRectangle.X, waveFooterRectangle.Y), Color.White);

          }


          #endregion


          #region Unloading Content

          public override void UnloadContent()
          {
               // Have the Level release all of its content.
               arcadeLevel.UnloadLevelContent();
          }

          #endregion
     }
}