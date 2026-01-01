using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Services
{
    public interface ICredentialsService
    {
        Task SaveCredentialsAsync(string username, string password, bool rememberMe);
        Task<(string username, string password, bool rememberMe)> GetCredentialsAsync();
        Task ClearCredentialsAsync();
        Task<bool> HasRememberMeAsync();
    }
}
