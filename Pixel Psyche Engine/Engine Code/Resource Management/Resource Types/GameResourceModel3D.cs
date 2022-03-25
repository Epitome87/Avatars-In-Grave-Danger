
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
#endregion

namespace PixelEngine.ResourceManagement
{
     public class GameResourceModel3D : GameResourceBase
     {
          #region Fields

          private Model3D model3D = new Model3D();

          #endregion

          #region Properties

          public Model3D Model3D
          {
               get { return model3D; }

               set { model3D = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="key">key name</param>
          /// <param name="assetName">asset name</param>
          /// <param name="resource">model resource</param>
          public GameResourceModel3D(string key, string assetName, Model resource)
               : base(key, assetName)
          {
               this.Model3D.Model = resource;

               this.resource = (object)this.Model3D.Model;
          }

          #endregion

          #region Disposal

          protected override void Dispose(bool disposing)
          {
               if (disposing)
               {
                    if (this.Model3D != null)
                    {
                         this.Model3D.Model = null;
                         this.Model3D = null;
                    }
               }

               base.Dispose(disposing);
          }

          #endregion

          #region Public Draw Methods

          /// <summary>
          /// Draw the Model3D at the specified World Matrix.
          /// </summary>

          public void DrawModel(Matrix worldMatrix)
          {
               this.Model3D.DrawModel(worldMatrix);
          }

          #endregion
     }
}