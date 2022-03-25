#region File Description
//-----------------------------------------------------------------------------
// ThiefEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Screen;
using PixelEngine;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// Idea #1:
     /// 
     /// A Thief Enemy is one which spawns BEHIND the Player.
     /// 
     /// Once spawned, a Quick Time Event is initiated. 
     /// If the Player fails the QTE, the Thief Enemy will
     /// steal Money, Life, or an Upgrade from the Player.
     /// ---------------------------------------------------------
     /// Idea #2:
     /// 
     /// A Thief Enemy is one which crawls to the Player.
     /// 
     /// Soon after Spawning, a Quick Time Event is initiated.
     /// If the Player fails the QTE, the Thief Enemy will ram
     /// into him, knocking him over. This results in the Enemy
     /// stealing Money, a Life, or an Upgrade from the Player.
     /// </summary>
     public class ThiefEnemy : Enemy
     {
          #region Fields

          /// <summary>
          /// How long remains until the Quick Time Event begins.
          /// </summary>
          private float timeUntilQTE = 3.5f;

          /// <summary>
          /// Whether or not we are in the Quick Time Event already.
          /// </summary>
          private bool isInQTE = false;

          #endregion


          #region Initialization

          /// <summary>
          /// FastTypingEnemy Constructor.
          /// Creates a new Fast Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public ThiefEnemy(Vector3 position, Wave wave)
               : base(PixelEngine.EngineCore.Game, wave)
          {
               this.Speed = (3f / 2.0f) * (int)(ZombieGameSettings.Difficulty + 1) * (0.50f);

               if (ZombieGameSettings.Difficulty == Difficulty.Hard)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               // But we reset the speed every 10 waves.
               this.Speed *= MathHelper.Clamp((1.0f + (0.05f * (Wave.WaveNumber % 10))), 1.0f, 1.75f);

               this.Position = position;
               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedKillTime = 0.0f;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

 
               // Every 10 waves, increase their Health by 1.
               if (this.Wave.WaveNumber + 1 < 10)
               {
                    this.Health = 2;
               }

               else if (this.Wave.WaveNumber + 1 > 10)
               {
                    this.Health = 3;
               }

               else if (this.Wave.WaveNumber + 1 > 20)
               {
                    this.Health = 4;
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

               Avatar.PlayAnimation(AnimationType.Crawl, true);

               random = new Random();

               timeUntilQTE = 1.5f + ((float)random.NextDouble() * 2);
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
 
               // New: In testing: For speed-based animation playback.
               TimeSpan duration = gameTime.ElapsedGameTime;
               duration = TimeSpan.FromTicks((long)(duration.Ticks * (Math.Abs(this.Speed))));         // / 1 is good for everything except ZombieWalk
               GameTime test = new GameTime(gameTime.TotalGameTime, duration);
               Avatar.Update(test);//duration);//gameTime);

               timeUntilQTE -= (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (!isInQTE && timeUntilQTE <= 0)
               {
                    InitiateQTE();

                    isInQTE = true;
               }

               if (isInQTE)
               {
                    HandleQTE(gameTime);
               }
          }

          #endregion


          #region Quick Time Event Methods

          public void InitiateQTE()
          {
               if (this.Avatar.AvatarDescription.BodyType == AvatarBodyType.Male)
               {
                    AudioManager.PlayCue("Zombie_Charge1_Male");
               }

               else
               {
                    AudioManager.PlayCue("Zombie_Charge1_Female");
               }

               Speed *= 2.5f;
          }

          #endregion


          #region Overridden On Killed Method

          public override void OnKilled()
          {
               if (!isInQTE)
               {
                    AvatarZombieGame.AwardData.StoppedBeserkerFromCharging = true;
               }

               base.OnKilled();
          }

          #endregion


          #region Handle Quick Time Event Input

          GamePadState gpState = new GamePadState();

          InputState inputState = new InputState();

          private void HandleQTE(GameTime gameTime)
          {
               PlayerIndex playerIndex;

               if (inputState.IsNewButtonPress(Buttons.A, EngineCore.ControllingPlayer.Value, out playerIndex))
               {
               }
          }

          #endregion
     }
}
