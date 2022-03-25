
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A Menu Screen which presents menu entries for viewing
     /// Options, Controls, and How to Play.
     /// </remarks>
     public class MultiplayerMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry racingMenuEntry;
          MenuEntry backMenuEntry;

          PlayerBackgroundScreen playerScreen;

          public static bool IsModeSelected = false;
          public static int ModeSelected = 0;

          private AvatarBaseAnimation defaultAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.Wave);
          private AvatarCustomAnimation racingAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["Run"]);
          private AvatarCustomAnimation otherAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["Faint"]);

          #endregion

          #region Initialization

          public MultiplayerMenuScreen()
               : base("Multiplayer")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.Multiplayer;
               }

               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);


               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.5f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.StartingGame;
               }

               // Create our menu entries.
               racingMenuEntry = new MenuEntry("Racing", "Race your Avatar against others online!\nWin by being the faster typer.");
               backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.");

               // Hook up menu event handlers.
               racingMenuEntry.Selected += HowToPlayMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               racingMenuEntry.Position = new Vector2(racingMenuEntry.Position.X + 200, racingMenuEntry.Position.Y + 35);
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X + 200, backMenuEntry.Position.Y + 60);


               racingMenuEntry.DescriptionPosition = new Vector2(racingMenuEntry.DescriptionPosition.X + 200, racingMenuEntry.DescriptionPosition.Y - 50);
               backMenuEntry.DescriptionPosition = new Vector2(backMenuEntry.DescriptionPosition.X + 200, backMenuEntry.DescriptionPosition.Y - 50);


               // Add entries to the menu.
               MenuEntries.Add(racingMenuEntry);
               MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.FontScale = 1.5f;
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
                    entry.ShowIcon = false;
               }

               playerScreen = new PlayerBackgroundScreen();
               ScreenManager.AddScreen(playerScreen, AvatarZombieGame.ControllingPlayer);

               playerScreen.playerPosition = new Vector3(0.95f, 1.15f - 0.15f, 0.0f);
               playerScreen.border = new Rectangle(200, 150, 350, 450);
               playerScreen.player.Avatar.AvatarAnimation = defaultAnimation;// new AvatarBaseAnimation(AvatarAnimationPreset.Wave);
          }

          #endregion

          #region Menu Events

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new HowToPlayScreen(), e.PlayerIndex);
          }

          protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               base.OnCancel(playerIndex);

               // Remove this screen and the screens it contains.
               ScreenManager.RemoveScreen(playerScreen);
          }

          #endregion

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (this.SelectedMenuEntry == 0)
               {
                    //playerScreen.player.Avatar.PlayAnimation(AnimationType.Run, true); 
                    playerScreen.player.Avatar.AvatarAnimation = racingAnimation;
               }

               else if (this.SelectedMenuEntry == 1)
               {
                    //playerScreen.player.Avatar.PlayAnimation(AnimationType.Throw, true);
                    playerScreen.player.Avatar.AvatarAnimation = defaultAnimation;
               }
          }
          #region Draw

          /// <summary>
          /// Draws the screen. This darkens down the gameplay screen
          /// that is underneath us, and then chains to the base MenuScreen.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion
     }
}