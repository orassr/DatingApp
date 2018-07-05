using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Data;
using DatingApp.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }
#endregion
    }
}