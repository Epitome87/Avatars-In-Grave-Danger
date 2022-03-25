#region File Description
//-----------------------------------------------------------------------------
// UpgradeMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AvatarsInGraveDanger
{
    /// <remarks>
    /// A Screen containing the logic for when a Wave is Completed.
    /// Displays post-wave statistics such as number of kills, accuracy, etc.
    /// </remarks>
    public class PistolUpgradeMenuScreen : MenuScreen
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
        /// MenuEntry for purchasing Damage upgrade.
        /// </summary>
        MenuEntry damageUpgradeMenuEntry = new MenuEntry("Damage\n", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing Rate of Fire upgrade.
        /// </summary>
        MenuEntry rateOfFireUpgradeMenuEntry = new MenuEntry("Rate\nOf Fire", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing Clip Size upgrade.
        /// </summary>
        MenuEntry clipSizeUpgradeMenuEntry = new MenuEntry("Clip\nSize", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing Reload Speed upgrade.
        /// </summary>
        MenuEntry reloadSpeedUpgradeMenuEntry = new MenuEntry("Reload\nSpeed", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing Perfect Reload Area upgrade.
        /// </summary>
        MenuEntry reloadAreaUpgradeMenuEntry = new MenuEntry("Reload\nClutch Area", "Cost: $500");

        /// <summary>
        /// The Wave Manager.
        /// </summary>
        private WaveManager waveManager;

        /// <summary>
        /// The Arcade Level we are completing Waves in.
        /// </summary>
        private ArcadeLevel theArcadeLevel;


        private int basePrice = 10;//500;


        #endregion

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


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public PistolUpgradeMenuScreen(ArcadeLevel arcadeLevel, WaveManager _waveManager)
            : base("Pistol Upgrades")
        {
            this.MenuTitleFontScale *= 0.75f;

            TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            theArcadeLevel = arcadeLevel;
            waveManager = _waveManager;

            this.MenuTitleColor = Color.CornflowerBlue;

            // Hook up all the Selected event handlers.
            damageUpgradeMenuEntry.Selected += DamageUpgradeMenuEntrySelected;
            rateOfFireUpgradeMenuEntry.Selected += RateOfFireUpgradeMenuEntrySelected;
            clipSizeUpgradeMenuEntry.Selected += ClipSizeUpgradeMenuEntrySelected;
            reloadSpeedUpgradeMenuEntry.Selected += ReloadSpeedUpgradeMenuEntrySelected;
            reloadAreaUpgradeMenuEntry.Selected += ReloadAreaUpgradeMenuEntrySelected;

            waveCompleteMenuEntry.Selected += WaveCompleteMenuEntrySelected;

            // Add all the upgrade menu entries first.
            MenuEntries.Add(damageUpgradeMenuEntry);
            //MenuEntries.Add(rateOfFireUpgradeMenuEntry);
            MenuEntries.Add(clipSizeUpgradeMenuEntry);
            MenuEntries.Add(reloadSpeedUpgradeMenuEntry);
            MenuEntries.Add(reloadAreaUpgradeMenuEntry);

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

            this.theArcadeLevel.CurrentPlayer.Gun = this.theArcadeLevel.CurrentPlayer.PlayersGuns[0];

            damageUpgradeFactor = 0.2f;                         // +20% of initial damage.
            clipSizeUpgradeFactor = 1;                        // +20% of initial clip size.
            rateOfFireUpgradeFactor = 2.0f / 5.0f;            // +20% of intial value.
            reloadSpeedUpgradeFactor = 250.0f;                // 0.25 seconds faster each Upgrade.
            reloadAreaUpgradeFactor = 5.0f;                   // 5% larger each Upgrade.
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

        private int damageUpgradeCost = 0;
        private int clipSizeUpgradeCost = 0;
        private int rateOfFireUpgradeCost = 0;
        private int bulletSpeedUpgradeCost = 0;
        private int reloadSpeedUpgradeCost = 0;
        private int reloadAreaUpgradeCost = 0;


        private float damageUpgradeFactor = 0;
        private float rateOfFireUpgradeFactor = 0;
        private int clipSizeUpgradeFactor = 0;
        private float reloadSpeedUpgradeFactor = 0;
        private float reloadAreaUpgradeFactor = 0;

        private void DamageUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.Gun.DamageLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }

            damageUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.DamageLevel + 1) * basePrice;

            // Pretend to buy +0.1 Bullet Speed for $500
            if (this.theArcadeLevel.CurrentPlayer.Money >= damageUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= damageUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.Gun.BulletDamage += damageUpgradeFactor;

                this.theArcadeLevel.CurrentPlayer.Gun.DamageLevel++;

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

        private void RateOfFireUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }

            rateOfFireUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel + 1) * basePrice;

            if (this.theArcadeLevel.CurrentPlayer.Money >= rateOfFireUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= rateOfFireUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.Gun.RateOfFire += rateOfFireUpgradeFactor;

                this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel++;

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


        private void ClipSizeUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }

            clipSizeUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel + 1) * basePrice;

            // Pretend to buy +0.1 Bullet Speed for $500
            if (this.theArcadeLevel.CurrentPlayer.Money >= clipSizeUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= clipSizeUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.Gun.ClipSize += clipSizeUpgradeFactor;

                this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel++;

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


        void ReloadSpeedUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }

            reloadSpeedUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel + 1) * basePrice;

            // Pretend to buy +0.1 Bullet Speed for $500
            if (this.theArcadeLevel.CurrentPlayer.Money >= reloadSpeedUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= reloadSpeedUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.Gun.ReloadTime -= reloadSpeedUpgradeFactor;

                this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel++;

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

        void ReloadAreaUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel >= Player.MaxUpgradeLevel)
            {
                return;
            }

            reloadAreaUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel + 1) * basePrice;

            if (this.theArcadeLevel.CurrentPlayer.Money >= reloadAreaUpgradeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= reloadAreaUpgradeCost;

                this.theArcadeLevel.CurrentPlayer.Gun.ActiveReloadArea += reloadAreaUpgradeFactor;

                this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel++;

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
            damageUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.DamageLevel + 1) * basePrice;
            rateOfFireUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel + 1) * basePrice;
            clipSizeUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel + 1) * basePrice;
            bulletSpeedUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.BulletSpeedLevel + 1) * basePrice;
            reloadSpeedUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel + 1) * basePrice;
            reloadAreaUpgradeCost = (this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel + 1) * basePrice;


            if (this.theArcadeLevel.CurrentPlayer.Gun.DamageLevel >= Player.MaxUpgradeLevel)
            {
                damageUpgradeMenuEntry.Description = "Damage Maxed Out!\n\nCurrent Damage:\n" + (this.theArcadeLevel.CurrentPlayer.Gun.BulletDamage * 100f).ToString("N0") + "%";
            }

            else
            {
                damageUpgradeMenuEntry.Description = "Price: $" + damageUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Damage\nby 20%\n\nCurrent Damage:\n" + (this.theArcadeLevel.CurrentPlayer.Gun.BulletDamage * 100f).ToString("N0") + "%";
            }



            if (this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel >= Player.MaxUpgradeLevel)
            {
                rateOfFireUpgradeMenuEntry.Description = "Rate of Fire Maxed Out!\n\nCurrent Rate of Fire:\n" + this.theArcadeLevel.CurrentPlayer.Gun.RateOfFire.ToString("#0.00") + " shots / sec";
            }

            else
            {
                rateOfFireUpgradeMenuEntry.Description = "Price: $" + rateOfFireUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Rate of Fire\nby 20%\n\nCurrent Rate of Fire:\n" + this.theArcadeLevel.CurrentPlayer.Gun.RateOfFire.ToString("#0.00") + " shots / sec";
            }



            if (this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel >= Player.MaxUpgradeLevel)
            {
                clipSizeUpgradeMenuEntry.Description = "Clip Size Maxed out!\n\nCurrent Clip Size:\n" + this.theArcadeLevel.CurrentPlayer.Gun.ClipSize.ToString();
            }

            else
            {
                clipSizeUpgradeMenuEntry.Description = "Price: $" + clipSizeUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Clip Size\nby " + clipSizeUpgradeFactor.ToString() +
                     "\n\nCurrent Clip Size:\n" + this.theArcadeLevel.CurrentPlayer.Gun.ClipSize.ToString();
            }

            if (this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel >= Player.MaxUpgradeLevel)
            {
                reloadSpeedUpgradeMenuEntry.Description = "Reload Speed Max Out!\n\nCurrent Reload Speed:\n" + (this.theArcadeLevel.CurrentPlayer.Gun.ReloadTime / 1000f).ToString("#0.00") + " seconds";
            }

            else
            {
                reloadSpeedUpgradeMenuEntry.Description = "Price: $" + reloadSpeedUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Reload Speed\nby 0.25 seconds\n\nCurrent Reload Speed:\n" + (this.theArcadeLevel.CurrentPlayer.Gun.ReloadTime / 1000f).ToString("#0.00") + " seconds";
            }

            if (this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel >= Player.MaxUpgradeLevel)
            {
                reloadAreaUpgradeMenuEntry.Description = "Reload Clutch Area Maxed Out!\n\nCurrent Reload Clutch Area:\n" + (this.theArcadeLevel.CurrentPlayer.Gun.ActiveReloadArea).ToString();
            }

            else
            {
                reloadAreaUpgradeMenuEntry.Description = "Price: $" + reloadAreaUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Reload Clutch Area\nby 5%\n\nCurrent Reload Clutch Area:\n" + (this.theArcadeLevel.CurrentPlayer.Gun.ActiveReloadArea).ToString();
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


        #region Draw Gun Stats


        private void DrawBars(GameTime gameTime)
        {
            Color blackBarColorWithTransition = Color.Black * 0.5f * (TransitionAlpha / 255f);
            Color goldBarColorWithTransition = Color.Gold * (TransitionAlpha / 255f);

            Rectangle menuRectangle = new Rectangle(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Left + (400 / 2),
                 (int)EngineCore.ScreenCenter.Y - (int)((50f / 720f) * EngineCore.GraphicsDevice.Viewport.Height), 400, 310);

            //GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, menuRectangle, 2, Color.White * (TransitionAlpha / 255f));
            //MySpriteBatch.DrawCentered(blankTexture.Texture2D, menuRectangle, Color.Black * 0.5f * (TransitionAlpha / 255f));

            int barSize = 40;
            int startingX = 310;
            int startingY = 175;

            Rectangle barRect = new Rectangle(startingX, startingY, barSize, barSize);

            // Rate of Fire bars.
            for (int i = 0; i < 5; i++)
            {
                Color barColor = blackBarColorWithTransition;

                GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), 2, Color.Black * (TransitionAlpha / 255f));

                if (this.theArcadeLevel.CurrentPlayer.Gun.DamageLevel > i)
                //(this.theArcadeLevel.CurrentPlayer.Gun.RateOfFireLevel > i)
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

                if (this.theArcadeLevel.CurrentPlayer.Gun.ClipSizeLevel > i)
                {
                    barColor = goldBarColorWithTransition;
                }

                MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), barColor);
            }


            startingY += 90;

            // Reload Speed bars.
            for (int i = 0; i < 5; i++)
            {
                Color barColor = blackBarColorWithTransition;

                GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), 2, Color.Black * (TransitionAlpha / 255f));

                if (this.theArcadeLevel.CurrentPlayer.Gun.ReloadSpeedLevel > i)
                {
                    barColor = goldBarColorWithTransition;
                }

                MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), barColor);
            }

            startingY += 90;

            // Reload area.
            for (int i = 0; i < 5; i++)
            {
                Color barColor = blackBarColorWithTransition;

                GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), 2, Color.Black * (TransitionAlpha / 255f));

                if (this.theArcadeLevel.CurrentPlayer.Gun.ReloadAreaLevel > i)
                {
                    barColor = goldBarColorWithTransition;
                }

                MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX + (barSize * i), startingY, barSize, barSize), barColor);
            }
        }


        #endregion
    }
}