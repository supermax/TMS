#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMS.Common.Core;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Helpers
{
	// TODO improve cache
	/// <summary>
	/// Data Mapper
	/// </summary>
	/// <seealso cref="TMS.Common.Core.Singleton{TMS.Common.Helpers.DataMapper}" />
	public class DataMapper : Singleton<DataMapper>
	{
		private readonly IDictionary<Type, PropertyInfo[]> _cachedProps = new Dictionary<Type, PropertyInfo[]>();

		/// <summary>
		/// Copies the property values.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <param name="target">The target.</param>
		/// <param name="ignoreProps">The ignore props.</param>
		public void CopyPropertyValues(IDictionary<string, object> values, object target, params string[] ignoreProps)
		{
			var targetType = target.GetType();
			PropertyInfo[] targetProps;
			if (_cachedProps.ContainsKey(targetType))
			{
				targetProps = _cachedProps[targetType];
			}
			else
			{
#if !NETFX_CORE
				targetProps = targetType.GetProperties();
#else
				targetProps = targetType.GetRuntimeProperties().ToArray();
#endif
				_cachedProps[targetType] = targetProps;
			}

			foreach (var value in values)
			{
				foreach (var targetProp in targetProps)
				{
					if (value.Key != targetProp.Name || !targetProp.CanWrite) continue;

					var ignoreProp = ignoreProps.Any(t => t == value.Key);
					if (ignoreProp) continue;

					targetProp.SetValue(target, value.Value, null);
				}
			}
		}

		/// <summary>
		/// Copies the property values.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="ignoreProps">The ignore props.</param>
		/// <exception cref="System.OperationCanceledException">The source and the target are the same instance!</exception>
		public void CopyPropertyValues(object source, object target, params string[] ignoreProps)
		{
			if (ReferenceEquals(source, target))
			{
				throw new OperationCanceledException("The source and the target are the same instance!");
			}

			var sourceType = source.GetType();
			PropertyInfo[] sourceProps;
			if (_cachedProps.ContainsKey(sourceType))
			{
				sourceProps = _cachedProps[sourceType];
			}
			else
			{
#if !NETFX_CORE
				sourceProps = sourceType.GetProperties();
#else
				sourceProps = sourceType.GetRuntimeProperties().ToArray();
#endif
				_cachedProps[sourceType] = sourceProps;
			}

			var targetType = target.GetType();
			PropertyInfo[] targetProps;
			if (_cachedProps.ContainsKey(targetType))
			{
				targetProps = _cachedProps[targetType];
			}
			else
			{
#if !NETFX_CORE
				targetProps = targetType.GetProperties();
#else
				targetProps = targetType.GetRuntimeProperties().ToArray();
#endif
				_cachedProps[targetType] = targetProps;
			}

			foreach (var sourceProp in sourceProps)
			{
				foreach (var targetProp in targetProps)
				{
					if (sourceProp.Name != targetProp.Name ||
					    sourceProp.PropertyType != targetProp.PropertyType ||
					    !sourceProp.CanRead || !targetProp.CanWrite) continue;

					var ignoreProp = ignoreProps.Any(t => t == sourceProp.Name);
					if (ignoreProp) continue;

					var value = sourceProp.GetValue(source, null);
					targetProp.SetValue(target, value, null);
				}
			}
		}

		/// <summary>
		/// Copies the property values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="ignoreProps">The ignore props.</param>
		/// <exception cref="System.OperationCanceledException">The source and the target are the same instance!</exception>
		public void CopyPropertyValues<T>(T source, T target, params string[] ignoreProps)
		{
			if (ReferenceEquals(source, target))
			{
				throw new OperationCanceledException("The source and the target are the same instance!");
			}

			var sourceType = source.GetType();
			PropertyInfo[] props;
			if (_cachedProps.ContainsKey(sourceType))
			{
				props = _cachedProps[sourceType];
			}
			else
			{
#if !NETFX_CORE
				props = sourceType.GetProperties();
#else
				props = sourceType.GetRuntimeProperties().ToArray();
#endif
				_cachedProps[sourceType] = props;
			}

			foreach (var prop in props)
			{
				if (!prop.CanRead || !prop.CanWrite) continue;

				var ignoreProp = ignoreProps.Any(t => t == prop.Name);
				if (ignoreProp) continue;

				var value = prop.GetValue(source, null);
				prop.SetValue(target, value, null);
			}
		}

		/// <summary>
		/// Copies the property values.
		/// </summary>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="target">The target.</param>
		/// <param name="ignoreProps">The ignore props.</param>
		/// <exception cref="System.OperationCanceledException">The source and the target are the same instance!</exception>
		public void CopyPropertyValues(Type sourceType, object target, params string[] ignoreProps)
		{
			if (ReferenceEquals(sourceType, target))
			{
				throw new OperationCanceledException("The source and the target are the same instance!");
			}

			PropertyInfo[] sourceProps;
			if (_cachedProps.ContainsKey(sourceType))
			{
				sourceProps = _cachedProps[sourceType];
			}
			else
			{
#if !NETFX_CORE
				sourceProps = sourceType.GetProperties();
#else
				sourceProps = sourceType.GetRuntimeProperties().ToArray();
#endif
				_cachedProps[sourceType] = sourceProps;
			}

			var targetType = target.GetType();
			PropertyInfo[] targetProps;
			if (_cachedProps.ContainsKey(targetType))
			{
				targetProps = _cachedProps[targetType];
			}
			else
			{
#if !NETFX_CORE
				targetProps = targetType.GetProperties();
#else
				targetProps = targetType.GetRuntimeProperties().ToArray();
#endif
				_cachedProps[targetType] = targetProps;
			}

			foreach (var sourceProp in sourceProps)
			{
				foreach (var targetProp in targetProps)
				{
					if (sourceProp.Name != targetProp.Name ||
						sourceProp.PropertyType != targetProp.PropertyType ||
						!sourceProp.CanRead || !targetProp.CanWrite) continue;

					var ignoreProp = ignoreProps.Any(t => t == sourceProp.Name);
					if (ignoreProp) continue;

					var value = sourceProp.GetValue(null, null);
					targetProp.SetValue(target, value, null);
				}
			}
		}

		// TODO require serializable object
		/// <summary>
		/// Clones the specified source.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="src">The source.</param>
		/// <returns></returns>
		public T Clone<T>(T src) where T : new()
		{
			var str = JsonMapper.Default.ToJson(src);
			var clone = JsonMapper.Default.ToObject<T>(str);
			return clone;
		}
	}
}