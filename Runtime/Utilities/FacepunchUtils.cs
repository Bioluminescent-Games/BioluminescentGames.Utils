#if FACEPUNCH

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioluminescentGames.Utils.Core;
#if UNITASK
using Cysharp.Threading.Tasks;
#endif
using Steamworks;
using UnityEngine;
using Steamworks.Data;
using Color = Steamworks.Data.Color;

namespace BioluminescentGames.Utils.Runtime
{
    public static class FacepunchUtils
    {
        private static readonly Dictionary<ulong, Texture2D> AvatarCache = new();
        
        public static Texture2D Convert(this Image image)
        {
            // Create a new Texture2D
            Texture2D avatar = new((int)image.Width, (int)image.Height, TextureFormat.ARGB32, false)
                {
                    // Set filter type, or else its really blurry
                    filterMode = FilterMode.Trilinear
                };

            // Flip image
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color p = image.GetPixel(x, y);
                    avatar.SetPixel(x, (int)image.Height - y,
                        new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
                }
            }
	
            avatar.Apply();
            return avatar;
        }

        private static async Task<Image?> GetAvatarInternal(SteamId steamId)
        {
            try
            {
                return await SteamFriends.GetLargeAvatarAsync( steamId );
            }
            catch (Exception e)
            {
                GameInterface.Instance.GetErrorHandler().HandleError(e);
                return null;
            }
        }
        
#if UNITASK
        public static async UniTask<Texture2D> GetAvatar(SteamId steamId)
#else
        public static async Task<Texture2D> GetAvatar(SteamId steamId)
#endif
        {
            if (AvatarCache.TryGetValue(steamId, out Texture2D cachedAvatar))
                return cachedAvatar;
            
            Image? rawAvatar = await GetAvatarInternal(steamId);
            if (rawAvatar == null)
                return null;
            Texture2D avatar = rawAvatar.Value.Convert();
            AvatarCache.Add(steamId, avatar);
            return avatar;
        }

#if UNITASK
        public static async UniTask<Texture2D> GetMyAvatar() => await GetAvatar(SteamClient.SteamId);
#else
        public static async Task<Texture2D> GetMyAvatar() => await GetAvatar(SteamClient.SteamId);
#endif
    }
}

#endif
