using Api.DTOs;
using Api.Entities;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AccountController(IMapper mapper, IUserRepository userRepository, ITokenService tokenService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user is null)
                return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }

            var userResult = new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs
            };

            return Ok(userResult);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Username))
                return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _userRepository.Add(user);
            await _userRepository.SaveAllAsync();

            var userResult = new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };

            return Ok(userResult);
        }

        private async Task<bool> UserExists(string username)
        {
            return (await _userRepository.GetMembersAsync()).Any(u => u.Username == username.ToLower());
        }
    }
}
