using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace AvatarsInGraveDanger
{
     public class Revolver : Gun
     {
          public Revolver(Game game, Player owner)
               : base(game, owner)
          {
               this.IsFullyAutomatic = false;
               this.ClipSize = 5;
               this.AmmoRemainingInClip = 5;
               this.RateOfFire = 2f;




               this.GunScale = 0.002f;
               this.GunRotation = new Quaternion(0f, 0f, 0f, 0f);
               this.GunOffsetPosition = new Vector3(-0.075f, 0.25f, 0.25f);         
          }

          protected override void LoadContent()
          {
               base.LoadContent();

               gunModel = new Model3D();
               gunModel.Model = EngineCore.Content.Load<Model>(@"Models\Revolver");

               gunModel.AmbientLightColor = Color.White;
               gunModel.EmissiveColor = Color.White;
          }
     }
}
