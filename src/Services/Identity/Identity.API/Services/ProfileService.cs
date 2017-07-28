using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Identity.API.Models;
using System.Security.Claims;
using IdentityModel;

namespace Identity.API.Services
{
    /// <summary>
    /// 用户个人信息服务
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 获取用户配置信息的异步方法。
        /// 说明：每当请求用户请求时，都会调用此方法。
        /// </summary>
        /// <param name="context">配置数据请求上下文</param>
        /// <returns></returns>
        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == JwtClaimTypes.Subject).FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();
        }

        /// <summary>
        /// 每当身份服务器需要确定用户是否有效或激活时，将调用此方法。
        /// </summary>
        /// <param name="context">是否存活上下文</param>
        /// <returns></returns>
        async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == JwtClaimTypes.Subject).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        private IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
                claims.Add(new Claim("name", user.Name));

            if (!string.IsNullOrWhiteSpace(user.LastName))
                claims.Add(new Claim("last_name", user.LastName));

            if (!string.IsNullOrWhiteSpace(user.CardNumber))
                claims.Add(new Claim("card_number", user.CardNumber));

            if (!string.IsNullOrWhiteSpace(user.CardHolderName))
                claims.Add(new Claim("card_holder", user.CardHolderName));

            if (!string.IsNullOrWhiteSpace(user.SecurityNumber))
                claims.Add(new Claim("card_security_number", user.SecurityNumber));

            if (!string.IsNullOrWhiteSpace(user.Expiration))
                claims.Add(new Claim("card_expiration", user.Expiration));

            if (!string.IsNullOrWhiteSpace(user.City))
                claims.Add(new Claim("address_city", user.City));

            if (!string.IsNullOrWhiteSpace(user.Country))
                claims.Add(new Claim("address_country", user.Country));

            if (!string.IsNullOrWhiteSpace(user.State))
                claims.Add(new Claim("address_state", user.State));

            if (!string.IsNullOrWhiteSpace(user.Street))
                claims.Add(new Claim("address_street", user.Street));

            if (!string.IsNullOrWhiteSpace(user.ZipCode))
                claims.Add(new Claim("address_zip_code", user.ZipCode));

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            return claims;
        }
    }
}
