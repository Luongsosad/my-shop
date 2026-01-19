using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Services.OnboardingService
{
    public interface IOnboardingService
    {
        Task<bool> ShouldShowOnboardingAsync();
        Task MarkOnboardingCompleteAsync();
        Task ResetOnboardingAsync();
    }
}
