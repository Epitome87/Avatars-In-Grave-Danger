#region File Description
//-----------------------------------------------------------------------------
// InflatingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine;
using PixelEngine.Avatars;
using System;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Enemy is one which increases in size and speed
     /// the more he is angered!
     /// </summary>
     public class InflatingEnemy : Enemy
     {
          #region Fields


          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public InflatingEnemy(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               this.Speed = (0.5f / 4.0f) * (int)(ZombieGameSettings.Difficulty + 1) * (0.50f);

               if (ZombieGameSettings.Difficulty == Difficulty.Hard)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               // But we reset the speed every 10 waves, and add + 25% to the base speed.
               this.Speed *= this.Wave.WaveSpeedFactor;

               this.Position = position;
               this.BasePoints = 50;
               this.BonusPoints = 50;
               this.ElapsedKillTime = 0.0f;

               this.Health = 6;
               this.startingHealth = this.Health;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.DamageDoneToPlayer = 1;

               this.Avatar.Scale *= 1.5f;

               this.Initialize();
          }

          /// <summary>
          /// Loads the appropriate content for the Normal Typing Enemy.
          /// In this case, we want a unique Sprite and Font style for the enemy.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               Avatar.PlayAnimation(AnimationType.ZombieWalk, true);
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the Normal Typing Enemy.
          /// 
          /// Simply calls upon base.Update.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               base.Update(gameTime);
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Normal Typing Enemy.
          /// 
          /// Simply calls base.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion


          #region Overridden On Hit

          public override void OnHit(Player playerWhoHit)
          {
               base.OnHit(playerWhoHit);

               // Get faster.
               this.Speed *= 1.5f;

               // Grow larger.
               this.Avatar.Scale *= 1.1f;

               MathHelper.Clamp(this.Avatar.Scale, 1.0f, 4.0f);
          }

          #endregion
     }
}