using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;

namespace my_shop.App.Services.LoginService
{
    public class CredentialsService : ICredentialsService
    {
        private const string REMEMBER_ME_KEY = "remember_me";
        private const string USERNAME_KEY = "saved_username";
        private const string RESOURCE_NAME = "MyShopApp";

        /// <summary>
        /// Save credentials to secure storage
        /// </summary>
        public Task SaveCredentialsAsync(string username, string password, bool rememberMe)
        {
            try
            {
                // Save remember me flag
                ApplicationData.Current.LocalSettings.Values[REMEMBER_ME_KEY] = rememberMe;

                if (rememberMe)
                {
                    // Save username in plain storage
                    ApplicationData.Current.LocalSettings.Values[USERNAME_KEY] = username;

                    // Save password in secure PasswordVault
                    var vault = new PasswordVault();

                    // Remove old credential if exists
                    try
                    {
                        var oldCredential = vault.Retrieve(RESOURCE_NAME, username);
                        if (oldCredential != null)
                        {
                            vault.Remove(oldCredential);
                        }
                    }
                    catch
                    {
                        // No existing credential
                    }

                    // Add new credential
                    var credential = new PasswordCredential(RESOURCE_NAME, username, password);
                    vault.Add(credential);
                }
                else
                {
                    // Clear credentials if remember me is false
                    ClearCredentialsAsync();
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get saved credentials
        /// </summary>
        public Task<(string username, string password, bool rememberMe)> GetCredentialsAsync()
        {
            try
            {
                // Check remember me flag
                var rememberMe = false;
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(REMEMBER_ME_KEY))
                {
                    rememberMe = (bool)ApplicationData.Current.LocalSettings.Values[REMEMBER_ME_KEY];
                }


                if (!rememberMe)
                {
                    return Task.FromResult((string.Empty, string.Empty, false));
                }

                // Get username
                var username = string.Empty;
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(USERNAME_KEY))
                {
                    username = ApplicationData.Current.LocalSettings.Values[USERNAME_KEY] as string ?? string.Empty;
                }

                if (string.IsNullOrEmpty(username))
                {
                    return Task.FromResult((string.Empty, string.Empty, true));
                }

                // Get password from secure vault
                var password = string.Empty;
                try
                {
                    var vault = new PasswordVault();
                    var credential = vault.Retrieve(RESOURCE_NAME, username);
                    credential.RetrievePassword();
                    password = credential.Password;
                }
                catch
                {
                }

                return Task.FromResult((username, password, rememberMe));
            }
            catch (Exception ex)
            {
                return Task.FromResult((string.Empty, string.Empty, false));
            }
        }

        /// <summary>
        /// Clear all saved credentials
        /// </summary>
        public Task ClearCredentialsAsync()
        {
            try
            {
                // Clear remember me flag
                ApplicationData.Current.LocalSettings.Values.Remove(REMEMBER_ME_KEY);

                // Get username
                var username = string.Empty;
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(USERNAME_KEY))
                {
                    username = ApplicationData.Current.LocalSettings.Values[USERNAME_KEY] as string ?? string.Empty;
                    ApplicationData.Current.LocalSettings.Values.Remove(USERNAME_KEY);
                }

                // Clear password from vault
                if (!string.IsNullOrEmpty(username))
                {
                    try
                    {
                        var vault = new PasswordVault();
                        var credential = vault.Retrieve(RESOURCE_NAME, username);
                        vault.Remove(credential);
                    }
                    catch
                    {
                        // No credential to remove
                    }
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Check if remember me is enabled
        /// </summary>
        public Task<bool> HasRememberMeAsync()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(REMEMBER_ME_KEY))
            {
                return Task.FromResult((bool)ApplicationData.Current.LocalSettings.Values[REMEMBER_ME_KEY]);
            }
            return Task.FromResult(false);
        }
    }
}
