using System;

namespace MyRental
{
    public class UserService : IUserService
    {
        public Guid GetUserId() => Guid.NewGuid();
    }
}