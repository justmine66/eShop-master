﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Exceptions
{
    /// <summary>
    /// 对于目录应用程序域的异常类型
    /// </summary>
    public class CatalogDomainException : Exception
    {
        public CatalogDomainException()
        { }

        public CatalogDomainException(string message)
            : base(message)
        { }

        public CatalogDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
