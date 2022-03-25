#region File Description
//-----------------------------------------------------------------------------
// SaveGameData.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statement
using System.Collections.Generic;
#endregion

namespace PixelEngine
{
     #region Serializable SaveGameData Structure

     //[Serializable]
     public struct SaveGameData
     {
          // Settings. Temp design until I make it its own Struct.
          public int FontSize;          // Convert to Enum.
          public int Difficulty;        // Convert to Enum.

          public bool VibrationEnabled;

          public int SoundVolume;
          public int MusicVolume;

          // General gameplay information.
          public string PlayerName;
          public uint Level;
          public float HighScore;

          // Achievement data.
          public List<bool> IsUnlockedAchievement;
          public AchievementData AwardData;
     }
     
     #endregion

     #region Serializable AchievementData Structure

     //[Serializable]
     public struct AchievementData
     {
          // Gimmick achievement variables.
          public bool GamePurchased;
          public bool CreditsWatched;
          public bool VibrationSet;

          // Lifetime achievement variables.
          public uint TotalKills;
          public uint TotalSpeedKills;

          // Streak-based achievement variables.
          public uint SpeedStreak;
          public uint AccuracyStreak;

          // Level-based achievement variables.
          public uint CurrentWave;
          public float CurrentSurvivalTime;

          // Upgrade-based achievement variables.
          public uint TotalUpgrades;
          public uint TotalSimultaneousUpgrades;
          public bool FullyUpgradedOne;
          public bool FullyUpgradedAll;

          public uint TotalMoney;

          // Grenade-based achievement variables.
          public uint GrenadeSimultaneousKillCount;

          // Accuracy-based achievement variables.
          public uint Accuracy;
          public uint ConsecutivePerfectAccuracy;

          // Enemy-based achievement variables.
          public bool StoppedBeserkerFromCharging;
          public bool StoppedWarperFromWarping;


          public uint TotalCloseCalls;

          public bool ToldPeopleAboutGame;
     };

     #endregion
}