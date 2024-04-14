using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    //Xử lý dịch vụ mã thông báo token
    public interface ITokenService
    {
        //chuyển người dùng đến ứng dụng của mình
        string CreateToken(AppUser user);
    }
}