using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Services.LoginService
{
    public interface ISignupService
    {
        Task SignupAsync(string email, string password, string name);
    }
}
