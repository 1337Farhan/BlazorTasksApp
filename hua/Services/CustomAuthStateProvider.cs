using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;

namespace hua.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ILocalStorageService _localStorage;
        private readonly ILogger<CustomAuthStateProvider> _logger;

        public CustomAuthStateProvider(ILocalStorageService localStorage, ILogger<CustomAuthStateProvider> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
            _currentUser = _anonymous;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Handle prerendering scenario - local storage won't be available
                var email = await _localStorage.GetItemAsync<string>("userEmail");
                var userId = await _localStorage.GetItemAsync<string>("userId");
                var userName = await _localStorage.GetItemAsync<string>("userName");
                var userRole = await _localStorage.GetItemAsync<string>("userRole");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(userId))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userName ?? email),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    };

                    if (!string.IsNullOrEmpty(userRole))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var identity = new ClaimsIdentity(claims, "custom");
                    _currentUser = new ClaimsPrincipal(identity);
                    
                    // _logger.LogInformation("User authenticated from local storage: {Email}", email);
                }
                else
                {
                    _currentUser = _anonymous;
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // Handle prerendering scenario - return anonymous user
                _logger.LogDebug("Prerendering detected - returning anonymous authentication state");
                _currentUser = _anonymous;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving authentication state");
                _currentUser = _anonymous;
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task NotifyUserAuthenticationAsync(string email, string userId, string name, string role)
        {
            try
            {
                await _localStorage.SetItemAsync("userEmail", email);
                await _localStorage.SetItemAsync("userId", userId);
                await _localStorage.SetItemAsync("userName", name);
                await _localStorage.SetItemAsync("userRole", role);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, role)
                };

                var identity = new ClaimsIdentity(claims, "custom");
                _currentUser = new ClaimsPrincipal(identity);

                _logger.LogInformation("User authenticated: {Email}", email);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user authentication notification");
            }
        }

        public async Task NotifyUserLogoutAsync()
        {
            try
            {
                await _localStorage.RemoveItemAsync("userEmail");
                await _localStorage.RemoveItemAsync("userId");
                await _localStorage.RemoveItemAsync("userName");
                await _localStorage.RemoveItemAsync("userRole");

                _currentUser = _anonymous;
                _logger.LogInformation("User logged out");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user logout");
            }
        }
    }
}