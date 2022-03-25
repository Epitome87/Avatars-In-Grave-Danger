using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PixelEngine.Screen;
using PixelEngine;
using PixelEngine.ResourceManagement;
using PixelEngine.Graphics;

namespace AvatarsInGraveDanger
{
     public class CustomizeableSafeAreaScreen : MenuScreen
     {
          Rectangle safeAreaRectangle;
          GameResourceTexture2D blankTexture;

          public CustomizeableSafeAreaScreen() 
               : base("")
          {
               safeAreaRectangle = EngineCore.CustomizableSafeArea; //EngineCore.GraphicsDevice.Viewport.TitleSafeArea;
          }

          public override void LoadContent()
          {
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
               base.LoadContent();
          }

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               base.HandleInput(input, gameTime);

               PlayerIndex playerIndex;

  
               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.DPadRight, EngineCore.ControllingPlayer, out playerIndex))
               {
                    safeAreaRectangle = new Rectangle(safeAreaRectangle.X - 5, safeAreaRectangle.Y,
                         safeAreaRectangle.Width + 10, safeAreaRectangle.Height);
               }

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.DPadLeft, EngineCore.ControllingPlayer, out playerIndex))
               {
                    safeAreaRectangle = new Rectangle(safeAreaRectangle.X + 5, safeAreaRectangle.Y,
                         safeAreaRectangle.Width - 10, safeAreaRectangle.Height);
               }


               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.DPadUp, EngineCore.ControllingPlayer, out playerIndex))
               {
                    safeAreaRectangle = new Rectangle(safeAreaRectangle.X, safeAreaRectangle.Y - 5,
                         safeAreaRectangle.Width, safeAreaRectangle.Height + 10);
               }

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.DPadDown, EngineCore.ControllingPlayer, out playerIndex))
               {
                    safeAreaRectangle = new Rectangle(safeAreaRectangle.X, safeAreaRectangle.Y + 5,
                         safeAreaRectangle.Width, safeAreaRectangle.Height - 10);
               }

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.A, EngineCore.ControllingPlayer, out playerIndex))
               {
                    EngineCore.CustomizableSafeArea    //ZombieGameSettings.CustomTitleSafeArea 
                         = safeAreaRectangle;
               }

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.Y, EngineCore.ControllingPlayer, out playerIndex))
               {
                    safeAreaRectangle = EngineCore.GraphicsDevice.Viewport.TitleSafeArea;
               }
          }


          public override void Draw(GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               base.Draw(gameTime);

               MySpriteBatch.Begin();

               //MySpriteBatch.Draw(blankTexture.Texture2D, safeAreaRectangle, Color.White);

               // Left border.
               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(safeAreaRectangle.Left, safeAreaRectangle.Top, 10, safeAreaRectangle.Height), Color.White);

               // Right border.
               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(safeAreaRectangle.Right, safeAreaRectangle.Top, 10, safeAreaRectangle.Height), Color.White);

               // Top border.
               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(safeAreaRectangle.Left, safeAreaRectangle.Top, safeAreaRectangle.Width, 10), Color.White);

               // Bottom border.
               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(safeAreaRectangle.Left, safeAreaRectangle.Bottom, safeAreaRectangle.Width, 10), Color.White);

               MySpriteBatch.End();
          }
     }
}
