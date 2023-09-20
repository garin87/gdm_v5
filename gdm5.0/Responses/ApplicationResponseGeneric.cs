using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Shared.Enums;

namespace gdm5._0.Responses
{
    public class ApplicationResponseGeneric<T> : ApplicationResponse
    {

        public ApplicationResponseGeneric() : base() { }

        public ApplicationResponseGeneric(StatusCodeEnum statusCode) : base(statusCode) { }

        /// <summary>
        /// Gets or sets response data.
        /// </summary>
        public T Data { get; set; }
    }
}
