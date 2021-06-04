using System;
using System.Collections.Generic;
using System.Linq;

namespace DDDExperiment
{
    public static class ParallelValidator
    {
        public static (IError err, U) Create<E, U>(E e, U u)
            where U : class
            where E : IError
            => (e, u);

        public static (IError errors, U) Validate<T, U>(this (IError err, U o) source,
            Result<T> result, Func<U, T, U> fn) where U : class =>
                result.Value.Match(
                    Left: err => ((BaseError)source.err with { Errors = err.Errors }, source.o),
                    Right: res => (source.err, fn(source.o, res))
                );

        public static Result<T> ToResult<T>(this (IError err, T obj) o) =>
            o.err.Errors.Count() == 0
                ? Result<T>.Succeed(o.obj)
                : Result<T>.Failure(o.err);
    }
}