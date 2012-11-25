﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentData
{
	internal class SelectBuilder<TEntity> : ISelectBuilder<TEntity>
	{
		protected BuilderData Data { get; set; }
		protected ActionsHandler Actions { get; set; }

		private IDbCommand Command
		{
			get
			{
				if (Data.PagingItemsPerPage > 0
					&& string.IsNullOrEmpty(Data.OrderBy))
					throw new FluentDataException("Order by must defined when using Paging.");

				Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForSelectBuilder(Data));
				return Data.Command;
			}
		}

		public SelectBuilder(IDbCommand command)
		{
			Data =  new BuilderData(command, "");
			Actions = new ActionsHandler(Data);
		}

		public ISelectBuilder<TEntity> Select(string sql)
		{
			Data.Select += sql;
			return this;
		}

		public ISelectBuilder<TEntity> Select(string sql, Expression<Func<TEntity, object>> mapToProperty)
		{
			var alias = Data.Command.Data.Context.Data.Provider.GetSelectBuilderAlias(sql, ReflectionHelper.GetPropertyNameFromExpression(mapToProperty).Replace(".", "_"));
			if (Data.Select.Length > 0)
				Data.Select += ",";

			Data.Select += alias;
			return this;
		}

		public ISelectBuilder<TEntity> From(string sql)
		{
			Data.From += sql;
			return this;
		}

		public ISelectBuilder<TEntity> Where(string sql)
		{
			Data.WhereSql += sql;
			return this;
		}

		public ISelectBuilder<TEntity> WhereAnd(string sql)
		{
			if(Data.WhereSql.Length > 0)
				Where(" and ");
			Where(sql);
			return this;
		}

		public ISelectBuilder<TEntity> WhereOr(string sql)
		{
			if(Data.WhereSql.Length > 0)
				Where(" or ");
			Where(sql);
			return this;
		}

		public ISelectBuilder<TEntity> OrderBy(string sql)
		{
			Data.OrderBy += sql;
			return this;
		}

		public ISelectBuilder<TEntity> GroupBy(string sql)
		{
			Data.GroupBy += sql;
			return this;
		}

		public ISelectBuilder<TEntity> Having(string sql)
		{
			Data.Having += sql;
			return this;
		}

		public ISelectBuilder<TEntity> Paging(int currentPage, int itemsPerPage)
		{
			Data.PagingCurrentPage = currentPage;
			Data.PagingItemsPerPage = itemsPerPage;
			return this;
		}

		public ISelectBuilder<TEntity> Parameter(string name, object value)
		{
			Data.Command.Parameter(name, value);
			return this;
		}

		public TList QueryMany<TList>(Action<TEntity, IDataReader> customMapper = null) where TList : IList<TEntity>
		{
			return Command.QueryMany<TEntity, TList>(customMapper);
		}

		public List<TEntity> QueryMany(Action<TEntity, IDataReader> customMapper = null)
		{
			return Command.QueryMany(customMapper);
		}

		public void QueryComplexMany(IList<TEntity> list, Action<IList<TEntity>, IDataReader> customMapper)
		{
			Command.QueryComplexMany<TEntity>(list, customMapper);
		}

		public TEntity QuerySingle(Action<TEntity, IDataReader> customMapper = null)
		{
			return Command.QuerySingle(customMapper);
		}

		public TEntity QueryComplexSingle(Func<IDataReader, TEntity> customMapper)
		{
			return Command.QueryComplexSingle(customMapper);
		}
	}
}
