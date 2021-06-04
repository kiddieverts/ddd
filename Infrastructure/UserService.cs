using System;

namespace DDDExperiment
{
    public class UserService : IUserService
    {
        public Guid GetUserId() => Guid.NewGuid();
    }
}