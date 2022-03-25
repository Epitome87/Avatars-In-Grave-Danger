#region File Description
//-----------------------------------------------------------------------------
// AvatarTypingGameSettings.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine;
#endregion

namespace AvatarsInGraveDanger
{
     #region Difficulty Enum

     public enum Difficulty
     {
          Normal,
          Hard,
     }

     #endregion

     /// <remarks>
     /// Inherits from GameSettings to expand on "game settings"
     /// that are not generic enough to be defined within GameSettings.
     /// </remarks>
     //[Serializable]
     public class ZombieGameSettings : PixelEngine.GameSettings
     {
          #region Fields

          private static Difficulty difficulty = Difficulty.Normal;

          private static int soundVolume = 5;//8;
          private static int musicVolume = 5;//8;

          private static bool vibrationEnabled = true;


          private static Rectangle customTitleSafeArea = 
               new Rectangle(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Y, 
                    EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Width, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Height);


          public static Rectangle CustomTitleSafeArea
          {
               get { return customTitleSafeArea; }
               set { customTitleSafeArea = value; }
          }

          #endregion

          #region Properties

          public static bool VibrationEnabled
          {
               get { return vibrationEnabled; }
               set { vibrationEnabled = value; }
          }


          /// <summary>
          /// Gets or sets the Enum Difficulty of the game.
          /// </summary>
          public static Difficulty Difficulty
          {
               get { return difficulty; }
               set { difficulty = value; }
          }

          public static int SoundVolume
          {
               get { return soundVolume; }
               set { soundVolume = value; }
          }

          public static int MusicVolume
          {
               get { return musicVolume; }
               set { musicVolume = value; }
          }

          #endregion
     }
}