

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class RayRenderer
{
     static VertexPositionColor[] verts = new VertexPositionColor[2];

     static VertexPositionColor[] arrowVerts = {
            new VertexPositionColor(Vector3.Zero, Color.White),
            new VertexPositionColor(new Vector3(.5f, 0f, -.5f), Color.White),
            new VertexPositionColor(new Vector3(-.5f, 0f, -.5f), Color.White),
            new VertexPositionColor(new Vector3(0f, .5f, -.5f), Color.White),
            new VertexPositionColor(new Vector3(0f, -.5f, -.5f), Color.White),
        };

     static int[] arrowIndexs = {
            0, 1,
            0, 2,
            0, 3,
            0, 4,
        };

     //  static VertexDeclaration vertDecl;
     static BasicEffect effect;

     /// <summary>
     /// Renders a Ray for debugging purposes.
     /// </summary>
     /// <param name="ray">The ray to render.</param>
     /// <param name="length">The distance along the ray to render.</param>
     /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
     /// <param name="view">The current view matrix.</param>
     /// <param name="projection">The current projection matrix.</param>
     /// <param name="color">The color to use drawing the ray.</param>
     public static void Render(Ray ray, float length, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color color)
     {
          if (effect == null)
          {
               effect = new BasicEffect(graphicsDevice);
               effect.VertexColorEnabled = false;
               effect.LightingEnabled = false;
          }

          verts[0] = new VertexPositionColor(ray.Position, Color.White);
          verts[1] = new VertexPositionColor(ray.Position + (ray.Direction * length), Color.White);

          effect.DiffuseColor = color.ToVector3();
          effect.Alpha = (float)color.A / 255f;

          effect.World = Matrix.Identity;
          effect.View = view;
          effect.Projection = projection;

          //note you may wish to comment these next 2 lines out and set the RasterizerState elswehere in code 
          //rather than here for every ray draw call. 
          RasterizerState rs = graphicsDevice.RasterizerState;
          graphicsDevice.RasterizerState = RasterizerState.CullNone;

          foreach (EffectPass pass in effect.CurrentTechnique.Passes)
          {
               pass.Apply();

               graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 1);

               effect.World = Matrix.Invert(Matrix.CreateLookAt(
                   verts[1].Position,
                   verts[0].Position,
                   (ray.Direction != Vector3.Up) ? Vector3.Up : Vector3.Left));

               //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, arrowVerts, 0, 5, arrowIndexs, 0, 4);
          }

          //note you may wish to comment the next line out and set the RasterizerState elswehere in code 
          //rather than here for every ray draw call. 
          graphicsDevice.RasterizerState = rs;
     }
}