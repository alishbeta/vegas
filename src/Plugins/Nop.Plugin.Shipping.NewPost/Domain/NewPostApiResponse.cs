using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost.Domain
{
    public class NewPostApiResponse<T>
    {
        public bool success { get; set; }
        public T data { get; set; }
        public object errors { get; set; }
        public object warnings { get; set; }
        public object info { get; set; }
        public object messageCodes { get; set; }
        public object errorCodes { get; set; }
        public object warningCodes { get; set; }
        public object infoCodes { get; set; }
    }
}
