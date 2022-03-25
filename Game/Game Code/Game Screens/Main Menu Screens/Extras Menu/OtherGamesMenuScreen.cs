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
using PixelEngine;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A MenuScreen which 
     /// </remarks>
     public class OtherGamesMenuScreen : MenuScreen
     {
          #region Fields

          /// <summary>
          /// The Back Menu Entry.
          /// </summary>
          MenuEntry backMenuEntry;

          /// <summary>
          /// The Avatar Typing Game Box.
          /// Could also be just a Model3D.
          /// </summary>
          SceneObject avatarTypingBox = new SceneObject();

          /// <summary>
          /// The Avatars In Grave Danger Game Box.
          /// Could also be just a Model3D.
          /// </summary>
          SceneObject zombieBox = new SceneObject();

          /// <summary>
          /// Used to slowly rotate our game covers.
          /// </summary>
          float rotation = 0.0f;

          GameResourceTexture2D backgroundTexture;

          #endregion


          #region Initialization

          public OtherGamesMenuScreen()
               : base("Games By Pixel Pysche")
          {
               /*
               this.menuHeader = new MenuHeader("HEY", new Vector2(EngineCore.ScreenCenter.X, 100), FontType.TitleFont, 1.0f, Color.Pink);
               this.menuFooter = new MenuFooter("PAGE NUMBER", new Vector2(EngineCore.ScreenCenter.X, 600), FontType.MenuFont, 1.5f, Color.Yellow);
               this.menuDescription = new MenuDescription("DESCRIPTION HERE", new Vector2(EngineCore.ScreenCenter.X * 1.5f, EngineCore.ScreenCenter.Y * 1.5f), FontType.HudFont, 1.0f, Color.White);
               */

               // FOR GAME COVER
               avatarTypingBox.Model.LoadContent(@"Models\Game Covers\GameBox_AvatarTyping");
               avatarTypingBox.Position = new Vector3(0, 1.5f, 0.5f);
               avatarTypingBox.Rotation = new Quaternion(0f, avatarTypingBox.Rotation.Y, avatarTypingBox.Rotation.Z, 0f); //-90
               avatarTypingBox.Rotation = new Quaternion(avatarTypingBox.Rotation.X, 180f - 45f, avatarTypingBox.Rotation.Z, 0f); // 180
               avatarTypingBox.Rotation = new Quaternion(avatarTypingBox.Rotation.X, avatarTypingBox.Rotation.Y, 180f, 0f);
               avatarTypingBox.World = Matrix.CreateScale(0.20f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(avatarTypingBox.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(avatarTypingBox.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(avatarTypingBox.Rotation.Z))
                    * Matrix.CreateTranslation(avatarTypingBox.Position);




               zombieBox.Model.LoadContent(@"Models\Game Covers\GameBox_GraveDanger");
               zombieBox.Position = new Vector3(0, 1.5f, 0.5f);
               zombieBox.Rotation = new Quaternion(0f, zombieBox.Rotation.Y, zombieBox.Rotation.Z, 0f); //-90
               zombieBox.Rotation = new Quaternion(zombieBox.Rotation.X, 180f - 45f, zombieBox.Rotation.Z, 0f); // 180
               zombieBox.Rotation = new Quaternion(zombieBox.Rotation.X, zombieBox.Rotation.Y, 180f, 0f);
               zombieBox.World = Matrix.CreateScale(0.20f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(zombieBox.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(zombieBox.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(zombieBox.Rotation.Z))
                    * Matrix.CreateTranslation(zombieBox.Position);


               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.WastingTime;
               }

               // Create our menu entries.
               backMenuEntry = new MenuEntry("View Game Description");
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50);

               // Hook up menu event handlers.
               //backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               MenuEntries.Add(backMenuEntry);


               /*
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50);
               AdvancedMenuEntry testMenu = new AdvancedMenuEntry("SIGH", "SIGH DESCRIPTION SIGH");
               testMenu.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50);
               MenuEntries.Add(testMenu);
               */
               
               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.ShowGradientBorder = false;
                    entry.ShowBorder = true;

                    entry.BorderColor = Color.CornflowerBlue * (0.5f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.35f;

                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.34f;
                    entry.DescriptionColor = Color.DarkOrange;

                    entry.Position = new Vector2(entry.Position.X, entry.DescriptionPosition.Y - 200f);

                    entry.ShowDescriptionBorder = false;
               }
               
               /*
               foreach (AdvancedMenuEntry entry in MenuEntries)
               {
                    entry.ShowGradientBorder = false;
                    entry.ShowBorder = true;

                    entry.BorderColor = Color.CornflowerBlue * (0.5f);

                    entry.FontType = FontType.TitleFont;
                    entry.FontScale *= 0.35f;

                    entry.DescriptionFontType = FontType.TitleFont;
                    entry.DescriptionFontScale *= 0.34f;
                    entry.DescriptionColor = Color.DarkOrange;

                    entry.ShowDescriptionBorder = true;
               }
               */
               backMenuEntry.IsPulsating = false;

               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);


               backgroundTexture = ResourceManager.LoadTexture(@"Textures\Gradients\Gradient_Star");
          }

          #endregion


          #region Menu Events

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               SceneGraphManager.sceneObjects.Remove(avatarTypingBox);

               base.OnCancel(playerIndex);
          }

          #endregion

          int currentGameSelected = 0;

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               base.HandleInput(input, gameTime);

               PlayerIndex playerIndex;

               if (isRotating)
                    return;

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.DPadRight, EngineCore.ControllingPlayer, out playerIndex))
               {
                    currentGameSelected++;

                    currentGameSelected %= 2;
               }

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.DPadLeft, EngineCore.ControllingPlayer, out playerIndex))
               {
                    currentGameSelected--;

                    if (currentGameSelected < 0)
                         currentGameSelected = 1;
               }

               if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.A, EngineCore.ControllingPlayer, out playerIndex))
               {
                    if (rotateToBack)
                    {
                         backMenuEntry.Text = "View Game Description";
                    }

                    else
                    {
                         backMenuEntry.Text = "View Game Cover";
                    }

                    isRotating = true;
                    rotateToBack = !rotateToBack;
               }
          }

          bool isRotating = false;
          bool rotateToBack = false;

          private void UpdateRotation(GameTime gameTime)
          {
               if (isRotating)
               {
                    if (rotateToBack)
                    {
                         rotation -= 2f;

                         if (rotation <= -180f)
                         {
                              rotation = -180f;
                              isRotating = false;
                         }
                    }

                    else
                    {
                         rotation += 2f;

                         if (rotation >= 0)
                         {
                              rotation = 0f;
                              isRotating = false;
                         }
                    }
               }
          }

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);
               
               MySpriteBatch.Begin();

               MySpriteBatch.Draw(backgroundTexture.Texture2D, new Rectangle(0, 0, EngineCore.GraphicsDevice.Viewport.Width, EngineCore.GraphicsDevice.Viewport.Height), Color.DarkOrange);
    
               MySpriteBatch.End();

                              switch (currentGameSelected)
               {
                    case 0:
                         avatarTypingBox.Draw(gameTime);
                         break;
                    case 1:
                         zombieBox.Draw(gameTime);
                         break;
               }

               base.Draw(gameTime);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               UpdateRotation(gameTime);


               avatarTypingBox.Rotation = new Quaternion(0f, 180f + rotation, 180f, 0f);          

               avatarTypingBox.World = Matrix.CreateScale(0.020f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(avatarTypingBox.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(avatarTypingBox.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(avatarTypingBox.Rotation.Z))
                    * Matrix.CreateTranslation(avatarTypingBox.Position);


               zombieBox.Rotation = new Quaternion(0f, 180f + rotation, 180f, 0f);

               zombieBox.World = Matrix.CreateScale(0.020f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(zombieBox.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(zombieBox.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(zombieBox.Rotation.Z))
                    * Matrix.CreateTranslation(zombieBox.Position);
          }

          #endregion
     }
}