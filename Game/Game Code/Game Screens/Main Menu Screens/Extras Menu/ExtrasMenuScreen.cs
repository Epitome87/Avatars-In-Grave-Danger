#region File Description
//-----------------------------------------------------------------------------
// ExtrasMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A MenuScreen which contains a list of other MenuScreens, 
     /// which represent "Extras" the Player can opt to view.
     /// 
     /// "Extras" include the Credits screen, the Awards screen, and the Tell A Friend screen.
     /// </remarks>
     public class ExtrasMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry creditsMenuEntry;
          MenuEntry awardsMenuEntry;
          MenuEntry tellFriendMenuEntry;
          MenuEntry otherGamesMenuEntry;
          MenuEntry backMenuEntry;

          #endregion


          #region Initialization

          public ExtrasMenuScreen()
               : base("E X T R A S")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.WastingTime;
               }

               // Create our menu entries.
               creditsMenuEntry = new MenuEntry("View\nCredits", "See Who Worked On This Game!");
               awardsMenuEntry = new MenuEntry("View\nAwards", "View the Awards You Have Earned!");
               tellFriendMenuEntry = new MenuEntry("Tell\nA Friend!", "Tell Friends About This Game!\nPlease?!");
               otherGamesMenuEntry = new MenuEntry("Our\nGames!", "View Our Previously Released\n& Upcoming Games!");
               backMenuEntry = new MenuEntry("Go\nBack", "Return to the Main Menu.");

               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y);

               // Hook up menu event handlers.
               creditsMenuEntry.Selected += CreditsMenuEntrySelected;
               awardsMenuEntry.Selected += AwardsMenuEntrySelected; ;
               tellFriendMenuEntry.Selected += TellFriendMenuEntrySelected;
               otherGamesMenuEntry.Selected += OtherGamesMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               MenuEntries.Add(awardsMenuEntry);
               MenuEntries.Add(creditsMenuEntry);
               MenuEntries.Add(tellFriendMenuEntry);
               MenuEntries.Add(otherGamesMenuEntry);
               //MenuEntries.Add(backMenuEntry);

               this.numberOfColumns = 2;

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.Position = new Vector2(entry.Position.X - 250, entry.Position.Y + 150);


                    entry.AdditionalVerticalSpacing = 35;
                    entry.menuEntryBorderScale = new Vector2(1.0f, 1.0f);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;

                    entry.ShowGradientBorder = true;
                    entry.ShowBorder = false;

                    entry.menuEntryBorderSize = new Vector2(200, 200);
                    entry.useCustomBorderSize = true;
                    entry.FontScale = 0.75f;
                    entry.ShowIcon = false;

                    entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                    entry.SelectedBorderColor = Color.DarkOrange * (255f / 255f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.32f;

                    entry.DescriptionColor = Color.DarkOrange;
                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.32f;
               }

               this.MenuTitleColor = Color.DarkOrange;

               backMenuEntry.IsPulsating = true;
          }

          #endregion


          #region Menu Events

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void AwardsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new AwardsScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void UnlockablesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               //ScreenManager.AddScreen(new UnlockablesScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Tell A Friend menu entry is selected.
          /// </summary>
          void TellFriendMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (Gamer.SignedInGamers[e.PlayerIndex] == null)
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer,
                         "Feature Unavailable", "This feature requires a Player Profile that is signed into XBOX Live.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               if (Gamer.SignedInGamers[e.PlayerIndex].IsGuest ||
                   !Gamer.SignedInGamers[e.PlayerIndex].IsSignedInToLive)
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer,
                         "Feature Unavailable",
                         "This feature requires a Player Profile that is signed into XBOX Live. The Player Profile also cannot be a Guest Profile.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               Guide.ShowComposeMessage(e.PlayerIndex,
                    "Check out 'Avatars In Grave Danger' on the Indie Marketplace! It's only $1 for zombie-fueled action, and all of the profits go towards future updates & improvements. Visit Game Marketplace - Games & Demos - Indie Games & try the free demo!", 
                    Gamer.SignedInGamers[e.PlayerIndex].GetFriends());
          }

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new ZombieCreditsScreen(), e.PlayerIndex);
          }


          void OtherGamesMenuEntrySelected(Object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new OtherGamesMenuScreen(), e.PlayerIndex);
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               base.OnCancel(playerIndex);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (AvatarZombieGame.CurrentPlayer == null)
               {
                    tellFriendMenuEntry.UnselectedColor = Color.Gray;
                    tellFriendMenuEntry.SelectedColor = Color.Gray;

                    return;
               }

               // Handle Guest / Non-Live Profile Menu Entry Disabling.
               if (AvatarZombieGame.CurrentPlayer != null & AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer != null)
               {
                    if (!AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsSignedInToLive ||
                         AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsGuest)
                    {
                         tellFriendMenuEntry.UnselectedColor = Color.Gray;
                         tellFriendMenuEntry.SelectedColor = Color.Gray;
                    }

                    else
                    {
                         tellFriendMenuEntry.UnselectedColor = Color.White;
                         tellFriendMenuEntry.SelectedColor = Color.White;
                    }
               }

               /*
               if (Guide.IsVisible)
               {
                    guideWasVisible = true;
               }

               if (guideWasVisible && !Guide.IsVisible)
               {
                    AvatarZombieGame.AwardData.ToldPeopleAboutGame = true;
               }
               */
          }

          //bool guideWasVisible = false;

          #endregion
     }
}