#region File Description
//-----------------------------------------------------------------------------
// BonusEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Text;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// A Bonus Enemy is one which crosses the arena horizontally at a fast speed.
     /// Since he cannot cross the line of defense, he is unable to hurt the Player.
     /// However, the Player can kill the enemy to obtain a drop, such as ammo or life.
     /// </summary>
     public class BonusEnemy : Enemy
     {
          #region Fields

          private bool IsDroppingItem = false;

          #endregion


          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public BonusEnemy(Vector3 position, Wave wave)
               : base(EngineCore.Game, wave)
          {
               this.Speed = (3.5f / 4.0f) * (1f + ((int)(ZombieGameSettings.Difficulty) * 0.5f));

               if (ZombieGameSettings.Difficulty == Difficulty.Hard)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               this.Speed *= MathHelper.Clamp((1.0f + (0.05f * Wave.WaveNumber)), 1.0f, 1.75f);

               this.Position = position;
               this.BasePoints = 50;
               this.BonusPoints = 50;
               this.ElapsedKillTime = 0.0f;

               this.Health = 2;
               this.startingHealth = this.Health;

               this.Color = Color.White;
               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               this.DamageDoneToPlayer = 0;

               this.Initialize();
          }

          /// <summary>
          /// Loads the appropriate content for the Normal Typing Enemy.
          /// In this case, we want a unique Sprite and Font style for the enemy.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               Avatar.PlayAnimation(AnimationType.ZombieWalk, true);
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion


          #region Update

          public override void Update(GameTime gameTime)
          {
               // Slow down animations based on if Slow-Motion is occuring.
               TimeSpan duration = gameTime.ElapsedGameTime;
               duration = TimeSpan.FromTicks((long)(duration.Ticks * Player.SlowMotionFactor));
               GameTime tweakedGameTime = new GameTime(gameTime.TotalGameTime, duration);
               

               this.Avatar.AmbientLightColor = this.Color.ToVector3();

               // If this Enemy is Dying...
               if (this.IsDying)
               {
                    this.Color = Color.White;

                    // Increment how long he's been in his Dying state!
                    elapsedDyingTime += (float)(gameTime.ElapsedGameTime.TotalSeconds * Player.SlowMotionFactor);

                    // And update his Avatar!
                    Avatar.Update(tweakedGameTime);

                    // Return because we don't want to update his position or increase his ElapsedTime.
                    return;
               }

               // Update the Enemy's Position (and thus Avatar Position).
               this.Position = new Vector3(this.Position.X - ((this.Speed / 25f) * Player.SlowMotionFactor), 0, this.Position.Z);
               this.Avatar.Position = this.Position;

               // Rotate the Enemy to face right.
               this.Avatar.Rotation = new Vector3(this.Avatar.Rotation.X, 90f, this.Avatar.Rotation.Z);

               this.WorldPosition = this.Avatar.WorldMatrix;


               Avatar.Update(tweakedGameTime);
          }

          #endregion


          #region Draw

          public override void Draw(GameTime gameTime)
          {
               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Draw the enemy (his / her avatar).
               Avatar.Draw(gameTime);

               // If the enemy is dying...
               if (IsDroppingItem)
               {
                    // Draw the Money Dropped text.
                    DrawItemDropped(gameTime);
               }

               // Draw the Health Meter only if enemy is not dying.
               if (!IsDying && !IsEscaping)
               {
                    if (!StartScreen.IsOnStartScreen)
                         DrawHealthMeter(gameTime);
               }

               MySpriteBatch.End();
          }

          #endregion


          #region Draw Bonus Dropped

          private string itemDroppedString = "";

          public void DrawItemDropped(GameTime gameTime)
          {
               Vector3 screenSpace = PixelEngine.Graphics.GraphicsHelper.ConvertToScreenspaceVector3(this.WorldPosition);

               if (this.Position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    TextManager.DrawCentered(false, TextManager.Fonts[(int)FontType.TitleFont].SpriteFont, itemDroppedString, new Vector2(screenSpace.X, screenSpace.Y), Color.LightGreen, 0.20f);
               }
          }

          #endregion


          #region Overridden IsCollision, OnEscaped and OnKilled Methods

          public override bool IsCollision(Vector3 playerPosition)
          {
               if (this.Position.X < -16f)
               {
                    return true;
               }

               return false;
          }

          public override void OnEscaped()
          {
               // Just handle it normally, for now...
               base.OnEscaped();
          }

          public override void OnKilled()
          {
               // Handle it normally.

               base.OnKilled();

               IsDroppingItem = true;

               // But also award a Bonus, such as extra life, ammo, etc.
               //this.Wave.CurrentPlayer.Gun.TotalAmmo += 10;

               int bonusItem = random.Next(4);

               switch (bonusItem)
               {
                    case 0:
                         this.Wave.CurrentPlayer.Health++;
                         itemDroppedString = "BONUS!\nHealth + 1!";
                         break;
                    case 1:
                         this.Wave.CurrentPlayer.NumberOfGrenades++;
                         itemDroppedString = "BONUS!\nGrenade + 1!";
                         break;
                    case 2:
                         this.Wave.CurrentPlayer.Money += 500;
                         itemDroppedString = "BONUS!\n+ $500!";
                         break;
                    case 3:
                         // NOTHING!
                         break;
               }


               // TO-DO: Announce the Item Drop to the Player.
          }

          #endregion
     }
}