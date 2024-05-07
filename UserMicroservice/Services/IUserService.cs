using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.UserMicroservice.Models;

namespace MyBusiness.UserMicroservice.Services
{
    public interface IUserService
    {
        Task<string> RegistrationAsync(User user);

        Task<string> LoginAsync(string email, string password);
    }
}