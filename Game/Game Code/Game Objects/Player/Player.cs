#region File Description
//-----------------------------------------------------------------------------
// Player.cs
// Author: Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace AvatarsInGraveDanger
{
    /// <remarks>
    /// Defines a Player, including his Avatar appearance.
    /// </remarks>
    public class Player : DrawableGameComponent, ICameraTrackable
    {
        /// <summary>
        /// The possible animation types on the avatar.
        /// </summary>
        public enum PlayerAnimationType
        {
            Walk, Run, StrafeLeft, StrafeRight,
            Jump, Throw, Shoot, Reload, Kick, Punch, Faint
        };

        // Tells us if we are using an idle, walking, or other animations
        public PlayerAnimationType currentType;
        
        
        public List<Gun> PlayersGuns = new List<Gun>();

        #region Fields

        private List<Gun> playersGuns = new List<Gun>();

        public List<Barrier> playersBarriers = new List<Barrier>();

        /// <summary>
        /// The amount of Money the Player possesses.
        /// 
        /// Money can be used to buy Health, Grenades, and Upgrades.
        /// </summary>
        private int money = 0;

        /// <summary>
        /// The Player's Gamer Information.
        /// </summary>
        private GamerInformation gamerInformation;

        /// <summary>
        /// The Player's Avatar.
        /// 
        /// The Avatar describes the Player's physical appearance.
        /// </summary>
        private Avatar avatar;

        /// <summary>
        /// The Player's Health.
        /// </summary>
        private int health;

        /// <summary>
        /// The Player's Position (world coordinates).
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// A Player contains a list of Achievements he has earned.
        /// </summary>
        private List<PixelEngine.AchievementSystem.Achievement> earnedAchievements = new List<PixelEngine.AchievementSystem.Achievement>();

        /// <summary>
        /// World Matrix used for rendering (Just a helper variable).
        /// </summary>
        private Matrix worldMatrix;

        /// <summary>
        /// The Player's Gun.
        /// </summary>
        public Gun playersGun;

        /// <summary>
        /// Whether or not we have / should have the Player's
        /// Gun Equipped.
        /// </summary>
        private bool isGunEquipped = false;


        /// <summary>
        /// Whether or not the Player is on a Gameplay Screen.
        /// </summary>
        public bool IsInGameplay = false;

        /// <summary>
        /// An Avatar Animation that gives the Player a
        /// standing appearance.
        /// </summary>
        private AvatarBaseAnimation standAnimation;

        /// <summary>
        /// Whether or not the Player is currently Jumping.
        /// </summary>
        private bool isJumping = false;


        /// <summary>
        /// Whether or not the Player is currently Reloading.
        /// </summary>
        public bool isReloading = false;

        /// <summary>
        /// Whether or not the Player is currently moving, i.e
        /// Strafing. We need this to handle animation playing.
        /// </summary>
        public bool isMoving = false;

        /// <summary>
        /// The number of Grenades the Player currently has.
        /// </summary>
        private int numberOfGrenades = 1;

        /// <summary>
        /// Whether or not the Player is currently Throwing a Grenade.
        /// </summary>
        private bool isThrowing = false;

        /// <summary>
        /// A list of the Player's Grenades.
        /// </summary>
        private List<Grenade> grenadeList = new List<Grenade>();

        /// <summary>
        /// The Player's Slow Motion Device.
        /// </summary>
        public SlowMotionDevice slowMotionDevice;

        /// <summary>
        /// The Player's 5 Upgradeable Attributes.
        /// </summary>
        private int strafeSpeedLevel = 0;
        private float strafingSpeed = 1.0f;

        private int slowMotionLevel = 0;
        private float slowMotionDuration = 0f;

        private int cashPerkLevel = 0;
        private float cashPerk = 1.0f;

        public float CashPerk
        {
            get { return cashPerk; }
            set { cashPerk = value; }
        }

        /// <summary>
        /// The Player's Strafe Speed.
        /// This is one of his 5 Upgradeable Attributes.
        /// </summary>
        public float StrafeSpeed
        {
            get { return strafingSpeed; }
            set { strafingSpeed = value; }
        }

        public float SlowMotionDuration
        {
            get { return slowMotionDuration; }
            set { slowMotionDuration = value; }
        }

        /// <summary>
        /// Constant helper variable representing the
        /// max level a Player's Attribute Upgrades can be.
        /// 
        /// Since we begin at Level 0, a Max Upgrade Level of 5 means
        /// we can Upgrade the attribute 5 times.
        /// </summary>
        public const int MaxUpgradeLevel = 5;


        public const int MinUpgradeLevel = 0;

        /// <summary>
        /// Helper variable for handling when the Player's Heart Beat
        /// sound effect is to play.
        /// 
        /// The Heart Beat sound effect signals the Player is close to dying.
        /// </summary>
        private float elapsedHeartbeat = 3f;

        /// <summary>
        /// Helper variable for determining how fast we should
        /// play the Player's Walking Animation. This is based
        /// on how fast he is currently strafing.
        /// </summary>
        private float walkAnimationSpeed = 0f;

        /// <summary>
        /// Not really used.
        /// 
        /// Location of our reticule.
        /// </summary>
        private float cursorY = 0f;

        /// <summary>
        /// Private helper variable needed for Avatar.LoadUserAvatar() call.
        /// We just grab this value from the Player constructor.
        /// </summary>
        private PlayerIndex PlayerIndex = PlayerIndex.One;

        /// <summary>
        /// Not used.
        /// 
        /// A simple, quick way to find the location of the Avatar's Right hand,
        /// for placing objects such as the gun and grenade in it.
        /// </summary>
        public Vector3 RightHandPosition = new Vector3();





        public AvatarMultipleAnimation reloadAndStrafe;



        public AvatarCustomAnimation fireAnimation;
        public AvatarCustomAnimation reloadAnimation;


        public AvatarCustomAnimation strafeLeftAnimation;
        public AvatarCustomAnimation strafeRightAnimation;



        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the GamerInformation that is associated with this Player.
        /// 
        /// GamerInformation is essentially a wrapper for Microsoft's SignedInGamer properties.
        /// </summary>
        public GamerInformation GamerInformation
        {
            get { return gamerInformation; }
            set { gamerInformation = value; }
        }

        /// <summary>
        /// The Level of the Player's Strafe Speed attribute.
        /// We clamp this between 1 and MaxUpgradeLevel.
        /// </summary>
        public int StrafeSpeedLevel
        {
            get
            {
                return (int)MathHelper.Clamp(strafeSpeedLevel, MinUpgradeLevel, MaxUpgradeLevel);
            }

            set
            {
                strafeSpeedLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
            }
        }

        /// <summary>
        /// The Level of the Player's Strafe Speed attribute.
        /// We clamp this between 1 and MaxUpgradeLevel.
        /// </summary>
        public int SlowMotionLevel
        {
            get
            {
                return (int)MathHelper.Clamp(slowMotionLevel, MinUpgradeLevel, MaxUpgradeLevel);
            }

            set
            {
                slowMotionLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
            }
        }

        public static float SlowMotionFactor
        {
            get
            {
                if (IsSlowMotion)
                {
                    return slowMotionFactor;
                }

                else
                {
                    return 1f;
                }
            }
        }

        private static float slowMotionFactor = 0.25f;

        public static bool IsSlowMotion = false;


        /// <summary>
        /// The Level of the Player's Strafe Speed attribute.
        /// We clamp this between 1 and MaxUpgradeLevel.
        /// </summary>
        public int CashPerkLevel
        {
            get
            {
                return (int)MathHelper.Clamp(cashPerkLevel, MinUpgradeLevel, MaxUpgradeLevel);
            }

            set
            {
                cashPerkLevel = (int)MathHelper.Clamp(value, MinUpgradeLevel, MaxUpgradeLevel);
            }
        }

        /// <summary>
        /// Returns a List of Grenades owned by the Player.
        /// </summary>
        public List<Grenade> GrenadeList
        {
            get { return grenadeList; }
            private set { grenadeList = value; }
        }

        /// <summary>
        /// How much Money the Player has in his posession.
        /// 
        /// Money can be used to purchase Items and Upgrades.
        /// </summary>
        public int Money
        {
            get { return money; }
            set { money = value; }
        }

        /// <summary>
        /// Gets or sets the number of Grenades in the Player's possession.
        /// </summary>
        public int NumberOfGrenades
        {
            // Return a value between 0 and 999.
            get
            {
                return (int)MathHelper.Clamp(numberOfGrenades, 0f, 999f);
            }

            // Set to a value between 0 and 999.
            set
            {
                numberOfGrenades = (int)MathHelper.Clamp(value, 0f, 999f);
            }
        }

        /// <summary>
        /// Gets or sets the Player's Avatar.
        /// </summary>
        public Avatar Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }


        /// <summary>
        /// Gets or sets the Player's Health.
        /// </summary>
        public int Health
        {
            get
            {
                return (int)MathHelper.Clamp(health, 0f, 99f);
            }

            set
            {
                if (value < 0)
                {
                    health = 0;
                }

                else
                {
                    health = (int)MathHelper.Clamp(value, 0f, 99f);
                }
            }
        }

        /// <summary>
        /// Returns true if the Player is still Alive, i.e
        /// if he has Health remaining.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return (this.Health > 0);
            }
        }

        /// <summary>
        /// Gets or sets the Player's 3D Position.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets a list of Achievements the Player has obtained.
        /// </summary>
        public List<PixelEngine.AchievementSystem.Achievement> EarnedAchievements
        {
            get { return earnedAchievements; }
        }


        /// <summary>
        /// Gets or sets the Gun object the Player is using.
        /// </summary>
        public Gun Gun
        {
            get { return playersGun; }
            set { playersGun = value; }
        }

        #endregion


        #region ICameraTrackable Properties

        /// <summary>
        /// Gets or sets the position being tracked by the Camera.
        /// In this case, the Tracked Position is the position of the
        /// player's avatar.
        /// </summary>
        public Vector3 TrackedPosition
        {
            get
            {
                if (avatar != null)
                    return avatar.WorldMatrix.Translation;
                return worldMatrix.Translation;
            }

            set { worldMatrix.Translation = value; }
        }

        /// <summary>
        /// Gets or sets the position (Matrix form) being tracked by the Camera.
        /// In this case, the Tracked World Matrix is the world matrix of the
        /// player's avatar.
        /// </summary>
        public Matrix TrackedWorldMatrix
        {
            get { return avatar.WorldMatrix; }
        }

        #endregion


        #region Initialization

        /// <summary>
        /// Constructs a new player.
        /// </summary>
        public Player(PlayerIndex playerIndex)
            : base(EngineCore.Game)
        {
            slowMotionDevice = new SlowMotionDevice(EngineCore.Game);










            PlayersGuns.Add(new Revolver(EngineCore.Game, this));//Pistol(EngineCore.Game, this));
            PlayersGuns.Add(new Uzi(EngineCore.Game, this));

            playersGun = PlayersGuns[0];

            this.health = 5;

            this.GamerInformation = new GamerInformation(playerIndex);

            this.position = new Vector3(0f, 0f, -4f);

            // Set the ThirdPersonCamera to track the Player.
            //Camera.Track = this;

            this.PlayerIndex = playerIndex;

            LoadContent();

            worldMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Loads the Player's assets (Avatar).
        /// </summary>
        protected override void LoadContent()
        {
            Avatar = new Avatar(EngineCore.Game, 1.0f);

            // Create the Player's Avatar.
            Avatar.LoadUserAvatar(this.PlayerIndex);

            Avatar.Rotation = new Vector3(0, 180, 0);
            Avatar.Scale = 1.0f;

            Avatar.Position = position;

            // Initialize the list of bones in world space
            Avatar.BonesInWorldSpace = new List<Matrix>(AvatarRenderer.BoneCount);

            for (int i = 0; i < AvatarRenderer.BoneCount; i++)
                Avatar.BonesInWorldSpace.Add(Matrix.Identity);

            // Load the preset animations
            standAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.Stand0);

            // Play the regular Stand Animation by default.
            Avatar.PlayAnimation(standAnimation, true);














            // List of the bone index values for the right arm and its children
            List<AvatarBone> bonesUsedInFirstAnimation = new List<AvatarBone>();
            List<AvatarBone> bonesUsedInSecondAnimation = new List<AvatarBone>();

            //bonesUsedInFirstAnimation.Add(AvatarBone.ToeLeft);
            bonesUsedInSecondAnimation.Add(AvatarBone.ShoulderLeft);
            bonesUsedInSecondAnimation.Add(AvatarBone.ShoulderRight);
            bonesUsedInSecondAnimation.Add(AvatarBone.WristLeft);
            bonesUsedInSecondAnimation.Add(AvatarBone.WristRight);
            bonesUsedInSecondAnimation.Add(AvatarBone.ElbowLeft);
            bonesUsedInSecondAnimation.Add(AvatarBone.ElbowRight);

            List<List<AvatarBone>> listOfBonesUsedForEachAnimation = new List<List<AvatarBone>>();
            listOfBonesUsedForEachAnimation.Add(bonesUsedInFirstAnimation);
            listOfBonesUsedForEachAnimation.Add(bonesUsedInSecondAnimation);

            List<AvatarBaseAnimation> animationsToCombine = new List<AvatarBaseAnimation>();
            animationsToCombine.Add(new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["StrafeLeftWithGun"]));
            animationsToCombine.Add(new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["Reload"]));

            reloadAndStrafe = new AvatarMultipleAnimation(Avatar.AvatarRenderer, animationsToCombine, listOfBonesUsedForEachAnimation);




            /*
            List<List<AvatarBone>> sigh = new List<List<AvatarBone>>();
            List<AvatarBone> bonesUsed = new List<AvatarBone>();
            List<AvatarBaseAnimation> animations = new List<AvatarBaseAnimation>();
            bonesUsed.Add(AvatarBone.ShoulderLeft);
            bonesUsed.Add(AvatarBone.ShoulderRight);
            bonesUsed.Add(AvatarBone.WristLeft);
            bonesUsed.Add(AvatarBone.WristRight);
            bonesUsed.Add(AvatarBone.ElbowLeft);
            bonesUsed.Add(AvatarBone.ElbowRight);
            sigh.Add(bonesUsed);

            reloadAnimation = new AvatarMultipleAnimation(Avatar.AvatarRenderer, animations, sigh);
            */

            for (int i = 0; i < AvatarRenderer.BoneCount; ++i)
            {
                finalBoneTransforms.Add(Matrix.Identity);
            }

            // Find the bone index values for the right arm and its children
            leftArmBones = FindInfluencedBones(AvatarBone.ShoulderLeft,
                                                this.Avatar.AvatarRenderer.ParentBones);

            // Find the bone index values for the right arm and its children
            rightArmBones = FindInfluencedBones(AvatarBone.ShoulderRight,
                                                this.Avatar.AvatarRenderer.ParentBones);


            rightElbowBones = FindInfluencedBones(AvatarBone.ElbowRight, this.Avatar.AvatarRenderer.ParentBones);

            rightHandBones = FindInfluencedBones(AvatarBone.WristRight, this.Avatar.AvatarRenderer.ParentBones);



            fireAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["FireGun"]);
            reloadAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["Reload"]);




            strafeLeftAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["StrafeLeftWithGun"]);
            strafeRightAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["StrafeRightWithGun"]);
        }

        /// <summary>
        /// Creates a list of bone index values for the given avatar bone 
        /// and its children.
        /// </summary>
        /// <param name="avatarBone">The root bone to start search</param>
        /// <param name="parentBones">List of parent bones from the avatar 
        /// renderer</param>
        /// <returns></returns>
        List<int> FindInfluencedBones(AvatarBone avatarBone,
                                                     ReadOnlyCollection<int> parentBones)
        {
            // New list of bones that will be influenced
            List<int> influencedList = new List<int>();
            // Add the first bone
            influencedList.Add((int)avatarBone);

            // Start searching after the first bone
            int currentBoneID = influencedList[0] + 1;

            // Loop until we are done with all of the bones
            while (currentBoneID < parentBones.Count)
            {
                // Check to see if the current bone is a child of any of the 
                // previous bones we have found
                if (influencedList.Contains(parentBones[currentBoneID]))
                {
                    // Add the bone to the influenced list
                    influencedList.Add(currentBoneID);
                }
                // Move to the next bone
                currentBoneID++;
            }

            return influencedList;
        }


        /// <summary>
        /// Overridden UnloadContent method.
        /// 
        /// Release any unmanaged resources here.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload Avatar content.
            if (this.Avatar != null)
            {
                this.Avatar.Dispose();
                this.Avatar = null;
            }

            // Unload Grenade content.
            if (this.GrenadeList != null)
            {
                this.GrenadeList.Clear();
                this.GrenadeList = null;
            }

            base.UnloadContent();
        }

        #endregion


        #region Update

        /// <summary>
        /// Update the Player (position, status, etc).
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // If the Player is low on Health...
            if (this.Health <= 2)
            {
                // Decrease the timing mechanism for playing the heartbeat sound.
                elapsedHeartbeat -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Is it time to play the heartbeat sound effect?
                if (elapsedHeartbeat <= 0f)
                {
                    // Play the Heartbeat sound.
                    AudioManager.PlayCue("Heartbeat");

                    // Reset the timer for when the sound is to play again.
                    elapsedHeartbeat = this.Health + 1f;

                    // Finally, vibrate the controller for half a second.
                    InputState.SetVibration(EngineCore.ControllingPlayer.Value, 1.0f, 0.0f, 0.5f);
                }
            }

            // Update the Player's Gun.
            //playersGun.Update(gameTime);

            // Update all the Player's Guns: 
            // This fixes the bug where bullets disappear / stop moving after we switch weapons.
            foreach (Gun guns in PlayersGuns)
            {
                guns.Update(gameTime);
            }

            // Update the Player's Grenades.
            UpdateGrenades(gameTime);

            UpdateBarriers(gameTime);

            UpdateSlowMotionDevice(gameTime);

            // If the Player is still Alive...
            if (this.IsAlive)
            {
                // If we are in a Jump Animation...
                if (currentType == PlayerAnimationType.Jump)
                {
                    // Update the Avatar normally, i.e without tweaking its update speed.
                    Avatar.Update(gameTime);

                    if (Avatar.AvatarAnimation.IsFinished)
                    {
                        isJumping = false;
                    }
                }

                // If we are in a Throwing Animation...
                else if (currentType == PlayerAnimationType.Throw)
                {
                    // Update the Avatar normally, i.e without tweaking its update speed.
                    Avatar.Update(gameTime);

                    if (Avatar.AvatarAnimation.IsFinished)
                    {
                        isThrowing = false;
                    }
                }

                /*
                // If we are in a Reload Animation...
                else if (currentType == PlayerAnimationType.Reload)
                {
                     // Update the Avatar normally, i.e without tweaking its update speed.
                     Avatar.Update(gameTime);

                     if (Avatar.AvatarAnimation.IsFinished)
                     {
                          isReloading = false;
                     }
                }
                */

                // If we are in a Shooting Animation...
                else if (currentType == PlayerAnimationType.Shoot)
                {
                    Avatar.Update(gameTime);

                    //TimeSpan duration = gameTime.ElapsedGameTime;
                    //fireAnimation.Update(duration, false);

                    //avatar.AvatarAnimation.Update(duration, false);
                    //UpdateTransforms();
                }

                // If we are in a Strafe Animation...
                else if (currentType == PlayerAnimationType.StrafeLeft || currentType == PlayerAnimationType.StrafeRight)
                {
                    if (avatar.AvatarAnimation != null)
                    {
                        TimeSpan duration = gameTime.ElapsedGameTime;

                        // Speed-based animation playing.
                        int y = Convert.ToInt32(isMoving);

                        duration = TimeSpan.FromTicks((long)(duration.Ticks * (Math.Abs(walkAnimationSpeed) / 0.0075f) * 1.5f) * y);

                        if (ArcadeLevel.IsFirstPerson && !WaveCompleteScreen.IsScreenUp)
                        {
                            duration = TimeSpan.FromTicks((long)(duration.Ticks * (Math.Abs(walkAnimationSpeed) / 0.0075f) * 0.25f) * y);
                        }


                        if (currentType == PlayerAnimationType.StrafeLeft)
                        {
                            strafeLeftAnimation.Update(duration, true);
                        }
                        else
                        {
                            strafeRightAnimation.Update(duration, true);
                        }

                        //avatar.AvatarAnimation.Update(duration, true);

                        // To update Bones for attachments
                        Avatar.UpdateWithoutAnimating();





                        // FUCKING NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
                        if (this.Gun.IsReloading)
                        {
                            duration = gameTime.ElapsedGameTime;
                            reloadAnimation.Update(duration, false);

                            //avatar.AvatarAnimation.Update(duration, false);
                            UpdateTransforms();
                        }
                    }
                }

                // Otherwise we are probably using a built-in Animation.
                else
                {
                    // Update the Avatar normally, i.e without tweaking its update speed.
                    Avatar.Update(gameTime);
                }
            }

            // Otherwise, the Player is dead, so...
            else
            {
                // Update the Avatar normally, i.e without tweaking its update speed.
                Avatar.Update(gameTime);
            }

            //world.Translation = new Vector3(world.Translation.X, world.Translation.Y, world.Translation.Z + 0.25f / 60f);
            this.Position = worldMatrix.Translation;
            this.Avatar.Position = this.Position;
        }







        // List of the bone index values for the right arm and its children
        List<int> leftArmBones;
        List<int> rightArmBones;

        List<int> rightElbowBones;
        List<int> rightHandBones;

        List<Matrix> finalBoneTransforms = new List<Matrix>(AvatarRenderer.BoneCount);

        /// <summary>
        /// Combines the transforms of the clap and wave animations
        /// </summary>
        private void UpdateTransforms()
        {
            // List of bone transforms from the clap and wave animations
            IList<Matrix> fireTransforms = fireAnimation.CustomBoneTransforms;
            IList<Matrix> reloadTransforms = reloadAnimation.CustomBoneTransforms;

            IList<Matrix> strafeLeftTransforms = strafeLeftAnimation.CustomBoneTransforms;
            IList<Matrix> strafeRightTransforms = strafeRightAnimation.CustomBoneTransforms;


            // Check to see if we are playing the shoot animation in 3rd Person.
            if (currentType == PlayerAnimationType.Shoot)// && !ArcadeLevel.IsFirstPerson)
            {
                // Copy the celebrate transforms to the final list of transforms
                for (int i = 0; i < finalBoneTransforms.Count; i++)
                {
                    finalBoneTransforms[i] = fireTransforms[i];// strafeLeftTransforms[i];
                }
                /*
                // Overwrite the transforms for the bones that are part of the right arm 
                // with the wave animation transforms.
                for (int i = 0; i < rightArmBones.Count; i++)
                {
                    finalBoneTransforms[rightArmBones[i]] = fireTransforms[rightArmBones[i]];
                }

                // Overwrite the transforms for the bones that are part of the right arm 
                // with the wave animation transforms.
                for (int i = 0; i < rightElbowBones.Count; i++)
                {
                    finalBoneTransforms[rightElbowBones[i]] = fireTransforms[rightElbowBones[i]];
                }

                // Overwrite the transforms for the bones that are part of the right arm 
                // with the wave animation transforms.
                for (int i = 0; i < rightHandBones.Count; i++)
                {
                    finalBoneTransforms[rightHandBones[i]] = fireTransforms[rightHandBones[i]];
                }
                */
            }


            // Check to see if we are playing the strafe left animation while reloading.
            else if (currentType == PlayerAnimationType.StrafeLeft && this.Gun.IsReloading)
            {
                // Copy the celebrate transforms to the final list of transforms
                for (int i = 0; i < finalBoneTransforms.Count; i++)
                {
                    finalBoneTransforms[i] = strafeLeftTransforms[i];
                }

                // Overwrite the transforms for the bones that are part of the right arm 
                // with the wave animation transforms.
                for (int i = 0; i < leftArmBones.Count; i++)
                {
                    finalBoneTransforms[leftArmBones[i]] = reloadTransforms[leftArmBones[i]];
                }

                // Overwrite the transforms for the bones that are part of the right arm 
                // with the wave animation transforms.
                for (int i = 0; i < rightArmBones.Count; i++)
                {
                    finalBoneTransforms[rightArmBones[i]] = reloadTransforms[rightArmBones[i]];
                }
            }

            // Check to see if we are playing the strafe right animation while reloading.
            else if (currentType == PlayerAnimationType.StrafeRight && this.Gun.IsReloading)
            {
                // Copy the celebrate transforms to the final list of transforms
                for (int i = 0; i < finalBoneTransforms.Count; i++)
                {

                    finalBoneTransforms[i] = strafeRightTransforms[i];
                }

                // Overwrite the transforms for the bones that are part of the right arm 
                // with the wave animation transforms.
                for (int i = 0; i < leftArmBones.Count; i++)
                {
                    finalBoneTransforms[leftArmBones[i]] = reloadTransforms[leftArmBones[i]];
                }
            }

            else
            {
                // Copy the celebrate transforms to the final list of transforms
                for (int i = 0; i < finalBoneTransforms.Count; i++)
                {
                    finalBoneTransforms[i] = this.Avatar.AvatarAnimation.CustomBoneTransforms[i];
                }
            }
        }

        #endregion


        #region Update Slow Motion Device

        private void UpdateSlowMotionDevice(GameTime gameTime)
        {
            if (slowMotionDevice != null)
            {
                slowMotionDevice.Update(gameTime);
            }
        }

        #endregion


        #region Update Barriers

        private void UpdateBarriers(GameTime gameTime)
        {
            // Ensure the Grenade list is not null.
            if (playersBarriers != null)
            {
                // Now render each Grenade in the list.
                foreach (Barrier barrier in playersBarriers)
                {
                    barrier.Update(gameTime);

                    if (!barrier.IsActive)
                    {
                        playersBarriers.Remove(barrier);
                        break;
                    }
                }
            }
        }

        #endregion


        #region Update Grenades

        /// <summary>
        /// Updates the Player's Grenades by iterating through
        /// the Grenade list and calling Grenade.Update().
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateGrenades(GameTime gameTime)
        {
            if (grenadeList != null)
            {
                foreach (Grenade nade in grenadeList)
                {
                    nade.Update(gameTime, this.Avatar);

                    if (nade.RemoveGrenade)
                    {
                        grenadeList.Remove(nade);
                        break;
                    }
                }
            }
        }

        #endregion


        #region Handle Input

        public void HandleInput(InputState input, GameTime gameTime)
        {
            PlayerIndex playerIndex;
            
            
            // Handle the input for the Player's Gun.
            if (!isThrowing)
            {
                playersGun.HandleInput(input, gameTime, this);
            }


            if (slowMotionDevice != null)
            {
                slowMotionDevice.HandleInput(input, gameTime, this);

            }

            // Left Trigger: Throw Grenade.
            if (input.IsNewButtonPress(Buttons.LeftTrigger, EngineCore.ControllingPlayer, out playerIndex))
            {
                // Only Throw if we aren't already Throwing one.
                if (!isThrowing)
                {
                    ThrowGrenade(gameTime);
                }
            }

            // A Button: Jump.
            if (input.IsNewButtonPress(Buttons.A, EngineCore.ControllingPlayer, out playerIndex))
            {
                //Player.IsSlowMotion = !Player.IsSlowMotion;
            }

            // A Button: Jump.
            if (input.IsNewButtonPress(Buttons.A, EngineCore.ControllingPlayer, out playerIndex))
            {
                // Only Jump if we aren't already Jumping.
                if (!isJumping)
                {
                    //Jump(gameTime);
                }
            }

            // A Button: Jump.
            if (input.IsNewButtonPress(Buttons.DPadLeft, EngineCore.ControllingPlayer, out playerIndex))
            {
                if (!this.Gun.IsReloading)
                {
                    gunIndex--;

                    if (gunIndex < 0)
                    {
                        gunIndex = PlayersGuns.Count - 1;
                    }

                    gunIndex = (int)MathHelper.Clamp(gunIndex, 0, PlayersGuns.Count);

                    playersGun = PlayersGuns[gunIndex];

                    playersGun.ResetReloading();
                }
            }

            if (input.IsNewButtonPress(Buttons.DPadRight, EngineCore.ControllingPlayer, out playerIndex))
            {
                if (!this.Gun.IsReloading)
                {
                    gunIndex++;

                    gunIndex %= 2;

                    gunIndex = (int)MathHelper.Clamp(gunIndex, 0, PlayersGuns.Count);

                    playersGun = PlayersGuns[gunIndex];

                    playersGun.ResetReloading();
                }
            }

            // Handle Avatar input, ie strafing, etc.
            HandleAvatarInput(input, gameTime);
        }

        private int gunIndex = 0;

        #endregion


        #region Draw

        /// <summary>
        /// Draws the Player, first checking if its Avatar is registered with AvatarManager.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred);

            // Render the Player's Grenades, if necessary.
            DrawGrenades(gameTime);

            MySpriteBatch.End();



            MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred); //MySpriteBatch.Begin(BlendState.AlphaBlend);              

            // Temporary way to render slow motion bar at correct times.
            if (isGunEquipped)
            {
                DrawSlowMotionDevice(gameTime);
            }

            DrawBarriers(gameTime);

            // If we have a weapon equipped...
            if (isGunEquipped)
            {
                // And if we are not throwing (don't render gun during Grenade Throwing)...
                if (!isThrowing)
                {
                    // Render the Player's Gun!
                    playersGun.Draw(gameTime);


                    foreach (Gun gun in PlayersGuns)
                    {
                        gun.DrawBullets(gameTime);
                    }
                }
            }

            MySpriteBatch.End();


            // If we are reloading, and not in first person...
            if (this.Gun.IsReloading && !ArcadeLevel.IsFirstPerson)
            {
                this.Avatar.AvatarRenderer.World = this.Avatar.WorldMatrix;
                this.Avatar.AvatarRenderer.Projection = CameraManager.ActiveCamera.ProjectionMatrix;
                this.Avatar.AvatarRenderer.View = CameraManager.ActiveCamera.ViewMatrix;

                this.Avatar.AvatarRenderer.AmbientLightColor = this.Avatar.AmbientLightColor;
                this.Avatar.AvatarRenderer.LightColor = this.Avatar.LightColor;
                this.Avatar.AvatarRenderer.LightDirection = this.Avatar.LightDirection;

                Avatar.AvatarRenderer.Draw(finalBoneTransforms, this.Avatar.AvatarExpression);
            }

            /*
            // If we are not reloading and / or are in first person...
            else
            {
                // Render the Player's Avatar through its default draw.
                Avatar.Draw(gameTime);
            }
            */

            else
            {
                if (ArcadeLevel.IsFirstPerson)
                {
                    Avatar.DrawFirstPerson(gameTime);
                }

                else
                {
                    Avatar.Draw(gameTime);
                }

            }
        }


        /// <summary>
        /// Draws the Player, first checking if its Avatar is registered with AvatarManager.
        /// </summary>
        public void DrawWithoutLaserSight(GameTime gameTime)
        {
            MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred);

            // Render the Player's Grenades, if necessary.
            DrawGrenades(gameTime);

            MySpriteBatch.End();



            MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred); //MySpriteBatch.Begin(BlendState.AlphaBlend);              

            DrawBarriers(gameTime);

            // If we have a weapon equipped...
            if (isGunEquipped)
            {
                // And if we are not throwing (don't render gun during Grenade Throwing)...
                if (!isThrowing)
                {
                    // Render the Player's Gun!
                    playersGun.DrawWithoutLaserSight(gameTime);
                }
            }

            MySpriteBatch.End();

            // Render the Player's Avatar, ie physical appearance.
            if (ArcadeLevel.IsFirstPerson)
            {
                Avatar.DrawFirstPerson(gameTime);
            }

            else
            {
                Avatar.Draw(gameTime);
            }

        }

        private void DrawSlowMotionDevice(GameTime gameTime)
        {
            if (slowMotionDevice != null)
            {
                //if (slowMotionDevice.IsInUse)
                {
                    slowMotionDevice.Draw(gameTime);
                }
            }
        }


        private void DrawBarriers(GameTime gameTime)
        {
            // Ensure the Grenade list is not null.
            if (playersBarriers != null)
            {
                // Now render each Grenade in the list.
                foreach (Barrier barrier in playersBarriers)
                {
                    barrier.Draw(gameTime);
                }
            }
        }

        /// <summary>
        /// Helper Draw method which iterates through the Player's
        /// Grenades and renders them appropriately.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawGrenades(GameTime gameTime)
        {
            // Ensure the Grenade list is not null.
            if (grenadeList != null)
            {
                // Now render each Grenade in the list.
                foreach (Grenade nade in grenadeList)
                {
                    nade.Draw(gameTime);
                }
            }
        }

        /// <summary>
        /// Draws the Player, first checking if its Avatar is registered with AvatarManager.
        /// </summary>
        public void DrawWithoutAvatar(GameTime gameTime)
        {
            // Testing: This use to come before Avatar.Draw: This is ncessary for gun to be placed proper.
            if (isGunEquipped)
            {
                playersGun.DrawWithoutGun(gameTime);
            }
        }

        #endregion


        #region Public Spawn Methods - Not even used!

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Spawn(Vector3 position)
        {
            Position = position;
        }

        #endregion


        #region Gun Equip & Unequip Methods

        public void EquipWeapon()
        {
            isGunEquipped = true;
        }

        public void UnequipWeapon()
        {
            isGunEquipped = false;

            if (this.Gun.BulletList != null)
            {
                this.Gun.BulletList.Clear();
            }
        }

        #endregion


        #region Throwing Grenade Method

        public void ThrowGrenade(GameTime gameTime)
        {
            // Do we even have Grenades?!
            if (numberOfGrenades <= 0)
            {
                return;
            }

            // We do; so subtract this one.
            numberOfGrenades--;

            // Make sure we do not now have less than 0.
            numberOfGrenades = (int)MathHelper.Clamp(numberOfGrenades, 0f, 100f);

            // We are now Throwing.
            isThrowing = true;

            // We are now in a Throw Animation.
            currentType = PlayerAnimationType.Throw;

            // Play the Throwing animation, once.
            Avatar.PlayAnimation(AnimationType.Throw, false);

            // Create our new Grenade at the Player's location.
            Grenade grenade = new Grenade(this.Game);
            grenade.Position = new Vector3(this.Position.X - 0.25f, 2.5f, this.Position.Z);

            // And add it to the Player's Grenade list.
            grenadeList.Add(grenade);
        }

        #endregion


        #region Jumping Method

        public void Jump(GameTime gameTime)
        {
            // We are now Jumping.
            isJumping = true;

            // We are now in a Jump Animation.
            currentType = PlayerAnimationType.Jump;

            // Play the Jump animation, once.
            Avatar.PlayAnimation(AnimationType.Jump, false);
        }

        #endregion


        #region ToString Override

        public override string ToString()
        {
            return String.Format("Player's World Position: {0}", avatar.Position.ToString());
        }

        #endregion


        #region Input Handling

        /// <summary>
        /// Check for user input to play animations on the avatar
        /// </summary>
        public void HandleAvatarInput(InputState inputState, GameTime gameTime)
        {
            // Update the avatars location
            UpdateAvatarMovement(inputState, gameTime);
        }

        /// <summary>
        /// Update the avatars movement based on user input
        /// </summary>
        public void UpdateAvatarMovement(InputState inputState, GameTime gameTime)
        {
            // Don't process further input if we are Jumping or Throwing a Grenade.
            if (isThrowing || isJumping || isReloading)
                return;

            // Create vector from the left thumbstick location
            Vector2 rightThumbStick = inputState.CurrentGamePadStates[(int)EngineCore.ControllingPlayer].ThumbSticks.Right;

            // Create vector from the left thumbstick location
            Vector2 leftThumbStick = inputState.CurrentGamePadStates[(int)EngineCore.ControllingPlayer].ThumbSticks.Left;

            // The direction for our Avatar
            Vector3 avatarForward = worldMatrix.Forward;

            // The amount we want to translate
            Vector3 translate = Vector3.Zero;


            // Between 0 and 15 degrees.
            cursorY = rightThumbStick.Y * 10.0f;

            this.Gun.AngleOfAimer = -2f;// +cursorY;

            // Clamp thumbstick to make sure the user really wants to move
            if (leftThumbStick.Length() > 0.2f && Math.Abs(leftThumbStick.X) > 0.299f)
            {
                isMoving = true;

                if (leftThumbStick.X < 0.3f)
                {
                    // If we aren't already strafing left...
                    if (currentType != PlayerAnimationType.StrafeLeft)
                    {
                        TimeSpan lastAnimationPosition = avatar.AvatarAnimation.CurrentPosition;

                        // Set our animation to Strafe Left and play it!
                        currentType = PlayerAnimationType.StrafeLeft;

                        //if (!ArcadeLevel.IsFirstPerson)
                        {
                            avatar.PlayAnimation(strafeLeftAnimation, false);//AnimationType.StrafeLeftWithGun, false);

                            avatar.AvatarAnimation.CurrentPosition = lastAnimationPosition;
                        }
                    }
                }

                else if (leftThumbStick.X > 0.3f)
                {
                    if (currentType != PlayerAnimationType.StrafeRight)
                    {
                        TimeSpan lastAnimationPosition = avatar.AvatarAnimation.CurrentPosition;

                        currentType = PlayerAnimationType.StrafeRight;

                        //if (!ArcadeLevel.IsFirstPerson)
                        {
                            avatar.PlayAnimation(strafeRightAnimation, false);//AnimationType.StrafeRightWithGun, false);

                            avatar.AvatarAnimation.CurrentPosition = lastAnimationPosition;
                        }
                    }
                }

                // Hard-coded speed at which the Player moves.
                float walkSpeed = 0.0075f;

                // Multiply this by the Player's Strafe Speed attribute.
                walkSpeed *= StrafeSpeed;

                // And lastly, by how far we are leaning the left thumbstick.
                walkSpeed *= Math.Abs(leftThumbStick.Length());


                // Set how fast we play our Strafe Animation to this value - 
                // Necessary for Speed-Based Animation.
                walkAnimationSpeed = walkSpeed;

                // Create our direction vector.
                leftThumbStick.Normalize();

                // Find the new avatar forward
                avatarForward.X = leftThumbStick.X;
                avatarForward.Y = 0;
                avatarForward.Z = -leftThumbStick.Y;

                // Translate the thumbstick using the current camera rotation
                avatarForward = Vector3.Transform(avatarForward, Matrix.CreateRotationY(MathHelper.Pi));//cameraRotation));
                avatarForward.Normalize();

                /*
                // Determine the amount of translation
                translate = avatarForward
                            * ((float)gameTime.ElapsedGameTime.TotalMilliseconds
                            * walkSpeed);// 0.0009f);
                */
                avatarForward.Z = 0;

                translate = avatarForward * ((float)gameTime.ElapsedGameTime.TotalMilliseconds * walkSpeed);
            }


            else
            {
                // If we were moving last frame, we no longer are.
                if (currentType == PlayerAnimationType.StrafeLeft ||
                     currentType == PlayerAnimationType.StrafeRight)
                {
                    isMoving = false;
                }
            }

            // Update the world matrix
            worldMatrix.Forward = //avatarForward; Vector3.Backward;//

            // Normalize the matrix
            worldMatrix.Right = Vector3.Cross(worldMatrix.Forward, Vector3.Up);
            worldMatrix.Right = Vector3.Normalize(worldMatrix.Right);
            worldMatrix.Up = Vector3.Cross(worldMatrix.Right, worldMatrix.Forward);
            worldMatrix.Up = Vector3.Normalize(worldMatrix.Up);

            // If your movement AFTER translation is added is greater than 15...don't move!
            if (Math.Abs(worldMatrix.Translation.X + translate.X) > 15.0f)
            {
                // Don't add X Translation!
                worldMatrix.Translation += new Vector3(0.0f, translate.Y, translate.Z);
            }

            else
                // Add translation
                worldMatrix.Translation += translate;


            // Update the Enemy's Position (and thus Avatar Position).
            this.Position = worldMatrix.Translation;
        }

        #endregion


        #region Testing
        /*
          /// <summary>
          /// Move camera based on user input
          /// </summary>
          private void HandleCameraInput()
          {
               // should we reset the camera?
               if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
               {
                    cameraArc = CameraDefaultArc;
                    cameraDistance = CameraDefaultDistance;
                    cameraRotation = CameraDefaultRotation;
               }

               // Update Camera
               cameraArc -= currentGamePadState.ThumbSticks.Right.Y * 0.05f;
               cameraRotation += currentGamePadState.ThumbSticks.Right.X * 0.1f;
               cameraDistance += currentGamePadState.Triggers.Left * 0.1f;
               cameraDistance -= currentGamePadState.Triggers.Right * 0.1f;

               // Limit the camera movement
               if (cameraDistance > 5.0f)
                    cameraDistance = 5.0f;
               else if (cameraDistance < 2.0f)
                    cameraDistance = 2.0f;

               if (cameraArc > MathHelper.Pi / 5)
                    cameraArc = MathHelper.Pi / 5;
               else if (cameraArc < -(MathHelper.Pi / 5))
                    cameraArc = -(MathHelper.Pi / 5);

               // Update the camera position
               Vector3 cameraPos = new Vector3(0, cameraDistance, cameraDistance);
               cameraPos = Vector3.Transform(cameraPos, Matrix.CreateRotationX(cameraArc));
               cameraPos = Vector3.Transform(cameraPos,
                                             Matrix.CreateRotationY(cameraRotation));

               cameraPos += world.Translation;

               // Create new view matrix
               Matrix view = Matrix.CreateLookAt(cameraPos, world.Translation + new Vector3(0, 1.2f, 0), Vector3.Up);

               // Set the new view on the avatar renderer
               if (avatar.AvatarRenderer != null)
               {
                    avatar.AvatarRenderer.View = view;
               }
          }
          */
        #endregion
    }
}
