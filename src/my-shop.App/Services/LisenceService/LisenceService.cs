using my_shop.App.Services.LoginService;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Services.LicenseService
{
    public class LicenseService
    {
        private const int TRIAL_DAYS = 15;
        private const string ACTIVATION_SECRET = "MyShop2026SecretKey";

        public class LicenseStatus
        {
            public bool IsValid { get; set; }
            public bool IsInTrial { get; set; }
            public bool IsActivated { get; set; }
            public int DaysRemaining { get; set; }
            public string Message { get; set; } = string.Empty;
            public DateTimeOffset? TrialStartDate { get; set; }
            public DateTimeOffset? TrialEndDate { get; set; }
        }

        private readonly IAdminRepository _adminRepository;
        private readonly IAuthService _authService; // ← Changed from AuthService to IAuthService

        public LicenseService(
            IAdminRepository adminRepository,
            IAuthService authService) // ← Changed from AuthService to IAuthService
        {
            _adminRepository = adminRepository;
            _authService = authService;
        }

        public async Task<LicenseStatus> CheckLicenseAsync()
        {
            try
            {
                var currentAdminId = _authService.CurrentAdminId;
                Debug.WriteLine($"Checking license for admin ID: {currentAdminId}");

                var admin = await _adminRepository.GetByIdAsync(currentAdminId);

                if (admin == null)
                {
                    Debug.WriteLine("Admin not found");
                    return new LicenseStatus
                    {
                        IsValid = false,
                        Message = "Admin not found"
                    };
                }

                Debug.WriteLine($"Admin found: {admin.Email}, IsActivated: {admin.IsActivated}");

                // If already activated, always valid
                if (admin.IsActivated)
                {
                    return new LicenseStatus
                    {
                        IsValid = true,
                        IsActivated = true,
                        Message = "Software is activated"
                    };
                }

                // Check trial period
                var trialStartDate = admin.TrialStartDate ?? admin.CreatedAt;
                var trialEndDate = trialStartDate.AddDays(TRIAL_DAYS);
                var now = DateTimeOffset.UtcNow;
                var daysRemaining = (int)(trialEndDate - now).TotalDays;

                Debug.WriteLine($"Trial: Start={trialStartDate}, End={trialEndDate}, DaysRemaining={daysRemaining}");

                if (now <= trialEndDate)
                {
                    return new LicenseStatus
                    {
                        IsValid = true,
                        IsInTrial = true,
                        DaysRemaining = Math.Max(0, daysRemaining),
                        TrialStartDate = trialStartDate,
                        TrialEndDate = trialEndDate,
                        Message = $"Trial period: {Math.Max(0, daysRemaining)} days remaining"
                    };
                }

                return new LicenseStatus
                {
                    IsValid = false,
                    IsInTrial = false,
                    DaysRemaining = 0,
                    TrialStartDate = trialStartDate,
                    TrialEndDate = trialEndDate,
                    Message = "Trial period expired. Please activate the software."
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking license: {ex.Message}");
                return new LicenseStatus
                {
                    IsValid = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public string GenerateActivationCode(string email)
        {
            // Generate activation code based on email and secret
            var input = $"{email}{ACTIVATION_SECRET}";
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var code = Convert.ToBase64String(hashBytes)
                    .Replace("+", "")
                    .Replace("/", "")
                    .Replace("=", "")
                    .Substring(0, 20)
                    .ToUpper();

                // Format: XXXXX-XXXXX-XXXXX-XXXXX
                return $"{code.Substring(0, 5)}-{code.Substring(5, 5)}-{code.Substring(10, 5)}-{code.Substring(15, 5)}";
            }
        }

        public async Task<bool> ActivateSoftwareAsync(string activationCode)
        {
            var currentAdminId = _authService.CurrentAdminId;
            var admin = await _adminRepository.GetByIdAsync(currentAdminId);
            if (admin == null) return false;

            // Validate activation code
            var expectedCode = GenerateActivationCode(admin.Email);
            Debug.WriteLine($"Expected code: {expectedCode}");
            Debug.WriteLine($"Provided code: {activationCode}");

            if (activationCode.Replace("-", "").ToUpper() != expectedCode.Replace("-", "").ToUpper())
            {
                Debug.WriteLine("Activation code mismatch");
                return false;
            }

            // Activate
            admin.IsActivated = true;
            admin.ActivationCode = activationCode;
            admin.ActivatedAt = DateTimeOffset.UtcNow;
            admin.UpdatedAt = DateTimeOffset.UtcNow;

            await _adminRepository.UpdateAsync(admin);
            Debug.WriteLine("Software activated successfully");
            return true;
        }

        public async Task<bool> InitializeTrialAsync()
        {
            var currentAdminId = _authService.CurrentAdminId;
            var admin = await _adminRepository.GetByIdAsync(currentAdminId);
            if (admin == null) return false;

            if (admin.TrialStartDate == null)
            {
                admin.TrialStartDate = DateTimeOffset.UtcNow;
                admin.UpdatedAt = DateTimeOffset.UtcNow;
                await _adminRepository.UpdateAsync(admin);
                Debug.WriteLine("Trial initialized");
            }

            return true;
        }
    }

    // Interface for repository
    public interface IAdminRepository
    {
        Task<Domain.Entities.Admin?> GetByIdAsync(int id);
        Task UpdateAsync(Domain.Entities.Admin admin);
    }
}