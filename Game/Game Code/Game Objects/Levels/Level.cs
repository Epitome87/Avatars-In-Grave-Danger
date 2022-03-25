#region File Description
//-----------------------------------------------------------------------------
// Level.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A small environment with collections of items and enemies.
     /// The level owns the player and controls the game's win and lose
     /// conditions as well as scoring.
     /// 
     /// Since there are various Level types, this class is abstract.
     /// </remarks>
     public abstract class Level : DrawableGameComponent
     {
          #region Fields

          // A List of Players playing this Level.
          private List<Player> players;

          // A Level contains a Stage - the physical layout of the Level.
          protected Stage stage;

          // A Level contains a Wave Manager, as there are Wave objects within the Level.
          protected WaveManager waveManager;

          // A Level contains a Player, or Players.
          protected Player thePlayer;

          // General Resources used in all Levels.

          protected GameResourceTexture2D blankTexture;
          protected GameResourceTexture2D borderTexture;

          protected GameResourceTexture2D blankHudBgTexture;
          protected GameResourceTexture2D blankHudBgBorderTexture;

          // Resources used for HUD.
          protected GameResourceTexture2D heartIconTexture;
          protected GameResourceTexture2D enemyIconTexture;
          protected GameResourceTexture2D speedyIconTexture;
          protected GameResourceTexture2D perfectIconTexture;
          public GameResourceFont hudFont;

          protected GameResourceTexture2D heartLeftIconTexture;
          protected GameResourceTexture2D heartRightIconTexture;
          #endregion


          #region Properties

          /// <summary>
          /// Gets or sets a List of Players (human-controlled) who are part of the Level.
          /// </summary>
          public List<Player> ActivePlayers
          {
               get { return players; }
               set { players = value; }
          }

          /// <summary>
          /// Gets or sets the Stage this Level contains;
          /// that is, an object that defines the appearance of the Level,
          /// contains SceneObjects, and adds those to the SceneGraphManager.
          /// </summary>
          public Stage Stage
          {
               get { return stage; }
               set { stage = value; }
          }

          /// <summary>
          /// Gets or sets the Player who is participating in this ArcadeLevel instance. 
          /// </summary>
          public Player CurrentPlayer
          {
               get { return thePlayer; }
               set { thePlayer = value; }
          }

          /// <summary>
          /// Gets or sets the Enemy Manager managing this ArcadeLevel instance's enemies.
          /// </summary>
          public WaveManager CurrentWaveManager
          {
               get { return waveManager; }
               set { waveManager = value; }
          }

          /// <summary>
          /// Gets the Player's Score obtained in this ArcadeLevel instance.
          /// </summary>
          public int Score
          {
               get { return waveManager.Score; }
          }

          public int TotalScore
          {
               get { return waveManager.Score + waveManager.CurrentWave.Score; }
          }

          /// <summary>
          /// Gets the number of Enemies Killed during this ArcadeLevel instance.
          /// </summary>
          public int EnemiesKilled
          {
               get { return waveManager.CurrentWave.EnemiesKilled; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Level Constructor.
          /// </summary>
          public Level(Game game)
               : base(game)
          {
               hudFont = ResourceManager.LoadFont(@"Fonts\BankGothicMd_Regular_28_NOBORDER");
          }

          new public virtual void LoadContent()
          {
               base.LoadContent();
          }

          /*
          protected override void UnloadContent()
          {
               base.UnloadContent();
          }
          */
          protected void LoadPlayers()
          {
          }

          protected void LoadEnemies()
          {
          }

          protected void LoadScripts()
          {
          }

          protected void LoadWave()
          {
          }

          protected void LoadEnvironment()
          {
          }

          #endregion


          #region Handle Input

          public virtual void HandleInput(InputState input, GameTime gameTime)
          {
          }

          #endregion


          #region Update

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               // Draw 3D stuff.
               // Draw 2D stuff.
          }

          #endregion


          #region Draw Overlay

          /// <summary>
          /// Renders the Overlay / HUD used by the Level.
          /// 
          /// Deriving Level objects should define their own DrawOverlay
          /// and put all HUD-related rendering inside of it.
          /// </summary>
          /// <param name="gameTime"></param>
          protected virtual void DrawOverlay(GameTime gameTime)
          {
          }

          #endregion


          #region Helper Draw Methods

          static int vWidth = ScreenManager.GraphicsDevice.Viewport.Width;
          static int vHeight = ScreenManager.GraphicsDevice.Viewport.Height;
          //Rectangle healthAndComboBgPosition = new Rectangle((int)(vWidth * 0.10f), (int)(vHeight * 0.10f), 205, 62);

          protected Rectangle heartIconPosition = new Rectangle((int)(vWidth * 0.10f) + (int)(205 * 0.25f), (int)(vHeight * 0.10f) + 31, 42, 42);
          protected Vector2 heartValuePosition = new Vector2((int)(vWidth * 0.10f) + (int)(205 * 0.66f), (vHeight * 0.10f) + 31);
          protected Vector2 heartIconPosition2 = new Vector2(125 + 42 / 2 - 50, 100 - 42 / 2);

          protected List<float> counter = new List<float>();
          protected List<float> rotations = new List<float>();

          protected int lastHP = 5;//CurrentPlayer.Health;

          #region Draw Heart / Health, Combo & Score

          /// <summary>
          /// Renders the Heart texture representing Health.
          /// </summary>
          /// <param name="gameTime"></param>
          protected virtual void DrawHeart(GameTime gameTime)
          {
               //MySpriteBatch.Draw(heartIconTexture.Texture2D, heartIconPosition, Color.Red);

               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float pulsate = 1.0f;

               if (CurrentPlayer.Health <= 2)
               {
                    if (CurrentPlayer.Health == 2)
                    {
                         pulsate = System.Math.Abs((float)System.Math.Sin(time * 3)) + 0.5f;// time * 6,   + 1; 
                         pulsate = MathHelper.Clamp(pulsate, 0.5f, 1.5f);// *0.75f;
                    }

                    else
                    {
                         pulsate = System.Math.Abs((float)System.Math.Sin(time * 6)) + 0.5f;// time * 6,   + 1; 
                         pulsate = MathHelper.Clamp(pulsate, 0.5f, 1.5f);
                    }
               }

               float fuckthisX = pulsate * 0.05f * (heartIconTexture.Texture2D.Width / 2);//42 / 2);
               float fuckthisY = pulsate * 0.05f * (heartIconTexture.Texture2D.Height / 2);

               Vector2 heartOrigin = new Vector2(fuckthisX, fuckthisY);

               heartIconPosition2 = new Vector2((vWidth * 0.10f) + (int)(205.0f * 0.1f), (vHeight * 0.10f) + (int)(62 * 0.15f));

               MySpriteBatch.Draw(heartIconTexture.Texture2D, heartIconPosition2, null, Color.Red, 0, heartOrigin, 0.05f * pulsate);

               if (lastHP > CurrentPlayer.Health)// && !animateHeart)
               {
                    AudioManager.PlayCue("Heart Pop");
                    counter.Add(0f);
                    rotations.Add(0f);
               }

               for (int i = 0; i < counter.Count; i++)
               {
                    counter[i] += 7.5f;
                    rotations[i] += 4.5f;

                    Rectangle heartLeftIconPosition = new Rectangle((int)(vWidth * 0.10f), (int)(vHeight * 0.10f) + (int)(62 * 0.15f) + (int)counter[i], 100, 100);
                    Vector2 heartLeftIconPos = new Vector2((vWidth * 0.10f), (vHeight * 0.10f) + (int)(62 * 0.15f) + (int)counter[i]); //new Vector2((vWidth * 0.10f), 100 - 42 / 2 + (int)counter[i]);

                    MySpriteBatch.Draw(heartLeftIconTexture.Texture2D, heartLeftIconPos, null, Color.Red, MathHelper.ToRadians(rotations[i]),
                         new Vector2(0.05f * heartLeftIconTexture.Texture2D.Width / 2, 0.05f * heartLeftIconTexture.Texture2D.Height / 2), 0.05f);

                    MySpriteBatch.Draw(heartRightIconTexture.Texture2D, heartLeftIconPos, null, Color.Red, MathHelper.ToRadians(-rotations[i]),
                         new Vector2(heartLeftIconPosition.X, heartLeftIconPosition.Y), 0.05f);

                    if (counter[i] > 750)
                    {
                         counter.Remove(counter[i]);
                         rotations.Remove(rotations[i]);

                         break;
                    }
               }

               lastHP = CurrentPlayer.Health;
          }

          /// <summary>
          /// Renders the player's Health text.
          /// </summary>
          /// <param name="gameTime"></param>
          protected virtual void DrawHealth(GameTime gameTime)
          {
               string s = string.Format("{0}", CurrentPlayer.Health.ToString());
               TextManager.DrawCentered(false, hudFont.SpriteFont, s, heartValuePosition, Color.White, 0.75f);
          }

          /// <summary>
          /// Draws the player's Combo Meter.
          /// </summary>
          /// <param name="gameTime"></param>
          protected void DrawCombo(GameTime gameTime)
          {
               int startingX = 180;
               int startingY = 75;
               int width = 140;
               int height = 20;

               MySpriteBatch.Draw(blankTexture.Texture2D,
                    new Rectangle(startingX, startingY, width, height), Color.CornflowerBlue * (50f / 255f));

               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX, startingY,
                         (int)((50f / 100f) * width), height),//((CurrentPlayer.ComboMeter / 100) * width), height),
                         Color.CornflowerBlue);

               MySpriteBatch.Draw(borderTexture.Texture2D,
                    new Rectangle(startingX, startingY, width, height), Color.Black);
          }

          /// <summary>
          /// Renders the player's current Score.
          /// </summary>
          /// <param name="gameTime"></param>
          private void DrawScore(GameTime gameTime)
          {
               string s = string.Format("{0}", TotalScore.ToString());

               // Top Center.
               TextManager.DrawCentered(false, hudFont.SpriteFont, "Score", new Vector2(1280 / 2, 100f), Color.Gold, 0.4f);
               TextManager.DrawCentered(false, hudFont.SpriteFont, s, new Vector2(1280 / 2, 130), Color.White, 0.75f);
          }

          #endregion

          #region Draw Perfect & Speedy Kill Streaks

          /// <summary>
          /// Renders the "Perfect x" text.
          /// </summary>
          /// <param name="gameTime"></param>
          private void DrawPerfectStreak(GameTime gameTime)
          {
               string s = string.Format("Perfect x {0}", AvatarZombieGame.AwardData.AccuracyStreak.ToString());

               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(250f, 100f), Color.CornflowerBlue, 0.8f);
          }

          /// <summary>
          /// Renders the "Speedy x" text.
          /// </summary>
          /// <param name="gameTime"></param>
          private void DrawSpeedStreak(GameTime gameTime)
          {
               string s = string.Format("Speedy x {0}", AvatarZombieGame.AwardData.SpeedStreak.ToString());

               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(250f, 155f), Color.LightGreen, 0.8f);
          }

          #endregion

          #endregion


          #region Unloading Level Content

          public virtual void UnloadLevelContent()
          {
               this.UnloadContent();
          }

          protected override void UnloadContent()
          {
               if (waveManager != null)
               {
                    waveManager.CurrentWave.Dispose();
                    waveManager = null;
               }

               //SceneGraphManager.RemoveObjects();

               base.UnloadContent();
          }

          #endregion
     }
}