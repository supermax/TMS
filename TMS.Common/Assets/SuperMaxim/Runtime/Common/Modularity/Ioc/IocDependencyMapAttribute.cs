using System;

namespace TMS.Common.Modularity.Ioc
{
	/// <summary>
	/// IOC Dependency Mapping Attribute
	/// </summary>
	/// <seealso cref="Attribute" />
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property)]
	public class IocDependencyMapAttribute : Attribute
	{

	}
}