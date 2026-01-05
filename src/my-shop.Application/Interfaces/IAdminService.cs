using my_shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.Application.Interfaces
{
    public interface IAdminService
    {
        Task<Admin> CreateAsync(string email, string password, string name);
        Task<string> SessionAsync(string email, string password);
    }
}
