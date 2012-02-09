﻿using System;
using System.Collections.Generic;

namespace FluentData
{
	internal abstract class BaseStoredProcedureBuilder
	{
		protected BuilderData Data { get; set; }
		protected ActionsHandler Actions { get; set; }

		private IDbCommand Command
		{
			get
			{
				Data.DbCommand.CommandType(DbCommandTypes.StoredProcedure);
				Data.DbCommand.Sql(Data.DbProvider.GetSqlForStoredProcedureBuilder(Data));
				return Data.DbCommand;
			}
		}

		public BaseStoredProcedureBuilder(IDbProvider provider, IDbCommand command, string name)
		{
			Data =  new BuilderData(provider, command, name);
			Actions = new ActionsHandler(Data);
		}


		public void Dispose()
		{
			Command.Dispose();
		}

		public TParameterType ParameterValue<TParameterType>(string outputParameterName)
		{
			return Command.ParameterValue<TParameterType>(outputParameterName);
		}

		public int Execute()
		{
			return Command.Execute();
		}

		public List<dynamic> Query()
		{
			return Command.Query();
		}

		public TList Query<TEntity, TList>() where TList : IList<TEntity>
		{
			return Command.Query<TEntity, TList>();
		}

		public TList Query<TEntity, TList>(Action<dynamic, TEntity> customMapper) where TList : IList<TEntity>
		{
			return Command.Query<TEntity, TList>(customMapper);
		}

		public TList Query<TEntity, TList>(Action<IDataReader, TEntity> customMapper) where TList : IList<TEntity>
		{
			return Command.Query<TEntity, TList>(customMapper);
		}

		public List<TEntity> Query<TEntity>()
		{
			return Command.Query<TEntity>();
		}

		public List<TEntity> Query<TEntity>(Action<dynamic, TEntity> customMapper)
		{
			return Command.Query<TEntity>(customMapper);
		}

		public List<TEntity> Query<TEntity>(Action<IDataReader, TEntity> customMapper)
		{
			return Command.Query<TEntity>(customMapper);
		}

		public TList QueryComplex<TEntity, TList>(Action<IDataReader, IList<TEntity>> customMapper) where TList : IList<TEntity>
		{
			return Command.QueryComplex<TEntity, TList>(customMapper);
		}

		public List<TEntity> QueryComplex<TEntity>(Action<IDataReader, IList<TEntity>> customMapper)
		{
			return Command.QueryComplex<TEntity>(customMapper);
		}

		public TList QueryNoAutoMap<TEntity, TList>(Action<dynamic, TEntity> customMapper) where TList : IList<TEntity>
		{
			return Command.QueryNoAutoMap<TEntity, TList>(customMapper);
		}

		public TList QueryNoAutoMap<TEntity, TList>(Action<IDataReader, TEntity> customMapper) where TList : IList<TEntity>
		{
			return Command.QueryNoAutoMap<TEntity, TList>(customMapper);
		}

		public List<TEntity> QueryNoAutoMap<TEntity>(Action<dynamic, TEntity> customMapper)
		{
			return Command.QueryNoAutoMap<TEntity>(customMapper);
		}

		public List<TEntity> QueryNoAutoMap<TEntity>(Action<IDataReader, TEntity> customMapper)
		{
			return Command.QueryNoAutoMap<TEntity>(customMapper);
		}

		public dynamic QuerySingle()
		{
			return Command.QuerySingle();
		}

		public TEntity QuerySingle<TEntity>()
		{
			return Command.QuerySingle<TEntity>();
		}

		public TEntity QuerySingle<TEntity>(Action<IDataReader, TEntity> customMapper)
		{
			return Command.QuerySingle<TEntity>(customMapper);
		}

		public T QueryValue<T>()
		{
			return Command.QueryValue<T>();
		}
	}
}
