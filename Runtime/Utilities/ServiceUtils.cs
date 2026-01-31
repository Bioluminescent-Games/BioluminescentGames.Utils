#if ENABLE_SERVICES

using Unity.Services.Core;

namespace BioluminescentGames.Utils.Utilities
{
    public static class ServiceUtils
    {
        public enum FeedbackSeverity
        {
            Info,
            Warning,
            Error,
            Fatal,
            Unknown
        }

        public readonly struct Feedback
        {
            public readonly string Message;
            public readonly FeedbackSeverity Severity;

            public Feedback(string message, FeedbackSeverity severity)
            {
                Message = message;
                Severity = severity;
            }

            public override string ToString()
            {
                return $"Feedback with Message: \"{Message}\" and Severity: {Severity.ToString()}";
            }
        }

        public static Feedback GetUserFriendlyFeedbackFromErrorCode(RequestFailedException exception)
        {
            int errorCode = exception.ErrorCode;

            FeedbackSeverity severity = errorCode switch
            {
                CommonErrorCodes.TransportError => FeedbackSeverity.Error,
                CommonErrorCodes.Timeout => FeedbackSeverity.Warning,
                CommonErrorCodes.ServiceUnavailable => FeedbackSeverity.Error,
                CommonErrorCodes.ApiMissing => FeedbackSeverity.Error,
                CommonErrorCodes.RequestRejected => FeedbackSeverity.Error,
                CommonErrorCodes.TooManyRequests => FeedbackSeverity.Error,
                CommonErrorCodes.InvalidToken => FeedbackSeverity.Error,
                CommonErrorCodes.TokenExpired => FeedbackSeverity.Error,
                CommonErrorCodes.Forbidden => FeedbackSeverity.Error,
                CommonErrorCodes.NotFound => FeedbackSeverity.Error,
                CommonErrorCodes.InvalidRequest => FeedbackSeverity.Error,
                CommonErrorCodes.ProjectPolicyAccessDenied => FeedbackSeverity.Fatal,
                CommonErrorCodes.PlayerPolicyAccessDenied => FeedbackSeverity.Error,
                CommonErrorCodes.Conflict => FeedbackSeverity.Fatal,
                _ => FeedbackSeverity.Unknown
            };

            return new Feedback(exception.Message, severity);
        }
    }
}

#endif
