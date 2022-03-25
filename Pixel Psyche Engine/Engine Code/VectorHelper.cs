using System;
using Microsoft.Xna.Framework;

namespace PixelEngine
{
     /// <summary>
     /// http://www.nfostergames.com/Lessons/VectorMath.htm Source.
     /// </summary>
     public static class VectorHelper
     {
          /// <summary>
          /// Normalizes (finds the Length) of the Vector3.
          /// </summary>
          /// <param name="vector">The Vector3 to normalize.</param>
          /// <returns>The Length of the Vector3.</returns>
          public static float Normalize(Vector3 vector)
          {
               float lengthSqr = ((vector.X * vector.X) + (vector.Y * vector.Y)) + (vector.Z * vector.Z);
               float length = (float)Math.Sqrt((float)lengthSqr);
               float num = 1f / length;
               vector.X *= num;
               vector.Y *= num;
               vector.Z *= num;

               return length;
          } 

          /// <summary>
          /// Returns True if the first Vector3 is larger than the second.
          /// </summary>
          /// <param name="vectorA">The first Vector3.</param>
          /// <param name="vectorB">The second Vector3.</param>
          /// <returns>True if first Vector3 is longer.</returns>
          public static bool FirstVectorLongest(Vector3 vectorA, Vector3 vectorB)
          {
               float lengthSqrA = ((vectorA.X * vectorA.X) + (vectorA.Y * vectorA.Y)) + (vectorA.Z * vectorA.Z);
               float lengthSqrB = ((vectorB.X * vectorB.X) + (vectorB.Y * vectorB.Y)) + (vectorB.Z * vectorB.Z);

               return lengthSqrA > lengthSqrB;
          } 

          /// <summary>
          /// Returns the angle between two factors, in radians.
          /// </summary>
          /// <param name="vectorA">The first Vector3.</param>
          /// <param name="vectorB">The second Vector3.</param>
          /// <returns>Dotproduct, ie angle between the vectors.</returns>
          public static float AngleBetweenTwoVectors(Vector3 vectorA, Vector3 vectorB)
          {
               float dotProduct = 0.0f;
               Vector3.Dot(ref vectorA, ref vectorB, out dotProduct);

               return dotProduct;
          }

          public static double GetSignedAngleBetweenTwoVectors(Vector3 Source, Vector3 Dest, Vector3 DestsRight)
          {
               // We make sure all of our vectors are unit length
               Source.Normalize();
               Dest.Normalize();
               DestsRight.Normalize();

               float forwardDot = Vector3.Dot(Source, Dest);
               float rightDot = Vector3.Dot(Source, DestsRight);

               // Make sure we stay in range no matter what, so Acos doesn't fail later
               forwardDot = MathHelper.Clamp(forwardDot, -1.0f, 1.0f);

               double angleBetween = Math.Acos((float)forwardDot);

               if (rightDot < 0.0f)
                    angleBetween *= -1.0f;

               return angleBetween;
          } 
     }
}
