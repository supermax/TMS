#region Usings

using System;
using System.ComponentModel;

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	///     Interface for Observable
	/// </summary>
	public interface IObservable : IDisposable, INotifyPropertyChanged
#if !NETFX_CORE && !WINDOWS_PHONE && !SILVERLIGHT && !UNITY3D
, INotifyPropertyChanging
#endif
	{
	}
}