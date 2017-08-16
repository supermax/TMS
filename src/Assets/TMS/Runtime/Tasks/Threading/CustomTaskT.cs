#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections;
using System.Reflection;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CustomTask<T> : CustomTask
	{
		private readonly Func<CustomTask, T> _function;
		private T _result;

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="function">The function.</param>
		public CustomTask(Func<CustomTask, T> function)
		{
			_function = function;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="function">The function.</param>
		public CustomTask(Func<T> function)
		{
			_function = t => function();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public CustomTask(Action<CustomTask> action)
		{
			_function = t =>
			{
				action(t);
				return default(T);
			};
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public CustomTask(Action action)
		{
			_function = t =>
			{
				action();
				return default(T);
			};
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		public CustomTask(IEnumerator enumerator)
		{
			_function = t => (T) enumerator;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">The arguments.</param>
		/// <exception cref="System.ArgumentException">methodName;Fitting method with the given name was not found.</exception>
		public CustomTask(IReflect type, string methodName, params object[] args)
		{
			var methodInfo = type.GetMethod(methodName,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static);
			if (methodInfo == null)
				throw new ArgumentException("methodName", "Fitting method with the given name was not found.");

			_function = t => (T) methodInfo.Invoke(null, args);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomTask{T}" /> class.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">The arguments.</param>
		/// <exception cref="System.ArgumentException">methodName;Fitting method with the given name was not found.</exception>
		public CustomTask(object that, string methodName, params object[] args)
		{
			var methodInfo = that.GetType()
				.GetMethod(methodName,
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);

			if (methodInfo == null)
				throw new ArgumentException("methodName", "Fitting method with the given name was not found.");

			_function = t => (T) methodInfo.Invoke(that, args);
		}

		/// <summary>
		///     Gets the raw result.
		/// </summary>
		/// <value>
		///     The raw result.
		/// </value>
		public override object RawResult
		{
			get
			{
				if (!IsEnding)
				{
					Wait();
				}
				return _result;
			}
		}

		/// <summary>
		///     Gets the result.
		/// </summary>
		/// <value>
		///     The result.
		/// </value>
		public T Result
		{
			get
			{
				if (!IsEnding)
				{
					Wait();
				}
				return _result;
			}
		}

		protected override IEnumerator Do()
		{
			_result = _function(this);
			if (_result is IEnumerator)
			{
				return (IEnumerator) _result;
			}
			return null;
		}

		public override TResult Wait<TResult>()
		{
			Priority--;
			return (TResult) (object) Result;
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			Priority--;
			return WaitForSeconds(seconds, default(TResult));
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			if (!HasEnded)
			{
				WaitForSeconds(seconds);
			}

			if (IsSucceeded)
			{
				return (TResult) (object) _result;
			}

			return defaultReturnValue;
		}

		/// <summary>
		///     Runs the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		public new CustomTask<T> Run(CustomTaskDispatcherBase target) // TODO why 'new' ?
		{
			((CustomTask) this).Run(target);
			return this;
		}

		/// <summary>
		///     Runs this instance.
		/// </summary>
		/// <returns></returns>
		public new CustomTask<T> Run() // TODO why 'new' ?
		{
			((CustomTask) this).Run();
			return this;
		}
	}
}