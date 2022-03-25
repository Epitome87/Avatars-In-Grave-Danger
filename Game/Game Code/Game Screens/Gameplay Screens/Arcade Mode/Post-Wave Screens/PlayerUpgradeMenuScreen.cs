#region File Description
//-----------------------------------------------------------------------------
// PlayerUpgradeMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
    /// <remarks>
    /// A Screen containing the logic for when a Wave is Completed.
    /// Displays post-wave statistics such as number of kills, accuracy, etc.
    /// </remarks>
    public class PlayerUpgradeMenuScreen : MenuScreen
    {
        #region Fields

        /// <summary>
        /// Helper variable to keep track of elapsed time;
        /// Used for determining when the Merchant's
        /// introduction dialogue should be stopped.
        /// </summary>
        float elapsedTime = 0f;

        /// <summary>
        /// The TextObject which will be used to handle
        /// the Merchant's introduction dialogue.
        /// 
        /// It will use a Reading Text Effect.
        /// </summary>
        TextObject merchantTextObject = new TextObject();

        /// <summary>
        /// MenuEntry for returning to Wave Complete screen.
        /// </summary>
        MenuEntry waveCompleteMenuEntry = new MenuEntry("Exit\nUpgrades", "Return to the\nprevious menu.");

        /// <summary>
        /// MenuEntry for purchasing Strafe Speed upgrade.
        /// </summary>
        MenuEntry strafeSpeedUpgradeMenuEntry = new MenuEntry("Strafe\nSpeed", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing Cash Bonus upgrade.
        /// </summary>
        MenuEntry moneyPerkUpgradeMenuEntry = new MenuEntry("Cash\nBonus", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing Slow Motion upgrade.
        /// </summary>
        MenuEntry slowMotionMenuUpgradeEntry = new MenuEntry("Slow\nMotion", "Cost: $500");

        /// <summary>
        /// The Wave Manager.
        /// </summary>
        private WaveManager waveManager;

        /// <summary>
        /// The Arcade Level we are completing Waves in.
        /// </summary>
        private ArcadeLevel theArcadeLevel;


        private int basePrice = 10;//500;

        private int strafeUpgradeCost = 0;
        private int cashPerkUpgradeCost = 0;
        private int slowMotionUpgradeCost = 0;

        Random random = new Random();

        bool showConfirmation = false;

        float elapsedConfirmationTime = 3f;

        string confirmationString = "";


        Rectangle introductionRectangle = new Rectangle((int)(EngineCore.ScreenCenter.X),
           (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 75), 600, 150);

        Rectangle upgradeRectangle = new Rectangle(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Right - (400 / 2),
             (int)EngineCore.ScreenCenter.Y - (int)((50f / 720f) * EngineCore.GraphicsDevice.Viewport.Height), 400, 310);

        Rectangle confirmationRectangle = new Rectangle((int)(EngineCore.ScreenCenter.X),
           (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - (125 / 2)), 600, 125);

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerUpgradeMenuScreen(ArcadeLevel arcadeLevel, WaveManager _waveManager)
            : base("Character Upgrades")
        {
            this.MenuTitleFontScale *= 0.75f;

            TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            theArcadeLevel = arcadeLevel;
            waveManager = _waveManager;

            this.MenuTitleColor = Color.CornflowerBlue;

            // Hook up all the Selected event handlers.
            strafeSpeedUpgradeMenuEntry.Selected += StrafeUpgradeMenuEntrySelected;
            moneyPerkUpgradeMenuEntry.Selected += MoneyPerkUpgradeMenuEntrySelected;
            slowMotionMenuUpgradeEntry.Selected += SlowMotionUpgradeMenuEntrySelected;

            waveCompleteMenuEntry.Selected += WaveCompleteMenuEntrySelected;

            // Add all the upgrade menu entries first.
            MenuEntries.Add(strafeSpeedUpgradeMenuEntry);
            MenuEntries.Add(moneyPerkUpgradeMenuEntry);
            MenuEntries.Add(slowMotionMenuUpgradeEntry);

            // Finally, the Wave Complete menu entry.
            MenuEntries.Add(waveCompleteMenuEntry);


            this.MenuTitleColor = Color.DarkOrange;



            this.numberOfColumns = 1;

            foreach (MenuEntry entry in MenuEntries)
            {
                entry.IsEntryTextVisible = true;
                entry.Position = new Vector2(entry.Position.X - 20f - EngineCore.GraphicsDevice.Viewport.Width * 0.32f, entry.Position.Y + EngineCore.GraphicsDevice.Viewport.Height * 0.104f - 15f);
                entry.AdditionalVerticalSpacing = 25f;
                entry.menuEntryBorderScale = new Vector2(1.0f, 1.0f);

                entry.IsPulsating = false;
                entry.SelectedColor = Color.DarkOrange;// entry.UnselectedColor;

                entry.ShowGradientBorder = false;// true;
                entry.ShowBorder = false;

                entry.menuEntryBorderSize = new Vector2(200, 200);
                entry.useCustomBorderSize = true;
                entry.FontScale = 0.75f;
                entry.ShowIcon = false;

                entry.UnselectedBorderColor = Color.Black * (155f / 255f);
                entry.SelectedBorderColor = Color.DarkOrange * (255f / 255f);

                entry.DescriptionFontScale *= 0.60f;
                entry.DescriptionColor = Color.White;

                entry.DescriptionPosition = new Vector2(upgradeRectangle.X, upgradeRectangle.Y - 50f);

                entry.ShowDescriptionBorder = false;

                entry.DescriptionFontType = FontType.HudFont;

                entry.FontType = FontType.TitleFont;
                entry.FontScale *= 0.34f;
            }
            waveCompleteMenuEntry.FontScale = 0.75f * 0.35f;
            waveCompleteMenuEntry.Position = new Vector2(waveCompleteMenuEntry.Position.X + 0, waveCompleteMenuEntry.Position.Y + 25f);
            waveCompleteMenuEntry.ShowGradientBorder = true;
            waveCompleteMenuEntry.SelectedColor = waveCompleteMenuEntry.UnselectedColor;

            // Alter the position for Wave Complete menu entry.
            //waveCompleteMenuEntry.Position = new Vector2((150f / 1280f) * EngineCore.GraphicsDevice.Viewport.Width, (175f / 720f) * EngineCore.GraphicsDevice.Viewport.Height);
            //waveCompleteMenuEntry.DescriptionPosition = new Vector2(waveCompleteMenuEntry.DescriptionPosition.X, EngineCore.ScreenCenter.Y);

            List<CameraEffect> effectList = new List<CameraEffect>();

            effectList.Add(new PointEffect(0.5f, new Vector3(-20f, 2f, 50f)));
            effectList.Add(new MoveEffect(1.0f, new Vector3(-20f, 2f, 32.5f)));


            AvatarZombieGame.AwardData.TotalSimultaneousUpgrades = 0;
        }

        #endregion


        #region Menu Entry Selected Events

        private void UpgradePurchased()
        {
            string[] acceptedStrings = 
               { 
                    "Thank you!", 
                    "I'm selling that at a loss here.", 
                    "Now I can refurnish the\nrock I hide behind.", 
                    "Hope that comes in handy.",
                    "Where are you even getting this money?",
                    "This goes straight into\nmy retirement fund.",
                    "Looks like my business\nis no longer in...\ngrave danger.",
                    "This money better not be counterfeit.",
                    "Absolutely no refunds.",
               };

            int randomNum = random.Next(9);

            confirmationString = acceptedStrings[randomNum];

            switch (randomNum)
            {
                case 0:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleLaugh, false);
                    break;
                case 1:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.Wave, false);
                    break;
                case 2:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.Clap, false);
                    break;
                default:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.Wave, false);
                    break;

            }
        }

        private void UpgradeNotPurchased()
        {
            string[] deniedStrings = 
               { 
                    "You don't have enough money.", 
                    "Do I look like a charity?", 
                    "You can't afford that.",
                    "Did a zombie eat your brain?\nYou need more money.",
                    "Math isn't your strong suit, is it?\nCost - Your Funds = Get Out!",
               };

            int randomNum = random.Next(5);

            confirmationString = deniedStrings[randomNum];

            switch (randomNum)
            {
                case 0:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleYawn, false);
                    break;
                case 1:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, false);
                    break;
                case 2:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleConfused, false);
                    break;
                default:
                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleConfused, false);
                    break;
            }
        }


        /// <summary>
        /// Event Handler for when the Strafe Speed Upgrade entry is Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrafeUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }

            strafeUpgradeCost = (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel + 1) * basePrice;

            // Buy + 0.1 Strafing Speed.
            if (this.theArcadeLevel.CurrentPlayer.Money >= strafeUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= strafeUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.StrafeSpeed += 0.1f;

                this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel++;

                AudioManager.PlayCue("MoneySpent");

                AvatarZombieGame.AwardData.TotalUpgrades++;
                AvatarZombieGame.AwardData.TotalSimultaneousUpgrades++;

                UpgradePurchased();
            }

            else
            {
                AudioManager.PlayCue("Mistype");

                UpgradeNotPurchased();
            }

            showConfirmation = true;

            elapsedConfirmationTime = 3.0f;
        }

        /// <summary>
        /// Event Handler for when the Strafe Speed Upgrade entry is Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoneyPerkUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.CashPerkLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }


            cashPerkUpgradeCost = (this.theArcadeLevel.CurrentPlayer.CashPerkLevel + 1) * basePrice;

            // Buy + 0.1 Strafing Speed.
            if (this.theArcadeLevel.CurrentPlayer.Money >= cashPerkUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= cashPerkUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.CashPerk += 0.1f;

                this.theArcadeLevel.CurrentPlayer.CashPerkLevel++;

                AudioManager.PlayCue("MoneySpent");

                AvatarZombieGame.AwardData.TotalUpgrades++;
                AvatarZombieGame.AwardData.TotalSimultaneousUpgrades++;

                UpgradePurchased();
            }

            else
            {
                AudioManager.PlayCue("Mistype");

                UpgradeNotPurchased();
            }

            showConfirmation = true;

            elapsedConfirmationTime = 3.0f;
        }


        /// <summary>
        /// Event Handler for when the Strafe Speed Upgrade entry is Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlowMotionUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.SlowMotionLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }


            slowMotionUpgradeCost = (this.theArcadeLevel.CurrentPlayer.SlowMotionLevel + 1) * 0;// basePrice;

            // Buy + 0.1 Strafing Speed.
            if (this.theArcadeLevel.CurrentPlayer.Money >= slowMotionUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= slowMotionUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.slowMotionDevice.UsageRate -= (2.5f);

                this.theArcadeLevel.CurrentPlayer.SlowMotionLevel++;

                AudioManager.PlayCue("MoneySpent");

                AvatarZombieGame.AwardData.TotalUpgrades++;
                AvatarZombieGame.AwardData.TotalSimultaneousUpgrades++;

                UpgradePurchased();
            }

            else
            {
                AudioManager.PlayCue("Mistype");

                UpgradeNotPurchased();
            }

            showConfirmation = true;

            elapsedConfirmationTime = 3.0f;
        }

        /// <summary>
        /// Event handler for when the Continue Game menu entry is selected.
        /// </summary>
        void WaveCompleteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        public override void HandleInput(InputState input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);
        }


        protected override void OnCancel(PlayerIndex playerIndex)
        {
            this.ExitScreen();
        }

        #endregion


        #region Update

        /// <summary>
        /// Overriden Update method.
        /// 
        /// It firstly calls the base Update method.
        /// Next, we update the Merchant CinematicEvent if we should.
        /// Next, we update the Merchant's TextObject.
        /// Finally, we signal when we should stop the Merchant's dialogue.
        /// </summary>
        /// <param name="gameTime">GameTime from the XNA Game instance.</param>
        /// <param name="otherScreenHasFocus">Whether or not another screen has focus.</param>
        /// <param name="coveredByOtherScreen">Whether or not this screen is covered by another.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                     bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            UpdateMenuDescriptions();

            if (showConfirmation)
            {
                elapsedConfirmationTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (elapsedConfirmationTime <= 0f)
                {
                    elapsedConfirmationTime = 3f;
                    showConfirmation = false;

                    WaveCompleteScreen.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleIdleShiftWeight, true);
                }
            }

            #region Check Upgrade Achievement Progresses

            if (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel >= Player.MaxUpgradeLevel
                 || this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel >= Player.MaxUpgradeLevel
                 || this.theArcadeLevel.CurrentPlayer.Gun.BulletSpeedLevel >= Player.MaxUpgradeLevel
                 || this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel >= Player.MaxUpgradeLevel
                 || this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel >= Player.MaxUpgradeLevel
                 || this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel >= Player.MaxUpgradeLevel)
            {
                AvatarZombieGame.AwardData.FullyUpgradedOne = true;
            }


            if (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel >= Player.MaxUpgradeLevel
                 && this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel >= Player.MaxUpgradeLevel
                 && this.theArcadeLevel.CurrentPlayer.Gun.BulletSpeedLevel >= Player.MaxUpgradeLevel
                 && this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel >= Player.MaxUpgradeLevel
                 && this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel >= Player.MaxUpgradeLevel
                 && this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel >= Player.MaxUpgradeLevel)
            {
                AvatarZombieGame.AwardData.FullyUpgradedAll = true;
            }

            #endregion

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            merchantTextObject.Update(gameTime);
        }

        #endregion


        #region Update Menu Descriptions

        /// <summary>
        /// A helper method which updates this Menu's menu entries' description.
        /// It mostly needs to update the cost of the Upgrades and the Player's
        /// various statistics.
        /// </summary>
        private void UpdateMenuDescriptions()
        {
            strafeUpgradeCost = (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel + 1) * basePrice;
            cashPerkUpgradeCost = (this.theArcadeLevel.CurrentPlayer.CashPerkLevel + 1) * basePrice;
            slowMotionUpgradeCost = (this.theArcadeLevel.CurrentPlayer.SlowMotionLevel + 1) * basePrice;

            #region Strafe Speed

            if (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel >= Player.MaxUpgradeLevel)
            {
                strafeSpeedUpgradeMenuEntry.Description = "Strafe Speed Maxed Out!\n\nCurrent Strafe Speed:\n" + (this.theArcadeLevel.CurrentPlayer.StrafeSpeed * 100).ToString() + "%";
            }

            else
            {
                strafeSpeedUpgradeMenuEntry.Description = "Price: $" + strafeUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Strafe Speed\nby 10%\n\nCurrent Strafe Speed:\n" + (this.theArcadeLevel.CurrentPlayer.StrafeSpeed * 100).ToString() + "%";
            }

            #endregion

            #region Cash Bonus

            if (this.theArcadeLevel.CurrentPlayer.CashPerkLevel >= Player.MaxUpgradeLevel)
            {
                moneyPerkUpgradeMenuEntry.Description = "Cash Bonus Maxed Out!\n\nCurrent Cash Bonus:\n" + (this.theArcadeLevel.CurrentPlayer.CashPerk * 100).ToString() + "%";
            }

            else
            {
                moneyPerkUpgradeMenuEntry.Description = "Price: $" + cashPerkUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Money Earned\nby 10%\n\nCurrent Cash Bonus:\n" + (this.theArcadeLevel.CurrentPlayer.CashPerk * 100).ToString() + "%";
            }

            #endregion

            #region Slow Motion

            if (this.theArcadeLevel.CurrentPlayer.SlowMotionLevel >= Player.MaxUpgradeLevel)
            {
                slowMotionMenuUpgradeEntry.Description = "Slow Motion Maxed Out!\n\nCurrent Slow Motion Duration:\n" + (100f / this.theArcadeLevel.CurrentPlayer.slowMotionDevice.UsageRate).ToString("#0.0") + " secs";
            }

            else
            {
                slowMotionMenuUpgradeEntry.Description = "Price: $" + slowMotionUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease how long time slows down.\n\nCurrent Slow Motion Duration:\n" + (100f / this.theArcadeLevel.CurrentPlayer.slowMotionDevice.UsageRate).ToString("#0.0") + " secs";
            }

            #endregion
        }

        #endregion


        #region Draw Gun Stats

        private void DrawBars(GameTime gameTime)
        {
            Color blackBarColorWithTransition = Color.Black * 0.5f * (TransitionAlpha / 255f);
            Color goldBarColorWithTransition = Color.Gold * (TransitionAlpha / 255f);

            Rectangle menuRectangle = new Rectangle(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Left + (400 / 2),
                 (int)EngineCore.ScreenCenter.Y - (int)((50f / 720f) * EngineCore.GraphicsDevice.Viewport.Height), 400, 310);


            int barSize = 40;
            int startingX = 310;
            int startingY = 175;

            Rectangle barRect = new Rectangle(startingX, startingY, barSize, barSize);

            // Rate of Fire bars.
            for (int i = 0; i < 5; i++)
            {
                Color barColor = blackBarColorWithTransition;

                GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), 2, Color.Black * (TransitionAlpha / 255f));

                if (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel > i)
                {
                    barColor = goldBarColorWithTransition;
                }

                MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), barColor);
            }


            startingY += 90;

            // Clip Size bars.
            for (int i = 0; i < 5; i++)
            {
                Color barColor = blackBarColorWithTransition;

                GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), 2, Color.Black * (TransitionAlpha / 255f));

                if (this.theArcadeLevel.CurrentPlayer.CashPerkLevel > i)
                {
                    barColor = goldBarColorWithTransition;
                }

                MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), barColor);
            }

            startingY += 90;

            // Clip Size bars.
            for (int i = 0; i < 5; i++)
            {
                Color barColor = blackBarColorWithTransition;

                GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), 2, Color.Black * (TransitionAlpha / 255f));

                if (this.theArcadeLevel.CurrentPlayer.SlowMotionLevel > i)
                {
                    barColor = goldBarColorWithTransition;
                }

                MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), barColor);
            }
        }


        #endregion


        #region Draw

        /// <summary>
        /// Overridden Draw Method.
        /// 
        /// This renders the Mechant's dialogue at the appropriate time.
        /// It also renders a Merchant Avatar.
        /// 
        /// Finally, a call to the base Draw method is called in order 
        /// to render all the menu entries and their descriptions.
        /// </summary>
        /// <param name="gameTime">GameTime from the XNA Game instance.</param>
        public override void Draw(GameTime gameTime)
        {
            WaveCompleteScreen.DrawMerchant(gameTime);

            MySpriteBatch.Begin();


            DrawBars(gameTime);


            Color blackWithTransition = Color.Black * (175f / 255f) * (TransitionAlpha / 255f);

            if (showConfirmation)
            {
                GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, confirmationRectangle,
                     2, Color.White * (TransitionAlpha / 255f));

                MySpriteBatch.DrawCentered(blankTexture.Texture2D, confirmationRectangle, blackWithTransition);

                TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.TitleFont].SpriteFont, confirmationString,
                     new Vector2(confirmationRectangle.X, confirmationRectangle.Y), Color.Gold, 0.25f);
            }


            GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, upgradeRectangle, 2, Color.White * (TransitionAlpha / 255f));

            MySpriteBatch.DrawCentered(blankTexture.Texture2D, upgradeRectangle, blackWithTransition);

            if (this.SelectedEntryIndex != MenuEntries.Count - 1)
            {
                MySpriteBatch.Draw360Button(Buttons.A,
                     new Vector2(EngineCore.GraphicsDevice.Viewport.Width * (940f / 1280f) - 90f,
                          EngineCore.ScreenCenter.Y + 90f),
                          Color.White * (TransitionAlpha / 255f), 0.0f, MySpriteBatch.Measure360Button(Microsoft.Xna.Framework.Input.Buttons.A) / 2f, 0.4f, SpriteEffects.None, 0f);

                string purchaseOrBrowse = "Purchase";

                TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, purchaseOrBrowse,
                     new Vector2(EngineCore.GraphicsDevice.Viewport.Width * (940f / 1280f) + 25f,
                          EngineCore.ScreenCenter.Y + 90f), Color.Gold * (TransitionAlpha / 255f), 0.75f);

                TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Your Money: $" + this.theArcadeLevel.CurrentPlayer.Money.ToString("N0"),
                     new Vector2((940f / 1280f) * EngineCore.GraphicsDevice.Viewport.Width,
                          EngineCore.ScreenCenter.Y + 55f), Color.White * (TransitionAlpha / 255f), 0.75f);

            }

            MySpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}