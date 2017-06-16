using System;
using System.Collections.Generic;
using System.Reflection;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Modularity.Regions.Adapters;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// Region Mapping Host
	/// </summary>
	public class RegionMapping : ViewModel, IRegion
	{
		private static readonly Tuple<RegionViewMappingType, Action<RegionViewMappingPayload>>
			RegionViewMappingActionsLocker = new Tuple<RegionViewMappingType, Action<RegionViewMappingPayload>>();

		protected static IDictionary<RegionViewMappingType, Action<RegionMapping, RegionViewMappingPayload>> _regionViewMappingActions;
		
		/// <summary>
		/// Gets the region manager.
		/// </summary>
		/// <value>
		/// The region manager.
		/// </value>
		protected IRegionManager RegionManager
		{
			get
			{
				var man = Resolve<IRegionManager>();
				return man;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public virtual string Name
		{
			get { return gameObject.name; }
			set { gameObject.name = value; }
		}

		/// <summary>
		/// Invoked after <see cref="Awake" />
		/// </summary>
		protected override void Start()
		{
			base.Start();

			Subscribe<RegionViewMappingPayload>(ProcessPayload, payload => payload.RegionName == Name);

			RegionManager.RegisterRegion(this);
		}

		protected override void OnDestroy()
		{
			Unsubscribe<RegionViewMappingPayload>(ProcessPayload);

			if (RegionManager != null)
			{
				RegionManager.UnregisterRegion(this);
			}

			base.OnDestroy();
		}
		
		/// <summary>
		/// Gets the region view mapping actions.
		/// </summary>
		/// <value>
		/// The region view mapping actions.
		/// </value>
		protected static IDictionary<RegionViewMappingType, Action<RegionMapping, RegionViewMappingPayload>> RegionViewMappingActions
		{
			get { return RegionViewMappingActionsLocker.InitWithLock(ref _regionViewMappingActions, GetRegionViewMappingActions); }
		}

		/// <summary>
		/// Gets the region view mapping actions.
		/// </summary>
		/// <returns></returns>
		protected static IDictionary<RegionViewMappingType, Action<RegionMapping, RegionViewMappingPayload>> GetRegionViewMappingActions()
		{
			var actions = new Dictionary<RegionViewMappingType, Action<RegionMapping, RegionViewMappingPayload>>();
			var methods = typeof(RegionMapping).GetMethods(
				BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
			foreach (var method in methods)
			{
				InitRegionViewMappingAction(actions, method);
			}
			return actions;
		}

		/// <summary>
		/// Initializes the region view mapping action.
		/// </summary>
		/// <param name="actions">The actions.</param>
		/// <param name="method">The method.</param>
		protected static void InitRegionViewMappingAction(
			IDictionary<RegionViewMappingType, Action<RegionMapping, RegionViewMappingPayload>> actions,
			MethodInfo method)
		{
			if (method == null) return;

			var attributes = method.GetCustomAttributes(typeof(ViewMappingActionAttribute), true);
			if (attributes.IsNullOrEmpty()) return;

			foreach (ViewMappingActionAttribute attribute in attributes)
			{
				if (attribute == null || actions.ContainsKey(attribute.MappingType)) continue;

				var canProceed = attribute.BeforeRegistration(method.DeclaringType, method);
				if(!canProceed) continue;

				var act = GetRegionViewMappingAction(method, attribute);
				if(act == null) continue;

				actions.Add(attribute.MappingType, act);

				attribute.AfterRegistration(method.DeclaringType, method);
			}
		}

		/// <summary>
		/// Gets the region view mapping action.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="attribute">The attribute.</param>
		/// <returns></returns>
		protected static Action<RegionMapping, RegionViewMappingPayload> GetRegionViewMappingAction(
			MethodInfo method, ViewMappingActionAttribute attribute)
		{
			var parameters = method.GetParameters();
			if (parameters.Length < 2) return null;

			var p1 = parameters[0];
			if (p1 == null || p1.ParameterType != typeof(RegionMapping)) return null;

			var p2 = parameters[1];
			if (p2 == null || p2.ParameterType != typeof(RegionViewMappingPayload)) return null;

			Action<RegionMapping, RegionViewMappingPayload> act =
				(mapping, payload) => InvokeRegionViewMappingAction(method, attribute, mapping, payload);
			return act;
		}

		/// <summary>
		/// Invokes the region view mapping action.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="attribute">The attribute.</param>
		/// <param name="mapping">The mapping.</param>
		/// <param name="payload">The payload.</param>
		protected static void InvokeRegionViewMappingAction(
			MethodInfo method, ViewMappingActionAttribute attribute,
			RegionMapping mapping, RegionViewMappingPayload payload)
		{
			var canProceed = attribute.BeforeAction(method.DeclaringType, method, mapping, payload);
			if(!canProceed) return;

			method.Invoke(mapping, new object[] { mapping, payload });

			attribute.AfterAction(method.DeclaringType, method, mapping, payload);
		}

		[ViewMappingAction(RegionViewMappingType.DestroyRegion)]
		protected static void DestroyRegion(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.DestroyRegion();
		}

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="mapping">The mapping.</param>
		/// <param name="vmp">The VMP.</param>
		[ViewMappingAction(RegionViewMappingType.MapView)]
		protected static void MapView(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			var vm = vmp.View != null ?
						mapping.MapView(vmp.View) :
						mapping.MapView(vmp.PrefabName);
			vm.DataContext = vmp.DataContext;
		}

		/// <summary>
		/// Unmaps the view.
		/// </summary>
		/// <param name="mapping">The mapping.</param>
		/// <param name="vmp">The VMP.</param>
		[ViewMappingAction(RegionViewMappingType.UnmapView)]
		protected static void UnmapView(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			if (vmp.View != null)
			{
				mapping.UnmapView(vmp.View);
			}
			else
			{
				mapping.UnmapView(vmp.PrefabName);
			}
		}

		[ViewMappingAction(RegionViewMappingType.UnmapAllViews)]
		protected static void UnmapAllViews(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.UnmapAllViews();
		}

		[ViewMappingAction(RegionViewMappingType.LockView)]
		protected static void LockView(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			// TODO pass specific view
			mapping.Lock();
		}

		[ViewMappingAction(RegionViewMappingType.UnlockView)]
		protected static void UnlockView(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			// TODO pass specific view
			mapping.Unlock();
		}

		[ViewMappingAction(RegionViewMappingType.LockAllViews)]
		protected static void LockAllViews(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.Lock();
		}

		[ViewMappingAction(RegionViewMappingType.UnlockAllViews)]
		protected static void UnlockAllViews(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.Unlock();
		}

		[ViewMappingAction(RegionViewMappingType.HideView)]
		protected static void HideView(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			
		}

		[ViewMappingAction(RegionViewMappingType.ShowView)]
		protected static void ShowView(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			if (vmp.View != null)
				mapping.ShowView(mapping, vmp.View);
			else
				mapping.ShowView(mapping, vmp.PrefabName);
		}

		[ViewMappingAction(RegionViewMappingType.ShowAllViews)]
		protected static void ShowAllViews(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.ShowAllViews();
		}

		[ViewMappingAction(RegionViewMappingType.HideAllViews)]
		protected static void HideAllViews(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.HideAllViews();
		}

		[ViewMappingAction(RegionViewMappingType.UpdateDataContext)]
		protected static void UpdateDataContext(RegionMapping mapping, RegionViewMappingPayload vmp)
		{
			mapping.UpdateDataContext(vmp.DataContext);
		}

		/// <summary>
		/// Called when [view mapping] payload received.
		/// </summary>
		/// <param name="vmp">The VMP.</param>
		public virtual void ProcessPayload(RegionViewMappingPayload vmp)
		{
			if (!RegionViewMappingActions.ContainsKey(vmp.ViewMappingType))
			{
				LogError("Cannot find action to perform \"{0}\".", vmp.ViewMappingType);
				return;
			}

			var act = RegionViewMappingActions[vmp.ViewMappingType];
			if (act == null)
			{
				LogError("The action for \"{0}\" is NULL.", vmp.ViewMappingType);
				return;
			}

			act(this, vmp);
		}

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <returns></returns>
		public virtual ViewMapping MapView(string prefabName)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.MapView(this, prefabName);
			return vm;
		}

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="view">The view.</param>
		public virtual ViewMapping MapView(GameObject view)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.MapView(this, view);
			return vm;
		}

		/// <summary>
		/// Unmaps the view.
		/// </summary>
		/// <param name="view">The view.</param>
		public virtual ViewMapping UnmapView(GameObject view)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.UnmapView(this, view);
			return vm;
		}

		/// <summary>
		/// Unmaps the view.
		/// </summary>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <returns></returns>
		public virtual ViewMapping UnmapView(string prefabName)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.UnmapView(this, prefabName);
			return vm;
		}

		/// <summary>
		/// Unmaps all views.
		/// </summary>
		public virtual void UnmapAllViews()
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return;

			adapter.UnmapAllViews(this);
		}

		public virtual void HideAllViews()
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return;

			adapter.HideAllViews(this);
		}

		public virtual void ShowAllViews()
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return;

			adapter.ShowAllViews(this);
		}

		public virtual ViewMapping ShowView(RegionMapping target, GameObject view)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.ShowView(this, view);
			return vm;
		}

		public virtual ViewMapping HideView(RegionMapping target, GameObject view)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.HideView(this, view);
			return vm;
		}

		public virtual ViewMapping ShowView(RegionMapping target, string prefabName)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.ShowView(this, prefabName);
			return vm;
		}

		public virtual ViewMapping HideView(RegionMapping target, string prefabName)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.HideView(this, prefabName);
			return vm;
		}

		/// <summary>
		/// Maps the view.
		/// </summary>
		/// <param name="viewCreator">The view creator.</param>
		public virtual ViewMapping MapView(Func<GameObject> viewCreator)
		{
			var view = viewCreator();
			var vm = MapView(view);
			return vm;
		}

		/// <summary>
		/// Locks this instance.
		/// </summary>
		public virtual void Lock()
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return;

			adapter.LockTarget(this);
		}

		/// <summary>
		/// Unlocks this instance.
		/// </summary>
		public virtual void Unlock()
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return;

			adapter.UnlockTarget(this);
		}

		/// <summary>
		/// Destroys the region.
		/// </summary>
		public virtual void DestroyRegion()
		{
			if(gameObject == null) return;
			gameObject.Destroy();
		}

		/// <summary>
		/// Updates all views data context.
		/// </summary>
		/// <param name="dataContext">The data context.</param>
		public virtual void UpdateDataContext(object dataContext)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return;

			adapter.UpdateDataContext(this, dataContext);
		}

		/// <summary>
		/// Updates the data context of specific view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="dataContext">The data context.</param>
		public virtual ViewMapping UpdateDataContext(GameObject view, object dataContext)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.UpdateDataContext(this, view, dataContext);
			return vm;
		}

		/// <summary>
		/// Updates the data context of specific view.
		/// </summary>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <param name="dataContext">The data context.</param>
		public virtual ViewMapping UpdateDataContext(string prefabName, object dataContext)
		{
			var adapter = GetRegionAdapter();
			if (adapter == null) return null;

			var vm = adapter.UpdateDataContext(this, prefabName, dataContext);
			return vm;
		}

		/// <summary>
		/// Gets the region adapter.
		/// </summary>
		/// <returns></returns>
		protected virtual BaseRegionAdapter GetRegionAdapter()
		{
			var adapter = GetComponent<BaseRegionAdapter>();
			if (adapter != null) return adapter;

			LogError("Cannot find '{0}'", typeof(BaseRegionAdapter));
			return null;
		}
	}
}