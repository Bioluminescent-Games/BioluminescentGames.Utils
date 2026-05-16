namespace BioluminescentGames.Utils.Core
{
    public abstract class GameInterface
    {
        public static GameInterface Instance { get; protected set; }

        public abstract IInputHandler GetInputHandler();
        public abstract IErrorHandler GetErrorHandler();
    }
}
