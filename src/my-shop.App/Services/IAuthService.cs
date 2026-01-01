using my_shop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Services
{
    public interface IAuthService
    {
        Task<AdminLoginResponseDto> LoginAsync(string email, string password);
        Task LogoutAsync();
    }
}
