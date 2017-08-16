using System.Collections.ObjectModel;

// ReSharper disable CheckNamespace
namespace System.Collections.Specialized
// ReSharper restore CheckNamespace
{
	/// <summary>
	///     Notifies listeners of dynamic changes, such as when items get added and removed or the whole list is refreshed.
	/// </summary>
	public interface INotifyCollectionChanged
	{
		/// <summary>
		///     Occurs when the collection changes.
		/// </summary>
		event NotifyCollectionChangedEventHandler CollectionChanged;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
	public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

	/// <summary>
	/// 
	/// </summary>
	public enum NotifyCollectionChangedAction
	{
		/// <summary>
		/// New item(s) added
		/// </summary>
		Add,

		/// <summary>
		/// Item(s) removed
		/// </summary>
		Remove,

		/// <summary>
		/// Item(s) replaced
		/// </summary>
		Replace,

		/// <summary>
		/// Item(s) moved
		/// </summary>
		Move,

		/// <summary>
		/// The collection was reset (cleaned or item(s) range added)
		/// </summary>
		Reset,
	}

	/// <summary>
	///     Provides data for the <see cref="E:System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged" /> event.
	/// </summary>
	public class NotifyCollectionChangedEventArgs : EventArgs
	{
		private NotifyCollectionChangedAction _action;
		private int _newStartingIndex = -1;
		private int _oldStartingIndex = -1;

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This must be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     .
		/// </param>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
		{
			if (action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException("action");
			}
			InitializeAdd(action, null, -1);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a one-item change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     ,
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Add" />
		///     , or
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Remove" />
		///     .
		/// </param>
		/// <param name="changedItem">The item that is affected by the change.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Reset, Add, or Remove, or if <paramref name="action" /> is Reset and
		///     <paramref
		///         name="changedItem" />
		///     is not null.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
		{
			if (action != NotifyCollectionChangedAction.Add && 
				action != NotifyCollectionChangedAction.Remove &&
			    action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException("action");
			}

			if (action == NotifyCollectionChangedAction.Reset)
			{
				if (changedItem != null)
					throw new ArgumentException("action");
				InitializeAdd(action, null, -1);
			}
			else
			{
				InitializeAddOrRemove(action, new[] { changedItem }, -1);
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a one-item change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     ,
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Add" />
		///     , or
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Remove" />
		///     .
		/// </param>
		/// <param name="changedItem">The item that is affected by the change.</param>
		/// <param name="index">The index where the change occurred.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Reset, Add, or Remove, or if <paramref name="action" /> is Reset and either
		///     <paramref
		///         name="changedItem" />
		///     is not null or <paramref name="index" /> is not -1.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
		{
			if (action != NotifyCollectionChangedAction.Add && 
				action != NotifyCollectionChangedAction.Remove &&
			    action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException("action");
			}

			if (action == NotifyCollectionChangedAction.Reset)
			{
				if (changedItem != null)
				{
					throw new ArgumentException("action");
				}
				if (index != -1)
				{
					throw new ArgumentException("action");
				}
				InitializeAdd(action, null, -1);
			}
			else
			{
				InitializeAddOrRemove(action, new[] { changedItem }, index);
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a multi-item change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     ,
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Add" />
		///     , or
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Remove" />
		///     .
		/// </param>
		/// <param name="changedItems">The items that are affected by the change.</param>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
		{
			if (action != NotifyCollectionChangedAction.Add && 
				action != NotifyCollectionChangedAction.Remove &&
			    action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException("action");
			}

			if (action == NotifyCollectionChangedAction.Reset)
			{
				if (changedItems != null)
				{
					throw new ArgumentException("action");
				}
				InitializeAdd(action, null, -1);
			}
			else
			{
				if (changedItems == null)
				{
					throw new ArgumentNullException("changedItems");
				}
				InitializeAddOrRemove(action, changedItems, -1);
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a multi-item change or a
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" />
		///     ,
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Add" />
		///     , or
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Remove" />
		///     .
		/// </param>
		/// <param name="changedItems">The items affected by the change.</param>
		/// <param name="startingIndex">The index where the change occurred.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Reset, Add, or Remove, if <paramref name="action" /> is Reset and either
		///     <paramref
		///         name="changedItems" />
		///     is not null or <paramref name="startingIndex" /> is not -1, or if action is Add or Remove and
		///     <paramref
		///         name="startingIndex" />
		///     is less than -1.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///     If <paramref name="action" /> is Add or Remove and <paramref name="changedItems" /> is null.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
		{
			if (action != NotifyCollectionChangedAction.Add &&
			    action != NotifyCollectionChangedAction.Remove &&
			    action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException("action");
			}

			if (action == NotifyCollectionChangedAction.Reset)
			{
				if (changedItems != null)
				{
					throw new ArgumentException("action");
				}
				if (startingIndex != -1)
				{
					throw new ArgumentException("action");
				}

				InitializeAdd(action, null, -1);
			}
			else
			{
				if (changedItems == null)
				{
					throw new ArgumentNullException("changedItems");
				}
				if (startingIndex < -1)
				{
					throw new ArgumentException("startingIndex");
				}

				InitializeAddOrRemove(action, changedItems, startingIndex);
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a one-item
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can only be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     .
		/// </param>
		/// <param name="newItem">The new item that is replacing the original item.</param>
		/// <param name="oldItem">The original item that is replaced.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Replace.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException("action");
			}

			InitializeMoveOrReplace(action, new[] { newItem }, new[] { oldItem }, -1, -1);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a one-item
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     .
		/// </param>
		/// <param name="newItem">The new item that is replacing the original item.</param>
		/// <param name="oldItem">The original item that is replaced.</param>
		/// <param name="index">The index of the item being replaced.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Replace.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem,
												int index)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException("action");
			}

			InitializeMoveOrReplace(action, new[] { newItem }, new[] { oldItem }, index, index);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a multi-item
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can only be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     .
		/// </param>
		/// <param name="newItems">The new items that are replacing the original items.</param>
		/// <param name="oldItems">The original items that are replaced.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Replace.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///     If <paramref name="oldItems" /> or <paramref name="newItems" /> is null.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException("action");
			}
			
			if (newItems == null)
			{
				throw new ArgumentNullException("newItems");
			}
			
			if (oldItems == null)
			{
				throw new ArgumentNullException("oldItems");
			}

			InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a multi-item
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can only be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     .
		/// </param>
		/// <param name="newItems">The new items that are replacing the original items.</param>
		/// <param name="oldItems">The original items that are replaced.</param>
		/// <param name="startingIndex">The index of the first item of the items that are being replaced.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Replace.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///     If <paramref name="oldItems" /> or <paramref name="newItems" /> is null.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems,
												int startingIndex)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException("action");
			}
			
			if (newItems == null)
			{
				throw new ArgumentNullException("newItems");
			}
			
			if (oldItems == null)
			{
				throw new ArgumentNullException("oldItems");
			}
			
			InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a one-item
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can only be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />
		///     .
		/// </param>
		/// <param name="changedItem">The item affected by the change.</param>
		/// <param name="index">The new index for the changed item.</param>
		/// <param name="oldIndex">The old index for the changed item.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Move or <paramref name="index" /> is less than 0.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index,
												int oldIndex)
		{
			if (action != NotifyCollectionChangedAction.Move)
			{
				throw new ArgumentException("action");
			}

			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			
			var objArray = new[] { changedItem };
			InitializeMoveOrReplace(action, objArray, objArray, index, oldIndex);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> class that describes a multi-item
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />
		///     change.
		/// </summary>
		/// <param name="action">
		///     The action that caused the event. This can only be set to
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />
		///     .
		/// </param>
		/// <param name="changedItems">The items affected by the change.</param>
		/// <param name="index">The new index for the changed items.</param>
		/// <param name="oldIndex">The old index for the changed items.</param>
		/// <exception cref="T:System.ArgumentException">
		///     If <paramref name="action" /> is not Move or <paramref name="index" /> is less than 0.
		/// </exception>
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index,
												int oldIndex)
		{
			if (action != NotifyCollectionChangedAction.Move)
			{
				throw new ArgumentException("action");
			}
			
			if (index < 0)
			{
				throw new ArgumentException("index");
			}

			InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="newItems">The new items.</param>
		/// <param name="oldItems">The old items.</param>
		/// <param name="newIndex">The new index.</param>
		/// <param name="oldIndex">The old index.</param>
		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, 
													ICollection newItems, ICollection oldItems,
													int newIndex, int oldIndex)
		{
			_action = action;
			NewItems = newItems == null ? null : CreateReadOnlyCollection(newItems);
			OldItems = oldItems == null ? null : CreateReadOnlyCollection(oldItems);
			_newStartingIndex = newIndex;
			_oldStartingIndex = oldIndex;
		}

		/// <summary>
		///     Gets the action that caused the event.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedAction" /> value that describes the action that caused the event.
		/// </returns>
		public NotifyCollectionChangedAction Action
		{
			get { return _action; }
		}

		/// <summary>
		///     Gets the list of new items involved in the change.
		/// </summary>
		/// <returns>
		///     The list of new items involved in the change.
		/// </returns>
		public IList NewItems { get; private set; }

		/// <summary>
		///     Gets the list of items affected by a
		///     <see
		///         cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />
		///     , Remove, or Move action.
		/// </summary>
		/// <returns>
		///     The list of items affected by a <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />, Remove, or Move action.
		/// </returns>
		public IList OldItems { get; private set; }

		/// <summary>
		///     Gets the index at which the change occurred.
		/// </summary>
		/// <returns>
		///     The zero-based index at which the change occurred.
		/// </returns>
		public int NewStartingIndex
		{
			get { return _newStartingIndex; }
		}

		/// <summary>
		///     Gets the index at which a <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />, Remove, or Replace action occurred.
		/// </summary>
		/// <returns>
		///     The zero-based index at which a <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />, Remove, or Replace action occurred.
		/// </returns>
		public int OldStartingIndex
		{
			get { return _oldStartingIndex; }
		}

		private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
		{
			if (action == NotifyCollectionChangedAction.Add)
			{
				InitializeAdd(action, changedItems, startingIndex);
			}
			else
			{
				if (action != NotifyCollectionChangedAction.Remove) return;
				InitializeRemove(action, changedItems, startingIndex);
			}
		}

		private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
		{
			_action = action;
			NewItems = newItems == null ? null : CreateReadOnlyCollection(newItems);
			_newStartingIndex = newStartingIndex;
		}

		private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
		{
			_action = action;
			OldItems = oldItems == null ? null : CreateReadOnlyCollection(oldItems);
			_oldStartingIndex = oldStartingIndex;
		}

		private static IList CreateReadOnlyCollection(ICollection originalItems)
		{
			var newItems = new object[originalItems.Count];
			originalItems.CopyTo(newItems, 0);
			var col = new ReadOnlyCollection<object>(newItems);
			return col;
		}

		private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems,
											 int startingIndex, int oldStartingIndex)
		{
			InitializeAdd(action, newItems, startingIndex);
			InitializeRemove(action, oldItems, oldStartingIndex);
		}
	}
}
