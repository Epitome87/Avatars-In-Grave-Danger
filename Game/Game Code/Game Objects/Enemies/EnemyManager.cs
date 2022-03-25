#region File Description
//-----------------------------------------------------------------------------
// EnemyManager.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Screen;
#endregion

namespace AvatarWaveDefense
{
     /// <remarks>
     /// The Enemy Manager handles the drawing and updating of all Enemies within it.
     /// It Spawns Waves and adds / removes Enemies when necessary. It also ensures 
     /// Enemies have, for the most part, unique TypingWord.Text properties.
     /// </remarks>
     public class EnemyManager : DrawableGameComponent
     {
          #region SpawnLocation Enum

          public enum SpawnLocation
          {
               Empty,
               FarLeft,
               Left,
               Right,
               FarRight,
          }

          public enum ColorSpawnLocation
          {
               Green, 
               Red,
               Yellow,
               Blue
          }

          public static Color ConvertToColor(ColorSpawnLocation color)
          {
               switch (color)
               {
                    case ColorSpawnLocation.Green:
                         return Color.Green;

                    case ColorSpawnLocation.Red:
                         return Color.Red;

                    case ColorSpawnLocation.Yellow:
                         return Color.Yellow;

                    case ColorSpawnLocation.Blue:
                         return Color.Blue;

                    default:
                         return Color.White;
               }
          }

          #endregion


          #region Fields

          public List<Enemy> aliveEnemies = new List<Enemy>();
          private List<Enemy> dyingEnemies = new List<Enemy>();
          private List<Enemy> escapingEnemies = new List<Enemy>();

          private Random random = new Random();
          private Enemy currentTarget;
          public bool isTargetSelected;
          private float ElapsedTime = 0.0f;

          // Meta-Data for Player.
          private int enemiesKilled;
          private int enemiesEscaped;
          private int perfectKills;
          private int speedKills;
          private int score;
          private float killTime;
          private uint waveNumber;
          private bool waveDestroyed;

          public int numberOfEnemiesSpawned = 0;

          public List<ColorSpawnLocation> usedColorSpawnLocations = new List<ColorSpawnLocation>();
          public List<SpawnLocation> usedSpawnLocations = new List<SpawnLocation>();

          public Player currentPlayer = AvatarZombieGame.CurrentPlayer;

          public const float MIN_SPAWN_Z = 30;//35f;
          public const float MAX_SPAWN_Z = 5;//20f;

          private int numberOfSimultaneousEnemies = 3;

          private AvatarAnimation normalAnimation = new AvatarAnimation(AvatarAnimationPreset.MaleAngry);

          // A List of scripted enemies that should be spawned.
          public List<Enemy> scriptedEnemies = new List<Enemy>();
          public List<EnemyType> scriptedEnemyTypes = new List<EnemyType>();

          public bool IsScriptedWave = true;


          public int Mode = 1;     // 0 is Normal, 1 is Colored.

          #endregion


          #region Properties

          /// <summary>
          /// Amount of Enemies killed in this Enemy Manager.
          /// </summary>
          public int EnemiesKilled
          {
               get { return enemiesKilled; }
               set { enemiesKilled = value; }
          }

          /// <summary>
          /// Amount of Enemies who escaped in this Enemy Manager.
          /// </summary>
          public int EnemiesEscaped
          {
               get { return enemiesEscaped; }
               set { enemiesEscaped = value; }
          }

          /// <summary>
          /// Amount of Perfect Kills made against this Enemy Manager.
          /// </summary>
          public int PerfectKills
          {
               get { return perfectKills; }
               set { perfectKills = value; }
          }

          /// <summary>
          /// Amount of Speed Kills made against this Enemy Manager.
          /// </summary>
          public int SpeedKills
          {
               get { return speedKills; }
               set { speedKills = value; }
          }

          /// <summary>
          /// Score obtained on this Enemy Manager.
          /// </summary>
          public int Score
          {
               get { return score; }
               set { score = value; }
          }

          /// <summary>
          /// Amount of Kill Time made against this Enemy Manager.
          /// </summary>
          public float KillTime
          {
               get { return killTime; }
               set { killTime = value; }
          }

          public List<float> GetWaveData()
          {
               List<float> waveData = new List<float>();
               waveData.Add(WaveNumber);
               waveData.Add(Score);
               waveData.Add(EnemiesKilled);
               waveData.Add(KillTime);
               waveData.Add(PerfectKills);
               waveData.Add(SpeedKills);
               waveData.Add(EnemiesEscaped);

               return waveData;
          }

          /// <summary>
          /// Amount of Enemies in the Enemy Manager.
          /// </summary>
          public int Size
          {
               get { return aliveEnemies.Count; }
          }

          /// <summary>
          /// The current Wave Number being played.
          /// </summary>
          public uint WaveNumber
          {
               get { return waveNumber; }
               set { waveNumber = value; }
          }

          /// <summary>
          /// Returns True if Wave has been destroyed, false otherwise.
          /// </summary>
          public bool WaveDestroyed
          {
               get { return waveDestroyed; }
               set { waveDestroyed = value; }
          }

          /// <summary>
          /// Gets or sets how many Enemies are on-screen at any given time.
          /// </summary>
          public int NumberOfSimultaneousEnemies
          {
               get { return numberOfSimultaneousEnemies; }
               set { numberOfSimultaneousEnemies = value; }
          }

          #endregion


          #region Initialization

          public EnemyManager(Game game)
               : base(game)
          {
               WaveNumber = 1;
               WaveDestroyed = false;
          }

          #endregion


          #region Update

          /// <summary>
          /// Allows each Enemy to run update logic.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // For typical wave defense gameplay.
               foreach (Enemy enemy in aliveEnemies)
               {
                    foreach (Bullet bullet in this.currentPlayer.Gun.BulletList)
                    {
                         Vector3 bPos = new Vector3();
                         bPos = bullet.Position;

                         Vector3 bPosMin = new Vector3(bPos.X + 0.5f, bPos.Y, bPos.Z);
                         Vector3 bPosMax = new Vector3(bPos.X - 0.5f, bPos.Y, bPos.Z);

                         if (enemy.Position.X <= bPosMin.X && enemy.Position.X >= bPosMax.X)
                         {
                              // Potential collision. Check for depth now.
                              if (bPos.Z >= enemy.Position.Z)
                              {
                                   // Collision.

                                   // If this was an Explosive Bullet...
                                   if (bullet.bulletType == Bullet.BulletType.Explosive)  //if (bullet.IsExplosiveBullet)
                                   {
                                        // Burn this enemy to a crisp!
                                        enemy.Avatar.LightColor = new Vector3(0, 0, 0);
                                        enemy.Avatar.LightDirection = new Vector3(0, 0, 0);
                                        enemy.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);

                                        // Create an explosion.

                                        // Harm other nearby enemies.
                                        foreach (Enemy nearbyEnemy in this.GetEnemies())
                                        {
                                             // The Enemy shouldn't explode itself, obviously!
                                             if (nearbyEnemy == enemy)
                                                  continue;

                                             BoundingSphere explosiveSphere = new BoundingSphere(enemy.Position, 5.0f);

                                             if (explosiveSphere.Intersects(new BoundingSphere(nearbyEnemy.Position, 0.1f)))
                                             {
                                                  nearbyEnemy.OnKilled();

                                                  // BURN HIM TO A CRISP!!!
                                                  nearbyEnemy.Avatar.LightColor = new Vector3(0, 0, 0);
                                                  nearbyEnemy.Avatar.LightDirection = new Vector3(0, 0, 0);
                                                  nearbyEnemy.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);
                                             }
                                        }

                                        enemy.OnHit();

                                        break;
                                   }

                                   else
                                   {
                                        enemy.OnHit();

                                        if (bullet.bulletType != Bullet.BulletType.Piercing)
                                        {
                                             // Flag to remove the bullet.
                                             // Should we put this here even?
                                             bullet.RemoveBullet = true;
                                        }

                                        // Break since the bullet is now gone.
                                        break;
                                   }
                              }
                         }
                    }
               }
               

               /*
               // Testing colored bullets.
               foreach (TypingEnemy enemy in aliveEnemies)
               {
                    foreach (Bullet bullet in this.currentPlayer.BulletList)
                    {
                         Vector3 bPos = new Vector3();
                         bPos = bullet.Position;

                         Vector3 bPosMin = new Vector3(bPos.X + 0.75f, bPos.Y, bPos.Z);
                         Vector3 bPosMax = new Vector3(bPos.X - 0.75f, bPos.Y, bPos.Z);

                         if (enemy.Position.X <= bPosMin.X && enemy.Position.X >= bPosMax.X)
                         {
                              // Potential collision. Check for depth now.
                              if (bPos.Z >= enemy.Position.Z)
                              {
                                   if (currentPlayer.ConvertToColor(currentPlayer.ColorOfBullet) == enemy.Color)
                                   {
                                        // Collision.
                                        enemy.OnHit();
                                        //enemy.OnKilled();

                                        bullet.RemoveBullet = true;

                                        // Break since the bullet is now gone.
                                        break;
                                   }
                              }
                         }
                    }
               }
               */

               
               /*
               // Testing Rock Band type gameplay.
               foreach (TypingEnemy enemy in aliveEnemies)
               {
                    foreach (Bullet bullet in this.currentPlayer.BulletList)
                    {
                         Vector3 bPos = new Vector3();
                         bPos = bullet.Position;

                         Vector3 bPosMin = new Vector3(bPos.X + 100f, bPos.Y, bPos.Z);
                         Vector3 bPosMax = new Vector3(bPos.X - 100f, bPos.Y, bPos.Z);

                         if (enemy.Position.X <= bPosMin.X && enemy.Position.X >= bPosMax.X)
                         {
                              // Potential collision. Check for depth now.
                              if (enemy.Position.Z <= (1.0f + 0.5f) && enemy.Position.Z >= (1.0f - 1.5f))
                              {
                                   if (currentPlayer.ConvertToColor(currentPlayer.ColorOfBullet) == EnemyManager.ConvertToColor(enemy.ColorSpawnLocation))//enemy.Color)
                                   {
                                        // Collision.
                                        enemy.OnHit();
                                        //enemy.OnKilled();

                                        bullet.RemoveBullet = true;

                                        // Break since the bullet is now gone.
                                        break;
                                   }
                              }
                         }
                    }
               }
               */
               
               

               if (currentTarget != null)
               {
                    Player.TargetLocation = currentTarget.Position;
                    //currentPlayer.FuckingTurn();
               }


               ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               #region Update Alive & Dying Eneimes

               // Update all the alive enemies.
               foreach (Enemy enemy in aliveEnemies)
               {
                    //enemy.Avatar.AmbientLightColor = enemy.Color.ToVector3();

                    enemy.Update(gameTime);
               }

               // Update all the dying enemies.
               foreach (Enemy enemy in dyingEnemies)
               {
                    enemy.Avatar.AmbientLightColor = enemy.Color.ToVector3();

                    enemy.Update(gameTime);
               }

               // Update all the escaping enemies.
               foreach (Enemy enemy in escapingEnemies)
               {
                    enemy.Update(gameTime);
               }

               // Check for dying enemies which have become dead.
               foreach (Enemy enemy in dyingEnemies)
               {
                    if (enemy.ElapsedDyingTime >= 2.5f)//5.0f)
                    {
                         if (dyingEnemies != null)
                         {
                              this.RemoveDyingEnemy(enemy, true);
                              break;
                         }
                    }
               }

               // Check for escaping enemies which have become escaped.
               foreach (Enemy enemy in escapingEnemies)
               {
                    if (enemy.Position.Z < -10 || enemy.Position.X < -25)
                    {
                         if (escapingEnemies != null)
                         {
                              this.RemoveEscapingEnemy(enemy, true);
                              break;
                         }
                    }
               }

               // Check for newly-dead enemies.
               foreach (Enemy enemy in aliveEnemies)
               {
                    if (enemy.IsDead)
                    {
                         if (enemy.IsSpeedKill)
                         {
                              SpeedKills++;
                         }

                         if (enemy.IsPerfectKill)
                         {
                              PerfectKills++;
                         }

                         float scaledPoints = enemy.BasePoints * 
                              ((int)AvatarTypingGameSettings.Difficulty + 1) * 0.5f;

                         Score += (int)scaledPoints;



                         // testing for money logic.
                         this.currentPlayer.Money += enemy.BasePoints;



                         EnemiesKilled++;
                         KillTime += enemy.ElapsedKillTime;

                         dyingEnemies.Add(enemy);

                         this.RemoveEnemy(enemy, false);

                         isTargetSelected = false;

                         break;
                    }
               }

               // Only check for escaping enemies if there are enemies present.
               if (aliveEnemies.Count > 0)
               {
                    Enemy escapedEnemy = null;

                    foreach (Enemy enemy in aliveEnemies)
                    {
                         if (currentPlayer == null)
                         {
                              if (enemy.IsCollision(new Vector3(0, 0, 4)))//new Vector3(0, 0, 0)))
                              {
                                   enemy.OnCollide(currentPlayer);

                                   enemy.OnEscaped();

                                   EnemiesEscaped++;
                                   escapedEnemy = enemy;

                                   this.RemoveEnemy(enemy, true);

                                   break;
                              }
                         }

                         else if (enemy.IsCollision(this.currentPlayer.Position))
                         {
                              enemy.OnCollide(currentPlayer);

                              enemy.OnEscaped();

                              EnemiesEscaped++;
                              escapedEnemy = enemy;
                              
                              // How about we don't remove the Enemy until he's off-screen?!
                              //this.RemoveEnemy(enemy, true);

                              enemy.IsEscaping = true;
                              escapingEnemies.Add(enemy);
                              this.RemoveEnemy(enemy, false);

                              break;
                         }
                    }

                    // If our target was the one that escaped...
                    if (currentTarget == escapedEnemy)
                    {
                         // We no longer have a valid target selected.
                         isTargetSelected = false;

                         // Reset the target to point to nothing.
                         currentTarget = null;
                    }
               }

               #endregion
          }

          #endregion


          #region Draw

          /// <summary>
          /// Tells each Enemy to draw itself. Uses the default SpriteBatch and Font found in ScreenManager.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               // Draw the Dead Enemies first!
               foreach (Enemy enemy in dyingEnemies)
               {
                    enemy.Avatar.AmbientLightColor = EnemyManager.ConvertToColor(enemy.ColorSpawnLocation).ToVector3();// enemy.Color.ToVector3();
                    enemy.Draw(gameTime, ScreenManager.Font);
               }

               // Draw the Alive Enemies next - just their Avatars!
               for (int i = aliveEnemies.Count - 1; i >= 0; i--)
               {
                    if (i >= aliveEnemies.Count)
                         break;

                    //aliveEnemies[i].Avatar.AmbientLightColor = EnemyManager.ConvertToColor(aliveEnemies[i].ColorSpawnLocation).ToVector3(); //aliveEnemies[i].Color.ToVector3();
                    aliveEnemies[i].Draw(gameTime, ScreenManager.Font);
               }

               // Draw the Escaping Enemies last!
               foreach (Enemy enemy in escapingEnemies)
               {

                    enemy.Avatar.AmbientLightColor = EnemyManager.ConvertToColor(enemy.ColorSpawnLocation).ToVector3(); //enemy.Avatar.AmbientLightColor = enemy.Color.ToVector3();
                    enemy.Draw(gameTime, ScreenManager.Font);
               }
          }

          /// <summary>
          /// Tells each Enemy to draw itself. Uses the default SpriteBatch and Font found in ScreenManager.
          /// </summary>
          public void DrawWithoutWords(GameTime gameTime)
          {
               // Draw the Dead Enemies first!
               foreach (Enemy enemy in dyingEnemies)
               {
                    enemy.Draw(gameTime, ScreenManager.Font);
               }

               // Draw the Alive Enemies next - just their Avatars!
               for (int i = aliveEnemies.Count - 1; i >= 0; i--)
               {
                    if (i >= aliveEnemies.Count)
                         break;

                    aliveEnemies[i].Draw(gameTime, ScreenManager.Font);
               }
          }

          #endregion


          #region Public Add / Get Methods

          /// <summary>
          /// Adds a new Enemy to the Enemy Manager.
          /// </summary>
          public void AddEnemy(Enemy enemy)
          {
               enemy.Wave = this;
               //enemy.EnemyManager = this;
               aliveEnemies.Add(enemy);
          }

          /// <summary>
          /// Expose an array holding all the enemies. We return a copy rather
          /// than the real master list, because enemies should only ever be added
          /// or removed using the AddEnemy and RemoveEnemy methods.
          /// </summary>
          public Enemy[] GetEnemies()
          {
               return aliveEnemies.ToArray();
          }

          /// <summary>
          /// Expose an array holding all the enemies. We return a copy rather
          /// than the real master list, because enemies should only ever be added
          /// or removed using the AddEnemy and RemoveEnemy methods.
          /// </summary>
          public Enemy GetEnemyAt(int index)
          {
               return aliveEnemies[index];
          }

          #endregion


          #region Wave Initialization
          
          /// <summary>
          /// Generates an Enemy of the specified type.
          /// 
          /// Simply uses a switch-case to instanciate the correct TypingEnemy type.
          /// </summary>
          /// <param name="typeOfEnemy">The type of Enemy to generate.</param>
          /// <returns>The generated Enemy with default properties.</returns>
          public Enemy GenerateEnemy(EnemyType typeOfEnemy)
          {
               Vector3 position = new Vector3();
               Enemy enemy;

               switch ((int)typeOfEnemy)
               {
                    // Arcade Mode with Scripted Enabled will just use these.

                    case (int)EnemyType.Normal:
                         enemy = new NormalTypingEnemy(position, this, "X");
                         break;

                    case (int)EnemyType.Fast:
                         enemy = new FastTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Kamikaze:
                         enemy = new SuicideTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Explosive:
                         enemy = new ExplodingTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Deflatable:
                         enemy = new DeflatingTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Horde:
                         enemy = new HordeTypingEnemy(position, this);
                         break;
  
                    case (int)EnemyType.Bonus:
                         enemy = new BonusEnemy(new Vector3(15f, 0f, 20f), this);
                         break;

                    // Survival Mode extends its choices to allow percentage-based appearance.

                    // Normal Enemy Chance: 6 / 23
                    case 8:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case 9:
                         enemy = new NormalTypingEnemy(position, this);
                          break;

                    case 10:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case 11:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case 12:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    // Fast Enemy Chance: 3 / 20
                    case 13:
                         enemy = new FastTypingEnemy(position, this);
                         break;

                    case 14:
                         enemy = new FastTypingEnemy(position, this);
                         break;

                    // Suicide Enemy Chance: 3 / 20
                    case 15:
                         enemy = new SuicideTypingEnemy(position, this);
                         break;

                    case 16:
                         enemy = new SuicideTypingEnemy(position, this);
                         break;

                    // Explosive Enemy Chance: 3 / 20
                    case 17:
                         enemy = new ExplodingTypingEnemy(position, this);
                         break;

                    case 18:
                         enemy = new ExplodingTypingEnemy(position, this);
                         break;

                    // Deflating Enemy Chance: 2 / 20
                    case 19:
                         enemy = new DeflatingTypingEnemy(position, this);
                         break;


                    // Normal Enemy if we somehow get an un-specified number.
                    default:
                         enemy = new NormalTypingEnemy(position, this);
                         break;
               }

               return enemy;
          }

          #endregion


          #region Helper Remove Methods

          /// <summary>
          /// Removes an Enemy from the Enemy Manager.
          /// </summary>
          private void RemoveEnemy(Enemy enemy, bool removeFromMemory)
          {
               if (aliveEnemies != null)
               {
                    aliveEnemies.Remove(enemy);

                    if (removeFromMemory)
                    {
                         enemy.Unload();
                         enemy = null;
                    }
               }
          }

          /// <summary>
          /// Removes an Enemy from the Enemy Manager.
          /// </summary>
          private void RemoveDyingEnemy(Enemy enemy, bool removeFromMemory)
          {
               if (aliveEnemies != null)
               {
                    dyingEnemies.Remove(enemy);

                    if (removeFromMemory)
                    {
                         enemy.Unload();
                         enemy = null;
                    }
               }
          }


          /// <summary>
          /// Removes an Enemy from the Enemy Manager.
          /// </summary>
          private void RemoveEscapingEnemy(Enemy enemy, bool removeFromMemory)
          {
               if (escapingEnemies != null)
               {
                    escapingEnemies.Remove(enemy);

                    if (removeFromMemory)
                    {
                         enemy.Unload();
                         enemy = null;
                    }
               }
          }
          /// <summary>
          /// Removes all Enemies from the Enemy Manager.
          /// </summary>
          public void RemoveAllEnemies()
          {
               if (aliveEnemies != null)
                    aliveEnemies.Clear();

               if (dyingEnemies != null)
                    dyingEnemies.Clear();

               if (escapingEnemies != null)
                    escapingEnemies.Clear();
          }

          #endregion


          #region Disposal

          protected override void UnloadContent()
          {
               this.RemoveAllEnemies();
          }

          #endregion
     }
}