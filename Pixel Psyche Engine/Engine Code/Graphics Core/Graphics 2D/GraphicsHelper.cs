#region File Description
//-----------------------------------------------------------------------------
// GraphicsHelper.cs
// Matt McGrath.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine.CameraSystem;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// A static helper class which aids in commonly used Graphics-related tasks.
     /// </summary>
     public static class GraphicsHelper
     {
          #region World-To-Screenspace conversions.

          /// <summary>
          /// Convert a World Position Matrix into 2D Screenspace.
          /// </summary>
          public static Vector2 ConvertToScreenspaceVector2(Matrix worldPosition)
          {
               // Calculate screenspace of space position.
               Vector3 screenSpace3D = EngineCore.Game.GraphicsDevice.Viewport.Project(Vector3.Zero,
                                                                         CameraManager.ActiveCamera.ProjectionMatrix,
                                                                         CameraManager.ActiveCamera.ViewMatrix,
                                                                         worldPosition);


               Vector2 screenSpace2D = new Vector2();

               // Get 2D position from screenspace vector.
               screenSpace2D.X = screenSpace3D.X;
               screenSpace2D.Y = screenSpace3D.Y;

               return screenSpace2D;
          }

          /// <summary>
          /// Converts a World Position Matrix into a 3D Screenspace.
          /// Just ignore the Vector3.Z result if needed.
          /// </summary>
          public static Vector3 ConvertToScreenspaceVector3(Matrix worldPosition)
          {
               // Calculate screenspace of space position.
               Vector3 screenSpace3D = EngineCore.Game.GraphicsDevice.Viewport.Project(Vector3.Zero,
                                                                         CameraManager.ActiveCamera.ProjectionMatrix,
                                                                         CameraManager.ActiveCamera.ViewMatrix,
                                                                         worldPosition);

               return screenSpace3D;
          }


          // NOT USEABLE YET
          public static Matrix ConvertToWorldspace(Vector3 min, Vector3 max, Matrix world)
          {
                Vector3 pos1 = EngineCore.Game.GraphicsDevice.Viewport.Unproject(
                     new Vector3(min.X, min.Y, 0), CameraManager.ActiveCamera.ProjectionMatrix, CameraManager.ActiveCamera.ViewMatrix, world);
                
               Vector3 pos2 = EngineCore.Game.GraphicsDevice.Viewport.Unproject(
                    new Vector3(max.X, max.Y, 1), CameraManager.ActiveCamera.ProjectionMatrix, CameraManager.ActiveCamera.ViewMatrix, world);
                
               Vector3 dir = Vector3.Normalize(pos2 - pos1);

               return new Matrix();
          }

          public static void DrawBorderFromRectangle(Texture2D texture, Rectangle rect, int borderSize, Color color)
          {
               Rectangle left = new Rectangle(rect.X - borderSize, rect.Y - borderSize, borderSize, rect.Height + (borderSize * 2));
               Rectangle right = new Rectangle(rect.Right, rect.Y - borderSize, borderSize, rect.Height + (borderSize * 2));
               Rectangle top = new Rectangle(rect.X - borderSize, rect.Y - borderSize, rect.Width + (borderSize * 2), borderSize);
               Rectangle bottom = new Rectangle(rect.X - borderSize, rect.Bottom, rect.Width + (borderSize * 2), borderSize);

               MySpriteBatch.Draw(texture, left, null, color);
               MySpriteBatch.Draw(texture, right, null, color);
               MySpriteBatch.Draw(texture, top, null, color);
               MySpriteBatch.Draw(texture, bottom, null, color);
          }

          public static void DrawBorderCenteredFromRectangle(Texture2D texture, Rectangle rectangle, int borderSize, Color color)
          {
               Rectangle tempRectangle = rectangle;

               tempRectangle.X = (int)(rectangle.X - (rectangle.Width / 2f));
               tempRectangle.Y = (int)(rectangle.Y - (rectangle.Height / 2f));

               DrawBorderFromRectangle(texture, tempRectangle, borderSize, color);
          }

          #endregion
     }
}