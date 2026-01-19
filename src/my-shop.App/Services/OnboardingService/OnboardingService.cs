using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Services.OnboardingService
{
    public class OnboardingService : IOnboardingService
    {
        private const string ONBOARDING_KEY = "onboarding_completed";
        private readonly Windows.Storage.ApplicationDataContainer _localSettings;

        public OnboardingService()
        {
            _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

        public Task<bool> ShouldShowOnboardingAsync()
        {
            try
            {
                // Check if onboarding has been completed
                if (_localSettings.Values.ContainsKey(ONBOARDING_KEY))
                {
                    var completed = _localSettings.Values[ONBOARDING_KEY] as bool?;
                    Debug.WriteLine($"🔵 [Onboarding] Completed: {completed}");
                    return Task.FromResult(!(completed ?? false));
                }

                // First time user - show onboarding
                Debug.WriteLine("🔵 [Onboarding] First time user - showing onboarding");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Onboarding] Error checking status: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public Task MarkOnboardingCompleteAsync()
        {
            try
            {
                _localSettings.Values[ONBOARDING_KEY] = true;
                Debug.WriteLine("✅ [Onboarding] Marked as completed");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Onboarding] Error marking complete: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        public Task ResetOnboardingAsync()
        {
            try
            {
                _localSettings.Values.Remove(ONBOARDING_KEY);
                Debug.WriteLine("🔵 [Onboarding] Reset - will show on next launch");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Onboarding] Error resetting: {ex.Message}");
                return Task.CompletedTask;
            }
        }
    }
}
