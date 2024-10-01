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
                };

                var createdCustomer = await _userManager.CreateAsync(customer, model.Password);

                if(createdCustomer.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(customer, Constants.CustomerRole);

                    if(roleResult.Succeeded)
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

        [HttpPost]
        //[ValidateAntiForgeryToken] 
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [AllowAnonymous]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string customerId, string code)
        {
            if (customerId == null || code == null)
            {
                return BadRequest("Email confirmation error.");
            }

            var customer = await _userManager.FindByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound($"Can not find a user with ID '{customerId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(customer, code);
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
