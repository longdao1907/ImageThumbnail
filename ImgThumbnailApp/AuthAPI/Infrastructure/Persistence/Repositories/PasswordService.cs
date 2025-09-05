﻿using AuthAPI.Core.Application.Interfaces;

namespace AuthAPI.Infrastructure.Persistence.Repositories
{
    public class PasswordService: IPasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
