﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.OneC;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using OfficeOpenXml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Constants

        //it's quite fast hash (to cheaply distinguish between objects)
        private const string IMAGE_HASH_ALGORITHM = "SHA1";

        private const string UPLOADS_TEMP_PATH = "~/App_Data/TempUploads";

        #endregion

        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDataProvider _dataProvider;
        private readonly IDateRangeService _dateRangeService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IShippingService _shippingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public ImportManager(CatalogSettings catalogSettings,
            ICategoryService categoryService,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            IDataProvider dataProvider,
            IDateRangeService dateRangeService,
            IEncryptionService encryptionService,
            ILocalizationService localizationService,
            ILogger logger,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INopFileProvider fileProvider,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IServiceScopeFactory serviceScopeFactory,
            IShippingService shippingService,
            ISpecificationAttributeService specificationAttributeService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxCategoryService taxCategoryService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService)
        {
            this._catalogSettings = catalogSettings;
            this._categoryService = categoryService;
            this._countryService = countryService;
            this._customerActivityService = customerActivityService;
            this._dataProvider = dataProvider;
            this._dateRangeService = dateRangeService;
            this._encryptionService = encryptionService;
            this._fileProvider = fileProvider;
            this._localizationService = localizationService;
            this._logger = logger;
            this._manufacturerService = manufacturerService;
            this._measureService = measureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._pictureService = pictureService;
            this._productAttributeService = productAttributeService;
            this._productService = productService;
            this._productTagService = productTagService;
            this._productTemplateService = productTemplateService;
            this._serviceScopeFactory = serviceScopeFactory;
            this._shippingService = shippingService;
            this._specificationAttributeService = specificationAttributeService;
            this._stateProvinceService = stateProvinceService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._taxCategoryService = taxCategoryService;
            this._urlRecordService = urlRecordService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
            this._vendorSettings = vendorSettings;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Utilities

        private static ExportedAttributeType GetTypeOfExportedAttribute(ExcelWorksheet worksheet, PropertyManager<ExportProductAttribute> productAttributeManager, PropertyManager<ExportSpecificationAttribute> specificationAttributeManager, int iRow)
        {
            productAttributeManager.ReadFromXlsx(worksheet, iRow, ExportProductAttribute.ProducAttributeCellOffset);

            if (productAttributeManager.IsCaption)
            {
                return ExportedAttributeType.ProductAttribute;
            }

            specificationAttributeManager.ReadFromXlsx(worksheet, iRow, ExportProductAttribute.ProducAttributeCellOffset);

            if (specificationAttributeManager.IsCaption)
            {
                return ExportedAttributeType.SpecificationAttribute;
            }

            return ExportedAttributeType.NotSpecified;
        }

        private static void SetOutLineForSpecificationAttributeRow(object cellValue, ExcelWorksheet worksheet, int endRow)
        {
            var attributeType = (cellValue ?? string.Empty).ToString();

            if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
            {
                worksheet.Row(endRow).OutlineLevel = 1;
            }
            else
            {
                if (SpecificationAttributeType.Option.ToSelectList(useLocalization: false)
                    .Any(p => p.Text.Equals(attributeType, StringComparison.InvariantCultureIgnoreCase)))
                    worksheet.Row(endRow).OutlineLevel = 1;
                else if (int.TryParse(attributeType, out var attributeTypeId) && Enum.IsDefined(typeof(SpecificationAttributeType), attributeTypeId))
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        private static void CopyDataToNewFile(ImportProductMetadata metadata, ExcelWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
        {
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var outWorksheet = xlPackage.Workbook.Worksheets.Add(typeof(Product).Name);
                    metadata.Manager.WriteCaption(outWorksheet);
                    var outRow = 2;
                    for (var row = startRow; row <= endRow; row++)
                    {
                        outWorksheet.Row(outRow).OutlineLevel = worksheet.Row(row).OutlineLevel;
                        for (var cell = 1; cell <= endCell; cell++)
                        {
                            outWorksheet.Cells[outRow, cell].Value = worksheet.Cells[row, cell].Value;
                        }

                        outRow += 1;
                    }

                    xlPackage.Save();
                }
            }
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            for (var i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);
            
            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>The image or null if the image has not changed</returns>
        protected virtual Picture LoadPicture(string picturePath, string name, int? picId = null)
        {
            if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = _fileProvider.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);
                if (existingPicture != null)
                {
                    var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                    //picture binary after validation (like in database)
                    var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                    if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                        existingBinary.SequenceEqual(newPictureBinary))
                    {
                        pictureAlreadyExists = true;
                    }
                }
            }

            if (pictureAlreadyExists) return null;

            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(name));
            return newPicture;
        }

        private void LogPictureInsertError(string picturePath, Exception ex)
        {
            var extension = _fileProvider.GetFileExtension(picturePath);
            var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

            var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
            var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;
            _logger.Error($"Insert picture failed (file name: {fileName})", ex);
        }

        protected virtual void ImportProductImagesUsingServices(IList<ProductPictureMetadata> productPictureMetadata)
        {
            foreach (var product in productPictureMetadata)
            {
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    var newPictureBinary = _fileProvider.ReadAllBytes(picturePath);
                    var pictureAlreadyExists = false;
                    if (!product.IsNew)
                    {
                        //compare with existing product pictures
                        var existingPictures = _pictureService.GetPicturesByProductId(product.ProductItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            break;
                        }
                    }

                    if (pictureAlreadyExists)
                        continue;

                    try
                    {
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                    catch (Exception ex)
                    {
                        LogPictureInsertError(picturePath, ex);
                    }
                }
            }
        }

        protected virtual void ImportProductImagesUsingHash(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash - 1;
            var productsImagesIds = _productService.GetProductsImagesIds(allProductsBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(productsImagesIds.SelectMany(p => p.Value).ToArray());

            foreach (var product in productPictureMetadata)
            {
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;
                    try
                    {
                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = _fileProvider.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!product.IsNew)
                        {
                            var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray(),
                                IMAGE_HASH_ALGORITHM);
                            var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType)
                                .Take(takeCount)
                                .ToArray(), IMAGE_HASH_ALGORITHM);

                            var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                                ? productsImagesIds[product.ProductItem.Id]
                                : new int[0];

                            pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key))
                                .Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                        }

                        if (pictureAlreadyExists)
                            continue;

                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                    catch (Exception ex)
                    {
                        LogPictureInsertError(picturePath, ex);
                    }
                }
            }
        }

        protected virtual string UpdateCategoryByXlsx(Category category, PropertyManager<Category> manager, Dictionary<string, Category> allCategories, bool isNew, out bool isParentCategoryExists)
        {
            var seName = string.Empty;
            isParentCategoryExists = true;
            var isParentCategorySet = false;

            foreach (var property in manager.GetProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        category.Name = property.StringValue.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                        break;
                    case "Description":
                        category.Description = property.StringValue;
                        break;
                    case "CategoryTemplateId":
                        category.CategoryTemplateId = property.IntValue;
                        break;
                    case "MetaKeywords":
                        category.MetaKeywords = property.StringValue;
                        break;
                    case "MetaDescription":
                        category.MetaDescription = property.StringValue;
                        break;
                    case "MetaTitle":
                        category.MetaTitle = property.StringValue;
                        break;
                    case "ParentCategoryId":
                        if (!isParentCategorySet)
                        {
                            var parentCategory = allCategories.Values.FirstOrDefault(c => c.Id == property.IntValue);
                            isParentCategorySet = parentCategory != null;

                            isParentCategoryExists = isParentCategorySet || property.IntValue == 0;

                            category.ParentCategoryId = parentCategory?.Id ?? property.IntValue;
                        }

                        break;
                    case "ParentCategoryName":
                        if (_catalogSettings.ExportImportCategoriesUsingCategoryName && !isParentCategorySet)
                        {
                            var categoryName = manager.GetProperty("ParentCategoryName").StringValue;
                            if (!string.IsNullOrEmpty(categoryName))
                            {
                                var parentCategory = allCategories.ContainsKey(categoryName)
                                    //try find category by full name with all parent category names
                                    ? allCategories[categoryName]
                                    //try find category by name
                                    : allCategories.Values.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCulture));

                                if (parentCategory != null)
                                {
                                    category.ParentCategoryId = parentCategory.Id;
                                    isParentCategorySet = true;
                                }
                                else
                                {
                                    isParentCategoryExists = false;
                                }
                            }
                        }

                        break;
                    case "Picture":
                        var picture = LoadPicture(manager.GetProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                        if (picture != null)
                            category.PictureId = picture.Id;
                        break;
                    case "PageSize":
                        category.PageSize = property.IntValue;
                        break;
                    case "AllowCustomersToSelectPageSize":
                        category.AllowCustomersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        category.PageSizeOptions = property.StringValue;
                        break;
                    case "PriceRanges":
                        category.PriceRanges = property.StringValue;
                        break;
                    case "ShowOnHomePage":
                        category.ShowOnHomePage = property.BooleanValue;
                        break;
                    case "IncludeInTopMenu":
                        category.IncludeInTopMenu = property.BooleanValue;
                        break;
                    case "Published":
                        category.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        category.DisplayOrder = property.IntValue;
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            category.UpdatedOnUtc = DateTime.UtcNow;
            return seName;
        }

        protected virtual Category GetCategoryFromXlsx(PropertyManager<Category> manager, ExcelWorksheet worksheet, int iRow, Dictionary<string, Category> allCategories, out bool isNew, out string curentCategoryBreadCrumb)
        {
            manager.ReadFromXlsx(worksheet, iRow);

            //try get category from database by ID
            var category = allCategories.Values.FirstOrDefault(c => c.Id == manager.GetProperty("Id")?.IntValue);

            if (_catalogSettings.ExportImportCategoriesUsingCategoryName && category == null)
            {
                var categoryName = manager.GetProperty("Name").StringValue;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    category = allCategories.ContainsKey(categoryName)
                        //try find category by full name with all parent category names
                        ? allCategories[categoryName]
                        //try find category by name
                        : allCategories.Values.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCulture));
                }
            }

            isNew = category == null;

            category = category ?? new Category();

            curentCategoryBreadCrumb = string.Empty;

            if (isNew)
            {
                category.CreatedOnUtc = DateTime.UtcNow;
                //default values
                category.PageSize = _catalogSettings.DefaultCategoryPageSize;
                category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                category.Published = true;
                category.IncludeInTopMenu = true;
                category.AllowCustomersToSelectPageSize = true;
            }
            else
                curentCategoryBreadCrumb = _categoryService.GetFormattedBreadCrumb(category);

            return category;
        }

        protected virtual void SaveCategory(bool isNew, Category category, Dictionary<string, Category> allCategories, string curentCategoryBreadCrumb, bool setSeName, string seName)
        {
            if (isNew)
                _categoryService.InsertCategory(category);
            else
                _categoryService.UpdateCategory(category);

            var categoryBreadCrumb = _categoryService.GetFormattedBreadCrumb(category);
            if (!allCategories.ContainsKey(categoryBreadCrumb))
                allCategories.Add(categoryBreadCrumb, category);
            if (!string.IsNullOrEmpty(curentCategoryBreadCrumb) && allCategories.ContainsKey(curentCategoryBreadCrumb) &&
                categoryBreadCrumb != curentCategoryBreadCrumb)
                allCategories.Remove(curentCategoryBreadCrumb);

            //search engine name
            if (setSeName)
                _urlRecordService.SaveSlug(category, _urlRecordService.ValidateSeName(category, seName, category.Name, true), 0);
        }

        protected virtual void SetOutLineForProductAttributeRow(object cellValue, ExcelWorksheet worksheet, int endRow)
        {
            try
            {
                var aid = Convert.ToInt32(cellValue ?? -1);

                var productAttribute = _productAttributeService.GetProductAttributeById(aid);

                if (productAttribute != null)
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
            catch (FormatException)
            {
                if ((cellValue ?? string.Empty).ToString() == "AttributeId")
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        protected virtual void ImportProductAttribute(PropertyManager<ExportProductAttribute> productAttributeManager, Product lastLoadedProduct)
        {
            if (!_catalogSettings.ExportImportProductAttributes || lastLoadedProduct == null || productAttributeManager.IsCaption)
                return;

            var productAttributeId = productAttributeManager.GetProperty("AttributeId").IntValue;
            var attributeControlTypeId = productAttributeManager.GetProperty("AttributeControlType").IntValue;

            var productAttributeValueId = productAttributeManager.GetProperty("ProductAttributeValueId").IntValue;
            var associatedProductId = productAttributeManager.GetProperty("AssociatedProductId").IntValue;
            var valueName = productAttributeManager.GetProperty("ValueName").StringValue;
            var attributeValueTypeId = productAttributeManager.GetProperty("AttributeValueType").IntValue;
            var colorSquaresRgb = productAttributeManager.GetProperty("ColorSquaresRgb").StringValue;
            var imageSquaresPictureId = productAttributeManager.GetProperty("ImageSquaresPictureId").IntValue;
            var priceAdjustment = productAttributeManager.GetProperty("PriceAdjustment").DecimalValue;
            var priceAdjustmentUsePercentage = productAttributeManager.GetProperty("PriceAdjustmentUsePercentage").BooleanValue;
            var weightAdjustment = productAttributeManager.GetProperty("WeightAdjustment").DecimalValue;
            var cost = productAttributeManager.GetProperty("Cost").DecimalValue;
            var customerEntersQty = productAttributeManager.GetProperty("CustomerEntersQty").BooleanValue;
            var quantity = productAttributeManager.GetProperty("Quantity").IntValue;
            var isPreSelected = productAttributeManager.GetProperty("IsPreSelected").BooleanValue;
            var displayOrder = productAttributeManager.GetProperty("DisplayOrder").IntValue;
            var pictureId = productAttributeManager.GetProperty("PictureId").IntValue;
            var textPrompt = productAttributeManager.GetProperty("AttributeTextPrompt").StringValue;
            var isRequired = productAttributeManager.GetProperty("AttributeIsRequired").BooleanValue;
            var attributeDisplayOrder = productAttributeManager.GetProperty("AttributeDisplayOrder").IntValue;

            var productAttributeMapping =
                lastLoadedProduct.ProductAttributeMappings.FirstOrDefault(
                    pam => pam.ProductAttributeId == productAttributeId);

            if (productAttributeMapping == null)
            {
                //insert mapping
                productAttributeMapping = new ProductAttributeMapping
                {
                    ProductId = lastLoadedProduct.Id,
                    ProductAttributeId = productAttributeId,
                    TextPrompt = textPrompt,
                    IsRequired = isRequired,
                    AttributeControlTypeId = attributeControlTypeId,
                    DisplayOrder = attributeDisplayOrder
                };
                _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
            }
            else
            {
                productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                productAttributeMapping.TextPrompt = textPrompt;
                productAttributeMapping.IsRequired = isRequired;
                productAttributeMapping.DisplayOrder = attributeDisplayOrder;
                _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
            }

            var pav = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);

            var attributeControlType = (AttributeControlType)attributeControlTypeId;

            if (pav == null)
            {
                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        return;
                }

                pav = new ProductAttributeValue
                {
                    ProductAttributeMappingId = productAttributeMapping.Id,
                    AttributeValueType = (AttributeValueType)attributeValueTypeId,
                    AssociatedProductId = associatedProductId,
                    Name = valueName,
                    PriceAdjustment = priceAdjustment,
                    PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage,
                    WeightAdjustment = weightAdjustment,
                    Cost = cost,
                    IsPreSelected = isPreSelected,
                    DisplayOrder = displayOrder,
                    ColorSquaresRgb = colorSquaresRgb,
                    ImageSquaresPictureId = imageSquaresPictureId,
                    CustomerEntersQty = customerEntersQty,
                    Quantity = quantity,
                    PictureId = pictureId
                };

                _productAttributeService.InsertProductAttributeValue(pav);
            }
            else
            {
                pav.AttributeValueTypeId = attributeValueTypeId;
                pav.AssociatedProductId = associatedProductId;
                pav.Name = valueName;
                pav.ColorSquaresRgb = colorSquaresRgb;
                pav.ImageSquaresPictureId = imageSquaresPictureId;
                pav.PriceAdjustment = priceAdjustment;
                pav.PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage;
                pav.WeightAdjustment = weightAdjustment;
                pav.Cost = cost;
                pav.CustomerEntersQty = customerEntersQty;
                pav.Quantity = quantity;
                pav.IsPreSelected = isPreSelected;
                pav.DisplayOrder = displayOrder;
                pav.PictureId = pictureId;

                _productAttributeService.UpdateProductAttributeValue(pav);
            }
        }
        
        private void ImportSpecificationAttribute(PropertyManager<ExportSpecificationAttribute> specificationAttributeManager, Product lastLoadedProduct)
        {
            if (!_catalogSettings.ExportImportProductSpecificationAttributes || lastLoadedProduct == null || specificationAttributeManager.IsCaption)
                return;

            var attributeTypeId = specificationAttributeManager.GetProperty("AttributeType").IntValue;
            var allowFiltering = specificationAttributeManager.GetProperty("AllowFiltering").BooleanValue;
            var specificationAttributeOptionId = specificationAttributeManager.GetProperty("SpecificationAttributeOptionId").IntValue;
            var productId = lastLoadedProduct.Id;
            var customValue = specificationAttributeManager.GetProperty("CustomValue").StringValue;
            var displayOrder = specificationAttributeManager.GetProperty("DisplayOrder").IntValue;
            var showOnProductPage = specificationAttributeManager.GetProperty("ShowOnProductPage").BooleanValue;

            //if specification attribute option isn't set, try to get first of possible specification attribute option for current specification attribute
            if (specificationAttributeOptionId == 0)
            {
                var specificationAttribute = specificationAttributeManager.GetProperty("SpecificationAttribute").IntValue;
                specificationAttributeOptionId = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(specificationAttribute)
                                                     .FirstOrDefault()?.Id ?? specificationAttributeOptionId;
            }

            var productSpecificationAttribute = specificationAttributeOptionId == 0
                ? null
                : _specificationAttributeService.GetProductSpecificationAttributes(productId, specificationAttributeOptionId).FirstOrDefault();

            var isNew = productSpecificationAttribute == null;

            if (isNew)
            {
                productSpecificationAttribute = new ProductSpecificationAttribute();
            }

            if (attributeTypeId != (int)SpecificationAttributeType.Option)
            {
                //we allow filtering only for "Option" attribute type
                allowFiltering = false;
            }

            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
            {
                customValue = null;
            }

            productSpecificationAttribute.AttributeTypeId = attributeTypeId;
            productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
            productSpecificationAttribute.ProductId = productId;
            productSpecificationAttribute.CustomValue = customValue;
            productSpecificationAttribute.AllowFiltering = allowFiltering;
            productSpecificationAttribute.ShowOnProductPage = showOnProductPage;
            productSpecificationAttribute.DisplayOrder = displayOrder;

            if (isNew)
            {
                _specificationAttributeService.InsertProductSpecificationAttribute(productSpecificationAttribute);
            }
            else
            {
                _specificationAttributeService.UpdateProductSpecificationAttribute(productSpecificationAttribute);
            }
        }

        private string DownloadFile(string urlString, IList<string> downloadedFiles)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;

            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
                return urlString;

            if (!_catalogSettings.ExportImportAllowDownloadImages)
                return string.Empty;

            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(UPLOADS_TEMP_PATH);
            _fileProvider.CreateDirectory(tempDirectory);

            var fileName = _fileProvider.GetFileName(urlString);
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var filePath = _fileProvider.Combine(tempDirectory, fileName);
            try
            {
                WebRequest.Create(urlString);
            }
            catch
            {
                return string.Empty;
            }

            try
            {
                byte[] fileData;
                using (var client = new WebClient())
                {
                    fileData = client.DownloadData(urlString);
                }

                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }

                downloadedFiles?.Add(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error("Download image failed", ex);
            }

            return string.Empty;
        }

        private ImportProductMetadata PrepareImportProductData(ExcelWorksheet worksheet)
        {
            //the columns
            var properties = GetPropertiesByExcelCells<Product>(worksheet);

            var manager = new PropertyManager<Product>(properties, _catalogSettings);

            var productAttributeProperties = new[]
            {
                new PropertyByName<ExportProductAttribute>("AttributeId"),
                new PropertyByName<ExportProductAttribute>("AttributeName"),
                new PropertyByName<ExportProductAttribute>("AttributeTextPrompt"),
                new PropertyByName<ExportProductAttribute>("AttributeIsRequired"),
                new PropertyByName<ExportProductAttribute>("AttributeControlType"),
                new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder"),
                new PropertyByName<ExportProductAttribute>("ProductAttributeValueId"),
                new PropertyByName<ExportProductAttribute>("ValueName"),
                new PropertyByName<ExportProductAttribute>("AttributeValueType"),
                new PropertyByName<ExportProductAttribute>("AssociatedProductId"),
                new PropertyByName<ExportProductAttribute>("ColorSquaresRgb"),
                new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId"),
                new PropertyByName<ExportProductAttribute>("PriceAdjustment"),
                new PropertyByName<ExportProductAttribute>("PriceAdjustmentUsePercentage"),
                new PropertyByName<ExportProductAttribute>("WeightAdjustment"),
                new PropertyByName<ExportProductAttribute>("Cost"),
                new PropertyByName<ExportProductAttribute>("CustomerEntersQty"),
                new PropertyByName<ExportProductAttribute>("Quantity"),
                new PropertyByName<ExportProductAttribute>("IsPreSelected"),
                new PropertyByName<ExportProductAttribute>("DisplayOrder"),
                new PropertyByName<ExportProductAttribute>("PictureId")
            };

            var productAttributeManager = new PropertyManager<ExportProductAttribute>(productAttributeProperties, _catalogSettings);

            var specificationAttributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute>("AttributeType", p => p.AttributeTypeId),
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttribute", p => p.SpecificationAttributeId),
                new PropertyByName<ExportSpecificationAttribute>("CustomValue", p => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttributeOptionId", p => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute>("AllowFiltering", p => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute>("ShowOnProductPage", p => p.ShowOnProductPage),
                new PropertyByName<ExportSpecificationAttribute>("DisplayOrder", p => p.DisplayOrder)
            };

            var specificationAttributeManager = new PropertyManager<ExportSpecificationAttribute>(specificationAttributeProperties, _catalogSettings);

            var endRow = 2;
            var allCategories = new List<string>();
            var allSku = new List<string>();

            var tempProperty = manager.GetProperty("Categories");
            var categoryCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            tempProperty = manager.GetProperty("SKU");
            var skuCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            var allManufacturers = new List<string>();
            tempProperty = manager.GetProperty("Manufacturers");
            var manufacturerCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
            {
                productAttributeManager.SetSelectList("AttributeControlType", AttributeControlType.TextBox.ToSelectList(useLocalization: false));
                productAttributeManager.SetSelectList("AttributeValueType", AttributeValueType.Simple.ToSelectList(useLocalization: false));

                specificationAttributeManager.SetSelectList("AttributeType", SpecificationAttributeType.Option.ToSelectList(useLocalization: false));
                specificationAttributeManager.SetSelectList("SpecificationAttribute", _specificationAttributeService
                    .GetSpecificationAttributes()
                    .Select(sa => sa as BaseEntity)
                    .ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty));

                manager.SetSelectList("ProductType", ProductType.SimpleProduct.ToSelectList(useLocalization: false));
                manager.SetSelectList("GiftCardType", GiftCardType.Virtual.ToSelectList(useLocalization: false));
                manager.SetSelectList("DownloadActivationType",
                    DownloadActivationType.Manually.ToSelectList(useLocalization: false));
                manager.SetSelectList("ManageInventoryMethod",
                    ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false));
                manager.SetSelectList("LowStockActivity",
                    LowStockActivity.Nothing.ToSelectList(useLocalization: false));
                manager.SetSelectList("BackorderMode", BackorderMode.NoBackorders.ToSelectList(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod",
                    RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", RentalPricePeriod.Days.ToSelectList(useLocalization: false));

                manager.SetSelectList("Vendor",
                    _vendorService.GetAllVendors(showHidden: true).Select(v => v as BaseEntity)
                        .ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
                manager.SetSelectList("ProductTemplate",
                    _productTemplateService.GetAllProductTemplates().Select(pt => pt as BaseEntity)
                        .ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
                manager.SetSelectList("DeliveryDate",
                    _dateRangeService.GetAllDeliveryDates().Select(dd => dd as BaseEntity)
                        .ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
                manager.SetSelectList("ProductAvailabilityRange",
                    _dateRangeService.GetAllProductAvailabilityRanges().Select(range => range as BaseEntity)
                        .ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
                manager.SetSelectList("TaxCategory",
                    _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity)
                        .ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
                manager.SetSelectList("BasepriceUnit",
                    _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity)
                        .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
                manager.SetSelectList("BasepriceBaseUnit",
                    _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity)
                        .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            }

            var allAttributeIds = new List<int>();
            var allSpecificationAttributeOptionIds = new List<int>();

            var attributeIdCellNum = 1 + ExportProductAttribute.ProducAttributeCellOffset;
            var specificationAttributeOptionIdCellNum =
                specificationAttributeManager.GetIndex("SpecificationAttributeOptionId") +
                ExportProductAttribute.ProducAttributeCellOffset;

            var productsInFile = new List<int>();

            //find end of data
            var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;
            while (true)
            {
                var allColumnsAreEmpty = manager.GetProperties
                    .Select(property => worksheet.Cells[endRow, property.PropertyOrderPosition])
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                if (new[] { 1, 2 }.Select(cellNum => worksheet.Cells[endRow, cellNum])
                        .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString())) &&
                    worksheet.Row(endRow).OutlineLevel == 0)
                {
                    var cellValue = worksheet.Cells[endRow, attributeIdCellNum].Value;
                    SetOutLineForProductAttributeRow(cellValue, worksheet, endRow);
                    SetOutLineForSpecificationAttributeRow(cellValue, worksheet, endRow);
                }

                if (worksheet.Row(endRow).OutlineLevel != 0)
                {
                    var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(worksheet, productAttributeManager, specificationAttributeManager, endRow);

                    //skip caption row
                    if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified && newTypeOfExportedAttribute != typeOfExportedAttribute)
                    {
                        typeOfExportedAttribute = newTypeOfExportedAttribute;
                        endRow++;
                        continue;
                    }

                    switch (typeOfExportedAttribute)
                    {
                        case ExportedAttributeType.ProductAttribute:
                            productAttributeManager.ReadFromXlsx(worksheet, endRow,
                                ExportProductAttribute.ProducAttributeCellOffset);
                            if (int.TryParse((worksheet.Cells[endRow, attributeIdCellNum].Value ?? string.Empty).ToString(), out var aid))
                            {
                                allAttributeIds.Add(aid);
                            }

                            break;
                        case ExportedAttributeType.SpecificationAttribute:
                            specificationAttributeManager.ReadFromXlsx(worksheet, endRow, ExportProductAttribute.ProducAttributeCellOffset);

                            if (int.TryParse((worksheet.Cells[endRow, specificationAttributeOptionIdCellNum].Value ?? string.Empty).ToString(), out var saoid))
                            {
                                allSpecificationAttributeOptionIds.Add(saoid);
                            }

                            break;
                    }

                    endRow++;
                    continue;
                }

                if (categoryCellNum > 0)
                {
                    var categoryIds = worksheet.Cells[endRow, categoryCellNum].Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(categoryIds))
                        allCategories.AddRange(categoryIds
                            .Split(new[] { ";", ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .Distinct());
                }

                if (skuCellNum > 0)
                {
                    var sku = worksheet.Cells[endRow, skuCellNum].Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(sku))
                        allSku.Add(sku);
                }

                if (manufacturerCellNum > 0)
                {
                    var manufacturerIds = worksheet.Cells[endRow, manufacturerCellNum].Value?.ToString() ??
                                          string.Empty;
                    if (!string.IsNullOrEmpty(manufacturerIds))
                        allManufacturers.AddRange(manufacturerIds
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }

                //counting the number of products
                productsInFile.Add(endRow);

                endRow++;
            }

            //performance optimization, the check for the existence of the categories in one SQL request
            var notExistingCategories = _categoryService.GetNotExistingCategories(allCategories.ToArray());
            if (notExistingCategories.Any())
            {
                throw new ArgumentException(string.Format(_localizationService.GetResource("Admin.Catalog.Products.Import.CategoriesDontExist"), string.Join(", ", notExistingCategories)));
            }

            //performance optimization, the check for the existence of the manufacturers in one SQL request
            var notExistingManufacturers = _manufacturerService.GetNotExistingManufacturers(allManufacturers.ToArray());
            if (notExistingManufacturers.Any())
            {
                throw new ArgumentException(string.Format(_localizationService.GetResource("Admin.Catalog.Products.Import.ManufacturersDontExist"), string.Join(", ", notExistingManufacturers)));
            }

            //performance optimization, the check for the existence of the product attributes in one SQL request
            var notExistingProductAttributes = _productAttributeService.GetNotExistingAttributes(allAttributeIds.ToArray());
            if (notExistingProductAttributes.Any())
            {
                throw new ArgumentException(string.Format(_localizationService.GetResource("Admin.Catalog.Products.Import.ProductAttributesDontExist"), string.Join(", ", notExistingProductAttributes)));
            }

            //performance optimization, the check for the existence of the specification attribute options in one SQL request
            var notExistingSpecificationAttributeOptions = _specificationAttributeService.GetNotExistingSpecificationAttributeOptions(allSpecificationAttributeOptionIds.Where(saoId => saoId != 0).ToArray());
            if (notExistingSpecificationAttributeOptions.Any())
            {
                throw new ArgumentException($"The following specification attribute option ID(s) don't exist - {string.Join(", ", notExistingSpecificationAttributeOptions)}");
            }

            return new ImportProductMetadata
            {
                EndRow = endRow,
                Manager = manager,
                Properties = properties,
                ProductsInFile = productsInFile,
                ProductAttributeManager = productAttributeManager,
                SpecificationAttributeManager = specificationAttributeManager,
                SkuCellNum = skuCellNum,
                AllSku = allSku
            };
        }

        private void ImportProductsFromSplitedXlsx(ExcelWorksheet worksheet, ImportProductMetadata metadata)
        {
            foreach (var path in SplitProductFile(worksheet, metadata))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // Resolve
                    var importManager = scope.ServiceProvider.GetRequiredService<IImportManager>();

                    using (var sr = new StreamReader(path))
                    {
                        importManager.ImportProductsFromXlsx(sr.BaseStream);
                    }
                }

                try
                {
                    _fileProvider.DeleteFile(path);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private IList<string> SplitProductFile(ExcelWorksheet worksheet, ImportProductMetadata metadata)
        {
            var fileIndex = 1;
            var fileName = Guid.NewGuid().ToString();
            var endCell = metadata.Properties.Max(p => p.PropertyOrderPosition);

            var filePaths = new List<string>();

            while (true)
            {
                var curIndex = fileIndex * _catalogSettings.ExportImportProductsCountInOneFile;

                var startRow = metadata.ProductsInFile[(fileIndex - 1) * _catalogSettings.ExportImportProductsCountInOneFile];

                var endRow = metadata.CountProductsInFile > curIndex + 1
                    ? metadata.ProductsInFile[curIndex - 1]
                    : metadata.EndRow;

                var filePath = $"{_fileProvider.MapPath(UPLOADS_TEMP_PATH)}/{fileName}_part_{fileIndex}.xlsx";

                CopyDataToNewFile(metadata, worksheet, filePath, startRow, endRow, endCell);

                filePaths.Add(filePath);
                fileIndex += 1;

                if (endRow == metadata.EndRow)
                    break;
            }

            return filePaths;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get property list by excel cells
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="worksheet">Excel worksheet</param>
        /// <returns>Property list</returns>
        public static IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ExcelWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Cells[1, poz];

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }

        public virtual Tuple<bool, string> ImportUsersFromOneC(IList<OneCUser> users)
        {
            try
            {
                int usersAdded = 0, usersUpdated = 0;
                foreach (var user in users)
                {
                    var customer = _customerService.GetCustomerById(user.Id);

                    if (customer == null)
                    {
                        usersAdded++;
                        customer = new Customer()
                        {
                            Id = 0,
                            Username = user?.Username,
                            Email = user?.Email,
                            IdOneC = user.IdOneC,
                            NumberDiscountCard = user.NumberDiscountCard,
                            DiscountPercent = user.Percent,
                            SumActiveBonus = user.SumActiveBonus,
                            SumBonus = user.SumBonus,
                            TotalSpent = user.TotalSpent,
                            CustomerGuid = Guid.NewGuid(),
                            Active = true,
                            CreatedOnUtc = DateTime.UtcNow,
                            LastActivityDateUtc = DateTime.UtcNow,
                            RegisteredInStoreId = _storeContext.CurrentStore.Id
                        };
                        _customerService.InsertCustomer(customer);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, user.City);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, user.Address);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, user.Apartament);

                        customer.CustomerCustomerRoleMappings.Add(new CustomerCustomerRoleMapping { CustomerRole = new CustomerRole() { Id = 3 } });

                        _customerService.UpdateCustomer(customer);
                    }
                    else
                    {
                        usersUpdated++;
                        customer.Username = user?.Username;
                        customer.Email = user?.Email;
                        customer.IdOneC = user.IdOneC;
                        customer.NumberDiscountCard = user.NumberDiscountCard;
                        customer.DiscountPercent = user.Percent;
                        customer.SumActiveBonus = user.SumActiveBonus;
                        customer.SumBonus = user.SumBonus;
                        customer.TotalSpent = user.TotalSpent;
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, user.City);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, user.Address);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, user.Apartament);
                        _customerService.UpdateCustomer(customer);
                    }
                }
                return new Tuple<bool, string>(true, String.Format("Users {0} added. Users {1} updated.", usersAdded, usersUpdated));
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
        }

        /// <summary>
        /// Import products from 1C
        /// </summary>
        /// <param name="products">List<ProductFromOneC></param>
        public virtual Tuple<bool, string> ImportProductsFromOneC(List<OneCProduct> products)
        {
            try
            {
                int newCount = 0, updateCount = 0, manufacturAddedCount = 0;
                foreach (var item in products)
                {
                    //manufacture
                    Manufacturer manufacturer = _manufacturerService.GetManufacturerByName(item.Manufacturer);
                    if (!string.IsNullOrEmpty(item.Manufacturer) && manufacturer == null)
                    {
                        manufacturer = new Manufacturer()
                        {
                            Name = item.Manufacturer,
                            ManufacturerTemplateId = 1,
                            PageSize = 6,
                            AllowCustomersToSelectPageSize = true,
                            PageSizeOptions = "6, 3, 9",
                            SubjectToAcl = false,
                            LimitedToStores = false,
                            Published = true,
                            Deleted = false,
                            DisplayOrder = 1,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        };
                        _manufacturerService.InsertManufacturer(manufacturer);
                        manufacturAddedCount++;
                    }

                    //warehouse
                    var productWarehouseStatuses = new List<ProductWarehouseStatus>();
                    foreach (var itemWarehouse in item.Warehouses)
                    {
                        var warehouse = _shippingService.GetWarehouseByName(itemWarehouse.WarehouseName);
                        if (warehouse == null)
                        {
                            warehouse = new Warehouse()
                            {
                                Name = itemWarehouse.WarehouseName
                            };
                            _shippingService.InsertWarehouse(warehouse);
                        }

                        var status = _productService.GetStatusByName(itemWarehouse.WarehouseStatus);
                        if (status == null)
                        {
                            status = new Status()
                            {
                                Name = itemWarehouse.WarehouseStatus
                            };
                            _productService.InsertStatus(status);
                        }

                        productWarehouseStatuses.Add(new ProductWarehouseStatus()
                        {
                            WarehouseId = warehouse.Id,
                            StatusId = status.Id
                        });
                    }

                    //status
                    int statusId = 0;
                    if (!string.IsNullOrEmpty(item.Status))
                    {
                        statusId = _productService.GetStatusByName(item.Status)?.Id ?? 1;
                    }

                    //product
                    //var product = _productService.GetProductBySku(item.Sku) ?? _productService.GetProductByName(item.Name);
                    var product = _productService.GetProductBySku(item.Sku);
                    if (product == null)
                    {
                        product = new Product()
                        {
                            Name = item.Name,
                            Price = decimal.Parse(item.Price.Replace('.', ',')),
                            Sku = item.Sku,
                            Published = false,
                            ProductTypeId = 5,
                            ParentGroupedProductId = 0,
                            VisibleIndividually = true,
                            ProductTemplateId = 1,
                            VendorId = 0,
                            ShowOnHomePage = false,
                            AllowCustomerReviews = true,
                            GiftCardTypeId = 0,
                            IsShipEnabled = true,
                            TaxCategoryId = 0,
                            OrderMaximumQuantity = 10000,
                            OrderMinimumQuantity = 1,
                            StockQuantity = 10000,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                            MakeCode = item.Model,
                            SleepLength = (int)decimal.Parse(item.SleepLength.Replace('.', ',')),
                            SleepWidth = (int)decimal.Parse(item.SleepWidth.Replace('.', ',')),
                            Height = decimal.Parse(item.Height.Replace('.', ',')),
                            Weight = decimal.Parse(item.Weight.Replace('.', ',')),
                            Length = decimal.Parse(item.Length.Replace('.', ',')),
                            Width = decimal.Parse(item.Width.Replace('.', ','))
                        };

                        if (statusId > 0)
                        {
                            product.StatusId = statusId;
                        }
                        else
                        {
                            product.StatusId = 3;//The status is "Ожидает модерации" it needs for if adding new product without status.
                        }

                        _productService.InsertProduct(product);

                        if (manufacturer != null)
                        {
                            _manufacturerService.InsertProductManufacturer(new ProductManufacturer()
                            {
                                DisplayOrder = 1,
                                IsFeaturedProduct = false,
                                ManufacturerId = manufacturer.Id,
                                ProductId = product.Id,
                            });
                        }

                        foreach (var itemProductWarehouseStatuse in productWarehouseStatuses)
                        {
                            itemProductWarehouseStatuse.ProductId = product.Id;
                            _shippingService.InsertProductWarehouseStatus(itemProductWarehouseStatuse);
                        }

                        newCount++;
                    }
                    else
                    {
                        product.Name = item.Name;
                        product.Sku = item.Sku;
                        product.Price = decimal.Parse(item.Price);
                        product.OrderMaximumQuantity = 10000;
                        product.OrderMinimumQuantity = 1;
                        product.StockQuantity = 10000;
                        product.MakeCode = item.Model;
                        product.SleepLength = (int)decimal.Parse(item.SleepLength.Replace('.', ','));
                        product.SleepWidth = (int)decimal.Parse(item.SleepWidth.Replace('.', ','));
                        product.Height = decimal.Parse(item.Height.Replace('.', ','));
                        product.Weight = decimal.Parse(item.Weight.Replace('.', ','));
                        product.Length = decimal.Parse(item.Length.Replace('.', ','));
                        product.Width = decimal.Parse(item.Width.Replace('.', ','));

                        if (statusId > 0)
                        {
                            product.StatusId = statusId;
                        }

                        _productService.UpdateProduct(product);

                        if (manufacturer != null)
                        {
                            var productManufacturers = _manufacturerService.GetProductManufacturersByProductId(product.Id);
                            if (productManufacturers.FirstOrDefault(x => x.ManufacturerId == manufacturer.Id) == null)
                            {
                                productManufacturers.Clear();
                                _manufacturerService.InsertProductManufacturer(new ProductManufacturer()
                                {
                                    DisplayOrder = 1,
                                    IsFeaturedProduct = false,
                                    ManufacturerId = manufacturer.Id,
                                    ProductId = product.Id
                                });
                            }
                        }

                        foreach (var itemProductWarehouseStatuse in productWarehouseStatuses)
                        {
                            itemProductWarehouseStatuse.ProductId = product.Id;
                            var productWarehouseStatuse = _shippingService.GetProductWarehouseStatus(product.Id, itemProductWarehouseStatuse.Id);
                            if (productWarehouseStatuse == null)
                            {
                                _shippingService.InsertProductWarehouseStatus(new ProductWarehouseStatus()
                                {
                                    ProductId = product.Id,
                                    StatusId = itemProductWarehouseStatuse.StatusId,
                                    WarehouseId = itemProductWarehouseStatuse.WarehouseId
                                });
                            }
                            else
                            {
                                _shippingService.UpdateProductWarehouseStatus(itemProductWarehouseStatuse);
                            }
                        }

                        updateCount++;
                    }

                    //specification attribute
                    if (item.Attributes != null)
                    {
                        foreach (var itemAttribute in item.Attributes)
                        {
                            var attribute = _specificationAttributeService.GetSpecificationAttributeByName(itemAttribute.Name);

                            if (attribute == null)
                            {
                                attribute = new SpecificationAttribute()
                                {
                                    Name = itemAttribute.Name,
                                    DisplayOrder = 0
                                };
                                _specificationAttributeService.InsertSpecificationAttribute(attribute);
                            }

                            var option = _specificationAttributeService.GetSpecificationAttributeOptionByName(itemAttribute.Value, attribute.Id);

                            if (option == null)
                            {
                                option = new SpecificationAttributeOption()
                                {
                                    DisplayOrder = 0,
                                    Name = itemAttribute.Value,
                                    SpecificationAttributeId = attribute.Id
                                };
                                _specificationAttributeService.InsertSpecificationAttributeOption(option);
                            }

                            var productOption = _specificationAttributeService.GetProductSpecificationAttributeByProductIdProductSpecificationAttributeId(product.Id, option.Id);

                            if (productOption == null)
                            {
                                productOption = new ProductSpecificationAttribute()
                                {
                                    ProductId = product.Id,
                                    SpecificationAttributeOptionId = option.Id,
                                    DisplayOrder = 0,
                                    AttributeTypeId = 0,
                                    ShowOnProductPage = true,
                                    AllowFiltering = true,
                                    CustomValue = null
                                };
                                _specificationAttributeService.InsertProductSpecificationAttribute(productOption);
                            }
                        }
                    }

                    //attributes
                    //foreach (var itemAttribute in item.Attributes)
                    //{
                    //    var attribute = _productAttributeService.GetProductAttributeByName(itemAttribute.Name);
                    //    if (attribute == null)
                    //    {
                    //        attribute = new ProductAttribute()
                    //        {
                    //            Name = itemAttribute.Name,
                    //        };
                    //        _productAttributeService.InsertProductAttribute(attribute);
                    //    }

                    //    var productAttributeMapping = _productAttributeService.GetProductAttributeMappingByAttributeIdProductId(attribute.Id, product.Id);
                    //    if (productAttributeMapping == null)
                    //    {
                    //        _productAttributeService.InsertProductAttributeMapping(new ProductAttributeMapping()
                    //        {
                    //            ProductAttributeId = attribute.Id,
                    //            ProductId = product.Id,
                    //            AttributeControlTypeId = itemAttribute.UiType == 0 ? 1 : itemAttribute.UiType
                    //        });
                    //    }
                    //    else
                    //    {
                    //        productAttributeMapping.AttributeControlTypeId = itemAttribute.UiType == 0 ? 1 : itemAttribute.UiType;
                    //        _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
                    //    }

                    //    foreach (var itemAttributeValue in itemAttribute.AttributeValues)
                    //    {
                    //        var attributeValue = _productAttributeService.GetProductAttributeValueByName(productAttributeMapping.Id, itemAttributeValue.Name);
                    //        if (attributeValue == null)
                    //        {
                    //            attributeValue = new ProductAttributeValue()
                    //            {
                    //                Name = itemAttributeValue.Name,
                    //                Quantity = itemAttributeValue.Count,
                    //                AttributeValueTypeId = 0,
                    //                ProductAttributeMappingId = productAttributeMapping.Id
                    //            };
                    //            _productAttributeService.InsertProductAttributeValue(attributeValue);
                    //        }
                    //    }
                    //}
                }
                return new Tuple<bool, string>(true, String.Format("Products {0} added. Products {1} Updated. Manufactures {2} added.", newCount, updateCount, manufacturAddedCount));
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
        }

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportProductsFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var downloadedFiles = new List<string>();

                var metadata = PrepareImportProductData(worksheet);

                if (_catalogSettings.ExportImportSplitProductsFile && metadata.CountProductsInFile > _catalogSettings.ExportImportProductsCountInOneFile)
                {
                    ImportProductsFromSplitedXlsx(worksheet, metadata);
                    return;
                }

                //performance optimization, load all products by SKU in one SQL request
                var allProductsBySku = _productService.GetProductsBySku(metadata.AllSku.ToArray(), _workContext.CurrentVendor?.Id ?? 0);

                //validate maximum number of products per vendor
                if (_vendorSettings.MaximumProductNumber > 0 &&
                    _workContext.CurrentVendor != null)
                {
                    var newProductsCount = metadata.CountProductsInFile - allProductsBySku.Count;
                    if (_productService.GetNumberOfProductsByVendorId(_workContext.CurrentVendor.Id) + newProductsCount > _vendorSettings.MaximumProductNumber)
                        throw new ArgumentException(string.Format(_localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"), _vendorSettings.MaximumProductNumber));
                }

                //performance optimization, load all categories IDs for products in one SQL request
                var allProductsCategoryIds = _categoryService.GetProductCategoryIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all categories in one SQL request
                Dictionary<CategoryKey, Category> allCategories;
                try
                {
                    allCategories = _categoryService
                        .GetAllCategories(showHidden: true, loadCacheableCopy: false)
                        .ToDictionary(c => new CategoryKey(c, _categoryService, _storeMappingService), c => c);
                }
                catch (ArgumentException)
                {
                    //categories with the same name are not supported in the same category level
                    throw new ArgumentException(_localizationService.GetResource("Admin.Catalog.Products.Import.CategoriesWithSameNameNotSupported"));
                }

                //performance optimization, load all manufacturers IDs for products in one SQL request
                var allProductsManufacturerIds = _manufacturerService.GetProductManufacturerIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all manufacturers in one SQL request
                var allManufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);

                //product to import images
                var productPictureMetadata = new List<ProductPictureMetadata>();

                Product lastLoadedProduct = null;
                var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;

                for (var iRow = 2; iRow < metadata.EndRow; iRow++)
                {
                    //imports product attributes
                    if (worksheet.Row(iRow).OutlineLevel != 0)
                    {
                        if (lastLoadedProduct == null)
                            continue;

                        var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(worksheet, metadata.ProductAttributeManager, metadata.SpecificationAttributeManager, iRow);

                        //skip caption row
                        if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified &&
                            newTypeOfExportedAttribute != typeOfExportedAttribute)
                        {
                            typeOfExportedAttribute = newTypeOfExportedAttribute;
                            continue;
                        }

                        switch (typeOfExportedAttribute)
                        {
                            case ExportedAttributeType.ProductAttribute:
                                ImportProductAttribute(metadata.ProductAttributeManager, lastLoadedProduct);
                                break;
                            case ExportedAttributeType.SpecificationAttribute:
                                ImportSpecificationAttribute(metadata.SpecificationAttributeManager, lastLoadedProduct);
                                break;
                            case ExportedAttributeType.NotSpecified:
                            default:
                                continue;
                        }

                        continue;
                    }

                    metadata.Manager.ReadFromXlsx(worksheet, iRow);

                    var product = metadata.SkuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == metadata.Manager.GetProperty("SKU").StringValue) : null;

                    var isNew = product == null;

                    product = product ?? new Product();

                    //some of previous values
                    var previousStockQuantity = product.StockQuantity;
                    var previousWarehouseId = product.WarehouseId;

                    if (isNew)
                        product.CreatedOnUtc = DateTime.UtcNow;

                    foreach (var property in metadata.Manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "ProductType":
                                product.ProductTypeId = property.IntValue;
                                break;
                            case "ParentGroupedProductId":
                                product.ParentGroupedProductId = property.IntValue;
                                break;
                            case "VisibleIndividually":
                                product.VisibleIndividually = property.BooleanValue;
                                break;
                            case "Name":
                                product.Name = property.StringValue;
                                break;
                            case "ShortDescription":
                                product.ShortDescription = property.StringValue;
                                break;
                            case "FullDescription":
                                product.FullDescription = property.StringValue;
                                break;
                            case "Vendor":
                                //vendor can't change this field
                                if (_workContext.CurrentVendor == null)
                                    product.VendorId = property.IntValue;
                                break;
                            case "ProductTemplate":
                                product.ProductTemplateId = property.IntValue;
                                break;
                            case "ShowOnHomePage":
                                //vendor can't change this field
                                if (_workContext.CurrentVendor == null)
                                    product.ShowOnHomePage = property.BooleanValue;
                                break;
                            case "MetaKeywords":
                                product.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                product.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                product.MetaTitle = property.StringValue;
                                break;
                            case "AllowCustomerReviews":
                                product.AllowCustomerReviews = property.BooleanValue;
                                break;
                            case "Published":
                                product.Published = property.BooleanValue;
                                break;
                            case "SKU":
                                product.Sku = property.StringValue;
                                break;
                            case "ManufacturerPartNumber":
                                product.ManufacturerPartNumber = property.StringValue;
                                break;
                            case "Gtin":
                                product.Gtin = property.StringValue;
                                break;
                            case "IsGiftCard":
                                product.IsGiftCard = property.BooleanValue;
                                break;
                            case "GiftCardType":
                                product.GiftCardTypeId = property.IntValue;
                                break;
                            case "OverriddenGiftCardAmount":
                                product.OverriddenGiftCardAmount = property.DecimalValue;
                                break;
                            case "RequireOtherProducts":
                                product.RequireOtherProducts = property.BooleanValue;
                                break;
                            case "RequiredProductIds":
                                product.RequiredProductIds = property.StringValue;
                                break;
                            case "AutomaticallyAddRequiredProducts":
                                product.AutomaticallyAddRequiredProducts = property.BooleanValue;
                                break;
                            case "IsDownload":
                                product.IsDownload = property.BooleanValue;
                                break;
                            case "DownloadId":
                                product.DownloadId = property.IntValue;
                                break;
                            case "UnlimitedDownloads":
                                product.UnlimitedDownloads = property.BooleanValue;
                                break;
                            case "MaxNumberOfDownloads":
                                product.MaxNumberOfDownloads = property.IntValue;
                                break;
                            case "DownloadActivationType":
                                product.DownloadActivationTypeId = property.IntValue;
                                break;
                            case "HasSampleDownload":
                                product.HasSampleDownload = property.BooleanValue;
                                break;
                            case "SampleDownloadId":
                                product.SampleDownloadId = property.IntValue;
                                break;
                            case "HasUserAgreement":
                                product.HasUserAgreement = property.BooleanValue;
                                break;
                            case "UserAgreementText":
                                product.UserAgreementText = property.StringValue;
                                break;
                            case "IsRecurring":
                                product.IsRecurring = property.BooleanValue;
                                break;
                            case "RecurringCycleLength":
                                product.RecurringCycleLength = property.IntValue;
                                break;
                            case "RecurringCyclePeriod":
                                product.RecurringCyclePeriodId = property.IntValue;
                                break;
                            case "RecurringTotalCycles":
                                product.RecurringTotalCycles = property.IntValue;
                                break;
                            case "IsRental":
                                product.IsRental = property.BooleanValue;
                                break;
                            case "RentalPriceLength":
                                product.RentalPriceLength = property.IntValue;
                                break;
                            case "RentalPricePeriod":
                                product.RentalPricePeriodId = property.IntValue;
                                break;
                            case "IsShipEnabled":
                                product.IsShipEnabled = property.BooleanValue;
                                break;
                            case "IsFreeShipping":
                                product.IsFreeShipping = property.BooleanValue;
                                break;
                            case "ShipSeparately":
                                product.ShipSeparately = property.BooleanValue;
                                break;
                            case "AdditionalShippingCharge":
                                product.AdditionalShippingCharge = property.DecimalValue;
                                break;
                            case "DeliveryDate":
                                product.DeliveryDateId = property.IntValue;
                                break;
                            case "IsTaxExempt":
                                product.IsTaxExempt = property.BooleanValue;
                                break;
                            case "TaxCategory":
                                product.TaxCategoryId = property.IntValue;
                                break;
                            case "IsTelecommunicationsOrBroadcastingOrElectronicServices":
                                product.IsTelecommunicationsOrBroadcastingOrElectronicServices = property.BooleanValue;
                                break;
                            case "ManageInventoryMethod":
                                product.ManageInventoryMethodId = property.IntValue;
                                break;
                            case "ProductAvailabilityRange":
                                product.ProductAvailabilityRangeId = property.IntValue;
                                break;
                            case "UseMultipleWarehouses":
                                product.UseMultipleWarehouses = property.BooleanValue;
                                break;
                            case "WarehouseId":
                                product.WarehouseId = property.IntValue;
                                break;
                            case "StockQuantity":
                                product.StockQuantity = property.IntValue;
                                break;
                            case "DisplayStockAvailability":
                                product.DisplayStockAvailability = property.BooleanValue;
                                break;
                            case "DisplayStockQuantity":
                                product.DisplayStockQuantity = property.BooleanValue;
                                break;
                            case "MinStockQuantity":
                                product.MinStockQuantity = property.IntValue;
                                break;
                            case "LowStockActivity":
                                product.LowStockActivityId = property.IntValue;
                                break;
                            case "NotifyAdminForQuantityBelow":
                                product.NotifyAdminForQuantityBelow = property.IntValue;
                                break;
                            case "BackorderMode":
                                product.BackorderModeId = property.IntValue;
                                break;
                            case "AllowBackInStockSubscriptions":
                                product.AllowBackInStockSubscriptions = property.BooleanValue;
                                break;
                            case "OrderMinimumQuantity":
                                product.OrderMinimumQuantity = property.IntValue;
                                break;
                            case "OrderMaximumQuantity":
                                product.OrderMaximumQuantity = property.IntValue;
                                break;
                            case "AllowedQuantities":
                                product.AllowedQuantities = property.StringValue;
                                break;
                            case "AllowAddingOnlyExistingAttributeCombinations":
                                product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                                break;
                            case "NotReturnable":
                                product.NotReturnable = property.BooleanValue;
                                break;
                            case "DisableBuyButton":
                                product.DisableBuyButton = property.BooleanValue;
                                break;
                            case "DisableWishlistButton":
                                product.DisableWishlistButton = property.BooleanValue;
                                break;
                            case "AvailableForPreOrder":
                                product.AvailableForPreOrder = property.BooleanValue;
                                break;
                            case "PreOrderAvailabilityStartDateTimeUtc":
                                product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CallForPrice":
                                product.CallForPrice = property.BooleanValue;
                                break;
                            case "Price":
                                product.Price = property.DecimalValue;
                                break;
                            case "OldPrice":
                                product.OldPrice = property.DecimalValue;
                                break;
                            case "ProductCost":
                                product.ProductCost = property.DecimalValue;
                                break;
                            case "CustomerEntersPrice":
                                product.CustomerEntersPrice = property.BooleanValue;
                                break;
                            case "MinimumCustomerEnteredPrice":
                                product.MinimumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "MaximumCustomerEnteredPrice":
                                product.MaximumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "BasepriceEnabled":
                                product.BasepriceEnabled = property.BooleanValue;
                                break;
                            case "BasepriceAmount":
                                product.BasepriceAmount = property.DecimalValue;
                                break;
                            case "BasepriceUnit":
                                product.BasepriceUnitId = property.IntValue;
                                break;
                            case "BasepriceBaseAmount":
                                product.BasepriceBaseAmount = property.DecimalValue;
                                break;
                            case "BasepriceBaseUnit":
                                product.BasepriceBaseUnitId = property.IntValue;
                                break;
                            case "MarkAsNew":
                                product.MarkAsNew = property.BooleanValue;
                                break;
                            case "MarkAsNewStartDateTimeUtc":
                                product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "MarkAsNewEndDateTimeUtc":
                                product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "Weight":
                                product.Weight = property.DecimalValue;
                                break;
                            case "Length":
                                product.Length = property.DecimalValue;
                                break;
                            case "Width":
                                product.Width = property.DecimalValue;
                                break;
                            case "Height":
                                product.Height = property.DecimalValue;
                                break;
                        }
                    }

                    //set some default values if not specified
                    if (isNew && metadata.Properties.All(p => p.PropertyName != "ProductType"))
                        product.ProductType = ProductType.SimpleProduct;
                    if (isNew && metadata.Properties.All(p => p.PropertyName != "VisibleIndividually"))
                        product.VisibleIndividually = true;
                    if (isNew && metadata.Properties.All(p => p.PropertyName != "Published"))
                        product.Published = true;

                    //sets the current vendor for the new product
                    if (isNew && _workContext.CurrentVendor != null)
                        product.VendorId = _workContext.CurrentVendor.Id;

                    product.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                    {
                        _productService.InsertProduct(product);
                    }
                    else
                    {
                        _productService.UpdateProduct(product);
                    }

                    //quantity change history
                    if (isNew || previousWarehouseId == product.WarehouseId)
                    {
                        _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                            product.WarehouseId, _localizationService.GetResource("Admin.StockQuantityHistory.Messages.ImportProduct.Edit"));
                    }
                    //warehouse is changed 
                    else
                    {
                        //compose a message
                        var oldWarehouseMessage = string.Empty;
                        if (previousWarehouseId > 0)
                        {
                            var oldWarehouse = _shippingService.GetWarehouseById(previousWarehouseId);
                            if (oldWarehouse != null)
                                oldWarehouseMessage = string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                        }

                        var newWarehouseMessage = string.Empty;
                        if (product.WarehouseId > 0)
                        {
                            var newWarehouse = _shippingService.GetWarehouseById(product.WarehouseId);
                            if (newWarehouse != null)
                                newWarehouseMessage = string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                        }

                        var message = string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.ImportProduct.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                        //record history
                        _productService.AddStockQuantityHistoryEntry(product, -previousStockQuantity, 0, previousWarehouseId, message);
                        _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
                    }

                    var tempProperty = metadata.Manager.GetProperty("SeName");
                    if (tempProperty != null)
                    {
                        var seName = tempProperty.StringValue;
                        //search engine name
                        _urlRecordService.SaveSlug(product, _urlRecordService.ValidateSeName(product, seName, product.Name, true), 0);
                    }

                    tempProperty = metadata.Manager.GetProperty("Categories");

                    if (tempProperty != null)
                    {
                        var categoryList = tempProperty.StringValue;

                        //category mappings
                        var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? new int[0] : allProductsCategoryIds[product.Id];

                        var importedCategories = categoryList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(categoryName => new CategoryKey(categoryName))
                            .Select(categoryKey => allCategories.ContainsKey(categoryKey) ? allCategories[categoryKey].Id : allCategories.Values.FirstOrDefault(c => c.Name == categoryKey.Key)?.Id ?? int.Parse(categoryKey.Key)).ToList();

                        foreach (var categoryId in importedCategories)
                        {
                            if (categories.Any(c => c == categoryId))
                                continue;

                            var productCategory = new ProductCategory
                            {
                                ProductId = product.Id,
                                CategoryId = categoryId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _categoryService.InsertProductCategory(productCategory);
                        }

                        //delete product categories
                        var deletedProductCategories = categories.Where(categoryId => !importedCategories.Contains(categoryId))
                            .Select(categoryId => product.ProductCategories.First(pc => pc.CategoryId == categoryId));
                        foreach (var deletedProductCategory in deletedProductCategories)
                        {
                            _categoryService.DeleteProductCategory(deletedProductCategory);
                        }
                    }

                    tempProperty = metadata.Manager.GetProperty("Manufacturers");
                    if (tempProperty != null)
                    {
                        var manufacturerList = tempProperty.StringValue;

                        //manufacturer mappings
                        var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? new int[0] : allProductsManufacturerIds[product.Id];
                        var importedManufacturers = manufacturerList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => allManufacturers.FirstOrDefault(m => m.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList();
                        foreach (var manufacturerId in importedManufacturers)
                        {
                            if (manufacturers.Any(c => c == manufacturerId))
                                continue;

                            var productManufacturer = new ProductManufacturer
                            {
                                ProductId = product.Id,
                                ManufacturerId = manufacturerId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _manufacturerService.InsertProductManufacturer(productManufacturer);
                        }

                        //delete product manufacturers
                        var deletedProductsManufacturers = manufacturers.Where(manufacturerId => !importedManufacturers.Contains(manufacturerId))
                            .Select(manufacturerId => product.ProductManufacturers.First(pc => pc.ManufacturerId == manufacturerId));
                        foreach (var deletedProductManufacturer in deletedProductsManufacturers)
                        {
                            _manufacturerService.DeleteProductManufacturer(deletedProductManufacturer);
                        }
                    }

                    tempProperty = metadata.Manager.GetProperty("ProductTags");
                    if (tempProperty != null)
                    {
                        var productTags = tempProperty.StringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                        //searching existing product tags by their id
                        var productTagIds = productTags.Where(pt => int.TryParse(pt, out var _)).Select(int.Parse);
                        var pruductTagsByIds = product.ProductProductTagMappings
                            .Select(mapping => mapping.ProductTag).Where(pt => productTagIds.Contains(pt.Id)).ToList();
                        productTags.AddRange(pruductTagsByIds.Select(pt => pt.Name));
                        var filter = pruductTagsByIds.Select(pt => pt.Id.ToString()).ToList();

                        //product tag mappings
                        _productTagService.UpdateProductTags(product, productTags.Where(pt => !filter.Contains(pt)).ToArray());
                    }

                    var picture1 = DownloadFile(metadata.Manager.GetProperty("Picture1")?.StringValue, downloadedFiles);
                    var picture2 = DownloadFile(metadata.Manager.GetProperty("Picture2")?.StringValue, downloadedFiles);
                    var picture3 = DownloadFile(metadata.Manager.GetProperty("Picture3")?.StringValue, downloadedFiles);

                    productPictureMetadata.Add(new ProductPictureMetadata
                    {
                        ProductItem = product,
                        Picture1Path = picture1,
                        Picture2Path = picture2,
                        Picture3Path = picture3,
                        IsNew = isNew
                    });

                    lastLoadedProduct = product;

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    //_productService.UpdateHasTierPricesProperty(product);
                    //_productService.UpdateHasDiscountsApplied(product);
                }

                if (_mediaSettings.ImportProductImagesUsingHash && _pictureService.StoreInDb && _dataProvider.SupportedLengthOfBinaryHash > 0)
                    ImportProductImagesUsingHash(productPictureMetadata, allProductsBySku);
                else
                    ImportProductImagesUsingServices(productPictureMetadata);

                foreach (var downloadedFile in downloadedFiles)
                {
                    if (!_fileProvider.FileExists(downloadedFile))
                        continue;

                    try
                    {
                        _fileProvider.DeleteFile(downloadedFile);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                //activity log
                _customerActivityService.InsertActivity("ImportProducts", string.Format(_localizationService.GetResource("ActivityLog.ImportProducts"), metadata.CountProductsInFile));
            }
        }

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        public virtual int ImportNewsletterSubscribersFromTxt(Stream stream)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    string email;
                    var isActive = true;
                    var storeId = _storeContext.CurrentStore.Id;
                    //parse
                    if (tmp.Length == 1)
                    {
                        //"email" only
                        email = tmp[0].Trim();
                    }
                    else if (tmp.Length == 2)
                    {
                        //"email" and "active" fields specified
                        email = tmp[0].Trim();
                        isActive = bool.Parse(tmp[1].Trim());
                    }
                    else if (tmp.Length == 3)
                    {
                        //"email" and "active" and "storeId" fields specified
                        email = tmp[0].Trim();
                        isActive = bool.Parse(tmp[1].Trim());
                        storeId = int.Parse(tmp[2].Trim());
                    }
                    else
                        throw new NopException("Wrong file format");

                    //import
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    }

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        public virtual int ImportStatesFromTxt(Stream stream)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new NopException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    var published = bool.Parse(tmp[3].Trim());
                    var displayOrder = int.Parse(tmp[4].Trim());

                    var country = _countryService.GetCountryByTwoLetterIsoCode(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplayOrder = displayOrder;
                        _stateProvinceService.UpdateStateProvince(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder
                        };
                        _stateProvinceService.InsertStateProvince(state);
                    }

                    count++;
                }
            }

            //activity log
            _customerActivityService.InsertActivity("ImportStates",
                string.Format(_localizationService.GetResource("ActivityLog.ImportStates"), count));

            return count;
        }

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportManufacturersFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<Manufacturer>(worksheet);

                var manager = new PropertyManager<Manufacturer>(properties, _catalogSettings);

                var iRow = 2;
                var setSeName = properties.Any(p => p.PropertyName == "SeName");

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var manufacturer = _manufacturerService.GetManufacturerById(manager.GetProperty("Id").IntValue);

                    var isNew = manufacturer == null;

                    manufacturer = manufacturer ?? new Manufacturer();

                    if (isNew)
                    {
                        manufacturer.CreatedOnUtc = DateTime.UtcNow;

                        //default values
                        manufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
                        manufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                        manufacturer.Published = true;
                        manufacturer.AllowCustomersToSelectPageSize = true;
                    }

                    var seName = string.Empty;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Name":
                                manufacturer.Name = property.StringValue;
                                break;
                            case "Description":
                                manufacturer.Description = property.StringValue;
                                break;
                            case "ManufacturerTemplateId":
                                manufacturer.ManufacturerTemplateId = property.IntValue;
                                break;
                            case "MetaKeywords":
                                manufacturer.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                manufacturer.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                manufacturer.MetaTitle = property.StringValue;
                                break;
                            case "Picture":
                                var picture = LoadPicture(manager.GetProperty("Picture").StringValue, manufacturer.Name, isNew ? null : (int?)manufacturer.PictureId);

                                if (picture != null)
                                    manufacturer.PictureId = picture.Id;

                                break;
                            case "PageSize":
                                manufacturer.PageSize = property.IntValue;
                                break;
                            case "AllowCustomersToSelectPageSize":
                                manufacturer.AllowCustomersToSelectPageSize = property.BooleanValue;
                                break;
                            case "PageSizeOptions":
                                manufacturer.PageSizeOptions = property.StringValue;
                                break;
                            case "PriceRanges":
                                manufacturer.PriceRanges = property.StringValue;
                                break;
                            case "Published":
                                manufacturer.Published = property.BooleanValue;
                                break;
                            case "DisplayOrder":
                                manufacturer.DisplayOrder = property.IntValue;
                                break;
                            case "SeName":
                                seName = property.StringValue;
                                break;
                        }
                    }

                    manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _manufacturerService.InsertManufacturer(manufacturer);
                    else
                        _manufacturerService.UpdateManufacturer(manufacturer);

                    //search engine name
                    if (setSeName)
                        _urlRecordService.SaveSlug(manufacturer, _urlRecordService.ValidateSeName(manufacturer, seName, manufacturer.Name, true), 0);

                    iRow++;
                }

                //activity log
                _customerActivityService.InsertActivity("ImportManufacturers",
                    string.Format(_localizationService.GetResource("ActivityLog.ImportManufacturers"), iRow - 2));
            }
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportCategoriesFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<Category>(worksheet);

                var manager = new PropertyManager<Category>(properties, _catalogSettings);

                var iRow = 2;
                var setSeName = properties.Any(p => p.PropertyName == "SeName");

                //performance optimization, load all categories in one SQL request
                var allCategories = _categoryService
                    .GetAllCategories(showHidden: true, loadCacheableCopy: false)
                    .GroupBy(c => _categoryService.GetFormattedBreadCrumb(c))
                    .ToDictionary(c => c.Key, c => c.First());

                var saveNextTime = new List<int>();

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    //get category by data in xlsx file if it possible, or create new category
                    var category = GetCategoryFromXlsx(manager, worksheet, iRow, allCategories, out var isNew, out var curentCategoryBreadCrumb);

                    //update category by data in xlsx file
                    var seName = UpdateCategoryByXlsx(category, manager, allCategories, isNew, out var isParentCategoryExists);

                    if (isParentCategoryExists)
                    {
                        //if parent category exists in database then save category into database
                        SaveCategory(isNew, category, allCategories, curentCategoryBreadCrumb, setSeName, seName);
                    }
                    else
                    {
                        //if parent category doesn't exists in database then try save category into database next time
                        saveNextTime.Add(iRow);
                    }

                    iRow++;
                }

                var needSave = saveNextTime.Any();

                while (needSave)
                {
                    var remove = new List<int>();

                    //try to save unsaved categories
                    foreach (var rowId in saveNextTime)
                    {
                        //get category by data in xlsx file if it possible, or create new category
                        var category = GetCategoryFromXlsx(manager, worksheet, rowId, allCategories, out var isNew, out var curentCategoryBreadCrumb);
                        //update category by data in xlsx file
                        var seName = UpdateCategoryByXlsx(category, manager, allCategories, isNew, out var isParentCategoryExists);

                        if (!isParentCategoryExists)
                            continue;

                        //if parent category exists in database then save category into database
                        SaveCategory(isNew, category, allCategories, curentCategoryBreadCrumb, setSeName, seName);
                        remove.Add(rowId);
                    }

                    saveNextTime.RemoveAll(item => remove.Contains(item));

                    needSave = remove.Any() && saveNextTime.Any();
                }

                //activity log
                _customerActivityService.InsertActivity("ImportCategories",
                    string.Format(_localizationService.GetResource("ActivityLog.ImportCategories"), iRow - 2 - saveNextTime.Count));

                if (!saveNextTime.Any())
                    return;

                var caregoriesName = new List<string>();

                foreach (var rowId in saveNextTime)
                {
                    manager.ReadFromXlsx(worksheet, rowId);
                    caregoriesName.Add(manager.GetProperty("Name").StringValue);
                }

                throw new ArgumentException(string.Format(_localizationService.GetResource("Admin.Catalog.Categories.Import.CategoriesArentImported"), string.Join(", ", caregoriesName)));
            }
        }

        #endregion

        #region Nested classes

        protected class ProductPictureMetadata
        {
            public Product ProductItem { get; set; }

            public string Picture1Path { get; set; }

            public string Picture2Path { get; set; }

            public string Picture3Path { get; set; }

            public bool IsNew { get; set; }
        }

        public class CategoryKey
        {
            public CategoryKey(Category category, ICategoryService categoryService, IStoreMappingService storeMappingService)
            {
                Key = categoryService.GetFormattedBreadCrumb(category);
                StoresIds = category.LimitedToStores ? storeMappingService.GetStoresIdsWithAccess(category).ToList() : new List<int>();
                Category = category;
            }

            public CategoryKey(string key, List<int> storesIds = null)
            {
                Key = key.Trim();
                StoresIds = storesIds ?? new List<int>();
            }

            public List<int> StoresIds { get; }

            public Category Category { get; }

            public string Key { get; }

            public bool Equals(CategoryKey y)
            {
                if (y == null)
                    return false;

                if (Category != null && y.Category != null)
                    return Category.Id == y.Category.Id;

                if ((StoresIds.Any() || y.StoresIds.Any())
                    && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                    return false;

                return Key.Equals(y.Key);
            }

            public override int GetHashCode()
            {
                if (!StoresIds.Any()) 
                    return Key.GetHashCode();

                var storesIds = StoresIds.Select(id => id.ToString())
                    .Aggregate(string.Empty, (all, current) => all + current);

                return $"{storesIds}_{Key}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as CategoryKey;
                return other?.Equals(other) ?? false;
            }
        }

        #endregion
    }
}