using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PixelEngine.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Screen;
using PixelEngine.Graphics;
using PixelEngine;

namespace AvatarsInGraveDanger
{
     class MessageBoxWithKeyboardScreen : MessageBoxScreen
     {
          #region Initialization

          TextObject headerTextObject;

          /// <summary>
          /// Constructor automatically includes the standard "A=ok, B=cancel"
          /// usage text prompt.
          /// </summary>
          public MessageBoxWithKeyboardScreen(string message)
               : this(message, "Accept", "Cancel")
          {
               headerTextObject = new TextObject(message, EngineCore.ScreenCenter);
               headerTextObject.FontType = FontType.TitleFont;
               headerTextObject.Scale *= 0.35f;
               headerTextObject.AddTextEffect(new ReadingEffect(1.0f, message));// true, "KeyPress"));
               headerTextObject.IsCenter = true;
          }

          /// <summary>
          /// Constructor lets the caller specify whether to include the standard
          /// "A=ok, B=cancel" usage text prompt.
          /// </summary>
          public MessageBoxWithKeyboardScreen(string message, string accept, string cancel) 
               : base(message, accept, cancel)
          {
               Accept = accept;
               Cancel = cancel;

               this.message = message;

               IsPopup = true;

               TransitionOnTime = TimeSpan.FromSeconds(1.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the message box.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               SpriteFont font = TextManager.Fonts[(int)FontType.HudFont].SpriteFont;// ScreenManager.Font;

               // Darken down any other screens that were drawn beneath the popup.
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);

               // Center the message text in the viewport.
               Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
               Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
               Vector2 textSize = font.MeasureString(message);
               Vector2 textPosition = (viewportSize - textSize) / 2;

               // The background includes a border somewhat larger than the text itself.
               const int hPad = 32;
               const int vPad = 16;

               Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                             (int)textPosition.Y - vPad,
                                                             (int)textSize.X + hPad * 2,
                                                             (int)textSize.Y + vPad * 2);

               backgroundRectangle.Height *= 2;

               // Fade the popup alpha during transitions.
               Color color = Color.White * (TransitionAlpha / 255f);
    
               headerTextObject.Color = color;

               spriteBatch.Begin();

               // Draw the background rectangle.
               GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, backgroundRectangle, 2, Color.White * (TransitionAlpha / 255f)); 
               MySpriteBatch.Draw(gradientTexture.Texture2D, backgroundRectangle, Color.CornflowerBlue * (TransitionAlpha / 255f) * (100f / 255f));
 
               headerTextObject.Update(gameTime);
               headerTextObject.Draw(gameTime);

               textPosition = EngineCore.ScreenCenter;
               textPosition.Y += font.LineSpacing;
               
               //TextManager.DrawCentered(false, font, "[Enter] or                          ", textPosition, color);

               MySpriteBatch.Draw(acceptIcon.Texture2D, 
                    new Rectangle((int)EngineCore.ScreenCenter.X - acceptIcon.Texture2D.Width / 1, 
                         (int)textPosition.Y - 3 - font.LineSpacing / 2, 
                         font.LineSpacing, font.LineSpacing),
                  null, color);

               TextManager.DrawCentered(false, font, "       " + Accept, textPosition, color);
               //TextManager.DrawCentered(false, font, "               " + Accept, textPosition, color);


               textPosition.Y += font.LineSpacing;
               
               //TextManager.DrawCentered(false, font, "[Esc] or                      ", textPosition, color);
               
               MySpriteBatch.Draw(cancelIcon.Texture2D, 
                    new Rectangle((int)EngineCore.ScreenCenter.X - cancelIcon.Texture2D.Width / 1, 
                         (int)textPosition.Y - 3 - font.LineSpacing / 2, 
                         font.LineSpacing, font.LineSpacing), 
                         null, color);

               TextManager.DrawCentered(false, font, "       " + Cancel, textPosition, color);
               //TextManager.DrawCentered(false, font, "               " + Cancel, textPosition, color);
               
               spriteBatch.End();
          }


          #endregion
     }
}