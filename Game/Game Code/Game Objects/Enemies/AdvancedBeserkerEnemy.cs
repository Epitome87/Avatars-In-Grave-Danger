#region File Description
//-----------------------------------------------------------------------------
// BeserkerEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Audio;
using PixelEngine.Avatars;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Beserker Enemy is one which starts out at a leisurely pace,
     /// stomps, then begins charging at a fast speed.
     /// </summary>
     public class AdvancedBeserkerEnemy : Enemy
     {
          #region Fields

          /// <summary>
          /// The Beserker's "State" can be:
          /// - Strutting: Simply running leisurely.
          /// - Charging: Going Beserk!
          /// </summary>
          private enum BeserkerState
          {
               Strutting,
               Charging
          }
          private BeserkerState CurrentBeserkerState = BeserkerState.Strutting;

          /// <summary>
          /// How much time remains (seconds) until the Beserker...goes Beserk!
          /// </summary>
          private float timeUntilCharge = 3f;

          #endregion


          #region Initialization

          /// <summary>
          /// FastTypingEnemy Constructor.
          /// Creates a new Fast Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public AdvancedBeserkerEnemy(Vector3 position, Wave wave)
               : base(PixelEngine.EngineCore.Game, wave)
          {
               this.Speed = (3.0f / 4.0f) * (1f + ((int)(ZombieGameSettings.Difficulty) * 0.5f));

               if (ZombieGameSettings.Difficulty == Difficulty.Hard)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               // But we reset the speed every 10 waves, and add + 25% to the base speed.
               this.Speed *= this.Wave.WaveSpeedFactor;

               this.Position = position;
               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedKillTime = 0.0f;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.Health = 2;

               // Every 10 waves, increase their Health by 1.
               this.Health += ((this.Wave.WaveNumber) / 10);

               if (ZombieGameSettings.Difficulty == Difficulty.Hard)
               {
                    this.Health += 1;
               }

               this.startingHealth = this.Health;

               this.Initialize();
          }

          /// <summary>
          /// Overridden LoadContent method.
          /// 
          /// Cals upon base.LoadContent, but also loads this Enemy's font.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               Avatar.PlayAnimation(AnimationType.Run, true);

               random = new Random();

               if (ZombieGameSettings.Difficulty == Difficulty.Normal)
               {
                    timeUntilCharge = 2.0f + ((float)random.NextDouble() * 3);
               }

               // Give the player an extra second to react on Hard.
               else
               {
                    timeUntilCharge = 3.0f + ((float)random.NextDouble() * 3);
               }
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime)
          {
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    Avatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                    Avatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                    Avatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
               }

               // If this Enemy is Dying...
               if (this.IsDying)
               {
                    this.Color = Color.White;

                    // Increment how long he's been in his Dying state!
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // And update his Avatar!
                    Avatar.Update(gameTime);

                    // Return because we don't want to update his position or increase his ElapsedTime.
                    return;
               }


               Vector3 targetPosition = this.Wave.CurrentPlayer.Position;
               Vector3 direction = targetPosition - this.Position;

               Position = new Vector3(
                    Position.X + (direction.X * this.Speed / 400f),
                    Position.Y + (direction.Y * (float)gameTime.ElapsedGameTime.TotalSeconds),
                    Position.Z - this.Speed / 25f);
                    //Position.Z + (direction.Z * (float)gameTime.ElapsedGameTime.TotalSeconds));

               // Update the Enemy's Position (and thus Avatar Position).
               //this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;
               this.WorldPosition = this.Avatar.WorldMatrix;


               // Increment how long we've been fighting this Enemy!
               this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


               // New: In testing: For speed-based animation playback.
               TimeSpan elapsedAnimationTime = gameTime.ElapsedGameTime;

               if (CurrentBeserkerState == BeserkerState.Charging)
                    elapsedAnimationTime = TimeSpan.FromTicks((long)(elapsedAnimationTime.Ticks * (Math.Abs(this.Speed / 2f))));         // / 1 is good for everything except ZombieWalk

               else
               {
                    elapsedAnimationTime = TimeSpan.FromTicks((long)(elapsedAnimationTime.Ticks * (Math.Abs(this.Speed))));
               }

               GameTime tweakedAnimationPlayingSpeed = new GameTime(gameTime.TotalGameTime, elapsedAnimationTime);

               Avatar.Update(tweakedAnimationPlayingSpeed);

               timeUntilCharge -= (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (CurrentBeserkerState == BeserkerState.Strutting && timeUntilCharge <= 0)
               {
                    Charge();

                    CurrentBeserkerState = BeserkerState.Charging;
               }
          }

          #endregion


          #region Charge!!!

          public void Charge()
          {
               // Make the appropriate Charging sound based on gender.
               if (this.Avatar.AvatarDescription.BodyType == AvatarBodyType.Male)
               {
                    AudioManager.PlayCue("Zombie_Charge1_Male");
               }

               else
               {
                    AudioManager.PlayCue("Zombie_Charge1_Female");
               }

               // We do this because increasing the Speed by 2.5 on Hard is just...too hard!
               if (ZombieGameSettings.Difficulty == Difficulty.Normal)
               {
                    Speed *= 2.5f;
               }

               else
               {
                    Speed *= 2.0f;
               }
          }

          #endregion


          #region Overridden On Killed

          public override void OnKilled()
          {
               // If he died while still in his un-charging state...
               if (CurrentBeserkerState == BeserkerState.Strutting)
               {
                    // The Player gets an Award!!!
                    AvatarZombieGame.AwardData.StoppedBeserkerFromCharging = true;
               }

               base.OnKilled();
          }

          #endregion
     }
}