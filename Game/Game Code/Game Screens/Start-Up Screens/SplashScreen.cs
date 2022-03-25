#region File Description
//-----------------------------------------------------------------------------
// SplashScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// The Splash screen is a simple screen which runs upon game start-up.
     /// The screen does not accept input; it merely displays for a few seconds,
     /// during which time the Studio Logo and Copyright information are displayed.
     /// </summary>
     public class SplashScreen : GameScreen
     {
          #region Fields

          float elapsedTime;
 
          GameResourceTexture2D studioLogo;

          Avatar zombieMattAvatar;

          SceneObject graveModel = new SceneObject();

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public SplashScreen()
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.0f);

               elapsedTime = 0f;
          }

          public override void LoadContent()
          {
               studioLogo = ResourceManager.LoadTexture(@"Studio Logo\Frame0_rescale");

               zombieMattAvatar = new Avatar(EngineCore.Game);
               zombieMattAvatar.LoadAvatar(CustomAvatarType.Matt_Zombiefied);

               zombieMattAvatar.PlayAnimation(AnimationType.ZombieWalk, true);

               zombieMattAvatar.Position = new Vector3(4f, 0f, 0f);
               zombieMattAvatar.Rotation = new Vector3(0f, 90, 0f);
               zombieMattAvatar.Scale = 3.0f;

               // Set up the cool blue lighting!
               zombieMattAvatar.LightDirection = new Vector3(1, 1, 1);
               zombieMattAvatar.LightColor = new Vector3(0, 0, 0);
               zombieMattAvatar.AmbientLightColor = Color.DarkGray.ToVector3();

               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(2, 5, 0);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 1f, 10f);
               CameraManager.ActiveCamera.ViewMatrix = Matrix.CreateLookAt(CameraManager.ActiveCamera.Position, CameraManager.ActiveCamera.LookAt, Vector3.Up);
               CameraManager.ActiveCamera.FieldOfView = 45f;

               graveModel.Model.LoadContent("Grave_Tall");


               Vector3 gravePosition = new Vector3(0f, -0.5f, 2f);

               graveModel.Position = gravePosition;
               graveModel.Rotation = new Quaternion(-90f, graveModel.Rotation.Y, graveModel.Rotation.Z, 0f);
               graveModel.Rotation = new Quaternion(graveModel.Rotation.X, 180f, graveModel.Rotation.Z, 0f);
               graveModel.Rotation = new Quaternion(graveModel.Rotation.X, graveModel.Rotation.Y, 0f, 0f);

               graveModel.World = Matrix.CreateScale(0.03f * 1.10f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(graveModel.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(graveModel.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(graveModel.Rotation.Z))
                    * Matrix.CreateTranslation(gravePosition);

               graveModel.Model.AmbientLightColor = Color.CornflowerBlue;
               graveModel.Model.EmissiveColor = Color.Black;

               SceneGraphManager.AddObject(graveModel);

               AudioManager.PlayMusic("Arcadia");
          }

          #endregion


          #region Handle Input

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               // Don't allow input.
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               //if (!IsActive) return;

               // Update elapsed time counters.
               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               float moveDistance = (float)gameTime.ElapsedGameTime.TotalSeconds;
 
               // After 5 seconds, we want to proceed to the Start Screen.
               if (elapsedTime >= 7.5f)
               {
                    ExitScreen();
                    ScreenManager.AddScreen(new GameplayBackgroundScreen(), ControllingPlayer);
                    ScreenManager.AddScreen(new StartScreen(), ControllingPlayer);
               }

               zombieMattAvatar.Position = new Vector3(zombieMattAvatar.Position.X - moveDistance, zombieMattAvatar.Position.Y, zombieMattAvatar.Position.Z);

               zombieMattAvatar.Update(gameTime);
          }

          #endregion


          #region Draw

          bool playedOnce = false;

          public override void Draw(GameTime gameTime)
          {
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 4, -2);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0.5f, 10f);
               CameraManager.ActiveCamera.ViewMatrix = Matrix.CreateLookAt(CameraManager.ActiveCamera.Position, CameraManager.ActiveCamera.LookAt, Vector3.Up);
               CameraManager.ActiveCamera.FieldOfView = 45f;

               if (elapsedTime >= 3f)
               {
                    if (!playedOnce)
                    {
                         AudioManager.PlayCue("ZombieMattGroan");
                         playedOnce = true;
                    }
               }
               
               SceneGraphManager.Draw(gameTime);

               MySpriteBatch.Begin();

               MySpriteBatch.Draw(studioLogo.Texture2D,
                    new Rectangle((int)EngineCore.ScreenCenter.X - 190, (int)EngineCore.ScreenCenter.Y - 50, 380, 300), Color.Gray);

               MySpriteBatch.End();
               
               zombieMattAvatar.Draw(gameTime);

               

               base.Draw(gameTime);
          }

          #endregion
     }
}
