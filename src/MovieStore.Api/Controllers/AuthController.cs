using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieStore.Api.Data;
using MovieStore.Api.Middleware;

namespace MovieStore.Api.Controllers;

// This should go to a different project in the real world but I am just playing around

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly JwtOptions _jwtOptions;
    private readonly IConfiguration _configuration;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        JwtOptions jwtOptions,
        IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtOptions = jwtOptions;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        // Validate input (e.g., email, password)
        // Create a new IdentityUser
        var user = new IdentityUser<Guid> { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Generate JWT token
            var token = GenerateJwtToken(user);
            _logger.LogInformation($"User {user.UserName} created");
            return Ok(new AccessTokenResponse
            {
                AccessToken = token,
                ExpiresIn = 3600,
                //change this
                RefreshToken = token
            });
        }
        else
        {
            _logger.LogInformation($"Creation of user {model.Email} failed");
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogInformation($"User {model.Email} login attempt failed"); // and maybe do something to avoid spam/DDoS
            return BadRequest("Invalid email or password.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (signInResult.Succeeded)
        {
            // Generate JWT token
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, userRoles);
            _logger.LogInformation($"User {model.Email} successfully logged in");
            return Ok(new AccessTokenResponse
            {
                AccessToken = token,
                ExpiresIn = 3600,
                //change this
                RefreshToken = token
            });
        }
        else
        {
            _logger.LogInformation($"User {model.Email} login attempt failed"); // and maybe do something to avoid spam/DDoS
            return BadRequest("Invalid email or password.");
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest model)
    {
        if (model.UserId is null || model.ConfirmationCode is null)
        {
            return BadRequest($"User Id and Confirmation code must be not null");
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.ConfirmationCode);

        // Email confirmed successfully
        return result.Succeeded
        ? Ok("Email confirmed!")
        : BadRequest("Email confirmation failed.");

    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Send the new confirmation email with the token
        }
        // Return appropriate response
        return Ok("Email to confirm email sent");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Send the password reset email with the token
        }
        // Return appropriate response
        return Ok("Email to reset password sent");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.ResetCode, model.NewPassword);

        // Password reset successful
        return result.Succeeded
        ? Ok("Password reset successful!")
        : BadRequest("Password reset failed.");
    }

    // [HttpPost("enable-2fa")]
    // public async Task<IActionResult> EnableTwoFactorAuth(Enable2FAModel model)
    // {
    //     var user = await _userManager.FindByIdAsync(model.UserId);
    //     if (user == null)
    //     {
    //         return BadRequest("User not found.");
    //     }

    //     // Generate a TOTP secret key for the user
    //     var totpSecretKey = GenerateTotpSecretKey();
    //     user.TotpSecretKey = totpSecretKey; // Save the secret key in the user's profile

    //     // Display the QR code for the user to scan (use a QR code library)
    //     var qrCodeImageUrl = GenerateQrCodeImageUrl(user.Email, totpSecretKey);

    //     // Return the QR code URL to the client
    //     return Ok(new { QrCodeImageUrl = qrCodeImageUrl });
    // }

    // [HttpPost("verify-2fa")]
    // public async Task<IActionResult> VerifyTwoFactorAuth(Verify2FAModel model)
    // {
    //     var user = await _userManager.FindByIdAsync(model.UserId);
    //     if (user == null)
    //     {
    //         return BadRequest("User not found.");
    //     }

    //     // Validate the TOTP code entered by the user
    //     if (ValidateTotpCode(user.TotpSecretKey, model.TotpCode))
    //     {
    //         // TOTP code is valid; user is authenticated
    //         // Generate and return a new JWT token
    //         var token = GenerateJwtToken(user);
    //         return Ok(new { Token = token });
    //     }
    //     else
    //     {
    //         return BadRequest("Invalid TOTP code.");
    //     }
    // }

    [HttpPost("roles/add")]
    [Authorize(Roles = IdentityData.AdminUserPolicyName)]
    //[Authorize(Policy = IdentityData.AdminUserPolicyName)]
    //[RequiresClaim(IdentityData.AdminUserClaimName, "true")]
    public async Task<IActionResult> AddRole(string name)
    {
        IdentityResult result = await _roleManager.CreateAsync(new IdentityRole<Guid>(name));

        return result.Succeeded
        ? Ok($"Role {name} created")
        : BadRequest($"Role {name} failed to be created");

    }

    [HttpPost("roles/addUserRole")]
    [Authorize(Roles = IdentityData.AdminUserPolicyName)]
    public async Task<IActionResult> AddUserRole(UpdateUserRoleRequest model)
    {
        if (model.UserId is null || model.RoleName is null)
        {
            return BadRequest($"User Id and Role name must be not null");
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded
            ? Ok($"Role {model.RoleName} added to {user.UserName}")
            : BadRequest($"Role {model.RoleName} failed to be added to {user.UserName}");

        }
        else
        {
            return BadRequest($"User Id {model.UserId} doesn't exist");
        }
    }

    [HttpPost("roles/removeUserRole")]
    [Authorize(Roles = IdentityData.AdminUserPolicyName)]
    public async Task<IActionResult> RemoveUserRole(UpdateUserRoleRequest model)
    {
        if (model.UserId is null || model.RoleName is null)
        {
            return BadRequest($"User Id and Role name must be not null");
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null)
        {
            return BadRequest($"User Id {model.UserId} doesn't exist");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

        return result.Succeeded
        ? Ok($"Role {model.RoleName} removed from {user.UserName}")
        : BadRequest($"Role {model.RoleName} failed to be removed from {user.UserName}");
    }

    // // Helper method to generate a TOTP secret key
    // private string GenerateTotpSecretKey()
    // {
    //     // Implement your logic to generate a random secret key
    //     // (e.g., use a secure random generator)
    //     // Return the base32-encoded secret key
    // }

    // // Helper method to generate a QR code image URL
    // private string GenerateQrCodeImageUrl(string email, string totpSecretKey)
    // {
    //     // Implement your logic to generate a QR code image URL
    //     // (e.g., use a QR code library like QRCoder)
    //     // Return the URL to the client
    // }

    // // Helper method to validate the TOTP code
    // private bool ValidateTotpCode(string totpSecretKey, string totpCode)
    // {
    //     // Implement your logic to validate the TOTP code
    //     // (e.g., use a TOTP library)
    //     // Return true if valid, false otherwise
    // }

    private string GenerateJwtToken(IdentityUser<Guid> user, IList<string>? userRoles = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "NoEmail"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (userRoles is not null)
        {
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
        }

        var token = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.CreateEncodedJwt(token);
    }
}

public class AccessTokenResponse
{
    public string TokenType { get; } = "Jwt";
    public required string AccessToken { get; set; }
    public required int ExpiresIn { get; set; }
    public required string RefreshToken { get; set; }
}

public class ConfirmEmailRequest
{
    public string? UserId { get; }
    public string? ConfirmationCode { get; }
}

public class Enable2FARequest
{
    public string? UserId { get; }
    public bool? Enable { get; }
    public string? TwoFactorCode { get; }
    public bool ResetSharedKey { get; }
    public bool ResetRecoveryCodes { get; }
    public bool ForgetMachine { get; }


}

public class Enable2FAResponse
{
    public string? SharedKey { get; set; }
    public int RecoveryCodesLeft { get; set; }
    public string[]? RecoveryCodes { get; set; }
    public bool? IsTwoFactorEnable { get; set; }
    public bool IsMachineRemembered { get; set; }
}

public class Verify2FARequest
{
    public string? UserId { get; }
    public string? TotpCode { get; }
}

public class UpdateUserRoleRequest
{
    public string? UserId { get; init; }
    public string? RoleName { get; init; }
}
