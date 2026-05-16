using System;

namespace BioluminescentGames.Utils.Core
{
    public interface IErrorHandler
    {
        void HandleError(string errorMessage);
        void HandleError(string errorMessage, string description);
        void HandleError(Exception exception);
    }
}