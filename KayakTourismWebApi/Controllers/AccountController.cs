using KayakTourismWebApi.Controllers;
using KayakTourismWebApi.DTOs.Account;
using KayakTourismWebApi.DTOs.AccountNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace KayakTourismWebApi.ControllersNS
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ITwoFactorAuthenticationService _twoFactorAuthServ;

        public AccountController(UserManager<Customer> userManager, 
            ITokenService tokenService, 
            SignInManager<Customer> signInManager,
            IEmailSender emailSender,
            ITwoFactorAuthenticationService twoFactorAuthService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _twoFactorAuthServ = twoFactorAuthService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = new Customer
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumder,
                    TwoFactorEnabled = true,
                };

                var createdCustomer = await _userManager.CreateAsync(customer, model.Password);

                if(createdCustomer.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(customer, Constants.CustomerRole);

                    if (roleResult.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(customer);

                        var callbackUrl = Url.Action(
                            nameof(ConfirmEmail),
                            "Account",
                            new { userId = customer.Id, code },
                            protocol: HttpContext.Request.Scheme);

                        await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                            $"Please, confirm your email by clicking this link: \n{callbackUrl}");
                        return Ok(new { Message = "Please, check your email to finish account creating" });
                    }

                    return StatusCode(500, roleResult.Errors);  
                }

                return StatusCode(500, createdCustomer.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if(!ModelState.IsValid || loginDto == null)
            {
                return BadRequest(ModelState);
            }

            var customer = await _userManager.Users.FirstOrDefaultAsync(x =>  x.Email == loginDto.Email.ToLower());
            if (customer == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(customer, loginDto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized("Username or password is not correct");
            }

            return Ok(new NewCustomerDto
            {
                Username = customer.UserName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Token = _tokenService.CreateToken(customer)
            });
        }

        [HttpPost("login2fa")]
        public async Task<IActionResult> Login2fa([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            await _signInManager.SignOutAsync();
            

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailSender.SendEmailAsync(model.Email, "Your authentication code", $"Your code is {code}");
            _twoFactorAuthServ.SaveToken(user.Id, code);

            return Ok("Code sent to email.");
        }

        //[AllowAnonymous]
        [HttpPost("verify2faCode")]
        public async Task<IActionResult> Verify2faCode([FromBody] VerifyCodeDto model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return BadRequest(ModelState);
            }

            await _signInManager.SignOutAsync();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email.");
            }

            var storedCode = _twoFactorAuthServ.GetToken(user.Id);
            if(storedCode == model.Code)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Username or password is not correct");
                }

                _twoFactorAuthServ.InvalidateToken(user.Id);
                return Ok(new NewCustomerDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Token = _tokenService.CreateToken(user)
                });
            }

            return Unauthorized("Invalid authentication code.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                try
                {
                    await _signInManager.SignOutAsync();
                    return Ok("Logged out successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return BadRequest("User is not logged in.");
            }
        }

        [AllowAnonymous]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Email confirmation error.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Can not find a user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok("Email is confirmed.");
            }

            return StatusCode(500, "Email confirmation error.");
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound("User with this email doesn't exist.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action(
                nameof(AccountController.ResetPassword),
                "Account",
                new { token = token, email = model.Email },
                protocol : HttpContext.Request.Scheme
            );

            await _emailSender.SendEmailAsync(
                model.Email, 
                "Reset Password",
                $"Please reset your password by clicking here: {callbackUrl}");

            return Ok("the password reset confirmation link is sent to email");
        }

        [HttpGet("resetPassword")]
        //[AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPasswordDto { Token = token, Email = email };
            return Ok(new
            {
                model
            });
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (resetPasswordResult.Succeeded)
            {
                return Ok("Password has been changed");
            }
            return BadRequest(resetPasswordResult.Errors);
        }
    }
}
