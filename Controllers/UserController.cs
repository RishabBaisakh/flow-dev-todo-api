using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.DTOs.User;
using TodoApi.Services; // make sure this is included

namespace TodoApi.Controllers
{
	[ApiController]
	[Route("users")]
	public class UserController : ControllerBase
	{
		private readonly UserService _usersService;

		public UserController(UserService usersService)
		{
			_usersService = usersService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
		{
			var users = await _usersService.GetAsync();
			var result = users.Select(u => new UserDto
			{
					Id = u.Id,
					Name = u.Name,
					Avatar = u.Avatar
			});
			return Ok(result);
		}

		[HttpGet("{id:length(24)}")]
		public async Task<ActionResult<UserDto>> GetById(string id)
		{
			var user = await _usersService.GetByIdAsync(id);
			if (user == null) return NotFound();

			return Ok(new UserDto
			{
				Id = user.Id,
				Name = user.Name,
				Avatar = user.Avatar
			});
		}

		[HttpPost]
		public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
		{
			var user = new User
			{
				Name = dto.Name,
				Avatar = dto.Avatar
			};

			var created = await _usersService.CreateIfNotExistsAsync(user);

			if (!created)
				return BadRequest(new { message = "User with this name already exists" });

			var userDto = new UserDto
			{
				Id = user.Id,
				Name = user.Name,
				Avatar = user.Avatar
			};

			return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto);
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			await _usersService.DeleteAsync(id);
			return NoContent();
		}
	}
}
