using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// EF登录服务
    /// </summary>
    public class EFLoginService : ILoginService<ApplicationUser>
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;

        public EFLoginService(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        /// <param name="user">用户名</param>
        /// <returns>用户对象</returns>
        public async Task<ApplicationUser> FindByUsername(string user)
        {
            return await _userManager.FindByEmailAsync(user);
        }

        /// <summary>
        /// 验证凭据
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<bool> ValidateCredentials(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns></returns>
        public Task SignIn(ApplicationUser user)
        {
            return _signInManager.SignInAsync(user, true);
        }
    }
}
