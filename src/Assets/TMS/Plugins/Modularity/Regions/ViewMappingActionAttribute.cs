using System;
using System.Reflection;

namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// View Mapping Attribute
	/// <remarks>Can be applied to methods with signature void(<see cref="RegionMapping"/>, <see cref="RegionViewMappingPayload"/>)</remarks>
	/// </summary>
	public class ViewMappingActionAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the type of the mapping.
		/// </summary>
		/// <value>
		/// The type of the mapping.
		/// </value>
		public virtual RegionViewMappingType MappingType { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewMappingActionAttribute"/> class.
		/// </summary>
		/// <param name="mappingType">Type of the mapping.</param>
		public ViewMappingActionAttribute(RegionViewMappingType mappingType)
		{
			MappingType = mappingType;
		}

		/// <summary>
		/// Before calling the mapping action.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="method">The method.</param>
		/// <param name="mapping">The mapping.</param>
		/// <param name="payload">The payload.</param>
		protected internal virtual bool BeforeAction(
			Type ownerType, MethodInfo method, 
			RegionMapping mapping, RegionViewMappingPayload payload)
		{
			// TODO
			return true;
		}

		/// <summary>
		/// Afters the action.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="method">The method.</param>
		/// <param name="mapping">The mapping.</param>
		/// <param name="payload">The payload.</param>
		protected internal virtual void AfterAction(
			Type ownerType, MethodInfo method,
			RegionMapping mapping, RegionViewMappingPayload payload)
		{
			// TODO
		}

		/// <summary>
		/// Called before [registration] of attribute is executed.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		protected internal virtual bool BeforeRegistration(Type ownerType, MethodInfo method)
		{
			// TODO
			return true;
		}

		/// <summary>
		/// Called after [registration] of attribute is executed.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="method">The method.</param>
		protected internal virtual void AfterRegistration(Type ownerType, MethodInfo method)
		{
			// TODO
		}
	}
}