#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
// Matt McGrath, with help provided by:
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Menu
{
     /// <summary>
     /// Helper class represents a single entry in a MenuScreen. By default this
     /// just draws the entry text string, but it can be customized to display menu
     /// entries in different ways. This also provides an event that will be raised
     /// when the menu entry is selected.
     /// </summary>
     public class TexturedMenuEntry : MenuEntry
     {
          #region Fields

          private GameResourceTexture2D texture;

          private GameResourceTexture2D buttonTexture;
          private GameResourceTexture2D selectedBorderTexture;

          // Fields to provide a Menu Entry "Description".
          private string menuDescription = String.Empty;
          private Color descriptionColor = Color.CornflowerBlue;
          private Vector2 descriptionPosition;
          private float descriptionFontScale = 1.0f;


          // END NEW FEATURES

          private Rectangle menuEntryRectangle;

          /// <summary>
          /// Tracks a fading selection effect on the entry.
          /// </summary>
          /// <remarks>
          /// The entries transition out of the selection effect when they are deselected.
          /// </remarks>
          float selectionFade;

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets the text of this menu entry.
          /// </summary>
          public string Description
          {
               get { return menuDescription; }
               set { menuDescription = value; }
          }


          /// <summary>
          /// Gets or sets the Color the Description string is.
          /// </summary>
          public Color DescriptionColor
          {
               get { return descriptionColor; }
               set { descriptionColor = value; }
          }

          /// <summary>
          /// Gets or sets the position the Description string is at.
          /// </summary>
          public Vector2 DescriptionPosition
          {
               get { return descriptionPosition; }
               set { descriptionPosition = value; }
          }


          /// <summary>
          /// Gets or sets the Scale of this 
          /// Menu Entry's Description text.
          /// </summary>
          public float DescriptionFontScale
          {
               get { return descriptionFontScale; }
               set { descriptionFontScale = value; }
          }

          public Rectangle MenuEntryRectangle
          {
               get { return menuEntryRectangle; }

               set { menuEntryRectangle = value; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text.
          /// </summary>
          public TexturedMenuEntry(string menuTexture)
               : base("")
          {
               texture = ResourceManager.LoadTexture(menuTexture);

               selectedBorderTexture = ResourceManager.LoadTexture(@"Textures\HUD\BlankHudBgBorder_Thick");

               buttonTexture = ResourceManager.LoadTexture(@"Textures\TextBubble_3D");
          }

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text and Description.
          /// </summary>
          public TexturedMenuEntry(string menuTexture, string description)
               : this(menuTexture)
          {
               menuDescription = description;
               descriptionPosition = MenuScreen.DefaultDescriptionPosition;
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the menu entry.
          /// </summary>
          public override void Update(MenuScreen screen, bool isSelected,
                                                        GameTime gameTime)
          {
               // When the menu selection changes, entries gradually fade between
               // their selected and deselected appearance, rather than instantly
               // popping to the new state.
               float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

               if (isSelected)
                    selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
               else
                    selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the menu entry. This can be overridden to customize the appearance.
          /// </summary>
          public override void Draw(MenuScreen screen, Vector2 position,
               bool isSelected, GameTime gameTime)
          {
               Color color = new Color();

               // Modify the alpha to fade text out during transitions.
               color = color * (screen.TransitionAlpha / 255f);

               SpriteFont font = ScreenManager.Font;

               if (isSelected)
               {
                    MySpriteBatch.Draw(selectedBorderTexture.Texture2D, 
                         new Rectangle(menuEntryRectangle.X - 10, menuEntryRectangle.Y - 10, menuEntryRectangle.Width + 20, menuEntryRectangle.Height + 20), 
                         Color.Gold);
               }

               // Render button texture.
               MySpriteBatch.Draw(buttonTexture.Texture2D, menuEntryRectangle, Color.CornflowerBlue * (150f / 255f));

               // Render texture.
               MySpriteBatch.Draw(texture.Texture2D, menuEntryRectangle, Color.White);
          }

          #endregion
     }
}
