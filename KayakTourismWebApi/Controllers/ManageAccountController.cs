using KayakTourismWebApi.DTOs.ManageAccountNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public ManageAccountController(UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;   
            _signInManager = signInManager;
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
            
            return Unauthorized("Something went wrong");

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
        //private async Task<Customer> GetCustomerOrFail()
        //{
        //    var customer = await _userManager.GetUserAsync(User);

        //    if (customer == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //        //return Unauthorized();
        //    }

        //    return customer;
        //}

    }
}
