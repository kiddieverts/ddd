using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
    public record Result<T>
    {
        public Either<IError, T> Value { get; init; }

        public static Result<T> Succeed(T v) => new Result<T>(v);
        public static Result<T> Failure(IError err) => new Result<T>(err);

        public Result(T value) => Value = new Right<IError, T>(value);
        public Result(IError value) => Value = new Left<IError, T>(value);

        // public Result(IError value) => Value = new Left<IError[], T>(new IError[] { value });

        // public void OnSuccess(Action<IError> action) => Value.IfLeft(action);
        // public void OnError(Action<T> action) => Value.IfRight(action);

        public Result<T1> Select<T1>(Func<T, T1> func) =>
            Value.Match(Left: e => new Result<T1>(e), Right: r => new Result<T1>(func(r)));

        public Result<T1> SelectMany<T1>(Func<T, Result<T1>> func) =>
            Value.Match(Left: e => new Result<T1>(e), Right: r => func(r));

        public Task<Result<T1>> SelectMany<T1>(Func<T, Task<Result<T1>>> func) =>
            Value.Match(Left: e => Task.FromResult(new Result<T1>(e)), Right: r => func(r));

        public T1 Map<T1>(Func<T, T1> successFn, Func<IError, T1> errorFn) =>
            Value.Match(Left: error => errorFn(error), Right: result => successFn(result));
    }

    public abstract record Either<TLeft, TRight>
    {
        // public abstract void IfLeft(Action<TLeft> action);
        // public abstract void IfRight(Action<TRight> action);
        public abstract Either<TLeft, T1Right> Select<T1Right>(Func<TRight, T1Right> mapping);
        public abstract TResult Match<TResult>(Func<TLeft, TResult> Left, Func<TRight, TResult> Right);
    }

    public record Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TLeft value;
        public Left(TLeft left) => value = left;
        // public override void IfLeft(Action<TLeft> action) => action(value);
        // public override void IfRight(Action<TRight> action) { }
        public override TResult Match<TResult>(Func<TLeft, TResult> Left, Func<TRight, TResult> Right) => Left(value);
        public override Either<TLeft, T1Right> Select<T1Right>(Func<TRight, T1Right> mapping) =>
            new Left<TLeft, T1Right>(value);
    }

    public record Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TRight value;
        public Right(TRight right) => value = right;
        // public override void IfLeft(Action<TLeft> action) { }
        // public override void IfRight(Action<TRight> action) => action(value);
        public override TResult Match<TResult>(Func<TLeft, TResult> Left, Func<TRight, TResult> Right) =>
            Right(value);
        public override Either<TLeft, T1Right> Select<T1Right>(Func<TRight, T1Right> mapping) =>
            new Right<TLeft, T1Right>(mapping(value));
    }
}
