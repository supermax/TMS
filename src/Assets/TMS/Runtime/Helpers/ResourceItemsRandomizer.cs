#region Code Editor
// Owner
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using TMS.Common.Core;
using TMS.Common.Extensions;

namespace TMS.Common.Helpers
{
	/// <summary>
	/// Resource Items Randomizer
	/// </summary>
	public class ResourceItemsRandomizer
	{
		/// <summary>
		/// The randomizer static var
		/// </summary>
		private static readonly Random Rnd = new Random(1);

		/// <summary>
		/// The resource items var
		/// </summary>
		private readonly ResourceItemCollection _items = new ResourceItemCollection();

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>
		/// The items.
		/// </value>
		public ObservableCollectionEx<IResourceItem> Items
		{
			get { return _items; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [force sequence].
		/// </summary>
		/// <value>
		///   <c>true</c> if [force sequence]; otherwise, <c>false</c>.
		/// </value>
		public bool ForceSequence { get; set; } //TODO

		/// <summary>
		/// Gets the random item.
		/// </summary>
		/// <value>
		/// The random item.
		/// </value>
		public object RandomItem
		{
			get
			{
				if (Items.Count < 1) return null;
				var index = Rnd.Next(0, Items.Count);
				//Debug.WriteLine("[{0}]: Items.Count: {1}, RandomItemIndex: {2}", this, Items.Count, index);
				var item = Items[index];
				return item.Resource;
			}
		}
	}

	/// <summary>
	/// Interface for Resource Item
	/// </summary>
	public interface IResourceItem
	{
		object Resource { get; set; }
	}

	/// <summary>
	/// Resource Item
	/// </summary>
	public class ResourceItem : Observable, IResourceItem
	{
		private object _resource;

		/// <summary>
		/// Gets or sets the resource.
		/// </summary>
		/// <value>
		/// The resource.
		/// </value>
		public object Resource
		{
			get { return _resource; }
			set { SetValue(ref _resource, value); }
		}
	}

	/// <summary>
	/// The Collection of <see cref="ResourceItem"/>
	/// </summary>
	public class ResourceItemCollection : ObservableCollectionEx<IResourceItem>
	{
		/// <summary>
		/// Gets a <see cref="T:System.Collections.Generic.IList`1" /> wrapper around the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
		/// </summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IList`1" /> wrapper around the <see cref="T:System.Collections.ObjectModel.Collection`1" />.</returns>
		public new IList<IResourceItem> Items
		{
			get { return base.Items; }
		}

		/// <summary>
		/// Raises the items changed event.
		/// </summary>
		public void RaiseItemsChangedEvent()
		{
			OnPropertyChanged(new PropertyChangedEventArgs(this.GetMemberName(() => Items)));
		}
	}
}