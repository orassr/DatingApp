using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : Controller
    {
#region Fields
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaeyConfig;
        private Cloudinary _cloudinaey;
#endregion
#region Ctor
        public PhotosController(IDatingRepository repo, 
            IMapper mapper,
            IOptions<CloudinarySettings> cloudinaeyConfig)
            {
                _repo = repo;
                _mapper = mapper;
                _cloudinaeyConfig = cloudinaeyConfig;

                // Initializing a  new Cloudinary account
                Account acc =  new Account(
                    _cloudinaeyConfig.Value.CloudName,
                    _cloudinaeyConfig.Value.ApiKey,
                    _cloudinaeyConfig.Value.ApiSecret
                );

                _cloudinaey = new Cloudinary(acc);
            }
#endregion
#region  Methods
            [HttpGet("{id}", Name = "GetPhoto")]
            public async Task<IActionResult> GetPhoto(int id)
            {
                var photoFromRepo = _repo.GetPhoto(id);

                var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

                return Ok(photo);
            }

            [HttpPost]
            public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoDto)
            {
                var user = await _repo.GetUser(userId);

                if(user == null)
                    return BadRequest("Could not find the user");
                
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if(currentUserId != user.Id)
                    return Unauthorized();

                var file = photoDto.File;

                var uploadResults = new ImageUploadResult();

                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream)
                        };

                        uploadResults = _cloudinaey.Upload(uploadParams);
                    }
                }

                photoDto.Url = uploadResults.Uri.ToString();
                photoDto.PublicId = uploadResults.PublicId;

                var photo = _mapper.Map<Photo>(photoDto);
                photo.User = user;

                if(!user.Photos.Any(m => m.IsMain))
                    photo.IsMain = true;

                user.Photos.Add(photo);

                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

                if(await _repo.SaveAll())
                {
                    return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
                }

                return BadRequest("Could not add the photo");
            }

            [HttpPost("{id}/setMain")]
            public async Task<IActionResult> SetMainPhoto(int userId, int id)
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    return Unauthorized();

                var photoFromRepo = await _repo.GetPhoto(id);
                if (photoFromRepo == null)
                    return NotFound();
                
                if (photoFromRepo.IsMain)
                    return BadRequest("This is already the main photo");

                var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
                if (currentMainPhoto != null)
                    currentMainPhoto.IsMain = false;
                
                photoFromRepo.IsMain = true;

                if(await _repo.SaveAll())
                    return NoContent();
                
                return BadRequest("Could not set photo to main");
            }

#endregion
    }
}