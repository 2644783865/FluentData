﻿using System.Data;
using System.Linq;

namespace FluentData
{
	public partial class DbContext
	{
		private DbCommand CreateCommand
		{
			get
			{
				IDbConnection connection = null;

				if (Data.UseTransaction
					|| Data.UseSharedConnection)
				{
					if (Data.Connection == null)
						Data.Connection = Data.Provider.CreateConnection(Data.ConnectionString);
					connection = Data.Connection;
				}
				else
					connection = Data.Provider.CreateConnection(Data.ConnectionString);

				var cmd = connection.CreateCommand();
				cmd.Connection = connection;

				return new DbCommand(this, cmd);
			}
		}

		public IDbCommand Sql(string sql, params object[] parameters)
		{
			var command = CreateCommand.Sql(sql).Parameters(parameters);
			return command;
		}

		public IDbCommand MultiResultSql
		{
            get
	        {
	            var command = CreateCommand.UseMultiResult(true);
	            return command;
	        }
	    }
	}
}
