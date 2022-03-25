#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// The Pause menu pops up over the game screen,
     /// giving the player options to resume or quit.
     /// </remarks>
     public class PauseMenuScreen : MenuScreen
     {
          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public PauseMenuScreen()
               : base("G a m e  P a u s e d")
          {
               // Flag that there is no need for the game to transition
               // off when the pause menu is on top of it.
               IsPopup = true;

               // Create our menu entries.
               MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", "Continue Playing.");
               MenuEntry optionsMenuEntry = new MenuEntry("Help & Options", "Get Help &\nCustomize Your Experience.");
               MenuEntry awardsMenuEntry = new MenuEntry("View Awards", "View the Awards You Have Earned!");
               MenuEntry unlockablesMenuEntry = new MenuEntry("View Unlockables", "View the Unlockables You Have...Unlocked");
               MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", "Quit & Return to the Main Menu.\nNote: Current Progress Will Be Lost.");

               // Hook up menu event handlers.
               resumeGameMenuEntry.Selected += OnCancel;
               optionsMenuEntry.Selected += HelpAndOptionsMenuEntrySelected;
               awardsMenuEntry.Selected += AwardsMenuEntrySelected;
               unlockablesMenuEntry.Selected += UnlockablesMenuEntrySelected;
               quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;




               // Add entries to the menu.
               MenuEntries.Add(resumeGameMenuEntry);
               MenuEntries.Add(optionsMenuEntry);
               MenuEntries.Add(awardsMenuEntry);
               MenuEntries.Add(unlockablesMenuEntry);
               MenuEntries.Add(quitGameMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 100f);
                    entry.AdditionalVerticalSpacing = 25f;
                    entry.IsPulsating = false;
                    entry.ShowBorder = false;
                    entry.SelectedColor = entry.UnselectedColor;


                    entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                    entry.SelectedBorderColor = Color.DarkOrange * (255f / 255f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.32f;

                    entry.DescriptionColor = Color.DarkOrange;
                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.32f;
               }

               this.MenuTitleColor = Color.DarkOrange;

               quitGameMenuEntry.IsPulsating = true;
          }

          #endregion


          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          private void HelpAndOptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new HelpAndOptionsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          private void AwardsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new AwardsScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          private void UnlockablesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new UnlockablesMenuScreen("Unlockables"), e.PlayerIndex);
          }

          #endregion


          #region Handle Input

          /// <summary>
          /// Event handler for when the Quit Game menu entry is selected.
          /// </summary>
          void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               const string message = "Are you sure you want to quit this game?";

               MessageBoxWithKeyboardScreen confirmQuitMessageBox = new MessageBoxWithKeyboardScreen(message);

               confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

               ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
          }

          /// <summary>
          /// Event handler for when the user selects ok on the "are you sure
          /// you want to quit" message box. This uses the loading screen to
          /// transition from the game back to the main menu screen.
          /// </summary>
          void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               // We need to remove the GameplayScreen. Sloppy solution but it works.
               foreach (GameScreen screen in ScreenManager.GetScreens())
               {
                    if (screen.GetType().Equals((typeof(ArcadeGameplayScreen))) || screen.GetType().Equals((typeof(ArcadeWithTutorialGameplayScreen))))
                    {
                         ScreenManager.RemoveScreen(screen);
                         break;
                    }
               }

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the pause menu screen. This darkens down the gameplay screen
          /// that is underneath us, and then chains to the base MenuScreen.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 5);

               base.Draw(gameTime);
          }

          #endregion
     }
}
