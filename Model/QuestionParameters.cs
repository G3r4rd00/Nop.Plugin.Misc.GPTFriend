

using Nop.Core.Domain.Catalog;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.GPTFriend.Model
{
	public class QuestionParameters
	{
		public IList<Category> Categories { get; set; }
		public IList<Category> MainSubCategories { get; set; }
		public Category ParentCategory { get; set; }
		public Category Category { get; set; }
		public Product Product { get; set; }

		public Manufacturer MainManufacturer { get; set; }
	}
}
