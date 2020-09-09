using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMS.Common.Extensions;
using TMS.Common.Serialization.Json.Helpers;

#if DEBUG_JSON
using UnityEngine;
using System.Linq;
#endif

namespace TMS.Common.Serialization.Json.Metadata
{
    internal class MetadataHandler
    {
        private readonly IDictionary<Type, ArrayMetadata> _arrayMetadata = new Dictionary<Type, ArrayMetadata>();

        private readonly IDictionary<Type, ObjectMetadata> _objectMetadata = new Dictionary<Type, ObjectMetadata>();

		private readonly IDictionary<Type, IList<PropertyMetadata>> _typeProperties = new Dictionary<Type, IList<PropertyMetadata>>();

		private readonly object _arrayMetadataLock = new object();

		private readonly object _objectMetadataLock = new object();

		private readonly object _typePropertiesLock = new object();
        
#if DEBUG_JSON
		/// <summary>
		///     Gets or sets a value indicating whether [is debug mode].
		/// </summary>
		/// <remarks>DO NOT TURN ON CONSTANTLY! THIS WILL SLOW MAPER'S WORK</remarks>
		/// <value>
		///     <c>true</c> if [is debug mode]; otherwise, <c>false</c>.
		/// </value>
		internal bool IsDebugMode { get; set; }

		/// <summary>
		///     writes given message to debug log
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		internal virtual void Log(string format, params object[] args)
		{
			if (!IsDebugMode) return;
			var log = string.Format("[JSON_MAPPER] " + format, args);
            Debug.Log(log);
		}

#endif

        /// <summary>
		///     Adds the array metadata.
		/// </summary>
		/// <param name="type">The type.</param>
		internal ArrayMetadata AddArrayMetadata(Type type)
		{
            ArrayMetadata data;
			if (_arrayMetadata.ContainsKey(type))
			{
				data = _arrayMetadata[type];
                return data;
			}
			var typeWrapper = type.GetTypeWrapper();
			var isList = typeof(IList).IsAssignableFrom(type);
			data = new ArrayMetadata(type, typeWrapper.IsArray, isList);
						
			var props = typeWrapper.GetProperties();
			foreach (var pInfo in props)
			{
#if DEBUG_JSON
				Log("AddArrayMetadata(prop: {0})", pInfo);
#endif
				if (pInfo.Name != "Item")
				{
					continue;
				}

				var parameters = pInfo.GetIndexParameters();
				if (parameters.Length != 1)
				{
					continue;
				}

				if (parameters[0].ParameterType == typeof (int))
				{
					data.ElementType = pInfo.PropertyType;
				}
			}

			lock (_arrayMetadataLock)
			{
				_arrayMetadata.Add(type, data);
			}
            return data;
		}

        /// <summary>
		///     Adds the object metadata.
		/// </summary>
		/// <param name="type">The type.</param>
		internal ObjectMetadata AddObjectMetadata(Type type)
		{
            ObjectMetadata data;
			if (_objectMetadata.ContainsKey(type))
			{
                data = _objectMetadata[type];
				return data;
			}
			
			data = new ObjectMetadata(type);
			var typeWrapper = type.GetTypeWrapper();

			var props = typeWrapper.GetProperties();
			foreach (var pInfo in props)
			{
				AddPropertyMetadata(pInfo, data);
			}

			var fields = typeWrapper.GetFields();
			foreach (var fInfo in fields)
			{
				AddFieldMetadata(fInfo, data);
			}

			lock (_objectMetadataLock)
			{
				_objectMetadata.Add(type, data);
			}
            return data;
		}

        /// <summary>
		/// Adds the field metadata.
		/// </summary>
		/// <param name="fInfo">The f information.</param>
		/// <param name="data">The data.</param>
		private PropertyMetadata AddFieldMetadata(FieldInfo fInfo, ObjectMetadata data)
		{
			var isIgnorable = ReflectionHelper.IsIgnorableMember(fInfo);
#if DEBUG_JSON
			Log("AddObjectMetadata(field: {0}, IsIngnored: {1})", fInfo, isIgnorable);
#endif
			if (isIgnorable) return null;

			var attr = ReflectionHelper.GetDataMemberAttribute(fInfo);			
            var attrName = (attr != null ? attr.Name : null) ?? fInfo.Name;

			var fData = new PropertyMetadata(fInfo.FieldType, fInfo, true, attr);
			data.Properties.Add(attrName, fData);

            return fData;
		}

		/// <summary>
		/// Adds the property metadata.
		/// </summary>
		/// <param name="pInfo">The p information.</param>
		/// <param name="data">The data.</param>
		internal PropertyMetadata AddPropertyMetadata(PropertyInfo pInfo, ObjectMetadata data)
		{
			var isIgnorable = ReflectionHelper.IsIgnorableMember(pInfo);
#if DEBUG_JSON
			Log("AddObjectMetadata(prop: {0}, IsIngnored: {1})", pInfo, isIgnorable);
#endif
			if (isIgnorable) return null;

			// TODO improve this part to reduce reflection actions
			var attr = ReflectionHelper.GetDataMemberAttribute(pInfo);
			var attrName = (attr != null ? attr.Name : null) ?? pInfo.Name;			

			PropertyMetadata pData;
			if(data.Properties.ContainsKey(attrName))
			{
				pData = data.Properties[attrName];
				return pData;
			}

			// TODO check if we can ensure that this is an indexer			
			if (pInfo.Name == "Item")
			{
				var parameters = pInfo.GetIndexParameters();
				if (parameters.Length != 1)
				{
					return null;
				}
				if (parameters[0].ParameterType == typeof(string))
				{
					data.ElementType = pInfo.PropertyType; // TODO !!!! why reassigning type in this case?
				}
				return null;
			}
			
			pData = new PropertyMetadata(pInfo.PropertyType, pInfo, false, attr);
			data.Properties.Add(attrName, pData);

            return pData;
		}       
		
		/// <summary>
        ///     Adds the type properties.
        /// </summary>
        /// <param name="type">The type.</param>
        internal IList<PropertyMetadata> AddTypeProperties(Type type)
        {
			IList<PropertyMetadata> propsMeta;
            if (_typeProperties.ContainsKey(type))
            {
				propsMeta = _typeProperties[type];
                return propsMeta;
            }
            var typeWrapper = type.GetTypeWrapper();
#if DEBUG_JSON
            if (IsDebugMode)
            {
                Log("AddTypeProperties(typeWrapper.GetProperties())");
            }
#endif
            propsMeta = new List<PropertyMetadata>();
            var props = typeWrapper.GetProperties();
            foreach (var pInfo in props)
            {
                if (pInfo.Name == "Item" || ReflectionHelper.IsIgnorableMember(pInfo)) continue;
				
				var attr = ReflectionHelper.GetDataMemberAttribute(pInfo);
				propsMeta.Add(new PropertyMetadata(pInfo.PropertyType, pInfo, false, attr));
            }
#if DEBUG_JSON
            if (IsDebugMode)
            {
                Log("AddTypeProperties(typeWrapper.GetFields())");
            }
#endif
            var fields = typeWrapper.GetFields();
            foreach (var fInfo in fields)
            {
				if (!fInfo.IsPublic || ReflectionHelper.IsIgnorableMember(fInfo))
				{
					continue;
				}
				var attr = ReflectionHelper.GetDataMemberAttribute(fInfo);
                propsMeta.Add(new PropertyMetadata(fInfo.FieldType, fInfo, true, attr));
            }
#if DEBUG_JSON
            if (IsDebugMode)
            {
                foreach (var prop in propsMeta)
                {
                    Log("AddArrayMetadata(prop: {0})", prop);
                }
            }
#endif
            lock (_typePropertiesLock)
            {
                _typeProperties.Add(type, propsMeta);
            }
			return propsMeta;
        }
    }
}