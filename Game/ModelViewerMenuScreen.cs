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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Text;
using PixelEngine.CameraSystem;
using PixelEngine;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A MenuScreen which contains a list of other MenuScreens, 
     /// which represent "Extras" the Player can opt to view.
     /// 
     /// "Extras" include the Credits screen, the Awards screen, and the Tell A Friend screen.
     /// </remarks>
     public class ModelViewerMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry selectModelMenuEntry;
          MenuEntry selectTransMenuEntry;
          MenuEntry selectLightingMenuEntry;

          int currentMode = 0;

          List<SceneObject> modelList = new List<SceneObject>();
          List<string> modelNameList = new List<string>();

          float rotation = 0.0f;

          #endregion

          #region Initialization

          public ModelViewerMenuScreen()
               : base("")
          {
               selectModelMenuEntry = new MenuEntry("Select Model");
               selectTransMenuEntry = new MenuEntry("Transformation Mode");
               selectLightingMenuEntry = new MenuEntry("Lighting Mode");

               selectModelMenuEntry.Selected += SelectModelMenuEntrySelected;
               selectTransMenuEntry.Selected += SelectTransMenuEntrySelected;
               selectLightingMenuEntry.Selected += SelectLightingMenuEntrySelected;

               MenuEntries.Add(selectModelMenuEntry);
               MenuEntries.Add(selectTransMenuEntry);
               MenuEntries.Add(selectLightingMenuEntry);

               modelNameList.Add(@"Castle");
               modelNameList.Add(@"GameBox");
               modelNameList.Add(@"Models\Ball");
               modelNameList.Add(@"Models\Ground");
               modelNameList.Add(@"Models\LaserGun");
               modelNameList.Add(@"Models\Tree");

               for (int i = 0; i < modelNameList.Count; i++)
               {
                    SceneObject sceneObj = new SceneObject();
                    sceneObj.Model.LoadContent(modelNameList[i]);

                    sceneObj.Position = new Vector3(0, 1.5f, 5.5f);
                    sceneObj.Rotation = new Quaternion(0f, sceneObj.Rotation.Y, sceneObj.Rotation.Z, 0f); //-90
                    sceneObj.Rotation = new Quaternion(sceneObj.Rotation.X, 180f - 45f, sceneObj.Rotation.Z, 0f); // 180
                    sceneObj.Rotation = new Quaternion(sceneObj.Rotation.X, sceneObj.Rotation.Y, 180f, 0f);





                    sceneObj.Rotation = new Quaternion(0f, sceneObj.Rotation.Y, sceneObj.Rotation.Z, 0f); //-90
                    sceneObj.Rotation = new Quaternion(sceneObj.Rotation.X, 180f - 45f, sceneObj.Rotation.Z, 0f); // 180
                    sceneObj.Rotation = new Quaternion(sceneObj.Rotation.X, sceneObj.Rotation.Y, 0f, 0f);





                    sceneObj.World = Matrix.CreateScale(1f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(sceneObj.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(sceneObj.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(sceneObj.Rotation.Z))
                         * Matrix.CreateTranslation(sceneObj.Position);



                    modelList.Add(sceneObj);
               }

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 0;
                    entry.Position = new Vector2(entry.Position.X - 400f, entry.Position.Y - 75f);
                    entry.IsPulsating = false;
                    entry.SelectedColor = Color.CornflowerBlue;
                    entry.ShowBorder = false;
                    entry.menuEntryBorderSize = new Vector2(0, 0);
                    entry.ShowIcon = false;
                    entry.FontScale = 0.5f;
               }
          }

          #endregion

          #region Menu Events

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          void SelectModelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               currentMode = 0;

          }

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          void SelectTransMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               MenuEntries.Clear();

               currentMode = 1;

               MenuEntry positionMenu = new MenuEntry("Position");
               MenuEntry rotationMenu = new MenuEntry("Rotation");
               MenuEntry scaleMenu = new MenuEntry("Scale");

               positionMenu.AdditionalVerticalSpacing = 0;
               positionMenu.Position = new Vector2(positionMenu.Position.X - 400f, positionMenu.Position.Y - 75f);
               positionMenu.IsPulsating = false;
               positionMenu.SelectedColor = Color.CornflowerBlue;
               positionMenu.ShowBorder = false;
               positionMenu.menuEntryBorderSize = new Vector2(0, 0);

               rotationMenu.AdditionalVerticalSpacing = 0;
               rotationMenu.Position = new Vector2(rotationMenu.Position.X - 400f, rotationMenu.Position.Y - 75f);
               rotationMenu.IsPulsating = false;
               rotationMenu.SelectedColor = Color.CornflowerBlue;
               rotationMenu.ShowBorder = false;
               rotationMenu.menuEntryBorderSize = new Vector2(0, 0);

               scaleMenu.AdditionalVerticalSpacing = 0;
               scaleMenu.Position = new Vector2(scaleMenu.Position.X - 400f, scaleMenu.Position.Y - 75f);
               scaleMenu.IsPulsating = false;
               scaleMenu.SelectedColor = Color.CornflowerBlue;
               scaleMenu.ShowBorder = false;
               scaleMenu.menuEntryBorderSize = new Vector2(0, 0);

               MenuEntries.Add(positionMenu);
               MenuEntries.Add(rotationMenu);
               MenuEntries.Add(scaleMenu);
          }

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          void SelectLightingMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               MenuEntries.Clear();

               currentMode = 2;

               MenuEntry ambientMenu = new MenuEntry("Ambient");
               MenuEntry diffuseMenu = new MenuEntry("Diffuse");
               MenuEntry emissiveMenu = new MenuEntry("Emissive");

               ambientMenu.AdditionalVerticalSpacing = 0;
               ambientMenu.Position = new Vector2(ambientMenu.Position.X - 400f, ambientMenu.Position.Y - 75f);
               ambientMenu.IsPulsating = false;
               ambientMenu.FontScale = 0.5f;
               ambientMenu.SelectedColor = Color.CornflowerBlue;
               ambientMenu.ShowBorder = false;
               ambientMenu.menuEntryBorderSize = new Vector2(0, 0);

               diffuseMenu.AdditionalVerticalSpacing = 0;
               diffuseMenu.Position = new Vector2(diffuseMenu.Position.X - 400f, diffuseMenu.Position.Y - 75f);
               diffuseMenu.IsPulsating = false;
               diffuseMenu.FontScale = 0.5f;
               diffuseMenu.SelectedColor = Color.CornflowerBlue;
               diffuseMenu.ShowBorder = false;
               diffuseMenu.menuEntryBorderSize = new Vector2(0, 0);

               emissiveMenu.AdditionalVerticalSpacing = 0;
               emissiveMenu.Position = new Vector2(emissiveMenu.Position.X - 400f, emissiveMenu.Position.Y - 75f);
               emissiveMenu.IsPulsating = false;
               emissiveMenu.SelectedColor = Color.CornflowerBlue;
               emissiveMenu.FontScale = 0.5f;
               emissiveMenu.ShowBorder = false;
               emissiveMenu.menuEntryBorderSize = new Vector2(0, 0);

               MenuEntries.Add(ambientMenu);
               MenuEntries.Add(diffuseMenu);
               MenuEntries.Add(emissiveMenu);
          }

          protected override void  OnCancel(PlayerIndex playerIndex)
          {
               if (currentModelIndex == 0)
               {
                    base.OnCancel(playerIndex);
               }               
               
               else
               {
                    MenuEntries.Clear();

                    MenuEntries.Add(selectModelMenuEntry);
                    MenuEntries.Add(selectTransMenuEntry);
                    MenuEntries.Add(selectLightingMenuEntry);
               }
          }

          #endregion

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               base.HandleInput(input, gameTime);

               PlayerIndex index;
               CameraManager.ActiveCamera.HandleInput(input, EngineCore.ControllingPlayer);

               switch (currentMode)
               {
                    // Model Mode.
                    case 0:
                         if (input.IsNewButtonPress(Buttons.DPadLeft, null, out index))
                         {
                              currentModelIndex--;

                              if (currentModelIndex < 0) currentModelIndex = 0;
                              if (currentModelIndex > modelList.Count - 1) currentModelIndex = modelList.Count - 1;
                         }

                         if (input.IsNewButtonPress(Buttons.DPadRight, null, out index))
                         {
                              currentModelIndex++;

                              if (currentModelIndex < 0) currentModelIndex = 0;
                              if (currentModelIndex > modelList.Count - 1) currentModelIndex = modelList.Count - 1;
                         }

                         break;

                    // Transformation Mode.
                    case 1:
                         if (input.IsButtonDown(Buttons.DPadLeft, null, out index))
                         {
                              modelList[currentModelIndex].Position =
                                   new Vector3(modelList[currentModelIndex].Position.X + 0.01f, modelList[currentModelIndex].Position.Y, modelList[currentModelIndex].Position.Z);
                         }

                         if (input.IsButtonDown(Buttons.DPadRight, null, out index))
                         {
                              modelList[currentModelIndex].Position =
                                   new Vector3(modelList[currentModelIndex].Position.X - 0.01f, modelList[currentModelIndex].Position.Y, modelList[currentModelIndex].Position.Z);
                         }

                         break;

                    // Lighting Mode.
                    case 2:
                         break;
               }
               
          }

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               MySpriteBatch.Begin();

               TextManager.Draw(ScreenManager.Font, "Model: " + modelNameList[currentModelIndex].ToString(), new Vector2(50f, 500), Color.CornflowerBlue, 0f, new Vector2(), 0.5f);

               TextManager.Draw(ScreenManager.Font, "Position: " + modelList[currentModelIndex].Position.ToString(), new Vector2(50f, 525), Color.White, 0f, new Vector2(), 0.5f);
               TextManager.Draw(ScreenManager.Font, "Rotation: " + modelList[currentModelIndex].Rotation.ToString(), new Vector2(50f, 550), Color.White, 0f, new Vector2(), 0.5f);
               TextManager.Draw(ScreenManager.Font, "Scale: " + modelList[currentModelIndex].Scale.ToString(), new Vector2(50f, 575), Color.White, 0f, new Vector2(), 0.5f);

               TextManager.Draw(ScreenManager.Font, "Ambient: " + modelList[currentModelIndex].Model.AmbientLightColor.ToString(), new Vector2(875, 500), Color.Gold, 0f, new Vector2(), 0.5f);
               TextManager.Draw(ScreenManager.Font, "Diffuse: " + modelList[currentModelIndex].Model.DiffuseColor.ToString(), new Vector2(875, 525), Color.Gold, 0f, new Vector2(), 0.5f);
               TextManager.Draw(ScreenManager.Font, "Emissive: " + modelList[currentModelIndex].Model.EmissiveColor.ToString(), new Vector2(875, 550), Color.Gold, 0f, new Vector2(), 0.5f);
               TextManager.Draw(ScreenManager.Font, "Specular: " + modelList[currentModelIndex].Model.SpecularColor.ToString(), new Vector2(875, 575), Color.Gold, 0f, new Vector2(), 0.5f);

               modelList[currentModelIndex].Draw(gameTime);

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               rotation += 0.25f;

               // FOR CASTLE
               modelList[currentModelIndex].Rotation = new Quaternion(modelList[currentModelIndex].Rotation.X, modelList[currentModelIndex].Rotation.Y, modelList[currentModelIndex].Rotation.Z, 0f); 
               modelList[currentModelIndex].Rotation = new Quaternion(modelList[currentModelIndex].Rotation.X, 0f + rotation, modelList[currentModelIndex].Rotation.Z, 0f);
               modelList[currentModelIndex].Rotation = new Quaternion(modelList[currentModelIndex].Rotation.X, modelList[currentModelIndex].Rotation.Y, modelList[currentModelIndex].Rotation.Z, 0f);

               modelList[currentModelIndex].World = Matrix.CreateScale(0.025f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(modelList[currentModelIndex].Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(modelList[currentModelIndex].Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(modelList[currentModelIndex].Rotation.Z))
                    * Matrix.CreateTranslation(modelList[currentModelIndex].Position);
          }

          int currentModelIndex = 0;

          #endregion
     }
}