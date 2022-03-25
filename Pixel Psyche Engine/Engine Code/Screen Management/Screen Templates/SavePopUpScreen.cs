#region File Description
//-----------------------------------------------------------------------------
// SavePopUpScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
#endregion

namespace PixelEngine
{
     /// <remarks>
     /// A Pop-Up Screen which acts comparable to a typical game's "Now Saving" pop-up.
     /// Although this is a screen, it should be used as if it were a Component.
     /// 
     /// Default behavior: The text "Saving" is rendered onto the top-left of the screen.
     /// A "." is added to the text after each passing second, until finally we end with
     /// "Saving..." and the text disappears.
     /// </remarks>
     public class SavePopUpScreen : MenuScreen
     {
          #region Fields

          // Helper variables used for Update calculations.
          private Vector2 startingPosition = new Vector2(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Left, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);
          private float elapsedTime = 0.0f;

          private Rectangle PopupRectangle = new Rectangle();

          GameResourceTexture2D rectangleBorderTexture;
 
          #endregion

          #region Properties

          #endregion

          private string SavingText = "! Saving";

          #region Initialization

          public SavePopUpScreen(string savingText)
               : base("")
          {
               this.IsOverlayPopup = true;

               SavingText = savingText;
          }

          public override void LoadContent()
          {
               base.LoadContent();

               rectangleBorderTexture = ResourceManagement.ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
          }

          #endregion

          #region Update

          string nowSavingText = ".";

          public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (elapsedTime > 0)
               {
                    nowSavingText = ".";
               }
               if (elapsedTime >= 1)
               {
                    nowSavingText = "..";
               }
               if (elapsedTime >= 2)
               {
                    nowSavingText = "...";
               }

               if (elapsedTime >= 3)
               {
                    //this.ExitPopupScreen();
               }




               if (elapsedTime <= 3f)
               {
                    /*
                    startingPosition.Y += 2;

                    if (startingPosition.Y >= 200)
                         startingPosition.Y = 200;
                    */
               }

               else
               {
                    //startingPosition.Y -= 2;

                    startingPosition.Y -= 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // If the Achievement reaches here, it's time to remove the PopUp Screen.
                    if (startingPosition.Y < -200)
                    {
                         this.ExitPopupScreen();
                    }
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);

               string text = SavingText;// "! Saving...";

               // Width and Height of the Award string.
               Vector2 widthHeight = ScreenManager.Font.MeasureString(text);

               // Rectangle for Save Popup (rectangular border for now). Should be larger than the previous value.
               Rectangle awardPopupRect = 
                    new Rectangle((int)startingPosition.X, (int)startingPosition.Y, (int)widthHeight.X + 20, (int)widthHeight.Y + 20);


               // The center of the text: Simply the center of the rectangle.
               Vector2 textCenter = new Vector2(awardPopupRect.Center.X, awardPopupRect.Center.Y);

               MySpriteBatch.Begin();

               // Draw the rectangular border behind the text.
               MySpriteBatch.Draw(rectangleBorderTexture.Texture2D, awardPopupRect, Color.Black * (125f / 255f));

               // Draw the "! Saving" text.
               TextManager.DrawCentered(false, ScreenManager.Font, SavingText + nowSavingText, textCenter, Color.Gold, 0.75f);

               MySpriteBatch.End();
          }

          #endregion
     }
}