#region File Description
//-----------------------------------------------------------------------------
// ControlsMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Menu screen that displays the Controls.
     /// </remarks>
     public class ControlsMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry otherControlsMenuEntry;
          MenuEntry backMenuEntry;

          GameResourceTexture2D controlsTexture;

          bool showKeyboardControls = false;

          #endregion

          
          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public ControlsMenuScreen()
               : base("C O N T R O L S")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               this.numberOfColumns = 2;

               // Create our menu entries.
               otherControlsMenuEntry = new MenuEntry("Keyboard Controls");
               backMenuEntry = new MenuEntry("Back");

               backMenuEntry.DescriptionPosition = new Vector2(backMenuEntry.DescriptionPosition.X, backMenuEntry.DescriptionPosition.Y + 40f);
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X - 225, backMenuEntry.DescriptionPosition.Y - 160f);

               otherControlsMenuEntry.DescriptionPosition = new Vector2(otherControlsMenuEntry.DescriptionPosition.X, otherControlsMenuEntry.DescriptionPosition.Y + 40f);
               otherControlsMenuEntry.Position = new Vector2(otherControlsMenuEntry.Position.X - 225, otherControlsMenuEntry.DescriptionPosition.Y - 160f);//165f);

               // Hook up menu event handlers.
               otherControlsMenuEntry.Selected += OtherControlsMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               //MenuEntries.Add(otherControlsMenuEntry);
               //MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 5;

                    entry.menuEntryBorderSize = new Vector2(425, 100);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;

                    entry.ShowDescriptionBorder = false;
               }

               backMenuEntry.IsPulsating = true;
          }

          public override void LoadContent()
          {
               base.LoadContent();

               controlsTexture = ResourceManager.LoadTexture(@"Menus\Controller");
          }

          #endregion


          #region Menu Entry Events

          void OtherControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               showKeyboardControls = !showKeyboardControls;

               if (showKeyboardControls)
               {
                    otherControlsMenuEntry.Text = "Gamepad Controls";
                    //otherControlsMenuEntry.Description = "View the Controls for the 360 Gamepad";
               }

               else
               {
                    otherControlsMenuEntry.Text = "Keyboard Controls";
                    //otherControlsMenuEntry.Description = "View the Controls for a USB Keyboard";
               }
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               //ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               base.Draw(gameTime);

               MySpriteBatch.Begin();

               Color color = Color.White * (TransitionAlpha / 255f);

               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle((int)(EngineCore.GraphicsDevice.Viewport.Width * 0.20f), (int)(EngineCore.GraphicsDevice.Viewport.Height * 0.30f),
                                   (int)(EngineCore.GraphicsDevice.Viewport.Width * 0.60f), (int)(EngineCore.GraphicsDevice.Viewport.Height * 0.40f)), Color.Black * 0.5f * (TransitionAlpha / 255f));

               MySpriteBatch.Draw(controlsTexture.Texture2D, new Rectangle((int)(EngineCore.GraphicsDevice.Viewport.Width * 0.20f), (int)(EngineCore.GraphicsDevice.Viewport.Height * 0.20f),
                                   (int)(EngineCore.GraphicsDevice.Viewport.Width * 0.60f), (int)(EngineCore.GraphicsDevice.Viewport.Height * 0.60f)), color);

               MySpriteBatch.End();
          }

          #endregion
     }
}
