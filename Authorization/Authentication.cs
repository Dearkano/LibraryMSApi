using LibraryMSAPI.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace LibraryMSAPI.Authorization
{

    //public class AuthorizationOptions : AuthenticationOptions { }
    public class Authentication : AuthenticationHandler<AuthenticationSchemeOptions>

    {
        public LibraryDbContext dbContext { get; }
        public Authentication(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,LibraryDbContext libraryDbContext) : base(options, logger, encoder, clock)
        {
            dbContext = libraryDbContext; 
        }
        
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["Authorization"];
            var data = await (from i in dbContext.Authorizations where token.Equals(i.token) select i).ToArrayAsync();
            if (data.Length == 0) return AuthenticateResult.Fail(new Exception ());
            var name = data[0].name;
            var data1 = await (from j in dbContext.Cards where j.name.Equals(name) select j).ToArrayAsync();
            var claim = new Claim(ClaimTypes.Name, name);
            var claims = new Claim[1] {claim};
            var identity = new ClaimsIdentity(claims, "DefaultAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationProperties = new AuthenticationProperties();
            string scheme = "DefaultAuthentication";
            var authenticationTicket = new AuthenticationTicket(claimsPrincipal, authenticationProperties, scheme);
            return AuthenticateResult.Success(authenticationTicket);
        }
    }
}
