using System;

namespace TMS.Common.Modularity
{
	/// <summary>
	/// IOC Dependecy Mapping Attribute
	/// </summary>
	/// <seealso cref="Attribute" />
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property)]
	public class IocDependencyMapAttribute : Attribute
	{

	}
}