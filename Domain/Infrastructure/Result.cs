using System.Collections.Generic;

namespace MyRental
{
    public record Result<T>
    {
        public bool IsSuccess { get; init; }

        public T Value;
        public List<string> Errors { get; init; } = new List<string>();

        private Result() { }

        public static Result<T> Succeed(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Value = value
            };
        }
        public static Result<T> Failure(string err)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Errors = new List<string> { err }
            };
        }

        public static Result<T> Failure(List<string> err)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Errors = err
            };
        }
    }

}
