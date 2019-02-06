﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Topics;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the topic model factory
    /// </summary>
    public partial class TopicModelFactory : ITopicModelFactory
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicModelFactory(IAclService aclService,
            ILocalizationService localizationService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IPictureService pictureService,
            ITopicTemplateService topicTemplateService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            this._pictureService = pictureService;
            this._aclService = aclService;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._topicService = topicService;
            this._topicTemplateService = topicTemplateService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>Topic model</returns>
        protected virtual TopicModel PrepareTopicModel(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : _localizationService.GetLocalized(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? "" : _localizationService.GetLocalized(topic, x => x.Body),
                MetaKeywords = _localizationService.GetLocalized(topic, x => x.MetaKeywords),
                MetaDescription = _localizationService.GetLocalized(topic, x => x.MetaDescription),
                MetaTitle = _localizationService.GetLocalized(topic, x => x.MetaTitle),
                SeName = _urlRecordService.GetSeName(topic),
                TopicTemplateId = topic.TopicTemplateId,
                Published = topic.Published
            };
            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the topic model by topic identifier
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <returns>Topic model</returns>
        public virtual TopicModel PrepareTopicModelById(int topicId)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_ID_KEY,
                topicId,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var topic = _topicService.GetTopicById(topicId);
                //ACL (access control list)
                if (topic == null || !_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            });

            return cachedModel;
        } 
        public virtual CustomTopicModel PrepareTopicById(int topicId)
		{
			var storeId = _storeContext.CurrentStore.Id;
			var model = new CustomTopicModel();
			model.Topic = _topicService.GetTopicById(topicId);
			var test = _topicService.GetAllTopics(storeId);
			model.PreviousTopic = _topicService.GetAllTopics(storeId).FirstOrDefault(x => x.TopicCategoryId == model.Topic.TopicCategoryId && x.Id < topicId);
			model.NextTopic = _topicService.GetAllTopics(storeId).FirstOrDefault(x => x.TopicCategoryId == model.Topic.TopicCategoryId && x.Id > topicId);
            model.PictureUrl = _pictureService.GetPictureUrl(model.Topic.PictureId) ?? _pictureService.GetDefaultPictureUrl(250);
			return model;
        }

		public virtual IList<Topic> PrepareTopicListModel(int storeId)
		{
			var topics = new List<int>() { 5 };	 //main topic id
			foreach (var topicCategory in _topicService.GetAllTopicCategories().Where(x => x.ParentId == 5))
			{
				topics.Add(topicCategory.Id);
			}
			return _topicService.GetAllTopics(storeId).Where(x => topics.Contains(x.TopicCategoryId)).ToList(); 
		}

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>Topic model</returns>
        public virtual TopicModel PrepareTopicModelBySystemName(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_SYSTEMNAME_KEY,
                systemName,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //load by store
                var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
                if (topic == null)
                    return null;
                return PrepareTopicModel(topic);
            });

            return cachedModel;
        }

        /// <summary>
        /// Get topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>View path</returns>
        public virtual string PrepareTemplateViewPath(int topicTemplateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TEMPLATE_MODEL_KEY, topicTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _topicTemplateService.GetTopicTemplateById(topicTemplateId);
                if (template == null)
                    template = _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });
            return templateViewPath;
        }

        #endregion
    }
}