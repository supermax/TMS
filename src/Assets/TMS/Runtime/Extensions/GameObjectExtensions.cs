using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMS.Common.Extensions
{
	/// <summary>
	/// Game Object Extensions
	/// </summary>
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Creates the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static IDelegateCommand CreateCommand<T>(this GameObject target, Action<T> method)
		{
			var cmd = target.AddComponent<DelegateCommand>();
			cmd.Init(o => method((T)o));
			return cmd;
		}

		/// <summary>
		/// Creates the command.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static IDelegateCommand CreateCommand(this GameObject target, Action method)
		{
			var cmd = target.AddComponent<DelegateCommand>();
			cmd.Init(o => method());
			return cmd;
		}

		/// <summary>
		/// Destroys the command.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		public static IDelegateCommand DestroyCommand(this GameObject target, IDelegateCommand command)
		{
			var commands = target.GetComponents<DelegateCommand>();
			if (commands == null) return null;

			DelegateCommand res = null;
			foreach (var cmd in commands)
			{
				if (!cmd.Equals(command)) continue;
				res = cmd;
				break;
			}

			Object.Destroy(res);
			return res;
		}

		/// <summary>
		/// Destroys the command.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static IDelegateCommand DestroyCommand(this GameObject target, Action method)
		{
			var commands = target.GetComponents<DelegateCommand>();
			if (commands == null) return null;

			DelegateCommand res = null;
			foreach (var cmd in commands)
			{
				if (!cmd.IsSameExecutionMethod(method)) continue;
				res = cmd;
				break;
			}

			Object.Destroy(res);
			return res;
		}

		/// <summary>
		/// Destroys the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static IDelegateCommand DestroyCommand<T>(this GameObject target, Action<T> method)
		{
			var commands = target.GetComponents<DelegateCommand>();
			if (commands == null) return null;

			DelegateCommand res = null;
			foreach (var cmd in commands)
			{
				if (!cmd.IsSameExecutionMethod(method)) continue;
				res = cmd;
				break;
			}

			Object.Destroy(res);
			return res;
		}

		/// <summary>
		/// Destroys all commands.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		public static IEnumerable<IDelegateCommand> DestroyAllCommands(this GameObject target)
		{
			var commands = target.GetComponents<DelegateCommand>();
			if (commands == null) return null;

			commands.ForEach(Object.Destroy);
			return commands;
		}

		/// <summary>
		/// Destroys the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		public static void Destroy(this GameObject target)
		{
			Object.Destroy(target);
		}
	}
}