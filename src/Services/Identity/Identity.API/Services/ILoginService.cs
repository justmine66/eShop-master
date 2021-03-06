﻿using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// 登录服务
    /// </summary>
    /// <typeparam name="TUser">用户类型</typeparam>
    public interface ILoginService<TUser>
    {
        /// <summary>
        /// 验证凭据
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<bool> ValidateCredentialsAsync(TUser user, string password);

        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        /// <param name="user">用户名</param>
        /// <returns>用户对象</returns>
        Task<TUser> FindByUsernameAsync(string user);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns></returns>
        Task SignInAsync(TUser user);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="AuthenticationProperties">用户对象</param>
        /// <returns></returns>
        Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties);
    }
}
