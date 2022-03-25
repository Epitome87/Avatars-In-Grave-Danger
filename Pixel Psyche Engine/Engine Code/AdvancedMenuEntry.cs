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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace PixelEngine.Menu
{
     /// <summary>
     /// Helper class represents a single entry in a MenuScreen. By default this
     /// just draws the entry text string, but it can be customized to display menu
     /// entries in different ways. This also provides an event that will be raised
     /// when the menu entry is selected.
     /// </summary>
     public class AdvancedMenuEntry
     {
          public TextObject menuDescriptionTextObject = new TextObject();

          public enum ButtonIconType
          {
               /// <summary>
               /// Don't show any button icon.
               /// </summary>
               None,

               /// <summary>
               /// Show only one button icon.
               /// </summary>
               One,

               /// <summary>
               /// Alternate between two button icons.
               /// </summary>
               Alternate
          }
          public ButtonIconType buttonIconType = ButtonIconType.One;

          public enum BorderSizeType
          {
               /// <summary>
               /// Indicates no Border will be used.
               /// </summary>
               None,

               /// <summary>
               /// Indicates a Border with size determined by the 
               /// menu entry's text will be used.
               /// </summary>
               Autofit,

               /// <summary>
               /// Indicates that a custom Border size will be used.
               /// </summary>
               Custom,


               /// <summary>
               /// Indicates that a Border which takes up the 
               /// entire width of the screen will be used.
               /// </summary>
               Screenwidth
          }
          public BorderSizeType borderSizeType = BorderSizeType.Autofit;

          public enum BorderStyleType
          {
               None,

               BlankTexture,

               GradientTexture,

               CustomTexture,
          }
          public BorderStyleType borderStyleType = BorderStyleType.GradientTexture;


          #region Fields

          private string text;
          private Vector2 menuPosition;
          private Vector2 origin;
          private bool isCenter;


          // Fields to help customize an Entry's color appearance..
          private Color selectedColor = Color.White;
          private Color unselectedColor = Color.White;

          // Fields to provide a Menu Entry "Description".
          private string menuDescription = String.Empty;
          private Color descriptionColor = Color.CornflowerBlue;
          private Vector2 descriptionPosition;

          // Fields to help customize an Entry's appearance.
          private bool showIcon = false;
          private bool showPlainBorder = false;
          public Color BorderColor = Color.DarkOrange * (50f / 255f);

          protected static GameResourceTexture2D ButtonIcon;
          protected static GameResourceTexture2D blankTexture;

          private float fontScale = 1.0f;
          private float descriptionFontScale = 1.0f;

          private float additionalVerticalSpacing = 0.0f;

          // NEW FEATURES


          public bool useCustomBorderSize = false;
          private bool showGradientBorder = true;

          // The actual texture for the border.
          private static GameResourceTexture2D GradientBorderTexture;
          private static GameResourceTexture2D GradientBorderTexture_TheBorder;

          // For the border texture size and scale.
          public Vector2 menuEntryBorderSize = new Vector2(500, 100);
          public Vector2 menuEntryBorderScale = new Vector2(1.0f, 1.0f);

          // For the border texture colors.
          private Color selectedBorderColor = Color.Black * (125f / 255f);//Color.DarkOrange * (125f / 255f);
          private Color unselectedBorderColor = Color.CornflowerBlue * (125f / 255f);


          private Rectangle menuEntryRectangle;

          private bool isPulsating = true;

          /// <summary>
          /// Tracks a fading selection effect on the entry.
          /// </summary>
          /// <remarks>
          /// The entries transition out of the selection effect when they are deselected.
          /// </remarks>
          float selectionFade;

          #endregion


          private TextObject menuEntryTextObject;




          public FontType DescriptionFontType
          {
               get { return descriptionFontType; }
               set { descriptionFontType = value; }
          }
          private FontType descriptionFontType = FontType.MenuFont;

          #region Properties

          /// <summary>
          /// Gets or sets the text of this menu entry.
          /// </summary>
          public string Text
          {
               get { return text; }
               set
               {
                    text = value;
                    menuEntryTextObject.Text = value;
               }
          }

          /// <summary>
          /// Gets or sets the text of this menu entry.
          /// </summary>
          public string Description
          {
               get { return menuDescription; }
               set { menuDescription = value; }
          }

          /// <summary>
          /// Gets or sets the IsCenter of this menu entry.
          /// </summary>
          /// <remarks>
          /// If set to True, Menu Entries are centered on screen.
          /// If set to False, they are left-aligned.
          /// </remarks>
          public bool IsCenter
          {
               get { return isCenter; }
               set { isCenter = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Entry is while Selected.
          /// </summary>
          public Color SelectedColor
          {
               get { return selectedColor; }
               set { selectedColor = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Entry is while Unselected.
          /// </summary>
          public Color UnselectedColor
          {
               get { return unselectedColor; }
               set { unselectedColor = value; }
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
          /// Gets or sets whether or not to show the Button Icon
          /// beside the Menu Entry.
          /// </summary>
          public bool ShowIcon
          {
               get { return showIcon; }
               set { showIcon = value; }
          }

          /// <summary>
          /// Gets or sets whether or not to show the transparent
          /// border highlighting the selected Menu Entry.
          /// </summary>
          public bool ShowBorder
          {
               get { return showPlainBorder; }
               set
               {
                    showPlainBorder = value;

                    // We can't have both a plain and gradient border,
                    // so if we're showing the plain, don't show the gradient.
                    if (showPlainBorder)
                    {
                         showGradientBorder = false;
                    }
               }
          }

          /// <summary>
          /// Gets or sets whether or not we should show / use the 
          /// gradient texture for our menu entry's border.
          /// This type of texture will replace the plain transparent one.
          /// 
          /// Default value: True
          /// </summary>
          public bool ShowGradientBorder
          {
               get { return showGradientBorder; }
               set
               {
                    showGradientBorder = value;

                    // We can't have both a plain and gradient border,
                    // so if we're showing the gradient, don't show the plain.
                    if (showGradientBorder)
                    {
                         showPlainBorder = false;
                    }
               }
          }

          /// <summary>
          /// Gets or sets the color of the menu entry's border - when selected.
          /// </summary>
          public Color SelectedBorderColor
          {
               get { return selectedBorderColor; }
               set { selectedBorderColor = value; }
          }

          /// <summary>
          /// Gets or sets the Color of the menu entry's border - when unselected.
          /// </summary>
          public Color UnselectedBorderColor
          {
               get { return unselectedBorderColor; }
               set { unselectedBorderColor = value; }
          }

          /// <summary>
          /// Gets or sets the Position of this Menu Entry.
          /// </summary>
          public Vector2 Position
          {
               get { return menuPosition; }
               set { menuPosition = value; }
          }

          /// <summary>
          /// Gets or sets the Origin of this Menu Entry.
          /// </summary>
          public Vector2 Origin
          {
               get { return origin; }
               set { origin = value; }
          }

          /// <summary>
          /// Gets or sets the Scale of this Menu Entry's text.
          /// </summary>
          public float FontScale
          {
               get { return fontScale; }
               set { fontScale = value; }
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

          /// <summary>
          /// Gets the Vertical Spacing used between this MenuEntry and the next.
          /// Default: The height of this MenuEntry.
          /// Note: Set AdditionalVerticalSpacing for less / more padding.
          /// </summary>
          public float VerticalSpacing
          {
               get
               {
                    return EngineCore.ResolutionScale * this.GetTrueHeight() *
                         this.FontScale + additionalVerticalSpacing;
               }
          }

          /// <summary>
          /// Gets or sets the Additional Vertical Spacing used between this
          /// MenuEntry and the next.
          /// Default: 0
          /// </summary>
          public float AdditionalVerticalSpacing
          {
               get { return additionalVerticalSpacing; }
               set { additionalVerticalSpacing = value; }
          }

          public Rectangle MenuEntryRectangle
          {
               get { return menuEntryRectangle; }

               set { menuEntryRectangle = value; }
          }

          /// <summary>
          /// Gets or sets whether or not this menu entry should
          /// have its text use a pulsating effect.
          /// </summary>
          public bool IsPulsating
          {
               get { return isPulsating; }
               set { isPulsating = value; }
          }

          #endregion


          #region Events

          /// <summary>
          /// Event raised when the menu entry is selected.
          /// </summary>
          public event EventHandler<PlayerIndexEventArgs> Selected;


          /// <summary>
          /// Event raised for when the menu entry is highlighted.
          /// </summary>
          public event EventHandler<PlayerIndexEventArgs> Highlighted;

          /// <summary>
          /// Method for raising the Selected event.
          /// </summary>
          protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
          {
               if (Selected != null)
                    Selected(this, new PlayerIndexEventArgs(playerIndex));
          }

          /// <summary>
          /// Method for raising the Highlighted event.
          /// </summary>
          /// <param name="playerIndex"></param>
          protected internal virtual void OnHighlightEntry(PlayerIndex playerIndex)
          {
               if (Highlighted != null)
               {
                    Highlighted(this, new PlayerIndexEventArgs(playerIndex));
               }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text.
          /// </summary>
          public AdvancedMenuEntry(string menuText)
          {
               text = menuText;

               menuPosition = new Vector2(EngineCore.ScreenCenter.X, 0f);

               descriptionPosition = MenuScreen.DefaultDescriptionPosition;

               IsCenter = true;
               ShowIcon = true;
               ShowBorder = false;

               menuEntryTextObject = new TextObject(text, menuPosition);
               menuEntryTextObject.TextEffect = null;
               menuEntryTextObject.FontType = FontType.MenuFont;


               menuDescriptionTextObject.AddTextEffect(new ReadingEffect(2.0f, ""));
          }

          public FontType FontType
          {
               get { return fontType; }
               set { fontType = value; }
          }

          FontType fontType = FontType.MenuFont;




          /// <summary>
          /// Constructs a new Menu Entry with the specified Text and Description.
          /// </summary>
          public AdvancedMenuEntry(string text, string description)
               : this(text)
          {
               menuDescription = description;
          }

          private static GameResourceTexture2D buttonIcon;

          /// <summary>
          /// Load relevant content (Button Icon texture, etc).
          /// </summary>
          public static void LoadContent()
          {
               ContentManager content;
               content = ScreenManager.Game.Content;

               // Icon to display the "A" Button next to Selected Entries.
               //ButtonIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonA");

               // NEW AS OF 5-31-2011: Testing for alternating Buttin Icon images:
               buttonIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonA");

               // Blank texture used to highlight Selected Entries.
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");

               // Texture used to border the Menu Entry.
               GradientBorderTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank1_WithoutBorder");
               GradientBorderTexture_TheBorder = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank1_TheBorder");
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the menu entry.
          /// </summary>
          public virtual void Update(AdvancedMenuScreen screen, bool isSelected,
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


               if (isSelected)
               {
                    menuEntryTextObject.Update(gameTime);

                    //menuDescriptionTextObject.Update(gameTime);
               }
          }

          #endregion




          public bool ShowDescriptionBorder = true;

          #region Draw

          /// <summary>
          /// Draws the menu entry. This can be overridden to customize the appearance.
          /// </summary>
          public virtual void Draw(AdvancedMenuScreen screen, Vector2 position,
               bool isSelected, GameTime gameTime)
          {
               // Color the menu entry according to whether it's selected.
               Color color = isSelected ? selectedColor : unselectedColor;

               // Modify the alpha to fade text out during transitions.
               color = color * (screen.TransitionAlpha / 255f);


               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float pulsate = 0.0f;

               if (isPulsating)
               {
                    pulsate = (float)Math.Sin(time * 6) + 1;
               }

               float scale = fontScale + pulsate * 0.05f * selectionFade;


               SpriteFont font = TextManager.Fonts[(int)this.FontType].SpriteFont;

               float scaleFactor = EngineCore.ResolutionScale * fontScale;


               if (isSelected)
               {
                    if (showIcon)
                    {
                         // Testing new position for buttons
                         DrawButtonIcon(gameTime, screen, position, scaleFactor, font);
                    }

                    // Draw the Description Area for this Menu Entry.
                    DrawDescriptionArea(gameTime, screen);
               }


               // Draw text, centered on the middle of each line.
               if (IsCenter)
               {
                    menuEntryTextObject.IsCenter = true;
                    menuEntryTextObject.Position = position;
                    menuEntryTextObject.Color = color;
                    menuEntryTextObject.Scale = scale;
                    menuEntryTextObject.FontType = fontType;

                    menuEntryTextObject.Draw(gameTime);
               }

               else
               {
                    Origin = new Vector2(0, font.LineSpacing / 2.0f);

                    menuEntryTextObject.Position = new Vector2(200f, position.Y);
                    menuEntryTextObject.Color = color;
                    menuEntryTextObject.Scale = scale;
                    menuEntryTextObject.Origin = Origin;
                    menuEntryTextObject.FontType = fontType;

                    menuEntryTextObject.Draw(gameTime);
               }
          }

          private void DrawButtonIcon(GameTime gameTime, AdvancedMenuScreen screen, Vector2 position, float scaleFactor, SpriteFont font)
          {
               Vector2 buttonPosition = new Vector2();

               Rectangle buttonRectangle = new Rectangle((int)position.X, (int)position.Y,
                    (int)(scaleFactor * this.GetTrueHeight() / 2.0f), (int)(scaleFactor * this.GetTrueHeight() / 2.0f));


               if (IsCenter)
               {
                    buttonPosition = 
                         new Vector2(position.X - (menuEntryBorderSize.X / 2) - 50, position.Y - ((scaleFactor * font.LineSpacing / 2)));

                    int buttonWidthHeight = 
                         (int)(scaleFactor * this.GetTrueHeight());

                    buttonPosition =
                         new Vector2(position.X - (menuEntryBorderSize.X / 2) - buttonWidthHeight, position.Y - (buttonWidthHeight / 2));
               }

               else
               {
                    buttonPosition = new Vector2(200f - (scaleFactor * this.GetTrueHeight() * 2.0f), position.Y - (scaleFactor * this.GetTrueHeight() / 2.0f));
               }



               Color iconColor = Color.White * (screen.TransitionAlpha / 255f);

                    MySpriteBatch.Draw(buttonIcon.Texture2D, new Rectangle(
                                        (int)(buttonPosition.X),
                                        (int)(buttonPosition.Y),
                                        (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale),
                                        (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale)),
                                        iconColor);
          }

          #endregion


          public virtual void DrawDescriptionArea(GameTime gameTime, AdvancedMenuScreen screen)
          {
               Vector2 slidingPosition = screen.Description.Position;

               // Make the menu slide into place during transitions, using a
               // power curve to make things look more interesting (this makes
               // the movement slow down as it nears the end).
               float transitionOffset = (float)Math.Pow(screen.TransitionPosition, 2);

               if (screen.ScreenState == ScreenState.TransitionOn)
                    slidingPosition.Y += transitionOffset * 256;
               else
                    slidingPosition.Y += transitionOffset * 512;


               
               menuDescriptionTextObject.Text = menuDescription;
               menuDescriptionTextObject.Position = slidingPosition;
               menuDescriptionTextObject.FontType = DescriptionFontType;
               menuDescriptionTextObject.Color = descriptionColor * (screen.TransitionAlpha / 255f);
               menuDescriptionTextObject.Scale = descriptionFontScale;
               menuDescriptionTextObject.IsCenter = true;
               menuDescriptionTextObject.Origin = menuDescriptionTextObject.Font.MeasureString(menuDescription) / 2f;



               // Render the Description Border, if it's Enabled.
               if (ShowDescriptionBorder)
               {
                         Rectangle descriptionBorderRect = new Rectangle((int)slidingPosition.X, (int)slidingPosition.Y, 800, 100);
                         MySpriteBatch.DrawCentered(blankTexture.Texture2D, descriptionBorderRect, Color.Black * (150f / 255f) * (screen.TransitionAlpha / 255f));
               }

               menuDescriptionTextObject.Update(gameTime);

               // Draw the actual Menu Entry Text now.
               menuDescriptionTextObject.Draw(gameTime);
          }


          #region Public Methods

          /// <summary>
          /// Queries how much space this menu entry requires.
          /// </summary>
          public virtual int GetHeight(MenuScreen screen)
          {
               return TextManager.Fonts[(int)fontType].SpriteFont.LineSpacing;
          }

          /// <summary>
          /// Queries how much space this menu entry requires -
          /// line breaks included.
          /// </summary>
          /// <returns></returns>
          public virtual float GetTrueHeight()
          {
               return TextManager.Fonts[(int)fontType].SpriteFont.MeasureString(this.Text).Y;
          }

          #endregion
     }
}
