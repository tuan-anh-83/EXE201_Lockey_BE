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

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
		public IActionResult GetUsers()
		{
			var users = _userRepository.GetUsers();

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(users);
		}

		[HttpGet("{userId}")]
		[ProducesResponseType(200, Type = typeof(User))]
		[ProducesResponseType(400)]
		public IActionResult GetUser(int userId)
		{
			if (!_userRepository.UserExists(userId))
				return NotFound();

			var users = _userRepository.GetUser(userId);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(users);
		}

		//API Create User
		[HttpPost]
		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(400)]
		public IActionResult CreateUser([FromBody] User user)
		{
			if (user == null)
				return BadRequest(ModelState);

			// Kiểm tra nếu user đã tồn tại
			var existingUser = _userRepository.GetUserByEmail(user.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("", "User already exists");
				return StatusCode(409, ModelState); // 409 Conflict
			}

			if (!_userRepository.CreateUser(user))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState); // 500 Internal Server Error
			}

			return Ok("Successfully created"); // Trả về 200 OK cùng với thông báo
		}


        [HttpPost("login")]
        public IActionResult Login(UserDto accountDTO)
        {
            var user = _userRepository.GetUserByEmail(accountDTO.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(accountDTO.Password, user.Password))
            {
                return BadRequest("Invalid username or password");
            }
/*
            ///Dong Hai # Save value originalPassword  to use method view profile
            ///
            Response.Cookies.Append("originalPassword", accountDTO.Password);
            ///end
            ///*/
			var userRole = _userRepository.GetUserByEmail(accountDTO.Email).Role;
            var jwt = _jwtService.Generate(user.Id, userRole);

            var userDto = new
            {
                accessToken = jwt, 
				email = user.Email
            };

            return Ok(userDto);
        }

    }
}
