#region

using System;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Array Metadata
	/// </summary>
	internal class ArrayMetadata
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
		///     Gets or sets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		public bool IsArray 
		{ 
			get; 
			private set; 
		}

		/// <summary>
		///     Gets or sets a value indicating whether this instance is list.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is list; otherwise, <c>false</c>.
		/// </value>
		public bool IsList 
		{ 
			get; 
			private set; 
		}

		internal ArrayMetadata(Type elementType, bool isArray, bool isList)
		{
			ElementType = typeof (JsonData);  //elementType ?? typeof (JsonData); 
			IsArray = isArray;
			IsList = isList;
		}
	}
}