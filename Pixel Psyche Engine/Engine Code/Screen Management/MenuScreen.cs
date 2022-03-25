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
using PixelEngine.Menu;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
using PixelEngine.Graphics;
#endregion

namespace PixelEngine.Screen
{
     /// <remarks>
     /// Base class for screens that contain a menu of options. The user can
     /// move up and down to select an entry, or cancel to back out of the screen.
     /// </remarks>
     public abstract class MenuScreen : GameScreen
     {
          #region Fields

          // The list of menu entries found within this menu screen.
          protected List<MenuEntry> menuEntries = new List<MenuEntry>();

          // The Menu's Title, Position, Color and Scale to be rendered.
          protected string menuTitle;
          protected Vector2 menuTitlePosition = MenuScreen.DefaultMenuTitlePosition;
          protected Color menuTitleColor = Color.CornflowerBlue;
          protected float titleFontScale = 1.25f;//1.5f;

          // The speed at which the Menu Title transitions.
          protected float menuTitleTransitionSpeed = 100f;

          // The menu's sounds.
          protected string menuScrollSound = "MenuScroll_Swish";//Whoosh";
          protected string buttonPressSound = "ButtonPress";

          // Begin with the first menu entry being the one selected.
          protected int selectedEntry = 0;

          // Describes the menu's column and spacing layout.
          protected int numberOfColumns = 1;
          protected float spacingBetweenColumns = 485;

 
          /*
          // By default, the position for the Menu Title. Up top, just barely out of the title safe area.
          public static Vector2 defaultMenuTitlePosition = new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);
                
          // By default, the position for the first menu entry, just barely below the Menu Title.
          public static Vector2 DefaulMenuEntryStartPosition =
               new Vector2(0.0f, EngineCore.GraphicsDevice.Viewport.Height * 0.175f);
          
          // By default, the position for the menu entry Description, at the bottom, just out of the title safe area.
          // This default fits about two line of text for the Description, before we encounter the title safe area cut-off.
          public static Vector2 DefaultDescriptionPosition = 
               new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.Height - (EngineCore.GraphicsDevice.Viewport.Height * 0.175f));
          
          // By default, the position for the Back menu entry, at the bottom, above the default Description position.
          public static Vector2 DefaultBackButtonPosition = new Vector2();
          */
          public static Vector2 DefaultMenuTitlePosition
          {
               get 
               {
                    return //new Vector2(EngineCore.ScreenCenter.X, EngineCore.CustomizableSafeArea.Top);
                    new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top); 
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
                    return //new Vector2(EngineCore.ScreenCenter.X, EngineCore.CustomizableSafeArea.Height - 100f);
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.Height - (EngineCore.GraphicsDevice.Viewport.Height * 0.175f));
               }
          }

          public static Vector2 DefaultBackButtonPosition
          {
               get { return Vector2.Zero; }
          }

          #endregion

          #region Properties

          /// <summary>
          /// Gets the list of menu entries, so derived classes can add
          /// or change the menu contents.
          /// </summary>
          protected IList<MenuEntry> MenuEntries
          {
               get { return menuEntries; }
          }

          /// <summary>
          /// Get or Set the menu's title / header string.
          /// </summary>
          protected string MenuTitle
          {
               get { return menuTitle; }
               set { menuTitle = value; }
          }

          /// <summary>
          /// Get or Set the Color the menu's Title string is rendered in.
          /// </summary>
          protected Color MenuTitleColor
          {
               get { return menuTitleColor; }
               set { menuTitleColor = value; }
          }

          /// <summary>
          /// Get or Set the Position the menu's Title string is rendered at.
          /// </summary>
          protected Vector2 MenuTitlePosition
          {
               get { return 
                    //new Vector2(EngineCore.ScreenCenter.X, EngineCore.CustomizableSafeArea.Top); }
                    new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top); }
               //return menuTitlePosition; }
               set { menuTitlePosition = value; }
          }

          /// <summary>
          /// Get or Set the scale the Title string is rendered in.
          /// </summary>
          protected float MenuTitleFontScale
          {
               get { return titleFontScale; }
               set { titleFontScale = value; }
          }

          /// <summary>
          /// Get or Set the 0-indexed number representing which
          /// menu entry is currently selected.
          /// </summary>
          protected int SelectedMenuEntry
          {
               get { return selectedEntry; }
               set { selectedEntry = value; }
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

                         // Blank texture used to highlight Selected Entries.
          protected GameResourceTexture2D     blankTexture;


          public int SelectedEntryIndex
          {
               get { return selectedEntry; }

               // Shouldn't have a setter but I need it for one thing :(
               set { selectedEntry = value; }
          }

          public MenuEntry SelectedMenuEntryTESTING
          {
               get { return MenuEntries[selectedEntry]; }
          }

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public MenuScreen(string menuTitle)
          {
               this.menuTitle = menuTitle;

               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.0f);
          }

          public override void LoadContent()
          {
               base.LoadContent();

               MenuEntry.LoadContent();

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

                    selectedEntry--;

                    if (selectedEntry < 0)
                    {
                         if (menuEntries.Count == 0)
                              selectedEntry = 0;

                         else
                              selectedEntry = menuEntries.Count - 1;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);



                    OnHighlightEntry(selectedEntry, playerIndex);
               }

               // Move to the next menu entry?
               else if (input.IsMenuDown(ControllingPlayer, out playerIndex))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                    {
                         selectedEntry = 0;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);


                    OnHighlightEntry(selectedEntry, playerIndex);
               }

               // Move to the left menu entry?
               else if (input.IsMenuLeft(ControllingPlayer))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    // New:
                    int previouslySelectedEntry = 0;
                    // End

                    // The If is just a test; normally there's no If statement.
                    if (numberOfColumns > 1)
                    {
                         // New:
                         previouslySelectedEntry = selectedEntry;
                         // End

                         selectedEntry -=
                              (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

                         if (selectedEntry < 0)
                         {
                              selectedEntry = 0;
                         }
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    // New: Don't play scroll sound if we didn't even scroll.
                    if (menuEntries.Count > 1 && this.numberOfColumns > 1)
                    {
                         if (previouslySelectedEntry != selectedEntry)
                              AudioManager.PlayCue(menuScrollSound);
                    }
               }

               else if (input.IsMenuRight(ControllingPlayer))
               {
                    // This is new as of 11-15-2011.
                    if (menuEntries.Count <= 1)
                    {
                         return;
                    }

                    // New:
                    int previouslySelectedEntry = 0;
                    // End

                    // The If is just a test; normally there's no If statement.
                    if (numberOfColumns > 1)
                    {
                         // New:
                         previouslySelectedEntry = selectedEntry;
                         // End

                         selectedEntry +=
                              (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

                         if (selectedEntry >= menuEntries.Count)
                              selectedEntry = menuEntries.Count - 1;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    // New: Don't play scroll sound if we didn't even scroll.
                    if (menuEntries.Count > 1 && this.numberOfColumns > 1)
                    {
                         if (previouslySelectedEntry != selectedEntry)
                              AudioManager.PlayCue(menuScrollSound);
                    }
               }

               if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
               {
                    OnSelectEntry(selectedEntry, playerIndex);
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
                    menuEntries[selectedEntry].OnSelectEntry(playerIndex);

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
                    menuEntries[selectedEntry].OnHighlightEntry(playerIndex);

                    menuEntries[selectedEntry].menuDescriptionTextObject.TextEffect = new ReadingEffect(1.0f, menuEntries[selectedEntry].Description);
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
          /// Updates the Menu.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // Update each nested MenuEntry object.
               for (int i = 0; i < menuEntries.Count; i++)
               {
                    bool isSelected = IsActive && (i == selectedEntry);

                    menuEntries[i].Update(this, isSelected, gameTime);
               }
          }

          #endregion
  
          #region Draw

          public FontType MenuTitleFont
          {
               get { return menuTitleFont; }
               set { menuTitleFont = value; }
          }
          private FontType menuTitleFont = FontType.TitleFont;

          public bool UseExactMenuEntryPositions = false;

          /// <summary>
          /// Draws the Menu.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               SpriteFont font = ScreenManager.Font;

               Vector2 position = MenuScreen.DefaultMenuEntryStartPosition;

               // Make the menu slide into place during transitions, using a
               // power curve to make things look more interesting (this makes
               // the movement slow down as it nears the end).
               float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

               if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256 * 4;
               else
                    position.X += transitionOffset * 512 * 4;

               // New
               //This will get the # of rows we want in each column
               int maxRowsPerColumn =
                    (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

               // End

               spriteBatch.Begin();

               // Draw each menu entry in turn.
               for (int i = 0; i < menuEntries.Count; i++)
               {
                    MenuEntry menuEntry = menuEntries[i];

                    bool isSelected = IsActive && (i == selectedEntry);

                    if (UseExactMenuEntryPositions)
                    {
                         #region Forced-Position Rendering

                         // New Method.

                         menuEntry.Draw(this, new Vector2(menuEntries[i].Position.X, menuEntries[i].Position.Y), isSelected, gameTime);


                         // End New method.

                         #endregion
                    }

                    else
                    {
                         #region Auto-Position Rendering

                         // Old Method.

                         menuEntry.Draw(this, new Vector2(position.X + menuEntries[i].Position.X,
                                  position.Y + menuEntries[i].Position.Y), isSelected, gameTime);

                         position.Y += menuEntry.VerticalSpacing;

                         // We need to handle an odd # of menu entries different
                         // from an even # of entries so we mod to figure out
                         // if we need to start a new column.
                         if (maxRowsPerColumn % 2 == 2) // Even
                         {
                              if (i % maxRowsPerColumn == maxRowsPerColumn)
                              {
                                   // We use 'hard' values in here to keep the rows and columns
                                   // lined up perfectly. These will vary greatly from project to
                                   // project based on what you want to do so make sure to play
                                   // with them to get a result you like.
                                   position.X += SpacingBetweenColumns;

                                   // This variable should be the same as the 'position' value
                                   // set at the top of this method. It looks like this (for me)
                                   // so make sure to match the Y value:
                                   position.Y = DefaultMenuEntryStartPosition.Y;
                              }
                         }

                         else // Odd
                         {
                              if (i % maxRowsPerColumn == maxRowsPerColumn - 1)
                              {
                                   // Same comments from above apply here.
                                   position.X += SpacingBetweenColumns;
                                   position.Y = DefaultMenuEntryStartPosition.Y;
                              }
                         }

                         // End Old method.

                         #endregion
                    }
               }

               // Draw the menu title.
               Vector2 titlePosition = MenuTitlePosition;
               Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
               Color titleColor = menuTitleColor * (TransitionAlpha / 255f);

               titlePosition.Y -= (transitionOffset * 512);

               // WE USE TO DRAW THE MENU TITLE WITH THIS:
               //TextManager.DrawCentered(false, font, menuTitle, titlePosition, titleColor, titleFontScale);

               // BUT NOW WE USE TEXT OBJECT
               TextObject titleTextObject = new TextObject(menuTitle, titlePosition);
               titleTextObject.FontType = MenuTitleFont;
               titleTextObject.Color = titleColor;
               titleTextObject.Origin = titleTextObject.Font.MeasureString(menuTitle) / 2;
               titleTextObject.Scale = titleFontScale / 2;
               titleTextObject.Origin = new Vector2(titleTextObject.Origin.X, 0);


               /*
               GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D,
                    new Rectangle(0, (int)titlePosition.Y, EngineCore.GraphicsDevice.Viewport.Width, (int)(titleTextObject.Font.MeasureString(menuTitle).Y * titleTextObject.Scale)),
                    2, this.MenuTitleColor * (TransitionAlpha / 255f));
               */


               MySpriteBatch.Draw(blankTexture.Texture2D, 
                    new Rectangle(0, (int)titlePosition.Y, 
                         EngineCore.GraphicsDevice.Viewport.Width, (int)(titleTextObject.Font.MeasureString(menuTitle).Y * titleTextObject.Scale)), Color.Black * (150f / 255f));

               titleTextObject.Draw(gameTime);

               spriteBatch.End();
          }

          #endregion
     }
}
