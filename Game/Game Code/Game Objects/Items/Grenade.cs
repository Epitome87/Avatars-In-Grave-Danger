using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PixelEngine.ResourceManagement;
using PixelEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Audio;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Avatars;

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// Defines a Bullet object.
     /// </remarks>
     public class Grenade : DrawableGameComponent
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
               particle.MaxAge = maxAge;
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
               float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
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
          private Model3D grenadeModel;

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
          public bool RemoveGrenade;

          Vector3 impactPosition3D = new Vector3();

          #endregion


          #region Properties

          /// <summary>
          /// Gets or sets the Model3D of the Gun.
          /// </summary>
          public Model3D Model
          {
               get { return grenadeModel; }
               set { grenadeModel = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Instanciates a Bullet.
          /// </summary>
          /// <param name="game">The current Game instance.</param>
          /// <param name="enemyManager">The EnemyManager to handle this TypingEnemy.</param>
          public Grenade(Game game)
               : base(game)
          {
               Position = new Vector3(0.0f);
               Velocity = new Vector3(0f, 0.1f, 0.2f);
               TargetPosition = new Vector3(0.0f);
               RemoveGrenade = false;


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
               grenadeModel = new Model3D();
               grenadeModel.Model = EngineCore.Content.Load<Model>(@"Models\Weapons\Grenade");

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

          float timeUntilExplosion = 4f;

          float explosionTime = 3f;

          public bool Detonated = false;

          public bool ReadyToKill = false;

          private bool isInAir = true;

          float elapsedTimeInAir = 0f;

          float timeUntilRelease = 0.75f;

          float rotation = 0f;

          Matrix renderMatrix = new Matrix();

          private bool pulledPin = false;

          #region Update

          /// <summary>
          /// Updates the Position of the Bullet.
          /// </summary>
          public void Update(GameTime gameTime, Avatar ownersAvatar)//Player thePlayer)
          {
               Vector3 propScale = new Vector3();
               Quaternion propRotation = new Quaternion();
               Vector3 propTranslation = new Vector3();


               // Do the Pull Pin sound immediately.
               if (!pulledPin)
               {
                    AudioManager.PlayCue("Grenade_PinPull");
                    pulledPin = true;
               }


               timeUntilRelease -= (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (timeUntilRelease <= 0.0f)
               {
                    if (playThrowingSound)
                    {
                         AudioManager.PlayCue("Grenade_Throw");
                         playThrowingSound = false;
                    }
               }

               if (timeUntilRelease <= 0f)
               {
                    if (isInAir)
                    {
                         rotation += 4f;

                         float AG = -0.2f;// -9.8f;

                         elapsedTimeInAir += (float)gameTime.ElapsedGameTime.TotalSeconds;

                         float t = elapsedTimeInAir;

                         Position = new Vector3(Position.X + Velocity.X, Position.Y + Velocity.Y + (0.5f * AG * t * t), Position.Z + Velocity.Z);

                         if (Position.Y < 0.25f)
                         {
                              isInAir = false;
                         }
                    }
               }



               else
               {
                    ownersAvatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Decompose(out propScale, out propRotation, out propTranslation);

                    Position = new Vector3(propTranslation.X, propTranslation.Y - 0.025f, propTranslation.Z);

                    renderMatrix =
                         Matrix.CreateRotationX(MathHelper.ToRadians(propRotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(propRotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(propRotation.Z))
                         * Matrix.CreateTranslation(Position);
               }


               timeUntilExplosion -= (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (timeUntilExplosion <= 0f)
               {
                    if (addExplosion)
                    {
                         ReadyToKill = true;

                         AudioManager.PlayCue("Grenade_Explosion");

                         impactPosition3D = this.Position;
                         Vector2 explosionPosition = GraphicsHelper.ConvertToScreenspaceVector2(Matrix.CreateTranslation(this.Position));

                         AddExplosion(explosionPosition, 30, 1000.0f, 5000.0f, gameTime);
                         addExplosion = false;
                    }
               }

               if (particleList.Count > 0)
               {
                    UpdateParticles(gameTime);
               }



               if (timeUntilExplosion <= 0f)
               {
                    explosionTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (explosionTime <= 0f)
                    {
                         RemoveGrenade = true;
                    }
               }
          }

          private bool playThrowingSound = true;

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Grenade model.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               Matrix mat =
                      Matrix.CreateRotationY(MathHelper.ToRadians(rotation))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(rotation))
                    * Matrix.CreateScale(0.001f * 0.75f)
                    * Matrix.CreateTranslation(Position);

               Matrix mat1 = Matrix.CreateScale(0.001f * 0.75f) * renderMatrix;

               if (timeUntilRelease <= 0f)
               {
                    grenadeModel.DrawModel(mat);
               }

               else
               {
                    grenadeModel.DrawModel(mat1);
               }
               

               DrawExplosion();
          }

          #endregion

     }
}
