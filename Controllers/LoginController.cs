using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryMSAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LibraryMSAPI.Controllers
{
    public class UserInfo
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string password { get; set; }
    }
    [Route("api/[controller]")]
    public class LoginController:Controller
    {
        public LibraryDbContext DbContext { get; }
        public LoginController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
        }
        // login  name&password
        [HttpPost]
        public async Task<string> Post([FromBody]UserInfo userInfo)
        {
            var name = userInfo.name;
            var password = userInfo.password;
            var thisUsers = await (from user in DbContext.Cards where user.name.Trim().Equals(name) select user).ToArrayAsync();
            if (thisUsers.Length == 0) return "no this user";
            if (thisUsers[0].active == 1) return "already dead";
            var _password =thisUsers[0].password.Trim();
            if (password.Equals(_password))
            {
                var result = "cc98" + name + "CC98" + password;
                MD5 md5 = new MD5CryptoServiceProvider();
                var output = BitConverter.ToString((md5.ComputeHash(Encoding.UTF8.GetBytes(result)))).Replace("-", "");
                var newA = new Data.Authorization(name, output);
                DbContext.Authorizations.Add(newA);
                await DbContext.SaveChangesAsync();
                return output;
            }
            else
            {
                return "password error";
            }
          
        }
   
    }
}
