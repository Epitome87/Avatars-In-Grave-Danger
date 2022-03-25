#region File Description
//-----------------------------------------------------------------------------
// FortressStage.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine.Graphics;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// Defines a FortressStage.
     /// 
     /// The FortressStage object simply serves as an easy means of initializing a set
     /// of SceneObjects and organizing them in a manner that forms a Fortress setting.
     /// 
     /// Like other Stage objects, it will most likely be used within a Level object to 
     /// compose the actual appearance of the Level (but not the Gameplay and logic).
     /// </remarks>
     public class GraveyardStage : Stage
     {
          #region Fields

          // Scene Models.

          private SceneObject graveyardSceneObject = new SceneObject();

          #endregion


          #region Initialization

          public GraveyardStage(Game game)
               : base(game)
          {
               this.QuadOrigin = Vector3.Zero;
               this.QuadNormal = Vector3.Forward;
               this.QuadUp = Vector3.Up;
               this.QuadWidth = 50;
               this.QuadHeight = 50;


               this.QuadOrigin = Vector3.Zero;
               this.QuadNormal = Vector3.Up;// Vector3.Forward;
               this.QuadUp = Vector3.Backward;// Vector3.Up;
               this.QuadWidth = 1;// 540;
               this.QuadHeight = 1;// 180;
          }

          public override void DisposeLevel()
          {
               base.DisposeLevel();
          }

          #endregion


          #region Load Stage's Content

          public override void LoadContent()
          {
               SceneGraphManager.RemoveObjects();

               graveyardSceneObject.Model.LoadContent(@"Models\Graveyard\Graveyard_11-20");//Graveyard_Grass_10-27");

               // FOR SCENEFINAL_A WHICH WORKS
               graveyardSceneObject.Position = new Vector3(0f, -0.2f, 40f);
               graveyardSceneObject.Rotation = new Quaternion(0f, graveyardSceneObject.Rotation.Y, graveyardSceneObject.Rotation.Z, 0f);
               graveyardSceneObject.Rotation = new Quaternion(graveyardSceneObject.Rotation.X, 180f, graveyardSceneObject.Rotation.Z, 0f);
               graveyardSceneObject.Rotation = new Quaternion(graveyardSceneObject.Rotation.X, graveyardSceneObject.Rotation.Y, 0f, 0f);
               graveyardSceneObject.World = Matrix.CreateScale(0.05f)//(0.1f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(graveyardSceneObject.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(graveyardSceneObject.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(graveyardSceneObject.Rotation.Z))
                    * Matrix.CreateTranslation(graveyardSceneObject.Position);

               //graveyardSceneObject.Model.AmbientLightColor = new Color(500, 0, 0);
               //graveyardSceneObject.Model.DiffuseColor = new Color(0, 0, 500);


               //graveyardSceneObject.Model.AmbientLightColor = Color.Beige;
               //graveyardSceneObject.Model.DiffuseColor = new Color(80, 100, 130);

               graveyardSceneObject.Model.AmbientLightColor = Color.Brown;
               graveyardSceneObject.Model.DiffuseColor = new Color(80, 100, 130);


               SceneGraphManager.AddObject(graveyardSceneObject);
          }

          #endregion
     }
}