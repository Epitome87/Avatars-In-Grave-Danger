#region File Description
//-----------------------------------------------------------------------------
// AchievementList.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using PixelEngine.AchievementSystem;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// AchievementList is simply a helper class to initialize the
     /// game-specific Achievements. Achievement creation is done in this
     /// file, as well as linking each delegate to an appropriate Progression Checking
     /// method.
     /// </remarks>
     public static class AchievementList
     {
          #region Achievement Progression Checking Methods

          #region Gimmick, "Free" Achievements

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to purchasing Avatar Typing.
          /// </summary>
          public static bool GamePurchasedProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.GamePurchased;
          }

          /// <summary>
          /// A CheckProgressDelegate for checking if the Player is
          /// a friend of "i Epitome i"s.
          /// </summary>
          public static bool DevelopersFriendProgressFunction(int targetValue)
          {
               if (AvatarZombieGame.CurrentPlayer == null)
                    return false;

               if (AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer == null)
                    return false;

               if (AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsGuest ||
                   !AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.IsSignedInToLive)
                    return false;

               // First of all, is the Player Epitome?!?!
               if (AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.Gamertag.ToLower() == "i epitome i")
               {
                    return true;
               }


               Microsoft.Xna.Framework.GamerServices.FriendCollection friends;

               // Grab the Player's Friend List.
               friends = AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.GetFriends();

               // Iterate through all the Player's friends...
               foreach (Microsoft.Xna.Framework.GamerServices.FriendGamer friend in friends)
               {
                    // If they have "i Epitome i" as a friend, Achievement Unlocked!
                    if (friend.Gamertag.ToLower() == "i epitome i")
                    {
                         return true;
                    }
               }

               // Otherwise, they receive no Achievement.
               return false;
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to watching the Credits.
          /// </summary>
          public static bool CreditsWatchedProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.CreditsWatched;
          }

          public static bool VibrationSetProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.VibrationSet;
          }

          #endregion

          #region Life-Time Achievements

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool TotalKillsProgressFunction(int targetValue)
          {
               return (AvatarZombieGame.AwardData.TotalKills >= targetValue);
          }

          #endregion

          #region Streak-Based Achievements

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool SpeedKillStreakProgressFunction(int targetValue)
          {
               return (AvatarZombieGame.AwardData.SpeedStreak >= targetValue);
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool PerfectKillStreakProgressFunction(int targetValue)
          {
               return (AvatarZombieGame.AwardData.AccuracyStreak >= targetValue);
          }

          #endregion

          #region Level-Based Achievements

          public static bool WaveCompleteNormalProgressFunction(int targetValue)
          {
               return ZombieGameSettings.Difficulty >= Difficulty.Normal &
                    AvatarZombieGame.AwardData.CurrentWave >= targetValue;
          }

          public static bool WaveCompleteHardProgressFunction(int targetValue)
          {
               return ZombieGameSettings.Difficulty >= Difficulty.Hard &
                    AvatarZombieGame.AwardData.CurrentWave >= targetValue;
          }

          #endregion

          #region Grenade-Based Achievements

          public static bool GrenadeSimultaneousKillProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.GrenadeSimultaneousKillCount >= targetValue;
          }

          #endregion

          #region Upgrade-Based Achievements

          public static bool TotalUpgradesProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.TotalUpgrades >= targetValue;
          }

          public static bool NoUpgradesUntilWave5ProgressFunction(int targetValue)
          {
               return
                    AvatarZombieGame.AwardData.TotalUpgrades == 0 && AvatarZombieGame.AwardData.CurrentWave >= targetValue;
          }

          public static bool TotalSimultaneousUpgradesProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.TotalSimultaneousUpgrades >= targetValue;
          }

          public static bool FullyUpgradedOneProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.FullyUpgradedOne;
          }

          public static bool FullyUpgradedAllProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.FullyUpgradedAll;
          }

          #endregion

          #region Accuracy-Based Achievements

          public static bool AccuracyProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.Accuracy >= targetValue;
          }

          public static bool ConsecutivePerfectAccuracyProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.ConsecutivePerfectAccuracy >= targetValue;
          }

          #endregion

          #region Enemy-Based Achievements

          public static bool StopBeserkerProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.StoppedBeserkerFromCharging; ;
          }

          public static bool StopWarperProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.StoppedWarperFromWarping;
          }

          #endregion



          public static bool CloseCallsProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.TotalCloseCalls >= targetValue;
          }


          // Not working correctly - Remove it?
          public static bool ToldPeopleAboutGameProgressFunction(int targetValue)
          {
               return AvatarZombieGame.AwardData.ToldPeopleAboutGame;
          }

          #endregion


          #region Achievement Initialization

          public static void InitializeAchievements()
          {
               #region Gimmick, "Free" Achievements

               AchievementManager.AddAchievement(new Achievement("Money, Please!", "You Purchased Avatars In Grave Danger", 15, 0, GamePurchasedProgressFunction));

               AchievementManager.AddAchievement(new Achievement("Friends In High Places", "You Know The Developer", 15, 0, DevelopersFriendProgressFunction));

               AchievementManager.AddAchievement(new Achievement("Recognized!", "You Fully Watched Credits", 15, 0, CreditsWatchedProgressFunction));

               //AchievementManager.AddAchievement(new Achievement("Spread Our Codes To The Stars", "You Told Friends About This Game", 15, 0, ToldPeopleAboutGameProgressFunction));

               #endregion

               #region Secret Achievements.

               AchievementManager.AddAchievement(new Achievement("I Just Wanted To Hold", "You Enabled Vibration...And Stayed to Enjoy It", 15, true, 0, VibrationSetProgressFunction));

               #endregion

               #region Life-Time Achievements.

               // Novice Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Zombie Tamer", "Obtain 100 Total Kills", 15, 100, TotalKillsProgressFunction));

               // Amateur Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Zombie Destroyer", "Obtain 500 Total Kills", 25, 500, TotalKillsProgressFunction));

               // Pro Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Zombie Exterminator", "Obtain 1,000 Total Kills", 50, 1000, TotalKillsProgressFunction));


               // God Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "For Reals", "Obtain 5,000 Total Kills", 100, 5000, TotalKillsProgressFunction));

               #endregion

               #region Level-Based Awards.

               #region Arcade Mode: Normal Awards

               AchievementManager.AddAchievement(new Achievement("Fights, Battles Have Begun", "Complete Wave 1", 15, 1, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Revenge Will Surely Come", "Complete Wave 2", 15, 2, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Your Hard Times Are Ahead", "Complete Wave 3", 15, 3, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Don't Give Up The Fight", "Complete Wave 4", 15, 4, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("You Will Be Alright", "Complete Wave 5", 20, 5, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("You're Swelling Up", "Complete Wave 10", 25, 10, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("You're Unstoppable", "Complete Wave 15", 50, 15, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("I've Exposed Your Lies, Baby", "Complete The Game On Normal", 75, 20, WaveCompleteNormalProgressFunction));

               #endregion

               #region Arcade Mode: Insane Awards

               AchievementManager.AddAchievement(new Achievement("Invincible", "Complete The Game On Hard", 150, 20, WaveCompleteHardProgressFunction));

               #endregion

               #endregion

               #region Grenade-Based Awards

               AchievementManager.AddAchievement(new Achievement("You Set My Soul Alight", "Defeat 5 Enemies With A Single Grenade", 15, 5, GrenadeSimultaneousKillProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Baker's Dozen", "Defeat 13 Enemies With A Single Grenade", 25, 13, GrenadeSimultaneousKillProgressFunction));

               #endregion

               #region Upgrade-Based Awards.

               // Don't upgrade until Wave 5
               AchievementManager.AddAchievement(new Achievement("Bare Necessities", "Don't Purchase An Upgrade Before Wave 6", 20, 5, NoUpgradesUntilWave5ProgressFunction));

               // Purchase your first upgrade
               AchievementManager.AddAchievement(new Achievement("My First", "Purchase Your First Upgrade", 15, 1, TotalUpgradesProgressFunction));

               // Purchase 5 upgrades in a single wave
               AchievementManager.AddAchievement(new Achievement("Hoarder", "Purchase 5 Upgrades In A Single Wave", 20, 5, TotalSimultaneousUpgradesProgressFunction));

               // Fully upgrade 1 thing.
               AchievementManager.AddAchievement(new Achievement("Specialist", "Fully Upgraded One Attribute", 25, 0, FullyUpgradedOneProgressFunction));

               // Fully upgrade everything.
               AchievementManager.AddAchievement(new Achievement("Pimped Out", "Full Upgraded All Attributes", 75, 0, FullyUpgradedAllProgressFunction));

               #endregion

               #region Score-Based Awards

               #endregion

               #region Enemy-Based Awards

               // Kill a Beserker before he charges
               AchievementManager.AddAchievement(new Achievement("What's The Hurry?", "Defeat a Beserker Zombie Before It Charges", 15, 0, StopBeserkerProgressFunction));

               // Kill a warper before he warps
               AchievementManager.AddAchievement(new Achievement("Can't Let You Do That...", "Defeat a Warper Zombie Before It Warps", 15, 0, StopWarperProgressFunction));

               #endregion

               #region Accuracy-Based Achievements

               AchievementManager.AddAchievement(new Achievement("I Don't Miss", "Obtain 100% Accuracy During A Wave", 15, 100, AccuracyProgressFunction));
               AchievementManager.AddAchievement(new Achievement("...Ever", "Obtain 100% Accuracy 5 Consecutive Waves", 25, 5, ConsecutivePerfectAccuracyProgressFunction));

               #endregion

               #region Close Call!-Based Achievements

               AchievementManager.AddAchievement(new Achievement("Close Call!", "Obtain Your First Close Call", 15, 1, CloseCallsProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Too Close", "Obtain 100 Close Calls", 50, 100, CloseCallsProgressFunction));

               #endregion
          }

          #endregion
     }
}