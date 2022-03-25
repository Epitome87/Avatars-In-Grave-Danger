using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace AvatarsInGraveDanger
{
     public class Pistol : Gun
     {
          public Pistol(Game game, Player owner)
               : base(game, owner)
          {
               this.IsFullyAutomatic = false;
               this.ClipSize = 5;
               this.AmmoRemainingInClip = 5;
               this.RateOfFire = 2f;
          }

          protected override void LoadContent()
          {
               base.LoadContent();

               gunModel = new Model3D();
               gunModel.Model = EngineCore.Content.Load<Model>(@"Models\Weapons\LaserGun");
          }

          protected override void DrawGun()
          {
               // Determine the Position of the gun.
               Vector3 gunPosition = new Vector3(0f, 0.125f - (0.125f / 2.0f), 0.04f);

               float targetRotationZ = -1 * (gunRotationUh);

               gunOwner.Avatar.Rotation = new Vector3(0f, 180f, 0f);
               gunOwner.Avatar.Rotation = new Vector3(gunOwner.Avatar.Rotation.Z, gunOwner.Avatar.Rotation.Y + targetRotationZ, gunOwner.Avatar.Rotation.Z);

               Quaternion rotation = new Quaternion();

               rotation = new Quaternion(90f, rotation.Y, rotation.Z, 0f);      // + Rotate around z
               rotation = new Quaternion(rotation.X, 0f, rotation.Z, 0f);       // - Rotate around X (pts tip down)
               rotation = new Quaternion(rotation.X, rotation.Y, 0f + 5, 0f);   // Rotates around Y

               //gunPosition = new Vector3(-0.05f, 0f, 0f);

               Matrix gunOffset = Matrix.CreateScale(0.0035f * 0.90f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z))
                    * Matrix.CreateTranslation(gunPosition);

               gunModel.DiffuseColor = Color.Silver;
               gunModel.SpecularColor = Color.Silver;


               // Render the Gun model now.
               if (gunOwner.Avatar.BonesInWorldSpace != null)
               {
                    gunOwner.Avatar.UpdateWithoutAnimating();

                    Vector3 propScale = new Vector3();
                    Quaternion propRotation = new Quaternion();
                    Vector3 propTranslation = new Vector3();

                    gunOwner.Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Decompose(out propScale, out propRotation, out propTranslation);

                    Matrix withoutRotation = Matrix.CreateScale(propScale) * Matrix.CreateTranslation(propTranslation);

                    Matrix matrixWithoutTilting = Matrix.CreateScale(0.004f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z))
                         * Matrix.CreateTranslation(gunPosition + propTranslation);

                    gunModel.DrawModel(matrixWithoutTilting);
               }
          }
     }
}
