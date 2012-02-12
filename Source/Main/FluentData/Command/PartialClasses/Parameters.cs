﻿using System.Linq;

namespace FluentData
{
	internal partial class DbCommand
	{
		public IDbCommand Parameters(params object[] parameters)
		{
			for (int i = 0; i < parameters.Count(); i++)
				Parameter(i.ToString(), parameters[i]);
			return this;
		}

		public IDbCommand Parameter(string name, object value, DataTypes parameterType, ParameterDirection direction)
		{
			var parameter = new Parameter();
			parameter.DataTypes = parameterType;
			parameter.ParameterName = name;
			parameter.Direction = direction;
			parameter.Value = value;
			_data.Parameters.Add(parameter);
			return this;
		}

		public IDbCommand Parameter(string name, object value)
		{
			Parameter(name, value, DataTypes.Object, ParameterDirection.Input);
			return this;
		}

		public IDbCommand ParameterOut(string name, DataTypes parameterType)
		{
			if (!_data.DbContextData.DbProvider.SupportsOutputParameters)
				throw new FluentDataException("The selected database does not support output parameters");
			Parameter(name, null, parameterType, ParameterDirection.Output);
			return this;
		}

		public TParameterType ParameterValue<TParameterType>(string outputParameterName)
		{
			outputParameterName = _data.DbContextData.DbProvider.GetParameterName(outputParameterName);
			if (!_data.InnerCommand.Parameters.Contains(outputParameterName))
				throw new FluentDataException(string.Format("Parameter {0} not found", outputParameterName));

			var value = (_data.InnerCommand.Parameters[outputParameterName] as System.Data.IDataParameter).Value;
			if (value == null)
				return default(TParameterType);

			return (TParameterType) value;
		}
	}
}
