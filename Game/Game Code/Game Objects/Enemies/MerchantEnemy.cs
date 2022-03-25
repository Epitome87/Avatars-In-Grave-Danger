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
using PixelEngine.CameraSystem;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Slinger Enemy is one with average Health size and average speed.
     /// A Slinger Enemy actively attacks the player, through means of throwing a projectile.
     /// </summary>
     public class MerchantEnemy : Enemy
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





          /// <summary>
          /// Lines the Merchant shouts periodically when he is being
          /// hurt by the Player's attacks.
          /// </summary>
          private string[] hurtTalkingPoints = new string[] 
          { 
               "You're using my own items against me. The irony!",
               "You're hurting my feelings.\nAnd my body.",
               "It's not too late to make amends.",
               "Just let hobble my way past you.",
          };


          private string[] walkingTalkingPoints = new string[]
          {
               ""
          };

          #endregion


          #region Properties

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public MerchantEnemy(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               // Add 50% for each higher difficulty.
               this.Speed = (0.5f / 4.0f) * (1f + ((int)(ZombieGameSettings.Difficulty) * 0.5f));


               this.Position = position;
               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedKillTime = 0.0f;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.Health = 21;

               this.startingHealth = this.Health;

               // Prevent Enemies that spawning at same moment from throwing at same time.
               random = new Random();

               timeBetweenThrows = 1.0f + (float)(random.NextDouble() * 5f);

               this.Initialize();

               this.Avatar.AvatarDescription = new AvatarDescription(AvatarDatabase.MerchantAvatarDescription);


               talkingPoints.Add("");
               talkingPoints.Add("You're using my own items against me. The irony!");
               talkingPoints.Add("You're hurting my feelings.\nAnd my body.");
               talkingPoints.Add("It's not too late to make amends.");
               talkingPoints.Add("Just let hobble my way past you.");
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
                         projectileBoundingSphere.Radius = 0.01f;

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
                    timeSinceThrown += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timeSinceThrown >= 0.75f)
                    {
                         timeSinceThrown = 0.0f;
                         ShootProjectile();
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

          private bool isTalking = false;

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
                    if (!StartScreen.IsOnStartScreen)
                         DrawHealthMeter(gameTime);
               }

               if (IsEscaping)
               {
                    DrawEscapeText(gameTime);
               }

               if (IsCloseCallKill)
               {
                    DrawCloseCallText(gameTime);
               }

               if (isTalking)
               {
                    DrawDialogue(gameTime);
                    
                    timeUntilDialogueIsDone -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timeUntilDialogueIsDone <= 0)
                    {
                         timeUntilDialogueIsDone = 3.0f;
                         isTalking = false;
                         talkingPoints.RemoveAt(talkingPoints.Count - 1);
                    }
               }

               foreach (Projectile projectile in ProjectileList)
               {
                    Matrix mat = Matrix.CreateScale(0.0025f * 2f) *
                         Matrix.CreateRotationZ(MathHelper.ToRadians(-180f)) *
                         Matrix.CreateRotationX(MathHelper.ToRadians(rotation -= 7.5f)) *
                         Matrix.CreateTranslation(projectile.Position);

                    projectileModel.DrawModel(mat);
               }


               // Draw a ball always in their hand.
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

               MySpriteBatch.End();
          }

          List<string> talkingPoints = new List<string>();
          float timeUntilDialogueIsDone = 3.0f;

          private void DrawDialogue(GameTime gameTime)
          {
               // This will render it around their ankles.
               Matrix renderPosition =
                    Matrix.CreateTranslation(new Vector3(this.Position.X, this.Position.Y + (this.Avatar.AvatarDescription.Height * this.Avatar.Scale / 4f), this.Position.Z));

               Vector3 screenSpace = PixelEngine.Graphics.GraphicsHelper.ConvertToScreenspaceVector3(renderPosition);

               if (talkingPoints != null && talkingPoints.Count > 0)
               {
                    TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.TitleFont].SpriteFont, 
                         talkingPoints[talkingPoints.Count - 1], new Vector2(screenSpace.X, screenSpace.Y), Color.Red, 0.25f);
               }
          }

          #endregion


          public override void DrawHealthMeter(GameTime gameTime)
          {
               // Use this to render it above their head.
               //Matrix renderPosition = Matrix.CreateTranslation(new Vector3(this.position.X, this.position.Y + this.Avatar.AvatarDescription.Height * this.Avatar.Scale * 1.20f, this.position.Z));

               // This will render it around their ankles.
               Matrix renderPosition =
                    Matrix.CreateTranslation(new Vector3(this.Position.X, this.Position.Y + (this.Avatar.AvatarDescription.Height * this.Avatar.Scale / 4f), this.Position.Z));

               Vector3 screenSpace = PixelEngine.Graphics.GraphicsHelper.ConvertToScreenspaceVector3(renderPosition);

               // Prevent the Health Meter from being rendered if the Enemy is behind the Camera.
               if (this.Position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    Color healthColor = Color.Green;

                    if (this.Health <= (this.startingHealth * 0.75f))
                    {
                         healthColor = Color.Yellow;
                    }

                    if (this.Health <= (this.startingHealth * 0.50f))
                    {
                         healthColor = Color.DarkOrange;
                    }

                    if (this.Health <= (this.startingHealth * 0.25f))
                    {
                         healthColor = Color.Red;
                    }

                    MySpriteBatch.DrawCentered(blankTexture.Texture2D, new Rectangle((int)screenSpace.X, (int)screenSpace.Y, 4 + (int)(this.Health * 5), 4 + 15), Color.Black);
                    MySpriteBatch.DrawCentered(gradientTexture.Texture2D, new Rectangle((int)screenSpace.X, (int)screenSpace.Y, (int)(this.Health * 5), 15), healthColor * 1f);
               }
          }



          /// <summary>
          /// Called whenever an Enemy is hit.
          /// 
          /// Does all logic corresponding to being hit, such as
          /// getting rid of the shot character, flinging it off screen, 
          /// playing the Shot Sound effect, etc.
          /// </summary>
          public override void OnHit(Player playerWhoHit)
          {
               this.Wave.NumberOfHits++;

               // Sound of Bullet connecting to Enemy.
               AudioManager.PlayCue(this.CollisionSound);

               this.Health -= playerWhoHit.Gun.BulletDamage / 5f;


               if ((int)this.Health == 20)
               {
                    isTalking = true;
               }

               if ((int)this.Health == 15)
               {
                    isTalking = true;
               }

               // If the Enemy is point blank...
               if (Math.Abs(this.Position.Z - playerWhoHit.Position.Z) < 3f)
               {
                    // Deal the damage again: This means 2x damage!
                    this.Health -= playerWhoHit.Gun.BulletDamage;

                    // Or let's do 1-Hit KO for now...
                    this.Health = 0;

                    IsCloseCallKill = true;

                    this.Wave.NumberOfCloseCalls++;

                    AvatarZombieGame.AwardData.TotalCloseCalls++;
               }

               if (this.Health <= 0)
               {
                    this.OnKilled();
               }

               // New change: Only play the Hit sound if Enemy isn't dying.
               // No reason to play Hit and Death sound at once!
               else
               {
                    // Sound of Enemy's reaction to getting hit.
                    AudioManager.PlayCue(this.HurtSound);
               }
          }


          #region Projectile Shooting Methods

          public void ShootProjectile()
          {
               Random random = new Random();

               Projectile projectile = new Projectile();

               projectile.Position = projectilePosition;
               projectile.Speed = (15.0f / 2.0f) * (int)(ZombieGameSettings.Difficulty + 1) * (0.50f);

               // Now add 10% speed each Wave.
               projectile.Speed *= MathHelper.Clamp((1.0f + (0.1f * Wave.WaveNumber)), 1.0f, 2.0f);

               ProjectileList.Add(projectile);
          }

          #endregion
     }
}