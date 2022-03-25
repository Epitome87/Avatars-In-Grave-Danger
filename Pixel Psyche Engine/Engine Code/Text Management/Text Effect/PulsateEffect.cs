#region File Description
//-----------------------------------------------------------------------------
// PulsateEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Text
{
     /// <summary>
     /// A type of TextEffect which draws TextObjects with a pulsating effect.
     /// That is, the text pulsates (using a Sine function) with a given Intensity (scale)
     /// and Period (how many times per second a complete cycle is made).
     /// </summary>
     public class PulsateEffect : TextEffect
     {
          #region Fields

          private float _elapsedTime;
          private float _remaining;
          private float _intensity;
          private float _pulsatePeriod;

          private bool _runInfinitely = false;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public PulsateEffect(float runTime, String message)
               : base(runTime)
          {
               _elapsedTime = 0.0f;
               _remaining = runTime;
               _intensity = 1.0f;
               _pulsatePeriod = 1.0f;

               if (runTime <= 0)
                    _runInfinitely = true;
          }

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public PulsateEffect(float runTime, float intense, float period, String message)
               : base(runTime)
          {
               _elapsedTime = 0.0f;
               _remaining = runTime;
               _intensity = intense;
               _pulsatePeriod = period;

               if (runTime <= 0)
                    _runInfinitely = true;
          }

          #endregion

          #region Update

          /// <summary>
          /// Overriden Update method.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public override void Update(GameTime gameTime, TextObject textObj)
          {
               _elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               Step = (RunTime == 0 ? 1 :
                      MathHelper.Clamp((float)((RunTime - _remaining) / RunTime), 0, 1));

               if (!_runInfinitely)
               {
                    if (Step >= 1)
                         return;
               }

               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float pulsate = (float)Math.Sin( (time * 6) * _pulsatePeriod ) + 1;

               float scale = 1 + pulsate * (_intensity * 0.05f);
               textObj.Scale = scale;

               _remaining -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion

          #region Draw

          /// <summary>
          /// Overriden Draw method.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public override void Draw(GameTime gameTime, TextObject textObj)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(textObj.Font, textObj.Text, textObj.Position, textObj.Color,
                              textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
          }

          #endregion
     }
}