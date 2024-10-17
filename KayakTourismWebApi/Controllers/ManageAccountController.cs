using KayakTourismWebApi.DTOs.ManageAccount;
using KayakTourismWebApi.DTOs.ManageAccountNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.Controllers
{
    [Route("api/manageAccount")]
    [ApiController]
    public class ManageAccountController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IEmailSender _emailSender;

        public ManageAccountController(UserManager<Customer> userManager, 
            SignInManager<Customer> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;   
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _userManager.GetUserAsync(User);
            if(customer == null)
            {
                return Unauthorized("First, log in to your account");
            }

            var result = await _userManager.ChangePasswordAsync(customer, model.OldPassword, model.NewPassword);
            
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(customer, isPersistent: true);
                return Ok("Password changed successfully.");
            }
            
            return BadRequest("Something went wrong");

            //var user = await _userManager.FindByIdAsync(model.CustomerId);
            //if(user == null)
            //{
            //    BadRequest("lenin grib");
            //}

            //var passwordChangingResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            //if (passwordChangingResult.Succeeded)
            //{
            //    return Ok("password is changed");
            //}

            //return BadRequest("something is wrong");
        }

        [Authorize]
        [HttpPost("changeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _userManager.GetUserAsync(User);
            if (customer == null)
            {
                return Unauthorized("First, log in to your account");
            }
            
            var token = await _userManager.GenerateChangeEmailTokenAsync(customer, model.Email);
            var callbackUrl = Url.Action(
                nameof(ConfirmNewEmail),
                "ManageAccount",
                new 
                { 
                    userId = customer.Id, 
                    token = token, 
                    newEmail = model.Email 
                },
                protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email, 
                "Confirm your new email",
                $"Please, confirm your new email by clicking this link: \n{callbackUrl}");

            return Ok(new { Message = "Please, check your email to finish account creating" });
        }

        [Authorize]
        [HttpGet("confirmNewEmail")]
        public async Task<IActionResult> ConfirmNewEmail(string userId, string token, string newEmail)
        {
            if (userId == null || token == null || newEmail == null)
            {
                return BadRequest("Email confirmation error.");
            }

            var customer = await _userManager.FindByIdAsync(userId);
            if (customer == null)
            {
                return NotFound($"Can not find a user with ID '{userId}'.");
            }

            var result = await _userManager.ChangeEmailAsync(customer, newEmail, token);
            if (result.Succeeded)
            {
                return Ok("New email has set.");
            }

            return StatusCode(500, "Email confirmation error.");
        }
    }
}
