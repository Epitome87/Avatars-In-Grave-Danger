#region File Description
//-----------------------------------------------------------------------------
// AwardPopUpScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine.AchievementSystem;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine
{
     /// <remarks>
     /// A Pop-Up Screen which acts comparable to the 360's "Achievement Unlocked" pop-up.
     /// Although this is a screen, it should be used as if it were a Component. 
     /// 
     /// Default behavior: The Award Unlocked icon flies in from the left until it 
     /// arrives at its destination. A short sound is displayed with the icon, along
     /// with "Award Unlocked: 'Name of Award'". The icon disappears after 5 seconds.
     /// </remarks>
     public class AwardPopUpScreen : MenuScreen
     {
          #region Fields

          private Achievement award;

          private GameResourceTexture2D rectangleTexture;
          private GameResourceTexture2D awardImageTexture;

          private float elapsedTime = 0.0f;
          private float awardDisplayTime = 5.0f;

          private string awardPopSound = "AwardPop";
          private Color awardBackdropColor = Color.Black * (150f / 255f);
          private Color awardTextColor = Color.White * (225f / 255f);
          private float awardScale = 0.6f;//0.75f;

          // Helper variables used for Update calculations.
          private Vector2 startingPosition = new Vector2(-500, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);//EngineCore.GraphicsDevice.Viewport.Height - 200);
          private bool playSound = true;
          private Rectangle achievementRectangle = new Rectangle();
          private Vector2 widthHeight;
          private string awardPopupText;
          private Vector2 textCenter;
          private Rectangle awardImageRect;
          private Vector2 distanceToTravel;

          #endregion


          #region Properties

          /// <summary>
          /// The time (seconds) the Award Pop-Up is displayed for.
          /// </summary>
          public float AwardDisplayTime
          {
               get { return awardDisplayTime; }
               set { awardDisplayTime = value; }
          }

          /// <summary>
          /// The sound (cue name) heard when an Award "Pops".
          /// </summary>
          public string AwardPopSound
          {
               get { return awardPopSound; }
               set { awardPopSound = value; }
          }

          /// <summary>
          /// The Color of the Award Pop-Up backdrop.
          /// </summary>
          public Color AwardBackdropColor
          {
               get { return awardBackdropColor; }
               set { awardBackdropColor = value; }
          }

          /// <summary>
          /// The Color of the Award Pop-Up text.
          /// </summary>
          public Color AwardTextColor
          {
               get { return awardTextColor; }
               set { awardTextColor = value; }
          }

          /// <summary>
          /// The Scale to render the Award Pop-Up in.
          /// </summary>
          public float AwardScale
          {
               get { return awardScale; }
               set { awardScale = value; }
          }

          #endregion

          SpriteFont font;

          #region Initialization

          /// <summary>
          /// AwardPopUpScreen Constructor.
          /// </summary>
          /// <param name="awardObtained">The Achievement that was obtained / will pop up.</param>
          public AwardPopUpScreen(Achievement awardObtained)
               : base("")
          {
               font = TextManager.Fonts[(int)FontType.HudFont].SpriteFont;

               this.IsOverlayPopup = true;

               award = awardObtained;

               // Tell the AchievementManager that this pop-up screen is now active.
               AchievementManager.isPopUpOnScreen = true;

               awardPopupText = "AWARD UNLOCKED\n" + award.PointValue + " - " + award.Title;

               // Width and Height of the Award string.
               widthHeight = font.MeasureString(awardPopupText) * awardScale;

               // Rectangle for Award Popup (rectangular border for now). Should be larger than the previous value.
               achievementRectangle = new Rectangle((int)startingPosition.X, (int)startingPosition.Y,
                    (int)widthHeight.X + 20 + 75, (int)widthHeight.Y + 20);

               // The center point the Award string should be rendered at: StartingPosition + Half of AwardPopup.Width
               textCenter = new Vector2(achievementRectangle.Center.X + (75 / 2), (float)(achievementRectangle.Center.Y));// + achievementRectangle.Height / 8));

               // Rectangle for Award Image.
               awardImageRect = new Rectangle((int)startingPosition.X + 5, achievementRectangle.Y, achievementRectangle.Height - 10, achievementRectangle.Height - 10);

               startingPosition.Y = (EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);// (EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - achievementRectangle.Height);

               distanceToTravel = new Vector2(EngineCore.GraphicsDevice.Viewport.Width - achievementRectangle.Width - 100 + 500, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);//EngineCore.GraphicsDevice.Viewport.Height - 200);
          }

          /// <summary>
          /// LoadContent Override.
          /// Loads assets necessary for popping our style of Achievements:
          /// A border Texture, a trophy Texture, etc.
          /// </summary>
          public override void LoadContent()
          {
               base.LoadContent();

               rectangleTexture = ResourceManagement.ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
               awardImageTexture = ResourceManagement.ResourceManager.LoadTexture(@"Achievements\Award"); //this.award.Picture; 

               AudioManager.PlayCue(awardPopSound);
          }

          #endregion


          #region Handle Input

          /// <summary>
          /// HandleInput Override.
          /// Does nothing, as this is not an interactive screen.
          /// </summary>
          /// <param name="input"></param>
          public override void HandleInput(InputState input, GameTime gameTime)
          {
               // Do not allow input; it's just a pop-up!
          }

          #endregion


          #region Update

          /// <summary>
          /// Update Override.
          /// 
          /// Updates the Pop-Up Screen logic, such as updating the position of the Achievement earned,
          /// playing a sound as the Achievement flies into place, and making it fly off screen when it
          /// has been displayed for a sufficient amount of time.
          /// </summary>
          /// <param name="gameTime">GameTime.</param>
          /// <param name="otherScreenHasFocus">Whether or not another Screen has focus.</param>
          /// <param name="coveredByOtherScreen">Whether or not this screen is covered by another.</param>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               // If we haven't surpassed our designated Award Display Time (5 seconds by default)...
               if (elapsedTime <= awardDisplayTime)
               {
                    // Increase the X position of the Award. This rate insures we reach our Destination at 25% of our Display Time.
                    startingPosition.X += distanceToTravel.X / (awardDisplayTime * 0.25f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // If we are at our Destination, ensure we stay there, giving the player time to read the Award.
                    if (startingPosition.X >= (EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Right - achievementRectangle.Width))
                         startingPosition.X = (EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Right - achievementRectangle.Width);
               }

               // Otherwise, we have surpasses our designated Award Display Time.
               else
               {
                    // If our Play Sound flag is true...
                    if (playSound)
                    {
                         // ...Play the Award Pop Sound!
                         AudioManager.PlayCue(awardPopSound);
                    }

                    // Reset the flag: We only want the sound to play once.
                    playSound = false;

                    // Move the Award off the screen, 300 pixels per second.
                    startingPosition.X += 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // If the Achievement reaches here, it's time to remove the PopUp Screen.
                    if (startingPosition.X > EngineCore.GraphicsDevice.Viewport.Width + 100)
                    {
                         AchievementManager.isPopUpOnScreen = false;
                         this.ExitPopupScreen();
                    }
               }


               // Rectangle for Award Popup (rectangular border for now).
               achievementRectangle.X = (int)startingPosition.X;
               achievementRectangle.Y = (int)startingPosition.Y;

               // Update our award image rectangle.
               awardImageRect.X =  (int)startingPosition.X + 6;
               awardImageRect.Y = (int)startingPosition.Y + 5;

               // Update the center point the Award string should be rendered at: StartingPosition + Half of AwardPopup.Width
               textCenter.X = achievementRectangle.Center.X + (75 / 2);
               textCenter.Y = (float)(achievementRectangle.Center.Y);// + achievementRectangle.Height / 8);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);

               MySpriteBatch.Begin();

               GraphicsHelper.DrawBorderFromRectangle(rectangleTexture.Texture2D, achievementRectangle, 2, Color.CornflowerBlue);

               // Draw the blank rectangular texture.
               MySpriteBatch.Draw(rectangleTexture.Texture2D, achievementRectangle, awardBackdropColor);

               // Draw the texture representing the award unlocked.
               MySpriteBatch.Draw(awardImageTexture.Texture2D, awardImageRect, Color.White * (255f / 255f));

               // Lastly, draw the "Award Unlocked!" text, along with the name of the award.
               TextManager.DrawCentered(false, font, awardPopupText, textCenter, awardTextColor, awardScale);
               
               MySpriteBatch.End();
          }

          #endregion
     }
}