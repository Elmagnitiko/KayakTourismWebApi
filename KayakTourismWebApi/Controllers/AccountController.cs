using KayakTourismWebApi.DTOs.AccountNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KayakTourismWebApi.ControllersNS
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        public AccountController(UserManager<Customer> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = new Customer
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumder,
                };

                var createdCustomer = await _userManager.CreateAsync(customer, registerDto.Password);

                if(createdCustomer.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(customer, Constants.CustomerRole);
                    if(roleResult.Succeeded)
                    {
                        return Ok("Customer created");
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
    }
}
