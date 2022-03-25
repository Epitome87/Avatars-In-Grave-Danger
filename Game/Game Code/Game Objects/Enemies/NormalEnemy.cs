#region File Description
//-----------------------------------------------------------------------------
// NormalTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine;
using PixelEngine.Avatars;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Normal Typing Enemy is one with average Health size and average speed.
     /// </summary>
     public class NormalEnemy : Enemy
     {
          #region Fields


          #endregion

          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public NormalEnemy(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               this.Speed = (1.5f / 4.0f) * (1f + ((int)(ZombieGameSettings.Difficulty) * 0.5f));

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

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.Health = 1;

               // Every 10 waves, increase their Health by 1.
               this.Health += ((this.Wave.WaveNumber) / 10);

               this.startingHealth = this.Health;

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
     }
}