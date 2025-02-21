

using ChatGPT.Net;
using DocumentFormat.OpenXml.Office2010.Excel;
using LinqToDB;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.GPTFriend.Factories;
using Nop.Plugin.Misc.GPTFriend.Model;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.GPTFriend.Services
{
	public class GptFriendService 
	{
		private readonly QuestionFactory _questionFactory;
		private readonly QuestionParametersFactory _questionParametersFactory;
		private readonly IProductService _productService;
		private readonly IManufacturerService _manufacturerService;
		private readonly ICategoryService _categoryService;
		private readonly IUrlRecordService _urlRecordService;


        private ChatGpt api;

		public GptFriendService(IUrlRecordService urlRecordService, QuestionFactory questionFactory, QuestionParametersFactory questionParametersFactory, IProductService productService, IManufacturerService manufacturerService, ICategoryService categoryService)
		{
			_urlRecordService = urlRecordService;
			_questionFactory = questionFactory;
			_questionParametersFactory = questionParametersFactory;
			_manufacturerService = manufacturerService;
			_productService = productService;
			_categoryService = categoryService;
		}

		private void SetGptApi(GPTFriendSettings settings)
		{
			api = new ChatGpt(settings.GptKey);
			api.Config.MaxTokens = settings.GptMaxTokens;
			api.Config.Model = settings.GptModel.ToString();
		}

		private async Task<string> TransformToSeoText(GPTFriendSettings settings, string question, bool trim = true)
		{			
            var response = await api.Ask(question);
			if (trim)
				response = response.Trim('"');
			System.Threading.Thread.Sleep(settings.GptDelay); //retraso 10 segundos evitar too many requests
			return response;
        }

		public async Task<Task> FillManufacturers(GPTFriendSettings settings, bool add_metadata)
		{
			SetGptApi(settings);
			var manufacturers = await _manufacturerService.GetAllManufacturersAsync();
			foreach (var manufacturer in manufacturers)
			{
				QuestionParameters parameters = _questionParametersFactory.PrepareManufacturerParameters(manufacturer);
				if(manufacturer.Description.IsNullOrEmpty())
				{
					//string question = _questionFactory.PrepareQuestion(settings.GptMarcasSystemMessage, parameters);
					//manufacturer.Description = await TransformToSeoText(question);
     //               manufacturer.MetaTitle = await TransformToSeoText(question);
     //               manufacturer.MetaKeywords = await TransformToSeoText(question, false);
     //               manufacturer.MetaDescription = await TransformToSeoText(question);
                }
				
				await _manufacturerService.UpdateManufacturerAsync(manufacturer);
			}
			return Task.CompletedTask;
		}

		public async Task<Task> FillAllFamilys(GPTFriendSettings settings)
		{
			SetGptApi(settings);
			var categorys = (await _categoryService.GetAllCategoriesAsync()).ToList();
			
			foreach (var cat in categorys)
			{
				if (!cat.Description.IsNullOrEmpty())
					continue;

				var path = GetCategoryPathRecursive(cat, categorys);
				string question = settings.GptFamiliasSystemMessage
										.Replace("@Categoria", cat.Name, StringComparison.InvariantCultureIgnoreCase)
										.Replace("@Path", path, StringComparison.InvariantCultureIgnoreCase);

				string response = await TransformToSeoText(settings, question);
				string json = GetJson(response).json;

				JObject jsonObject = ParseJsonToObject(json);
				if (jsonObject == null)
					continue;

				cat.MetaTitle = (string)jsonObject["MetaTitle"];
				cat.MetaDescription = (string)jsonObject["MetaDescription"];
				cat.Description = (string)jsonObject["Descripcion"];
				cat.UpdatedOnUtc = DateTime.UtcNow;

				await _categoryService.UpdateCategoryAsync(cat);
				await _urlRecordService.SaveSlugAsync(cat, await _urlRecordService.ValidateSeNameAsync(cat, string.Empty, cat.Name, true), 0);
			}

			return Task.CompletedTask;
		}

        private string GetCategoryPathRecursive(Category category, IList<Category> categories)
        {
            if (category.ParentCategoryId == 0)
            {
                return category.Name;
            }

            var parentCategory = categories.FirstOrDefault(c => c.Id == category.ParentCategoryId);
            if (parentCategory == null)
            {
                return category.Name;
            }

            return GetCategoryPathRecursive(parentCategory, categories) + "->" + category.Name;
        }

		public static (string json, string text) GetJson(string input)
		{
            if (input.StartsWith("{"))
                return (input, "");

			// Expresión regular para capturar el JSON entre '''json y '''
			string pattern = @"```json(.*?)```";
			Regex regex = new Regex(pattern, RegexOptions.Singleline);
			Match match = regex.Match(input);

			string validJson = "";
            string nonJsonText = input;
			if (match.Success)
			{
				// Extraer el JSON con el formato correcto
				string jsonWithSingleQuotes = match.Groups[1].Value;
				// Reemplazar comillas simples por comillas dobles para hacer el JSON válido
				validJson = jsonWithSingleQuotes.Replace("'", "\"");

                nonJsonText = regex.Replace(input, "");
			}

			return (validJson, nonJsonText);
        }

        public async Task<Task> FillAllProductsCategorys(GPTFriendSettings settings)
		{
            return null;
            //var categories = await _categoryService.GetAllCategoriesAsync();


            //         var qry2 = (from c in categories
            //                    select new
            //                    {
            //                        id = c.Id,
            //                        ruta_categoria = GetCategoryPathRecursive(c, categories)
            //                    })
            //		   .OrderBy(r => r.ruta_categoria)
            //		   .ToArray();


            //string categorys_csv = string.Join("\r\n", qry2.Select(r => r.id.ToString() + ";" + r.ruta_categoria).ToArray());
            //if (!(await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).Any())
//				{

                //					string question = @"

                //Selecciona el id de categoría más adecuada para un artículo de sexshop basándote en una descripción y palabras clave proporcionadas.
                //Selecciona la categoría más específica que esté disponible dentro de la jerarquía de subtegorías seleccionadas. En otras palabras, si hay varias subcategorías dentro de una categoría principal, debemos seleccionar la subcategoría más específica en lugar de la categoría principal en sí misma. 
                //Dame como resultado solamente el id de categoria seleccionado.

                //Descripción: " + product.ShortDescription + @"

                //" + (product.AdminComment.IsNullOrEmpty()? "" : "Palabras clave: " + product.AdminComment) + @".

                //Lista de Categorías:

                //ID;RUTA_CATEGORIA
                //" + categorys_csv;

                //					string response = await TransformToSeoText(question);

                //                    Regex regex = new Regex(@"\d+");
                //                    Match match = regex.Match(response);

                //					// Si se encuentra un número, convertirlo a entero y devolverlo
                //					if (match.Success)
                //					{
                //						int idFamiliaPrincipal = int.Parse(match.Value);
                //						if ((await _categoryService.GetCategoryByIdAsync(idFamiliaPrincipal)) != null)
                //						{
                //							await _categoryService.InsertProductCategoryAsync(new ProductCategory()
                //							{
                //								CategoryId = idFamiliaPrincipal,
                //								ProductId = product.Id,
                //								DisplayOrder = 0,
                //								IsFeaturedProduct = false
                //							}); 
                //						}
                //					}
                //				}
        }
        public async Task<Task> FillAllProducts(GPTFriendSettings settings)
		{
			SetGptApi(settings);
			var qry = (await _productService.SearchProductsAsync(showHidden: false))
                        .Where(r => r.CreatedOnUtc < DateTime.Now.AddMonths(-2))
                        .ToList();

			
			await Parallel.ForEachAsync(qry, async (product, cancellationToken) =>
			{
				if (!product.AdminComment.IsNullOrEmpty() && product.AdminComment.Contains("GPT Update"))
					return;

				if (product.Name.IsNullOrEmpty() || product.FullDescription.IsNullOrEmpty())
					return;

				var pm = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
				string fabricante = string.Empty;
				if (pm.Any())
				{
					var man = await _manufacturerService.GetManufacturerByIdAsync(pm[0].ManufacturerId);
					fabricante = man.Name;
				}

				string question = settings.GptProductSystemMessage
										.Replace("@@ProductName", product.Name)
										.Replace("@@Fabricante", fabricante.IsNullOrEmpty() ? "" : "Fabricante:" + fabricante)
										.Replace("@@FullDescription", product.FullDescription);

				string response = await TransformToSeoText(settings, question);
				string json = GetJson(response).json;

				JObject jsonObject = ParseJsonToObject(json);
				if (jsonObject == null)
					return;
				product.MetaTitle = (string)jsonObject["MetaTitle"];
				product.MetaDescription = (string)jsonObject["MetaDescription"];
				product.FullDescription = (string)jsonObject["LongDescription"];
				product.ShortDescription = (string)jsonObject["ShortDescription"];
				product.AdminComment = "GPT Update: " + DateTime.Now.ToShortDateString();
				// product.Name = (string)jsonObject["ProductName"];
				product.UpdatedOnUtc = DateTime.UtcNow;

				await _productService.UpdateProductAsync(product);
				await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, string.Empty, product.Name, true), 0);
			});



			return Task.CompletedTask;
		}

        public static JObject ParseJsonToObject(string input)
        {
			input = input.Trim();
			if (input.IsNullOrEmpty()) 
				return null;

            try
            {
				JObject obj = JObject.Parse(input);
                return obj;
            }
            catch (JsonReaderException)
            {
				return null;
			}
        }
    }
}
