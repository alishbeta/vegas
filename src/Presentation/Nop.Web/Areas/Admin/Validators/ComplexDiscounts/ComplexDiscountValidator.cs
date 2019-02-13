using FluentValidation;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.ComplexDiscounts;
using Nop.Core.Domain.Discounts;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Validators.ComplexDiscount
{
    public partial class ComplexDiscountValidator : BaseNopValidator<ComplexDiscountModel>
    {
        public ComplexDiscountValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.ComplexDiscounts.Fields.Name.Required"));
            RuleFor(x => x.DiscountPercent).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.ComplexDiscounts.Fields.DiscountPercent.Required"));

            SetDatabaseValidationRules<Core.Domain.Discounts.ComplexDiscount>(dbContext);
        }
    }
}
