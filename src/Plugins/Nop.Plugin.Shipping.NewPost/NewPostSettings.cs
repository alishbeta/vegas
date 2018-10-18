using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost
{
    public class NewPostSettings : ISettings
    {
        public string ApiKey { get; set; }
        public string Url { get; internal set; }
    }
}
