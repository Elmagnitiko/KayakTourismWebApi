using KayakTourismWebApi.Controllers;
using KayakTourismWebApi.DTOs.Account;
using KayakTourismWebApi.DTOs.AccountNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public AccountController(UserManager<Customer> userManager, 
            ITokenService tokenService, 
            SignInManager<Customer> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent:true , lockoutOnFailure: false);

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailSender.SendEmailAsync(model.Email, "Your authentication code", $"Your code is {code}");

            return Ok("Code sent to email.");
        }

        [HttpPost("verify2faCode")]
        public async Task<IActionResult> Verify2faCode([FromBody] VerifyCodeDto model)
        {
            await _signInManager.SignOutAsync();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email.");
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", model.Code, false, false);
            if (result.Succeeded)
            {
                return Ok("Authentication successful.");
            }
            else
            {
                return Unauthorized("Invalid authentication code.");
            }
        }

        //[AllowAnonymous]
        //[HttpPost("login2fa")]
        //public async Task<IActionResult> LoginWith2FA([FromBody] LoginDto loginDto)
        //{
        //    if (!ModelState.IsValid || loginDto == null)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var customer = await _userManager.Users.FirstOrDefaultAsync(c => c.Email == loginDto.Email.ToLower());
        //    if (customer == null)
        //    {
        //        return Unauthorized("Invalid email or password");
        //    }

        //    var result = await _signInManager.CheckPasswordSignInAsync(customer, loginDto.Password, lockoutOnFailure: false);
        //    if (!result.Succeeded)
        //    {
        //        return Unauthorized("Email or password is not correct");
        //    }

        //    if (customer.TwoFactorEnabled)
        //    {
        //        var code = await _userManager.GenerateTwoFactorTokenAsync(customer, "Email");
        //        await _emailSender.SendEmailAsync(
        //            customer.Email,
        //            "Two factor authentication",
        //            $"Your two factor authentication key: \n\n{code}");
        //    }

        //    return Ok("The code was sent to the email.");
        //}

        //[AllowAnonymous]
        //[HttpPost("verify2FACode")]
        //public async Task<IActionResult> Verify2FACode([FromBody]LoginWith2FACodeDto loginDto)
        //{
        //    if (!ModelState.IsValid || loginDto == null)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var customer = await _userManager.Users.FirstOrDefaultAsync(c => c.Email == loginDto.Email.ToLower());
        //    if (customer == null)
        //    {
        //        return Unauthorized("Invalid email or password");
        //    }

        //    var result = await _signInManager.CheckPasswordSignInAsync(customer, loginDto.Password, lockoutOnFailure: false);
        //    if (!result.Succeeded)
        //    {
        //        return Unauthorized("Email or password is not correct");
        //    }

        //    var twoFactorSignInResult = await _signInManager.TwoFactorSignInAsync("Email",
        //        loginDto.TwoFactorCode.Replace(" ", "").Replace("-", ""),
        //        loginDto.RememberMe,
        //        loginDto.RememberMachine);

        //    if (!twoFactorSignInResult.Succeeded)
        //    {
        //        return Unauthorized("Invalid two factor authentivation code.");

        //    }

        //    return Ok($"User {customer.Email} is loged in");
        //}

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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
