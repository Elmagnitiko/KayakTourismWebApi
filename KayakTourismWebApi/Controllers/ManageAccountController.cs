using KayakData.DTOs.ManageAccountNS;
using KayakData.ModelsNS;
using KayakTourismWebApi.HelpersNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

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

            var customer = await _userManager.GetUserAsync(HttpContext.User);

            if (customer == null)
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
        }

        [Authorize]
        [HttpPost("changeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _userManager.GetUserAsync(HttpContext.User);
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
                    customerId = customer.Id, 
                    token = token, 
                    newEmail = model.Email 
                },
                protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email, 
                "Confirm your new email",
                $"Please, confirm your new email by clicking this link: \n{callbackUrl}");

            return Ok(new { Message = "Please, check your email to confirm new email." });
        }

        [AllowAnonymous]
        [HttpGet("confirmNewEmail")]
        public async Task<IActionResult> ConfirmNewEmail(string customerId, string token, string newEmail)
        {
            if (customerId == null || token == null || newEmail == null)
            {
                return BadRequest("Email confirmation error.");
            }

            var customer = await _userManager.FindByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound($"Can not find a customer with ID '{customerId}'.");
            }

            var result = await _userManager.ChangeEmailAsync(customer, newEmail, token);
            customer.UserName = newEmail;
            _ = await _userManager.UpdateAsync(customer);

            if (result.Succeeded)
            {
                return Ok("New email has set.");
            }

            return StatusCode(500, "Email confirmation error.");
        }
    }
}
