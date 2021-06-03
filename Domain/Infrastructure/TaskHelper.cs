using System;
using System.Threading.Tasks;

namespace MyRental
{
    public static class TaskHelper
    {
        public static async Task<Result<T>> SelectMany<R, T>(this Task<Result<R>> result, Func<R, Task<Result<T>>> f)
        {
            var r = await result;
            return await r.SelectMany(f);
        }
    }
}