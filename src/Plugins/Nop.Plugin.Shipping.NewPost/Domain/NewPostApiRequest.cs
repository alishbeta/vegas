using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost.Domain
{
    public class NewPostApiRequest<T>
    {
        public string apiKey { get; set; }
        public string modelName { get; set; }
        public string calledMethod { get; set; }
        public T methodProperties { get; set; }
    }
}
