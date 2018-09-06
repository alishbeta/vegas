using Nop.Core.Domain.Topics;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Topics
{
    public class CustomTopicModel : BaseNopModel
    {
		public Topic Topic { get; set; }
		public Topic NextTopic { get; set; }
		public Topic PreviousTopic { get; set; }
	}
}
