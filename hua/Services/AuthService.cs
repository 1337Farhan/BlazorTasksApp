using System.Security.Cryptography;
using System.Text;
using hua.Data;
using hua.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace hua.Services
{
    public class AuthService(IDbContextFactory<AppDbContext> contextFactory, EmailService emailService)
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory = contextFactory;
        private readonly EmailService _emailService = emailService;

        public async Task<User> Signup(string name, string email, string password, string role = "admin")
        {
            using var context = _contextFactory.CreateDbContext();
            if (await context.Users.AnyAsync(u => u.Email == email))
                throw new Exception("User with this email already exists.");

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            CreateVerificationToken(out string token);

            User user = new()
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = role,
                JoinDate = DateTime.UtcNow,
                EmailVerificationToken = token,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(user.Email, user.EmailVerificationToken);

            return user;
        }

        public async Task<User> Login(string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("User not found.");

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid password.");

            if (!user.IsEmailVerified)
            {
                throw new Exception("User is not verified, please check your email and try again.");
            }

            return user;
        }

        public async Task<ActivationResult> ActivateUserAccount(string verificationToken)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                // Find user by verification token
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.EmailVerificationToken == verificationToken);

                if (user == null)
                {
                    return new ActivationResult
                    {
                        Success = false,
                        Message = "Invalid verification token. Please make sure you're using the latest activation link from your email."
                    };
                }

                // Check if token is expired
                if (!user.IsEmailVerified && user.EmailVerificationTokenExpiry < DateTime.UtcNow)
                {
                    return new ActivationResult
                    {
                        Success = false,
                        Message = "Verification token has expired. Please request a new activation email."
                    };
                }

                // Check if user is already activated
                if (user.IsEmailVerified)
                {
                    return new ActivationResult
                    {
                        Success = true,
                        Message = "Account is already activated. You can now log in."
                    };
                }

                // Activate the user
                user.IsEmailVerified = true;

                await context.SaveChangesAsync();

                return new ActivationResult
                {
                    Success = true,
                    Message = "Account activated successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ActivationResult
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<bool> VerifyPasswordResetTokenAsync(string token)
{
            using var context = _contextFactory.CreateDbContext();
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
            if (user == null) return false;
            
            return user.PasswordResetTokenExpiry > DateTime.UtcNow;
        }
        
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null; // Clear the token after use
            user.PasswordResetTokenExpiry = null;

            await context.SaveChangesAsync();
            return true;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private static void CreateVerificationToken(out string token)
        {
            var guidPart = Guid.NewGuid().ToString("N").Substring(0, 16);
            var timePart = DateTime.UtcNow.Ticks.ToString("x");
            token = $"{guidPart}{timePart}";
        }

        public class ActivationResult
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }
        
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            using var context = _contextFactory.CreateDbContext();
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return true; // Return true for security (don't reveal if email exists)
            
            // Generate reset token
            var resetToken = GenerateSecureToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            
            await context.SaveChangesAsync();
            
            // Send email with reset token
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
            
            return true;
        }

        private static string GenerateSecureToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
