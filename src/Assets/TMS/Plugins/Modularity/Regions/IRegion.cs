using System;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// Interface for RegionMapping
	/// </summary>
	public interface IRegion : IDisposable
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; set; }

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="view">The view.</param>
		ViewMapping MapView(GameObject view);

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <returns></returns>
		ViewMapping MapView(string prefabName);

		/// <summary>
		/// Unmaps the view.
		/// </summary>
		/// <param name="view">The view.</param>
		ViewMapping UnmapView(GameObject view);

		/// <summary>
		/// Unmaps the view.
		/// </summary>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <returns></returns>
		ViewMapping UnmapView(string prefabName);

		/// <summary>
		/// Unmaps all views.
		/// </summary>
		void UnmapAllViews();

		/// <summary>
		/// Hides all views.
		/// </summary>
		void HideAllViews();

		/// <summary>
		/// Destroys the region.
		/// </summary>
		void DestroyRegion();

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="viewCreator">The view creator.</param>
		ViewMapping MapView(Func<GameObject> viewCreator);

		/// <summary>
		/// Locks this instance.
		/// </summary>
		void Lock();

		/// <summary>
		/// Unlocks this instance.
		/// </summary>
		void Unlock();

		/// <summary>
		/// Updates all views data context.
		/// </summary>
		/// <param name="dataContext">The data context.</param>
		void UpdateDataContext(object dataContext);

		/// <summary>
		/// Updates the data context of specific view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="dataContext">The data context.</param>
		/// <returns></returns>
		ViewMapping UpdateDataContext(GameObject view, object dataContext);

		/// <summary>
		/// Updates the data context of specific view.
		/// </summary>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <param name="dataContext">The data context.</param>
		/// <returns></returns>
		ViewMapping UpdateDataContext(string prefabName, object dataContext);

		/// <summary>
		/// Processes the payload.
		/// </summary>
		/// <param name="vmp">The VMP.</param>
		void ProcessPayload(RegionViewMappingPayload vmp);
	}
}