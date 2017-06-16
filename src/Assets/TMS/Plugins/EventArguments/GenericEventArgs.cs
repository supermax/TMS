#region Usings

using System;

#endregion

namespace TMS.Common.EventArguments
{
	/// <summary>
	///     Generic EventArgs
	/// </summary>
	/// <typeparam name="T">Type of the parameter</typeparam>
	public class GenericEventArgs<T> : EventArgs
	{
		/// <summary>
		///     Gets Default Instance of Type
		/// </summary>
		public static readonly GenericEventArgs<T> Default = new GenericEventArgs<T>(default(T));

		/// <summary>
		///     Initializes a new instance of the <see cref="GenericEventArgs&lt;T&gt;" /> class.
		/// </summary>
		/// <param name="arg">The argument.</param>
		public GenericEventArgs(T arg)
		{
			Argument = arg;
		}

		/// <summary>
		///     Gets or sets the argument.
		/// </summary>
		/// <value>The argument.</value>
		public T Argument { get; private set; }
	}
}