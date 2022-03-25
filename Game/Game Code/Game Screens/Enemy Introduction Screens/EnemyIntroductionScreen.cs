#region File Description
//-----------------------------------------------------------------------------
// EnemyIntroductionScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     public class EnemyIntroductionScreen : PagedMenuScreen
     {
          #region Fields

          private ContentManager content;
          private Texture2D backgroundTexture;

          private Enemy enemy;
          private EnemyType enemyType;
          private string introText = String.Empty;

          TypingEffect txtEffect = new TypingEffect("", 25.0f);

          TextObject generalInfoText = new TextObject("", 
               new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 25),
                    FontType.MenuFont, Color.Gold, 0.0f, Vector2.Zero, 0.75f, true, new TypingEffect("", 15f));

          #endregion

          #region Properties

          public Enemy Enemy
          {
               get { return enemy; }
               set { enemy = value; }
          }

          public string IntroText
          {
               get { return introText; }
               set { introText = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public EnemyIntroductionScreen(EnemyType enemyToIntro, string enemyIntroText)
               : base(enemyIntroText, 3, 1)
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);

               enemyType = enemyToIntro;
               IntroText = enemyIntroText;

               for (int i = 0; i < this.NumberOfPages; i ++)
               {
                    MenuEntries[i].Text = "Exit Introduction";
                    MenuEntries[i].Selected += SkipSelected;
                    MenuEntries[i].IsCenter = true;
                    MenuEntries[i].Position = new Vector2(MenuEntries[i].Position.X, MenuEntries[i].DescriptionPosition.Y - 140);
                    MenuEntries[i].ShowBorder = false;
                    MenuEntries[i].SelectedColor = Color.White;
               }

               Wave manager = new Wave(null, 1, 1, null);

               switch (enemyType)
               {
                    case EnemyType.Normal:
                         enemy = new NormalEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Slinger:
                         enemy = new SlingerEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Beserker:
                         enemy = new BeserkerEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Warper:
                         enemy = new WarperEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Horde:
                         enemy = new InflatingEnemy(Vector3.Zero, manager);
                         break;
               }

               if (enemyType == EnemyType.Boss)
                    enemy.Avatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, true);

               txtEffect.IsSoundEnabled = false;

               generalInfoText.IsCenter = true;
               generalInfoText.Text = GrabText();

               txtEffect.Text = generalInfoText.Text;
               generalInfoText.TextEffect = txtEffect;
          }

          /// <summary>
          /// Loads graphics content for this screen. The background texture is quite
          /// big, so we use our own local ContentManager to load it. This allows us
          /// to unload before going from the menus into the game itself, wheras if we
          /// used the shared ContentManager provided by the Game class, the content
          /// would remain loaded forever.
          /// </summary>
          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               backgroundTexture = content.Load<Texture2D>(@"Textures\Gradients\Gradient_Star");
          }


          /// <summary>
          /// Unloads graphics content for this screen.
          /// </summary>
          public override void UnloadContent()
          {
               content.Unload();
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               base.OnCancel(playerIndex);
          }


          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Difficulty menu entry is selected.
          /// </summary>
          void NextPageSelected(object sender, PlayerIndexEventArgs e)
          {
               base.OnNextPage();  
          }

          void SkipSelected(object sender, PlayerIndexEventArgs e)
          {
               base.OnCancel(e.PlayerIndex);
          }

          protected override void OnNextPage()
          {
               base.OnNextPage();

               generalInfoText.Text = GrabText();

               txtEffect = new TypingEffect(generalInfoText.Text, 25.0f);
               txtEffect.IsSoundEnabled = true;

               generalInfoText.TextEffect = txtEffect;
               //generalInfoText.AddTextEffect(txtEffect);
          }

          protected override void OnPreviousPage()
          {
               base.OnPreviousPage();

               generalInfoText.Text = GrabText();

               txtEffect = new TypingEffect(generalInfoText.Text, 25.0f);
               txtEffect.IsSoundEnabled = true;

               generalInfoText.TextEffect = txtEffect;
               //generalInfoText.AddTextEffect(txtEffect);
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the background screen. Unlike most screens, this should not
          /// transition off even if it has been covered by another screen: it is
          /// supposed to be covered, after all! This overload forces the
          /// coveredByOtherScreen parameter to false in order to stop the base
          /// Update method wanting to transition off.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, false);

               Enemy.Avatar.Update(gameTime);

               generalInfoText.Update(gameTime);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the background screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
               Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
               byte fade = TransitionAlpha;

               spriteBatch.GraphicsDevice.Clear(Color.Black);

               spriteBatch.Begin();

               Color color = Color.CornflowerBlue;
               if (Enemy != null)
               {
                    color = Enemy.Color;

                    if (color == Color.Black)
                    {
                         color = Color.Gray;
                    }
               }
 
               spriteBatch.Draw(backgroundTexture, fullscreen, color * (fade / 255f));

               spriteBatch.End();

               DrawEnemy(gameTime);

               spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

               DrawIntroduction(gameTime);

               spriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion

          #region Helper Draw Methods

          /// <summary>
          /// Draws the Enemy to be "Introduced".
          /// </summary>
          private void DrawEnemy(GameTime gameTime)
          {
               Enemy.Avatar.Position = new Vector3(0f, 0f, 6f);

               Enemy.DrawWithoutCamera(gameTime, ScreenManager.Font, new Vector3(0f, 1.0f, -5f), new Vector3(0f, 0f, 0f));
          }

          /// <summary>
          /// Draws a String representation of the Enemy's Introduction text.
          /// </summary>
          private void DrawIntroduction(GameTime gameTime)
          {
               generalInfoText.Draw(gameTime);
          }

          private string GrabText()
          {
               switch (enemyType)
               {
                    case EnemyType.Normal:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "Health: " + Enemy.Health.ToString() + "\nSpeed: " + Enemy.Speed.ToString("0.00");
                                   break;
                              case 2:
                                   introText = "Fodder Zombies have no special characteristics.";
                                   break;
                              case 3:
                                   introText = "Base Score: 50 Points\nSpeed Kill: + 50 Points\nPerfect Kill: + 50 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Slinger:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "Health: " + Enemy.Health.ToString() + "\nSpeed: " + Enemy.Speed.ToString("0.00");
                                   break;
                              case 2:
                                   introText = "This Zombie will throw projectiles at you.";
                                   break;
                              case 3:
                                   introText = "Base Score: 50 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Beserker:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "Health: " + Enemy.Health.ToString() + "\nSpeed: " + Enemy.Speed.ToString("0.00");
                                   break;
                              case 2:
                                   introText = "Speedster Enemies are 50% faster than Normal Enemies.";
                                   break;
                              case 3:
                                   introText = "Base Score: 100 Points\nSpeed Kill: + 100 Points\nPerfect Kill: + 100 Points";
                                   break;
                         }
                         break;


                    case EnemyType.Horde:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "Health: " + Enemy.Health.ToString() + "\nSpeed: " + Enemy.Speed.ToString("0.00");
                                   break;
                              case 2:
                                   introText = "Horde Enemies travel in large groups together. Their sentences contain\n" +
                                               "only one character, but they are tremendously fast.\n" +
                                               "Is the Horde free points, or certain death?";
                                   break;
                              case 3:
                                   introText = "Base Score: 250 Points\nSpeed Kill: + 250 Points\nPerfect Kill: + 250 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Boss:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "Health: " + Enemy.Health.ToString() + "\nSpeed: " + Enemy.Speed.ToString("0.00");
                                   break;
                              case 2:
                                   introText = "Boss Enemies wield many sentences - not just one!\n" +
                                               "Every so often they will throw projectiles toward the player.\n" +
                                               "Typing the projectile's sentence will destroy it.";
                                   break;
                              case 3:
                                   introText = "Base Score: 10,000 Points\nSpeed Kill: + 500 Points per sentence\nPerfect Kill: + 500 Points per sentence";
                                   break;
                         }
                         break;
               }

               return introText;
          }

          #endregion
     }
}