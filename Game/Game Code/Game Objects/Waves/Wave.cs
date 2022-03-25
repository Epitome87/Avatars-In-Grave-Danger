#region File Description
//-----------------------------------------------------------------------------
// Wave.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PixelEngine.Graphics;
#endregion

namespace AvatarsInGraveDanger
{
    /// <remarks>
    /// A class that serves as a "Wave" in a defense-type game.
    /// 
    /// A Wave consists of a number of enemies which it handles and
    /// spawns more of when necessary.
    /// 
    /// For this reason, a Wave is a sort of "Enemy Management" tool.
    /// 
    /// The Waves handles the drawing and updating of all Enemies within it.
    /// It Spawns Waves and adds / removes Enemies when necessary. It also ensures 
    /// Enemies have, for the most part, unique spawn locations.
    /// </remarks>
    public class Wave : DrawableGameComponent
    {
        #region SpawnLocation Enum

        public enum SpawnLocation
        {
            Empty,

            // From Left-To-Right
            Left4,
            Left3,
            Left2,
            Left1,

            Right1,
            Right2,
            Right3,
            Right4
        }

        #endregion

        // New 12-05-2011
        private WaveManager waveManager;

        public WaveManager WaveManager
        {
            get { return waveManager; }
            set { waveManager = value; }
        }

        #region Fields

        /// <summary>
        /// Number of Enemies this Wave contains.
        /// </summary>
        private int numberOfEnemies;

        /// <summary>
        /// The Wave Number of this Wave.
        /// </summary>
        private int waveNumber;

        /// <summary>
        /// A timing device to handle spawning enemies periodically.
        /// </summary>
        private float spawnTimer = 0f;

        /// <summary>
        /// The number of Enemies spawned so far during this Wave.
        /// </summary>
        private int enemiesSpawned = 0;

        /// <summary>
        /// Whether or not an Enemy has reached the path.
        /// </summary>
        private bool isEnemyAtEnd;

        /// <summary>
        /// Whether or not the Wave is currently spawning Enemies.
        /// 
        /// This is false before a Wave has started.
        /// </summary>
        private bool isSpawningEnemies;

        /// <summary>
        /// A reference to our Level.
        /// </summary>
        private Level level;

        /// <summary>
        /// A list of the Enemies found during this Wave.
        /// 
        /// * CURRENTLY NOT USED / NEEDED
        /// </summary>
        private List<Enemy> enemies = new List<Enemy>();

        /// <summary>
        /// A List of all the Alive Enemies in this Wave.
        /// </summary>
        private List<Enemy> aliveEnemies = new List<Enemy>();

        /// <summary>
        /// A List of all the Dying Enemies in this Wave.
        /// </summary>
        private List<Enemy> dyingEnemies = new List<Enemy>();

        /// <summary>
        /// A List of all the Escaping Enemies in this Wave.
        /// </summary>
        private List<Enemy> escapingEnemies = new List<Enemy>();



        // Meta-Data for this Wave. (Mine)

        /// <summary>
        /// The number of Enemies killed this Wave.
        /// </summary>
        private int enemiesKilled;

        /// <summary>
        /// The number of Enemies that Escaped this Wave.
        /// </summary>
        private int enemiesEscaped;

        /// <summary>
        /// The Score earned during this Wave.
        /// </summary>
        private int score;



        /// <summary>
        /// Returns whether or not this Wave is over.
        /// 
        /// Wave is over if there are no enemies left and
        /// we have already spawned all them.
        /// </summary>
        public bool WaveOver
        {
            get
            {
                return aliveEnemies.Count == 0
                     && enemiesSpawned == numberOfEnemies;
            }

        }

        /// <summary>
        /// Gets the Wave Number.
        /// </summary>
        public int WaveNumber
        {
            get { return waveNumber; }
        }

        /// <summary>
        /// NOT USED
        /// Whether or not an Enemy has reached
        /// the end of the path.
        /// </summary>
        public bool IsEnemyAtEnd
        {
            get { return isEnemyAtEnd; }
            set { isEnemyAtEnd = value; }
        }

        /// <summary>
        /// Gets the List of Enemies in this Wave.
        /// </summary>
        public List<Enemy> Enemies
        {
            get { return enemies; }
        }






        /// <summary>
        /// Gets the number of Enemies remaining.
        /// 
        /// This is the number of enemies this wave, minus the amount killed,
        /// minus the amount that have escaped.
        /// </summary>
        public int EnemiesRemaining
        {
            get
            {
                return numberOfEnemies - this.enemiesKilled - this.enemiesEscaped;
            }
        }





        private Random random = new Random();

        /// <summary>
        /// The amount of time that has elapsed 
        /// since this Wave started.
        /// </summary>
        private float elapsedWaveTime = 0.0f;

        public float ElaspedWaveTime
        {
            get { return elapsedWaveTime; }
        }

        /// <summary>
        /// A List of all the Spawn Locations being used currently in this Wave.
        /// </summary>
        private List<SpawnLocation> usedSpawnLocations = new List<SpawnLocation>();


        // DO NOT EVER USE THIS, EVEN IN THIS CLASS.
        private Player currentPlayer = AvatarZombieGame.CurrentPlayer;

        /// <summary>
        /// A reference to our Player.
        /// </summary>
        public Player CurrentPlayer
        {
            get { return this.WaveManager.CurrentPlayer; }// currentPlayer; }
            set { currentPlayer = value; }
        }





        public List<SpawnLocation> UsedSpawns
        {
            get { return usedSpawnLocations; }
        }







        public const float MIN_SPAWN_Z = 30;
        public const float MAX_SPAWN_Z = 5;

        #endregion


        #region Properties

        /// <summary>
        /// The Speed Factor each set of 10 waves is based off of.
        /// 
        /// On Wave 1, this may be 1.
        /// On Wave 11, this may be 1.25
        /// On Wave 21, this may be 1.5, etc.
        /// 
        /// Each Wave, 5% will be added onto this value to get the total speed factor.
        /// 
        /// Essentially, this means Wave 15 has the same speed as Wave 10, etc.
        /// </summary>
        public float BaseSpeedFactor
        {
            get
            {
                return 1f + ((WaveNumber / 10) * (0.25f));
            }
        }

        /// <summary>
        /// The Speed Factor for this Wave.
        /// 
        /// This is, ultimately, the Speed Factor we use for determining an Enemy's Speed.
        /// This number takes into account the Base Speed (What the speed is at Waves 10, 20, etc)
        /// and adds how much we add per wave. In this case, +5% each wave.
        /// </summary>
        public float WaveSpeedFactor
        {
            get
            {
                return BaseSpeedFactor + 0.05f * (WaveNumber % 10);
            }
        }

        /// <summary>
        /// Amount of Enemies killed during this Wave.
        /// </summary>
        public int EnemiesKilled
        {
            get { return enemiesKilled; }
            set { enemiesKilled = value; }
        }

        /// <summary>
        /// Amount of Enemies who escaped during this Wave.
        /// </summary>
        public int EnemiesEscaped
        {
            get { return enemiesEscaped; }
            set { enemiesEscaped = value; }
        }


        /// <summary>
        /// Score obtained during this Wave.
        /// </summary>
        public int Score
        {
            get { return score; }
            set { score = value; }
        }


        public List<float> GetWaveData()
        {
            List<float> waveData = new List<float>();

            waveData.Add(WaveNumber);
            waveData.Add(Score);
            waveData.Add(EnemiesKilled);
            waveData.Add(EnemiesEscaped);

            return waveData;
        }


        public int ShotsFiredThisWave
        {
            get { return shotsFiredThisWave; }
            set { shotsFiredThisWave = value; }
        }

        public int ShotsFired = 0;

        int shotsFiredThisWave = 0;


        /// <summary>
        /// Gets or sets the number of "Hits" the Player
        /// achieved this Wave.
        /// 
        /// A Hit is made when a Player damages the Enemy, essentially.
        /// </summary>
        public int NumberOfHits
        {
            get { return numberOfHits; }
            set { numberOfHits = value; }
        }

        private int numberOfHits = 0;






        public int NumberOfCloseCalls
        {
            get { return numberOfCloseCalls; }
            set { numberOfCloseCalls = value; }
        }
        private int numberOfCloseCalls = 0;

        /// <summary>
        /// Amount of Enemies in the Enemy Manager.
        /// </summary>
        public int Size
        {
            get { return aliveEnemies.Count; }
        }


        /// <summary>
        /// Gets or sets how many Enemies are on-screen at any given time.
        /// </summary>
        public int NumberOfSimultaneousEnemies
        {
            get { return numberOfSimultaneousEnemies; }
            set { numberOfSimultaneousEnemies = value; }
        }

        int numberOfSimultaneousEnemies = int.MaxValue;

        #endregion


        #region Initialization

        public Wave(WaveManager owner, int waveNumber, int numberOfEnemies, Level level)
            : base(PixelEngine.EngineCore.Game)
        {
            this.WaveManager = owner;

            this.waveNumber = waveNumber;
            this.numberOfEnemies = numberOfEnemies;

            this.level = level;
        }

        #endregion


        #region Update

        /// <summary>
        /// Allows each Enemy to run update logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            elapsedWaveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.CurrentPlayer != null)
            {
                //CheckBarrierCollisions(gameTime);
                CheckGrenadeCollisions(gameTime);
                CheckAimerCollisions(gameTime);
                CheckBulletCollisions(gameTime);
            }

            #region Check if we should spawn enemy

            // If we've already spawned enough enemies...
            if (enemiesSpawned >= numberOfEnemies)
            {
                // Let the Wave know we are no longer spawning them.
                isSpawningEnemies = false;
            }

            // Otherwise, we are still spawning enemies.
            if (isSpawningEnemies)
            {
                spawnTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);

                // Each wave, we wait 0.20 seconds less before spawning an enemy.
                // New: But we reset this every 10 Waves.
                float spawnPeriod = 3.0f - ((WaveNumber % 10) * 0.20f);

                // Give the spawn period a minimum and maximum.
                spawnPeriod = MathHelper.Clamp(spawnPeriod, 0.5f, 4.0f);

                if (spawnTimer > spawnPeriod && aliveEnemies.Count < 15)
                {
                    // Time to Spawn a new enemy!
                    SpawnEnemy();
                }
            }

            #endregion

            #region Update Alive, Dying, & Escaping Enemies

            // Update all the alive enemies.
            foreach (Enemy enemy in aliveEnemies)
            {
                enemy.Update(gameTime);
            }

            // Update all the dying enemies.
            foreach (Enemy enemy in dyingEnemies)
            {
                enemy.Update(gameTime);
            }

            // Update all the escaping enemies.
            foreach (Enemy enemy in escapingEnemies)
            {
                enemy.Update(gameTime);
            }

            #endregion

            #region Check for state changes in Alive, Dying, & Escaping Enemies

            // Check for dying enemies which have become dead.
            foreach (Enemy enemy in dyingEnemies)
            {
                if (enemy.ElapsedDyingTime >= 2.5f)
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
                if (enemy.Position.Z < -6 || enemy.Position.X < -18)
                {
                    if (escapingEnemies != null)
                    {
                        this.RemoveEscapingEnemy(enemy, true);
                        break;
                    }
                }
            }

            if (this.CurrentPlayer != null)
            {
                // Check for newly-dead enemies.
                foreach (Enemy enemy in aliveEnemies)
                {
                    if (enemy.IsDead)
                    {
                        Score += enemy.BasePoints;

                        // Award the Player some money.
                        this.CurrentPlayer.Money += (int)(enemy.MoneyDropped * this.CurrentPlayer.CashPerk);// (int)(100 * this.CurrentPlayer.CashPerk);

                        // Increment the number of enemies killed this wave.
                        EnemiesKilled++;

                        WaveManager.EnemiesKilled++;

                        // This enemy's dead: Add him to the dying enemy list.
                        dyingEnemies.Add(enemy);

                        // Remove this enemy from his previous list - but not from memory!
                        this.RemoveEnemy(enemy, false);

                        break;
                    }
                }
            }

            // Only check for escaping enemies if there are enemies present.
            if (aliveEnemies.Count > 0)
            {
                Enemy escapedEnemy = null;

                foreach (Enemy enemy in aliveEnemies)
                {
                    if (this.CurrentPlayer == null)
                    {
                        if (enemy.IsCollision(new Vector3(0, 0, 0)))
                        {
                            enemy.OnCollide(this.CurrentPlayer);

                            enemy.OnEscaped();

                            EnemiesEscaped++;

                            this.WaveManager.EnemiesEscaped++;

                            escapedEnemy = enemy;

                            this.RemoveEnemy(enemy, true);

                            break;
                        }
                    }

                    else if (enemy.IsCollision(this.CurrentPlayer.Position))
                    {
                        enemy.OnCollide(this.CurrentPlayer);

                        enemy.OnEscaped();

                        EnemiesEscaped++;
                        this.WaveManager.EnemiesEscaped++;

                        escapedEnemy = enemy;

                        // How about we don't remove the Enemy until he's off-screen?!
                        //this.RemoveEnemy(enemy, true);

                        enemy.IsEscaping = true;
                        escapingEnemies.Add(enemy);
                        this.RemoveEnemy(enemy, false);

                        break;
                    }
                }
            }

            #endregion
        }



        #endregion


        #region Check For Collisions With Barriers - Awful Spot!!!

        private void CheckBarrierCollisions(GameTime gameTime)
        {
            foreach (Barrier barrier in this.CurrentPlayer.playersBarriers)
            {
                BoundingSphere barrierBoundingSphere = new BoundingSphere(new Vector3(barrier.position.X, barrier.position.Y, barrier.position.Z), 3f);

                foreach (Enemy enemy in aliveEnemies)
                {
                    BoundingSphere enemyBoundingSphere = enemy.GetBoundingSphereForEntireBody();

                    ContainmentType collisionType = enemyBoundingSphere.Contains(barrierBoundingSphere);

                    if (collisionType != ContainmentType.Disjoint)
                    {
                        enemy.OnKilled();
                    }
                }
            }
        }

        #endregion


        #region Check For Collisions With Grenades - Awful Spot!!!

        private void CheckGrenadeCollisions(GameTime gameTime)
        {
            int grenadeKillCount = 0;

            if (this.CurrentPlayer.GrenadeList != null)
            {
                foreach (Grenade nade in this.CurrentPlayer.GrenadeList)
                {
                    if (nade.ReadyToKill && !nade.Detonated)
                    {
                        nade.Detonated = true;

                        foreach (Enemy enemy in this.aliveEnemies)
                        {
                            enemy.OnKilled();

                            grenadeKillCount++;
                        }

                        AvatarZombieGame.AwardData.GrenadeSimultaneousKillCount = (uint)grenadeKillCount;
                    }
                }
            }
        }

        #endregion


        #region Check Bullet Collisions - Awful Spot!!!

        private void CheckAimerCollisions(GameTime gameTime)
        {
            // We figure out the additional height to aim our Bullet based on our angle of aiming.
            float targetHeight = (float)Math.Tan(MathHelper.ToRadians(this.CurrentPlayer.Gun.AngleOfAimer)) * 40.0f;

            // The starting position of our Bullet.
            // This is just the tip of our Gun, estimated.
            //Vector3 startingPosition = new Vector3(this.CurrentPlayer.Position.X - 0.30f, this.CurrentPlayer.Position.Y + 1.80f, this.CurrentPlayer.Position.Z + 0.5f);

            Vector3 startingPosition = this.CurrentPlayer.Gun.GunTipPositionOffset;


            // The destination of our Bullet.
            // X: Can't aim left/right so this is simply the starting X.
            // Y: You can aim up/down 30 degrees. We find the Y value and add it to the starting Y (how high off ground Avatar's Gun is.)
            // Z: We simplify this and set it to 40 -- the maximum distance a target can possibly be.
            Vector3 destination = new Vector3(startingPosition.X, startingPosition.Y + targetHeight, startingPosition.Z + 80.0f);


            // We create a Ray from the starting position, in the direction of the destination.
            // If an Enemy intersects this ray, he is in danger!
            Vector3 normalizedDirection = destination - startingPosition;
            normalizedDirection.Normalize();

            Ray aimerRay = new Ray(startingPosition, normalizedDirection);


            foreach (Enemy enemy in aliveEnemies)
            {
                BoundingSphere enemyBoundingSphere = enemy.GetBoundingSphereForEntireBody();

                // New: Add the bullet radius. This will make sighting the enemies
                // use the same calculation as bullets hitting the enemies.
                // This means no more being able to hit an enemy despite the laser saying we arent aiming on him.
                enemyBoundingSphere.Radius += 0.005f;

                float? collisionDepth = 0f;

                aimerRay.Intersects(ref enemyBoundingSphere, out collisionDepth);

                if (collisionDepth.HasValue)
                {
                    this.CurrentPlayer.Gun.laserSightDepth = collisionDepth.Value;
                    enemy.Color = Color.DarkOrange;
                    break;
                }

                else
                {
                    this.CurrentPlayer.Gun.laserSightDepth = 40f;
                    enemy.Color = Color.White;
                }
            }
        }

        private void CheckBulletCollisions(GameTime gameTime)
        {
            // For typical wave defense gameplay.
            foreach (Enemy enemy in aliveEnemies)
            {
                BoundingSphere enemyBoundingSphere = enemy.GetBoundingSphereForEntireBody();

                foreach (Gun gun in this.CurrentPlayer.PlayersGuns)
                {
                    foreach (Bullet bullet in gun.BulletList)
                    //foreach (Bullet bullet in this.CurrentPlayer.Gun.BulletList)
                    {
                        BoundingSphere bulletBoundingSphere = new BoundingSphere();
                        bulletBoundingSphere.Center = bullet.Position;
                        bulletBoundingSphere.Radius = 0.01f;

                        enemyBoundingSphere.Center =
                             new Vector3(enemy.Avatar.Position.X, bullet.Position.Y, enemy.Avatar.Position.Z);

                        ContainmentType collisionType = enemyBoundingSphere.Contains(bulletBoundingSphere);

                        if (collisionType != ContainmentType.Disjoint)
                        {
                            // Collision.

                            // If this was an Explosive Bullet...
                            if (bullet.bulletType == Bullet.BulletType.Explosive)
                            {
                                // Burn this enemy to a crisp!
                                enemy.Avatar.LightColor = new Vector3(0, 0, 0);
                                enemy.Avatar.LightDirection = new Vector3(0, 0, 0);
                                enemy.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);

                                // Create an explosion.
                                Vector2 explosionPosition = GraphicsHelper.ConvertToScreenspaceVector2(Matrix.CreateTranslation(bullet.Position));
                                bullet.AddExplosion(explosionPosition, 8, 45.0f * 10.0f, 2500.0f, gameTime);

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

                                enemy.OnHit(this.CurrentPlayer);

                                bullet.RemoveBullet = true;

                                break;
                            }

                            // Otherwise, non-explosive Bullet hit the Enemy.
                            else
                            {
                                enemy.OnHit(this.CurrentPlayer);

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
        }

        #endregion


        #region Draw

        /// <summary>
        /// Tells each Enemy to draw itself. Uses the default SpriteBatch and Font found in ScreenManager.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Draw the Dead Enemies first!
            foreach (Enemy deadEnemy in dyingEnemies)
            {
                deadEnemy.Draw(gameTime);
            }

            // Draw the Alive Enemies next - just their Avatars!
            for (int i = aliveEnemies.Count - 1; i >= 0; i--)
            {
                if (i >= aliveEnemies.Count)
                    break;

                aliveEnemies[i].Draw(gameTime);
            }


            // Draw the Escaping Enemies last!
            foreach (Enemy escapingEnemy in escapingEnemies)
            {
                escapingEnemy.Draw(gameTime);
            }
        }

        #endregion


        /// <summary>
        /// The percentage change we have of spawning a Bonus Enemy.
        /// </summary>
        private int bonusEnemyChance = 4;

        #region Spawn An Enemy

        /// <summary>
        /// Spawns an Enemy.
        /// </summary>
        private void SpawnEnemy()
        {
            // Creates, spawns, and positions a new Enemy.
            // Also calls AddEnemy(enemy), so the enemy is added to the list of Alive Enemies.
            int enemyTypeToSpawn = 0;

            int spawnBonusEnemy = random.Next(0, 100 / bonusEnemyChance);

            if (spawnBonusEnemy == 1)
            {
                enemyTypeToSpawn = 5;
            }

            else
            {
                // Produce a random number between 0 and 100.
                int randomNumber = random.Next(0, 100);



                switch (this.WaveNumber)
                {
                    // Rounds 1 and 2: Slingers.
                    case 0:
                        enemyTypeToSpawn = 1;
                        break;
                    case 1:
                        enemyTypeToSpawn = 1;
                        break;


                    // Rounds 3 and 4: Slingers and now Beserkers.
                    case 2:
                        if (randomNumber < 30)
                            enemyTypeToSpawn = 2;

                        else
                            enemyTypeToSpawn = 1;

                        break;
                    case 3:
                        if (randomNumber < 30)
                            enemyTypeToSpawn = 2;

                        else
                            enemyTypeToSpawn = 1;

                        break;


                    // Rounds 5 and 6: Slingers, Beserkers, and now Warpers.
                    case 4:
                        enemyTypeToSpawn = random.Next(1, 4);
                        break;
                    case 5:
                        enemyTypeToSpawn = random.Next(1, 4);
                        break;


                    // Rounds 9 and 10: Slingers, Beserkers, Warpers, and now Giants.
                    case 8:
                        enemyTypeToSpawn = random.Next(1, 5);
                        break;
                    case 9:
                        enemyTypeToSpawn = random.Next(1, 5);
                        break;


                    // By default, spawn all 4 Zombie types.
                    default:
                        enemyTypeToSpawn = random.Next(1, 5);
                        break;
                }
            }

            SpawnAndPositionEnemy((EnemyType)enemyTypeToSpawn);

            spawnTimer = 0f;
            enemiesSpawned++;
        }

        #endregion


        #region Start Wave

        /// <summary>
        /// Starts the Wave.
        /// 
        /// Simply sets the IsSpawningEnemies flag to True.
        /// </summary>
        public void Start()
        {
            // This is new as of 11-19-2011.
            if (usedSpawnLocations != null)
            {
                usedSpawnLocations.Clear();
            }


            isSpawningEnemies = true;

            random = new Random();

            // Spawn only a few enemies at a time.
            // But reset every 10 waves.
            for (int i = 0; i < (1 * ((WaveNumber % 10) + 1)); i++)
            {
                SpawnEnemy();
            }
        }

        #endregion


        #region Public Get Enemy Methods

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


        #region Helper Add Enemy Method

        /// <summary>
        /// Adds a new Enemy to the Enemy Manager.
        /// </summary>
        public void AddEnemy(Enemy enemy)
        {
            enemy.Wave = this;
            aliveEnemies.Add(enemy);
        }

        #endregion


        #region Helper Spawn & Generate Enemy Method

        /// <summary>
        /// Generates an Enemy of a certain type.
        /// </summary>
        private void SpawnAndPositionEnemy(EnemyType typeOfEnemy)
        {
            float xSpawnPosition = 0f;
            Wave.SpawnLocation spawnLocation = SpawnLocation.Empty;

            if (typeOfEnemy == EnemyType.Bonus)
            {
                xSpawnPosition = 18f;
            }

            else
            {
                //Wave.SpawnLocation spawnLocation = FindRandomEmptySpawnPoint();
                spawnLocation = FindRandomEmptySpawnPoint();

                xSpawnPosition = PlaceEnemyAtSpawnLocation(spawnLocation);
            }

            float zSpawnPosition = Wave.MIN_SPAWN_Z + (float)random.NextDouble() * Wave.MAX_SPAWN_Z;            // Between 30-35

            // New location for these:
            Enemy newEnemy = GenerateEnemy(typeOfEnemy, new Vector3(xSpawnPosition, 0f, zSpawnPosition));

            // WHY WERE WE FINDING A SPAWN POINT AGAIN?!
            newEnemy.SpawnLocation = spawnLocation;// FindRandomEmptySpawnPoint();


            newEnemy.WorldPosition = Matrix.CreateTranslation(newEnemy.Position) *
                                     Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) *
                                     Matrix.CreateScale(1.0f);

            AddEnemy(newEnemy);
        }

        /// <summary>
        /// Generates an Enemy of the specified type.
        /// 
        /// Simply uses a switch-case to instanciate the correct TypingEnemy type.
        /// </summary>
        /// <param name="typeOfEnemy">The type of Enemy to generate.</param>
        /// <returns>The generated Enemy with default properties.</returns>
        public Enemy GenerateEnemy(EnemyType typeOfEnemy, Vector3 spawnPosition)
        {
            Enemy enemy;

            switch ((int)typeOfEnemy)
            {
                case (int)EnemyType.Normal:
                    enemy = new NormalEnemy(spawnPosition, this);
                    break;

                case (int)EnemyType.Slinger:
                    enemy = new SlingerEnemy(spawnPosition, this);
                    break;

                case (int)EnemyType.Beserker:
                    enemy = new BeserkerEnemy(spawnPosition, this);
                    break;

                case (int)EnemyType.Warper:
                    enemy = new WarperEnemy(spawnPosition, this);
                    break;

                case (int)EnemyType.Horde:
                    enemy = new InflatingEnemy(spawnPosition, this);
                    break;

                case (int)EnemyType.Bonus:
                    enemy = new BonusEnemy(new Vector3(16f, 0f, 25f), this);
                    break;

                // Normal Enemy if we somehow get an un-specified number.
                default:
                    enemy = new NormalEnemy(spawnPosition, this);
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


        #region Helper Spawn Location Methods

        /*
          /// <summary>
          /// Returns the first available empty Spawn Point.
          /// </summary>
          /// <returns></returns>
          private AvatarsInGraveDanger.Wave.SpawnLocation FindFirstEmptySpawnPoint()
          {
               Wave.SpawnLocation enemySpawn = Wave.SpawnLocation.Empty;

               // Iterate through all SpawnLocation possibilities...
               for (int i = 1; i < 8; i++)
               {
                    // If we are not using this SpawnLocation, then...
                    if (!usedSpawnLocations.Contains((Wave.SpawnLocation)(i)))
                    {
                         // Use it! Add it to the list of used spawn locations.
                         usedSpawnLocations.Add((Wave.SpawnLocation)(i));

                         // The enemy's spawn will be this spawn.
                         enemySpawn = (AvatarsInGraveDanger.Wave.SpawnLocation)i;

                         // We have found a spawn, so break from the for-loop.
                         break;
                    }
               }

               return enemySpawn;
          }
          */

        /// <summary>
        /// Returns the first available empty Spawn Point.
        /// </summary>
        /// <returns></returns>
        private AvatarsInGraveDanger.Wave.SpawnLocation FindRandomEmptySpawnPoint()
        {
            Wave.SpawnLocation enemySpawn = Wave.SpawnLocation.Empty;

            // If not all Spawn Locations are currently in use...
            if (usedSpawnLocations.Count < 8)
            {
                // Pick a random one, between 0 + 1 and 7 + 1.
                int randomSpawn = random.Next(8) + 1;

                bool keepLookingForSpawn = true;

                while (keepLookingForSpawn)
                {
                    // If we are not using this SpawnLocation, then...
                    if (!usedSpawnLocations.Contains((Wave.SpawnLocation)(randomSpawn)))
                    {
                        // Use it! Add it to the list of used spawn locations.
                        usedSpawnLocations.Add((Wave.SpawnLocation)(randomSpawn));

                        // The enemy's spawn will be this spawn.
                        enemySpawn = (Wave.SpawnLocation)randomSpawn;

                        keepLookingForSpawn = false;

                        // We have found a spawn, so break from the while-loop.
                        return enemySpawn;
                    }

                    randomSpawn = random.Next(8) + 1;
                }
            }

            return enemySpawn;
        }

        private float PlaceEnemyAtSpawnLocation(Wave.SpawnLocation spawnLocation)
        {
            float xPosition = 0f;

            switch (spawnLocation)
            {
                case Wave.SpawnLocation.Left4:
                    xPosition = -15.0f;
                    break;

                case Wave.SpawnLocation.Left3:
                    xPosition = -12.0f;
                    break;

                case Wave.SpawnLocation.Left2:
                    xPosition = -9.0f;
                    break;

                case Wave.SpawnLocation.Left1:
                    xPosition = -3.0f;
                    break;

                case Wave.SpawnLocation.Right1:
                    xPosition = 3.0f;
                    break;

                case Wave.SpawnLocation.Right2:
                    xPosition = 9.0f;
                    break;

                case Wave.SpawnLocation.Right3:
                    xPosition = 12.0f;
                    break;

                case Wave.SpawnLocation.Right4:
                    xPosition = 15.0f;
                    break;


                default:
                    // Between -15 and -1.
                    xPosition = -15f + (float)random.NextDouble() * (15f);

                    int LeftOrRighSide = random.Next(2);

                    if (LeftOrRighSide == 0)
                    {
                        xPosition = Math.Abs(xPosition);
                    }
                    break;
            }

            return xPosition;
        }

        #endregion
    }
}