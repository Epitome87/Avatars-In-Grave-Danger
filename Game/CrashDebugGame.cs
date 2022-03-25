
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Screen;
public class CrashDebugGame : Game
{
     private SpriteBatch spriteBatch;
     private SpriteFont font;
     private readonly Exception exception;

     public CrashDebugGame(Exception exception)
     {
          this.exception = exception;
          new GraphicsDeviceManager(this);
          Content.RootDirectory = "Content";
     }

     protected override void LoadContent()
     {
          font = Content.Load<SpriteFont>(@"Fonts\BankGothicMd_Regular_28");
          spriteBatch = new SpriteBatch(GraphicsDevice);
     }

     protected override void Update(GameTime gameTime)
     {
          if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
               Exit();

          base.Update(gameTime);
     }

     protected override void Draw(GameTime gameTime)
     {
          GraphicsDevice.Clear(Color.Black);

          spriteBatch.Begin();

          spriteBatch.DrawString(
             font,
             "**** CRASH LOG ****",
             new Vector2(100f, 75f),
             Color.White, 0.0f, new Vector2(), 0.5f, SpriteEffects.None, 0f);

          spriteBatch.DrawString(
             font,
             "Press Back to Exit",
             new Vector2(100f, 100f),
             Color.Gold, 0.0f, new Vector2(), 0.5f, SpriteEffects.None, 0f);

          spriteBatch.DrawString(
             font,
             string.Format("Exception: {0}", exception.Message),
             new Vector2(100f, 120f),
             Color.White, 0.0f, new Vector2(), 0.5f, SpriteEffects.None, 0f);

          spriteBatch.DrawString(
             font, string.Format("Stack Trace:\n{0}", exception.StackTrace),
             new Vector2(100f, 140f),
             Color.White, 0.0f, new Vector2(), 0.5f, SpriteEffects.None, 0f);

          spriteBatch.End();

          base.Draw(gameTime);
     }
}