#region File Description
//-----------------------------------------------------------------------------
// Projectile.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace AvatarsInGraveDanger
{
     public class Projectile
     {
          #region Fields

          public float Speed;
          public Vector3 Position;
          public Vector3 TrueVelocity = new Vector3();

          #endregion


          #region Initialization

          public Projectile()
          {
               Speed = 2.0f;
               Position = new Vector3(0, 0, 0);
          }

          #endregion


          #region Updating

          public void Update(GameTime gameTime)
          {
               Position = new Vector3(
                    Position.X,
                    Position.Y - ((0.007f) * (Speed / 2.5f) * Player.SlowMotionFactor),
                    Position.Z - ((Speed / 25f) * Player.SlowMotionFactor));
   
               Matrix mat = Matrix.CreateTranslation(Position);
          }

          public void AdvancedUpdate(GameTime gameTime)
          { 
               Position = new Vector3(
                    Position.X + (TrueVelocity.X * (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor)),
                    Position.Y + (TrueVelocity.Y * (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor)),
                    Position.Z + (TrueVelocity.Z * (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor)));

               Matrix mat = Matrix.CreateTranslation(Position);
          }

          #endregion
     }
}