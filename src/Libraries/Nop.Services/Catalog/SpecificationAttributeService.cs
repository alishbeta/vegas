using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public SpecificationAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IUrlRecordService urlRecordService,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository)
        {
            _urlRecordService = urlRecordService;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
        }

        #endregion

        #region Methods

        public virtual IEnumerable<SimilarProductSizes> GetSimilarProductSizes(string makeCode,string colorName, int productId = 0, bool isSleepSizes = false)
        {
            var attributeQuery = from s in _specificationAttributeRepository.Table
                        orderby s.Id
                        where s.Name.ToLower() == "цвет"
                        select s;

            var attribute = attributeQuery.FirstOrDefault();

            if (attribute == null)
                return null;

            var optionQuery = from s in _specificationAttributeOptionRepository.Table
                              orderby s.Id
                              where s.SpecificationAttributeId == attribute.Id && s.Name == colorName
                              select s.Id;

            var option = optionQuery.FirstOrDefault();

            if (optionQuery == null)
                return null;

            var similarProductsQuery = from s in _productSpecificationAttributeRepository.Table
                                       orderby s.Id
                                       where s.SpecificationAttributeOptionId == option
                                       select s.Product;

            var products = similarProductsQuery.ToList();

            if (isSleepSizes)
            {
                var model = products.Where(x => x.MakeCode == makeCode).Select(x => new SimilarProductSizes()
                {
                    height = null,
                    length = x.SleepLength.ToString("#.##"),
                    productUrl = string.Format("/{0}", _urlRecordService.GetSeName(x.Id, "Product", null, true, true)),
                    width = x.SleepWidth.ToString("#.##")
                });
                return model;
            }
            else
            {
                var model = products.Select(x => new SimilarProductSizes()
                {
                    height = x.Height.ToString("#.##"),
                    length = x.Length.ToString("#.##"),
                    productUrl = string.Format("/{0}", _urlRecordService.GetSeName(x.Id, "Product", null, true, true)),
                    width = x.Width.ToString("#.##")
                });
                return model;
            }
        }

        public virtual IEnumerable<int> GetSimilarProductIdsByColor(string makeCode, string colorName, int productId = 0)
        {
            var attributeQuery = from s in _specificationAttributeRepository.Table
                                 orderby s.Id
                                 where s.Name.ToLower() == "цвет"
                                 select s;

            var attribute = attributeQuery.FirstOrDefault();

            if (attribute == null)
                return null;

            var optionsQuery = from s in _specificationAttributeOptionRepository.Table
                              orderby s.Id
                              where s.SpecificationAttributeId == attribute.Id && s.Name != colorName
                              select s.Id;

            var options = optionsQuery;

            if (optionsQuery == null)
                return null;

            var similarProductsQuery = from s in _productSpecificationAttributeRepository.Table
                                       orderby s.Id
                                       where options.Contains(s.SpecificationAttributeOptionId) && s.Product.MakeCode == makeCode
                                       select s.ProductId;

            return similarProductsQuery.ToList();
        }

        #region Specification attribute

        public virtual ProductSpecificationAttribute GetProductSpecificationAttributeByProductIdProductSpecificationAttributeId(int productId, int productSpecificationAttributeId)
        {
            if (productId == 0 && productSpecificationAttributeId == 0)
                return null;

            var query = from s in _productSpecificationAttributeRepository.Table
                        orderby s.Id
                        where s.ProductId == productId && s.SpecificationAttributeOptionId == productSpecificationAttributeId
                        select s;

            return query.FirstOrDefault();
        }

        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionByName(string name, int specificationAttributeId)
        {
            //if (specificationAttributeId <= 0)
            //       return null;

            //var query = from s in _specificationAttributeOptionRepository.Table
            //            orderby s.Id
            //            where s.SpecificationAttributeId == specificationAttributeId
            //            select s;

            //return query.FirstOrDefault();
            if (string.IsNullOrEmpty(name))
                return null;

            name = name.Trim();

            var query = from s in _specificationAttributeOptionRepository.Table
                        orderby s.Id
                        where s.Name.ToLower() == name.ToLower() && s.SpecificationAttributeId == specificationAttributeId
                        select s;

            return query.FirstOrDefault();
        }

        public virtual SpecificationAttribute GetSpecificationAttributeByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            name = name.Trim();

            var query = from s in _specificationAttributeRepository.Table
                        orderby s.Id
                        where s.Name.ToLower() == name.ToLower()
                        select s;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return _specificationAttributeRepository.GetById(specificationAttributeId);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Specification attributes</returns>
        public virtual IPagedList<SpecificationAttribute> GetSpecificationAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder, sa.Id
                        select sa;
            var specificationAttributes = new PagedList<SpecificationAttribute>(query, pageIndex, pageSize);
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            _specificationAttributeRepository.Delete(specificationAttribute);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            _specificationAttributeRepository.Insert(specificationAttribute);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            _specificationAttributeRepository.Update(specificationAttribute);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            return _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId);
        }

        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>Specification attribute options</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsByIds(int[] specificationAttributeOptionIds)
        {
            if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Length == 0)
                return new List<SpecificationAttributeOption>();

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where specificationAttributeOptionIds.Contains(sao.Id)
                        select sao;
            var specificationAttributeOptions = query.ToList();
            //sort by passed identifiers
            var sortedSpecificationAttributeOptions = new List<SpecificationAttributeOption>();
            foreach (var id in specificationAttributeOptionIds)
            {
                var sao = specificationAttributeOptions.Find(x => x.Id == id);
                if (sao != null)
                    sortedSpecificationAttributeOptions.Add(sao);
            }

            return sortedSpecificationAttributeOptions;
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder, sao.Id
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();
            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

        /// <summary>
        /// Returns a list of IDs of not existing specification attribute options
        /// </summary>
        /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
        /// <returns>List of IDs not existing specification attribute options</returns>
        public virtual int[] GetNotExistingSpecificationAttributeOptions(int[] attributeOptionIds)
        {
            if (attributeOptionIds == null)
                throw new ArgumentNullException(nameof(attributeOptionIds));

            var query = _specificationAttributeOptionRepository.Table;
            var queryFilter = attributeOptionIds.Distinct().ToArray();
            var filter = query.Select(a => a.Id).Where(m => queryFilter.Contains(m)).ToList();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        public virtual void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 1 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<ProductSpecificationAttribute> GetProductSpecificationAttributes(int productId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null)
        {
            var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            var showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";
            var key = string.Format(NopCatalogDefaults.ProductSpecificationAttributeAllByProductIdCacheKey,
                productId, specificationAttributeOptionId, allowFilteringCacheStr, showOnProductPageCacheStr);

            return _cacheManager.Get(key, () =>
            {
                var query = _productSpecificationAttributeRepository.Table;
                if (productId > 0)
                    query = query.Where(psa => psa.ProductId == productId);
                if (specificationAttributeOptionId > 0)
                    query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public virtual ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;

            return _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(productSpecificationAttribute);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            _productSpecificationAttributeRepository.Update(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductSpecificationAttributePatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a count of product specification attribute mapping records
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
        /// <returns>Count</returns>
        public virtual int GetProductSpecificationAttributeCount(int productId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _productSpecificationAttributeRepository.Table;
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return query.Count();
        }

        /// <summary>
        /// Get mapped products for specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Products</returns>
        public virtual IPagedList<Product> GetProductsBySpecificationAttributeId(int specificationAttributeId, int pageIndex, int pageSize)
        {
            var query = _productSpecificationAttributeRepository.Table;

            var products = query.Where(psa => psa.SpecificationAttributeOption.SpecificationAttributeId == specificationAttributeId)
                .Select(psa => psa.Product).Distinct().OrderBy(p => p.Name);

            return new PagedList<Product>(products, pageIndex, pageSize);
        }

        #endregion

        #endregion
    }
}