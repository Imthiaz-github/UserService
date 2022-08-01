using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;

namespace UserService.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService; 
        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Get all the users.
        /// </summary>
        /// <returns>Users</returns>
        /// <remarks>
        /// - Tables used. => User
        /// </remarks>
        [HttpGet]
        //[Route("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var response = await _userService.GetUsers().ConfigureAwait(false);
            if(response == null)
            {
                return StatusCode(StatusCodes.Status200OK, "Users not found");
                //return NoContent();
            }
            return Ok(response);
        }

        /// <summary>
        /// Get user by Id.
        /// </summary>
        /// <returns>User</returns>
        /// <remarks>
        /// - Tables used. => User
        /// </remarks>
        [HttpGet("{id}", Name ="GetUserById")]
        //[Route("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            if(id <=0)
            {
                return NotFound();
            }
            var response = await _userService.GetUserById(id).ConfigureAwait(false);
            
            return response != null ? Ok(response) : NotFound();
        }

        /// <summary>
        /// Create User.
        /// </summary>
        /// <returns>User</returns>
        /// <remarks>
        /// - Tables used. => User
        /// </remarks>
        [HttpPost]
        //[Route("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var userExists = await _userService.UserExists(user.UserName);
            if(userExists)
            {
                return StatusCode(409, $"User '{user.UserName}' already exists.");
            }
            var response = await _userService.CreateUser(user).ConfigureAwait(false);
            return CreatedAtRoute(nameof(GetUserById), new { id = response.UserId }, response);
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <returns>true/false</returns>
        /// <remarks>
        /// - Tables used. => User
        /// </remarks>
        [HttpDelete("{id}", Name = "DeleteUser")]
        //[Route("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var userId = await _userService.GetUserById(id).ConfigureAwait(false);
            if (userId == null)
                return NotFound($"User with the id:{id} does not exist");
            return await _userService.DeleteUser(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Update user.
        /// </summary>
        /// <returns>true/false</returns>
        /// <remarks>
        /// - Tables used. => User
        /// </remarks>
        [HttpPut("{id}", Name = "UpdateUser")]
        //[Route("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (id <= 0)
            {
                return NotFound();
            }
            var userId = await _userService.GetUserById(id).ConfigureAwait(false);
            if (userId == null)
                return NotFound($"User with the id:{id} does not exist");
            return await _userService.UpdateUser(id, user).ConfigureAwait(false);
        }
    }
}
