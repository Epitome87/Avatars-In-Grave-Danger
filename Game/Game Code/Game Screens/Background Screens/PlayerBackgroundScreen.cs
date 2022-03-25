#region File Description
//-----------------------------------------------------------------------------
// PlayerBackgroundScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A BackgroundScreen for the Start / Main-Menu in Avatar Typing.
     /// This is a BackgroundScreen to prevent re-loading assets and losing
     /// the state of the camera angles / enemies.
     /// </summary>
     public class PlayerBackgroundScreen : BackgroundScreen
     {
          #region Fields

          Texture2D gamerPic;// = AvatarTypingGame.CurrentPlayer.GamerInformation.GamerPicture;

          GameResourceTexture2D smallGradientTexture;
          GameResourceTexture2D blankTexture;
          GameResourceTexture2D borderTexture_Black;
          GameResourceTexture2D borderTexture_White;

          public static bool isActive = true;


          public Player player = AvatarZombieGame.CurrentPlayer; // Use to be Private

          public bool DrawWithoutCamera = false;
          public bool ShowAsGradient = true;
          public bool ShowBorder = true;
          public bool ShowGamerTag = true;
          public bool ShowGamerPic = true;
          

          public Vector3 playerPosition = new Vector3(0.95f, 0.95f, 0.0f);
          public Rectangle border = new Rectangle(200, 200, 350, 450);

          public Color BorderColor = Color.Yellow * (100f / 255f);
          public Color OutlineColor = Color.Black;

          public float playerScale = 3.76f;//0.9f;
          public float borderScale = 1.0f;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public PlayerBackgroundScreen()
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);
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
               smallGradientTexture = ResourceManager.LoadTexture(@"Textures\Gradients\Gradient_BlackToWhite1");
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_WithBorder");
               borderTexture_White = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_White");
               borderTexture_Black = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border");


               borderScale = playerScale / 0.9f;

               border = new Rectangle(200, 200, (int)(350 * borderScale), (int)(450 * borderScale));


               gamerPic = blankTexture.Texture2D;
               gamerPic = this.player.GamerInformation.GamerPicture;
          }

          /// <summary>
          /// Unloads graphics content for this screen.
          /// </summary>
          public override void UnloadContent()
          {
               base.UnloadContent();
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
               if (!isActive)
                    return;

               //player.Update(gameTime);

               player.Avatar.Update(gameTime);

               base.Update(gameTime, otherScreenHasFocus, false);
          }

          #endregion


          Random random = new Random();


          private Vector3 ambientLightColor = Color.White.ToVector3();
          private Vector3 lightDirection = new Vector3(-1.25f, -0.25f, -1.0f);
          private Vector3 lightColor = Color.CornflowerBlue.ToVector3();

          #region Draw

          /// <summary>
          /// Draws the background screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               if (!isActive)
                    return;

               player.Avatar.Position = playerPosition;
               player.Avatar.Rotation = Vector3.Zero;
               player.Avatar.Scale = playerScale;
               player.Avatar.LightingEnabled = true;// false;

               MySpriteBatch.Begin();

               if (ShowBorder)
               {
                    if (ShowAsGradient)
                    {
                         Rectangle newBorder;
                         newBorder = new Rectangle(border.X + 2, border.Y + 2, 
                              border.Width - 4, border.Height - 4);

                         MySpriteBatch.Draw(smallGradientTexture.Texture2D, newBorder, BorderColor);
                    }

                    else
                         MySpriteBatch.Draw(blankTexture.Texture2D, border, BorderColor);

                    if (OutlineColor == Color.White)
                    {
                         MySpriteBatch.Draw(borderTexture_White.Texture2D, border, OutlineColor);
                    }

                    else
                    {
                         MySpriteBatch.Draw(borderTexture_Black.Texture2D, border, OutlineColor);
                    }
               }

               MySpriteBatch.End();








               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    AvatarZombieGame.CurrentPlayer.Avatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                    AvatarZombieGame.CurrentPlayer.Avatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                    AvatarZombieGame.CurrentPlayer.Avatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
               }

               else
               {
                    AvatarZombieGame.CurrentPlayer.Avatar.AmbientLightColor = ambientLightColor;
                    AvatarZombieGame.CurrentPlayer.Avatar.LightDirection = lightDirection;
                    AvatarZombieGame.CurrentPlayer.Avatar.LightColor = lightColor;
               }




               if (DrawWithoutCamera)
               {
                    if (AvatarZombieGame.CurrentPlayer != null)
                    {
                         AvatarZombieGame.CurrentPlayer.Avatar.DrawToScreen(gameTime,
                              new Vector3(0f, 1f, -5f), new Vector3(0, 0.20f, 0f));
                    }
               }

               else
               {
                    if (AvatarZombieGame.CurrentPlayer != null)
                    {
                         AvatarZombieGame.CurrentPlayer.Draw(gameTime);
                    }
               }

               MySpriteBatch.Begin();

               if (ShowGamerTag)
               {
                    TextManager.DrawCentered(false, ScreenManager.Font, player.GamerInformation.GamerTag,
                         new Vector2(border.X + (border.Width / 2), border.Y + border.Height - (2 * gamerPic.Height)), Color.Gold); //border.Y + 330
               }

               if (ShowGamerPic)
               {
                   // MySpriteBatch.Draw(gamerPic, new Vector2(border.X + 135, border.Y + 370), Color.White);
                    MySpriteBatch.DrawCentered(gamerPic, new Vector2(border.X + (border.Width / 2), border.Y + border.Height - gamerPic.Height), Color.White);
               }

               MySpriteBatch.End();
          }

          #endregion
     }
}
