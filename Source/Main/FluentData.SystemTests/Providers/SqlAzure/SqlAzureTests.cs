﻿using System.Collections.Generic;
using FluentData._Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentData.Providers.SqlAzure
{
	[TestClass]
	public class SqlAzureTests : IDbProviderTests
	{
		protected IDbContext Context()
		{
			return new DbContext().ConnectionStringName("SqlAzure", DbProviderTypes.SqlAzure);
		}

		[TestMethod]
		public void Query_many_dynamic()
		{
			var products = Context().Sql("select * from Category")
									.Query();

			Assert.IsTrue(products.Count > 0);
		}

		[TestMethod]
		public void Query_single_dynamic()
		{
			var product = Context().Sql("select * from Product where ProductId = 1")
									.QuerySingle();

			Assert.IsNotNull(product);
		}

		[TestMethod]
		public void Query_many_strongly_typed()
		{
			var products = Context().Sql("select * from Product")
									.Query<Product>();

			Assert.IsTrue(products.Count > 0);
		}

		[TestMethod]
		public void Query_single_strongly_typed()
		{
			var product = Context().Sql("select * from Product where ProductId = 1")
									.QuerySingle<Product>();

			Assert.IsNotNull(product);
		}

		[TestMethod]
		public void Query_auto_mapping_alias()
		{
			var product = Context().Sql(@"select p.*,
											c.CategoryId as Category_CategoryId,
											c.Name as Category_Name
											from Product p
											inner join Category c on p.CategoryId = c.CategoryId
											where ProductId = 1")
									.QuerySingle<Product>();

			Assert.IsNotNull(product);
			Assert.IsNotNull(product.Category);
			Assert.IsNotNull(product.Category.Name);
		}

		[TestMethod]
		public void Query_custom_mapping_dynamic()
		{
			var products = Context().Sql(@"select * from Product")
									.QueryNoAutoMap<Product>(Custom_mapper_using_dynamic);

			Assert.IsNotNull(products[0].Name);
		}

		public void Custom_mapper_using_dynamic(dynamic row, Product product)
		{
			product.ProductId = row.ProductId;
			product.Name = row.Name;
		}

		[TestMethod]
		public void Query_custom_mapping_datareader()
		{
			var products = Context().Sql(@"select * from Product")
									.QueryNoAutoMap<Product>(Custom_mapper_using_datareader);

			Assert.IsNotNull(products[0].Name);
		}

		public void Custom_mapper_using_datareader(IDataReader row, Product product)
		{
			product.ProductId = row.GetInt32("ProductId");
			product.Name = row.GetString("Name");
		}

		[TestMethod]
		public void QueryValue()
		{
			int categoryId = Context().Sql("select CategoryId from Product where ProductId = 1")
										.QueryValue<int>();

			Assert.AreEqual(1, categoryId);
		}

		[TestMethod]
		public void QueryValues()
		{
			var categories = Context().Sql("select CategoryId from Category order by CategoryId").QueryValues<int>();

			Assert.AreEqual(2, categories.Count);
			Assert.AreEqual(1, categories[0]);
			Assert.AreEqual(2, categories[1]);
		}

		[TestMethod]
		public void Unnamed_parameters_one()
		{
			var product = Context().Sql("select * from Product where ProductId = @0")
									.Parameters(1)
									.QuerySingle();

			Assert.IsNotNull(product);
		}

		[TestMethod]
		public void Unnamed_parameters_many()
		{
			var products = Context().Sql("select * from Product where ProductId = @0 or ProductId = @1")
									.Parameters(1, 3)
									.Query();

			Assert.AreEqual(2, products.Count);
		}

		[TestMethod]
		public void Named_parameters()
		{
			var products = Context().Sql("select * from Product where ProductId = @ProductId1 or ProductId = @ProductId2")
									.Parameter("ProductId1", 1)
									.Parameter("ProductId2", 3)
									.Query();

			Assert.AreEqual(2, products.Count);
		}

		[TestMethod]
		public void In_query()
		{
			var ids = new List<int>() { 1, 3, 4, 6 };

			var products = Context().Sql("select * from Product where ProductId in(@0)")
									.Parameters(ids)
									.Query();

			Assert.AreEqual(4, products.Count);
		}

		[TestMethod]
		public void MultipleResultset()
		{
			using (var command = Context().MultiResultSql())
			{
				var categories = command.Sql(@"select * from Category;
									select * from Product;").Query();

				var products = command.Query();

				Assert.IsTrue(categories.Count > 0);
				Assert.IsTrue(products.Count > 0);
			}
		}

		[TestMethod]
		public void Insert_data_sql()
		{
			var productId = Context().Sql("insert into Product(Name, CategoryId) values(@0, @1);")
							.Parameters("The Warren Buffet Way", 1)
							.ExecuteReturnLastId();

			Assert.IsTrue(productId > 0);
		}

		[TestMethod]
		public void Insert_data_builder_no_automapping()
		{
			var productId = Context().Insert("Product")
								.Column("CategoryId", 1)
								.Column("Name", "The Warren Buffet Way")
								.ExecuteReturnLastId();

			Assert.IsTrue(productId > 0);
		}

		[TestMethod]
		public void Insert_data_builder_automapping()
		{
			var product = new Product();
			product.CategoryId = 1;
			product.Name = "The Warren Buffet Way";

			var productId = Context().Insert<Product>("Product", product)
								.AutoMap(x => x.ProductId)
								.ExecuteReturnLastId();

			Assert.IsTrue(productId > 0);
		}

		[TestMethod]
		public void Update_data_sql()
		{
			var rowsAffected = Context().Sql("update Product set Name = @0 where ProductId = @1")
								.Parameters("The Warren Buffet Way", 1)
								.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Update_data_builder()
		{
			var rowsAffected = Context().Update("Product")
								.Column("Name", "The Warren Buffet Way")
								.Where("ProductId", 1)
								.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Update_data_builder_automapping()
		{
			var product = Context().Sql("select * from Product where ProductId = 1")
								.QuerySingle<Product>();
			
			product.Name = "The Warren Buffet Way";

			var rowsAffected = Context().Update<Product>("Product", product)
										.Where(x => x.ProductId)
										.AutoMap(x => x.ProductId)
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Delete_data_sql()
		{
			var productId = Context().Sql("insert into Product(Name, CategoryId) values(@0, @1);")
							.Parameters("The Warren Buffet Way", 1)
							.ExecuteReturnLastId();

			var rowsAffected = Context().Sql("delete from Product where ProductId = @0")
									.Parameters(productId)
									.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Delete_data_builder()
		{
			var productId = Context().Sql(@"insert into Product(Name, CategoryId) values(@0, @1)")
								.Parameters("The Warren Buffet Way", 1)
								.ExecuteReturnLastId();

			var rowsAffected = Context().Delete("Product")
									.Where("ProductId", productId)
									.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Transactions()
		{
			using (var context = Context().UseTransaction)
			{
				context.Sql("update Product set Name = @0 where ProductId = @1")
						.Parameters("The Warren Buffet Way", 1)
						.Execute();

				context.Sql("update Product set Name = @0 where ProductId = @1")
						.Parameters("Bill Gates Bio", 2)
						.Execute();

				context.Commit();
			}
		}

		[TestMethod]
		public void Stored_procedure_sql()
		{
			var rowsAffected = Context().Sql("execute ProductUpdate @ProductId = @0, @Name = @1")
										.Parameters(1, "The Warren Buffet Way")
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Stored_procedure()
		{
			var rowsAffected = Context().Sql("ProductUpdate")
										.CommandType(DbCommandTypes.StoredProcedure)
										.Parameter("ProductId", 1)
										.Parameter("Name", "The Warren Buffet Way")
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Stored_procedure_builder()
		{
			var rowsAffected = Context().StoredProcedure("ProductUpdate")
										.Parameter("Name", "The Warren Buffet Way")
										.Parameter("ProductId", 1).Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void StoredProcedure_builder_automapping()
		{
		    var product = Context().Sql("select * from Product where ProductId = 1")
		                    .QuerySingle<Product>();

		    product.Name = "The Warren Buffet Way";

		    var rowsAffected = Context().StoredProcedure<Product>("ProductUpdate", product)
											.AutoMap(x => x.CategoryId).Execute();

		    Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void StoredProcedure_builder_using_expression()
		{
			var product = Context().Sql("select * from Product where ProductId = 1")
							.QuerySingle<Product>();
			product.Name = "The Warren Buffet Way";

			var rowsAffected = Context().StoredProcedure<Product>("ProductUpdate", product)
											.Parameter(x => x.ProductId)
											.Parameter(x => x.Name).Execute();

			Assert.AreEqual(1, rowsAffected);
		}
	}
}