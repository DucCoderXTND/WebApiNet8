using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config; //IConfiguration để đọc cấu hình từ file appsettings.json
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;

            //Được sử dụng để tạo một khóa đối xứng (Symmetric Key) từ một chuỗi byte,
            //và khóa này sẽ được sử dụng để ký (sign) và giải mã (verify) token JWT.
            //Mục tiêu chính là đảm bảo tính toàn vẹn và xác thực của token.
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Signingkey"]));
        }
        public string CreateToken(AppUser user)
        {
            //Đoạn mã này tạo một (List<Claim>) các claims để được sử 
            //dụng trong quá trình tạo JWT khi người dùng đăng nhập hoặc xác thực.
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            //Sử dụng trong tạo mã token tokenDescriptor đảm bảo rằng token
            //sẽ được ký bằng khóa và thuật toán đã được cấu hình.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //Tạo mã token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}