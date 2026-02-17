using System;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public class Result<T>
    {
        /// <summary>
        /// The value of the <see cref="Result{T}"/>
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Did the request succeed?
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Create a result with the most verbose method
        /// </summary>
        /// <param name="value">The contents of the <see cref="Result{T}"/></param>
        /// <param name="succeeded">If it succeeded</param>
        /// <param name="error">The error message</param>
        public Result(T value, bool succeeded = true, string error = null)
        {
            Value = value;
            Succeeded = succeeded;
            Error = error;
        }

        /// <summary>
        /// Return a success result
        /// </summary>
        /// <param name="value">The contents of the <see cref="Result{T}"/></param>
        /// <returns>The <see cref="Result{T}"/></returns>
        public static Result<T> Success(T value) => new(value);

        /// <summary>
        /// Return a failure result
        /// </summary>
        /// <param name="error">The error message</param>
        /// <returns>A failure result</returns>
        public static Result<T> Failure(string error) => new(default, false, error);

        /// <summary>
        /// Returns a successful result if flag == true.
        /// </summary>
        /// <param name="flag">The flag that determines to return a success or a failure.</param>
        /// <param name="value">The value to return if the flag is true</param>
        /// <param name="error">The error message if the flag is false</param>
        /// <returns>A <see cref="Result{T}"/> based on <see cref="flag"/></returns>
        public static Result<T> FromFlag(bool flag, T value, string error) => flag ? Success(value) : Failure(error);

        /// <summary>
        /// Returns true if the request was a success. Otherwise prints the error and returns false.
        /// </summary>
        /// <returns>Was the request a success?</returns>
        public bool Check()
        {
            if (Succeeded)
                return true;

            Debug.LogError(Error);
            return false;
        }

        public T Unwrap()
        {
            return Succeeded ? Value : throw new NullReferenceException(Error);
        }

        /// <summary>
        /// Converts T -> <see cref="Result{T}"/> as a success.
        /// </summary>
        /// <param name="val">The value to convert to a successful <see cref="Result{T}"/></param>
        /// <returns>The <see cref="Result{T}"/> as a success.</returns>
        public static implicit operator Result<T>(T val) => Success(val);
    }
}
