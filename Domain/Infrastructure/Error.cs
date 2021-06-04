using System;

namespace MyRental
{
    public record ValidationError : BaseError { }
    public record AuthorizationError : BaseError { }
    public record DomainError : BaseError { }
    public record SystemError : BaseError { }
    public record NotFoundError : BaseError { }

    public interface IError
    {
        ErrorType[] Errors { get; init; }
        Exception[] Exceptions { get; init; }
    }

    public record BaseError : IError
    {
        public ErrorType[] Errors { get; init; } = new ErrorType[] { };
        public Exception[] Exceptions { get; init; } = new Exception[] { };
        protected BaseError() { }
        public static BaseError Create(ErrorType err) =>
           new BaseError { Errors = new ErrorType[] { err } };

        public static BaseError Create(ErrorType[] err) =>
            new BaseError { Errors = err };

        public static BaseError Create(Exception exception) =>
            new BaseError { Exceptions = new Exception[] { exception } };

        public static BaseError Create(Exception[] exceptions) =>
            new BaseError { Exceptions = exceptions };
    }
}
