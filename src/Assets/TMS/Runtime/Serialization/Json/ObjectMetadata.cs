#region

using System;
using System.Collections.Generic;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Object Metadata
	/// </summary>
	internal class ObjectMetadata
	{
		/// <summary>
		///     The _element type
		/// </summary>
		private Type _elementType;

		/// <summary>
		///     Gets or sets the type of the element.
		/// </summary>
		/// <value>
		///     The type of the element.
		/// </value>
		public Type ElementType
		{
			get { return _elementType ?? typeof (JsonData); }
			set { _elementType = value; }
		}

		/// <summary>
		///     Gets or sets a value indicating whether this instance is dictionary.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is dictionary; otherwise, <c>false</c>.
		/// </value>
		public bool IsDictionary { get; set; }

		/// <summary>
		///     Gets or sets the properties.
		/// </summary>
		/// <value>
		///     The properties.
		/// </value>
		public IDictionary<string, PropertyMetadata> Properties { get; set; }
	}
}