using Microsoft.UI.Dispatching;
using my_shop.App.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace my_shop.App.Services.LoginService;

public interface ITokenExpirationMonitor
{
    void StartMonitoring();
    void StopMonitoring();
    event Action? TokenExpired; // Fired when token expires
}

public class TokenExpirationMonitor : ITokenExpirationMonitor
{
    private readonly ITokenService _tokenService;
    private readonly IJwtHelper _jwtHelper;
    private DispatcherQueueTimer? _timer;

    public event Action? TokenExpired;

    // Check every 30 seconds
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

    public TokenExpirationMonitor(
        ITokenService tokenService,
        IJwtHelper jwtHelper)
    {
        _tokenService = tokenService;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Start monitoring token expiration
    /// </summary>
    public void StartMonitoring()
    {
        if (_timer != null)
        {
            Debug.WriteLine("Token monitor already running");
            return;
        }

        Debug.WriteLine("Starting token expiration monitor");

        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        if (dispatcherQueue == null)
        {
            Debug.WriteLine("DispatcherQueue not available");
            throw new InvalidOperationException("DispatcherQueue not available. StartMonitoring must be called from UI thread.");
        }

        _timer = dispatcherQueue.CreateTimer();
        _timer.Interval = _checkInterval;
        _timer.Tick += async (s, e) => await CheckTokenExpirationAsync();
        _timer.Start();

        _ = CheckTokenExpirationAsync();

        Debug.WriteLine("Token expiration monitor started");
    }


    /// <summary>
    /// Stop monitoring
    /// </summary>
    public void StopMonitoring()
    {
        if (_timer == null)
        {
            Debug.WriteLine("Token monitor not running");
            return;
        }

        _timer.Stop();
        _timer = null;
        Debug.WriteLine("Token expiration monitor stopped");
    }

    /// <summary>
    /// Check if token is expired and force logout if needed
    /// </summary>
    private async Task CheckTokenExpirationAsync()
    {
        try
        {
            Debug.WriteLine("Checking token expiration...");

            var token = await _tokenService.GetTokenAsync();

            // No token - stop monitoring
            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("No token found, stopping monitor");
                StopMonitoring();
                return;
            }

            // Decode token
            var payload = _jwtHelper.DecodeToken(token);

            if (payload == null)
            {
                Debug.WriteLine("Invalid token detected!");
                await ForceLogoutAsync("Invalid token");
                return;
            }

            // Check if expired
            if (payload.IsExpired)
            {
                Debug.WriteLine($"Token EXPIRED at {payload.ExpirationDateTime:yyyy-MM-dd HH:mm:ss}");
                await ForceLogoutAsync("Token expired");
                return;
            }

            // Token still valid
            var timeLeft = payload.TimeUntilExpiration;
            Debug.WriteLine($"Token valid, expires in {timeLeft:hh\\:mm\\:ss}");

            // Warn if expiring soon (less than 5 minutes)
            if (timeLeft < TimeSpan.FromMinutes(5))
            {
                Debug.WriteLine($"WARNING: Token expiring soon! {timeLeft:hh\\:mm\\:ss} remaining");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Token monitor error: {ex.Message}");
        }
    }

    /// <summary>
    /// Force logout: clear token and notify
    /// </summary>
    private async Task ForceLogoutAsync(string reason)
    {
        Debug.WriteLine($"FORCE LOGOUT: {reason}");

        // Clear token
        await _tokenService.ClearTokenAsync();

        // Stop monitoring
        StopMonitoring();

        // Notify listeners (MainWindow will handle navigation)
        TokenExpired?.Invoke();

        Debug.WriteLine("Force logout completed");
    }
}