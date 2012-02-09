﻿using FluentData._Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentData
{
	[TestClass]
	public class InsertBuilderGenericTests
	{
		[TestMethod]
		public void Test_No_Automap()
		{
			var product = new Product();
			product.CategoryId = 1;
			product.Name = "TestProduct";

			using (var context = TestHelper.Context().UseTransaction)
			{
				product.ProductId = context.Insert<Product>("Product", product)
									.Column("Name", "TestProduct")
									.Column(x => x.CategoryId)
									.ExecuteReturnLastId();

				var createdProduct = TestHelper.GetProduct(context, product.ProductId);
				Assert.AreEqual("TestProduct", createdProduct.Name);
				Assert.AreEqual(1, createdProduct.CategoryId);
				Assert.IsNotNull(createdProduct);
			}
		}

		[TestMethod]
		public void TestAutomap()
		{
			var product = new Product();
			product.CategoryId = 1;
			product.Name = "TestProduct";

			using (var context = TestHelper.Context().UseTransaction)
			{
				product.ProductId = context.Insert<Product>("Product", product)
									.IgnoreProperty(x => x.ProductId)
									.AutoMap()
									.ExecuteReturnLastId();

				var createdProduct = TestHelper.GetProduct(context, product.ProductId);
				Assert.AreEqual("TestProduct", createdProduct.Name);
				Assert.AreEqual(1, createdProduct.CategoryId);
				Assert.IsNotNull(createdProduct);
			}
		}
	}
}
