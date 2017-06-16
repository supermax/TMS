#region

using System;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Array Metadata
	/// </summary>
	internal struct ArrayMetadata
	{
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
		///     Gets or sets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		public bool IsArray { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether this instance is list.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is list; otherwise, <c>false</c>.
		/// </value>
		public bool IsList { get; set; }
	}
}