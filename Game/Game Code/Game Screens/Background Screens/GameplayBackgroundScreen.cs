#region File Description
//-----------------------------------------------------------------------------
// GameplayBackgroundScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A BackgroundScreen for the Start / Main-Menu in Avatar Typing.
     /// This is a BackgroundScreen to prevent re-loading assets and losing
     /// the state of the camera angles / enemies.
     /// </remarks>
     public class GameplayBackgroundScreen : BackgroundScreen
     {
          #region Fields

          private Texture2D skyTexture;
          private Model3D groundModel = new Model3D();

          //private CinematicEvent triangularZoomEvent;
          private WaveManager waveManager;

          Random random = new Random();
          Vector3 position = new Vector3(0);
          int currentEnemy = 0;

          public static bool isUpdate = true;
          
          // Scene Models.
          private SceneObject graveyardScene = new SceneObject();

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public GameplayBackgroundScreen()
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);
          }

          public override void LoadContent()
          {
               skyTexture = EngineCore.Game.Content.Load<Texture2D>(@"Textures\Backgrounds\Sky_Night");

               Random random = new Random();

               // Load Enemies
               waveManager = new WaveManager(null, 20);

               for (int i = 0; i < 3; i++)
               {
                    waveManager.RemoveCurrentWave();
                    waveManager.StartNextWave();
               }



               /*
               for (int i = currentEnemy; i < currentEnemy + 1; i++)
               {
                    Enemy enemy = new NormalEnemy(position, waveManager.CurrentWave);

                    float randomX = -6f + (float)random.NextDouble() * (2f * 6f);
                    float randomZ = 30 + (float)random.NextDouble() * 5;
                    enemy.WorldPosition = Matrix.CreateTranslation(randomX, 0.0f, randomZ) *
                                             Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) *
                                             Matrix.CreateScale(1f);

                    enemy.Position = new Vector3(randomX, 0, randomZ);

                    enemy.Avatar.LoadRandomAvatar();

                    waveManager.CurrentWave.AddEnemy(enemy);
               }
               */

               // Clear the SceneGraphManager.
               SceneGraphManager.RemoveObjects();

               #region Add the Scene Objects to the Scene Graph

               graveyardScene.Model.LoadContent(@"Models\Graveyard\Graveyard_11-20");//Graveyard_Grass_10-27");

               graveyardScene.Position = new Vector3(0f, -0.2f, 40f);// new Vector3(0, 0, 30);
               graveyardScene.Rotation = new Quaternion(0f, graveyardScene.Rotation.Y, graveyardScene.Rotation.Z, 0f); //-90
               graveyardScene.Rotation = new Quaternion(graveyardScene.Rotation.X, 180f, graveyardScene.Rotation.Z, 0f); // 180
               graveyardScene.Rotation = new Quaternion(graveyardScene.Rotation.X, graveyardScene.Rotation.Y, 0f, 0f);
               graveyardScene.World = Matrix.CreateScale(0.05f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(graveyardScene.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(graveyardScene.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(graveyardScene.Rotation.Z))
                    * Matrix.CreateTranslation(graveyardScene.Position);  // X: Positive = left. Z: 

               graveyardScene.Model.AmbientLightColor = Color.CornflowerBlue;
               graveyardScene.Model.DiffuseColor = new Color(80, 100, 130);// Color.DarkBlue;// Color.CornflowerBlue;

               graveyardScene.Model.AmbientLightColor = Color.Brown;
               graveyardScene.Model.DiffuseColor = new Color(80, 100, 130);
               
               SceneGraphManager.AddObject(graveyardScene);

               #endregion
          }

          /// <summary>
          /// Unloads graphics content for this screen.
          /// </summary>
          public override void UnloadContent()
          {
               //base.UnloadContent();
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the background screen. Unlike most screens, this should not
          /// transition off even if it has been covered by another screen: it is
          /// supposed to be covered, after all! This overload forces the
          /// coveredByOtherScreen parameter to false in order to stop the base
          /// Update method wanting to transition off.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               if (!isUpdate)
                    return;

               if (StartScreen.IsOnStartScreen)
               {
                    waveManager.Update(gameTime);
               }
/*
               if (waveManager.CurrentWave.Size <= 1)
               {
                    Enemy enemy = new NormalEnemy(position, waveManager.CurrentWave);
                    float randomX = -6;
                    int randomSign = random.Next(2);
                    if (randomSign == 1)
                         randomX *= -1f;
                    float randomZ = 30 + (float)random.NextDouble() * 5;
                    enemy.WorldPosition = Matrix.CreateTranslation(randomX, 0.0f, randomZ) *
                                             Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) *
                                             Matrix.CreateScale(1f);

                    enemy.Position = new Vector3(randomX, 0, randomZ);

                    enemy.Avatar.LoadRandomAvatar();
                    waveManager.CurrentWave.AddEnemy(enemy);
               }
               */
               base.Update(gameTime, otherScreenHasFocus, false);
          }

          #endregion

          private Vector3 ambientLightColor = Color.White.ToVector3();
          private Vector3 lightDirection = new Vector3(-1.25f, -0.25f, -1.0f);
          private Vector3 lightColor = Color.CornflowerBlue.ToVector3();

          #region Draw

          /// <summary>
          /// Draws the background screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               if (!isUpdate)
                    return;
           
               // Draw the sky texture.
               Color skyColor = Color.Yellow;

               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    skyColor = new Color(random.Next(255), random.Next(255), random.Next(255));
               }

               MySpriteBatch.Begin();
               MySpriteBatch.Draw(skyTexture, new Rectangle(0, 0, EngineCore.GraphicsDevice.Viewport.Width, EngineCore.GraphicsDevice.Viewport.Height), skyColor);
               MySpriteBatch.End();


               if (AvatarZombieGame.SeizureModeEnabled)
               {
                    foreach (SceneObject sceneObject in SceneGraphManager.sceneObjects)
                    {
                         sceneObject.Model.AmbientLightColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                         sceneObject.Model.SpecularColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                         sceneObject.Model.EmissiveColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                    }
               }

               // Render the Scene.
               SceneGraphManager.Draw(gameTime);

               if (StartScreen.IsOnStartScreen)
               {
                    waveManager.Draw(gameTime);
               }
          }

          #endregion
     }
}
