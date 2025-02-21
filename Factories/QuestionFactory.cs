using DocumentFormat.OpenXml.Presentation;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.GPTFriend.Model;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.GPTFriend.Factories
{
	public enum Tags
	{
		nombre_categoria_padre,
		nombre_categoria,
		descripcion_categoria,
		sub_categoria_nombres,

		nombre_marca,
		descripcion_marca,

		nombre_producto,
		descripcion_producto,
		
		
	}

	public class QuestionFactory
	{		
		public string PrepareQuestion(string question, QuestionParameters parameters)
		{
			
			//Categoria
			question = question.Replace("{" + Tags.nombre_categoria + "}", parameters.Category == null? "": parameters.Category.Name);
			question = question.Replace("{" + Tags.descripcion_categoria + "}", parameters.Category == null ? "" : parameters.Category.Description);
			question = question.Replace("{" + Tags.sub_categoria_nombres + "}", parameters.MainSubCategories == null ? "" : string.Join(",", parameters.MainSubCategories.Select(r => r.Name)));
			question = question.Replace("{" + Tags.nombre_categoria_padre + "}", parameters.ParentCategory == null? "" : parameters.ParentCategory.Name);

			//Marcas
			question = question.Replace("{" + Tags.nombre_marca + "}", parameters.MainManufacturer == null? "" : parameters.MainManufacturer.Name);
			question = question.Replace("{" + Tags.descripcion_marca + "}", parameters.MainManufacturer == null ? "" : parameters.MainManufacturer.Description);
			
			//Productos
			question = question.Replace("{" + Tags.nombre_producto + "}", parameters.Product == null? "" : parameters.Product.Name);
			question = question.Replace("{" + Tags.descripcion_producto + "}", parameters.Product == null ? "" : parameters.Product.FullDescription);
			
			
			return question;
		}


	}
}
