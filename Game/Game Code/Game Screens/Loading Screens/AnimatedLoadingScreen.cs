#region File Description
//-----------------------------------------------------------------------------
// AnimatedLoadingScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// Derives from LoadingScreen, adding the use of  
     /// threads for animated loading screen support.
     /// </summary>
     public class AnimatedLoadingScreen : GameScreen
     {
          #region Fields

          /// <summary>
          /// Whether or not we believe loading is going to be slow.
          /// 
          /// If false, we won't even bother displaying a loading message.
          /// </summary>
          private bool loadingIsSlow;

          /// <summary>
          /// Whether or not the other screens have finished transitioning off.
          /// </summary>
          private bool otherScreensAreGone;

          /// <summary>
          /// An array of the screens we are going to be loading.
          /// </summary>
          private GameScreen[] screensToLoad;

          /// <summary>
          /// The "Loading" text to render while we are loading.
          /// </summary>
          private string loadingMessage;

          /// <summary>
          /// A custom message text to render while we are loading.
          /// </summary>
          private static string customMessage;
          private static bool IsCustomMessage;

          /// <summary>
          /// Whether or not we are going to remove other screens from the Screen Manager before loading.
          /// </summary>
          private static bool IsRemoveScreens;

          /// <summary>
          /// Whether or not we have actually began loading.
          /// </summary>
          private bool isLoading;

          /// <summary>
          /// Whether or not Tips are enabled (displayed).
          /// </summary>
          private static bool enableTip;

          private Thread updateThread;
          private bool threadFinished;          
          
          private GameResourceTexture2D backgroundTexture;
          private GameResourceTexture2D whiteGradientTexture;

          private Random random = new Random();

          /// <summary>
          /// An array of Tip strings to present during Loading.
          /// </summary>
          private string[] tips = 
          {
               "Try not sucking\nas much.",
               "Confused about the controls?\nSee the Controls Menu\nby Pausing the game.",
               "Sometimes it's okay\nto just give up.",
               "Getting owned?\nPlay on a lower Difficulty.",
               "Eliminate faster zombies first.",
               "Zombies traveling horizontally can drop\nHealth, Grenades, or Cash when defeated.",
               "Unlocking Awards can\nunlock hidden features.",
               "View your Awards in the\nExtras menu.",
               "Suggestions? Feedback?\nVisit Us at:\nFacebook.com/PixelPsycheStudios",
               "Visit Ned's Shop\nto improve your arsenal!",
               "A single Grenade can eliminate\nan entire horde of zombies!",
          };

          /// <summary>
          /// The current Tip string to display.
          /// </summary>
          private string tip = String.Empty;

          /// <summary>
          /// Helper variable to keep track of how long
          /// the loading process has taken.
          /// </summary>
          private float elapsedLoadingTime = 0.0f;

          #endregion


          #region Initialization

          /// <summary>
          /// The constructor is private: loading screens should
          /// be activated via the static Load method instead.
          /// </summary>
          private AnimatedLoadingScreen(bool loadingIsSlow, GameScreen[] screensToLoad, bool? showTip)
          {
               tip = /*"Tip: " +*/ tips[random.Next(tips.Length)];

               TextManager.Reset();

               this.loadingIsSlow = loadingIsSlow;
               this.screensToLoad = screensToLoad;

               TransitionOnTime = TimeSpan.FromSeconds(0.0);
               TransitionOffTime = TimeSpan.FromSeconds(0.0);

               if (IsCustomMessage)
               {
                    loadingMessage = customMessage;
                    IsRemoveScreens = false;
               }

               else
               {
                    loadingMessage = "L o a d i n g. . .";
                    IsRemoveScreens = true;
               }

               isLoading = false;
               threadFinished = false;
               enableTip = true;

               updateThread = new Thread(new ThreadStart(ThreadUpdateMethod));
          }

          /// <summary>
          /// Activates the loading screen.
          /// </summary>
          public static void Load(bool loadingIsSlow,
                                  PlayerIndex? controllingPlayer,
                                  params GameScreen[] screensToLoad)
          {
               // Tell all the current screens to transition off.
               foreach (GameScreen screen in ScreenManager.GetScreens())
               {
                    if (screen.GetType() != typeof(BackgroundScreen))
                         screen.ExitScreen();
               }

               AnimatedLoadingScreen.IsCustomMessage = false;

               // Create and activate the loading screen.
               AnimatedLoadingScreen loadingScreen = new AnimatedLoadingScreen(loadingIsSlow,
                                                               screensToLoad, enableTip);

               ScreenManager.AddScreen(loadingScreen, controllingPlayer);
          }

          public static void RemoveAllButGameplayScreen()
          {
               foreach (GameScreen screen in ScreenManager.GetScreens())
               {
                    if (screen.GetType() != typeof(GameplayBackgroundScreen))
                    {
                         //screen.ExitScreen();
                         ScreenManager.RemoveScreen(screen);
                    }
               }
          }

          /// <summary>
          /// Activates the loading screen.
          /// </summary>
          public static void Load(bool loadingIsSlow,
                                  PlayerIndex? controllingPlayer, string customMessage,
               bool removeScreens, params GameScreen[] screensToLoad)
          {
               // Tell all the current screens to transition off.
               if (removeScreens)
               {
                    foreach (GameScreen screen in ScreenManager.GetScreens())
                    {
                         if (screen.GetType() != typeof(GameplayBackgroundScreen))
                              ScreenManager.RemoveScreen(screen);// screen.ExitScreen();
                    }
               }

               AnimatedLoadingScreen.IsCustomMessage = true;
               AnimatedLoadingScreen.customMessage = customMessage;

               // Create and activate the loading screen.
               AnimatedLoadingScreen loadingScreen = new AnimatedLoadingScreen(loadingIsSlow,
                                                               screensToLoad, enableTip);

               ScreenManager.AddScreen(loadingScreen, controllingPlayer);
          }

          GameResourceTexture2D blankTexture;

          public override void LoadContent()
          {
               backgroundTexture = ResourceManager.LoadTexture(@"Textures\Backgrounds\Graveyard_Background");

               whiteGradientTexture = ResourceManager.LoadTexture(@"Menus\WhiteGradient");
      
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the animated loading screen.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (otherScreensAreGone && !isLoading)
               {
                    // We are now loading, so set the flag to True.
                    isLoading = true;

                    // And begin the update thread.
                    updateThread.Start();
               }

               // If our thread has finished...
               if (threadFinished)
               {
                    // Release the thread.
                    updateThread = null;
                    
                    // Remove the animated loading screen.
                    ScreenManager.RemoveScreen(this);

                    // We want to show the loading screen for at least 3 seconds...
                    // So we see how much time was spent loading and subtract that from 3.
                    float secondsToSleep = 3.0f - elapsedLoadingTime;

                    secondsToSleep = secondsToSleep > 0 ? secondsToSleep : 0;

                    if (loadingIsSlow)
                    {
                         // So we Sleep if necessary.
                         Thread.Sleep((int)secondsToSleep * 1000);
                    }
               }

               // Update the elapsed loading time.
               elapsedLoadingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
          }

          #endregion


          #region Draw

          private uint periodFrame = 0;
          private string periodText = "";

          /// <summary>
          /// Draws the loading screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               // If we are the only active screen, that means all the previous screens
               // must have finished transitioning off. We check for this in the Draw
               // method, rather than in Update, because it isn't enough just for the
               // screens to be gone: in order for the transition to look good we must
               // have actually drawn a frame without them before we perform the load.

               if (IsRemoveScreens)
               {
                    if ((ScreenState == ScreenState.Active) &&
                        (ScreenManager.GetScreens().Length == 1))
                    {
                         otherScreensAreGone = true;
                    }
               }

               else
               {
                    if ((ScreenState == ScreenState.Active))
                    {
                         otherScreensAreGone = true;
                    }
               }
               
               // The gameplay screen takes a while to load, so we display a loading
               // message while that is going on, but the menus load very quickly, and
               // it would look silly if we flashed this up for just a fraction of a
               // second while returning from the game to the menus. This parameter
               // tells us how long the loading is going to take, so we know whether
               // to bother drawing the message.
               if (loadingIsSlow)
               {
                    SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                    SpriteFont font = TextManager.Fonts[(int)FontType.HudFont].SpriteFont;// ScreenManager.Font;

                    // This is new
                    ScreenManager.GraphicsDevice.Clear(Color.Black);
                    // End this is new

                    MySpriteBatch.Begin();

   
                    // Draw the backdrop.
                    MySpriteBatch.Draw(backgroundTexture.Texture2D,
                         new Rectangle(0, 0, EngineCore.GraphicsDevice.Viewport.Width, EngineCore.GraphicsDevice.Viewport.Height), Color.White);




                    Vector2 widthHeight = font.MeasureString(loadingMessage) * 1.5f;
                    Rectangle rect = new Rectangle((int)EngineCore.ScreenCenter.X, (int)(EngineCore.ScreenCenter.Y * 0.60f), (int)widthHeight.X + 10, (int)widthHeight.Y + 10);

                    GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, rect, 4, Color.Black);
                    MySpriteBatch.DrawCentered(whiteGradientTexture.Texture2D, rect, Color.Gray * (200f / 255f));
                    TextManager.DrawCentered(false, font, loadingMessage, new Vector2(rect.X, rect.Y), Color.White, 1.5f);




                    Vector2 tipWidthHeight = font.MeasureString(tip) * 1f;
                    Rectangle tipRectangle = new Rectangle((int)EngineCore.ScreenCenter.X, (int)(EngineCore.ScreenCenter.Y * 1.25f), (int)tipWidthHeight.X + 10, (int)tipWidthHeight.Y + 10);

                    // Draw the Tip Area - rectangular boxes along with the tip text itself.
                    GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, tipRectangle, 4, Color.Black);
                    MySpriteBatch.DrawCentered(whiteGradientTexture.Texture2D, tipRectangle, Color.White * (200f / 255f));


                    if (enableTip)
                    {
                         TextManager.DrawCentered(true, font, tip, new Vector2(tipRectangle.X, tipRectangle.Y), Color.Gray, 1f);
                    }

                    string loadingText = "Loading";

                    periodFrame++;
                    periodFrame = periodFrame % (4 * 10);

                    periodText = "";

                    for (int i = 0; i < (periodFrame / 10); i++)
                    {
                         periodText += ".";
                    }

                    // Render the "Loading" text.
                    TextManager.DrawString(font, loadingText + periodText,
                              new Vector2(EngineCore.ScreenCenter.X * 1.25f, EngineCore.ScreenCenter.Y * 1.65f), Color.White, 1.25f);

                    MySpriteBatch.End();
               }
          }

          #endregion


          #region Thread Updating Method

          /// <summary>
          /// Updates our loading thread through use of threading and delegates.
          /// </summary>
          protected void ThreadUpdateMethod()
          {
#if XBOX
               updateThread.SetProcessorAffinity(new[] { 5 });
#endif

               AvatarZombieGame.Game.Exiting += delegate
               {
                    if (updateThread != null)
                    {
                         // Abort the update thread if game is exiting.
                         updateThread.Abort();
                    }
               };

               // Iterate through all the GameScreens to be loaded...
               foreach (GameScreen screen in screensToLoad)
               {
                    if (screen != null)
                    {
                         // Add each screen to load to the ScreenManager.
                         // This will automatically make calls to their LoadContent methods.
                         ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
               }

               // The thread has completed its task.
               threadFinished = true;
          }

          #endregion
     }
}
