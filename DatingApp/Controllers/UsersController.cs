using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    // Adding the LogUserActivity from the Startup.cs
    [ServiceFilter(typeof(LogUserActivity))]
    // All methods are protected and required Authorization.
    [Authorize]
    // Make the controller be available by the MVC.
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
#region Fields
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
#endregion
#region Ctor
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._repo = repo;  
            this._mapper = mapper;
        }
#endregion
#region Methods
        /// <summary>
        /// Getting the users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        /// <summary>
        /// Getting User by unique id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // Getting the id of the current user
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(id);

            if (userFromRepo == null)
                return NotFound($"Could not find user with an ID of {id}");

            if (currentUserId != userFromRepo.Id)
                return Unauthorized();

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

#endregion
    }
}