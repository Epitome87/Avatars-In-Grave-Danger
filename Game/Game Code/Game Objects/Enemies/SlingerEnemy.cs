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
using PixelEngine.Screen;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Slinger Enemy is one with average Health size and average speed.
     /// A Slinger Enemy actively attacks the player, through means of throwing a projectile.
     /// </remarks>
     public class SlingerEnemy : Enemy
     {
          #region Fields

          /// <summary>
          /// A list of Projectiles the Enemy has.
          /// </summary>
          public List<Projectile> ProjectileList = new List<Projectile>();

          /// <summary>
          /// The model used to represent the projectile.
          /// </summary>
          GameResourceModel3D projectileModel;

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
          
          private float timeObscuringLasts = 3.0f;
          private bool isObscuringScreen = false;
          GameResourceTexture2D trollTexture;

          #region Properties

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public SlingerEnemy(Vector3 position, Wave wave)
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

               projectileModel = ResourceManager.LoadModel3D(@"Models\Weapons\TrollFaceBall");

               trollTexture = ResourceManager.LoadTexture(@"TrollFace");

               // Tint the ball red in color.
               projectileModel.Model3D.AmbientLightColor = Color.Black;
               projectileModel.Model3D.EmissiveColor = Color.Black;
               projectileModel.Model3D.SpecularColor = Color.Black;

               // Initialize the list of bones in world space
               Avatar.BonesInWorldSpace = new List<Matrix>(AvatarRenderer.BoneCount);

               for (int i = 0; i < AvatarRenderer.BoneCount; i++)
                    Avatar.BonesInWorldSpace.Add(Matrix.CreateTranslation(new Vector3(0f, -10f, -50f)));//Matrix.Identity);
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();

               if (this.ProjectileList != null)
               {
                    this.ProjectileList.Clear();
                    this.ProjectileList = null;
               }
          }

          #endregion


          #region Update

          /// <summary>
          /// 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               if (ProjectileList != null)
               {
                    // Iterate through each of the Enemy's projectiles.
                    foreach (Projectile projectile in ProjectileList)
                    {
                         // And update them!
                         projectile.Update(gameTime);
                    }
               }

               if (ProjectileList != null)
               {
                    foreach (Projectile projectile in ProjectileList)
                    {
                         BoundingSphere projectileBoundingSphere = new BoundingSphere();
                         projectileBoundingSphere.Center = projectile.Position;
                         projectileBoundingSphere.Radius = 0.01f;// 0.0025f * 2.5f;//0.01f;

                         BoundingSphere targetBoundingSphere = new BoundingSphere();
                         targetBoundingSphere.Center = new Vector3(Wave.CurrentPlayer.Position.X, projectile.Position.Y, Wave.CurrentPlayer.Position.Z);
                         targetBoundingSphere.Radius = (Wave.CurrentPlayer.Avatar.AvatarDescription.Height * Wave.CurrentPlayer.Avatar.Scale * 0.5f) / 2.5f;

                         ContainmentType collisionType = targetBoundingSphere.Contains(projectileBoundingSphere);

                         if (collisionType != ContainmentType.Disjoint)
                         {
                              // This projectile is now gone.
                              ProjectileList.Remove(projectile);

                              // Play the collision sound.
                              AudioManager.PlayCue("Zombie_Hit");

                              // Player loses a Health.
                              Wave.CurrentPlayer.Health--;

                              // Set the Controller Vibration.
                              InputState.SetVibration(EngineCore.ControllingPlayer, 1.0f, 1.0f, 0.50f);


                              isObscuringScreen = true;

                              break;
                         }

                         // The projectile has long-since missed the Player; time to remove it!
                         if (projectile.Position.Z <= Wave.CurrentPlayer.Position.Z - 5f)
                         {
                              ProjectileList.Remove(projectile);
                              break;
                         }
                    }
               }

               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    Avatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                    Avatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                    Avatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
               }

               if (this.IsDying)
               {
                    this.Color = Color.White;
                                            
                    elapsedDyingTime += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);



                    // New: In testing: For speed-based animation playback.
                    TimeSpan dur = gameTime.ElapsedGameTime;
                    dur = TimeSpan.FromTicks((long)(dur.Ticks * Player.SlowMotionFactor));         // Was 0.75f.   1 is good for everything except ZombieWalk
                    GameTime oh = new GameTime(gameTime.TotalGameTime, dur);


                    Avatar.Update(oh);

                    return;
               }

               if (!isThrowing)
               {
                    // Update the Enemy's Position (and thus Avatar Position).
                    this.Position = new Vector3(this.Position.X, 0, this.Position.Z - ((this.Speed / 25f) * Player.SlowMotionFactor));
                    this.Avatar.Position = this.Position;
                    this.WorldPosition = this.Avatar.WorldMatrix;
               }

               elapsedTimeSinceThrew += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);
               this.ElapsedKillTime += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);


               if (elapsedTimeSinceThrew >= timeBetweenThrows)
               {
                    if (Math.Abs(this.Wave.CurrentPlayer.Position.Z - this.Position.Z) > 2.0f)
                    {
                         isThrowing = true;
                         elapsedTimeSinceThrew = 0.0f;
                         timeBetweenThrows = 1.0f + (float)(random.NextDouble() * 6f);
                         Avatar.PlayAnimation(AnimationType.Throw, false);
                    }
               }

               if (isThrowing)
               {
                    timeSinceThrown += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);

                    if (timeSinceThrown >= 0.75f)
                    {
                         timeSinceThrown = 0.0f;
                         ThrowProjectile();
                         isThrowing = false;
                    }
               }




               // New: In testing: For speed-based animation playback.
               TimeSpan durp = gameTime.ElapsedGameTime;
               durp = TimeSpan.FromTicks((long)(durp.Ticks * Player.SlowMotionFactor));         // Was 0.75f.   1 is good for everything except ZombieWalk
               GameTime ah = new GameTime(gameTime.TotalGameTime, durp);


               Avatar.Update(ah);







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



               if (isObscuringScreen)
               {
                    timeObscuringLasts -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timeObscuringLasts <= 0)
                    {
                         isObscuringScreen = false;
                         timeObscuringLasts = 3.0f;
                    }
               }
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Slinger Enemy.
          /// 
          /// We override this so we can add Slinger-specific rendering,
          /// such as rendering his projectiles.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
               
               MySpriteBatch.Begin(BlendState.AlphaBlend);

               foreach (Projectile projectile in ProjectileList)
               {
                    Matrix mat = Matrix.CreateScale(0.0025f * 2.5f) *
                         Matrix.CreateRotationZ(MathHelper.ToRadians(-180f)) *         
                         Matrix.CreateRotationX(MathHelper.ToRadians(rotation -= 7.5f)) *
                         Matrix.CreateTranslation(projectile.Position);

                    projectileModel.DrawModel(mat);
               }


               // Draw a ball always in their hand - slightly smaller than those that are thrown.
               Matrix projectileOffset = 
                    Matrix.CreateScale(0.0025f * 1.5f) * 
                    Matrix.CreateRotationZ(MathHelper.ToRadians(-90f)) * 
                    Matrix.CreateRotationX(MathHelper.ToRadians(-90f));

               // Render the ball model now.
               if (this.Avatar.BonesInWorldSpace != null)
               {
                    projectileModel.DrawModel(projectileOffset * this.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight]);
                    
                    Vector3 scale = new Vector3();
                    Quaternion quat = new Quaternion();
                    this.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Decompose(out scale, out quat, out projectilePosition);
               }



               if (isObscuringScreen)
               {
                    DrawTrollFace(gameTime);
               }

               MySpriteBatch.End();
          }

          #endregion


          #region Obscure Screen!

          private void DrawTrollFace(GameTime gameTime)
          {
               //MySpriteBatch.DrawCentered(trollTexture.Texture2D, new Rectangle((int)EngineCore.ScreenCenter.X, (int)EngineCore.ScreenCenter.Y, 550, 500), Color.White, 0f);
          }

          #endregion


          #region Projectile Shooting Methods

          public void ThrowProjectile()
          {
               Random random = new Random();

               Projectile projectile = new Projectile();

               projectile.Position = projectilePosition;
               projectile.Speed = (15.0f / 2.0f) * (int)(ZombieGameSettings.Difficulty + 1);

               // Now add 15% speed each Wave. Use to be 10%.
               projectile.Speed *= MathHelper.Clamp((1.0f + (0.15f * Wave.WaveNumber)), 1.0f, 4.0f);          // Clamp at 4.0 now, not 2.0

               ProjectileList.Add(projectile);
          }

          #endregion
     }
}