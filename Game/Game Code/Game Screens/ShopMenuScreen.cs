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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.CameraSystem;
using PixelEngine;
using PixelEngine.Avatars;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A MenuScreen which contains a list of other MenuScreens, 
     /// which represent "Extras" the Player can opt to view.
     /// 
     /// "Extras" include the Credits screen, the Awards screen, and the Tell A Friend screen.
     /// </remarks>
     public class ShopMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry gunUpgradeMenuEntry;
          MenuEntry playerUpgradeMenuEntry;
          MenuEntry backMenuEntry;


          SceneObject gunModel = new SceneObject();
          float rotation = 0.0f;

          Avatar playersAvatar = new Avatar(EngineCore.Game);
          Avatar storeKeeperAvatar = new Avatar(EngineCore.Game);

          #endregion

          #region Initialization

          public ShopMenuScreen()
               : base("Shop")
          {
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.FirstPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 0f, -5f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);

               // FOR GUN MODEL
               gunModel.Model.LoadContent(@"Models\LaserGun");
               gunModel.Position = new Vector3(2f, 1.5f, 0.5f);
               gunModel.Rotation = new Quaternion(0f, gunModel.Rotation.Y, gunModel.Rotation.Z, 0f);
               gunModel.Rotation = new Quaternion(gunModel.Rotation.X, 0f, gunModel.Rotation.Z, 0f); 
               gunModel.Rotation = new Quaternion(gunModel.Rotation.X, gunModel.Rotation.Y, 0f, 0f);
               gunModel.World = Matrix.CreateScale(0.20f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(gunModel.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(gunModel.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(gunModel.Rotation.Z))
                    * Matrix.CreateTranslation(gunModel.Position);


               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.WastingTime;
               }


               // Create our menu entries.
               gunUpgradeMenuEntry = new MenuEntry("Upgrade Gun");
               playerUpgradeMenuEntry = new MenuEntry("Upgrade Player");

               backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.");
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50);

               // Hook up menu event handlers.
               backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               MenuEntries.Add(gunUpgradeMenuEntry);
               MenuEntries.Add(playerUpgradeMenuEntry);
               MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 75);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
               }

               backMenuEntry.IsPulsating = true;


               playersAvatar = new Avatar(ScreenManager.Game, new Vector3(0f, 0f, 0f));
               playersAvatar.LoadUserAvatar(PlayerIndex.One);
               playersAvatar.PlayAnimation(AvatarManager.LoadedPresetAvatarAnimation["Wave"], true);

               storeKeeperAvatar = new Avatar(ScreenManager.Game, new Vector3(-2f, 0f, 0f));
               storeKeeperAvatar.Scale = 3f;
               storeKeeperAvatar.LoadAvatar(CustomAvatarType.Merchant);
               storeKeeperAvatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, true);
          }

          #endregion

          #region Menu Events

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void GunMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               
          }

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void PlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               SceneGraphManager.sceneObjects.Remove(gunModel);

               base.OnCancel(playerIndex);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               //ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 3 / 5);
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               storeKeeperAvatar.Draw(gameTime);


               if (this.SelectedMenuEntry == 0)
               {               
                    MySpriteBatch.Begin();
                    gunModel.Draw(gameTime);
                    MySpriteBatch.End();
               }

               if (this.SelectedMenuEntry == 1)
               {
                    //AvatarTypingGame.CurrentPlayer.Avatar.Draw(gameTime);
                    playersAvatar.Draw(gameTime);// ToScreen(gameTime, new Vector3(0, 0, -5f), new Vector3(0, 0, 0));
               }

               base.Draw(gameTime);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               storeKeeperAvatar.Update(gameTime);


               rotation += 0.50f;

               // If Gun higlighted
               if (this.SelectedMenuEntry == 0)
               {
                    gunModel.Rotation = new Quaternion(gunModel.Rotation.X, 0f + rotation, gunModel.Rotation.Z, 0f);
                    gunModel.World = Matrix.CreateScale(0.020f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(gunModel.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(gunModel.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(gunModel.Rotation.Z))
                         * Matrix.CreateTranslation(gunModel.Position);
               }

               // If Player highlighted.
               if (this.SelectedMenuEntry == 1)
               {
                    playersAvatar.Position = new Vector3(0f, 0f, 0f);
                    playersAvatar.Rotation = new Vector3(0f, rotation, 0f);
                    playersAvatar.Update(gameTime);
               }
          }

          #endregion
     }
}