#region File Description
//-----------------------------------------------------------------------------
// ArcadeWithTutorialGameplayScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Screen for "Arcade Mode", which consists of progressively
     /// harder Waves of Enemies the player must ward off.
     /// 
     /// This Screen also gives a quick interactive tutorial.
     /// </remarks>
     public class ArcadeWithTutorialGameplayScreen : MenuScreen
     {
          #region TeachingState Enum

          public enum TeachingState
          {
               /// <summary>
               /// Teaching the Player how to Strafe Left.
               /// </summary>
               TeachingStrafeLeft,

               /// <summary>
               /// Teaching the Player how to Strafe Right.
               /// </summary>
               TeachingStrafeRight,

               /// <summary>
               /// Teaching the Player how to Shoot.
               /// </summary>
               TeachingShooting,

               /// <summary>
               /// Teaching the Player how to Reload.
               /// </summary>
               TeachingReloading,

               /// <summary>
               /// Teaches the Player how to perform an Active Reload.
               /// </summary>
               TeachingActiveReloading,

               /// <summary>
               /// Teaching the Player how to Swap Weapons.
               /// </summary>
               TeachingWeaponSwapping,

               /// <summary>
               /// Teaching the Player how to Throw Grenade.
               /// </summary>
               TeachingGrenadeThrowing,

               TutorialOver
          }

          #endregion


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



















          /// <summary>
          /// Our beloved Merchant's Avatar!
          /// </summary>
          public static Avatar MerchantAvatar = new Avatar(EngineCore.Game);

          /// <summary>
          /// The TeachingState we are currently in.
          /// Examples include: TeachingStrafe, TeachingShooting, etc.
          /// </summary>
          private TeachingState currentTeachingState;


          // Bools to keep track of the Player's actions.

          /// <summary>
          /// Whether or not the Player has Strafed yet.
          /// </summary>
          private bool hasPlayerStrafedLeft;

          private bool hasPlayerStrafedRight;

          /// <summary>
          /// Whether or not the Player has Shot yet.
          /// </summary>
          private bool hasPlayerShot;

          /// <summary>
          /// Whether or not the Player has Reloaded yet.
          /// </summary>
          private bool hasPlayerReloaded;

          /// <summary>
          /// Whether or not the Player has Swapped Weapons yet.
          /// </summary>
          private bool hasPlayerSwappedWeapons;

          /// <summary>
          /// Whether or not the Player has Thrown Grenade yet.
          /// </summary>
          private bool hasPlayerThrownNade;

          /// <summary>
          /// Rectangle representing Ned's dialog box.
          /// </summary>
          private Rectangle dialogRectangle;


          /// <summary>
          /// The TextObject which will be used to handle
          /// the Merchant's introduction dialogue.
          /// 
          /// It will use a Reading Text Effect.
          /// </summary>
          TextObject nedsDialogTextObject = new TextObject();

          /// <summary>
          /// An array of strings that the Merchant 
          /// may use as his introduction dialogue.
          /// </summary>
          private string[] nedsTalkingPoints = new string[] 
          { 
               "Tilt the Left Thumbstick left\nto strafe to the left.\nTry it now.",                  // Dialogue 1
               "Tilt the Left Thumbstick right\nto strafe to the right.\nTry it now.",                // Dialogue 2
               "Press Right Trigger to shoot.\nTry it now.",                                          // Dialogue 3
               "Press Right Bumper to\ninitiate a reload.\nTry it now.",                              // Dialogue 4
               "Press Right Bumper to\nstop your reload.\nTry it now.",                               // Dialogue 5
               "Press left or right on\nthe D-Pad to swap weapons.\nTry it now.",                     // Dialogue 6
               "Press Left Trigger to\nthrow a Grenade.\nTry it now.",                                // Dialogue 7
               "That's all you need to know.\nPeace, bitch.",                                         // Dialogue 8
               "Tutorial over."
          };

          Random random = new Random();

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public ArcadeWithTutorialGameplayScreen()
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







               MerchantAvatar.LoadAvatar(CustomAvatarType.Merchant);
               MerchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleIdleShiftWeight, true);
               MerchantAvatar.Position = new Vector3(-20f, 0f, 35f);
               MerchantAvatar.Scale = 1.75f;

               // The tutorial begins by teaching how to Strafe.
               currentTeachingState = TeachingState.TeachingStrafeLeft;

               // Initialize all the player action flags to false.
               hasPlayerStrafedLeft = false;
               hasPlayerStrafedRight = false;
               hasPlayerShot = false;
               hasPlayerReloaded = false;
               hasPlayerSwappedWeapons = false;
               hasPlayerThrownNade = false;

               dialogRectangle = new Rectangle((int)(EngineCore.ScreenCenter.X), (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 75), 600, 150);

               nedsDialogTextObject = new TextObject(nedsTalkingPoints[0],
                    new Vector2(dialogRectangle.X, dialogRectangle.Y), FontType.TitleFont, Color.White, 0f,
                    new Vector2(dialogRectangle.X, dialogRectangle.Y), 0.25f, true, new ReadingEffect(3.0f, nedsDialogTextObject.Text));

               nedsDialogTextObject.IsAutoCenter = true;
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


               if (tutorialCompleted)
                    return;

               PlayerIndex playerIndex;

               switch (currentTeachingState)
               {
                    case TeachingState.TeachingStrafeLeft:

                         if (this.arcadeLevel.CurrentPlayer.currentType == Player.PlayerAnimationType.StrafeLeft)
                         {
                              hasPlayerStrafedLeft = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[1];

                              currentTeachingState = TeachingState.TeachingStrafeRight;
                         }

                         break;

                    case TeachingState.TeachingStrafeRight:

                         if (this.arcadeLevel.CurrentPlayer.currentType == Player.PlayerAnimationType.StrafeRight)
                         {
                              hasPlayerStrafedRight = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[2];

                              currentTeachingState = TeachingState.TeachingShooting;
                         }

                         break;

                    case TeachingState.TeachingShooting:

                         if (input.IsNewButtonPress(Buttons.RightTrigger, EngineCore.ControllingPlayer, out playerIndex))
                         {
                              hasPlayerShot = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[3];

                              currentTeachingState = TeachingState.TeachingReloading;
                         }

                         break;

                    case TeachingState.TeachingReloading:

                         if (input.IsNewButtonPress(Buttons.RightShoulder, EngineCore.ControllingPlayer, out playerIndex))
                         {
                              hasPlayerReloaded = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[4];

                              currentTeachingState = TeachingState.TeachingActiveReloading;
                         }

                         break;

                    case TeachingState.TeachingActiveReloading:

                         if (input.IsNewButtonPress(Buttons.RightShoulder, EngineCore.ControllingPlayer, out playerIndex))
                         {
                              hasPlayerReloaded = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[5];

                              currentTeachingState = TeachingState.TeachingWeaponSwapping;
                         }

                         break;

                    case TeachingState.TeachingWeaponSwapping:

                         if (input.IsNewButtonPress(Buttons.DPadLeft, EngineCore.ControllingPlayer, out playerIndex) ||
                              input.IsNewButtonPress(Buttons.DPadRight, EngineCore.ControllingPlayer, out playerIndex))
                         {
                              hasPlayerSwappedWeapons = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[6];

                              currentTeachingState = TeachingState.TeachingGrenadeThrowing;
                         }

                         break;

                    case TeachingState.TeachingGrenadeThrowing:

                         if (input.IsNewButtonPress(Buttons.LeftTrigger, EngineCore.ControllingPlayer, out playerIndex))
                         {
                              hasPlayerThrownNade = true;

                              nedsDialogTextObject.Text = nedsTalkingPoints[7];

                              currentTeachingState = TeachingState.TutorialOver;
                         }

                         break;
               }
          }

          /// <summary>
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
               }

               // Otherwise, if we are in Third Person View and not on the Wave Complete Screen...
               else if (IsThirdPersonView)
               {
                    // Setr 3rd-person camera that follows the player.
                    CameraManager.ActiveCamera.LookAt = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                    CameraManager.ActiveCamera.Position = new Vector3(arcadeLevel.CurrentPlayer.Position.X, 3.5f, arcadeLevel.CurrentPlayer.Position.Z - 3f);
               }

               // Update the Screen(s)
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (IsActive)
               {
                   //if (tutorialCompleted)
                    {
                         #region Enemy Introduction Screen Logic

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





                    MerchantAvatar.Update(gameTime);

                    if (!tutorialCompleted)
                    {
                         nedsDialogTextObject.Update(gameTime);

                         switch (currentTeachingState)
                         {
                              case TeachingState.TeachingStrafeLeft:
                                   break;

                              case TeachingState.TeachingShooting:
                                   break;

                              case TeachingState.TeachingReloading:
                                   break;

                              case TeachingState.TeachingWeaponSwapping:
                                   break;

                              case TeachingState.TeachingGrenadeThrowing:
                                   break;
                         }

                         if (currentTeachingState == TeachingState.TutorialOver)
                         {
                              elapsedTutorialEndingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                              if (elapsedTutorialEndingTime <= 0f)
                              {
                                   tutorialCompleted = true;

                                   nedsDialogTextObject.Text = "";
                              }
                         }
                    }
               }
          }

          private bool tutorialCompleted = false;
          private float elapsedTutorialEndingTime = 3.0f;


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









               DrawMerchant(gameTime);


               if (!tutorialCompleted)
               {
                    MySpriteBatch.Begin();

                    GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D,
                         dialogRectangle, 2, Color.White * (TransitionAlpha / 255f));

                    MySpriteBatch.DrawCentered(blankTexture.Texture2D, dialogRectangle, Color.Black * 0.5f * (TransitionAlpha / 255f));

                    nedsDialogTextObject.Color = Color.Gold * (175f / 255f) * (TransitionAlpha / 255f);

                    nedsDialogTextObject.Draw(gameTime);

                    MySpriteBatch.End();
               }
          }

          #endregion


          #region Draw Merchant

          public void DrawMerchant(GameTime gameTime)
          {
               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    MerchantAvatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                    MerchantAvatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                    MerchantAvatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
               }

               MerchantAvatar.Draw(gameTime);
          }

          #endregion


          #region Draw Pre-Wave Information

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