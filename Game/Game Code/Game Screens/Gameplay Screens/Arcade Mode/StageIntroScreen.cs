#region File Description
//-----------------------------------------------------------------------------
// StageIntroScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine.Screen;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A short introductory screen that displays what level is about to begin.
     /// </summary>
     public class StageIntroScreen : MenuScreen
     {
          #region Fields

          Wave wave;
          ArcadeLevel theArcadeLevel;
          
          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public StageIntroScreen(ArcadeLevel arcadeLevel, Wave _wave)
               : base("")
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.0);
               TransitionOffTime = TimeSpan.FromSeconds(0.0);

               theArcadeLevel = arcadeLevel;
               wave = _wave;
          }

          public override void HandleInput(InputState input, GameTime gameTime)
          {

          }

          public override void LoadContent()
          {
               base.LoadContent();

               theArcadeLevel.CurrentPlayer.Gun.AmmoRemainingInClip = theArcadeLevel.CurrentPlayer.Gun.ClipSize;
     
               theArcadeLevel.CurrentWaveManager.RemoveCurrentWave();
               theArcadeLevel.CurrentWaveManager.StartNextWave();
          }

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
               this.ExitScreen();

               ArcadeGameplayScreen.IntroduceEnemy = true;
          }

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion
     }
}