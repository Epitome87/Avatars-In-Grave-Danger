#region File Description
//-----------------------------------------------------------------------------
// ArcadeLevel.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
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
using PixelEngine.Text;
using PixelEngine.Avatars;
#endregion

namespace AvatarsInGraveDanger
{
    /// <remarks>
    /// A small environment with collections of items and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// 
    /// An Arcade Level consists of Waves of Enemies.
    /// </remarks>
    public class ArcadeLevel : Level
    {
        #region Fields

        // Temp & shitty solution for Mode determination.

        public static int Mode = 0;
        public const int MAX_WAVE = 25;

        // The background texture to be used.
        private GameResourceTexture2D skyTexture;

        private Random random = new Random();

        // Set up the cool blue lighting!
        private Vector3 ambientLightColor = Color.White.ToVector3();
        private Vector3 lightDirection = new Vector3(-1.25f, -0.25f, -1.0f);
        private Vector3 lightColor = Color.CornflowerBlue.ToVector3();

        // Quads to render HUD items onto.
        private Quad[] hudQuads = new Quad[5];
        private BasicEffect[] hudQuadEffects = new BasicEffect[5];

        // Our HUD Render Targets. Slap these onto Quads for 3D looking HUD!
        private RenderTarget2D ammoRenderTarget = new RenderTarget2D(ScreenManager.Game.GraphicsDevice, 205, 62);
        private RenderTarget2D waveRenderTarget = new RenderTarget2D(ScreenManager.Game.GraphicsDevice, 205, 62);
        private RenderTarget2D heartRenderTarget = new RenderTarget2D(ScreenManager.Game.GraphicsDevice, 205, 62);
        private RenderTarget2D scoreRenderTarget = new RenderTarget2D(ScreenManager.Game.GraphicsDevice, 205, 62);

        // Helper variables used for Quad HUD rendering & updating.
        private Vector3 fakeCameraPosition = new Vector3();
        private Vector3 fakeCameraLookAt = new Vector3(0f, 0f, 20f);
        private Vector3 quadPosition = new Vector3();
        private Quaternion quadRotation = new Quaternion();

        private Vector3[] quadPositions = new Vector3[5] 
          { 
               new Vector3(2f, 1.15f, 2.5f),
               new Vector3(2f, -1.15f, 2.5f),
               new Vector3(-2f, 1.15f, 2.5f),
               new Vector3(-2f, -1.15f, 2.5f),
               new Vector3(-2f, -0.95f, 2.5f)
          };

        private Quaternion[] quadRotations = new Quaternion[5]
          {
               new Quaternion(),
               new Quaternion(),
               new Quaternion(),
               new Quaternion(),
               new Quaternion()
          };

        private RenderTarget2D[] quadRenderTargets = new RenderTarget2D[5];

        #endregion


        #region Initialization

        public ArcadeLevel(Game game, Stage theStage)
            : base(game)
        {
            // Initialize a new Wave Manager.
            waveManager = new WaveManager(this, 25);

            AvatarZombieGame.AwardData.SpeedStreak = 0;
            AvatarZombieGame.AwardData.AccuracyStreak = 0;

            this.Stage = theStage;

            Vector3 quadOrigin = this.Stage.QuadOrigin;
            Vector3 quadNormal = this.Stage.QuadNormal;
            Vector3 quadUp = this.Stage.QuadUp;
            float quadWidth = this.Stage.QuadWidth;
            float quadHeight = this.Stage.QuadHeight;

            quadOrigin = new Vector3(0);

            for (int r = 0; r < 5; r++)
            {
                if (r != 4)
                {
                    hudQuads[r] = new Quad(new Vector3(quadOrigin.X + (r * 0f), quadOrigin.Y, quadOrigin.Z), Vector3.Forward, Vector3.Up, 2.0f, 1f);    // 2f, 1f
                }
                else
                {
                    hudQuads[r] = new Quad(new Vector3(quadOrigin.X + (r * 0f), quadOrigin.Y, quadOrigin.Z), Vector3.Forward, Vector3.Up, 2f, 0.25f);
                }
            }

            AvatarZombieGame.CurrentArcadeLevel = this;
        }

        GameResourceTexture2D gunIconTexture;
        GameResourceTexture2D grenadeIconTexture;

        GameResourceTexture2D uziIconTexture;
        GameResourceTexture2D pistolIconTexture;

        public override void LoadContent()
        {
            /*
            merchantAvatar.LoadAvatar(CustomAvatarType.Merchant);
            merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleIdleShiftWeight, true);
            merchantAvatar.Position = new Vector3(-20f, 0f, 35f);
            merchantAvatar.Scale = 1.75f;
            */











            // Load the blank texture and border, used on various HUD backdrops.
            blankTexture = ResourceManager.LoadTexture(@"Textures\TextBubble_3D");
            borderTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_Wide_White");

            // Load the HUD icon textures.
            heartIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\Heart");
            enemyIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\EnemyIcon");
            speedyIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\SpeedyIcon");
            perfectIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\PerfectIcon");

            pistolIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\GunIcon");
            uziIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\UziIcon");
            gunIconTexture = pistolIconTexture;

            grenadeIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\GrenadeIcon");
            heartLeftIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\Heart_Left");
            heartRightIconTexture = ResourceManager.LoadTexture(@"Textures\HUD\Heart_Right");
            blankHudBgTexture = ResourceManager.LoadTexture(@"Textures\HUD\BlankHudBG");
            blankHudBgBorderTexture = ResourceManager.LoadTexture(@"Textures\HUD\BlankHudBgBorder");

            // Load the Stage (Either Fortress or Graveyard thus far.)
            Stage.LoadContent();

            // Try to put this in Stage.cs
            skyTexture = ResourceManager.LoadTexture(@"Textures\Backgrounds\Sky_Night");


            // Load and begin the current Wave.
            //this.CurrentWaveManager.CurrentWave.Start();

            // Create the Player.
            thePlayer = new Player(AvatarZombieGame.CurrentPlayer.GamerInformation.PlayerIndex);
            thePlayer.Avatar.Scale = 1.75f;
            thePlayer.EquipWeapon();

            waveManager.CurrentPlayer = thePlayer;


            // Set up the Camera.
            CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
            CameraManager.ActiveCamera.Position = new Vector3(0, 3f, -6f);
            CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0.5f, 20f);

            FirstPersonCamera.headOffset = new Vector3(0, thePlayer.Avatar.AvatarDescription.Height * thePlayer.Avatar.Scale * 0.80f, 0.25f);// * 0.90, 0);


            #region Quad & Render Target Initialization

            // Testing 3D HUD
            for (int r = 0; r < 5; r++)
            {
                {
                    hudQuadEffects[r] = new BasicEffect(EngineCore.GraphicsDevice);

                    //hudQuadEffects[r].EnableDefaultLighting();

                    hudQuadEffects[r].World = Matrix.CreateTranslation(new Vector3(0, 0.01f, 0f)) * Matrix.CreateScale(1.0f);
                    hudQuadEffects[r].View = CameraManager.ActiveCamera.ViewMatrix;
                    hudQuadEffects[r].Projection = CameraManager.ActiveCamera.ProjectionMatrix;
                    hudQuadEffects[r].TextureEnabled = true;
                }
            }

            quadRenderTargets[0] = heartRenderTarget;
            quadRenderTargets[1] = waveRenderTarget;
            quadRenderTargets[2] = scoreRenderTarget;
            quadRenderTargets[3] = ammoRenderTarget;
            quadRenderTargets[4] = this.CurrentPlayer.Gun.ReloadRenderTarget;

            // Testing for 3D HUD
            for (int r = 0; r < 5; r++)
            {
                foreach (EffectPass pass in hudQuadEffects[r].CurrentTechnique.Passes)
                {
                    switch (r)
                    {
                        case 0:
                            quadRotation = new Quaternion(0f, quadRotation.Y, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, 30f, quadRotation.Z, 0f);      // 15 or 45
                            quadRotation = new Quaternion(quadRotation.X, quadRotation.Y, 0f, 0f);

                            quadRotations[0] = quadRotation;

                            hudQuads[r].Origin = quadPosition / 2;

                            break;

                        case 1:
                            quadRotation = new Quaternion(0f, quadRotation.Y, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, 30f, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, quadRotation.Y, 0f, 0f);

                            quadRotations[1] = quadRotation;

                            hudQuads[r].Origin = quadPosition / 1;

                            break;

                        case 2:
                            quadRotation = new Quaternion(0f, quadRotation.Y, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, -30f, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, quadRotation.Y, 0f, 0f);

                            quadRotations[2] = quadRotation;

                            hudQuads[r].Origin = quadPosition / 1;

                            break;

                        case 3:
                            quadRotation = new Quaternion(0f, quadRotation.Y, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, -30f, quadRotation.Z, 0f); //- 45f
                            quadRotation = new Quaternion(quadRotation.X, quadRotation.Y, 0f, 0f);


                            quadRotations[3] = quadRotation;

                            hudQuads[r].Origin = quadPosition / 1;

                            break;

                        case 4:
                            quadRotation = new Quaternion(0f, quadRotation.Y, quadRotation.Z, 0f);
                            quadRotation = new Quaternion(quadRotation.X, -30f, quadRotation.Z, 0f); //- 45f
                            quadRotation = new Quaternion(quadRotation.X, quadRotation.Y, 0f, 0f);

                            quadRotations[4] = quadRotation;

                            hudQuads[r].Origin = quadPosition / 1;

                            break;
                    }
                }
            }

            float hudScale = 1.0f;

            if (EngineCore.GraphicsDevice.Viewport.Width >= 1920 && EngineCore.GraphicsDevice.Viewport.Height >= 1080)
            {
                hudScale = 1280f / 1920f;
            }

            // Improved HUD Quad rendering
            for (int i = 0; i < 5; i++)
            {
                hudQuadEffects[i].Texture = quadRenderTargets[i];

                hudQuadEffects[i].World = Matrix.CreateScale(0.25f * 1.20f * hudScale)
                          * Matrix.CreateRotationX(MathHelper.ToRadians(quadRotations[i].X))
                          * Matrix.CreateRotationY(MathHelper.ToRadians(quadRotations[i].Y))
                          * Matrix.CreateRotationZ(MathHelper.ToRadians(quadRotations[i].Z))
                          * Matrix.CreateTranslation(quadPositions[i]);

                hudQuadEffects[i].View = Matrix.CreateLookAt(fakeCameraPosition, fakeCameraLookAt, Vector3.Up);

                hudQuadEffects[i].Projection = CameraManager.ActiveCamera.ProjectionMatrix;
            }

            #endregion
        }

        #endregion


        #region Handle Input

        public static bool IsFirstPerson = false;

        public override void HandleInput(InputState input, GameTime gameTime)
        {
            // Handle the Player's input.
            this.CurrentPlayer.HandleInput(input, gameTime);

            PlayerIndex index;

            #region Toggle First and Third Person

            if (input.IsNewButtonPress(Buttons.DPadDown, null, out index))
            {
                ArcadeLevel.IsFirstPerson = false;
            }

            if (input.IsNewButtonPress(Buttons.DPadUp, null, out index))
            {
                ArcadeLevel.IsFirstPerson = true;
            }

            #endregion


            #region Debug: Change Lighting
            /*
               if (input.IsButtonDown(Buttons.DPadLeft, null, out index))
               {
                    foreach (SceneObject obj in SceneGraphManager.sceneObjects)
                    {
                         obj.Model.AmbientLightColor.R += 1;
                    }
               }
               if (input.IsButtonDown(Buttons.DPadUp, null, out index))
               {
                    foreach (SceneObject obj in SceneGraphManager.sceneObjects)
                    {
                         obj.Model.AmbientLightColor.G += 1;
                    }
               }
               if (input.IsButtonDown(Buttons.DPadRight, null, out index))
               {
                    foreach (SceneObject obj in SceneGraphManager.sceneObjects)
                    {
                         obj.Model.AmbientLightColor.B += 1;
                    }
               }
               */
            #endregion
        }

        #endregion


        #region Update

        /// <summary>
        /// Overridden Update method.
        /// Updates the Enemy Manager and the Player.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Update the Wave Manager.
            waveManager.Update(gameTime);

            #region Update the Player & his GamerPresence

            // New 3-9-11 for game over screen fix.
            thePlayer.EquipWeapon();

            // Update the Player.
            thePlayer.Update(gameTime);

            // Do we really need this each frame?
            foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
            {
                signedInGamer.Presence.PresenceMode =
                     GamerPresenceMode.Score;

                signedInGamer.Presence.PresenceValue = TotalScore;
            }

            #endregion


            //merchantAvatar.Update(gameTime);
        }

        #endregion


        #region Draw

        /// <summary>
        /// Overridden Draw method.
        /// Draws the Arcade Level: Skybox, SceneGraph items, 
        /// the player, the enemies, and finally the HUD overlay.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (AvatarZombieGame.SeizureModeEnabled)
            {
                foreach (SceneObject sceneObject in SceneGraphManager.sceneObjects)
                {
                    sceneObject.Model.AmbientLightColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                    sceneObject.Model.SpecularColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                    sceneObject.Model.EmissiveColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                }
            }

            // If the Player is Reloading...
            if (this.CurrentPlayer.Gun.isReloading)
            {
                // Draw his Active Reload bar (texture on Quad).
                this.CurrentPlayer.Gun.DrawActiveReload(gameTime);
            }

            // Draw HUD textures to the various render targets.
            DrawHealth(gameTime);
            DrawAmmo(gameTime);
            DrawWave(gameTime);
            DrawScore(gameTime);

            // Draw the Sky Texture.
            MySpriteBatch.Begin();

            Color skyColor = Color.Yellow;//Color.White;

            if (AvatarZombieGame.SeizureModeEnabled)
            {
                skyColor = new Color(random.Next(255), random.Next(255), random.Next(255));
            }

            MySpriteBatch.Draw(skyTexture.Texture2D, new Rectangle(0, 0, EngineCore.GraphicsDevice.Viewport.Width, EngineCore.GraphicsDevice.Viewport.Height), skyColor);

            MySpriteBatch.End();


            // Render the Scene. No need for this in between SpriteBatch.Begin and End.
            SceneGraphManager.Draw(gameTime);



            MySpriteBatch.Begin();

            EngineCore.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Draw the HUD Quads themselves now.
            for (int r = 0; r < 5; r++)
            {
                if (r == 4)
                {
                    // Only render the reloading bar quad if we are reloading.
                    if (!this.CurrentPlayer.Gun.isReloading)
                    {
                        continue;
                    }

                    hudQuadEffects[4].Texture = this.CurrentPlayer.Gun.ReloadRenderTarget;
                }

                foreach (EffectPass pass in hudQuadEffects[r].CurrentTechnique.Passes)
                {
                    pass.Apply();

                    EngineCore.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>
                         (PrimitiveType.TriangleList, hudQuads[r].Vertices, 0, 4, hudQuads[r].Indexes, 0, 2);

                }
            }

            if (AvatarZombieGame.SeizureModeEnabled)
            {
                thePlayer.Avatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                thePlayer.Avatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                thePlayer.Avatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
            }

            else
            {
                thePlayer.Avatar.AmbientLightColor = ambientLightColor;
                thePlayer.Avatar.LightDirection = lightDirection;
                thePlayer.Avatar.LightColor = lightColor;
            }

            MySpriteBatch.End();


            // Enemies don't require a SpriteBatch call (they're purely 3D).
            waveManager.Draw(gameTime);

            // Neither does the Player.
            thePlayer.Draw(gameTime);

            // Nor the Merchant!
            WaveCompleteScreen.DrawMerchant(gameTime);

            // ...But the HUD does! So Begin()!
            MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred);

            // Draw the HUD overlay.
            DrawOverlay(gameTime);

            MySpriteBatch.End();
        }

        /// <summary>
        /// Overridden Draw method.
        /// Draws the Arcade Level: Skybox, SceneGraph items, 
        /// the player, the enemies, and finally the HUD overlay.
        /// </summary>
        /// <param name="gameTime"></param>
        public void DrawWithoutHUD(GameTime gameTime)
        {
            if (AvatarZombieGame.SeizureModeEnabled)
            {
                foreach (SceneObject sceneObject in SceneGraphManager.sceneObjects)
                {
                    sceneObject.Model.AmbientLightColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                    sceneObject.Model.SpecularColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                    sceneObject.Model.EmissiveColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                }
            }


            // If the Player is Reloading...
            if (this.CurrentPlayer.Gun.isReloading)
            {
                // Draw his Active Reload bar (texture on Quad).
                this.CurrentPlayer.Gun.DrawActiveReload(gameTime);
            }

            // Draw the Sky Texture.
            MySpriteBatch.Begin();

            Color skyColor = Color.Yellow;//Color.White;

            if (AvatarZombieGame.SeizureModeEnabled)
            {
                skyColor = new Color(random.Next(255), random.Next(255), random.Next(255));
            }

            MySpriteBatch.Draw(skyTexture.Texture2D, new Rectangle(0, 0, EngineCore.GraphicsDevice.Viewport.Width, EngineCore.GraphicsDevice.Viewport.Height), skyColor);

            MySpriteBatch.End();

            // Render the Scene. No need for this in between SpriteBatch.Begin and End.
            SceneGraphManager.Draw(gameTime);

            MySpriteBatch.Begin();

            EngineCore.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (AvatarZombieGame.SeizureModeEnabled)
            {
                thePlayer.Avatar.LightDirection = new Vector3(random.Next(2), random.Next(2), random.Next(2));
                thePlayer.Avatar.LightColor = new Vector3(random.Next(10), random.Next(10), random.Next(10));
                thePlayer.Avatar.AmbientLightColor = new Color(random.Next(255) * 4, random.Next(255) * 4, random.Next(255) * 4).ToVector3();
            }

            else
            {
                thePlayer.Avatar.AmbientLightColor = ambientLightColor;
                thePlayer.Avatar.LightDirection = lightDirection;
                thePlayer.Avatar.LightColor = lightColor;
            }


            MySpriteBatch.End();


            //ArcadeLevel.DrawMerchant(gameTime);

            // New placement: Use to be before enemyManager.Draw:
            thePlayer.DrawWithoutLaserSight(gameTime);
        }

        #endregion


        #region Draw HUD Methods

        Color hudBorderColor = Color.White;

        static int vWidth = ScreenManager.GraphicsDevice.Viewport.Width;
        static int vHeight = ScreenManager.GraphicsDevice.Viewport.Height;

        protected override void DrawOverlay(GameTime gameTime)
        {
            DrawFallingHeart(gameTime);
        }

        #region Draw Health & Falling Heart

        protected override void DrawHealth(GameTime gameTime)
        {
            string s = string.Format("x {0}", CurrentPlayer.Health.ToString());

            // Testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(heartRenderTarget);
            this.Game.GraphicsDevice.Clear(Color.Transparent);

            MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.Immediate);

            MySpriteBatch.Draw(blankHudBgBorderTexture.Texture2D, new Rectangle(0, 0, 205, 62), hudBorderColor);
            MySpriteBatch.Draw(blankHudBgTexture.Texture2D, new Rectangle(0, 0, 205, 62), Color.Black * (150f / 255f));


            MySpriteBatch.Draw(heartIconTexture.Texture2D, new Rectangle(10, 6, 50, 50), null, Color.Red);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, s, new Vector2(205f * 0.66f, 62f / 2f), Color.White, 1.0f);

            MySpriteBatch.End();

            // testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(null);
            this.Game.GraphicsDevice.Clear(Color.Black);
        }

        public void DrawFallingHeart(GameTime gameTime)
        {
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = 1.0f;

            if (CurrentPlayer.Health <= 2)
            {
                if (CurrentPlayer.Health == 2)
                {
                    pulsate = System.Math.Abs((float)System.Math.Sin(time * 3)) + 0.5f;// time * 6,   + 1; 
                    pulsate = MathHelper.Clamp(pulsate, 0.5f, 1.5f);// *0.75f;
                }

                else
                {
                    pulsate = System.Math.Abs((float)System.Math.Sin(time * 6)) + 0.5f;// time * 6,   + 1; 
                    pulsate = MathHelper.Clamp(pulsate, 0.5f, 1.5f);
                }
            }

            float fuckthisX = pulsate * 0.05f * (heartIconTexture.Texture2D.Width / 2);//42 / 2);
            float fuckthisY = pulsate * 0.05f * (heartIconTexture.Texture2D.Height / 2);

            Vector2 heartOrigin = new Vector2(fuckthisX, fuckthisY);

            heartIconPosition2 = new Vector2(0, 0);// new Vector2((vWidth * 0.10f) + (int)(205.0f * 0.1f), (vHeight * 0.10f) + (int)(62 * 0.15f));

            //MySpriteBatch.Draw(heartIconTexture.Texture2D, heartIconPosition2, null, Color.Red, 0, heartOrigin, 0.05f * pulsate);

            if (lastHP > CurrentPlayer.Health)// && !animateHeart)
            {
                AudioManager.PlayCue("Heart Pop");
                counter.Add(0f);
                rotations.Add(0f);
            }

            for (int i = 0; i < counter.Count; i++)
            {
                counter[i] += 7.5f;
                rotations[i] += 4.5f;

                Rectangle heartLeftIconPosition = new Rectangle((int)(vWidth * 0.10f), (int)(vHeight * 0.10f) + (int)(62 * 0.15f) + (int)counter[i], 100, 100);
                Vector2 heartLeftIconPos = new Vector2((vWidth * 0.10f), (vHeight * 0.10f) + (int)(62 * 0.15f) + (int)counter[i]); //new Vector2((vWidth * 0.10f), 100 - 42 / 2 + (int)counter[i]);

                MySpriteBatch.Draw(heartLeftIconTexture.Texture2D, heartLeftIconPos, null, Color.Red, MathHelper.ToRadians(rotations[i]),
                     new Vector2(0.05f * heartLeftIconTexture.Texture2D.Width / 2, 0.05f * heartLeftIconTexture.Texture2D.Height / 2), 0.05f);

                MySpriteBatch.Draw(heartRightIconTexture.Texture2D, heartLeftIconPos, null, Color.Red, MathHelper.ToRadians(-rotations[i]),
                     new Vector2(heartLeftIconPosition.X, heartLeftIconPosition.Y), 0.05f);

                if (counter[i] > EngineCore.GraphicsDevice.Viewport.Height * 1.10f)
                {
                    counter.Remove(counter[i]);
                    rotations.Remove(rotations[i]);

                    break;
                }
            }

            lastHP = CurrentPlayer.Health;
        }

        #endregion

        #region Draw Ammo

        public void DrawAmmo(GameTime gameTime)
        {
            string t = CurrentPlayer.Gun.AmmoRemainingInClip.ToString();
            string u = CurrentPlayer.Gun.TotalAmmo.ToString();

            if (!CurrentPlayer.Gun.isInfiniteAmmo)
            {
                t = t + "/" + u;
            }


            string grenadeCountString = CurrentPlayer.NumberOfGrenades.ToString();

            // Testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(ammoRenderTarget);
            this.Game.GraphicsDevice.Clear(Color.Transparent);


            MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.Immediate);

            MySpriteBatch.Draw(blankHudBgBorderTexture.Texture2D, new Rectangle(0, 0, 205, 62), hudBorderColor);
            MySpriteBatch.Draw(blankHudBgTexture.Texture2D, new Rectangle(0, 0, 205, 62), Color.Black * (150f / 255f));



            if (this.CurrentPlayer.Gun.GetType() == typeof(Uzi))
            {
                gunIconTexture = uziIconTexture;
            }

            else
            {
                gunIconTexture = pistolIconTexture;
            }

            MySpriteBatch.DrawCentered(gunIconTexture.Texture2D, new Rectangle((int)(205f * 0.175f), (int)(62f * 0.5f), (int)(40 * 1.5f), 40), Color.White);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, t, new Vector2(205f * 0.4f, 62f * 0.5f), Color.White, 0.75f);


            //TextManager.DrawCenteredTest(false, hudFont.SpriteFont, "/" + u, new Vector2(205f * 0.30f + 30f, 62f * 0.5f), Color.White, 0.75f);

            // 205 * 0.65
            MySpriteBatch.DrawCentered(grenadeIconTexture.Texture2D, new Rectangle((int)(205f * 0.70f), (int)(62f * 0.5f), (int)(40f * 0.82f), (int)(40f * 1f)), Color.White);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, grenadeCountString, new Vector2(205f * 0.875f, 62f * 0.5f), Color.White, 0.75f);


            MySpriteBatch.End();

            // testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(null);
            this.Game.GraphicsDevice.Clear(Color.Black);
        }

        #endregion

        #region Draw Wave Number & Enemies Remaining

        /// <summary>
        /// Renders the Wave # text.
        /// </summary>
        /// <param name="gameTime"></param>
        public void DrawWave(GameTime gameTime)
        {
            string s = string.Format("{0}", CurrentWaveManager.Round.ToString());

            // Testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(waveRenderTarget);
            this.Game.GraphicsDevice.Clear(Color.Transparent);

            MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.Immediate);

            MySpriteBatch.Draw(blankHudBgBorderTexture.Texture2D, new Rectangle(0, 0, 205, 62), hudBorderColor);
            MySpriteBatch.Draw(blankHudBgTexture.Texture2D, new Rectangle(0, 0, 205, 62), Color.Black * (150f / 255f));   //175

            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, "Wave", new Vector2(205 * 0.25f, 62 / 2 - 15), Color.White, 0.85f); // 0.5 scale
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, s, new Vector2(205 * 0.25f, 62 / 2 + 10), Color.Red, 1f);    // 0.75 scale


            DrawEnemyRemaining(gameTime);


            MySpriteBatch.End();

            this.Game.GraphicsDevice.SetRenderTarget(null);
            this.Game.GraphicsDevice.Clear(Color.Black);
        }

        /// <summary>
        /// Renders the # of Enemies Remaining.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawEnemyRemaining(GameTime gameTime)
        {
            string s = this.waveManager.CurrentWave.EnemiesRemaining.ToString();

            MySpriteBatch.Draw(enemyIconTexture.Texture2D, new Rectangle((int)(205 * 0.67f) - (30 / 2), 0 + 2, 30, 60 - 4), Color.White);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, s, new Vector2(205f * 0.85f, 62 / 2 + 10), Color.Red, 1.0f);
        }

        #endregion

        #region Draw Score

        /// <summary>
        /// Renders the player's current Score.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawScoreOld(GameTime gameTime)
        {
            string scoreString = TotalScore.ToString("N0");

            // Testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(scoreRenderTarget);
            this.Game.GraphicsDevice.Clear(Color.Transparent);

            MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.Immediate);

            MySpriteBatch.Draw(blankHudBgBorderTexture.Texture2D, new Rectangle(0, 0, 205, 62), hudBorderColor);
            MySpriteBatch.Draw(blankHudBgTexture.Texture2D, new Rectangle(0, 0, 205, 62), Color.Black * (150f / 255f));

            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, "Score", new Vector2(205 / 2, 0 + 15), Color.White, 0.85f);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, scoreString, new Vector2(205 / 2, 62 / 2 + 10), Color.Gold, 1f);

            MySpriteBatch.End();

            this.Game.GraphicsDevice.SetRenderTarget(null);
            this.Game.GraphicsDevice.Clear(Color.Black);
        }

        /// <summary>
        /// Renders the player's current Score.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawScore(GameTime gameTime)
        {
            string scoreString = TotalScore.ToString("N0");

            // Testing render target.
            this.Game.GraphicsDevice.SetRenderTarget(scoreRenderTarget);
            this.Game.GraphicsDevice.Clear(Color.Transparent);

            MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.Immediate);

            MySpriteBatch.Draw(blankHudBgBorderTexture.Texture2D, new Rectangle(0, 0, 205, 62), hudBorderColor);
            MySpriteBatch.Draw(blankHudBgTexture.Texture2D, new Rectangle(0, 0, 205, 62), Color.Black * (150f / 255f));




            float fontHeight = hudFont.SpriteFont.MeasureString("Wave").Y * 0.75f;



            string eh = "Score";
            float scoreWidth = hudFont.SpriteFont.MeasureString(eh).X * 0.75f;
            float scoreValueWidth = hudFont.SpriteFont.MeasureString(scoreString).X * 0.75f;

            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, "Score", new Vector2(2f + (scoreWidth * 0.5f), 15f), Color.White, 0.75f);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, scoreString, new Vector2(205f - (scoreValueWidth * 0.5f) - 2, 15f), Color.Gold, 0.75f);

            string moneySign = "Cash";
            float fontWidth = hudFont.SpriteFont.MeasureString(moneySign).X * 0.75f;

            string moneyValue = this.CurrentPlayer.Money.ToString();
            float moneyWidth = hudFont.SpriteFont.MeasureString(moneyValue).X * 0.75f;


            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, moneySign, new Vector2(2f + (fontWidth / 2f), 10f + (1 * fontHeight)), Color.White, 0.75f);
            TextManager.DrawCenteredTest(false, hudFont.SpriteFont, moneyValue, new Vector2(205f - (moneyWidth / 2f) - 2, 10f + (1 * fontHeight)), Color.SpringGreen, 0.75f);

            MySpriteBatch.End();

            this.Game.GraphicsDevice.SetRenderTarget(null);
            this.Game.GraphicsDevice.Clear(Color.Black);
        }

        #endregion

        #endregion


        #region Unloading Level Content

        public override void UnloadLevelContent()
        {
            this.UnloadContent();
        }

        protected override void UnloadContent()
        {
            if (waveManager != null)
            {
                waveManager.CurrentWave.Dispose();
                waveManager = null;
            }

            //SceneGraphManager.RemoveObjects();

            base.UnloadContent();
        }

        #endregion


        #region Wave Complete Method

        /// <summary>
        /// Called to signal Wave completion.
        /// The Wave Complete screen is presented before progressing to the next Wave.
        /// </summary>
        public void WaveComplete()
        {
            // New
            this.CurrentPlayer.GrenadeList.Clear();
            this.CurrentPlayer.Gun.BulletList.Clear();
            this.CurrentPlayer.Gun.ResetReloading();



            ScreenManager.AddScreen(new WaveCompleteScreen(this, waveManager), EngineCore.ControllingPlayer);

            // Unequip the weapon.
            //thePlayer.UnequipWeapon();

            IsOnWaveCompleteScreen = true;
        }



        public bool IsOnWaveCompleteScreen = false;

        #endregion




        /*
          /// <summary>
          /// Our beloved Merchant's Avatar!
          /// </summary>
          public static Avatar merchantAvatar = new Avatar(EngineCore.Game);
          public static Random Random = new Random();

          public static void DrawMerchant(GameTime gameTime)
          {
               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    merchantAvatar.LightDirection = new Vector3(Random.Next(2), Random.Next(2), Random.Next(2));
                    merchantAvatar.LightColor = new Vector3(Random.Next(10), Random.Next(10), Random.Next(10));
                    merchantAvatar.AmbientLightColor = new Color(Random.Next(255) * 4, Random.Next(255) * 4, Random.Next(255) * 4).ToVector3();
               }
               //merchantAvatar.Update(gameTime);
               merchantAvatar.Draw(gameTime);
          }
          */
    }
}