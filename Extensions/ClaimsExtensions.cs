using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Models;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        //Cách tiếp cận các claims
        //Phương thức này có mục đích là trả về username từ các Claims của người dùng từ cái tokenservice
        public static string GetUserName(this ClaimsPrincipal user){
            return user.Claims.SingleOrDefault(x=>x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }
    }
}