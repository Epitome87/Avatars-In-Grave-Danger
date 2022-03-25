#region File Description
//-----------------------------------------------------------------------------
// TextHandler.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using System;
using System.Globalization;
#endregion

namespace PixelEngine.Text
{
     public static class ColorExtensions
     {
          /// <summary>
          /// Creates an ARGB hex string representation of the <see cref="Color"/> value.
          /// </summary>
          /// <param name="color">The <see cref="Color"/> value to parse.</param>
          /// <param name="includeHash">Determines whether to include the hash mark (#) character in the string.</param>
          /// <returns>Returns a hex string representation of the specified <see cref="Color"/> value.</returns>
          public static string ToHex(this Color color, bool includeHash)
          {
               string[] argb = 
               {
                            color.A.ToString("X2"),
                            color.R.ToString("X2"),
                            color.G.ToString("X2"),
                            color.B.ToString("X2"),
               };

               return (includeHash ? "#" : string.Empty) + string.Join(string.Empty, argb);
          }
     }

     public static class StringExtensions
     {
          /// <summary>
          /// Creates a <see cref="Color"/> value from an ARGB or RGB hex string.  The string may begin with or without the
          /// hash mark (#) character.
          /// </summary>
          /// <param name="hexString">The ARGB hex string to parse.</param>
          /// <returns>Returns a <see cref="Color"/> value as defined by the ARGB or RGB hex string.</returns>
          /// <exception cref="InvalidOperationException">Thrown if the string is not a valid ARGB or RGB hex value.</exception>
          public static Color ToColor(this string hexString)
          {
               if (hexString.StartsWith("#"))
                    hexString = hexString.Substring(1);
               uint hex = uint.Parse(hexString, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
               Color color = Color.White;
               if (hexString.Length == 8)
               {
                    color.A = (byte)(hex >> 24);
                    color.R = (byte)(hex >> 16);
                    color.G = (byte)(hex >> 8);
                    color.B = (byte)(hex);
               }
               else if (hexString.Length == 6)
               {
                    color.R = (byte)(hex >> 16);
                    color.G = (byte)(hex >> 8);
                    color.B = (byte)(hex);
               }
               else
               {
                    throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
               }
               return color;
          }
     }



     #region Enums

     /// <summary>
     /// Enum representing a Font Type.
     /// </summary>
     public enum FontType
     {
          /// <summary>
          /// Font Used: Bloody Stomp
          /// Size: 72
          /// Border: 5 Pixels 
          /// </summary>
          TitleFont,

          /// <summary>
          /// Font Used: 
          /// </summary>
          MenuFont,

          /// <summary>
          /// Font Used: Bank Gothic Md, Regular
          /// Size: 28
          /// Border: None
          /// </summary>
          HudFont,

          /// <summary>
          /// Font Used: Seedy Motel
          /// Size: Large?
          /// Border: 5 Pixels
          /// </summary>
          SeedyMotelFont,
     };

     /// <summary>
     /// Enum representing a Format Type.
     /// </summary>
     public enum FormatType
     {
          TopLeft,
          TopCenter,
          TopRight,

          MidLeft,
          Center,
          MidRight,

          BottomLeft,
          BottomCenter,
          BottomRight
     };

     public enum AlignmentType
     {
          Left,
          Center,
          Right
     };

     #endregion

     /// <remarks>
     /// Helper class which makes drawing strings simpler.
     /// Fonts are pre-loaded and accessed through simple names.
     /// If the calling method does not supply a Font, a default one is used.
     /// 
     /// Note: TextManager automatically handles the Update and Drawing of TextObjects
     /// only if they are added via TextManager.Add(). If working with non-TextObject text,
     /// TextManager still provides useful methods for working with text--but it does not
     /// Update and Draw them automatically.
     /// 
     /// Essentially, all rendering of text could be done through this class rather than
     /// SpriteBatch.DrawString() calls.
     /// </remarks>
     public class TextManager : DrawableGameComponent
     {
          #region Singleton

          /// <summary>
          /// Our singleton.
          /// </summary>
          private static TextManager textHandler = null;

          #endregion


          #region Fields

          private ContentManager content;
          private static SpriteBatch spriteBatch;

          public static List<GameResourceFont> Fonts;
          private static GameResourceFont DefaultFont;
          private static List<TextObject> textObjects;

          private static bool isInitialized;

          #endregion


          #region Properties

          public static bool IsInitialized
          {
               get { return isInitialized; }
               set { isInitialized = value; }
          }

          /// <summary>
          /// Gets the "Menu" Font. Quick way to access it.
          /// </summary>
          public static SpriteFont MenuFont
          {
               get { return Fonts[(int)FontType.MenuFont].SpriteFont; }
          }

          /// <summary>
          /// Gets the "Title" Font. Quick way to access it.
          /// </summary>
          public static SpriteFont TitleFont
          {
               get { return Fonts[(int)FontType.TitleFont].SpriteFont; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          private TextManager(Game game)
               : base(game)
          {
               Fonts = new List<GameResourceFont>();
               textObjects = new List<TextObject>();
          }

          /// <summary>
          /// Initializes the Text Manager and adds it as a Game Component.
          /// </summary>
          public static void Initialize(Game game)
          {
               textHandler = new TextManager(game);

               if (game != null)
               {
                    game.Components.Add(textHandler);
               }
          }

          /// <summary>
          /// Override DrawableGameComponent.Initialize method.
          /// Simply calls GameComponent.Initialize and sets
          /// boolean IsInitialized to true.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();

               isInitialized = true;
          }

          /// <summary>
          /// Loads Content such as the various Fonts.
          /// </summary>
          protected override void LoadContent()
          {
               // Load content belonging to the screen manager.
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               GameResourceFont font;

               font = ResourceManager.LoadFont(@"Fonts\BloodyStompFont_72_5Outline");
               Fonts.Add(font);

               font = ResourceManager.LoadFont(@"Fonts\MenuFont_Border");
               Fonts.Add(font);

               font = ResourceManager.LoadFont(@"Fonts\BankGothicMd_Regular_28");
               Fonts.Add(font);

               font = ResourceManager.LoadFont(@"Fonts\SeedyMotelFont_Large");
               Fonts.Add(font);

               DefaultFont = font;
          }

          /// <summary>
          /// Unloads Content, such as the List of Fonts and Text Objects.
          /// </summary>
          protected override void UnloadContent()
          {
               Fonts.Clear();
               textObjects.Clear();
          }

          /// <summary>
          /// Resets the Text Manager by clearing the List of Text Objects it contains.
          /// </summary>
          public static void Reset()
          {
               textObjects.Clear();
          }

          #endregion


          #region Add and Remove Text

          /// <summary>
          /// Adds a Text Object to be managed by this Text Manager.
          /// <param name="textObj">The Text Object to add.</param>
          /// </summary>
          public static void AddText(TextObject textObj)
          {
               textObjects.Add(textObj);
          }

          /// <summary>
          /// Removes the specified Text Object being managed by this Text Manager.
          /// <param name="textObj">The Text Object to remove.</param>
          /// <returns>Returns True if the Text Object was removed, false otherwise.</returns>
          /// </summary>
          public static bool RemoveText(TextObject textObj)
          {
               return textObjects.Remove(textObj);
          }

          /// <summary>
          /// Removes all Text Objects being managed by this Text Manager.
          /// </summary>
          public static void RemoveAll()
          {
               textObjects.Clear();
          }

          #endregion


          #region Main Draw Method

          /// <summary>
          /// Draws all Text Objects being managed by the Text Manager.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               try
               {
                    spriteBatch.Begin();

                    foreach (TextObject textObj in textObjects)
                    {
                         textObj.Draw(gameTime);
                    }

                    spriteBatch.End();
               }

               catch (System.InvalidOperationException)
               { }
          }

          #endregion


          #region DrawString Overload Methods

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(string s, FontType font, Vector2 pos, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(Fonts[(int)font].SpriteFont, s, pos, color);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// Draws the string in the center.
          /// </summary>
          public static void DrawString(string s, FontType font, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               Vector2 textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
               MySpriteBatch.DrawString(Fonts[(int)font].SpriteFont, s, EngineCore.ScreenCenter, color,
                    0.0f, textOrigin, 1.0f, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// Hassle-Free DrawString: Uses the DefaultFont.
          /// </summary>
          public static void DrawString(string s, Vector2 pos, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(DefaultFont.SpriteFont, s, pos, color,
                    0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, s, pos, color);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color, float scale)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, s, pos, color,
                    0.0f, new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color,
               Vector2 origin, float scale)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, s, pos, color,
                    0.0f, origin, scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string word, Vector2 textPosition, Color color,
               float rotation, Vector2 textOrigin, float scale)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, word, textPosition, color, rotation,
                    textOrigin, scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string word, Vector2 textPosition, Color color,
               float rotation, Vector2 textOrigin, float scale, SpriteEffects spriteEffects, float depthLayer)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, word, textPosition, color, rotation,
                    textOrigin, scale, spriteEffects, depthLayer);
          }

          /// <summary>
          /// Draws the string in the position specified by FormatType.
          /// </summary>
          public static void DrawString(string s, FontType font, Color color, FormatType formatType)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               Vector2 textPos = new Vector2(0, 0);
               Vector2 textOrigin = new Vector2(0, 0);

               #region Switch (FormatType) Construct
               switch ((int)formatType)
               {
                    case (int)FormatType.TopLeft:
                         textPos = new Vector2(150, 150);
                         break;

                    case (int)FormatType.TopCenter:
                         textPos = new Vector2(EngineCore.ScreenCenter.X, 150);
                         textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
                         break;

                    case (int)FormatType.TopRight:
                         textPos = new Vector2(EngineCore.ScreenCenter.X * 2 - 150, 150);
                         break;

                    case (int)FormatType.MidLeft:
                         textPos = new Vector2(150, EngineCore.ScreenCenter.Y);
                         break;

                    case (int)FormatType.Center:
                         textPos = EngineCore.ScreenCenter;
                         textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
                         break;

                    case (int)FormatType.MidRight:
                         textPos = new Vector2(EngineCore.ScreenCenter.X * 2 - 50, EngineCore.ScreenCenter.Y);
                         break;

                    case (int)FormatType.BottomLeft:
                         textPos = new Vector2(150, EngineCore.ScreenCenter.Y * 2 - 150);
                         break;

                    case (int)FormatType.BottomCenter:
                         textPos = new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y * 2 - 150);
                         textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
                         break;

                    case (int)FormatType.BottomRight:
                         textPos = new Vector2(EngineCore.ScreenCenter.X * 2 - 150, EngineCore.ScreenCenter.Y * 2 - 150);
                         break;
               }
               #endregion

               MySpriteBatch.DrawString(Fonts[(int)font].SpriteFont, s, textPos, color,
                    0.0f, textOrigin, 1.0f, SpriteEffects.None, 0.0f);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime)
          {
               foreach (TextObject textObj in textObjects)
               {
                    textObj.Update(gameTime);
               }
          }

          #endregion


          #region Public Methods

          /// <summary>
          /// Returns number of TextObjects in TextHandler.
          /// </summary>
          public static int Size()
          {
               return textObjects.Count;
          }

          public static void SetScaleRange(int start, int amount, float scale)
          {
               if (start < 0 || (start + amount > textObjects.Count))
                    return;

               for (int i = start; i < amount; i++)
               {
                    textObjects[i].Scale = scale;
               }
          }

          /// <summary>
          /// Sets the AnimationEffect of all TextObjects to the same AnimationEffect.
          /// </summary>
          public static void SetAnimationAll()
          {
               foreach (TextObject textObj in textObjects)
               {
                    //textObj.TextEffect = new TypingEffect(2000.0f, textObj.Text);
                    textObj.AddTextEffect(new TypingEffect(2000.0f, textObj.Text));
               }
          }

          #endregion


          #region Formatting Methods - Wrap Text

          /// <summary>
          /// Returns a string that has newline characters (\n) inserted into it at the points where
          /// they would need to go to wrap a text string to a certain width.
          /// </summary>
          /// <param name="Text">The string to be wrapped.</param>
          /// <param name="FontName">The Font Type we are using on the string.</param>
          /// <param name="MaxLineWidth">The maximum width we want each line.</param>
          /// <returns></returns>
          public static string WrapText(string Text, FontType FontName, float MaxLineWidth)
          {
               // Create an array (words) with one entry for each word in the passed text string
               string[] words = Text.Split(' ');

               // A StringBuilder lets us add to a string and finally return the result
               StringBuilder sb = new StringBuilder();

               // How long is the line we are currently working on so far
               float lineWidth = 0.0f;

               // Store a measurement of the size of a space in the font we are using.
               float spaceWidth = Fonts[(int)FontName].SpriteFont.MeasureString(" ").X;

               // Loop through each word in the string
               foreach (string word in words)
               {
                    Vector2 size;
                    size = Fonts[(int)FontName].SpriteFont.MeasureString(word);

                    // If this word will fit on the current line, add it and keep track
                    // of how long the line has gotten.
                    if (lineWidth + size.X < MaxLineWidth)
                    {
                         sb.Append(word + " ");
                         lineWidth += size.X + spaceWidth;
                    }
                    else
                    // otherwise, append a newline character to start a new line.  Add the
                    // word and a space, and set the size of the new line.
                    {
                         sb.Append("\n" + word + " ");
                         lineWidth = size.X + spaceWidth;
                    }
               }
               // return the resultant string
               return sb.ToString();
          }
          
          #endregion





          public static void Draw(SpriteFont font, string text, Vector2 position, Color color,
               float rotation, Vector2 origin, float scale)
          {
               MySpriteBatch.DrawString(font, text, position, color, rotation,
                         origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);
          }













          public static void Draw(AlignmentType alignmentType, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale)
          {
               switch (alignmentType)
               {
                    case AlignmentType.Left:

                         MySpriteBatch.DrawString(font, text, position, color, rotation, origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);

                         break;

                    case AlignmentType.Center:

                         TextManager.DrawCentered(false, font, text, position, color, scale);

                         break;

                    case AlignmentType.Right:

                         float lineWidth = font.MeasureString(text).X * scale;
                         Vector2 alignedPosition = new Vector2(position.X - lineWidth, position.Y);

                         MySpriteBatch.DrawString(font, text, alignedPosition, color, rotation, origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);

                         break;
               }
          }






          /// <summary>
          /// Draws (using SpriteBatch.DrawString()) a \n-separated string to where each
          /// line is centered on Position, rather than being centered but left-aligned
          /// with one another.
          /// </summary>
          public static void DrawCentered(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;


               Vector2 startingPosition = new Vector2(position.X, position.Y - (font.LineSpacing * (size - 1) / 2f));

               foreach (string line in lines)
               {
                    origin = font.MeasureString(line) / 2f;

                    MySpriteBatch.DrawString(font, line, startingPosition, color,
                         0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

                    startingPosition.Y += font.LineSpacing;
               }

               /*
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                         ((float)(size - 1) / 2) * font.LineSpacing - (font.LineSpacing / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing);
               }

               foreach (string line in lines)
               {
                    origin = new Vector2(font.MeasureString(line).X / 2, font.LineSpacing / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

                    position.Y += font.LineSpacing;
               }
               */
          }

          public static void DrawCenteredOld(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color, float scale)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                          ((float)(size - 1) / 2f) * (font.MeasureString("a").Y * scale) - ((font.MeasureString("a").Y * scale) / 2f));
                    //((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing * scale);
               }

               foreach (string line in lines)
               {
                    origin = font.MeasureString(line) / 2f;//new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    position.Y += scale * font.LineSpacing;
               }
          }

          public static void DrawCentered(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color, float scale)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;


               Vector2 startingPosition = new Vector2(position.X, position.Y - (font.LineSpacing * scale * (size - 1) / 2f));

               foreach (string line in lines)
               {
                    origin = font.MeasureString(line) / 2f;

                    MySpriteBatch.DrawString(font, line, startingPosition, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    startingPosition.Y += scale * font.LineSpacing;
               }

               /*
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                          ((float)(size - 1) / 2f) * (font.MeasureString("a").Y * scale) - ((font.MeasureString("a").Y * scale) / 2f));
                    //((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing * scale);
               }

               foreach (string line in lines)
               {
                    origin = font.MeasureString(line) / 2f;//new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    position.Y += scale * font.LineSpacing;
               }
               */
          }

          /// <summary>
          /// Draws (using SpriteBatch.DrawString()) a \n-separated string to where each
          /// line is centered on Position, rather than being centered but left-aligned
          /// with one another.
          /// Unlike DrawCentered, this method re-positions the text as needed, to ensure
          /// the middle line of the text is always in the center of the screen.
          /// </summary>
          public static void DrawAutoCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
          {
               if (text == null)
                    text = "";

               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;


               Vector2 startingPosition = new Vector2(position.X, position.Y - (font.LineSpacing * scale * (size - 1) / 2f));

               foreach (string line in lines)
               {
                    origin = font.MeasureString(line) / 2f;

                    MySpriteBatch.DrawString(font, line, startingPosition, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    startingPosition.Y += scale * font.LineSpacing;
               }

               /*
               // Added this recently to prevent null text.
               if (text == null)
                    text = "";

               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;
               position = new Vector2(position.X, position.Y -
                    ((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));

               foreach (string line in lines)
               {
                    origin = new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    position.Y += scale * font.LineSpacing;
               }
               */
          }

          public static void DrawCentered(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color,
               float rotation, Vector2 textOrigin, float scale, SpriteEffects spriteEffects, float depthLayer)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;


               Vector2 startingPosition = new Vector2(position.X, position.Y - (font.LineSpacing * scale * (size - 1) / 2f));

               foreach (string line in lines)
               {
                    origin = font.MeasureString(line) / 2f;

                    MySpriteBatch.DrawString(font, line, startingPosition, color,
                         rotation, origin, scale, spriteEffects, depthLayer);

                    startingPosition.Y += scale * font.LineSpacing;
               }

               /*
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                         ((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing * scale);
               }

               foreach (string line in lines)
               {
                    origin = new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         rotation, origin, scale, spriteEffects, depthLayer);

                    position.Y += scale * font.LineSpacing;
               }
               */
          }

          public static string RemoveColorFormatText(string text)
          {
               List<string[]> stringList = new List<string[]>();

               // This removes the '[color:' string.
               string[] splits = text.Split(new string[] { "[color:" }, StringSplitOptions.RemoveEmptyEntries);

               foreach (var str in splits)
               {
                    // If this section starts with a color...
                    if (str.StartsWith("#"))
                    {
                         // #AARRGGBB
                         // #FFFFFFFFF
                         // #123456789
                         string color = str.Substring(0, 9);

                         // Substring after the hex code, and remove the [/color] tag.
                         string[] msgs = str.Substring(10).Split(new string[] { "[/color]" }, StringSplitOptions.RemoveEmptyEntries);

                         stringList.Add(msgs);
                    }

                    else
                    {
                         string[] addThis = new string[] { str };
                         stringList.Add(addThis);
                    }
               }

               string returnString = "";

               foreach (var str in stringList)
               {
                    foreach (var wut in str)
                    {
                         returnString += wut;
                    }
               }

               return returnString;
          }

          public static string GetFirstSubstring(string text)
          {
               // This removes the '[color:' string.
               string[] splits = text.Split(new string[] { "[color:" }, StringSplitOptions.RemoveEmptyEntries);

               foreach (var str in splits)
               {
                    // If this section starts with a color...
                    if (str.StartsWith("#"))
                    {
                    }

                    else
                    {
                         return str;
                    }
               }

               return "";
          }

          public static void DrawColorFormattedText(AlignmentType alignmentType, SpriteFont font, Vector2 position, string text, float scale, bool isCentered)
          {
               Vector2 renderPosition = position;

               if (alignmentType == AlignmentType.Center)
               {
                    int width = (int)(font.MeasureString(RemoveColorFormatText(text)).X * scale);

                    int widthOfFirstSubstring = (int)(font.MeasureString(GetFirstSubstring(text)).X * scale);

                    // First position Minus the substrings
                    width = (width - widthOfFirstSubstring) / 2;

                    renderPosition = new Vector2(renderPosition.X - width, renderPosition.Y);
               }













               Color defaultColor = Color.White;

               // Only bother if we have color commands involved.
               if (text.Contains("[color:"))
               {
                    // how far in x to offset from position
                    int currentOffset = 0;

                    // example:
                    // string.Format("You attempt to hit the [color:#FFFF0000]{0}[/color] but [color:{1}]MISS[/color]!",
                    // currentMonster.Name, Color.Red.ToHex(true));
                    string[] splits = text.Split(new string[] { "[color:" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var str in splits)
                    {
                         // If this section starts with a color...
                         if (str.StartsWith("#"))
                         {
                              // #AARRGGBB
                              // #FFFFFFFFF
                              // #123456789
                              string color = str.Substring(0, 9);

                              // Any subsequent msgs after the [/color] tag are defaultColor
                              string[] msgs = str.Substring(10).Split(new string[] { "[/color]" }, StringSplitOptions.RemoveEmptyEntries);

                              // Always draw [0] there should be at least one
                              TextManager.Draw(alignmentType, font, msgs[0], position + new Vector2(currentOffset, 0), color.ToColor(), 0f, Vector2.Zero, scale);

                              if (alignmentType == AlignmentType.Center)
                              {
                                   currentOffset += (int)(font.MeasureString(msgs[0]).X * scale) / 1;

                                   if (msgs.Length == 2)
                                   currentOffset += (int)(font.MeasureString(msgs[1]).X * scale) / 1;
                              }

                              else
                              {
                                   currentOffset += (int)(font.MeasureString(msgs[0]).X * scale);
                              }

                              // There should only ever be one other string or none.
                              if (msgs.Length == 2)
                              {
                                   TextManager.Draw(alignmentType, font, msgs[1], position + new Vector2(currentOffset, 0), defaultColor, 0f, Vector2.Zero, scale);
                                   currentOffset += (int)(font.MeasureString(msgs[1]).X * scale);
                              }
                         }
                         else
                         {
                              TextManager.Draw(alignmentType, font, str, renderPosition + new Vector2(currentOffset, 0), defaultColor, 0f, Vector2.Zero, scale);

                              if (alignmentType == AlignmentType.Center)
                              {
                                   currentOffset += (int)(font.MeasureString(str).X * scale / 1);
                              }

                              else
                              {
                                   currentOffset += (int)(font.MeasureString(str).X * scale);
                              }
                         }
                    }
               }
               else
               {
                    // just draw the string as ordered
                    TextManager.Draw(alignmentType, font, text, position, defaultColor, 0f, Vector2.Zero, scale);
               }
          }









          public static void DrawCenteredTest(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color, float scale)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                         ((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing * scale);
               }

               foreach (string line in lines)
               {
                    origin = new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawStringTest(font, line, position, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    position.Y += scale * font.LineSpacing;
               }
          }
     }
}