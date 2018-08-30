using Nop.Core.Domain.Topics;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Topics
{
    public partial class TopicCategoryModel : BaseNopEntityModel, ILocalizedModel<TopicCategoryLocalizedModel>
    {
        public TopicCategoryModel()
        {
            Locales = new List<TopicCategoryLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.Title")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.Parent")]
        public int ParentId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        public IList<TopicCategoryLocalizedModel> Locales { get; set; }
        public IList<TopicCategory> TopicCategories { get; set; }
    }

    public partial class TopicCategoryLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.Title")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.TopicCategories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }
    }
}
