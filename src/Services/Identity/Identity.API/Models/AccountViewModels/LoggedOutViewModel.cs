// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Identity.API.Models.AccountViewModels
{
    /// <summary>
    /// 注销视图模型
    /// </summary>
    public class LoggedOutViewModel
    {
        /// <summary>
        /// 注销后，重定向Uri
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SignOutIframeUrl { get; set; }
    }
}