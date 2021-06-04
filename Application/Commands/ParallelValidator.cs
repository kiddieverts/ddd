using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRental
{
    public static class ParallelValidator
    {
        public static (List<IError> errors, U) Create<U>(U u) where U : class => (new List<IError>(), u);

        public static (List<IError> errors, U) Validate<T, U>(this (List<IError> errors, U o) input,
            Result<T> result, Func<U, T, U> fn) where U : class =>
                result.Value.Match(
                    Left: err => (input.errors.Concat(err).ToList(), input.o),
                    Right: res => (input.errors, fn(input.o, res))
                );

        public static Result<T> ToResult<T>(this (List<IError> errors, T obj) o) =>
            o.errors.Count() == 0
                ? Result<T>.Succeed(o.obj)
                : Result<T>.Failure(o.errors);
    }
}