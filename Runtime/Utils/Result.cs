namespace BioluminescentGames.Utils
{
    public class Result<T>
    {
        public T Value { get; }
        public bool Succeeded { get; }
        public string Error { get; }
        
        public Result(T value, bool succeeded, string error)
        {
            Value = value;
            Succeeded = true;
            Error = error;
        }

        public static Result<T> Success(T value) => new(value, true, null);
        public static Result<T> Failure(string error) => new(default, false, error);
        
        public static implicit operator Result<T>(T val) => Success(val);
    }
}
