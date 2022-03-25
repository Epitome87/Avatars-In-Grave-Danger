#region File Description
//-----------------------------------------------------------------------------
// NormalTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Text;
using PixelEngine.Avatars;
using Microsoft.Xna.Framework.GamerServices;
using System.Collections.Generic;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Normal Typing Enemy is one with average Health size and average speed.
     /// </summary>
     public class DefendedBoss : Enemy
     {
          #region Fields

          List<Enemy> spawnedEnemies = new List<Enemy>();

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public DefendedBoss(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               this.Speed = 0f;

               this.Position = new Vector3(0f, 0f, 25f);// position;
               this.BasePoints = 50;
               this.BonusPoints = 50;
               this.ElapsedKillTime = 0.0f;

               this.Health = 6;

               this.startingHealth = this.Health;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.DamageDoneToPlayer = 1;

               this.Avatar.Scale *= 5f;

               this.Initialize();
          }

          /// <summary>
          /// Loads the appropriate content for the Normal Typing Enemy.
          /// In this case, we want a unique Sprite and Font style for the enemy.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               Avatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, true);

               for (int i = 0; i < 5; i++)
               {
                    SlingerEnemy enemy = new SlingerEnemy(new Vector3(random.Next(10), 0f, 20f), this.Wave);

                    enemy.Speed = 0f;

                    spawnedEnemies.Add(enemy);
               }
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the Normal Typing Enemy.
          /// 
          /// Simply calls upon base.Update.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               base.Update(gameTime);

               foreach (Enemy enemy in spawnedEnemies)
               {
                    enemy.Update(gameTime);
               }
          }

          #endregion


          #region Draw

          /// <summary>
          /// Draws the Normal Typing Enemy.
          /// 
          /// Simply calls base.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);

               foreach (Enemy enemy in spawnedEnemies)
               {
                    enemy.Draw(gameTime);
               }
          }

          #endregion


          public override void OnHit(Player playerWhoHit)
          {
               base.OnHit(playerWhoHit);

               this.Avatar.Scale = this.Avatar.Scale - 0.1f;
          }
     }
}