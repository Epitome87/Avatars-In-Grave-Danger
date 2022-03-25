#region File Description
//-----------------------------------------------------------------------------
// SlowMotionDevice.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Audio;
#endregion

namespace AvatarsInGraveDanger
{
     /// <remarks>
     /// Defines a Slow Motion Device object, which inherits from DrawableGameComponent to
     /// allow per-frame Update and Draw calls.
     /// 
     /// A Slow Motion Device allows the Player to slow down time (or at least the Zombies).
     /// It has a Power Meter, Recharge Rate, and Duration.
     /// 
     /// The Player can Upgrade the effects of this device.
     /// </remarks>
     public class SlowMotionDevice : DrawableGameComponent
     {
          #region Private Fields

          private GameResourceTexture2D blankTexture;
          private Vector2 powerMeterPosition = new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);


          private string activateSlowMotionSound = "SlowMotion_Out";
          private string deactivateSlowMotionSound = "SlowMotion_In";

          #endregion


          #region Properties

          /// <summary>
          /// Gets or sets the Power left in the Slow Motion Device.
          /// Power is required to activate the device, and it decreases 
          /// over time as the device is in use.
          /// </summary>
          public float Power
          {
               get { return power; }
               set { power = value; }
          }
          private float power;

          /// <summary>
          /// The minimum amount of Power needed to activate the Slow Motion Device.
          /// This minimum ensures Slow Motion can not be repeatedly activated.
          /// </summary>
          public float MinimumPowerRequired
          {
               get { return minimumPowerRequired; }
               set { minimumPowerRequired = value; }
          }
          private float minimumPowerRequired;

          /// <summary>
          /// Gets or sets whether the Slow Motion device is currently activated.
          /// </summary>
          public bool IsInUse
          {
               get { return isInUse; }
               set { isInUse = value; }
          }
          private bool isInUse;

         /// <summary>
         /// Gets or sets whether the Slot Motion device is currently equipped.
         /// </summary>
          public bool IsEquipped
          {
              get { return isEquipped; }
              set { isEquipped = value; }
          }
          private bool isEquipped;

          /// <summary>
          /// Gets or sets the duration that Slow Motion lasts.
          /// This value is in seconds.
          /// </summary>
          public float SlowMotionDuration
          {
               get { return slowMotionDuration; }
               set { slowMotionDuration = value; }
          }
          private float slowMotionDuration;

          /// <summary>
          /// Gets or sets the rate at which the Slow Motion bar replenishes.
          /// This value is percentage-replenished per second.
          /// </summary>
          public float RechargeRate
          {
               get { return rechargeRate; }
               set { rechargeRate = value; }
          }
          private float rechargeRate;

          /// <summary>
          /// Gets or sets the rate at which the Slow Motion bar replenishes.
          /// This value is percentage-replenished per second.
          /// </summary>
          public float UsageRate
          {
               get { return usageRate; }
               set { usageRate = value; }
          }
          private float usageRate;

          #endregion


          #region Intialization

          public SlowMotionDevice(Game game)
               : base(game)
          {
               power = 100f;
               minimumPowerRequired = 25f;
               usageRate = 25f;
               rechargeRate = 5f;

               LoadContent();
          }

          /// <summary>
          /// Initializes the enemy manager component.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();
          }

          /// <summary>
          /// Loads a particular enemy sprite sheet and sounds.
          /// </summary>
          protected override void LoadContent()
          {
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank");
          }

          #endregion


          #region Handle Input

          /// <summary>
          /// Handles Player input for Gun-related behavior, such as Reloading, Firing, etc.
          /// </summary>
          public bool HandleInput(InputState input, GameTime gameTime, Player playerRef)
          {
               PlayerIndex playerIndex;

               if (input.IsNewButtonPress(Buttons.A, EngineCore.ControllingPlayer, out playerIndex))
               {
                    return Activate();
               }

               return false;
          }

          #endregion


          #region Update

          /// <summary>
          /// Updates the Position of the Bullet.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               if (isInUse)
               {
                    power -= (usageRate * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    Player.IsSlowMotion = true;

                    AudioManager.AudioEngine.SetGlobalVariable("Global Playlist Variable", 10f);
               }

               // Otherwise, the Slow Motion Device is not in use...
               else
               {
                    // So replenish its power based on the recharge rate.
                    power += (rechargeRate * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    Player.IsSlowMotion = false;

                    // This will be called every Update when Not In Use: Will this hinder performance?
                    AudioManager.AudioEngine.SetGlobalVariable("Global Playlist Variable", 0f);
               }

               // We clamp the Power to be between 0 (empty) and 100 (full).
               power = MathHelper.Clamp(power, 0f, 100f);


               if (power <= 0f)
               {
                    isInUse = false;

                    Player.IsSlowMotion = false;

                    AudioManager.PlayCue(deactivateSlowMotionSound);

                    AudioManager.AudioEngine.SetGlobalVariable("Global Playlist Variable", 0f);
               }
          }

          #endregion

          int powerMeterWidth = 250;
          int powerMeterHeight = 40;

          #region Draw

          /// <summary>
          /// Draws the Barrier model.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               if (isInUse)
               {
                    DrawHealthMeter(gameTime);
               }

               else
               {
                    DrawHealthMeterInactive(gameTime);
               }
          }

          private void DrawHealthMeter(GameTime gameTime)
          {
               Color healthColor = Color.Green;

               if (power <= (100f * 0.75f))
               {
                    healthColor = Color.Yellow;
               }

               if (power <= (100f * 0.50f))
               {
                    healthColor = Color.DarkOrange;
               }

               if (power <= (100f * 0.25f))
               {
                    healthColor = Color.Red;
               }


               powerMeterPosition = new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);

               Rectangle rect = new Rectangle((int)powerMeterPosition.X - (powerMeterWidth / 2), (int)powerMeterPosition.Y, powerMeterWidth, powerMeterHeight);
               Rectangle rect2 = new Rectangle((int)powerMeterPosition.X - (powerMeterWidth / 2), (int)powerMeterPosition.Y, (int)(power * (powerMeterWidth / 100f)), powerMeterHeight);

               GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, rect, 2, Color.White);
               MySpriteBatch.Draw(blankTexture.Texture2D, rect, Color.Black * 0.5f);
               MySpriteBatch.Draw(blankTexture.Texture2D, rect2, healthColor);
          }

          private void DrawHealthMeterInactive(GameTime gameTime)
          {
               Color healthColor = Color.Green;

               if (power <= (100f * 0.75f))
               {
                    healthColor = Color.Yellow;
               }

               if (power <= (100f * 0.50f))
               {
                    healthColor = Color.DarkOrange;
               }

               if (power <= (100f * 0.25f))
               {
                    healthColor = Color.Red;
               }


               powerMeterPosition = new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top);

               Rectangle rect = new Rectangle((int)powerMeterPosition.X - (powerMeterWidth / 4), (int)powerMeterPosition.Y, powerMeterWidth / 2, powerMeterHeight / 2);
               Rectangle rect2 = new Rectangle((int)powerMeterPosition.X - (powerMeterWidth / 4), (int)powerMeterPosition.Y, (int)(power * (powerMeterWidth / 100f) / 2), powerMeterHeight / 2);

               GraphicsHelper.DrawBorderFromRectangle(blankTexture.Texture2D, rect, 2, Color.White * 0.5f);
               MySpriteBatch.Draw(blankTexture.Texture2D, rect, Color.Black * 0.5f * 0.75f);
               MySpriteBatch.Draw(blankTexture.Texture2D, rect2, healthColor * 0.75f);
          }

          #endregion


          #region Private Activate Method

          public bool Activate()
          {
               // If the device is already in use...
               if (isInUse)
               {
                    // Power it off.
                    isInUse = false;

                    AudioManager.PlayCue(deactivateSlowMotionSound);
               }

               // Otherwise, it is not currently in use...
               else
               {
                    // Determine if we are able to use it.
                    // Requirement: Do we have the Minimum Power?
                    if (power >= minimumPowerRequired)
                    {
                         // We do; Power it on!
                         isInUse = true;

                         AudioManager.PlayCue(activateSlowMotionSound);
                    }
               }

               return isInUse;
          }

          #endregion
     }
}
