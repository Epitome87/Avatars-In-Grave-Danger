#region File Description
//-----------------------------------------------------------------------------
// GamerInformationrmation.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Screen;
using System.Collections.Generic;
#endregion

namespace AvatarsInGraveDanger
{
     /// <summary>
     /// Defines a Wrapper Class that provides SignedInGamer functionality,
     /// such as PlayerIndex, Gamertag, GamerProfile, and GamerPicture.
     /// </summary>
     public class GamerInformation
     {
          #region Fields

          private SignedInGamer gamer;
          private PlayerIndex controllingPlayer;
          
          #endregion


          #region Properties

          /// <summary>
          /// Gets the SignedInGamer (which is what GamerInformationrmation is
          /// essentially a Wrapper Class for.
          /// </summary>
          public SignedInGamer Gamer
          {
               get { return gamer; }
               set { gamer = value; }
          }

          /// <summary>
          /// Gets the Gamer's PlayerIndex via SignedInGamer.PlayerIndex.
          /// </summary>
          public PlayerIndex PlayerIndex
          {
               get { return gamer.PlayerIndex; }
          }

          /// <summary>
          /// Gets the Gamer's Avatar Description via SignedInGamer.Avatar.
          /// </summary>
          public AvatarDescription AvatarDescription
          {
               get
               {
                    IAsyncResult result = AvatarDescription.BeginGetFromGamer(gamer, null, null);

                    return AvatarDescription.EndGetFromGamer(result);
               }
          }

          /// <summary>
          /// Gets the Gamer's GamerTag via SignedInGamer.Gamertag.
          /// </summary>
          public string GamerTag
          {
               get { return gamer.Gamertag; }
          }

          /// <summary>
          /// Gets the Gamer's Gamer Picture via GetProfile().GamerPicture.
          /// </summary>
          public Texture2D GamerPicture
          {
               get
               {
                    if (gamer != null)
                    {
                         // Read the gamer picture.
                         //Stream stream = gamer.GetProfile().GetGamerPicture();

                         //Texture2D picture = Texture2D.FromStream(ScreenManager.GraphicsDevice, stream);

                         return new Texture2D(ScreenManager.GraphicsDevice, 2, 2);//return picture;

                         // Use to be this in XNA 3.1: return gamer.GetProfile().GetGamerPicture()
                    }

                    else
                         return new Texture2D(ScreenManager.GraphicsDevice, 2, 2);
               } 
          }

          /// <summary>
          /// Gets the Gamer's Motto via GetProfile().Motto.
          /// </summary>
          public string Motto
          {
               get { return gamer.GetProfile().Motto; }
          }

          #endregion


          #region Initialization

          /// <summary>
          /// Initialize a GamerInformationrmation.
          /// </summary>
          /// <param name="playerIndex"></param>
          public GamerInformation(PlayerIndex playerIndex)
          {
               controllingPlayer = playerIndex;
               gamer = Microsoft.Xna.Framework.GamerServices.Gamer.SignedInGamers[controllingPlayer];
          }

          #endregion


          #region Can Buy Game Method

          /// <summary>
          /// Returns whether or not this Gamer can purchase the game.
          /// 
          /// Requirements: 
          /// Gamer is valid, Gamer is signed into Xbox Live, Gamer is not a Guest.
          /// 
          /// If all the above is true, we return the AllowPurchaseContent Gamer Privilege.
          /// </summary>
          /// <returns></returns>
          public bool CanBuyGame()
          {
               if (gamer == null)
                    return false;

               if (!gamer.IsSignedInToLive)
                    return false;

               if (gamer.IsGuest)
                    return false;

               return gamer.Privileges.AllowPurchaseContent;
          }

          #endregion




          public List<AvatarDescription> FriendsAvatars()
          {
              List<AvatarDescription> friendsAvatarDescriptions = new List<Microsoft.Xna.Framework.GamerServices.AvatarDescription>();

              Microsoft.Xna.Framework.GamerServices.FriendCollection friends;

              // Grab the Player's Friend List.
              friends = AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.GetFriends();

              // Iterate through all the Player's friends...
              foreach (Microsoft.Xna.Framework.GamerServices.FriendGamer friend in friends)
              {
                  IAsyncResult avatarResult = AvatarDescription.BeginGetFromGamer(Microsoft.Xna.Framework.GamerServices.Gamer.GetFromGamertag(friend.Gamertag), null, null);
                  AvatarDescription avatarDescription = AvatarDescription.EndGetFromGamer(avatarResult);

                  friendsAvatarDescriptions.Add(avatarDescription);
              }

              return friendsAvatarDescriptions;
          }

          AvatarDescription GetFriendAvatarDescription(string gamerTag)
          {
              AvatarDescription avatarDescription = AvatarDescription.CreateRandom();
              Gamer friendGamer;
              Microsoft.Xna.Framework.GamerServices.FriendCollection friends;
              // Grab the Player's Friend List.
              friends = AvatarZombieGame.CurrentPlayer.GamerInformation.Gamer.GetFriends();
              foreach (Microsoft.Xna.Framework.GamerServices.FriendGamer friend in friends)
              {
                  friendGamer = friend;

                  IAsyncResult avatarResult = AvatarDescription.BeginGetFromGamer(friendGamer, null, null);
                  //AvatarDescription.BeginGetFromGamer(Microsoft.Xna.Framework.GamerServices.Gamer.GetFromGamertag(gamerTag), null, null);

                  avatarDescription = AvatarDescription.EndGetFromGamer(avatarResult);
              }

              //IAsyncResult avatarResult = AvatarDescription.BeginGetFromGamer(Microsoft.Xna.Framework.GamerServices.Gamer.GetFromGamertag(gamerTag), null, null);

              //AvatarDescription avatarDescription = AvatarDescription.EndGetFromGamer(avatarResult);

              return avatarDescription;
          }
     }
}