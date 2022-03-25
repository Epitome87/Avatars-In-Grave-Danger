#region File Description
//-----------------------------------------------------------------------------
// ReadingEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.Text
{
     /// <summary>
     /// A type of TextEffect which draws TextObjects one String character at a time,
     /// causing the String to appear as if it is being typed in via keyboard.
     /// </summary>
     public class ReadingEffect : TextEffect
     {
          #region Fields

          /// <summary>
          /// Holds the string this TextEffect will update and render.
          /// 
          /// This may be different than the text of the TextObject this TextEffect belongs to.
          /// In this case, this text will be a substring of the TextObject text, and it will
          /// change depending on how far into the update method we are.
          /// </summary>
          private string effectText;

          /// <summary>
          /// How long, in seconds, has elapsed.
          /// </summary>
          private float elapsedTime;

          /// <summary>
          /// How long, in seconds, are remaining for this
          /// ReadingEffect to complete all of its logic.
          /// </summary>
          private float remaining;

          #endregion


          #region Initialization


          /// <summary>
          /// ReadingEffect constructor.
          /// </summary>
          /// <param name="runTime">How long, in seconds, the ReadingEffect should take to complete.</param>
          /// <param name="message">The text the ReadingEffect should render.</param>
          public ReadingEffect(float runTime, String message)
               : base(runTime)
          {
               this.effectText = message;
               elapsedTime = 0.0f;
               remaining = runTime;
          }

          #endregion


          #region Update

          /// <summary>
          /// Overriden Update method.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public override void Update(GameTime gameTime, TextObject textObj)
          {
               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               Step = (RunTime == 0 ? 1 :
                      MathHelper.Clamp((float)((RunTime -
                          remaining) / RunTime), 0, 1));

               this.effectText = textObj.Text.Substring(0, (int)(Step * textObj.Text.Length));

               remaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
          }

          #endregion


          #region Draw

          /// <summary>
          /// Overriden Draw method.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public override void Draw(GameTime gameTime, TextObject textObj)
          {
               if (textObj.IsCenter)
               {
                    TextManager.DrawCentered(false, textObj.Font, this.effectText, textObj.Position, textObj.Color,
                         textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
               }

               else if (textObj.IsAutoCenter)
               {
                    TextManager.DrawCentered(true, textObj.Font, this.effectText, textObj.Position, textObj.Color,
                         textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
               }

               else
               {
                    TextManager.Draw(textObj.Font, this.effectText, textObj.Position, textObj.Color,
                                   textObj.Rotation, textObj.Origin, textObj.Scale);
               }
          }

          #endregion
     }
}