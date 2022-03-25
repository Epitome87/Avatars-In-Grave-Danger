#region File Description
//-----------------------------------------------------------------------------
// TypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace AvatarsInGraveDanger
{
     #region Helper IComparer Class

     /// <summary>
     /// Compares to Enemy's Priorities.
     /// </summary>
     public class CompareByPrioty : IComparer<Enemy>
     {

          public CompareByPrioty()
          {
          }

          // Implementing the Compare method
          public int Compare(Enemy obj1, Enemy obj2)
          {
               Enemy Temp1 = obj1;
               Enemy Temp2 = obj2;

               if (Temp2.Position.Z < Temp1.Position.Z)
               {
                    return 1;
               }

               if (Temp2.Position.Z > Temp1.Position.Z)
               {
                    return -1;
               }

               else
               {
                    return 0;
               }
          }

          public int Compare1(Enemy obj1, Enemy obj2)
          {
               Enemy Temp1 = obj1;
               Enemy Temp2 = obj2;

               Vector2 testPosition2 = GraphicsHelper.ConvertToScreenspaceVector2(Temp2.Avatar.WorldMatrix);
               Vector2 testPosition1 = GraphicsHelper.ConvertToScreenspaceVector2(Temp1.Avatar.WorldMatrix);

               if (testPosition2.Y < testPosition1.Y)
               {
                    return 1;
               }

               if (testPosition2.Y > testPosition1.Y)
               {
                    return -1;
               }

               else
               {
                    return 0;
               }
          }
     }

     #endregion


     #region EnemyType Enum

     public enum EnemyType
     {
          Normal,        // 0
          Slinger,      // 1
          Beserker,      // 2          
          Warper,       // 3
          Horde,         // 4

          Bonus,         // 5
          Boss,          // 6  
     }

     #endregion

     /// <remarks>
     /// An abstract Typing Enemy. 
     /// Describes the Enemy as well as his sentences / key lists.
     /// </remarks>
     public abstract class Enemy : DrawableGameComponent, ICameraTrackable
     {

          public BoundingSphere GetBoundingSphereForEntireBody()
          {
               BoundingSphere enemyBoundingSphere = new BoundingSphere();

               enemyBoundingSphere.Center =
                             new Vector3(Avatar.Position.X, Avatar.AvatarDescription.Height * Avatar.Scale * 0.5f, Avatar.Position.Z);
               // +Vector3.Up * (enemy.Avatar.AvatarDescription.Height * enemy.Avatar.Scale * 0.5f);


               enemyBoundingSphere.Radius =
                    (Avatar.AvatarDescription.Height * Avatar.Scale * 0.5f) / 2.5f;

               return enemyBoundingSphere;
          }

          public BoundingSphere GetBoundingSphereForLegs()
          {
               BoundingSphere enemyBoundingSphere = new BoundingSphere();

               enemyBoundingSphere.Center =
                             new Vector3(Avatar.Position.X, Avatar.AvatarDescription.Height * Avatar.Scale * 0.25f, Avatar.Position.Z);

               // The radius is 25% the body (diameter is 1/2 the body)
               enemyBoundingSphere.Radius = Avatar.AvatarDescription.Height * Avatar.Scale * 0.25f;

               return enemyBoundingSphere;
          }

          public BoundingSphere GetBoundingSphereForHead()
          {
               BoundingSphere enemyBoundingSphere = new BoundingSphere();

               enemyBoundingSphere.Center =
                             new Vector3(Avatar.Position.X, Avatar.AvatarDescription.Height * Avatar.Scale * (0.6f), Avatar.Position.Z);

               // The radius is 1/6th the body.
               enemyBoundingSphere.Radius = Avatar.AvatarDescription.Height * Avatar.Scale * (1f / 5f);

               return enemyBoundingSphere;
          }


          protected int DamageDoneToPlayer = 1;

          private float health = 1;

          /// <summary>
          /// The Enemy's Health.
          /// </summary>
          public float Health
          {
               get { return health; }
               set { health = value; }
          }



          public bool IsCloseCallKill = false;

          #region Fields

          public Wave.SpawnLocation SpawnLocation = Wave.SpawnLocation.Empty;
          //public Wave.ColorSpawnLocation ColorSpawnLocation = Wave.ColorSpawnLocation.Green;

          // For creating random word lists for the enemies.
          protected Random random = new Random();

          // Stores the Enemy's appearance, as an Avatar.
          private Avatar avatar;

          // Sounds (asset name).

          /// <summary>
          /// The sound to play when an Enemy is hit
          /// 
          /// This is the sound of the Enemy getting hit (contact),
          /// NOT his reaction to it.
          /// </summary>
          private string collisionSound;

          /// <summary>
          /// The sound to play when an Enemy is hurt.
          /// 
          /// This occurs when an Enemy has been Hit.
          /// </summary>
          private string hurtSound;

          /// <summary>
          /// The sound to play when an Enemy is missed.
          /// </summary>
          private string missedSound;

          /// <summary>
          /// The sound to play when an Enemy dies.
          /// </summary>
          private string deathSound;

          /// <summary>
          /// The sound to play when an Enemy escapes.
          /// </summary>
          private string escapeSound;


          protected float startingHealth;

          // Booleans to track Enemy status.
          private bool isAlive;
          private bool isDying;
          private bool isSpeedKill;
          private bool wasKilledByPlayer;

          public Color Color = Color.DarkOrange;

          private float zVelocity;

          private Vector3 position;
          private Matrix worldPosition;


          private int basePoints;
          private int bonusPoints;
          private float speedBonusRequirement;

          protected float elapsedDyingTime;
          private float elapsedKillTime;

          protected GameResourceTexture2D gradientTexture;
          protected GameResourceTexture2D blankTexture;


          TextObject zombieEscapedTextObject = new TextObject("Zombie Escaped!",
               new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 50f), FontType.TitleFont, Color.IndianRed, 0.0f, Vector2.Zero, 0.5f, true);

          TextObject closeCallTextObject = new TextObject("Close Call!",
               new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 100f), FontType.TitleFont, Color.DarkOrange, 0.0f, Vector2.Zero, 0.5f, true);


          #endregion


          #region ICameraTrackable Fields

          public Vector3 TrackedPosition
          {
               get { return this.Position; }
               set { this.Position = value; }
          }

          public Matrix TrackedWorldMatrix
          {
               get { return this.WorldPosition; }
          }

          #endregion


          #region Properties

          public Wave Wave
          {
               get { return wave; }
               set { wave = value; }
          }
          private Wave wave;


          /// <summary>
          /// A 3D Model representing the physical appearance of the Typing Enemy.
          /// </summary>
          public Avatar Avatar
          {
               get { return avatar; }
               set { avatar = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy is shot.
          /// </summary>
          public string CollisionSound
          {
               get { return collisionSound; }
               set { collisionSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent the sound played
          /// when an Enemy has been hurt.
          /// 
          /// This is his "I'm hurt" sound!
          /// </summary>
          public string HurtSound
          {
               get { return hurtSound; }
               set { hurtSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy is missed.
          /// </summary>
          public string MissedSound
          {
               get { return missedSound; }
               set { missedSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy dies.
          /// </summary>
          public string DeathSound
          {
               get { return deathSound; }
               set { deathSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy escapes.
          /// </summary>
          public string EscapeSound
          {
               get { return escapeSound; }
               set { escapeSound = value; }
          }

          /// <summary>
          /// Returns true if the Enemy is alive, false otherwise.
          /// </summary>
          public bool IsAlive
          {
               get { return isAlive; }
               set { isAlive = value; }
          }

          /// <summary>
          /// Returns true if the Enemy is not alive, false otherwise.
          /// </summary>
          public bool IsDead
          {
               get { return !isAlive; }
               set { isAlive = !value; }
          }

          /// <summary>
          /// Returns true if the Enemy is dying (fainting to the ground), false otherwise.
          /// </summary>
          public bool IsDying
          {
               get { return isDying; }
               set { isDying = value; }
          }

          public bool IsEscaping = false;


          /// <summary>
          /// Returns true if the player has a Speed Kill against the Enemy.
          /// Affects the player's score and multipliers.
          /// </summary>
          public bool IsSpeedKill
          {
               get { return isSpeedKill; }
               set { isSpeedKill = value; }
          }

          /// <summary>
          /// Returns true if the enemy was Killed by the Player (and not other means),
          /// false otherwise.
          /// </summary>
          public bool WasKilledByPlayer
          {
               get { return wasKilledByPlayer; }
               set { wasKilledByPlayer = value; }
          }

          /// <summary>
          /// The Velocity the enemy is traveling at.
          /// </summary>
          public float Speed
          {
               get { return zVelocity; }
               set { zVelocity = value; }
          }

          /// <summary>
          /// Position in world space of the bottom center of this enemy.
          /// </summary>
          public Vector3 Position
          {
               get { return position; }
               set { position = value; }
          }

          /// <summary>
          /// Position in world space of the bottom center of this enemy.
          /// </summary>
          public Matrix WorldPosition
          {
               get { return worldPosition; }
               set { worldPosition = value; }
          }

          /// <summary>
          /// The amount of base points awarded to the player when defeated.
          /// </summary>
          public int BasePoints
          {
               get { return basePoints; }
               set { basePoints = value; }
          }

          /// <summary>
          /// The amount of extra points awarded to the player when the Enemy
          /// is defeated with a Speed or Perfect Bonus.
          /// </summary>
          public int BonusPoints
          {
               get { return bonusPoints; }
               set { bonusPoints = value; }
          }


          public int MoneyDropped
          {
               get { return moneyDropped; }
               set { moneyDropped = value; }
          }
          private int moneyDropped;

          /// <summary>
          /// The requirement needed for the player to earn a Speed Bonus.
          /// </summary>
          public float SpeedBonusRequirement
          {
               get { return speedBonusRequirement; }
               set { speedBonusRequirement = value; }
          }


          /// <summary>
          /// The elapsed time since the Enemy was first Defeated.
          /// </summary>
          public float ElapsedDyingTime
          {
               get { return elapsedDyingTime; }
               set { elapsedDyingTime = value; }
          }

          /// <summary>
          /// The elapsed time since the Enemy...Uh, not sure!
          /// </summary>
          public float ElapsedKillTime
          {
               get { return elapsedKillTime; }
               set { elapsedKillTime = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Creates an Enemy.
          /// </summary>
          /// <param name="game">The current Game instance.</param>
          /// <param name="enemyManager">The Wave this Enemy belongs to.</param>
          public Enemy(Game game, Wave wave)
               : base(game)
          {
               zombieEscapedTextObject.IsCenter = true;
               zombieEscapedTextObject.TextEffect = new ZoomInEffect(1.0f, 0.50f, 1500.0f, "Zombie Escaped!");

               closeCallTextObject.IsCenter = true;
               closeCallTextObject.TextEffect = new ZoomInEffect(1.0f, 0.50f, 1500.0f, "Close Call!");

               this.Position = new Vector3(0);
               this.Speed = 0.0f;
               this.IsAlive = true;

               this.SpeedBonusRequirement = 8.0f;
               this.CollisionSound = "Zombie_Hit";
               this.HurtSound = "Zombie_Hurt";
               this.MissedSound = "Mistype";
               this.DeathSound = "Zombie_Death";
               this.EscapeSound = "Zombie_Laughs_Male";
               this.Wave = wave;

               // Create the Enemy's Avatar.
               this.Avatar = new Avatar(game);
               this.Avatar.Scale = 1.75f;
               //this.Avatar.AmbientLightColor = this.Color.ToVector3();

               // NEW AS OF 3-7-2011: JUST TESTING
               this.Avatar.LoadRandomAvatar();

               this.startingHealth = Health;

               this.MoneyDropped = 100;
          }

          /// <summary>
          /// Initializes the enemy manager component.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();
          }

          /// <summary>
          /// Loads resources shared by all Enemies.
          /// 
          /// In our case, it's simply their Health Meter texture.
          /// </summary>
          protected override void LoadContent()
          {
               gradientTexture = ResourceManager.LoadTexture(@"Textures\TextBubble_3D");//Blank Textures\Blank");//TextBubble_3D");
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
          }

          /// <summary>
          /// Called when graphics resources need to be unloaded. Override this method
          /// to unload any component-specific graphics resources.
          /// </summary>
          protected override void UnloadContent()
          {
               //this.EnemyManager.Dispose();
               //this.EnemyManager = null;

               if (this.Avatar != null)
               {
                    // Remove reference to the Avatar Animation.
                    this.Avatar.AvatarAnimation = null;

                    // Dispose Avatar resources if it's not null.
                    this.Avatar.Dispose();

                    // Set the Avatar to null to free up memory.
                    this.Avatar = null;
               }

               //this.Dispose(true);
          }

          /// <summary>
          /// Call this manually to unload TypingEnemy resources.
          /// </summary>
          public void Unload()
          {
               // Simply call the overridden UnloadContent.
               UnloadContent();
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the Enemy.
          /// 
          /// Checks if the Enemy is dying, sets the Enemy's Avatar 
          /// World Matrix appropriately, elapses a timer variable if the Enemy
          /// is currently targetted, and finally updates the Enemy's Avatar.
          /// </summary>
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

               // Update the Enemy's Position (and thus Avatar Position).
               this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;

               this.WorldPosition = this.Avatar.WorldMatrix;


               // Increment how long we've been fighting / targetting this Enemy!
               this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


               // New: In testing: For speed-based animation playback.
               TimeSpan duration = gameTime.ElapsedGameTime;
               duration = TimeSpan.FromTicks((long)(duration.Ticks * (Math.Abs(this.Speed) / 0.75f)));         // / 1 is good for everything except ZombieWalk
               GameTime test = new GameTime(gameTime.TotalGameTime, duration);
               Avatar.Update(test);
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Typing Enemy; the Avatar itself, along with the word.
          /// </summary>
          public virtual void DrawWithoutCamera(GameTime gameTime, SpriteFont font, Vector3 fakeCameraPosition, Vector3 renderPosition)
          {
               //if (Avatar == null)
               //     return;

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Draw the enemy (his / her avatar).
               Avatar.DrawToScreen(gameTime, fakeCameraPosition, renderPosition);

               MySpriteBatch.End();
          }

          /// <summary>
          /// Draws the Typing Enemy; the Avatar itself, 
          /// along with the Enemy's sentence.
          /// </summary>
          public virtual new void Draw(GameTime gameTime)
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
               if (!this.IsDying && !this.IsEscaping)
               {
                    if (!StartScreen.IsOnStartScreen)
                         DrawHealthMeter(gameTime);
               }

               if (IsEscaping)
               {
                    if (!StartScreen.IsOnStartScreen)
                         DrawEscapeText(gameTime);
               }

               if (IsCloseCallKill)
               {
                    if (!StartScreen.IsOnStartScreen)
                         DrawCloseCallText(gameTime);
               }

               MySpriteBatch.End();
          }

          public void DrawEscapeText(GameTime gameTime)
          {
               zombieEscapedTextObject.Update(gameTime);
               zombieEscapedTextObject.Draw(gameTime);
          }

          public void DrawCloseCallText(GameTime gameTime)
          {
               closeCallTextObject.Update(gameTime);
               closeCallTextObject.Draw(gameTime);
          }

          bool didMoneySoundOnce = false;

          public void DrawMoneyDropped(GameTime gameTime)
          {
               Vector3 screenSpace = PixelEngine.Graphics.GraphicsHelper.ConvertToScreenspaceVector3(this.WorldPosition);

               if (this.Position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.TitleFont].SpriteFont, "+ $" + (this.MoneyDropped * this.Wave.CurrentPlayer.CashPerk).ToString(), new Vector2(screenSpace.X, screenSpace.Y), Color.LawnGreen, 0.20f * 0.75f);

                    TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.TitleFont].SpriteFont, "+ " + this.BasePoints.ToString(),// + " Points", 
                         new Vector2(screenSpace.X, screenSpace.Y + 20f), Color.Gold, 0.20f * 0.75f);
               }

               if (!didMoneySoundOnce)
               {
                    AudioManager.PlayCue("MoneySpent");

                    didMoneySoundOnce = true;
               }
          }


          public virtual void DrawHealthMeter(GameTime gameTime)
          {
               // Use this to render it above their head.
               //Matrix renderPosition = Matrix.CreateTranslation(new Vector3(this.position.X, this.position.Y + this.Avatar.AvatarDescription.Height * this.Avatar.Scale * 1.20f, this.position.Z));

               // This will render it around their ankles.
               Matrix renderPosition =
                    Matrix.CreateTranslation(new Vector3(this.position.X, this.position.Y + (this.Avatar.AvatarDescription.Height * this.Avatar.Scale / 4f), this.position.Z));

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

                    MySpriteBatch.DrawCentered(blankTexture.Texture2D, new Rectangle((int)screenSpace.X, (int)screenSpace.Y, 4 + (int)(this.Health * 15), 4 + 5), Color.Black, 0.1f);
                    MySpriteBatch.DrawCentered(gradientTexture.Texture2D, new Rectangle((int)screenSpace.X, (int)screenSpace.Y, (int)(this.Health * 15), 5), healthColor * 1f, 0.1f);
               }
          }

          #endregion


          #region Collision Methods

          /// <summary>
          /// Event triggered if a collision is found.
          /// Returns true if the Enemy escaped from the player.
          /// </summary>
          /// <returns></returns>
          public virtual bool IsCollision(Vector3 playerPosition)
          {
               if (this.Position.Z < (playerPosition.Z) && this.IsAlive)  // used to be position.Z - 1
               {
                    return true;
               }

               return false;
          }

          /// <summary>
          /// Event triggered during a collision.
          /// Causes the colliding enemy to damage the player.
          /// </summary>
          /// <param name="thePlayer"></param>
          public virtual void OnCollide(Player thePlayer)
          {
               // Don't do anything if Player is null.
               if (thePlayer == null)
                    return;

               // Otherwise, subtract 1 from the Player's Health.
               thePlayer.Health -= this.DamageDoneToPlayer;

               // If the Player has no Health, he's dead!
               if (thePlayer.Health <= 0)
               {
                    this.IsAlive = false;
               }
          }

          #endregion


          #region On Hit, Miss, Killed and Target Methods

          /// <summary>
          /// Called whenever an Enemy is hit.
          /// 
          /// Does all logic corresponding to being hit, such as
          /// getting rid of the shot character, flinging it off screen, 
          /// playing the Shot Sound effect, etc.
          /// </summary>
          public virtual void OnHit(Player playerWhoHit)
          {
               this.Wave.NumberOfHits++;

               // Sound of Bullet connecting to Enemy.
               AudioManager.PlayCue(this.CollisionSound);

               this.Health -= playerWhoHit.Gun.BulletDamage;

               // If the Enemy is point blank...
               if (Math.Abs(this.Position.Z - playerWhoHit.Position.Z) < 3f)
               {
                    // Deal the damage again: This means 2x damage!
                    this.Health -= playerWhoHit.Gun.BulletDamage;

                    // Or let's do 1-Hit KO for now...
                    this.Health = 0;

                    IsCloseCallKill = true;

                    this.Wave.NumberOfCloseCalls++;

                    // Add +1 to the player's Total Kills!
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

          /// <summary>
          /// Called whenever an Enemy is Killed.
          /// 
          /// Does all logic corresponding to an Enemy being Killed,
          /// such as incrementing the player's Total Kills, and Total Speedy
          /// / Perfect Kills (if earned). Sets the Enemy as IsAlive = false,
          /// IsTarget = false, IsDying = true, and IsActive = false.
          /// 
          /// Finally, removes the Enemy's spawn point from the spawn list, 
          /// plays the Enemy's Death Sound effect, and plays the Faint animation.
          /// </summary>
          public virtual void OnKilled()
          {
               // Calculate distance from Player: Just assume Player is at Z = 0.
               float distanceFromPlayer =
                    MathHelper.Clamp((int)Math.Abs(this.Position.Z - 0), 1f, 35f);

               // Award the Player Bonus Points based on how far the Enemy was from him when killed.
               this.BonusPoints = (int)(this.BasePoints * (distanceFromPlayer / 35f));

               this.BasePoints += this.BonusPoints;

               // Scale the points based on Difficulty.
               // For Normal, we get x1 points.
               // For Hard, we get x1.5 points.
               this.BasePoints = (int)(this.BasePoints * (1f + ((int)(ZombieGameSettings.Difficulty) * 0.5f)));


               // Add +1 to the player's Total Kills!
               AvatarZombieGame.AwardData.TotalKills++;


               // The enemy is no longer alive, and thus is not the target.
               this.IsAlive = false;
               this.IsDying = true;

               this.Wave.UsedSpawns.Remove(SpawnLocation);

               AudioManager.PlayCue(this.DeathSound);

               // Make the enemy's avatar begin the Faint animation.
               this.Faint();
          }

          /// <summary>
          /// Called whenever an Enemy Escapes.
          /// 
          /// Does all logic corresponding to Escaping, 
          /// such as setting IsActive = false and removing
          /// the Enemy's spawn position from the spawn list.
          /// </summary>
          public virtual void OnEscaped()
          {
               if (this.Avatar.AvatarDescription.BodyType == AvatarBodyType.Male)
               {
                    AudioManager.PlayCue(this.EscapeSound);
               }

               else
               {
                    AudioManager.PlayCue("Zombie_Laughs_Female");
               }

               this.Wave.UsedSpawns.Remove(SpawnLocation);

               if (!StartScreen.IsOnStartScreen)
               {
                    // Set the Controller Vibration.
                    InputState.SetVibration(EngineCore.ControllingPlayer, 1.0f, 1.0f, 0.50f);

                    AudioManager.PlayCue("ZombieEscapedTune");
               }
          }


          #endregion


          #region OnGetExploded Method

          public virtual void OnExplode(Vector2 explosionOrigin)
          {
               // BURN THEM TO A CRISP!!!
               this.Avatar.LightColor = new Vector3(0, 0, 0);
               this.Avatar.LightDirection = new Vector3(0, 0, 0);
               this.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);
          }

          #endregion


          #region Animation Actions

          /// <summary>
          /// Plays the custom Run animation for the Enemy's Avatar.
          /// </summary>
          public virtual void Run()
          {
               Avatar.PlayAnimation(AnimationType.Run, true);
          }

          /// <summary>
          /// Plays the Walk animation for the Enemy's Avatar.
          /// 
          /// Also reduced the Enemy's Speed by half.
          /// </summary>
          protected void Walk()
          {
               this.Avatar.PlayAnimation(AnimationType.Walk, true);
               //this.Speed = this.Speed / 2.0f;
          }

          /// <summary>
          /// Plays the custom Faint animation for the Enemy's Avatar.
          /// </summary>
          protected void Faint()
          {
               Avatar.PlayAnimation(AnimationType.Faint, false);
          }

          #endregion
     }
}
