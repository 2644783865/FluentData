﻿using FluentData._Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentData.Command
{
	[TestClass]
	public class AutoMapperTests
	{
		[TestMethod]
		public void Test_multiple_property_levels()
		{
			var report = TestHelper.Context().Sql(@"select o.*,
												l.OrderLineId as OrderLine_OrderLineId,
												p.ProductId as OrderLine_Product_ProductId,
												p.Name as OrderLine_Product_Name,
												c.CategoryId as OrderLine_Product_Category_CategoryId,
												c.Name as OrderLine_Product_Category_Name
											from [Order] o
											inner join OrderLine l on o.OrderId = l.OrderId
											inner join Product p on l.ProductId = p.ProductId
											inner join Category c on p.CategoryId = c.CategoryId")
									.Query<OrderReport>();

			Assert.IsTrue(report.Count > 0);
		}

		[TestMethod]
		public void Test_same_columns_and_properties_automap_must_not_fail()
		{
			var result = TestHelper.Context().Sql(@"select CategoryId, Name
											from Category").Query<Category>();

			Assert.IsTrue(result.Count > 0);
		}

		[TestMethod]
		public void Test_different_columns_and_properties_automap_must_fail()
		{
			try
			{
				var result = TestHelper.Context().Sql(@"select CategoryId as CategoryIdNotExist, Name
															from Category").Query<Category>();

				Assert.Fail();
			}
			catch (FluentDataException exception)
			{
				Assert.AreEqual("Could not map: CategoryIdNotExist", exception.Message);
			}
		}

		[TestMethod]
		public void Test_fieldname_propertyname_with_underscore()
		{
			var product = TestHelper.Context()
										.Sql(@"select top 1
													p.ProductId as product_Id,
													p.Name,
													c.CategoryId as Category_Id,
													c.Name as Category_Name
												from Product p
												inner join Category c on p.CategoryId = c.CategoryId").QuerySingle<ProductWithUnderscore>();

			Assert.IsFalse(string.IsNullOrEmpty(product.Category_Name));
		}

		public class ProductWithUnderscore
		{
			public int Product_Id { get; set; }
			public string Name { get; set; }
			public int Category_Id { get; set; }
			public string Category_Name { get; set; }
		}
	}
}
