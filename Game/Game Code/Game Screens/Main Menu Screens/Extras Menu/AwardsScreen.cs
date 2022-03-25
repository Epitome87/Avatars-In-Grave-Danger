#region File Description
//-----------------------------------------------------------------------------
// AwardsScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.AchievementSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// The Awards screen, found in the Options Menu, is a Menu Screen.
     /// It displays a list of possible Awards for the player to obtain.
     /// Awards which have been unlocked will appear in one color, while 
     /// locked awards will appear gray.
     /// </remarks>
     public class AwardsScreen : PagedMenuScreen
     {
          #region Fields

          GameResourceTexture2D lockTexture;
          GameResourceTexture2D pic;
          GameResourceTexture2D blank;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public AwardsScreen()
               : base("Awards 7 -  " + AchievementManager.EarnedAchievementValue.ToString()  + " / " + AchievementManager.TotalAchievementValue.ToString(), (uint)AchievementManager.Achievements.Count, 8)
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0);
               
               this.MenuTitleColor = Color.CornflowerBlue;
               this.SpacingBetweenColumns = 225f + 400f;
               this.numberOfColumns = 2;

               for (int i = 0; i < AchievementManager.Count; i++)
               {
                    if (i >= MenuEntries.Count)
                         break;

                    MenuEntries[i].Text = AchievementManager.Achievements[i].Title;
                    MenuEntries[i].Description = AchievementManager.Achievements[i].PointValue.ToString() + " Points\n" + AchievementManager.Achievements[i].Description;


                    if (i == 4 || i == 5 || i == 6 || i == 7)
                    {
                         if (!AchievementManager.Achievements[i].IsUnlocked)
                         MenuEntries[i].Description += "\nProgress: " + AvatarZombieGame.AwardData.TotalKills + " / " + AchievementManager.Achievements[i].TargetValue.ToString();
                    }

                    if (i == AchievementManager.Count - 1)
                    {
                         if (!AchievementManager.Achievements[i].IsUnlocked)
                         MenuEntries[i].Description += "\nProgress: " + AvatarZombieGame.AwardData.TotalCloseCalls + " / " + AchievementManager.Achievements[i].TargetValue.ToString();
                    }


                    MenuEntries[i].IsCenter = true;

                    MenuEntries[i].AdditionalVerticalSpacing = 25f; ;
                    MenuEntries[i].Position = new Vector2(MenuEntries[i].Position.X - 310, MenuEntries[i].Position.Y + EngineCore.GraphicsDevice.Viewport.Height * 0.10f);
                    MenuEntries[i].menuEntryBorderSize = new Vector2(300, 200);//695, 100);
                    MenuEntries[i].useCustomBorderSize = true;
                    MenuEntries[i].IsPulsating = false;
                    MenuEntries[i].ShowIcon = false;

                    MenuEntries[i].ShowDescriptionBorder = false;
                    MenuEntries[i].DescriptionPosition = new Vector2(MenuEntries[i].DescriptionPosition.X, MenuEntries[i].DescriptionPosition.Y - 50);


                    MenuEntries[i].UnselectedBorderColor = Color.Black * (155f / 255f);
                    MenuEntries[i].SelectedBorderColor = Color.DarkOrange * (255f / 255f);

                    MenuEntries[i].DescriptionColor = Color.DarkOrange;

                    MenuEntries[i].FontType = FontType.TitleFont;
                    MenuEntries[i].FontScale *= 0.25f;

                    MenuEntries[i].DescriptionFontType = FontType.TitleFont;
                    MenuEntries[i].DescriptionFontScale = 0.34f;
               }
          }

          public override void LoadContent()
          {
               base.LoadContent();

               lockTexture = ResourceManager.LoadTexture(@"Achievements\Lock_Clear");

               blank = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");
          }

          #endregion


          #region Handle Input

          public override void HandleInput(InputState input, GameTime gameTime)
          {
               base.HandleInput(input, gameTime);
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               MySpriteBatch.Begin(BlendState.AlphaBlend, SpriteSortMode.BackToFront);

               Color color = Color.White;

               for (int i = 0; i < AchievementManager.Count; i++)
               {
                    if (SelectedMenuEntry == i)
                    {
                         if (!AchievementManager.Achievements[i].IsUnlocked)
                         {
                              MySpriteBatch.Draw(lockTexture.Texture2D, 
                                   new Rectangle((int)EngineCore.ScreenCenter.X - 175, (int)this.MenuEntries[i].DescriptionPosition.Y - (75 / 1), 75, 75), Color.White);
                         }
                    }
               }

               for (int i = 0; i < this.MenuEntries.Count; i++)
               {
                    this.MenuEntries[i].Text = TextManager.WrapText(AchievementManager.Achievements[i].Title, FontType.TitleFont, 300 / this.MenuEntries[i].FontScale);

                    if (!AchievementManager.Achievements[i].IsUnlocked)
                    {
                         this.MenuEntries[i].UnselectedColor = Color.Gray;
                         this.MenuEntries[i].SelectedColor = Color.Gray;
                    }

                    else
                    {
                         this.MenuEntries[i].UnselectedColor = Color.White;
                         this.MenuEntries[i].SelectedColor = Color.White;
                    }
               }

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}
