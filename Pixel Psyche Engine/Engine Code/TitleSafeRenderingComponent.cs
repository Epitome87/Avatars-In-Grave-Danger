using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelEngine
{
     public class TitleSafeRenderingComponent : DrawableGameComponent
     {
          #region Singleton

          /// <summary>
          /// The singleton for the Achievement Manager.
          /// </summary>
          private static TitleSafeRenderingComponent titleSafeRenderingComponent = null;


          public static TitleSafeRenderingComponent GetInstance()
          {
               return titleSafeRenderingComponent;
          }

          #endregion


          #region Fields

          RenderTarget2D MyRenderTarget;
          Rectangle MyTitleSafeRectangle;
          SpriteBatch MySpriteBatch;

          private float zoomFactor;

          #endregion


          /// <summary>
          /// Initialize the Achievement Manager DrawableGameComponent.
          /// </summary>
          /// <param name="game"></param>
          public static void Initialize(Game game)
          {
               titleSafeRenderingComponent = new TitleSafeRenderingComponent(1f, game);

               if (game != null)
               {
                    game.Components.Add(titleSafeRenderingComponent);
               }
          }



          private TitleSafeRenderingComponent(float zoom, Game game)
               : base(game)
          {
               DrawOrder = int.MaxValue;

               zoomFactor = zoom;

               zoomFactor = MathHelper.Clamp(zoomFactor, 0f, 1f);

               if (zoomFactor > 0)
               {
                    // init render target for full viewport size
                    MyRenderTarget = new RenderTarget2D(EngineCore.GraphicsDevice, 1920, 1080);

                    // init with size of smaller view area (according to value of zoom)
                    MyTitleSafeRectangle = 
                         new Rectangle(
                              (int)(zoomFactor * EngineCore.GraphicsDevice.Viewport.TitleSafeArea.X), 
                              (int)(zoomFactor * EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Y), 
                              (int)(1920 - (2f * zoomFactor * EngineCore.GraphicsDevice.Viewport.TitleSafeArea.X)), 
                              (int)(1080 - (2f * zoomFactor * EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Y)));

                    MySpriteBatch = new SpriteBatch(EngineCore.GraphicsDevice);
               }
          }

          /// <summary>
          /// Prepares for rendering.
          /// 
          /// This sets our render target, if necessary, to our TitleSafeRendering render target.
          /// </summary>
          public void PrepareForTitleSafeZoomDraw()
          {
               if (zoomFactor > 0)
                    Game.GraphicsDevice.SetRenderTarget(MyRenderTarget);
          }


          /// <summary>
          /// Draw.
          /// 
          /// Due to its high draw order, this will be drawn last.
          /// So, here we will set our render target back to null (back buffer),
          /// clear the screen to Black, and render our scene via SpriteBatch, passing
          /// in our custom render target for the texture.
          /// </summary>
          /// <param name="gameTime"></param>
          public override void Draw(GameTime gameTime)
          {
               // If we are going to be zooming...
               if (zoomFactor > 0)
               {
                    Game.GraphicsDevice.SetRenderTarget(null);
                    Game.GraphicsDevice.Clear(Color.Black);

                    MySpriteBatch.Begin();
                    MySpriteBatch.Draw(MyRenderTarget, MyTitleSafeRectangle, Color.White);
                    MySpriteBatch.End();
               }
          }
     }
}
