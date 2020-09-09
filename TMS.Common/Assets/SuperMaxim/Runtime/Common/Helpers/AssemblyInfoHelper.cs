#if !UNITY_WSA
#region Usings

using System.Reflection;
using Application = UnityEngine.Application;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     Assembly Info Helper
	/// </summary>
	public static class AssemblyInfoHelper
	{
		/// <summary>
		///     Gets the assembly info.
		/// </summary>
		public static AssemblyName AssemblyInfo
		{
			get
			{
				if (Application.isEditor)
				{					
					return Assembly.GetExecutingAssembly().GetName();
				}
				var ass = Assembly.GetEntryAssembly();
				var name = ass.GetName();
				return name;
			}
		}
	}
}
#endif