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
    public class UpgradeMenuScreen : MenuScreen
    {
        #region Fields

        /// <summary>
        /// A CinematicEvent which points and moves the Camera toward
        /// the Merchant. Plays as soon as the screen is active.
        /// </summary>
        CinematicEvent moveToMerchantEvent;

        /// <summary>
        /// Whether or not we are going to be updating
        /// and playing the Merchant CinematicEvent.
        /// </summary>
        bool isMovingToMerchant = false;

        /// <summary>
        /// Whether or not the Merchant is currently making
        /// his randomized introduction dialogue.
        /// </summary>
        bool isShowingIntroduction = true;

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
        /// An array of strings that the Merchant 
        /// may use as his introduction dialogue.
        /// </summary>
        string[] merchantTalkingPoints = new string[] 
          { 
               "Lots of zombies out there...\nI'd help but I sprained my ankle in 'Nam.\nLast summer I vacationed there;\nlovely place.", 
               "Why aren't I helping?\n...Why aren't you buying?", 
               "I'm not hiding from the zombies.\nI just don't want them\ndiscovering my wares.",
               "Buy. Don't Buy.\nSee what I care.\n                    \n...Please buy.",
               "It's never a bad idea to\nhave a Zomblaster handy...\nJust in case.",
               "Getting annoyed of that heartbeat?\nBuy some health.",
               "Wouldn't it be crazy if I\nwas a zombie all along?\n                    \nEr, I mean, have a look around...",
               "Brainnnnnsssss!!!\n                    \nThis Sudoku is tough.\nWish I had more brains.",
               "Looks like you're in...\n     GRAVE DANGER!     \n     But in all seriousness,     \nyou could die at any moment.",
               "Sure is nice of the zombies to wait\npolitely while you shop around.",
               "You should spend all your money now.\nNever know if you might be\ncomin' back...",
               "The objects the Slingers throw?\nThey bought those from me.\nHm, perhaps that wasn't in my\nbest interest.",
               "I used to fight zombies\nlike you, but then I took\nan arrow in the knee.",
          };

        /// <summary>
        /// MenuEntry for returning to Wave Complete screen.
        /// </summary>
        MenuEntry waveCompleteMenuEntry = new MenuEntry("Exit\nShop", "Return to the\nWave Results Menu");

        /// <summary>
        /// MenuEntry for purchasing Health upgrade.
        /// </summary>
        MenuEntry healthUpgradeMenuEntry = new MenuEntry("Health", "Cost: $750");

        /// <summary>
        /// MenuEntry for purchasing a Grenade item.
        /// </summary>
        MenuEntry grenadeUpgradeMenuEntry = new MenuEntry("Grenade", "Cost: $1250");//("Zomblaster", "Cost: $1250");

        MenuEntry playerUpgradesMenuEntry = new MenuEntry("Character\nUpgrades", "Browse Upgrades For\nYour Character.");
        /// <summary>
        /// MenuEntry for purchasing Strafe Speed upgrade.
        /// </summary>
        MenuEntry strafeUpgradeMenuEntry = new MenuEntry("Strafe\nSpeed", "Cost: $500");

        /// <summary>
        /// MenuEntry for purchasing a Spike Barrier.
        /// </summary>
        MenuEntry barrierMenuEntry = new MenuEntry("Barrier", "Cost: $1000");

        /// <summary>
        /// The Wave Manager.
        /// </summary>
        private WaveManager waveManager;

        /// <summary>
        /// The Arcade Level we are completing Waves in.
        /// </summary>
        private ArcadeLevel theArcadeLevel;

        /// <summary>
        /// The cost of a Health.
        /// </summary>
        private int healthCost = 750;

        /// <summary>
        /// The cost of a Grenade.
        /// </summary>
        private int grenadeCost = 1250;

        /// <summary>
        /// An array of strings the merchant may use after
        /// the Player has successfully purchased an item.
        /// </summary>
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


        /// <summary>
        /// An array of strings the merchant may use after
        /// the Player has failed to purchase an item from him.
        /// </summary>
        string[] deniedStrings = 
          { 
                    "You don't have enough money.", 
                    "Do I look like a charity?", 
                    "You can't afford that.",
                    "Did a zombie eat your brain?\nYou need more money.",
                    "Math isn't your strong suit, is it?\nCost - Your Funds = Get Out!",
                    //"Do you know what happened to the\nlast guy who tried to rip me off?\nHe became a zombie.\nI miss him.",
          };

        private Random random = new Random();

        private bool showConfirmation = false;

        private float elapsedConfirmationTime = 3f;

        private string confirmationString = "";


        /// <summary>
        /// A rectangle to hold the Merchant's introductory dialogue.
        /// </summary>
        Rectangle introductionRectangle = new Rectangle((int)(EngineCore.ScreenCenter.X),
           (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 75), 600, 150);

        /// <summary>
        /// A rectangle to hold the Merchant's purchase-confirmation (whether failed or succeeded) dialogue.
        /// </summary>
        Rectangle confirmationRectangle = new Rectangle((int)(EngineCore.ScreenCenter.X),
           (int)(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Bottom - (125 / 2)), 600, 125);

        /// <summary>
        /// A rectangle to hold the information about the currently selected Upgrade.
        /// </summary>
        Rectangle upgradeRectangle = new Rectangle(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Right - (400 / 2),
             (int)EngineCore.ScreenCenter.Y - (int)((50f / 720f) * EngineCore.GraphicsDevice.Viewport.Height), 400, 310);

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpgradeMenuScreen(ArcadeLevel arcadeLevel, WaveManager _waveManager)
            : base("Ned's Totally On-The-Level Shop")
        {
            this.MenuTitleFontScale *= 0.75f;

            TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            theArcadeLevel = arcadeLevel;
            waveManager = _waveManager;

            this.MenuTitleColor = Color.CornflowerBlue;

            // Hook up all the Selected event handlers.
            healthUpgradeMenuEntry.Selected += HealthUpgradeMenuEntrySelected;
            grenadeUpgradeMenuEntry.Selected += GrenadeUpgradeMenuEntrySelected;
            //strafeUpgradeMenuEntry.Selected += StrafeUpgradeMenuEntrySelected;
            barrierMenuEntry.Selected += BarrierMenuEntrySelected;

            waveCompleteMenuEntry.Selected += WaveCompleteMenuEntrySelected;


            MenuEntry pistolUpgradeMenuEntry = new MenuEntry("Pistol\nUpgrades", "Browse upgrades for\nyour trusty Pistol.");
            //"Purifier\nUpgrades", "Browse upgrades\nfor your Zombie Purifier.\n\nYour Zombie Purifier is a\ndevice which, once loaded with\nan antidote, delivers a cure\nto any zombie it strikes.");
            pistolUpgradeMenuEntry.Selected += PistolUpgradeMenuEntrySelected;

            MenuEntry uziUpgradeMenuEntry = new MenuEntry("Uzi\nUpgrades", "Browse upgrades for\nyour zombie-stopping Uzi.");
            //"Cleanser\nUpgrades", 
            //"Browse upgrades\nfor your Undead Cleanser.\n\nYour Undead Cleanser can\nvaccinate zombies at a much\ngreater speed than the\nPurifier. However, its\nantidotes are less potent.");
            uziUpgradeMenuEntry.Selected += UziUpgradeMenuEntrySelected;

            playerUpgradesMenuEntry.Selected += PlayerUpgradeMenuEntrySelected;

            // Add all the upgrade menu entries first.
            MenuEntries.Add(healthUpgradeMenuEntry);


            MenuEntries.Add(playerUpgradesMenuEntry);

            MenuEntries.Add(pistolUpgradeMenuEntry);
            //MenuEntries.Add(strafeUpgradeMenuEntry);

            MenuEntries.Add(grenadeUpgradeMenuEntry);
            MenuEntries.Add(uziUpgradeMenuEntry);


            //MenuEntries.Add(barrierMenuEntry);


            // Finally, the Wave Complete menu entry.
            MenuEntries.Add(waveCompleteMenuEntry);

            this.numberOfColumns = 2;

            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Position = new Vector2(entry.Position.X - EngineCore.GraphicsDevice.Viewport.Width * 0.32f, entry.Position.Y + EngineCore.GraphicsDevice.Viewport.Height * 0.104f);
                entry.AdditionalVerticalSpacing = 25f + 25f;
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

                entry.DescriptionFontScale *= 0.60f;
                entry.DescriptionColor = Color.White;

                entry.DescriptionPosition = new Vector2(upgradeRectangle.X, upgradeRectangle.Y - 50f);

                entry.ShowDescriptionBorder = false;

                entry.DescriptionFontType = FontType.HudFont;

                entry.FontType = FontType.TitleFont;
                entry.FontScale *= 0.35f;
            }

            this.SpacingBetweenColumns = 225f;
            this.MenuTitleColor = Color.DarkOrange;


            // Alter the position for Wave Complete menu entry.
            //waveCompleteMenuEntry.Position = new Vector2((150f / 1280f) * EngineCore.GraphicsDevice.Viewport.Width, (175f / 720f) * EngineCore.GraphicsDevice.Viewport.Height);
            //waveCompleteMenuEntry.DescriptionPosition = new Vector2(waveCompleteMenuEntry.DescriptionPosition.X, EngineCore.ScreenCenter.Y);

            List<CameraEffect> effectList = new List<CameraEffect>();

            effectList.Add(new PointEffect(0.5f, new Vector3(-20f, 2f, 50f)));
            effectList.Add(new MoveEffect(1.0f, new Vector3(-20f, 2f, 32.5f)));

            moveToMerchantEvent = new CinematicEvent(effectList, false);

            isMovingToMerchant = true;


            merchantTextObject = new TextObject(merchantTalkingPoints[random.Next(merchantTalkingPoints.Length)],
                 new Vector2(introductionRectangle.X, introductionRectangle.Y), FontType.TitleFont, Color.White, 0f,
                 new Vector2(introductionRectangle.X, introductionRectangle.Y), 0.25f, true, new ReadingEffect(3.0f, merchantTextObject.Text));

            merchantTextObject.IsAutoCenter = true;


            AvatarZombieGame.AwardData.TotalSimultaneousUpgrades = 0;
        }

        #endregion


        #region Menu Entry Selected Events

        private void UpgradePurchased()
        {
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

        private void PlayerUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new PlayerUpgradeMenuScreen(this.theArcadeLevel, this.waveManager), e.PlayerIndex);
        }

        private void PistolUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new PistolUpgradeMenuScreen(this.theArcadeLevel, this.waveManager), e.PlayerIndex);
        }

        private void UziUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new UziUpgradeMenuScreen(this.theArcadeLevel, this.waveManager, 1), e.PlayerIndex);
        }

        /// <summary>
        /// Event Handler for when the Grenade Upgrade entry is Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarrierMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Buy Barrier
            if (this.theArcadeLevel.CurrentPlayer.Money >= 0)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= 0;

                this.theArcadeLevel.CurrentPlayer.playersBarriers.Add(new Barrier(EngineCore.Game, new Vector3(0f, 0f, 15f)));

                AudioManager.PlayCue("MoneySpent");

                AvatarZombieGame.AwardData.TotalUpgrades++;
                AvatarZombieGame.AwardData.TotalSimultaneousUpgrades++;
            }

            else
            {
                AudioManager.PlayCue("Mistype");
            }
        }


        /// <summary>
        /// Event Handler for when the Health Upgrade entry is Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HealthUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Buy Health for $500
            if (this.theArcadeLevel.CurrentPlayer.Money >= healthCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= healthCost;
                this.theArcadeLevel.CurrentPlayer.Health++;


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
        /// Event Handler for when the Grenade Upgrade entry is Selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrenadeUpgradeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Buy Grenade for $1,250
            if (this.theArcadeLevel.CurrentPlayer.Money >= grenadeCost)
            {
                this.theArcadeLevel.CurrentPlayer.Money -= grenadeCost;

                this.theArcadeLevel.CurrentPlayer.NumberOfGrenades++;

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

        private int strafeUpgradeCost = 0;



        /// <summary>
        /// Event handler for when the Continue Game menu entry is selected.
        /// </summary>
        void WaveCompleteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (!isShowingIntroduction)
            {
                base.HandleInput(input, gameTime);
            }
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            isMovingToMerchant = false;

            // If we are in First Person View and not on the Wave Complete Screen...
            if (ArcadeLevel.IsFirstPerson)
            {
                Vector3 headScale = new Vector3();
                Quaternion headRotation = new Quaternion();
                Vector3 headPos = new Vector3();
                this.theArcadeLevel.CurrentPlayer.Avatar.BonesInWorldSpace[(int)AvatarBone.Head].Decompose(out headScale, out headRotation, out headPos);

                CameraManager.ActiveCamera.LookAt = new Vector3(headPos.X, 0.0f, 20f);
                CameraManager.ActiveCamera.Position = new Vector3(headPos.X + 0.15f, headPos.Y + 0.15f, headPos.Z - 0.10f);
            }

            // Otherwise, if we are in Third Person View //and not on the Wave Complete Screen...
            if (!ArcadeLevel.IsFirstPerson)
            {
                // Set 3rd-person camera that follows the player.
                CameraManager.ActiveCamera.LookAt = new Vector3(this.theArcadeLevel.CurrentPlayer.Position.X, 0.0f, 20f);
                CameraManager.ActiveCamera.Position = new Vector3(this.theArcadeLevel.CurrentPlayer.Position.X, 3.5f, this.theArcadeLevel.CurrentPlayer.Position.Z - 3f);
            }   

            // And then remove this Wave Complete Screen.
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

                    //ArcadeLevel.merchantAvatar.PlayAnimation(AvatarAnimationPreset.MaleIdleShiftWeight, true);
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

            if (isMovingToMerchant)
            {
                moveToMerchantEvent.Update(gameTime);
            }

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


            merchantTextObject.Update(gameTime);

            if (elapsedTime >= 5.0f)
            {
                isShowingIntroduction = false;
            }
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
            strafeUpgradeCost = (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel + 1) * 500;

            healthUpgradeMenuEntry.Description = "Price: $" + healthCost.ToString("N0") + "\n\nDescription:\nReceive +1 Health\n\nCurrent Health:\n" + this.theArcadeLevel.CurrentPlayer.Health.ToString();

            grenadeUpgradeMenuEntry.Description = "Price: $" + grenadeCost.ToString("N0")
                 + "\n\nDescription:\nDefeats all on-screen zombies\nwith a huge explosion."//"\n\nDescription:\nDefeat all on-screen zombies\nwith a cleansing gas shower."
                 + "\n\nCurrent Grenades:\n" + this.theArcadeLevel.CurrentPlayer.NumberOfGrenades.ToString();

            if (this.theArcadeLevel.CurrentPlayer.StrafeSpeedLevel >= Player.MaxUpgradeLevel)
            {
                strafeUpgradeMenuEntry.Description = "Strafe Speed Maxed Out!\n\nCurrent Strafe Speed:\n" + (this.theArcadeLevel.CurrentPlayer.StrafeSpeed * 100).ToString() + "%";
            }

            else
            {
                strafeUpgradeMenuEntry.Description = "Price: $" + strafeUpgradeCost.ToString("N0") +
                     "\n\nDescription:\nIncrease Strafe Speed\nby 10%\n\nCurrent Strafe Speed:\n" + (this.theArcadeLevel.CurrentPlayer.StrafeSpeed * 100).ToString() + "%";
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

            Color blackWithTransition = Color.Black * (175f / 255f) * (TransitionAlpha / 255f);

            if (showConfirmation)
            {
                GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, confirmationRectangle, 2, Color.White * (TransitionAlpha / 255f));

                MySpriteBatch.DrawCentered(blankTexture.Texture2D, confirmationRectangle, blackWithTransition);

                TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.TitleFont].SpriteFont, confirmationString,
                     new Vector2(confirmationRectangle.X, confirmationRectangle.Y), Color.Gold, 0.25f);
            }

            if (isShowingIntroduction)
            {
                GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, introductionRectangle,
                     2, Color.White * (TransitionAlpha / 255f));


                MySpriteBatch.DrawCentered(blankTexture.Texture2D, introductionRectangle, blackWithTransition);

                merchantTextObject.Color = Color.Gold * (175f / 255f) * (TransitionAlpha / 255f);

                merchantTextObject.Draw(gameTime);
            }

            if (!isShowingIntroduction)
            {
                GraphicsHelper.DrawBorderCenteredFromRectangle(blankTexture.Texture2D, upgradeRectangle, 2, Color.White * (TransitionAlpha / 255f));


                MySpriteBatch.DrawCentered(blankTexture.Texture2D, upgradeRectangle, blackWithTransition);

                if (this.SelectedEntryIndex != MenuEntries.Count - 1)
                {
                    MySpriteBatch.Draw360Button(Buttons.A,
                         new Vector2(EngineCore.GraphicsDevice.Viewport.Width * (940f / 1280f) - 90f,
                              EngineCore.ScreenCenter.Y + 90f),
                              Color.White * (TransitionAlpha / 255f), 0.0f, MySpriteBatch.Measure360Button(Microsoft.Xna.Framework.Input.Buttons.A) / 2f, 0.4f, SpriteEffects.None, 0f);

                    string purchaseOrBrowse = "Purchase";

                    if (this.SelectedEntryIndex == 1 || this.SelectedEntryIndex == 4)
                    {
                        purchaseOrBrowse = "Browse";
                    }

                    TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, purchaseOrBrowse,
                         new Vector2(EngineCore.GraphicsDevice.Viewport.Width * (940f / 1280f) + 25f,
                              EngineCore.ScreenCenter.Y + 90f), Color.Gold * (TransitionAlpha / 255f), 0.75f);

                    TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.HudFont].SpriteFont, "Your Money: $" + this.theArcadeLevel.CurrentPlayer.Money.ToString("N0"),
                         new Vector2((940f / 1280f) * EngineCore.GraphicsDevice.Viewport.Width,
                              EngineCore.ScreenCenter.Y + 55f), Color.White * (TransitionAlpha / 255f), 0.75f);

                }
            }

            MySpriteBatch.End();

            // Only have the base call draw (to render menu stuff) 
            // if we aren't showing the introduction text.
            if (!isShowingIntroduction)
            {
                base.Draw(gameTime);
            }
        }

        #endregion
    }
}