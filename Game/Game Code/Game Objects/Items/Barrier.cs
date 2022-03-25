using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PixelEngine.ResourceManagement;
using PixelEngine;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.CameraSystem;

namespace AvatarsInGraveDanger
{
     public class Barrier : DrawableGameComponent
     {
          private GameResourceTexture2D blankTexture;

          private Model3D barrierModel;
          public Vector3 position;
          private float health;

          public Barrier(Game game, Vector3 location)
               : base(game)
          {
               position = location;

               health = 100f;

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
               barrierModel = new Model3D();
               barrierModel.Model = EngineCore.Content.Load<Model>(@"Models\SpikeBarrier");

               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
          }


          public bool IsActive = true;

          #region Update

          /// <summary>
          /// Updates the Position of the Bullet.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               health -= (1f * (float)gameTime.ElapsedGameTime.TotalSeconds);

               MathHelper.Clamp(health, 0f, 100f);

               if (health <= 0f)
               {
                    IsActive = false;
               }
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Barrier model.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               Matrix mat = Matrix.CreateRotationX(MathHelper.ToRadians(-90f)) * Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(position);

               barrierModel.DrawModel(mat);

               DrawHealthMeter(gameTime);
          }

          private void DrawHealthMeter(GameTime gameTime)
          {
               // This will render it around their ankles.
               Matrix renderPosition =
                    Matrix.CreateTranslation(new Vector3(this.position.X, this.position.Y, this.position.Z));

               Vector3 screenSpace = PixelEngine.Graphics.GraphicsHelper.ConvertToScreenspaceVector3(renderPosition);

               // Prevent the Health Meter from being rendered if the Enemy is behind the Camera.
               if (position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    Color healthColor = Color.Green;

                    if (health <= (100f * 0.75f))
                    {
                         healthColor = Color.Yellow;
                    }

                    if (health <= (100f * 0.50f))
                    {
                         healthColor = Color.Red;
                    }


                    Rectangle rect = new Rectangle((int)screenSpace.X, (int)screenSpace.Y, 100, 15);
                    Rectangle rect2 = new Rectangle((int)screenSpace.X, (int)screenSpace.Y, (int)health, 15);

                    GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, rect, 2, Color.White);
                    MySpriteBatch.DrawCentered(blankTexture.Texture2D, rect, Color.Black * 0.5f);
                    MySpriteBatch.DrawCentered(blankTexture.Texture2D, rect2, healthColor);
               }
          }

          #endregion
     }
}
