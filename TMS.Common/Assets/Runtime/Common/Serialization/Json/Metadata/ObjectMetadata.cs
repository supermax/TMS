#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Object Metadata
	/// </summary>
	internal class ObjectMetadata
	{
		/// <summary>
		///     Gets or sets the type of the element.
		/// </summary>
		/// <value>
		///     The type of the element.
		/// </value>
		public Type ElementType
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this instance is dictionary.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is dictionary; otherwise, <c>false</c>.
		/// </value>
		public bool IsDictionary
		{
			get;
			private set;
		}

		/// <summary>
		///     Gets or sets the properties.
		/// </summary>
		/// <value>
		///     The properties.
		/// </value>
		public IDictionary<string, PropertyMetadata> Properties
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectMetadata"/> class.
		/// </summary>
		/// <param name="elementType">Type of the element.</param>
		internal ObjectMetadata(Type elementType)
		{
			ElementType = typeof(JsonData); // elementType ?? typeof(JsonData); // TODO why alsways fallback => JsonData ?
			if(elementType != null)
			{
				IsDictionary = typeof(IDictionary).IsAssignableFrom(elementType);
			}
			Properties = new Dictionary<string, PropertyMetadata>();
		}
	}
}