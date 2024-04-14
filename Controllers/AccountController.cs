using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //UserManager là một lớp quản lý người dùng
        //chịu trách nhiệm quản lý thông tin người dùng vd Crud user information
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        //Login thì sẽ là post
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userNameExists = await _userManager.Users.FirstOrDefaultAsync(uName => uName.UserName == loginDto.UserName.ToLower());
            if (userNameExists == null)
            {
                return Unauthorized("Invalid User name");
            }

            //Giá trị false trong đoạn mã chỉ là để báo cho phương thức
            //kiểm tra xác thực rằng bạn không quan tâm đến trạng thái 
            //khóa của tài khoản trong quá trình xác thực. không ảnh hưởng đến result.Succeeded 
            var result = await _signInManager.CheckPasswordSignInAsync(userNameExists, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("User name not found or/and password incorrect");
            }
            return Ok(
                new NewUserDto
                {
                    UserName = userNameExists.UserName,
                    Email = userNameExists.Email,
                    Token = _tokenService.CreateToken(userNameExists)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = registerDto.UserName,
                                Email = registerDto.Email,
                                Token = _tokenService.CreateToken(appUser)
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                //Nếu gặp bất kỳ lỗi nào khác
                return StatusCode(500, ex);
            }
        }
    }
}