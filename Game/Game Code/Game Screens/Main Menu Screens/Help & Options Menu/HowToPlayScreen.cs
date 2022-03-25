#region File Description
//-----------------------------------------------------------------------------
// HowToPlayScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Menu screen that displays a "How To Play" description.
     /// </remarks>
     public class HowToPlayScreen : PagedMenuScreen
     {
          #region Fields

          GameResourceTexture2D hudTexture;
          GameResourceTexture2D grenadeTexture;
          GameResourceTexture2D reloadTexture;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public HowToPlayScreen()
               : base("H O W  T O  P L A Y", 11)
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);
          }

          public override void LoadContent()
          {
               base.LoadContent();

               hudTexture = ResourceManager.LoadTexture(@"Tutorial\Tutorial_HUD");
               grenadeTexture = ResourceManager.LoadTexture(@"Textures\HUD\GrenadeIcon");
               reloadTexture = ResourceManager.LoadTexture(@"Tutorial\Tutorial_Reloading");
          }

          #endregion


          #region Handle Input and Update


          protected override void OnCancel(PlayerIndex playerIndex)
          {
               base.OnCancel(playerIndex);
          }

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 1 / 5);

               base.Draw(gameTime);

               MySpriteBatch.Begin();

               Color color = Color.White * (TransitionAlpha / 255f);

               string s = "";
               string heading = "";

               FontType fontType = FontType.HudFont;

               float fontScale = 1f;

               if (EngineCore.GraphicsDevice.Viewport.Width == 1920)
               {
                    fontScale = 1.2f;
               }

               int lineWidth = (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Width * (1f / fontScale));

               switch (this.CurrentPageNumber)
               {
                    case 1:
                         heading = "Objective";
                         s = TextManager.WrapText("Stop the Zombies!\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("Defeat the zombies before they escape from the graveyard.", fontType, lineWidth);

                         break;

                    case 2:
                         heading = "HUD";
                         MySpriteBatch.Draw(hudTexture.Texture2D, 
                              new Rectangle((int)(EngineCore.GraphicsDevice.Viewport.Width * 0.20f), (int)(EngineCore.GraphicsDevice.Viewport.Height * 0.22f),
                                   (int)(EngineCore.GraphicsDevice.Viewport.Width * 0.60f), (int)(EngineCore.GraphicsDevice.Viewport.Height * 0.60f)), Color.White);
                         break;

                    case 3:
                         heading = "Losing Health";
                         s = TextManager.WrapText("You will lose Health when:\n", fontType, lineWidth);
                         s += TextManager.WrapText("- A Zombie successfully gets past you\n", fontType, lineWidth);
                         s += TextManager.WrapText("- A Zombie directly attacks you\n\n", fontType, lineWidth);
                         s = TextManager.WrapText("When you lose all your Health, you become Infected.", fontType, lineWidth);
                         break;

                    case 4:
                         heading = "Your Arsenal - Zombie Purifier";
                         s = TextManager.WrapText("Use your Zombie Purifier to free the zombies.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Fire Purifier: Right Trigger\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Reload Purifier: Right Bumper\n", fontType, lineWidth);
                         break;

                    case 5:
                         fontScale = 0.75f;
                         if (EngineCore.GraphicsDevice.Viewport.Width == 1920)
                         {
                              fontScale = 1.0f;
                         }
                         lineWidth = (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Width * (1f / fontScale));

                         heading = "Your Arsenal - Zombie Purifier";
                         s = TextManager.WrapText("Quick Reload: Press RB to begin reloading. Wait until the moving slider is in the 'Clutch Area' then press RB again.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("- If timed correctly, you will perform a Quick Reload.\n", fontType, lineWidth);
                         s += TextManager.WrapText("- If not, you will finish reloading at normal speed.", fontType, lineWidth);
                         MySpriteBatch.DrawCentered(reloadTexture.Texture2D, new Rectangle((int)EngineCore.ScreenCenter.X, (int)(EngineCore.ScreenCenter.Y * 1.5f), 500, 100), Color.White);
                         break;

                    case 6:
                         heading = "Your Arsenal - Undead Cleanser";
                         s = TextManager.WrapText("Use your Undead Cleanser to free the zombies.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Fire Cleanser: Right Trigger\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Reload Cleanser: Right Bumper\n", fontType, lineWidth);
                         s += TextManager.WrapText("\nThe Cleanser holds more ammo than a Purifier, and has a greater rate of fire. However, it is also weaker.", fontType, lineWidth);
                         break;

                    case 7:
                         heading = "Your Arsenal - Zomblaster";
                         s = TextManager.WrapText("A Zomblaster defeats all zombies currently on the screen.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Toss Zomblaster: Left Bumper", fontType, lineWidth);
                         s += TextManager.WrapText("\n\nA Zomblaster sends out a cleansing gas that cures any zombie currently on-screen.", fontType, lineWidth);
                         break;

                    case 8:
                         heading = "Points & Cash";
                         s = TextManager.WrapText("Upon defeating a zombie, points are awarded to the player.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("Zombies will also drop money, which the player can use to purchase upgrades.", fontType, lineWidth);
                         break;

                    case 9:
                         heading = "Upgrades";
                         s = TextManager.WrapText("After each wave, you can visit Ned's shop to purchase Health, Grenades, & the following Upgrades.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Strafe Speed\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Reload Speed\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Bullet Speed\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Clip Size\n", fontType, lineWidth);
                         s += TextManager.WrapText("- Reload Clutch Area", fontType, lineWidth);
                         break;

                    case 10:
                         heading = "Progression";
                         s = TextManager.WrapText("Survive the wave of zombies to progress to the next wave.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("With each new wave, the zombies become faster and more aggressive, so watch out!", fontType, lineWidth);
                         break;

                    case 11:
                         heading = "Victory";
                         s = TextManager.WrapText("Complete all 20 waves to stop the zombie invasion.\n\n", fontType, lineWidth);
                         s += TextManager.WrapText("Play again to beat your old High Score, unlock new Awards, and even hidden features!", fontType, lineWidth);
                         break;
               }

               this.MenuTitle = heading;

               // Render the Sub-Text.
               color = Color.White * (TransitionAlpha / 255f);

               TextManager.DrawCentered(true, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, s, new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y + 25), color, fontScale);

               MySpriteBatch.End();
          }

          #endregion
     }
}
