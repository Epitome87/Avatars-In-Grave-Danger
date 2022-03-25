using System;
using Microsoft.Xna.Framework;
using PixelEngine.AchievementSystem;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace AvatarsInGraveDanger
{
    public class UnlockablesMenuScreen : MenuScreen
    {
        private MenuEntry seizureModeMenuEntry;
        private MenuEntry secretCharacterMenuEntry;
        private MenuEntry backMenuEntry;


        public UnlockablesMenuScreen(string menuTitle)
            : base(menuTitle)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);


            seizureModeMenuEntry = new MenuEntry("Enable Seizure Mode", "Warning:\nSeizure Mode may cause seizures.");
            secretCharacterMenuEntry = new MenuEntry("Secret Character", "Play as a secret character!");
            backMenuEntry = new MenuEntry("Back", "Return to the previous screen.");

            seizureModeMenuEntry.Selected += SeizureModeMenuEntrySelected;
            secretCharacterMenuEntry.Selected += SecretCharacterMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(seizureModeMenuEntry);
            MenuEntries.Add(secretCharacterMenuEntry);
            MenuEntries.Add(backMenuEntry);


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

            UpdateMenuText();
        }


        private void SeizureModeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (AchievementManager.EarnedAchievementValue < 100)
                return;

            AvatarZombieGame.SeizureModeEnabled = !AvatarZombieGame.SeizureModeEnabled;

            UpdateMenuText();
        }

        private void SecretCharacterMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (AchievementManager.EarnedAchievementValue < 600)
                return;

            if (AvatarZombieGame.CurrentArcadeLevel != null)
            {
                if (isNedCharacter)
                {
                    AvatarZombieGame.CurrentArcadeLevel.CurrentPlayer.Avatar.LoadUserAvatar(e.PlayerIndex);
                }

                else
                {
                    AvatarZombieGame.CurrentArcadeLevel.CurrentPlayer.Avatar.LoadAvatar(PixelEngine.Avatars.CustomAvatarType.Merchant);
                }
            }

            isNedCharacter = !isNedCharacter;

            UpdateMenuText();
        }

        private bool isNedCharacter = false;

        private void UpdateMenuText()
        {
            if (AchievementManager.EarnedAchievementValue >= 100)
            {
                if (AvatarZombieGame.SeizureModeEnabled)
                {
                    seizureModeMenuEntry.Text = "Disable Seizure Mode";
                }

                else
                {
                    seizureModeMenuEntry.Text = "Enable Seizure Mode";
                }

                seizureModeMenuEntry.Description = "Warning:\nSeizure Mode may cause seizures.";

                seizureModeMenuEntry.SelectedColor = Color.White;
                seizureModeMenuEntry.UnselectedColor = Color.White;
            }

            else
            {
                seizureModeMenuEntry.Text = "Seizure Mode";
                seizureModeMenuEntry.Description = "Earn 100 Award Points To Unlock";

                seizureModeMenuEntry.SelectedColor = Color.Gray;
                seizureModeMenuEntry.UnselectedColor = Color.Gray;
            }


            if (AchievementManager.EarnedAchievementValue >= 600)
            {
                if (isNedCharacter)
                {
                    secretCharacterMenuEntry.Text = "Your Avatar";
                    secretCharacterMenuEntry.Description = "Play as your own Avatar";
                }

                else
                {
                    secretCharacterMenuEntry.Text = "Merchant Ned";
                    secretCharacterMenuEntry.Description = "Play as Merchant Ned";
                }

                secretCharacterMenuEntry.SelectedColor = Color.White;
                secretCharacterMenuEntry.UnselectedColor = Color.White;
            }

            else
            {
                secretCharacterMenuEntry.Text = "Secret Character";
                secretCharacterMenuEntry.Description = "Earn 600 Award Points To Unlock";

                secretCharacterMenuEntry.SelectedColor = Color.Gray;
                secretCharacterMenuEntry.UnselectedColor = Color.Gray;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            UpdateMenuText();
        }
    }
}
