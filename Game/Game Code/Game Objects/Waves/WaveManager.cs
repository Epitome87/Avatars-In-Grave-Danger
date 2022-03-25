#region File Description
//-----------------------------------------------------------------------------
// WaveManager.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// A class that manages Waves.
     /// </remarks>
     public class WaveManager
     {
          #region Fields

          /// <summary>
          /// How many waves the game will have.
          /// </summary>
          private int numberOfWaves;

          /// <summary>
          /// How long since the last wave ended.
          /// </summary>
          private float timeSinceLastWave;

          /// <summary>
          /// A queue of all the waves.
          /// </summary>
          private Queue<Wave> waves = new Queue<Wave>();

          /// <summary>
          /// Is the current wave over?
          /// </summary>
          private bool isWaveFinished;

          /// <summary>
          /// A reference to the level class.
          /// </summary>
          private Level level;     
          
          /// <summary>
          /// A reference to the Player.
          /// </summary>
          private Player player;

          /// <summary>
          /// The Total Score achieved throughout all Waves.
          /// * This does not include the Score for the current Wave, as
          /// we manually add to this value at the end of each Wave.
          /// </summary>
          private int score = 0;

          #endregion
 

          #region Properties

          /// <summary>
          /// Get the Wave at the front of the queue.
          /// </summary>
          public Wave CurrentWave
          {
               get { return waves.Peek(); }
          }

          /// <summary>
          /// Get a List of the current Enemies.
          /// </summary>
          public List<Enemy> Enemies
          {
               get { return CurrentWave.Enemies; }
          }

          /// <summary>
          /// Returns the Round number.
          /// This is WaveNumber + 1.
          /// </summary>
          public int Round
          {
               get { return CurrentWave.WaveNumber + 1; }
          }

          /// <summary>
          /// A reference to the Player.
          /// </summary>
          public Player CurrentPlayer
          {
               get { return player; }
               set { player = value; }
          }     

          /// <summary>
          /// Gets or Sets the Total Score achieved throughout all Waves.
          /// * This does not include the Score for the current Wave, as
          /// we manually add to this value at the end of each Wave.
          /// </summary>
          public int Score
          {
               get { return score; }
               set { score = value; }
          }

          /// <summary>
          /// Returns the Total number of Enemies Killed throughout all Waves.
          /// </summary>
          public int EnemiesKilled
          {
               get { return enemiesKilled; }
               set { enemiesKilled = value; }
          }

          int enemiesKilled = 0;

          /// <summary>
          /// Returns the Total number of Enemies Escaped throughout all Waves.
          /// </summary>
          public int EnemiesEscaped
          {
               get { return enemiesEscaped; }
               set { enemiesEscaped = value; }
          }

          int enemiesEscaped = 0;

          /// <summary>
          /// Gets or sets the Total number of Close Calls throughout all Waves.
          /// </summary>
          public int CloseCalls
          {
               get { return closeCalls; }
               set { closeCalls = value; }
          }

          private int closeCalls = 0;

          /// <summary>
          /// Returns whether all Waves in this Wave Manager
          /// have been completed. This results in a Game Complete.
          /// </summary>
          public bool IsAllWavesFinished
          {
               get { return isAllWavesFinished; }
          }

          private bool isAllWavesFinished = false;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructs a new Wave Manager object.
          /// </summary>
          /// <param name="level">A reference to the Level.</param>
          /// <param name="numberOfWaves">How many waves this game consists of.</param>
          public WaveManager(Level level, int numberOfWaves)
          {
               this.numberOfWaves = numberOfWaves;
               this.level = level;


               this.CurrentPlayer = AvatarZombieGame.CurrentPlayer;

               for (int i = 0; i < numberOfWaves; i++)
               {
                    int initialNumberOfEnemies = 5;

                    // Number Modifier: Wave Number * 5
                    // But we reset every 10 waves.
                    int numberModifier = ((i % 10) * 5);

                    Wave wave = new Wave//(this, i, 5 + ((8 % 10) * 5), level);
                    (this, i, initialNumberOfEnemies + numberModifier, level);

                    waves.Enqueue(wave);
               }

               // Start the next (first) wave.
               StartNextWave();
          }

          #endregion


          #region Update

          public void Update(GameTime gameTime)
          {
               if (isAllWavesFinished)
                    return;

               // Update the current Wave.
               CurrentWave.Update(gameTime);

               // Check if the current Wave has finished.
               if (CurrentWave.WaveOver)
               {
                    isWaveFinished = true;
               }

               // If the current Wave has finished...
               if (isWaveFinished)
               {
                    // Start the timer.
                    timeSinceLastWave += (float)gameTime.ElapsedGameTime.TotalSeconds;



                    // New (Mine): Check if all waves have been completed.
                    if (this.CurrentWave.WaveNumber + 1 == this.numberOfWaves)
                    {
                         isAllWavesFinished = true;
                    }
               }

               // If 30 seconds has passed...
               if (timeSinceLastWave > 30.0f)
               {
                    // Remove this now-finished Wave.
                    waves.Dequeue();

                    // Start the next Wave.
                    StartNextWave();
               }
          }

          #endregion


          #region Draw

          public void Draw(GameTime gameTime)
          {
               // Draw the current Wave.
               CurrentWave.Draw(gameTime);
          }

          #endregion


          #region Private Helper Methods

          /// <summary>
          /// Starts the next wave in the Wave Manager.
          /// </summary>
          public void StartNextWave()
          {
               // If there are still waves left...
               if (waves.Count > 0)
               {
                    // Start the next one!
                    waves.Peek().Start();

                    // Reset timer.
                    timeSinceLastWave = 0.0f;

                    // Reset wave finished flag.
                    isWaveFinished = false;
               }
          }

          #endregion


          #region Public Remove Current Wave

          /// <summary>
          /// Awful design decision.
          /// </summary>
          public void RemoveCurrentWave()
          {
               Score += CurrentWave.Score;

               waves.Dequeue();
          }

          #endregion
     }
}
