using System;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Modularity
{
	/// <summary>
	/// Module Info
	/// </summary>
	[JsonDataContract]
	public class ModuleInfo
	{
		/// <summary>
		/// Initializes the static instance of <see cref="ModuleInfo"/> class.
		/// </summary>
		static ModuleInfo()
		{
			JsonMapper.Default.RegisterImporter<string, ModulePriorityType>(
				input => (ModulePriorityType)Enum.Parse(typeof(ModulePriorityType), input));
		}

		/// <summary>
		///     Gets or sets the assembly path.
		/// </summary>
		/// <value>
		///     The assembly path.
		/// </value>
		[JsonDataMember("assemblyPath")]
		public string AssemblyPath { get; set; }

		/// <summary>
		///     Gets or sets the name of the module class.
		/// </summary>
		/// <value>
		///     The name of the module class.
		/// </value>
		[JsonDataMember("moduleClassName")]
		public string ModuleClassName { get; set; }

		/// <summary>
		///     Gets or sets the name of the module.
		/// </summary>
		/// <value>
		///     The name of the module.
		/// </value>
		[JsonDataMember("moduleName")]
		public string ModuleName { get; set; }

		/// <summary>
		/// Gets or sets the init method.
		/// </summary>
		/// <value>
		/// The init method.
		/// </value>
		[JsonDataMember("initMethod")]
		public ModuleInitMethodType InitMethod { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [configure IOC container].
		/// </summary>
		/// <value>
		///   <c>true</c> if [configure IOC container]; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMember("configureIoc")]
		public bool ConfigureIoc { get; set; }

		/// <summary>
		/// Gets or sets the type of the module priority.
		/// </summary>
		/// <value>
		/// The type of the module priority.
		/// </value>
		[JsonDataMember("priorityType")]
		public ModulePriorityType PriorityType { get; set; }
	}

	/// <summary>
	/// Module Initialization Method Types
	/// </summary>
	public enum ModuleInitMethodType
	{
		/// <summary>
		/// Initialize module immediately
		/// </summary>
		Init = 0,

		/// <summary>
		/// Create instance, but don't initialize
		/// </summary>
		Instance,

		/// <summary>
		/// Do nothing
		/// </summary>
		None,
	}

	/// <summary>
	/// Module Priority Type
	/// </summary>
	public enum ModulePriorityType
	{
		/// <summary>
		/// Critical, in case of error during init\load other modules will not be loaded
		/// </summary>
		Critical,

		/// <summary>
		/// The none critical
		/// </summary>
		NotCritical
	}
}