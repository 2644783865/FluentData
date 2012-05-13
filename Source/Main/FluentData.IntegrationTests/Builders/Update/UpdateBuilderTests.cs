﻿using FluentData._Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentData
{
	[TestClass]
	public class UpdateBuilderTests
	{
		[TestMethod]
		public void Test()
		{
			using (var context = TestHelper.Context().UseTransaction(true))
			{
				var productId = TestHelper.InsertProduct(context, "OldTestProduct", 1);

				var rowsAffected = context.Update("Product")
						.Column("Name", "NewTestProduct")
						.Column("CategoryId", 2)
						.Where("ProductId", productId)
						.Execute();

				Assert.AreEqual(1, rowsAffected);
				
				var product = TestHelper.GetProduct(context, productId);
				
				Assert.AreEqual("NewTestProduct", product.Name);
				Assert.AreEqual(2, product.CategoryId);
				Assert.IsNotNull(product);
			}
		}
	}
}