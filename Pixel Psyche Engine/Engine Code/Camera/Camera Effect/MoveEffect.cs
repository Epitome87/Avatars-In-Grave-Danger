#region File Description
//-----------------------------------------------------------------------------
// MoveEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <remarks>
     /// A CameraEffect which causes the Camera to move / change positions.
     /// </remarks>
     public class MoveEffect : CameraEffect
     {
          #region Fields

          float moveDistance;
          Vector3 startingPosition;
          Vector3 destinationPosition;
          Vector3 moveDistanceVector;

          #endregion

          #region Initialization

          public MoveEffect(float runTime, float distance)
               : base(runTime)
          {
               moveDistance = distance;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position;

               IsRunning = true;
               IsFinished = false;
          }

          public MoveEffect(float runTime, float distanceX, float distanceY, float distanceZ)
               : base(runTime)
          {
               moveDistanceVector = new Vector3(distanceX, distanceY, distanceZ);
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position;

               IsRunning = true;
               IsFinished = false;
          }

          public MoveEffect(float runTime, Vector3 destination)
               : base(runTime)
          {
               destinationPosition = destination;
               startingPosition = CameraManager.ActiveCamera.Position;

               moveDistanceVector = startingPosition - destination;
               elapsedTime = 0.0f;
               remainingTime = runTime;

               IsRunning = true;
               IsFinished = false;
          }

          public MoveEffect(bool fixedLookAt, float runTime, Vector3 destination)
               : base(runTime)
          {
               destinationPosition = destination;
               startingPosition = CameraManager.ActiveCamera.Position;

               moveDistanceVector = startingPosition - destination;
               elapsedTime = 0.0f;
               remainingTime = runTime;

               IsRunning = true;
               IsFinished = false;


               fixedLook = fixedLookAt;
          }

          bool fixedLook = false;

          public override void Reset()
          {
               startingPosition = CameraManager.ActiveCamera.Position;
               moveDistanceVector = startingPosition - destinationPosition;

               elapsedTime = 0.0f;

               // New
               remainingTime = runTime;

               IsRunning = true;
               IsFinished = false;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, Camera camera)
          {
               if (remainingTime < 0.0f)
               {
                    IsFinished = true;

                    // New
                    IsRunning = false;
               }

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));

               CameraManager.ActiveCamera.Position = new Vector3(startingPosition.X - (Step * moveDistanceVector.X),
                                   startingPosition.Y - (Step * moveDistanceVector.Y),
                                   startingPosition.Z - (Step * moveDistanceVector.Z));

               if (fixedLook)
                    CameraManager.ActiveCamera.LookAt = 
                         new Vector3(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.LookAt.Y, CameraManager.ActiveCamera.LookAt.Z);

               remainingTime = runTime - elapsedTime;
          }

          #endregion
     }
}