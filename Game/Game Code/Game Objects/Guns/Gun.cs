#region File Description
//-----------------------------------------------------------------------------
// Gun.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Avatars;
#endregion

namespace AvatarsInGraveDanger
{
     #region Gun  Class

     /// <remarks>
     /// Defines a Gun object.
     /// 
     /// A Gun has a List of Bullets it must maintain, as well as its
     /// own properties to describe its physical appearance and behavior.
     /// </remarks>
     public class Gun : DrawableGameComponent
     {
          /// <summary>
          /// Enum for different fire modes.
          /// </summary>
          private enum FireMode
          {
               Single,
               Burst,
          };

          /// <summary>
          /// The Gun's current FireMode type. This includes modes such as Single Fire, Burst Fire, etc.
          /// </summary>
          FireMode fireMode = FireMode.Single;


          private int damageLevel = 0;
          private int rateOfFireLevel = 0;
          private int bulletSpeedLevel = 0;
          private int clipSizeLevel = 0;
          private int reloadSpeedLevel = 0;
          private int reloadAreaLevel = 0;

          public static int MinUpgradeLevel = 0;
          public static int MaxUpgradeLevel = 5;



          /// <summary>
          /// The Level of the Player's Strafe Speed attribute.
          /// We clamp this between 1 and MaxUpgradeLevel.
          /// </summary>
          public int DamageLevel
          {
               get
               {
                    return (int)MathHelper.Clamp(damageLevel, MinUpgradeLevel, MaxUpgradeLevel);
               }

               set
               {
                    damageLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
               }
          }



          /// <summary>
          /// The Level of the Player's Strafe Speed attribute.
          /// We clamp this between 1 and MaxUpgradeLevel.
          /// </summary>
          public int RateOfFireLevel
          {
               get
               {
                    return (int)MathHelper.Clamp(rateOfFireLevel, MinUpgradeLevel, MaxUpgradeLevel);
               }

               set
               {
                    rateOfFireLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
               }
          }

          /// <summary>
          /// The Level of the Player's Bullet Speed attribute.
          /// We clamp this between 1 and MaxUpgradeLevel.
          /// </summary>
          public int BulletSpeedLevel
          {
               get
               {
                    return (int)MathHelper.Clamp(bulletSpeedLevel, MinUpgradeLevel, MaxUpgradeLevel);
               }

               set
               {
                    bulletSpeedLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
               }
          }

          /// <summary>
          /// The Level of the Player's Clip Size attribute.
          /// We clamp this between 1 and MaxUpgradeLevel.
          /// </summary>
          public int ClipSizeLevel
          {
               get
               {
                    return (int)MathHelper.Clamp(clipSizeLevel, MinUpgradeLevel, MaxUpgradeLevel);
               }

               set
               {
                    clipSizeLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
               }
          }

          /// <summary>
          /// The Level of the Player's Reload Speed attribute.
          /// We clamp this between 1 and MaxUpgradeLevel.
          /// </summary>
          public int ReloadSpeedLevel
          {
               get
               {
                    return (int)MathHelper.Clamp(reloadSpeedLevel, MinUpgradeLevel, MaxUpgradeLevel);
               }

               set
               {
                    reloadSpeedLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
               }
          }

          /// <summary>
          /// The Level of the Player's Reload Area attribute.
          /// We clamp this between 1 and MaxUpgradeLevel.
          /// </summary>
          public int ReloadAreaLevel
          {
               get
               {
                    return (int)MathHelper.Clamp(reloadAreaLevel, MinUpgradeLevel, MaxUpgradeLevel);
               }

               set
               {
                    reloadAreaLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
               }
          }


          #region Fields

          /// <summary>
          /// For randomization.
          /// </summary>
          private Random random = new Random();

          /// <summary>
          /// The Player that owns (uses) this Gun.
          /// </summary>
          protected Player gunOwner;

          /// <summary>
          /// The Bullet this Gun uses.
          /// </summary>
          public Bullet bulletToUse;

          /// <summary>
          /// The Damage the Bullet deals.
          /// </summary>
          public float BulletDamage = 1.0f;

          /// <summary>
          /// The Speed of the Bullet.
          /// </summary>
          public float BulletSpeed;

          /// <summary>
          /// String name for the Gun Fire cue.
          /// </summary>
          protected string gunFiredSoundName = "Pistol_Fire_1";

          /// <summary>
          /// String name for the Gun Fired (but empty) cue.
          /// </summary>
          protected string gunEmptyFiredSoundName = "Pistol_EmptyFire";

          /// <summary>
          /// String name for the Reloading cue.
          /// </summary>
          protected string gunReloadingSoundName = "Reload_1";

          /// <summary>
          /// String name for the Active Reload Failure cue.
          /// </summary>
          private string activeReloadFailureSoundName = "Mistype";

          /// <summary>
          /// The Rotation of the Gun.
          /// </summary>
          protected float gunRotationUh = 0.0f;

          /// <summary>
          /// A List to store Bullets fired by the Gun.
          /// </summary>
          private List<Bullet> bulletList = new List<Bullet>();

          /// <summary>
          /// The model to represent the Gun.
          /// </summary>
          protected Model3D gunModel;

          /// <summary>
          /// The texture to represent the Gun's crosshair.
          /// </summary>
          private GameResourceTexture2D crosshairTexture;

          /// <summary>
          /// The number of shots fired off by this Gun.
          /// </summary>
          private uint shotsFired = 0;

          /// <summary>
          /// How long (seconds) between firing a Bullet.
          /// 
          /// 0 means we can fire as fast as we want (probably limited 
          /// by how fast the Player presses the shoot button).
          /// </summary>
          //private float firingGracePeriod = 0.4f;

          public float RateOfFire
          {
               get { return rateOfFire; }
               set { rateOfFire = value; }
          }
          /// <summary>
          /// How long (seconds) between firing a Bullet.
          /// </summary>
          private float rateOfFire = 6f;//2f;


          /// <summary>
          /// Helper variable which handles how quickly a Gun can be fired. It uses the Rate of Fire property to figure this out.
          /// </summary>
          private float FiringGracePeriod
          {
               get { return (1f / rateOfFire); }
          }

          /// <summary>
          /// How long (seconds) has elapsed since the last
          /// time we fired the Gun.
          /// 
          /// We initialize this to the value of the grace period,
          /// that way the Player can fire his 1st shot immediately.
          /// </summary>
          private float elapsedTimeSinceFiring = 0f;

          /// <summary>
          /// Whether or not to force a reload on the player.
          /// </summary>
          private bool autoReload = true;

          /// <summary>
          /// How many total bullets the player has remaining.
          /// </summary>
          private int totalAmmo = 50;

          /// <summary>
          /// The maximum amount of ammo this Gun can carry.
          /// </summary>
          private int maxAmmoCapacity = 300;

          /// <summary>
          /// How many  bullets a single clip holds.
          /// </summary>
          private int clipSize = 32;//5;

          /// <summary>
          /// How many bullets left in current clip.
          /// </summary>
          private int ammoRemainingInClip = 32;//5;

          /// <summary>
          /// Whether or not the player is currently reloading.
          /// </summary>
          public bool isReloading = false;

          /// <summary>
          /// How fast the player can reload.
          /// </summary>
          private float reloadTime = 3000.0f;

          /// <summary>
          /// How long (seconds) it takes to reload upon a failed Active Reload.
          /// </summary>
          public float ReloadTime
          {
               get { return reloadTime; }
               set { reloadTime = value; }
          }

          /// <summary>
          /// How long the player has been reloading.
          /// </summary>
          private float reloadTimeElapsed = 0.0f;

          /// <summary>
          /// How large the area for a successful active reload is.
          /// Values range from 0 (none) to 100 (entire area is a success).
          /// </summary>
          private float activeReloadArea = 15.0f;

          public float ActiveReloadArea
          {
               get { return activeReloadArea; }
               set { activeReloadArea = value; }
          }

          /// <summary>
          /// Where the Active Reload sliding pin has been stopped at.
          /// 
          /// If stopped within the Active Reload Area, a successful Active Reload is made.
          /// </summary>
          private float activeReloadStop = 0.0f;

          /// <summary>
          /// Keeps track of the hidden timing mechanics.
          /// 
          /// Basically a helper variable to Active Reload Stop variable.
          /// </summary>
          private float timing = 0.0f;

          /// <summary>
          /// Used to keep track of Active Reload timing mechanics.
          /// </summary>
          private GameTime activeReloadGameTime = new GameTime();

          /// <summary>
          /// Whether or not a successful Active Reload was made.
          /// </summary>
          bool isActiveReloadSuccessful = false;

          /// <summary>
          /// Whether or not an Active Reload is being attempted.
          /// </summary>
          bool isActiveReloadAttempted = false;

          /// <summary>
          /// Whether or not this Gun has Infinite Ammo.
          /// </summary>
          public bool isInfiniteAmmo = true;

          /// <summary>
          /// Whether or not this Gun has an Infinite Ammo Clip.
          /// </summary>
          private bool isInfiniteClip = false;


          /// <summary>
          /// Whether or not this Gun is Fully Automatic.
          /// A Fully Automatic Gun is one which does not require a new 
          /// fire-button press to shoot a new bullet - the fire button can simply be held down.
          /// </summary>
          private bool isfullyAutomatic = false;

          /// <summary>
          /// Whether or not this Gun is Fully Automatic.
          /// 
          /// In this context, a Fully Automatic gun will continuously fire
          /// with the Fire Button held down, rather than needing
          /// to re-press it for each shot.
          /// </summary>
          public bool IsFullyAutomatic
          {
               get { return isfullyAutomatic; }
               set { isfullyAutomatic = value; }
          }

          public float AngleOfAimer = 0f;


          // Helper Variables

          private GameResourceTexture2D blankTexture;
          private GameResourceTexture2D blankTextureBlank;

          /// <summary>
          /// The RenderTarget2D we will draw our Gun HUD to.
          /// </summary>
          public RenderTarget2D ReloadRenderTarget = new RenderTarget2D(ScreenManager.Game.GraphicsDevice, 205, 36);







          /// <summary>
          /// The offset (from our Player's Position) of the Gun's Tip.
          /// </summary>
          public Vector3 GunTipPositionOffset
          {
               get
               {
                    return gunTipPositionOffset;
               }

               protected set
               {
                    gunTipPositionOffset = value; 
               }
          }
          private Vector3 gunTipPositionOffset;

          /// <summary>
          /// Gets the position of the Gun's Tip in World Space.
          /// 
          /// This is useful for firing Bullets at the correct location,
          /// attaching the laser-sight to the correct location, and
          /// for accurate collision detection.
          /// </summary>
          public Vector3 GunTipPosition
          {
               get
               {
                    return gunOwner.Position + gunTipPositionOffset;
               }
          }


          #endregion


          #region Properties

          /// <summary>
          /// List of Bullets fired by this Gun.
          /// </summary>
          public List<Bullet> BulletList
          {
               get { return bulletList; }
               set { bulletList = value; }
          }

          /// <summary>
          /// The total ammo count for this Gun.
          /// </summary>
          public int TotalAmmo
          {
               get 
               { 
                    return totalAmmo; 
               }

               set 
               {
                    totalAmmo = (int)MathHelper.Clamp(value, 0f, maxAmmoCapacity); 
               }
          }

          /// <summary>
          /// The maximum amount of Ammo this Gun can carry.
          /// </summary>
          public int MaximumAmmo
          {
               get { return maxAmmoCapacity; }
               set { maxAmmoCapacity = value; }
          }

          /// <summary>
          /// The size of the Gun's clip.
          /// </summary>
          public int ClipSize
          {
               get { return clipSize; }
               set { clipSize = value; }
          }

          /// <summary>
          /// The number of bullets remaining in the Gun's clip.
          /// </summary>
          public int AmmoRemainingInClip
          {
               get { return ammoRemainingInClip; }
               set { ammoRemainingInClip = value; }
          }

          /// <summary>
          /// Whether or not the Gun is currently reloading.
          /// </summary>
          public bool IsReloading
          {
               get { return isReloading; }
          }

          /// <summary>
          /// Gets or sets the number of shots fired 
          /// off by this Gun.
          /// </summary>
          public uint ShotsFired
          {
               get { return shotsFired; }
               set { shotsFired = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Instanciates a TypingEnemy.
          /// </summary>
          /// <param name="game">The current Game instance.</param>
          /// <param name="enemyManager">The EnemyManager to handle this TypingEnemy.</param>
          public Gun(Game game, Player owner)
               : base(game)
          {
               bulletToUse = new Bullet(game);

               gunOwner = owner;


               
               GunOffsetPosition = new Vector3(0f, 0.125f - (0.125f / 2.0f), 0.04f);
               GunRotation = new Quaternion(90f, 0f, 5f, 0f);
               GunScale = 0.0035f * 0.90f;

               // Why the fuck is this not being called automatically?
               this.LoadContent();
          }

          /// <summary>
          /// Initializes the enemy manager component.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();
          }

          /// <summary>
          /// Loads a particular enemy sprite sheet and sounds.
          /// </summary>
          protected override void LoadContent()
          {
               gunModel = new Model3D();
               gunModel.Model = EngineCore.Content.Load<Model>(@"Models\Weapons\Uzi");//LaserGun");

               blankTexture = ResourceManager.LoadTexture(@"Textures\TextBubble_3D");
               blankTextureBlank = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");

               crosshairTexture = ResourceManager.LoadTexture(@"Textures\HUD\PerfectIcon");
          }

          /// <summary>
          /// Called when graphics resources need to be unloaded. Override this method
          /// to unload any component-specific graphics resources.
          /// </summary>
          protected override void UnloadContent()
          {
               // Call the base UnloadContent method.
               base.UnloadContent();
          }

          /// <summary>
          /// Call this manually to unload Gun resources.
          /// </summary>
          public void Unload()
          {
               // Simply call the overridden UnloadContent.
               UnloadContent();
          }

          #endregion


          #region Handle Input

          /// <summary>
          /// Handles Player input for Gun-related behavior, such as Reloading, Firing, etc.
          /// </summary>
          public void HandleInput(InputState input, GameTime gameTime, Player playerRef)
          {
               PlayerIndex playerIndex;

               // Check for firing a Bullet.

               if (!isfullyAutomatic)
               {
                    if (input.IsNewButtonPress(Buttons.RightTrigger, EngineCore.ControllingPlayer, out playerIndex))
                    {
                         Fire(playerRef);
                    }
               }

               else
               {
                    if (input.IsButtonDown(Buttons.RightTrigger, EngineCore.ControllingPlayer, out playerIndex))
                    {
                         Fire(playerRef);
                    }
               }

               // Check for reloading the Gun.
               if (input.IsNewButtonPress(Buttons.RightShoulder, EngineCore.ControllingPlayer, out playerIndex))
               {
                    if (!this.isReloading)
                    {
                         // NEW: Don't allow reload if clip is full, or ammo reserves are empty!
                         if (this.ammoRemainingInClip != this.clipSize && this.totalAmmo != 0)
                         {
                              // Set the reloading flag to True.
                              this.isReloading = true;

                              // Reset the active reload timer.
                              timing = 0.0f;

                              // Play the Reload sound effect.
                              AudioManager.PlayCue(gunReloadingSoundName);






                              playerRef.reloadAnimation.CurrentPosition = TimeSpan.FromSeconds(0.001);




                              //playerRef.reloadAnimation.Update(TimeSpan.FromSeconds(0.1));


                              //playerRef.Avatar.PlayAnimation(playerRef.reloadAnimation, false);


                              //playerRef.Avatar.AvatarAnimation.CurrentPosition = TimeSpan.FromSeconds(0);
                              //playerRef.currentType = Player.PlayerAnimationType.Reload;
                              //playerRef.isReloading = true;
                              // Play the Throwing animation, once.
                              //playerRef.Avatar.PlayAnimation(playerRef.reloadAndStrafe, false);//AnimationType.Reload, false);
                              //playerRef.Avatar.PlayMultipleAnimation(false, playerRef.Avatar.AvatarRenderer, playerRef.reloadAndStrafe.Animations, playerRef.reloadAndStrafe.TempListOfBoneList);
                         }
                    }

                    // Else we are already reloading; check for active reload input!
                    else
                    {
                         // ...But if we've already attempted an Active once, we can't again.
                         if (isActiveReloadAttempted)
                         {
                              return;
                         }

                         isActiveReloadAttempted = true;

                         // If our reload stopped within our success area...
                         if (Math.Abs(activeReloadStop - 50.0f) < ((10 / 2) + (activeReloadArea / 2f))) 
                              //if (Math.Abs(activeReloadStop - 50.0f) < (activeReloadArea / 2f))
                         {
                              // We performed a successful active reload!
                              this.isActiveReloadSuccessful = true;
                         }

                         // But if not...
                         else
                         {
                              // We did not perform a successful active reload!
                              this.isActiveReloadSuccessful = false;

                              // Play active reload fail cue.
                              AudioManager.PlayCue(activeReloadFailureSoundName);
                         }
                    }
               }


               /*
               // DEBUG INPUT
               if (input.IsNewButtonPress(Buttons.X, null, out playerIndex))
               {
                    isfullyAutomatic = !isfullyAutomatic;
               }

               if (input.IsNewButtonPress(Buttons.LeftShoulder, null, out playerIndex))
               {
                    if (fireMode == FireMode.Burst)
                    {
                         fireMode = FireMode.Single;
                    }

                    else
                         fireMode = FireMode.Burst;
               }

               if (input.IsNewButtonPress(Buttons.Y, null, out playerIndex))
               {
                    bulletTypeIndex = (bulletTypeIndex + 1) % 3;

                    switch (bulletTypeIndex)
                    {
                         // standard
                         case 0:
                              bulletToUse.bulletType = Bullet.BulletType.Standard;
                              break;
                         // piercing
                         case 1:
                              bulletToUse.bulletType = Bullet.BulletType.Piercing;
                              break;
                         // explosive
                         case 2:
                              bulletToUse.bulletType = Bullet.BulletType.Explosive;
                              break;
                    }
               }
               */
          }

          int bulletTypeIndex = 0;

          #endregion


          #region Update

          /// <summary>
          /// Updates the Gun.
          /// 
          /// This involves updating the Gun's List of Bullets as well.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Increment the amount of time since firing.
               elapsedTimeSinceFiring += (float)gameTime.ElapsedGameTime.TotalSeconds;

               // Set our active reload game timer to the game time. (Sloppy solution - temp).
               activeReloadGameTime = gameTime;

               // Ensure our List of Bullets is valid.
               if (bulletList != null)
               {
                    // Update each one.
                    foreach (Bullet bullet in bulletList)
                    {
                         bullet.Update(gameTime);
                    }

                    // Check if the Remove Bullet flag is set.
                    foreach (Bullet bullet in bulletList)
                    {
                         if (bullet.RemoveBullet)
                         {
                              // And if so, remove the bullet from the List.
                              bulletList.Remove(bullet);
                              break;
                         }
                    }
               }

               // Check if we're wanting / needing to Reload.
               CheckForReload(gameTime);
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Gun model, along with each Bullet currently active.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               DrawLaserSight(gameTime);

               // Draw the Gun itself.
               DrawGun();

               // Draw the Gun's bullets.
               //DrawBullets(gameTime);
          }

          /// <summary>
          /// Draws the Gun model, along with each Bullet currently active.
          /// </summary>
          public void DrawWithoutLaserSight(GameTime gameTime)
          {
               // Draw the Gun itself.
               DrawGun();

               // Draw the Gun's bullets.
               //DrawBullets(gameTime);
          }

          /// <summary>
          /// Draws the Gun model, along with each Bullet currently active.
          /// </summary>
          public void DrawWithoutGun(GameTime gameTime)
          {
               DrawLaserSight(gameTime);

               // Draw the Gun's bullets.
               //DrawBullets(gameTime);
          }

          #endregion



          protected Quaternion GunRotation
          {
               get { return gunRotation; }
               set { gunRotation = value; }
          }
          protected Quaternion gunRotation;

          /// <summary>
          /// The offset of the Gun, in relation to the Player's Position.
          /// </summary>
          protected Vector3 GunOffsetPosition
          {
               get { return gunOffsetPosition; }
               set { gunOffsetPosition = value; }
          }
          protected Vector3 gunOffsetPosition;

          /// <summary>
          /// The scale of the Gun.
          /// This will determine how large it should be rendered.
          /// </summary>
          protected float GunScale
          {
               get { return gunScale; }
               set { gunScale = value; }
          }
          protected float gunScale;

          /// <summary>
          /// The final offset of the Gun, in relation to the Player's Position.
          /// This takes scale, rotation, and positioning into account.
          /// </summary>
          protected Matrix GunOffsetMatrix
          {
               get 
               { 
                    return 
                         Matrix.CreateScale(this.GunScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(this.GunRotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(this.GunRotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(this.GunRotation.Z))
                         * Matrix.CreateTranslation(this.GunOffsetPosition);
               }
          }
          protected Matrix gunOffsetMatrix;










          Ray bulletRay = new Ray();

          /// <summary>
          /// The depth (distance) at which the Laser Sight has collided with an Enemy. This determines how far we render the sight.
          /// </summary>
          public float laserSightDepth = 40f;

          public Ray GunsightRay = new Ray();






          #region Helper Draw Methods

          /// <summary>
          /// Draws the Gun model itself.
          /// </summary>
          protected virtual void DrawGun()
          {
               /*
               float targetRotationZ = -1 * (gunRotation);
               gunOwner.Avatar.Rotation = new Vector3(0f, 180f, 0f);
               gunOwner.Avatar.Rotation = new Vector3(gunOwner.Avatar.Rotation.Z, gunOwner.Avatar.Rotation.Y + targetRotationZ, gunOwner.Avatar.Rotation.Z);
               */

               // Render the Gun model now.
               if (gunOwner.Avatar.BonesInWorldSpace != null)
               {
                    gunOwner.Avatar.UpdateWithoutAnimating();

                    Vector3 propScale = new Vector3();
                    Quaternion propRotation = new Quaternion();
                    Vector3 propTranslation = new Vector3();

                    gunOwner.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Decompose(out propScale, out propRotation, out propTranslation);

                    Matrix withoutRotation = Matrix.CreateScale(propScale) * Matrix.CreateTranslation(propTranslation);

                    /*
                    Matrix matrixWithoutTilting = Matrix.CreateScale(this.GunScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(this.GunRotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(this.GunRotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(this.GunRotation.Z))
                         * Matrix.CreateTranslation(this.GunOffsetPosition + propTranslation);
                    */
                    Matrix matrixWithoutTilting = this.GunOffsetMatrix * Matrix.CreateTranslation(propTranslation);

                    gunModel.DrawModel(matrixWithoutTilting);
               }
          }

          /// <summary>
          /// Draws the Gun's Bullets.
          /// </summary>
          public void DrawBullets(GameTime gameTime)
          {
               // Draw the Gun's fired Bullets as well.
               foreach (Bullet bullet in bulletList)
               {
                    bullet.Draw(gameTime);
               }
          }

          /// <summary>
          /// Draws the Gun's Laser Sight.
          /// </summary>
          public virtual void DrawLaserSight(GameTime gameTime)
          {
               Quaternion rot = new Quaternion();
               rot = new Quaternion(0f, rot.Y, rot.Z, 0f);
               rot = new Quaternion(rot.X, 90f, rot.Z, 0f);
               rot = new Quaternion(rot.X, rot.Y, 0f, 0f);

               // X = - right
               Vector3 gunP = new Vector3(-0.04f, 0f, -0.05f);

               Matrix gunOffset = Matrix.CreateScale(0.0035f * 0.90f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(rot.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(rot.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(rot.Z))
                    * Matrix.CreateTranslation(gunP);

               Vector3 propScale = new Vector3();
               Quaternion propRotation = new Quaternion();
               Vector3 propTranslation = new Vector3();


               if (gunOwner.Avatar.BonesInWorldSpace != null)
               {
                    gunOwner.Avatar.UpdateWithoutAnimating();

                    gunOwner.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Decompose(out propScale, out propRotation, out propTranslation);
               }


               // We figure out the additional height to aim our Bullet based on our angle of aiming.
               float targetHeight = (float)Math.Tan(MathHelper.ToRadians(AngleOfAimer)) * 40.0f;

               // The starting position of our Bullet.
               // This is just the tip of our Gun, estimated.
               Vector3 startingPosition = new Vector3(-0.05f, 0.25f, 0f) + (gunP + propTranslation);

               // New
               GunTipPositionOffset = startingPosition;
               // End

               // The destination of our Bullet.
               // X: Can't aim left/right so this is simply the starting X.
               // Y: You can aim up/down 30 degrees. We find the Y value and add it to the starting Y (how high off ground Avatar's Gun is.)
               // Z: We simplify this and set it to 40 -- the maximum distance a target can possibly be.
               Vector3 destination = new Vector3(startingPosition.X, startingPosition.Y + targetHeight, startingPosition.Z + 40.0f);


               // We create a Ray from the starting position, in the direction of the destination.
               // If an Enemy intersects this ray, he is in danger!
               bulletRay = new Ray(startingPosition, (destination - startingPosition));


               // Render the Ray, to create the Laser-Sight of the Gun.
               RayRenderer.Render(bulletRay, (laserSightDepth / 40f), this.Game.GraphicsDevice, CameraManager.ActiveCamera.ViewMatrix, CameraManager.ActiveCamera.ProjectionMatrix, Color.IndianRed);


               // Find the 3D Y value at our new point.
               float newY = (float)Math.Tan(MathHelper.ToRadians(AngleOfAimer)) * (startingPosition.Z + laserSightDepth);


               Vector3 laserPos3D = new Vector3(destination.X, startingPosition.Y + newY, startingPosition.Z + laserSightDepth);
               Vector2 laserPos2D = GraphicsHelper.ConvertToScreenspaceVector2(Matrix.CreateTranslation(laserPos3D));


               // Create a rectangle based on the crosshair position.

               float crosshairSize = (10f / (laserSightDepth / 40f));

               crosshairSize = MathHelper.Clamp(crosshairSize, 10.0f, 60.0f);

               Rectangle crosshairRect = new Rectangle(
                    (int)laserPos2D.X - (int)(crosshairSize / 2f), (int)laserPos2D.Y - (int)(crosshairSize / 2f),
                    (int)crosshairSize, (int)crosshairSize);

               // Draw the crosshair texture!
               MySpriteBatch.Draw(crosshairTexture.Texture2D, crosshairRect, Color.DarkRed);
          }


          #endregion


          #region Fire Bullet Method

          /// <summary>
          /// Fires the Gun, releasing its bullet, decreasing the ammo count and bullets remaining in the current clip, etc.
          /// </summary>
          public void Fire(Player playerRef)
          {
               int counter = fireMode == FireMode.Single ? 1 : 3;

               for (int i = 0; i < counter; i++)// First off, can we even shoot?!
               {
                    // Scenario 1: We're trying to shoot before our firing grace period is up.
                    if (elapsedTimeSinceFiring < FiringGracePeriod)
                    {
                         return;
                    }

                    // Scenario 2: We're busy reloading or our clip is empty.
                    if (this.isReloading || this.ammoRemainingInClip <= 0)
                    {
                         if (!isfullyAutomatic)
                         {
                              // Play a clicking sound.
                              AudioManager.PlayCue(gunEmptyFiredSoundName);
                         }

                         // Return; nothing else to do if we can't shoot!
                         return;
                    }

                    // Scenario 3: If we reach here, it means we CAN shoot!



                    // Play the fire gun animation, once.
                    if (ArcadeLevel.IsFirstPerson)
                    {
                        if (!playerRef.isMoving)
                        {
                            playerRef.currentType = Player.PlayerAnimationType.Shoot;

                            playerRef.fireAnimation.CurrentPosition = TimeSpan.FromSeconds(0.001);
                            playerRef.Avatar.PlayAnimation(playerRef.fireAnimation, false);
                            
                            //playerRef.Avatar.PlayAnimation(AnimationType.FireGun, false);
                        }
                    }


                    // Increment the count for number of shots fired.
                    shotsFired++;

                    // Reset the elapsed time since last fire.
                    elapsedTimeSinceFiring = 0.0f;

                    // Set the Controller Vibration.
                    InputState.SetVibration(EngineCore.ControllingPlayer.Value, 1.0f, 1.0f, 0.25f);

                    // Play the Gun Fired sound cue.
                    AudioManager.PlayCue(gunFiredSoundName);

                    // Create a new Bullet.
                    Bullet bullet = new Bullet(this.Game);

                    // TEMPORARYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY Set our Bullet properties here.
                    bullet.bulletType = bulletToUse.bulletType;
                    bullet.Velocity = bulletToUse.Velocity;








                    // We figure out the additional height to aim our Bullet based on our angle of aiming.
                    float targetHeight = (float)Math.Tan(MathHelper.ToRadians(AngleOfAimer)) * 40.0f;

                    // The starting position of our Bullet.
                    // This is just the tip of our Gun, estimated.
                    Vector3 startingPosition = new Vector3(gunOwner.Position.X - 0.30f, gunOwner.Position.Y + 1.80f, gunOwner.Position.Z + 0.5f);

                    // The destination of our Bullet.
                    // X: Can't aim left/right so this is simply the starting X.
                    // Y: You can aim up/down 30 degrees. We find the Y value and add it to the starting Y (how high off ground Avatar's Gun is.)
                    // Z: We simplify this and set it to 40 -- the maximum distance a target can possibly be.
                    Vector3 destination = new Vector3(startingPosition.X, startingPosition.Y + targetHeight, startingPosition.Z + 40.0f);


                    // We create a Ray from the starting position, in the direction of the destination.
                    // If an Enemy intersects this ray, he is in danger!
                    bulletRay = new Ray(startingPosition, (destination - startingPosition));


                    bullet.TrueVelocity = destination - startingPosition;


                    // Set the target of the bullet to 40 units in front of the player.
                    bullet.TargetPosition = destination;
                    bullet.Position = startingPosition;

                    // The Bullet offset to ensure it aligns with the tip of the Gun model.
                    Vector3 bulletOffet = new Vector3(-0.05f, 0.25f, 0.5f);

                    // Testing to space for burst firing
                    if (fireMode == FireMode.Burst)
                    {
                         bulletOffet = new Vector3(-0.05f, 0.25f, 0.01f + (1.75f * i));
                    }


                    // Set the position of the Bullet to the Special Right bone in our Avatar (right palm).
                    if (gunOwner.Avatar.BonesInWorldSpace != null)
                         bullet.Position = bulletOffet + gunOwner.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Translation;


                    bulletList.Add(bullet);

                    if (!isInfiniteClip)
                    {
                         this.ammoRemainingInClip--;
                    }

                    // If ammo in clip is depleted...
                    if (this.ammoRemainingInClip <= 0)
                    {
                         // And we have ammo in reserve...
                         if (this.totalAmmo > 0)
                         {
                              // Force an auto-reload?
                              if (this.autoReload)
                              {
                                   // Set the reloading flag to True.
                                   this.isReloading = true;

                                   // Reset the active reload timer.
                                   timing = 0.0f;

                                   // Play the Reload sound effect.
                                   AudioManager.PlayCue(gunReloadingSoundName);
                              }
                         }
                    }
               }
          }

          #endregion
         

          #region Check For Reload and Reset Reload

          /// <summary>
          /// Checks if the Player wants to reload and handles the reloading logic accordingly.
          /// </summary>
          private void CheckForReload(GameTime gameTime)
          {
               if (!isReloading)
                    return;

               // Perform the Active Reload (a sort of mini-game) logic.
               this.ActiveReload(activeReloadGameTime);

               this.reloadTimeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               // If the required reload time has passed, OR we performed an Active Reload successfully, it's time to reload!
               if (this.reloadTimeElapsed >= this.reloadTime || this.isActiveReloadSuccessful)
               {
                    // We are done reloading.
                    this.isReloading = false;

                    // Reset the elapsed reload time.
                    this.reloadTimeElapsed = 0.0f;

                    // Reset the active reload success flag.
                    this.isActiveReloadSuccessful = false;

                    // Rset the active reload attempted flag.
                    this.isActiveReloadAttempted = false;

                    InputState.SetVibration(EngineCore.ControllingPlayer.Value, 1.0f, 1.0f, 0.1f);

                    // FILL THE PLAYER'S CLIP!

                    int ammoNeededToFillClip = this.ClipSize - this.ammoRemainingInClip;

                    // We need more ammo to fill the clip than we actually have.
                    if (ammoNeededToFillClip > this.totalAmmo)
                    {
                         // So just fill it as much as we can.
                         this.ammoRemainingInClip += this.totalAmmo;

                         if (!isInfiniteAmmo)
                         {
                              this.totalAmmo -= this.totalAmmo;
                         }
                    }

                    // Otherwise, we have enough ammo to fill as much of the clip as we currently need.
                    else
                    {
                         this.ammoRemainingInClip += ammoNeededToFillClip;

                         if (!isInfiniteAmmo)
                         {
                              this.totalAmmo -= ammoNeededToFillClip;
                         }
                    }

                    // Ensure we never get a negative amount of ammo.
                    if (this.totalAmmo < 0)
                    {
                         this.totalAmmo = 0;
                    }
               }
          }

          /// <summary>
          /// Resets all the logic related to Reloading the Gun. We use this when switching between Guns, for example, so we have a clean slate.
          /// </summary>
          public void ResetReloading()
          {
               // We are done reloading.
               this.isReloading = false;

               // Reset the elapsed reload time.
               this.reloadTimeElapsed = 0.0f;

               // Reset the active reload success flag.
               this.isActiveReloadSuccessful = false;

               // Rset the active reload attempted flag.
               this.isActiveReloadAttempted = false;
          }

          #endregion


          #region Active Reload Logic Method

          /// <summary>
          /// Handles the Active Reload aspect of the reloading logic.
          /// </summary>
          public void ActiveReload(GameTime gameTime)
          {
               // Grab the time.
               double time = gameTime.TotalGameTime.TotalSeconds;

               // Returns 0-2
               float pulsate = (float)Math.Sin((time * 6) * (0.25f)) + 1;

               // Increment the timing mechanism on the reload meter.
               timing += (float)gameTime.ElapsedGameTime.TotalSeconds;

               // Don't update bar if we didnt succeed. Used to let the player know 
               // where the bar was at when they made their reload attempt.
               if (this.isActiveReloadAttempted && !this.isActiveReloadSuccessful)
               {
                    return;
               }

               // Returns 0-1 * 100 => 0-100
               activeReloadStop = (float)Math.Abs(Math.Sin((timing * 6) * (0.25f))) * 100.0f;
          }

          #endregion


          #region Active Reload Drawing Method

          /// <summary>
          /// Draws the Active Reload. This includes the clutch area, the sliding pin, etc.
          /// </summary>
          public void DrawActiveReload(GameTime gameTime)
          {
               //TextManager.DrawString(((int)sigh).ToString(), new Vector2(250, 470), Color.White);


               int vWidth = ScreenManager.GraphicsDevice.Viewport.Width;
               int vHeight = ScreenManager.GraphicsDevice.Viewport.Height;

               Rectangle activeReloadBgPosition = new Rectangle((int)(vWidth - (vWidth * 0.10f) - (205)), vHeight - (int)(vHeight * 0.1f) - 62 - 25, 205, 20);
               Rectangle activeReloadBarPosition = new Rectangle(activeReloadBgPosition.X + (int)(activeReloadStop * 2.05f) - 5, activeReloadBgPosition.Y, 10, 20);
               Rectangle activeReloadRegionPosition = new Rectangle((int)(vWidth - (vWidth * 0.10f) - (205) + 98.5f), vHeight - (int)(vHeight * 0.1f) - 62 - 25, 15, 20);

               //MySpriteBatch.Draw(blankTexture.Texture2D, activeReloadBgPosition, Color.Black * (175f / 255f));
               //MySpriteBatch.Draw(blankTextureBlank.Texture2D, activeReloadRegionPosition, Color.White * (255f / 255f));      
               //MySpriteBatch.Draw(blankTexture.Texture2D, activeReloadBarPosition, Color.Gold * (255f / 255f));


               // Testing render target.
               this.Game.GraphicsDevice.SetRenderTarget(ReloadRenderTarget);
               this.Game.GraphicsDevice.Clear(Color.Transparent);

               int top = 8;

               MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.Immediate);

               //Rectangle uhRect = new Rectangle(2, top, 205 - 4, 20);
               //GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, uhRect, 3, Color.White);

               
               // Top
               MySpriteBatch.Draw(blankTextureBlank.Texture2D, new Rectangle(0, top, 205, 2), Color.White);

               // Bottom
               MySpriteBatch.Draw(blankTextureBlank.Texture2D, new Rectangle(0, 36 - top, 205, 2), Color.White);

               // Left
               MySpriteBatch.Draw(blankTextureBlank.Texture2D, new Rectangle(0, top, 2, 20), Color.White);

               // Right
               MySpriteBatch.Draw(blankTextureBlank.Texture2D, new Rectangle(205 - 2, top, 2, 20), Color.White);
               

               Color clutchAreaColor = Color.White;

               if (!this.isActiveReloadSuccessful && this.isActiveReloadAttempted)
               {
                    clutchAreaColor = Color.DarkRed;
               }

               if (Math.Abs(activeReloadStop - 50.0f) < ((10 / 2) + (activeReloadArea / 2f)))
               {
                    clutchAreaColor = Color.Gold;
               }

               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(0 + 2, top + 2, 205 - 4, 20 - 4), Color.Black * (150f / 255f)); 
               MySpriteBatch.Draw(blankTextureBlank.Texture2D, new Rectangle((int)((205f / 2f) - ((activeReloadArea / 2f)) * 2.05f), top + 2, (int)(activeReloadArea * 2.05f), 36 - (top * 2)), clutchAreaColor);
               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(0 + (int)(activeReloadStop * 2.05f) - 5, 0, top, 20 + (top * 2)), Color.White * (255f / 255f));

               MySpriteBatch.End();

               // testing render target.
               this.Game.GraphicsDevice.SetRenderTarget(null);
               this.Game.GraphicsDevice.Clear(Color.Black);
          }

          #endregion
     }

     #endregion


     #region Bullet Class

     /// <remarks>
     /// Defines a Bullet object.
     /// </remarks>
     public class Bullet : DrawableGameComponent
     {

          // Fields for explosion creation.
          private List<ParticleData> particleList = new List<ParticleData>();
          private GameResourceTexture2D explosionTexture;
          private Random randomizer = new Random();
          private bool addExplosion = true;


          #region Helper Particle Data Structure

          public struct ParticleData
          {
               public float BirthTime;
               public float MaxAge;
               public Vector2 OrginalPosition;
               public Vector2 Accelaration;
               public Vector2 Direction;
               public Vector2 Position;
               public float Scaling;
               public Color ModColor;
          }

          #endregion


          #region Explosion Particle Effect Methods

          public void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
          {
               for (int i = 0; i < numberOfParticles; i++)
                    AddExplosionParticle(explosionPos, size, maxAge, gameTime);

               float rotation = (float)randomizer.Next(10);
               Matrix mat =
                    Matrix.CreateTranslation(-explosionTexture.Texture2D.Width / 2, -explosionTexture.Texture2D.Height / 2, 0) *
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(size / (float)explosionTexture.Texture2D.Width * 2.0f) *
                    Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
          }

          private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
          {
               ParticleData particle = new ParticleData();

               particle.OrginalPosition = explosionPos;
               particle.Position = particle.OrginalPosition;

               particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
               particle.MaxAge = (1f / Player.SlowMotionFactor) * maxAge;
               particle.Scaling = 0.25f;
               particle.ModColor = Color.White;

               float particleDistance = (float)randomizer.NextDouble() * explosionSize;
               Vector2 displacement = new Vector2(particleDistance, 0);
               float angle = MathHelper.ToRadians(randomizer.Next(360));
               displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

               particle.Direction = displacement * 2.0f;
               particle.Accelaration = -particle.Direction;

               particleList.Add(particle);
          }

          private void UpdateParticles(GameTime gameTime)
          {
               float now = (float)(gameTime.TotalGameTime.TotalMilliseconds);

               for (int i = particleList.Count - 1; i >= 0; i--)
               {
                    ParticleData particle = particleList[i];
                    float timeAlive = now - particle.BirthTime;

                    if (timeAlive > particle.MaxAge)
                    {
                         particleList.RemoveAt(i);
                    }
                    else
                    {
                         float relAge = timeAlive / particle.MaxAge;
                         particle.Position = 0.5f * particle.Accelaration * relAge * relAge + particle.Direction * relAge + particle.OrginalPosition;


                         // testing
                         Vector3 testPos = GraphicsHelper.ConvertToScreenspaceVector3(Matrix.CreateTranslation(impactPosition3D));

                         particle.Position = 0.5f * particle.Accelaration * relAge * relAge + particle.Direction * relAge + new Vector2(testPos.X, testPos.Y);// particle.OrginalPosition;



                         float invAge = 1.0f - relAge;
                         particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));


                         //Vector2 positionFromCenter = particle.Position - particle.OrginalPosition;

                         // testing
                         Vector2 positionFromCenter = particle.Position - new Vector2(testPos.X, testPos.Y);

                         float distance = positionFromCenter.Length();
                         particle.Scaling = (50.0f + distance) / 200.0f;

                         particleList[i] = particle;
                    }
               }
          }

          private void DrawExplosion()
          {
               for (int i = 0; i < particleList.Count; i++)
               {
                    ParticleData particle = particleList[i];

                    MySpriteBatch.Draw(explosionTexture.Texture2D, particle.Position, null, particle.ModColor * (100f / 255f), i, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
               }
          }

          #endregion


          #region Fields

          /// <summary>
          /// The model to represent the Bullet.
          /// </summary>
          private Model3D bulletModel;

          /// <summary>
          /// Position of the Bullet.
          /// </summary>
          public Vector3 Position;

          /// <summary>
          /// Speed of the Bullet.
          /// </summary>
          public float Speed;

          /// <summary>
          /// Velocity of the Bullet.
          /// </summary>
          public Vector3 Velocity;

          /// <summary>
          /// Target position for the Bullet.
          /// </summary>
          public Vector3 TargetPosition;

          /// <summary>
          /// Flag for removing a Bullet.
          /// </summary>
          public bool RemoveBullet;

          /// <summary>
          /// Whether or not this Bullet is a Piercing one.
          /// 
          /// A Piercing Bullet keeps going upon impact with an Enemy.
          /// Useful for hitting all Enemies that are in a straight line.
          /// </summary>
          public bool IsPiercingBullet;

          /// <summary>
          /// Whether or not this Bullet is an Explosive one.
          /// 
          /// An Explosive Bullet creates an explosion that damages 
          /// nearby enemies.
          /// </summary>
          public bool IsExplosiveBullet;

          public enum BulletType
          {
               Standard,
               Piercing,
               Explosive
          }

          public BulletType bulletType = BulletType.Standard;


          #endregion


          #region Properties

          /// <summary>
          /// Gets or sets the Model3D of the Gun.
          /// </summary>
          public Model3D Model
          {
               get { return bulletModel; }
               set { bulletModel = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Instanciates a Bullet.
          /// </summary>
          /// <param name="game">The current Game instance.</param>
          /// <param name="enemyManager">The EnemyManager to handle this TypingEnemy.</param>
          public Bullet(Game game)
               : base(game)
          {
               Position = new Vector3(0.0f);
               Velocity = new Vector3(0f, 0f, 0.5f);
               TargetPosition = new Vector3(0.0f);
               RemoveBullet = false;

               IsPiercingBullet = false;
               IsExplosiveBullet = true;

               LoadContent();
          }

          /// <summary>
          /// Initializes the enemy manager component.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();
          }

          /// <summary>
          /// Loads a particular enemy sprite sheet and sounds.
          /// </summary>
          protected override void LoadContent()
          {
               bulletModel = new Model3D();
               bulletModel.Model = EngineCore.Content.Load<Model>(@"Models\Weapons\Bullet");
               //bulletModel.EmissiveColor = Color.Blue;
               //bulletModel.SpecularColor = Color.Blue;

               explosionTexture = ResourceManager.LoadTexture(@"Textures\Explosion");
          }

          /// <summary>
          /// Called when graphics resources need to be unloaded. Override this method
          /// to unload any component-specific graphics resources.
          /// </summary>
          protected override void UnloadContent()
          {

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

          public Vector3 TrueVelocity = new Vector3();


          #region Update

          /// <summary>
          /// Updates the Position of the Bullet.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // The bullet's position is now plus the velocity.
               //Position = Position + Velocity;

               Position = new Vector3(
                    Position.X + (Player.SlowMotionFactor * (TrueVelocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds)),
                    Position.Y + (Player.SlowMotionFactor * (TrueVelocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds)),
                    Position.Z + (Player.SlowMotionFactor * (TrueVelocity.Z * (float)gameTime.ElapsedGameTime.TotalSeconds)));


               // For muzzle flash.
               if (Position.Z > 0f)
               {
                    if (addExplosion)
                    {
                         impactPosition3D = new Vector3(this.Position.X, this.Position.Y, this.Position.Z + 1f);
                         Vector2 explosionPosition = GraphicsHelper.ConvertToScreenspaceVector2(Matrix.CreateTranslation(this.Position));

                         AddExplosion(explosionPosition, 8, 45.0f, 1000.0f, gameTime);
                         addExplosion = false;
                    }
               }

               // Remove Bullet at this point: No Enemies could possibly be beyond this point.
               if (Position.Z > 38f)
               {
                    RemoveBullet = true;
               }

               if (particleList.Count > 0)
               {
                    UpdateParticles(gameTime);
               }
          }

          Vector3 impactPosition3D = new Vector3();

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Bullet model.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               //DrawBulletType(gameTime);

               Matrix mat =
                    Matrix.CreateRotationY(MathHelper.ToRadians(-90f))
                    * Matrix.CreateScale(0.15f              * 0.5f)
                    * Matrix.CreateTranslation(Position);


               bulletModel.SpecularColor = Color.Silver;


               bulletModel.DrawModel(mat);



               //MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred);
               DrawExplosion();
               //MySpriteBatch.End();
          }

          #endregion


          public void DrawBulletType(GameTime gameTime)
          {
               PixelEngine.Text.TextManager.DrawCentered(false, ScreenManager.Font, "Bullet Type: " + bulletType.ToString(), new Vector2(250, 550), Color.Gold, 0.5f);
          }
     }

     #endregion
}
