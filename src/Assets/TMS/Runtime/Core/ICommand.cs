﻿#region

using System.ComponentModel;

#endregion

// ReSharper disable CheckNamespace

namespace System.Windows.Input
// ReSharper restore CheckNamespace
{
	/// <summary>
	///     Defines a command.
	/// </summary>
	[TypeConverter(
		"System.Windows.Input.CommandConverter, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null"
		)]
	public interface ICommand
	{
		/// <summary>
		///     Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		event EventHandler CanExecuteChanged;

		/// <summary>
		///     Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <returns>
		///     true if this command can be executed; otherwise, false.
		/// </returns>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		bool CanExecute(object parameter);

		/// <summary>
		///     Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		void Execute(object parameter);
	}
}