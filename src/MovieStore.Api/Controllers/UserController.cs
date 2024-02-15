using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieStore.Api.Data;

namespace MovieStore.Api.Controllers;

// This should go to a different project in the real world but I am just playing around

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly JwtOptions _jwtOptions;
    private readonly IConfiguration _configuration;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        JwtOptions jwtOptions,
        IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
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
            return Ok(new AccessTokenResponse
            {
                AccessToken = token,
                ExpiresIn = 3600,
                RefreshToken = token
            });
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("Invalid email or password.");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (signInResult.Succeeded)
        {
            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new AccessTokenResponse
            {
                AccessToken = token,
                ExpiresIn = 3600,
                RefreshToken = token
            });
        }
        else
        {
            return BadRequest("Invalid email or password.");
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.ConfirmationCode);
        if (result.Succeeded)
        {
            // Email confirmed successfully
            return Ok("Email confirmed!");
        }
        else
        {
            return BadRequest("Email confirmation failed.");
        }
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
        if (result.Succeeded)
        {
            // Password reset successful
            return Ok("Password reset successful!");
        }
        else
        {
            return BadRequest("Password reset failed.");
        }
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

    private string GenerateJwtToken(IdentityUser<Guid> user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "NoEmail"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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
    public string UserId { get; set; }
    public string ConfirmationCode { get; set; }
}

public class Enable2FARequest
{
    public string UserId { get; set; }
    public bool? Enable { get; set; }
    public string? TwoFactorCode { get; set; }
    public bool ResetSharedKey { get; set; }
    public bool ResetRecoveryCodes { get; set; }
    public bool ForgetMachine { get; set; }


}

public class Enable2FAResponse
{
    public string? SharedKey { get; set; }
    public int RecoveryCodesLeft { get; set; }
    public string[] RecoveryCodes { get; set; }
    public bool? IsTwoFactorEnable { get; set; }
    public bool IsMachineRemembered { get; set; }
}

public class Verify2FARequest
{
    public string UserId { get; set; }
    public string TotpCode { get; set; }
}
