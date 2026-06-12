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
        private static readonly List<ulong> AvatarsUnderway = new();
        
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
                    Color pixelCol = image.GetPixel(x, y);
                    avatar.SetPixel(x, (int)image.Height - y,
                        new UnityEngine.Color(pixelCol.r / 255.0f, pixelCol.g / 255.0f, pixelCol.b / 255.0f, pixelCol.a / 255.0f));
                }
            }
	
            avatar.Apply();
            return avatar;
        }

        #if UNITASK
        private static async UniTask<Image?> GetAvatarInternal(SteamId steamId)
        #else
        private static async Task<Image?> GetAvatarInternal(SteamId steamId)
        #endif
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
            await UniTask.WaitWhile(() => AvatarsUnderway.Contains(steamId.Value));
            
            if (AvatarCache.TryGetValue(steamId, out Texture2D cachedAvatar))
                return cachedAvatar;
            
            AvatarsUnderway.Add(steamId.Value);
            
            Image? rawAvatar = await GetAvatarInternal(steamId);
            if (rawAvatar == null)
                return null;
            Texture2D avatar = rawAvatar.Value.Convert();
            AvatarCache.Add(steamId, avatar);
            AvatarsUnderway.Remove(steamId.Value);
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
