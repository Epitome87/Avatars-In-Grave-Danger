#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
// Matt McGrath, with help provided by:
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Text;
#endregion

namespace PixelEngine.Screen
{
     /// <remarks>
     /// Base class for screens that contain a menu of options. The user can
     /// move up and down to select an entry, or cancel to back out of the screen.
     /// </remarks>
     public abstract class AdvancedMenuScreen : GameScreen
     {
          #region Menu Header Structure

          /// <summary>
          /// This is a structure which holds information for
          /// stuff that would appear at the top of a Menu Screen.
          /// </summary>
          public struct MenuHeader
          {
               /// <summary>
               /// The Text defining the Menu Header.
               /// </summary>
               public string Text;

               /// <summary>
               /// The Position of the Menu Header.
               /// </summary>
               public Vector2 Position;

               /// <summary>
               /// The FontType used to render the Text of the Menu Header.
               /// </summary>
               public FontType FontType;

               /// <summary>
               /// The scale for the Font used in rendering the Text of the Menu Header.
               /// </summary>
               public float FontScale;

               /// <summary>
               /// The color for the Font used in rendering the Text of the Menu Header.
               /// </summary>
               public Color Color;

               /// <summary>
               /// The Text Object used to encapsulate the appearance and
               /// behavior of how the Menu Header will be rendered. 
               /// </summary>
               private TextObject MenuHeaderTextObject;

               /// <summary>
               /// Menu Header Constructor.
               /// </summary>
               /// <param name="text">The Text to be displayed on the Menu Header.</param>
               /// <param name="position">The Position of the Text to be displayed on the Menu Header.</param>
               /// <param name="fontType">The FontType of the Text to be displayed on the Menu Header.</param>
               /// <param name="fontScale">The scale the Text to be displayed on the Menu Header.</param>
               /// <param name="color">The color of the Text to be displayed on the Menu Header.</param>
               public MenuHeader(string text, Vector2 position, FontType fontType, float fontScale, Color color)
               {
                    Text = text;
                    Position = position;
                    FontType = fontType;
                    FontScale = fontScale;
                    Color = color;

                    MenuHeaderTextObject = new TextObject(Text, Position, FontType, Color, 0f, Vector2.Zero, FontScale, true);
               }


               public void Draw(GameTime gameTime, float transitionAlpha, float transitionOffset, GameResourceTexture2D borderTexture)
               {
                    // Calculate the position and color of the Text based on transitioning.
                    Color titleColor = Color * (transitionAlpha / 255f);

                    Vector2 titlePosition = Position;
                    titlePosition.Y -= (transitionOffset * 512);

                    Rectangle menuHeaderRect = new Rectangle(0, (int)titlePosition.Y, EngineCore.GraphicsDevice.Viewport.Width,
                         (int)(TextManager.Fonts[(int)FontType].SpriteFont.MeasureString(Text).Y * FontScale));


                    MySpriteBatch.Draw(borderTexture.Texture2D, menuHeaderRect, Color.Black * (150f / 255f));

                    MenuHeaderTextObject.Position = titlePosition;
                    MenuHeaderTextObject.Color = titleColor;
                    MenuHeaderTextObject.Origin = new Vector2(MenuHeaderTextObject.Font.MeasureString(Text).X / 2, 0);

                    // Render the Title Text Object.
                    MenuHeaderTextObject.Draw(gameTime);
               }
          }

          #endregion


          #region Menu Footer Structure

          /// <summary>
          /// This is a structure which holds information for
          /// stuff that would appear at the bottom of a Menu Screen.
          /// </summary>
          public struct MenuFooter
          {
               /// <summary>
               /// The Text defining the Menu Footer.
               /// </summary>
               public string Text;

               /// <summary>
               /// The Position of the Menu Footer.
               /// </summary>
               public Vector2 Position;

               /// <summary>
               /// The FontType used to render the Text of the Menu Footer.
               /// </summary>
               public FontType FontType;

               /// <summary>
               /// The scale for the Font used in rendering the Text of the Menu Footer.
               /// </summary>
               public float FontScale;

               /// <summary>
               /// The color for the Font used in rendering the Text of the Menu Footer.
               /// </summary>
               public Color Color;

               /// <summary>
               /// The Text Object used to encapsulate the appearance and
               /// behavior of how the Menu Footer will be rendered. 
               /// </summary>
               private TextObject MenuFooterTextObject;

               public MenuFooter(string text, Vector2 position, FontType fontType, float fontScale, Color color)
               {
                    Text = text;
                    Position = position;
                    FontType = fontType;
                    FontScale = fontScale;
                    Color = color;

                    MenuFooterTextObject = new TextObject(Text, Position, FontType, Color, 0f, Vector2.Zero, FontScale, true);
               }

               public void Draw(GameTime gameTime, float transitionAlpha, float transitionOffset)
               {
                    // Calculate the position and color of the Text based on transitioning.
                    Color titleColor = Color * (transitionAlpha / 255f);

                    Vector2 titlePosition = Position;
                    titlePosition.Y += (transitionOffset * 512);


                    MenuFooterTextObject.Position = titlePosition;
                    MenuFooterTextObject.Color = titleColor;
                    MenuFooterTextObject.Origin = new Vector2(MenuFooterTextObject.Font.MeasureString(Text).X / 2, 0);

                    // Render the Title Text Object.
                    MenuFooterTextObject.Draw(gameTime);
               }
          }

          #endregion


          #region Menu Description Structure

          /// <summary>
          /// This is a structure which holds information for
          /// stuff that would appear as a "Menu Description".
          /// </summary>
          public struct MenuDescription
          {
               /// <summary>
               /// The Text defining the Menu Description.
               /// </summary>
               public string Text;

               /// <summary>
               /// The Position of the Menu Description.
               /// </summary>
               public Vector2 Position;

               /// <summary>
               /// The FontType used to render the Text of the Menu Description.
               /// </summary>
               public FontType FontType;

               /// <summary>
               /// The scale for the Font used in rendering the Text of the Menu Description.
               /// </summary>
               public float FontScale;

               /// <summary>
               /// The color for the Font used in rendering the Text of the Menu Description.
               /// </summary>
               public Color Color;

               /// <summary>
               /// The Text Object used to encapsulate the appearance and
               /// behavior of how the Menu Description will be rendered. 
               /// </summary>
               private TextObject MenuDescriptionTextObject;

               public MenuDescription(string text, Vector2 position, FontType fontType, float fontScale, Color color)
               {
                    Text = text;
                    Position = position;
                    FontType = fontType;
                    FontScale = fontScale;
                    Color = color;

                    MenuDescriptionTextObject = new TextObject(Text, Position, FontType, Color, 0f, Vector2.Zero, FontScale, true);
               }

               public void Draw(string descriptionString, GameTime gameTime, float transitionAlpha, float transitionOffset)
               {
                    // Calculate the position and color of the Text based on transitioning.
                    Color descriptionColor = Color * (transitionAlpha / 255f);

                    Vector2 descriptionPosition = Position;

                    // Transition from the bottom-up.
                    descriptionPosition.Y += (transitionOffset * 512);

                    MenuDescriptionTextObject.Text = descriptionString;
                    MenuDescriptionTextObject.Position = descriptionPosition;
                    MenuDescriptionTextObject.Color = descriptionColor;
                    MenuDescriptionTextObject.Origin = new Vector2(MenuDescriptionTextObject.Font.MeasureString(Text).X / 2, 0);

                    // Render the Title Text Object.
                    MenuDescriptionTextObject.Draw(gameTime);
               }
          }

          #endregion


          #region Fields

          /// <summary>
          /// Blank texture used to highlight Selected Entries.
          /// </summary>
          protected GameResourceTexture2D blankTexture;

          /// <summary>
          /// The list of menu entries found within this menu screen.
          /// </summary>
          protected List<AdvancedMenuEntry> menuEntries = new List<AdvancedMenuEntry>();

          /// <summary>
          /// The Menu Screen's "Menu Header".
          /// 
          /// This is a structure which holds information for
          /// stuff that would appear at the top of a Menu Screen.
          /// </summary>
          protected MenuHeader menuHeader = new MenuHeader();

          /// <summary>
          /// The Menu Screen's "Menu Footer".
          /// 
          /// This is a structure which holds information
          /// for stuff that would appear at the bottom of a Menu Screen.
          /// </summary>
          protected MenuFooter menuFooter = new MenuFooter();

          /// <summary>
          /// The Menu Screen's "Menu Description".
          /// 
          /// This is a structure which holds information for stuff
          /// that would appear in a Menu Screen's "Description" area.
          /// </summary>
          protected MenuDescription menuDescription = new MenuDescription();







          public MenuDescription Description
          {
               get { return menuDescription; }
          }

          /// <summary>
          /// The string for the sound cue asset which plays when
          /// a Menu Screen receives a menu scroll input.
          /// </summary>
          protected string menuScrollSound = "MenuScroll_Swish";

          /// <summary>
          /// The string for the sound cue asset which plays when
          /// a Menu Screen receives a button press input.
          /// </summary>
          protected string buttonPressSound = "ButtonPress";

          /// <summary>
          /// The 0-based index of the Menu Entry that is currently selected.
          /// </summary>
          protected int selectedEntryIndex = 0;

          /// <summary>
          /// The number of columns the Menu Entries in this Screen use.
          /// </summary>
          protected int numberOfColumns = 1;
          protected float spacingBetweenColumns = 485;


          public static Vector2 DefaultMenuTitlePosition
          {
               get
               {
                    return new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);
               }
          }

          public static Vector2 DefaultMenuEntryStartPosition
          {
               get
               {
                    return new Vector2(0.0f, EngineCore.GraphicsDevice.Viewport.Height * 0.175f);
               }
          }

          public static Vector2 DefaultDescriptionPosition
          {
               get
               {
                    return new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.Height - (EngineCore.GraphicsDevice.Viewport.Height * 0.175f));
               }
          }


          #endregion


          #region Properties

          /// <summary>
          /// Gets the list of menu entries, so derived classes can add
          /// or change the menu contents.
          /// </summary>
          protected IList<AdvancedMenuEntry> MenuEntries
          {
               get { return menuEntries; }
          }

          /// <summary>
          /// Get or Set the 0-indexed number representing which
          /// menu entry index is currently selected.
          /// </summary>
          protected int SelectedMenuEntryIndex
          {
               get { return selectedEntryIndex; }
          }

          /// <summary>
          /// Gets the Menu Entry that is currently "Selected."
          /// </summary>
          public AdvancedMenuEntry SelectedMenuEntry
          {
               get { return MenuEntries[selectedEntryIndex]; }
          }

          /// <summary>
          /// Get or Set the sound (asset name) that plays when
          /// the menu is scrolled through.
          /// </summary>
          protected string MenuScrollSound
          {
               get { return menuScrollSound; }
               set { menuScrollSound = value; }
          }

          /// <summary>
          /// Get or Set the sound (asset name) that plays when
          /// a button is pressed.
          /// </summary>
          protected string ButtonPressSound
          {
               get { return buttonPressSound; }
               set { buttonPressSound = value; }
          }

          public float SpacingBetweenColumns
          {
               get { return spacingBetweenColumns; }
               set { spacingBetweenColumns = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public AdvancedMenuScreen(string menuTitle)
          {
               menuHeader.Text = menuTitle;
               menuFooter.Text = "PRETEND PAGE NUMBER HERE";

               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.0f);
          }

          public override void LoadContent()
          {
               base.LoadContent();

               AdvancedMenuEntry.LoadContent();

               // Blank texture used to highlight Selected Entries.
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");
          }

          public override void UnloadContent()
          {

               base.UnloadContent();
          }

          #endregion


          #region Handle Input

          /// <summary>
          /// Responds to user input, changing the selected entry and accepting
          /// or cancelling the menu.
          /// </summary>
          public override void HandleInput(InputState input, GameTime gameTime)
          {
               // Accept or cancel the menu? We pass in our ControllingPlayer, which may
               // either be null (to accept input from any player) or a specific index.
               // If we pass a null controlling player, the InputState helper returns to
               // us which player actually provided the input. We pass that through to
               // OnSelectEntry and OnCancel, so they can tell which player triggered them.
               PlayerIndex playerIndex;

               // Move to the previous menu entry?
               if (input.IsMenuUp(ControllingPlayer, out playerIndex))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    selectedEntryIndex--;

                    if (selectedEntryIndex < 0)
                    {
                         if (menuEntries.Count == 0)
                              selectedEntryIndex = 0;

                         else
                              selectedEntryIndex = menuEntries.Count - 1;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);



                    OnHighlightEntry(selectedEntryIndex, playerIndex);
               }

               // Move to the next menu entry?
               else if (input.IsMenuDown(ControllingPlayer, out playerIndex))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    selectedEntryIndex++;

                    if (selectedEntryIndex >= menuEntries.Count)
                    {
                         selectedEntryIndex = 0;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);


                    OnHighlightEntry(selectedEntryIndex, playerIndex);
               }

               // Move to the left menu entry?
               else if (input.IsMenuLeft(ControllingPlayer))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    // The If is just a test; normally there's no If statement.
                    if (numberOfColumns > 1)
                    {
                         selectedEntryIndex -=
                              (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

                         if (selectedEntryIndex < 0)
                         {
                              selectedEntryIndex = 0;
                         }
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               else if (input.IsMenuRight(ControllingPlayer))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    // The If is just a test; normally there's no If statement.
                    if (numberOfColumns > 1)
                    {
                         selectedEntryIndex +=
                              (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

                         if (selectedEntryIndex >= menuEntries.Count)
                              selectedEntryIndex = menuEntries.Count - 1;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
               {
                    OnSelectEntry(selectedEntryIndex, playerIndex);
               }

               else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
               {
                    OnCancel(playerIndex);
               }
          }

          /// <summary>
          /// Handler for when the user has chosen a menu entry.
          /// </summary>
          protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
          {
               if (entryIndex >= menuEntries.Count     // New
                    || entryIndex < 0)
                    return;

               else
               {
                    menuEntries[selectedEntryIndex].OnSelectEntry(playerIndex);

                    //if (menuEntries.Count > 0)
                    {
                         AudioManager.PlayCue(buttonPressSound);
                    }
               }
          }


          protected virtual void OnHighlightEntry(int entryIndex, PlayerIndex playerIndex)
          {
               if (entryIndex >= menuEntries.Count     // New
                    || entryIndex < 0)
                    return;

               else
               {
                    menuEntries[selectedEntryIndex].OnHighlightEntry(playerIndex);

                    menuEntries[selectedEntryIndex].menuDescriptionTextObject.TextEffect = new ReadingEffect(1.0f, menuEntries[selectedEntryIndex].Description);
               }
          }

          /// <summary>
          /// Handler for when the user has cancelled the menu.
          /// </summary>
          protected virtual void OnCancel(PlayerIndex playerIndex)
          {
               ExitScreen();
          }

          /// <summary>
          /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
          /// </summary>
          protected void OnCancel(object sender, PlayerIndexEventArgs e)
          {
               OnCancel(e.PlayerIndex);
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the Menu Screen.
          /// 
          /// Performs logic such as updating the base Game Screen, updating each Menu Entry, etc.
          /// </summary>
          /// <param name="gameTime">The GameTime instance from the Game class.</param>
          /// <param name="otherScreenHasFocus">Whether or not another screen has focus over this one.</param>
          /// <param name="coveredByOtherScreen">Whether or not another screen is covering this one.</param>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // Update each MenuEntry object.
               for (int i = 0; i < menuEntries.Count; i++)
               {
                    bool isSelected = IsActive && (i == selectedEntryIndex);

                    menuEntries[i].Update(this, isSelected, gameTime);
               }
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Menu Screen.
          /// 
          /// Also calls each Menu Entry's Draw method.
          /// </summary>
          /// <param name="gameTime">The GameTime instance from the Game class.</param>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

               float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

               spriteBatch.Begin();

               // Draw each menu entry in turn.
               for (int i = 0; i < menuEntries.Count; i++)
               {
                    Vector2 positionWithTransition = menuEntries[i].Position;

                    if (ScreenState == ScreenState.TransitionOn)
                    {
                         positionWithTransition.X -= transitionOffset * 512 * 2;
                    }

                    else
                    {
                         positionWithTransition.X += transitionOffset * 512 * 4;
                    }

                    bool isSelected = IsActive && (i == selectedEntryIndex);

                    menuEntries[i].Draw(this, positionWithTransition, isSelected, gameTime);


                    if (i == selectedEntryIndex)
                    {
                         // Render the Menu Description area.
                         //menuDescription.Draw(menuEntries[i].Description, gameTime, TransitionAlpha, transitionOffset);
                    }
               }


               // Render the Menu Header.
               menuHeader.Draw(gameTime, TransitionAlpha, transitionOffset, blankTexture);

               // Render the Menu Footer area.
               menuFooter.Draw(gameTime, TransitionAlpha, transitionOffset);



               spriteBatch.End();
          }

          #endregion
     }
}
