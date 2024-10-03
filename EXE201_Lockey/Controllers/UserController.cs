using EXE201_Lockey.Data;
using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using EXE201_Lockey.Service;
using EXE201_Lockey.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXE201_Lockey.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly IEmailService _emailService;
		private readonly JWTService _jwtService;

		public UserController(IUserRepository userRepository, IEmailService emailService, JWTService jWTService)
		{
			_userRepository = userRepository;
			_emailService = emailService;
			_jwtService = jWTService;
		}

		// API Get all users
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
		public IActionResult GetUsers()
		{
			var users = _userRepository.GetUsers().Select(user => new UserDto
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				Phone = user.Phone,
				Address = user.Address,
				Role = user.Role,
				// Không trả về mật khẩu
			});

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(users);
		}

		// API Get a specific user by userId
		[HttpGet("{userId}")]
		[ProducesResponseType(200, Type = typeof(UserDto))]
		[ProducesResponseType(400)]
		public IActionResult GetUser(int userId)
		{
			if (!_userRepository.UserExists(userId))
				return NotFound();

			var user = _userRepository.GetUser(userId);

			var userDto = new UserDto
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				Phone = user.Phone,
				Address = user.Address
				// Không trả về mật khẩu
			};

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(userDto);
		}

		// API đăng ký người dùng mới
		[HttpPost]
		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(400)]
		public IActionResult CreateUser([FromBody] UserDto userDto)
		{
			if (userDto == null)
			{
				return BadRequest(ModelState);
			}

			var existingUser = _userRepository.GetUserByEmail(userDto.Email);
			if (existingUser != null)
			{
				return StatusCode(409, "User already exists");
			}

			var user = new User
			{
				Name = userDto.Name,
				Email = userDto.Email,
				Password = userDto.Password,
				Phone = userDto.Phone,
				Address = userDto.Address
			};

			if (!_userRepository.CreateUser(user))
			{
				return StatusCode(500, "Something went wrong while creating the user.");
			}

			return Ok("Signup successful. Please check your email for the OTP to verify your account.");
		}

		// API Đăng nhập
		[HttpPost("login")]
		public IActionResult Login(LoginDto accountDTO)
		{
			var user = _userRepository.GetUserByEmail(accountDTO.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(accountDTO.Password, user.Password))
			{
				return BadRequest("Invalid username or password");
			}

			var userRole = user.Role;
			var jwt = _jwtService.Generate(user.Id, userRole);

			var userDto = new
			{
				accessToken = jwt,
				email = user.Email
			};

			return Ok(userDto);
		}

		// API Update User
		[HttpPut("{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult UpdateUser(int userId, [FromBody] UserDto userDto)
		{
			if (userDto == null || userId != userDto.Id)
			{
				return BadRequest(ModelState); // Trả về 400 nếu dữ liệu không hợp lệ
			}

			if (!_userRepository.UserExists(userId))
			{
				return NotFound(); // Trả về 404 nếu không tìm thấy người dùng
			}

			// Lấy thông tin người dùng hiện tại từ cơ sở dữ liệu
			var user = _userRepository.GetUser(userId);
			if (user == null)
			{
				return NotFound(); // Trả về 404 nếu không tìm thấy người dùng
			}

			// Chỉ cập nhật những trường không null hoặc không trống
			user.Name = string.IsNullOrEmpty(userDto.Name) ? user.Name : userDto.Name;
			user.Email = string.IsNullOrEmpty(userDto.Email) ? user.Email : userDto.Email;
			user.Phone = string.IsNullOrEmpty(userDto.Phone) ? user.Phone : userDto.Phone;
			user.Address = string.IsNullOrEmpty(userDto.Address) ? user.Address : userDto.Address;

			// Cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
			if (!string.IsNullOrEmpty(userDto.Password))
			{
				user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
			}

			// Gọi phương thức cập nhật từ repository
			if (!_userRepository.UpdateUser(user))
			{
				ModelState.AddModelError("", "Something went wrong updating the user");
				return StatusCode(500, ModelState); // Trả về 500 nếu gặp lỗi khi cập nhật
			}

			return Ok("User updated successfully");
		}

		// API Delete user
		[HttpDelete("{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult DeleteUser(int userId)
		{
			if (!_userRepository.UserExists(userId))
			{
				return NotFound();
			}

			var user = _userRepository.GetUser(userId);

			if (!_userRepository.DeleteUser(user))
			{
				ModelState.AddModelError("", "Something went wrong while deleting the user");
				return StatusCode(500, ModelState);
			}

			return Ok("User deleted successfully");
		}

		// API Forgot Password
		[HttpPost("forgot-password")]
		public IActionResult ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
		{
			if (forgotPasswordDto == null || string.IsNullOrEmpty(forgotPasswordDto.Email))
			{
				return BadRequest("Invalid request.");
			}

			var user = _userRepository.GetUserByEmail(forgotPasswordDto.Email);
			if (user == null)
			{
				return Ok(new { Message = "If the email is registered, a password reset link will be sent." });
			}

			// Tạo token cho reset password, có thể sử dụng JWT hoặc mã ngẫu nhiên
			var token = Guid.NewGuid().ToString();

			// Lưu token trong cơ sở dữ liệu
			_userRepository.SavePasswordResetToken(user.Id, token);

			// Tạo URL reset password (bạn có thể gửi email kèm theo đường dẫn này)
			var resetUrl = $"http://localhost:3000/reset-password?token={token}&email={user.Email}";

			// Gửi email reset password
			_emailService.SendPasswordResetEmail(user.Email, resetUrl);

			return Ok(new { Message = "If the email is registered, a password reset link will be sent." });
		}

		// API Reset Password
		[HttpPost("reset-password")]
		public IActionResult ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
		{
			if (resetPasswordDto == null || string.IsNullOrEmpty(resetPasswordDto.Email) || string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
			{
				return BadRequest("Invalid request.");
			}

			var storedToken = _userRepository.GetPasswordResetTokenByEmail(resetPasswordDto.Email);
			if (storedToken == null || storedToken != resetPasswordDto.Token)
			{
				return BadRequest("Invalid or expired token.");
			}

			// Đặt lại mật khẩu
			var result = _userRepository.ResetPassword(resetPasswordDto.Email, resetPasswordDto.NewPassword);
			if (!result)
			{
				return StatusCode(500, "Error resetting password.");
			}

			return Ok(new { Message = "Password reset successfully." });
		}

		[HttpPost("verify-otp")]
		public IActionResult VerifyOtp([FromBody] OtpVerificationDto otpDto)
		{
			if (otpDto == null || string.IsNullOrEmpty(otpDto.Otp))
			{
				return BadRequest("Invalid request.");
			}

			var user = _userRepository.GetUserByOtp(otpDto.Otp);  // Get user by OTP
			if (user == null)
			{
				return NotFound("User not found or OTP invalid.");
			}

			// If OTP matches, mark the user as verified
			user.IsVerified = true;
			user.Otp = null; // Remove OTP after verification
			_userRepository.UpdateUser(user);

			return Ok("Account verified successfully.");
		}
	}


}
