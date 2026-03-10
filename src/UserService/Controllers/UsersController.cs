using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var user = await _userService.GetAllAsync();
            return Ok(user);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null) return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(CreateUserRequest request)
        {
            var (user, error) = await _userService.CreateAsync(request);

            if (error is not null) return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = user!.Id }, user);

        }


        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, CreateUserRequest request)
        {
            var user = await _userService.UpdateAsync(id, request);

            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id) {


            var user = await _userService.DeleteAsync(id);
            if (!user) return NotFound();

            return Ok(user);
        
        
        }

    }
}
