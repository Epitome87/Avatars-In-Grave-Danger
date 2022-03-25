#region File Description
//-----------------------------------------------------------------------------
// WarperEnemy.cs
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
     /// A Fast Typing Enemy is one which moves quickly, but has a short Health Bar.
     /// </summary>
     public class WarperEnemy : Enemy
     {
          #region Fields

          /// <summary>
          /// Represents which State a Warping Enemy can be in.
          /// - Walking: Enemy is walking normally.
          /// - Disappearing: Movement has halted, Enemy begins to stomp.
          /// - Invisible: Enemy turns invisible, and his Position is changed during this point.
          /// - Reappearing: Enemy reappears at new Position. Cycle repeats.
          /// </summary>
          private enum WarpingState
          {
               Walking,
               Disappearing,
               Invisible,
               Reappearing,
          }

          /// <summary>
          /// The current state of the Warping Enemy.
          /// We begin by Walking.
          /// </summary>
          private WarpingState CurrentWarpingState = WarpingState.Walking;

          /// <summary>
          /// How long, in seconds, until the Enemy Warps.
          /// </summary>
          private float timeUntilWarp = 3f;

          /// <summary>
          /// How long, in seconds, it takes the Enemy to Reappear.
          /// </summary>
          private float reappearTime = 1f;

          /// <summary>
          /// Whether or not the Warping Enemy has warped 
          /// at least one time already.
          /// 
          /// We check this value when the Enemy is Killed,
          /// so we can give the Player an Award if he Killed
          /// the Enemy before it ever Warped.
          /// </summary>
          private bool hasWarpedAtLeastOnce = false;

          #endregion


          #region Initialization

          /// <summary>
          /// FastTypingEnemy Constructor.
          /// Creates a new Fast Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public WarperEnemy(Vector3 position, Wave wave)
               : base(PixelEngine.EngineCore.Game, wave)
          {
               this.Speed = (1.75f / 4.0f) * (1f + ((int)(ZombieGameSettings.Difficulty) * 0.5f));

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
               this.ElapsedKillTime = 0f;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.Health = 2;

               // Every 10 waves, increase their Health by 1.
               this.Health += ((this.Wave.WaveNumber) / 10);

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

               Avatar.PlayAnimation(AnimationType.ZombieTwitchWalk, true);

               random = new Random();
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          // The close we are to warping, the darker color we are.
          Vector3 warpColor = new Vector3(-10f);

          public override void Update(GameTime gameTime)
          {
               if (CurrentWarpingState != WarpingState.Invisible)
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
                         elapsedDyingTime += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);


                         // New: In testing: For speed-based animation playback.
                         TimeSpan dur = gameTime.ElapsedGameTime;
                         dur = TimeSpan.FromTicks((long)(dur.Ticks * Player.SlowMotionFactor));         // Was 0.75f.   1 is good for everything except ZombieWalk
                         GameTime oh = new GameTime(gameTime.TotalGameTime, dur);


                         // And update his Avatar!
                         Avatar.Update(oh);//gameTime);

                         // Return because we don't want to update his position or increase his ElapsedTime.
                         return;
                    }

                    if (CurrentWarpingState == WarpingState.Walking)
                    {
                         // Update the Enemy's Position (and thus Avatar Position).
                         this.Position = new Vector3(this.Position.X, 0, this.Position.Z - ((this.Speed / 25f) * Player.SlowMotionFactor));
                         this.Avatar.Position = this.Position;
                         this.WorldPosition = this.Avatar.WorldMatrix;
                    }


                         // Increment how long we've been fighting / targetting this Enemy!
                         this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
   
                    // New: In testing: For speed-based animation playback.
                    TimeSpan duration = gameTime.ElapsedGameTime;
                    duration = TimeSpan.FromTicks((long)(duration.Ticks * (Math.Abs(this.Speed) * Player.SlowMotionFactor)));         // Was 0.75f.   1 is good for everything except ZombieWalk
                    GameTime test = new GameTime(gameTime.TotalGameTime, duration);

                    // If we are using a BaseAnimation, it needs no play-speed tweaking.
                    if (CurrentWarpingState == WarpingState.Disappearing || 
                         CurrentWarpingState == WarpingState.Invisible ||
                         CurrentWarpingState == WarpingState.Reappearing)//if (Avatar.AvatarAnimation.GetType() == typeof(AvatarBaseAnimation))
                    {
                         // New: In testing: For speed-based animation playback.
                         TimeSpan dur = gameTime.ElapsedGameTime;
                         dur = TimeSpan.FromTicks((long)(dur.Ticks * 0.75f * Player.SlowMotionFactor));         // Was 0.75f.   1 is good for everything except ZombieWalk
                         GameTime oh = new GameTime(gameTime.TotalGameTime, dur);


                         Avatar.Update(oh);
                    }

                    // If we are using a custom, we tweak the play-speed based on movement speed.
                    else
                    {
                         Avatar.Update(test);
                    }
               }

               timeUntilWarp -= (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);

               // If we are here, it is time to disappear
               if (timeUntilWarp <= 1.0f && timeUntilWarp > 0f)
               {
                    //this.Avatar.AmbientLightColor = warpColor;

                    if (CurrentWarpingState != WarpingState.Disappearing)
                    {
                         AudioManager.PlayCue("Zombie_Warp");

                         Avatar.PlayAnimation(AnimationType.Jump, true);

                         CurrentWarpingState = WarpingState.Disappearing;
                    }
               }

               if (timeUntilWarp <= 0f)
               {
                    // Go invisible!
                    CurrentWarpingState = WarpingState.Invisible;

                    // Let's Warp here instead: This makes it so we can't hurt them in their old location while they're disappearing.
                    Warp();
               }

               if (timeUntilWarp <= -0.5f)
               {
                    //Warp();
                    timeUntilWarp = 3f + ((float)random.NextDouble() * 4f);

                    CurrentWarpingState = WarpingState.Reappearing;

                    reappearTime = 1.0f;
               }

               // Reappear
               if (CurrentWarpingState == WarpingState.Reappearing)
               {
                    this.Avatar.AmbientLightColor = warpColor;

                    reappearTime -= (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);

                    if (reappearTime <= 0f)
                    {
                         this.Avatar.AmbientLightColor = Color.White.ToVector3();
                         
                         Avatar.PlayAnimation(AnimationType.ZombieTwitchWalk, true);
         
                         CurrentWarpingState = WarpingState.Walking;
                    }
               }
          }

          public override void Draw(GameTime gameTime)
          {
               if (CurrentWarpingState != WarpingState.Invisible)
               {
                    base.Draw(gameTime);
               }
          }


          public void Warp()
          {
               hasWarpedAtLeastOnce = true;

               float randomX = (float)random.NextDouble() * 15f;

               float randomSign = random.Next(2);

               if (randomSign == 0)
                    randomX *= -1f;

               this.Position = new Vector3(randomX, Position.Y, Position.Z);

               this.Avatar.Position = this.Position;
               this.WorldPosition = this.Avatar.WorldMatrix;

               this.Avatar.AmbientLightColor = Color.White.ToVector3();
          }

          public override void OnKilled()
          {
               if (!hasWarpedAtLeastOnce)
               {
                    AvatarZombieGame.AwardData.StoppedWarperFromWarping = true;
               }

               base.OnKilled();
          }
     }
}