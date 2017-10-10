using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Identity.API.Services
{
    /// <summary>
    /// EF登录服务
    /// </summary>
    public class EFLoginService : ILoginService<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EFLoginService(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        /// <param name="user">用户名</param>
        /// <returns>用户对象</returns>
        public async Task<ApplicationUser> FindByUsernameAsync(string user)
        {
            return await this._userManager.FindByEmailAsync(user);
        }

        /// <summary>
        /// 验证凭据
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<bool> ValidateCredentialsAsync(ApplicationUser user, string password)
        {
            return await this._userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns></returns>
        public Task SignInAsync(ApplicationUser user)
        {
            return this._signInManager.SignInAsync(user, true);
        }

        public Task SignInAsync(ApplicationUser user, AuthenticationProperties authenticationProperties)
        {
            return this._signInManager.SignInAsync(user, authenticationProperties);
        }
    }
}
