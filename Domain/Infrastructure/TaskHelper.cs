using System;
using System.Threading.Tasks;

namespace MyRental
{
    public static class TaskHelper
    {
        public static async Task<Result<T>> SelectMany<R, T>(this Task<Result<R>> result, Func<R, Task<Result<T>>> f) =>
            await result.SelectMany(f);

        public static Task<Result<T>> SelectMany<T>(this Task<Result<T>> result, Func<T, Result<T>> f) => result;
    }
}