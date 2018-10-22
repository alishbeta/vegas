using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.NewPost.Models
{
    public class NewPostModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.NewPost.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.NewPost.Fields.Url")]
        public string Url { get; set; }
    }
}
