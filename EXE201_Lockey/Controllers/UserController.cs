using EXE201_Lockey.Data;
using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using EXE201_Lockey.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXE201_Lockey.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly DataContext _dataContext;
		private readonly JWTService _jwtService;

		public UserController(IUserRepository userRepository, DataContext dataContext, JWTService jWTService)
		{
			_userRepository = userRepository;
			_dataContext = dataContext;
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
				Address = user.Address
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
				Id=user.Id,
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
				return BadRequest(ModelState);

			// Kiểm tra nếu user đã tồn tại
			var existingUser = _userRepository.GetUserByEmail(userDto.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("", "User already exists");
				return StatusCode(409, ModelState); // 409 Conflict
			}

			// Hash mật khẩu trước khi lưu
			var user = new User
			{
				Name = userDto.Name,
				Email = userDto.Email,
				Password = userDto.Password,
				Phone = userDto.Phone,
				Address = userDto.Address,
				Role = "Customer" // Mặc định vai trò là khách hàng
			};

			if (!_userRepository.CreateUser(user))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState); // 500 Internal Server Error
			}

			return Ok("Successfully created"); // Trả về 200 OK cùng với thông báo
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
				user.Password = userDto.Password;
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


	}

}
