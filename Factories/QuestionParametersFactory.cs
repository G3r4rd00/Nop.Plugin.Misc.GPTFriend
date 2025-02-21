
using DocumentFormat.OpenXml.Spreadsheet;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.GPTFriend.Model;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.GPTFriend.Factories
{
	public class QuestionParametersFactory
	{

		private readonly IProductService _productService;
		private readonly IManufacturerService _manufacturerService;
		private readonly ICategoryService _categoryService;
		

		public QuestionParametersFactory(IProductService productService, IManufacturerService manufacturerService, ICategoryService categoryService)
		{
			_manufacturerService = manufacturerService;
			_productService = productService;
			_categoryService = categoryService;
		}

		public async Task<QuestionParameters> PrepareCategoyParameters(Category category)
		{
			QuestionParameters parameters = new QuestionParameters();

			parameters.Category = category;
			parameters.ParentCategory = await _categoryService.GetCategoryByIdAsync(parameters.Category.ParentCategoryId);
			parameters.MainSubCategories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parameters.Category.ParentCategoryId);

			return parameters;
		}

		public QuestionParameters PrepareManufacturerParameters(Manufacturer manufacturer)
		{
			QuestionParameters parameters = new QuestionParameters();
			parameters.MainManufacturer = manufacturer;
			return parameters;
		}

		public async Task<QuestionParameters> PrepareProductParameters(Product product)
		{
			QuestionParameters parameters = new QuestionParameters();
			
			parameters.Product = product;

            var manufacturers = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id));
            var manufacturerId = manufacturers.Any() ? manufacturers[0].ManufacturerId : 0;
            if (manufacturerId > 0)
				parameters.MainManufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            var categoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).OrderBy(r => r.DisplayOrder).Select(r => r.CategoryId).ToArray();
            parameters.Categories = await _categoryService.GetCategoriesByIdsAsync(categoryIds);
            if (parameters.Categories.Any())
			{
                parameters.Category = parameters.Categories.OrderBy(r => r.DisplayOrder).First();
                parameters.ParentCategory = await _categoryService.GetCategoryByIdAsync(parameters.Category.ParentCategoryId);
                parameters.MainSubCategories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parameters.Category.ParentCategoryId);
            }
			

			return parameters;
		}

	}
}
