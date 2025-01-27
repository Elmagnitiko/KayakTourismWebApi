using KayakData.ModelsNS;
using KayakTourismWebApi.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using KayakData.DTOs.ManageAccountNS;
using Microsoft.AspNetCore.Mvc;
using Castle.Core.Resource;

namespace KayakTourismWebApi.Tests.Controller
{
    
    public class ManageAccountControllerTests
    {
        private readonly ManageAccountController _manageAccountController;
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly HttpContext _httpContext;

        private static readonly Customer fakeCustomer = new Customer
        {
            Id = "1",
            UserName = "test@example.com",
            Email = "test@example.com",
            PhoneNumber = "+375291234567",
            TwoFactorEnabled = false,
        };

        public ManageAccountControllerTests()
        {
            // Dependencies
            _userManager = A.Fake<UserManager<Customer>>();
            _signInManager = A.Fake<SignInManager<Customer>>();
            _emailSender = A.Fake<IEmailSender>();
            _httpContext = A.Fake<HttpContext>();

            // SUT
            _manageAccountController = new ManageAccountController(_userManager, _signInManager, _emailSender)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                },
                Url = A.Fake<IUrlHelper>()
            };
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            var model = new ChangePasswordDto
            {
                OldPassword = "12345_Kayak",
                NewPassword = "12345_Qwerty",
                ConfirmPassword = "12345_Qwerty",
            };

            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _userManager.ChangePasswordAsync(fakeCustomer, model.OldPassword, model.NewPassword))
                .Returns(IdentityResult.Success);
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);

            // Act
            var result = await _manageAccountController.ChangePassword(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenNotLogedIn()
        {
            // Ararnge
            var model = new ChangePasswordDto
            {
                OldPassword = "12345_Kayak",
                NewPassword = "12345_Qwerty",
                ConfirmPassword = "12345_Qwerty",
            };

            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _manageAccountController.ChangePassword(model);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenCanNotChangePassword()
        {
            // Arrange
            var model = new ChangePasswordDto
            {
                OldPassword = "12345_Kayak",
                NewPassword = "12345_Qwerty",
                ConfirmPassword = "12345_Qwerty",
            };

            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _userManager.ChangePasswordAsync(fakeCustomer, model.OldPassword, model.NewPassword))
                .Returns(IdentityResult.Failed());
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);

            // Act
            var result = await _manageAccountController.ChangePassword(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ChangeEmail_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            var model = new ChangeEmailDto
            {
                Email = "testemail@example.com",
            };

            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();

            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);

            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);

            A.CallTo(() => _userManager.GenerateChangeEmailTokenAsync(
                A<Customer>.Ignored, 
                A<string>.Ignored))
                .Returns("fake-email-token");

            A.CallTo(() => _emailSender.SendEmailAsync(
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored));

            // Act
            var result = await _manageAccountController.ChangeEmail(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ChangeEmail_ReturnsUnauthorized_WhenUserNotLogedIn()
        {
            // Arange
            var model = new ChangeEmailDto
            {
                Email = "testemail@example.com",
            };

            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();

            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);

            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _manageAccountController.ChangeEmail(model);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task ConfirmNewEmail_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync("id")).Returns(fakeCustomer);
            A.CallTo(() => _userManager.ChangeEmailAsync(fakeCustomer, "newemail@mail.com", "fake-token"))
                .Returns(IdentityResult.Success);
            // Act
            var result = await _manageAccountController.ConfirmNewEmail("id", "fake-token", "newemail@mail.com");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(null, "token", "newEmail")]
        [InlineData("customerId", null, "newEmail")]
        [InlineData("customerId", "token", null)]
        [InlineData(null, null, "newEmail")]
        [InlineData(null, "token", null)]
        [InlineData("customerId", null, null)]
        [InlineData(null, null, null)]
        public async Task ConfirmNewEmail_ReturnsBadRequest_WhenAnyArgumentIsNull(string customerId, string token, string newEmail)
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync("id")).Returns(fakeCustomer);
            A.CallTo(() => _userManager.ChangeEmailAsync(fakeCustomer, "newemail@mail.com", "fake-token"))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _manageAccountController.ConfirmNewEmail(customerId, token, newEmail); 

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Email confirmation error.");
        }
        [Fact]
        public async Task ConfirmNewEmail_ReturnsNotFound_WhenCanNotFindCustomerWuthSuchId()
        {
            // Arrange
            var customerId = "id";
            A.CallTo(() => _userManager.FindByIdAsync(customerId)).Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _manageAccountController.ConfirmNewEmail("id", "fake-token", "newemail@mail.com");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Can not find a customer with ID '{customerId}'.");
        }

        [Fact]
        public async Task ConfirmNewEmail_ReturnsCode500_WhenCanNotChangeEmail()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync("id")).Returns(fakeCustomer);
            A.CallTo(() => _userManager.ChangeEmailAsync(fakeCustomer, "newemail@mail.com", "fake-token"))
                .Returns(IdentityResult.Failed());
            // Act
            var result = await _manageAccountController.ConfirmNewEmail("id", "fake-token", "newemail@mail.com");

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }
    }
}
