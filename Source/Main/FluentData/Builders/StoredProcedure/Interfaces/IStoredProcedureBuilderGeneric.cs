using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentData
{
	public interface IStoredProcedureBuilder<T> : IBaseStoredProcedureBuilder, IDisposable
	{
		IStoredProcedureBuilder<T> AutoMap();
		IStoredProcedureBuilder<T> IgnoreProperty<TProp>(Expression<Func<T, TProp>> expression);
		IStoredProcedureBuilder<T> Parameter<TProp>(Expression<Func<T, TProp>> expression);
		IStoredProcedureBuilder<T> Parameter(string name, object value);
		IStoredProcedureBuilder<T> ParameterOut(string name, DataTypes parameterType);
	}
}