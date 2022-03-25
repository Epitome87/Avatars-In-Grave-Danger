using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PixelEngine.ResourceManagement;
using PixelEngine;
using PixelEngine.Avatars;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.CameraSystem;

namespace AvatarsInGraveDanger
{
     public class BaseballEnemy : Enemy
     {
          #region Fields

          /// <summary>
          /// The model used to represent the Baseball Bat.
          /// </summary>
          GameResourceModel3D baseballbatModel;

          /// <summary>
          /// A helper variable to handle rotating the projectile.
          /// </summary>
          float rotation = 0;

          /// <summary>
          /// How long since the Enemy has last thrown a projectile.
          /// </summary>
          float elapsedTimeSinceSwung = 0.0f;

          /// <summary>
          /// How long the Enemy should wait in between Throws.
          /// 
          /// The default time is 10 seconds, but we may randomize this.
          /// </summary>
          float timeBetweenSwings = 5.0f;


          float timeSwingAnimationLasts = 0f;

          /// <summary>
          /// Whether or not the Enemy is currently in the process of Swinging.
          /// </summary>
          bool isSwinging = false;

          /// <summary>
          /// Helper variable for putting the Attacker Enemy's Baseball Bat
          /// in the correct screen space.
          /// </summary>
          private Vector3 batPosition = new Vector3();

          #endregion


          #region Properties

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public BaseballEnemy(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               this.Speed = (1.5f / 2.0f) * (int)(ZombieGameSettings.Difficulty + 1) * (0.50f);

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

               // Prevent Enemies that spawning at same moment from throwing at same time.
               random = new Random();

               timeBetweenSwings = timeBetweenSwings = 1.0f + (float)(random.NextDouble() * 6f); //3f;

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

               baseballbatModel = ResourceManager.LoadModel3D(@"Models\baseballbat");

               // Tint the ball red in color.
               baseballbatModel.Model3D.AmbientLightColor = Color.DarkRed;
               baseballbatModel.Model3D.EmissiveColor = Color.DarkRed;
               baseballbatModel.Model3D.SpecularColor = Color.DarkRed;

               // Initialize the list of bones in world space
               Avatar.BonesInWorldSpace = new List<Matrix>(AvatarRenderer.BoneCount);

               for (int i = 0; i < AvatarRenderer.BoneCount; i++)
                    Avatar.BonesInWorldSpace.Add(Matrix.Identity);
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion


          #region Update

          private Player playerRef;

          /// <summary>
          /// 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               /*
               if (playerRef.Gun.BulletList != null)
               {
                    foreach (Bullet bullet in playerRef.Gun.BulletList)
                    {
                         BoundingSphere projectileBoundingSphere = new BoundingSphere();
                         projectileBoundingSphere.Center = bullet.Position;
                         projectileBoundingSphere.Radius = 0.01f;

                         BoundingSphere targetBoundingSphere = new BoundingSphere();
                         targetBoundingSphere.Center = new Vector3(Wave.CurrentPlayer.Position.X, bullet.Position.Y, Wave.CurrentPlayer.Position.Z);
                         targetBoundingSphere.Radius = (Wave.CurrentPlayer.Avatar.AvatarDescription.Height * Wave.CurrentPlayer.Avatar.Scale * 0.5f) / 2.5f;

                         ContainmentType collisionType = targetBoundingSphere.Contains(projectileBoundingSphere);

                         if (collisionType != ContainmentType.Disjoint)
                         {
                              // This projectile is now gone.
                              playerRef.Gun.BulletList.Remove(bullet);

                              // Play the collision sound.
                              AudioManager.PlayCue("Zombie_Hit");

                              // Player loses a Health.
                              Wave.CurrentPlayer.Health--;

                              break;
                         }

                         // The projectile has long-since missed the Player; time to remove it!
                         if (bullet.Position.Z <= Wave.CurrentPlayer.Position.Z - 5f)
                         {
                              playerRef.Gun.BulletList.Remove(bullet);
                              break;
                         }
                    }
               }
               */

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

               if (!isSwinging)
               {
                    // Update the Enemy's Position (and thus Avatar Position).
                    this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
                    this.Avatar.Rotation = new Vector3(0);
                    this.Avatar.Position = this.Position;
                    this.WorldPosition = this.Avatar.WorldMatrix;
               }

               this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               elapsedTimeSinceSwung += (float)gameTime.ElapsedGameTime.TotalSeconds;

               this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


               if (elapsedTimeSinceSwung >= timeBetweenSwings)
               {
                    isSwinging = true;

                    elapsedTimeSinceSwung = 0.0f;

                    timeBetweenSwings = 1.0f + (float)(random.NextDouble() * 6f);

                    Avatar.PlayAnimation(AnimationType.Swing, false);
               }

               if (isSwinging)
               {
                    timeSwingAnimationLasts += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    this.Avatar.Rotation = new Vector3(0f, -90f, 0f);

                    if (timeSwingAnimationLasts >= 1.25f)
                    {
                         isSwinging = false;

                         timeSwingAnimationLasts = 0f;

                         // Go back to his Zombie Walk!
                         Avatar.PlayAnimation(AnimationType.ZombieWalk, true);
                    }
               }

               Avatar.Update(gameTime);
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


               // Draw a ball always in their hand.
               DrawBaseballBat(gameTime);


               MySpriteBatch.End();
          }

          #endregion
          /// <summary>
          /// Draw the baseball bat
          /// </summary>
          private void DrawBaseballBat(GameTime gameTime)
          {
               Matrix baseballBatOffset = Matrix.CreateScale(1f);

               // Render the Gun model now.
               if (this.Avatar.BonesInWorldSpace != null)
               {
                    baseballbatModel.DrawModel(baseballBatOffset * this.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight]);

                    Vector3 scale = new Vector3();
                    Quaternion quat = new Quaternion();
                    this.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Decompose(out scale, out quat, out batPosition);
               }

               /*
               // Moves the bat closer to where we want it in the hand
               Matrix baseballBatOffset = Matrix.CreateRotationY(MathHelper.ToRadians(-20)) * Matrix.CreateTranslation(0.01f, 0.05f, 0.0f);

               foreach (ModelMesh mesh in this.baseballbatModel.Model3D.Model.Meshes)
               {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                         effect.EnableDefaultLighting();

                         // Position the bat to be near the avatars right hand. The position
                         // of the right special bone can be found by looking up the value in
                         // our list of world space bones with the index of the bone we are 
                         // looking for. The bat is translated and rotated a small amount to
                         // make it look better in the hand.
                         effect.World = baseballBatOffset * this.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight];

                         effect.View = CameraManager.ActiveCamera.ViewMatrix;
                         effect.Projection = CameraManager.ActiveCamera.ProjectionMatrix;
                    }
                    mesh.Draw();
               }
               */
          }
     }
}
