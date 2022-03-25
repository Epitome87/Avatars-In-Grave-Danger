using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;

namespace AvatarsInGraveDanger
{
     public class Uzi : Gun
     {
          public Uzi(Game game, Player owner)
               : base(game, owner)
          {
               this.IsFullyAutomatic = true;
               this.ClipSize = 16;
               this.AmmoRemainingInClip = 16;
               this.RateOfFire = 7.5f;
               this.gunFiredSoundName = "Uzi_Fire";
               this.gunReloadingSoundName = "Uzi_Reload";
               this.BulletDamage = 0.5f;

               this.TotalAmmo = 128;
               this.isInfiniteAmmo = false;


               this.GunScale = 0.004f;
               this.GunRotation = new Quaternion(0f, 90f, 0f, 0f);
               this.GunOffsetPosition = new Vector3(-0.1f, 0f, -0.05f);
          }

          protected override void LoadContent()
          {
               base.LoadContent();

               gunModel = new Model3D();
               gunModel.Model = EngineCore.Content.Load<Model>(@"Models\Weapons\Uzi");
          }
     }
}
