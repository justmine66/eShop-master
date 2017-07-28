// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace Identity.API.Models.AccountViewModels
{
    /// <summary>
    /// 许可输入模型
    /// </summary>
    public class ConsentInputModel
    {
        /// <summary>
        /// 按钮
        /// </summary>
        public string Button { get; set; }
        /// <summary>
        /// 许可的范围
        /// </summary>
        public IEnumerable<string> ScopesConsented { get; set; }
        /// <summary>
        /// 是否记住许可
        /// </summary>
        public bool RememberConsent { get; set; }
        /// <summary>
        /// 返回URL
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}