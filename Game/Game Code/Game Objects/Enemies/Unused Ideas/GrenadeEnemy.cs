#region File Description
//-----------------------------------------------------------------------------
// AttackerEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Slinger Enemy is one with average Health size and average speed.
     /// A Slinger Enemy actively attacks the player, through means of throwing a projectile.
     /// </summary>
     public class GrenadeEnemy : Enemy
     {
          #region Fields

          /// <summary>
          /// A list of Projectiles the Enemy has.
          /// </summary>
          public List<Grenade> GrenadeList = new List<Grenade>();

          /// <summary>
          /// The model used to represent the projectile.
          /// </summary>
          GameResourceModel3D grenadeModel;

          /// <summary>
          /// A helper variable to handle rotating the projectile.
          /// </summary>
          float rotation = 0;

          /// <summary>
          /// How long since the Enemy has last thrown a projectile.
          /// </summary>
          float elapsedTimeSinceThrew = 0.0f;

          /// <summary>
          /// How long since the Enemy has been told to Throw a projectile.
          /// 
          /// We use this to delay the time between the Enemy being told to Throw
          /// and the time when the projectile is actually thrown.
          /// 
          /// This way we give the Enemy a second to do his Throw animation before
          /// the projectile is actually released.
          /// </summary>
          float timeSinceThrown = 0.0f;

          /// <summary>
          /// How long the Enemy should wait in between Throws.
          /// 
          /// The default time is 10 seconds, but we may randomize this.
          /// </summary>
          float timeBetweenThrows = 5.0f;

          /// <summary>
          /// Whether or not the Enemy is currently in the process of Throwing.
          /// </summary>
          bool isThrowing = false;

          /// <summary>
          /// Helper variable for putting the Attacker Enemy's projectile
          /// in the correct screen space.
          /// </summary>
          private Vector3 projectilePosition = new Vector3();

          #endregion


          #region Properties

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public GrenadeEnemy(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               // Add 50% for each higher difficulty.
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
               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedKillTime = 0.0f;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.Health = 2;

               // Every 10 waves, increase their Health by 1.
               this.Health += ((this.Wave.WaveNumber) / 10);

               // And increase their Health if we are on Hard Difficulty.
               if (ZombieGameSettings.Difficulty == Difficulty.Hard)
               {
                    this.Health += 1;
               }

               this.startingHealth = this.Health;

               // Prevent Enemies that spawning at same moment from throwing at same time.
               random = new Random();

               timeBetweenThrows = 1.0f + (float)(random.NextDouble() * 5f);

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

               grenadeModel = ResourceManager.LoadModel3D(@"Models\Weapons\Grenade");

               // Tint the ball red in color.
               grenadeModel.Model3D.AmbientLightColor = Color.Black;
               grenadeModel.Model3D.EmissiveColor = Color.Black;
               grenadeModel.Model3D.SpecularColor = Color.Black;

               // Initialize the list of bones in world space
               Avatar.BonesInWorldSpace = new List<Matrix>(AvatarRenderer.BoneCount);

               for (int i = 0; i < AvatarRenderer.BoneCount; i++)
                    Avatar.BonesInWorldSpace.Add(Matrix.CreateTranslation(new Vector3(0f, -10f, -50f)));//Matrix.Identity);
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();

               if (this.GrenadeList != null)
               {
                    this.GrenadeList.Clear();
                    this.GrenadeList = null;
               }
          }

          #endregion


          #region Update

          /// <summary>
          /// 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               UpdateGrenades(gameTime);




               

               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    Avatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                    Avatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                    Avatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
               }

               if (this.IsDying)
               {
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Avatar.Update(gameTime);
                    return;
               }

               if (!isThrowing)
               {
                    // Update the Enemy's Position (and thus Avatar Position).
                    this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
                    this.Avatar.Position = this.Position;
                    this.WorldPosition = this.Avatar.WorldMatrix;
               }

               elapsedTimeSinceThrew += (float)gameTime.ElapsedGameTime.TotalSeconds;
               this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


               if (elapsedTimeSinceThrew >= timeBetweenThrows)
               {
                    isThrowing = true;
                    elapsedTimeSinceThrew = 0.0f;
                    timeBetweenThrows = 1.0f + (float)(random.NextDouble() * 6f);
                    Avatar.PlayAnimation(AnimationType.Throw, false);
               }

               if (isThrowing)
               {
                    timeSinceThrown += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timeSinceThrown >= 0.75f)
                    {
                         timeSinceThrown = 0.0f;
                         ThrowGrenade();
                         isThrowing = false;
                    }
               }

               Avatar.Update(gameTime);

               // If we are using an AvatarCustomAnimation, it must be the Throw animation...
               if (Avatar.AvatarAnimation.GetType() == typeof(AvatarCustomAnimation))
               {
                    // If the Throw animation is finished...
                    if (Avatar.AvatarAnimation.IsFinished)
                    {
                         // Go back to his Zombie Twitch Walk!
                         Avatar.PlayAnimation(AnimationType.ZombieWalk, true);
                    }
               }
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the animated enemy.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Draw the enemy (his / her avatar).
               Avatar.Draw(gameTime);

               // If the enemy is dying...
               if (this.IsDying)
               {
                    // Draw the Money Dropped text.
                    DrawMoneyDropped(gameTime);
               }

               // Draw the Health Meter only if enemy is not dying.
               else
               {
                    DrawHealthMeter(gameTime);
               }
               
               MySpriteBatch.End();

               MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred);

               foreach (Grenade grenade in GrenadeList)
               {
                    grenade.Draw(gameTime);
               }

               MySpriteBatch.End();
          }

          #endregion


          #region Projectile Shooting Methods

          public void ThrowGrenade()
          {
               Random random = new Random();

               // Create our new Grenade at the Player's location.
               Grenade grenade = new Grenade(this.Game);
               grenade.Position = projectilePosition;
               grenade.Velocity = new Vector3(0f, 0.1f, -0.2f);

               // And add it to the Player's Grenade list.
               GrenadeList.Add(grenade);
          }

          #endregion


          /// <summary>
          /// Updates the Player's Grenades by iterating through
          /// the Grenade list and calling Grenade.Update().
          /// </summary>
          /// <param name="gameTime"></param>
          private void UpdateGrenades(GameTime gameTime)
          {
               if (GrenadeList != null)
               {
                    foreach (Grenade nade in GrenadeList)
                    {
                         nade.Update(gameTime, this.Avatar);

                         if (nade.RemoveGrenade)
                         {
                              GrenadeList.Remove(nade);
                              break;
                         }
                    }
               }
          }


          #region Check For Collisions With Grenades - Awful Spot!!!

          private void CheckGrenadeCollisions(GameTime gameTime)
          {
               if (GrenadeList != null)
               {
                    foreach (Grenade nade in GrenadeList)
                    {
                         if (nade.ReadyToKill && !nade.Detonated)
                         {
                              nade.Detonated = true;

                              // Hit Player.
                         }
                    }
               }
          }

          #endregion

     }
}