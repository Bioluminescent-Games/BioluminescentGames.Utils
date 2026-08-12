using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BioluminescentGames.Utils.Core
{
    [AutoStaticsCleanup]
    public abstract partial class GameInterface
    {
        public static GameInterface Instance { get; protected set; }

        public abstract IInputHandler GetInputHandler();
        public abstract IErrorHandler GetErrorHandler();
        public virtual Camera GetCurrentCamera() => Camera.main;
    }
}
