using System;
using FakeItEasy;
using FluentAssertions;
using KayakTourismWebApi.ControllersNS;
using Microsoft.AspNetCore.Identity;
using KayakData.ModelsNS;
using KayakTourismWebApi.InterfacesNS;
using Microsoft.AspNetCore.Identity.UI.Services;
using KayakData.DTOs.AccountNS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using KayakTourismWebApi.HelpersNS;

namespace KayakTourismWebApi.Tests.Controller
{
    public class AccountControllerTests
    {
        private readonly AccountController _accountController;
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly HttpContext _httpContext;

        private static readonly Customer fakeCustomer = new Customer 
        { 
            UserName = "test@example.com", 
            Email = "test@example.com", 
            PhoneNumber = "+375291234567", 
            TwoFactorEnabled = false, 
        };
        private static readonly LoginDto loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        public AccountControllerTests()
        {
            // Dependencies
            _userManager = A.Fake<UserManager<Customer>>();
            _signInManager = A.Fake<SignInManager<Customer>>();
            _tokenService = A.Fake<ITokenService>();
            _emailSender = A.Fake<IEmailSender>();
            _httpContext = A.Fake<HttpContext>();

            // SUT
            _accountController = new AccountController(
                _userManager,
                _tokenService,
                _signInManager,
                _emailSender)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                },
                Url = A.Fake<IUrlHelper>()
            };

        }

        [Fact]
        public async Task Register_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            var model = new RegisterDto 
            { 
                Email = "test@example.com", 
                PhoneNumder = "+375291234567",
                Password = "Password123!" 
            };

            A.CallTo(() => _userManager.CreateAsync(A<Customer>.Ignored, A<string>.Ignored)).Returns(IdentityResult.Success);
            A.CallTo(() => _userManager.AddToRoleAsync(A<Customer>.Ignored, A<string>.Ignored)).Returns(IdentityResult.Success);
            A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<Customer>.Ignored)).Returns("confirmation-token");
            A.CallTo(() => _emailSender.SendEmailAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));

            // Act
            var responseResult = await _accountController.Register(model);

            // Assert
            responseResult.Should().BeOfType<OkObjectResult>();

        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(loginDto.Email))
                .Returns(fakeCustomer);

            A.CallTo(() => _signInManager.PasswordSignInAsync(
                fakeCustomer, loginDto.Password, true, false))
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            A.CallTo(() => _tokenService.CreateToken(fakeCustomer, _userManager))
                .Returns("fake-jwt-token");

            // Act
            var result = await _accountController.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>(); 
            var okResult = result as OkObjectResult; 
            var newCustomerDto = okResult.Value as NewCustomerDto; 
            newCustomerDto.Should().NotBeNull(); 
            newCustomerDto.Username.Should().Be(fakeCustomer.UserName); 
            newCustomerDto.Email.Should().Be(fakeCustomer.Email); 
            newCustomerDto.PhoneNumber.Should().Be(fakeCustomer.PhoneNumber);
            newCustomerDto.Token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenEmailIsNotCorrect()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(loginDto.Email))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _accountController.Login(loginDto);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be("Invalid email or password");
        }

        [Fact]
        public async Task Login_ReturnsUnauthorazied_WhenPasswordIsNotCorrectButEmailIsCorrect()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(loginDto.Email))
                .Returns(fakeCustomer);

            A.CallTo(() => _signInManager.PasswordSignInAsync(
                fakeCustomer, loginDto.Password, true, false))
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _accountController.Login(loginDto);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject; 
            unauthorizedResult.Value.Should().Be("Username or password is not correct");
        }

        [Fact]
        public async Task Login2fa_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(loginDto.Email))
                .Returns(fakeCustomer);
            A.CallTo(() => _userManager.CheckPasswordAsync(fakeCustomer,loginDto.Password))
                .Returns(true);
            A.CallTo(() => _signInManager.PasswordSignInAsync(
                fakeCustomer,
                loginDto.Password,
                true, false))
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);
            A.CallTo(() => _userManager.GenerateTwoFactorTokenAsync(fakeCustomer, "Email"))
                .Returns("fake-jwt-token");

            // Act
            var result = await _accountController.Login2fa(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login2fa_ReturnsUnauthorized_WhenEmailIsNotCorrect()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(loginDto.Email))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _accountController.Login2fa(loginDto);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be("Invalid email or password.");
        }

        [Fact]
        public async Task Login2fa_ReturnsUnauthorized_WhenPasswordIsNotCorrect()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(loginDto.Email))
                .Returns(fakeCustomer);
            A.CallTo(() => _userManager.CheckPasswordAsync(fakeCustomer, loginDto.Password))
                .Returns(false);
            // Act
            var result = await _accountController.Login2fa(loginDto);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be("Invalid email or password.");
        }
        [Fact]
        public async Task Verify2faCode_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _signInManager.GetTwoFactorAuthenticationUserAsync())
                .Returns(fakeCustomer);
            A.CallTo(() => _signInManager.TwoFactorSignInAsync(
                "Email",
                "123456",
                true, false))
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);
            A.CallTo(() => _tokenService.CreateToken(fakeCustomer, _userManager))
                .Returns("fake-jwt-token");

            // Act
            var result = await _accountController.Verify2faCode(new Verify2FACodeDto { Code = "123456"});

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var newCustomerDto = okResult.Value as NewCustomerDto;
            newCustomerDto.Should().NotBeNull();
            newCustomerDto.Username.Should().Be(fakeCustomer.UserName);
            newCustomerDto.Email.Should().Be(fakeCustomer.Email);
            newCustomerDto.PhoneNumber.Should().Be(fakeCustomer.PhoneNumber);
            newCustomerDto.Token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task Verify2faCode_ReturnsNotFound_WhenCustomerDoesNotSignedIn()
        {
            // Arrange
            A.CallTo(() => _signInManager.GetTwoFactorAuthenticationUserAsync())
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _accountController.Verify2faCode(new Verify2FACodeDto { Code = "123456" });

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Verify2faCode_ReturnsUnauthorized_WhenTwoFactorKeyIsVrong()
        {
            // Arrange
            A.CallTo(() => _signInManager.GetTwoFactorAuthenticationUserAsync())
                .Returns(fakeCustomer);
            A.CallTo(() => _signInManager.TwoFactorSignInAsync(
                "Email",
                "123456",
                true, false))
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _accountController.Verify2faCode(new Verify2FACodeDto { Code = "123456" });

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
        [Fact]
        public async Task ConfirmEmail_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync("42")).Returns(fakeCustomer);
            A.CallTo(() => _userManager.ConfirmEmailAsync(fakeCustomer, "123456"))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _accountController.ConfirmEmail("42", "123456");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsBadRequest_WhenUserIdIsNull()
        {
            // Arrange

            // Act
            var result = await _accountController.ConfirmEmail(null, "123456");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ConfirmEmail_ReturnsBadRequest_WhenTokenIsNull()
        {
            // Arrange

            // Act
            var result = await _accountController.ConfirmEmail("42", null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ConfirmEmail_ReturnsNotFound_WhenCannotFindUser()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync("42"))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _accountController.ConfirmEmail("42", "123456");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        //[Fact]
        public async Task ForgotPassword_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(fakeCustomer.Email))
                .Returns(Task.FromResult(fakeCustomer));
            A.CallTo(() => _userManager.IsEmailConfirmedAsync(fakeCustomer))
                .Returns(Task.FromResult(true));
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(fakeCustomer))
                .Returns(Task.FromResult("fake-password-reset-token"));
            A.CallTo(() => _accountController.Url.Action(A<UrlActionContext>.That.Matches(u => 
                u.Action == nameof(AccountController.ResetPassword) && u.Controller == "Account")))
                .Returns("http://example.com/resetPassword?token=test-token&email=test@example.com");

            // Act
            var result = await _accountController.ForgotPassword(
                new ForgotPasswordDto { Email = fakeCustomer.Email});

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            A.CallTo(() => _emailSender.SendEmailAsync(
                fakeCustomer.Email, 
                "Reset Password", 
                A<string>.That.Contains("http://example.com/resetPassword?token=test-token&email=test@example.com")))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync("testEmail@test.com"))
                .Returns(fakeCustomer);
            A.CallTo(() => _userManager.ResetPasswordAsync(A<Customer>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _accountController.ResetPassword(new ResetPasswordDto
            {
                Email = "testEmail@test.com",
                Password = "12345_Qwerty",
                ConfirmPassword = "12345_Qwerty",
                Token = "test-token"
            });

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ResetPassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync("testEmail@test.com"))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _accountController.ResetPassword(new ResetPasswordDto
            {
                Email = "testEmail@test.com",
                Password = "12345_Qwerty",
                ConfirmPassword = "12345_Qwerty",
                Token = "test-token"
            });

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenCanNotResetPassword()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync("testEmail@test.com"))
                .Returns(fakeCustomer);
            A.CallTo(() => _userManager.ResetPasswordAsync(A<Customer>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Failed());

            // Act
            var result = await _accountController.ResetPassword(new ResetPasswordDto
            {
                Email = "testEmail@test.com",
                Password = "12345_Qwerty",
                ConfirmPassword = "12345_Qwerty",
                Token = "test-token"
            });

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
