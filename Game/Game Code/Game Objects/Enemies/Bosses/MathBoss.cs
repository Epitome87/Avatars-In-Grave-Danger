#region File Description
//-----------------------------------------------------------------------------
// MathBoss.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using System.Collections.Generic;
using PixelEngine;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Boss which utilizes Math to harm the Player!
     /// </summary>
     public class MathBoss : Enemy
     {
          List<Avatar> bossDuplicates = new List<Avatar>();

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


          // The close we are to warping, the darker color we are.
          private Vector3 warpColor = new Vector3(-10f);

          #endregion


          #region Initialization

          /// <summary>
          /// FastTypingEnemy Constructor.
          /// Creates a new Fast Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public MathBoss(Vector3 position, Wave wave)
               : base(PixelEngine.EngineCore.Game, wave)
          {
               this.Speed = 0f;

               this.Position = new Vector3(0f, 0f, 10f);// position;

               this.Avatar.Scale = 5f;

               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedKillTime = 0f;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.Health = 10;

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
                    // Increment how long he's been in his Dying state!
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // And update his Avatar!
                    Avatar.Update(gameTime);

                    // Return because we don't want to update his position or increase his ElapsedTime.
                    return;
               }

               // Update the Enemy's Position (and thus Avatar Position).
               this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;
               this.WorldPosition = this.Avatar.WorldMatrix;


               // Increment how long we've been fighting / targetting this Enemy!
               this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


               if (this.ElapsedKillTime >= 10f)
               {
                    if (!hasMultiplied)
                    {
                         Multiply();
                    }
               }

               Avatar.Update(gameTime);

               foreach (Avatar duplicate in bossDuplicates)
               {
                    duplicate.AmbientLightColor = this.Color.ToVector3();

                    if (AvatarZombieGame.SeizureModeEnabled)
                    {
                         duplicate.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                         duplicate.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                         duplicate.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
                    }

                    duplicate.Update(gameTime);
               }
          }

          #endregion

          bool hasMultiplied = false;

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               if (CurrentWarpingState != WarpingState.Invisible)
               {
                    base.Draw(gameTime);
               }

               foreach (Avatar duplicate in bossDuplicates)
               {
                    duplicate.Draw(gameTime);
               }
          }

          #endregion


          #region Warp

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

          #endregion


          #region Multiply!

          private void Multiply()
          {
               hasMultiplied = true;

               bossDuplicates.Clear();

               // Creates copies of himself.
               for (int i = 0; i < 3; i++)
               {
                    Avatar duplicateAvatar = new Avatar(EngineCore.Game);

                    duplicateAvatar.AvatarDescription = this.Avatar.AvatarDescription;

                    duplicateAvatar.PlayAnimation(AvatarAnimationPreset.Clap, true);

                    duplicateAvatar.Scale = this.Avatar.Scale;

                    duplicateAvatar.Position = new Vector3(-3 * i, 0f, 10f);

                    bossDuplicates.Add(duplicateAvatar);
               }
          }

          #endregion
     }
}