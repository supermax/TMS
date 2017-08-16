#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	///     Collection Extensions
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		///     Performs action for each item in provided collection
		/// </summary>
		/// <typeparam name="T"> Type of the item </typeparam>
		/// <param name="enumeration"> The enumeration. </param>
		/// <param name="action"> The action. </param>
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach (var item in enumeration)
			{
				action(item);
			}
		}

		/// <summary>
		///     Performs action for each item in provided collection
		/// </summary>
		/// <typeparam name="T"> Type of the item </typeparam>
		/// <param name="enumeration"> The enumeration. </param>
		/// <param name="action"> The action (first argument represents counter for enumerator's loop). </param>
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<int, T> action)
		{
			var count = 0;
			foreach (var item in enumeration)
			{
				action(count++, item);
			}
		}

		/// <summary>
		///     Adds the range.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="source"> The source. </param>
		/// <param name="range"> The range. </param>
		public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> range)
		{
			foreach (var item in range)
			{
				source.Add(item);
			}
		}

		/// <summary>
		///     Determines whether [is null or empty] [the specified enumeration].
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="enumeration"> The enumeration. </param>
		/// <returns>
		///     <c>true</c> if [is null or empty] [the specified enumeration]; otherwise, <c>false</c> .
		/// </returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumeration)
		{
			if (enumeration == null) return true;
			using (var enumerator = enumeration.GetEnumerator())
			{
				//enumerator.Reset();
				if (enumerator.MoveNext())
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the length of the Enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static int GetLength<T>(this IEnumerable<T> source)
		{
			var sourceCount = 0;
			foreach (var item in source)
			{
				sourceCount++;
			}
			return sourceCount;
		}

		private static readonly Random Random = new Random();

		/// <summary>
		/// Shuffles a Enumerable.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<T> RandomShuffle<T>(this IEnumerable<T> source)
		{
			ArgumentValidator.AssertNotNull(source, "source");

			var sourceCount = source.GetLength();
			IList<int> validationList = new List<int>(sourceCount);
			for (var i = 0; i < sourceCount; i++)
			{
				validationList.Add(-1);
			}

			IList<T> result = new List<T>(sourceCount);
			for (var i = 0; i < sourceCount; i++)
			{
				result.Add(default(T));
			}

			using (var sourceEnumerator = source.GetEnumerator())
			{
				sourceEnumerator.Reset();

				for (var i = 0; i < sourceCount && sourceEnumerator.MoveNext(); i++)
				{
					var obj = sourceEnumerator.Current;
					var randValue = Random.Next(0, sourceCount);
					while (validationList[randValue] != -1)
					{
						randValue = (randValue + 1) % sourceCount;
					}
					result[randValue] = obj;
					validationList[randValue] = randValue;
				}
			}

			return result;
		}

		/// <summary>
		/// Index of given item.
		/// </summary>
		/// <param name="col">The collection.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public static int GetIndexOf(this IEnumerable col, object item)
		{
			ArgumentValidator.AssertNotNull(col, "col");
			ArgumentValidator.AssertNotNull(item, "item");

			var e = col.GetEnumerator();
			//e.Reset();
			var i = 0;
			while (e.MoveNext())
			{
				var equals = Equals(e.Current, item);
				if (equals) return i;
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Determines whether collection contains the given item.
		/// </summary>
		/// <param name="col">The collection.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public static bool Contains(this IEnumerable col, object item)
		{
			ArgumentValidator.AssertNotNull(col, "col");
			ArgumentValidator.AssertNotNull(item, "item");

			var idx = col.GetIndexOf(item);
			var contains = idx > -1;
			return contains;
		}

		/// <summary>
		/// Creates the array from given source.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="src">The source.</param>
		/// <returns></returns>
		public static T[] CreateArray<T>(this IEnumerable<T> src)
		{
			var i = 0;
			var ary = new T[src.GetLength()];
			foreach (var item in src)
			{
				ary[i++] = item;
			}
			return ary;
		}

		/// <summary>
		/// Returns an array of cloned dictionary items.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="src">The source.</param>
		/// <returns>An array of cloned dictionary items</returns>
		public static KeyValuePair<TKey, TValue>[] CreateArray<TKey, TValue>(this IDictionary<TKey, TValue> src)
		{
			using (var e = src.GetEnumerator())
			{
				e.Reset();

				var ary = new KeyValuePair<TKey, TValue>[src.Count];
				for (var i = 0; i < src.Count; i++)
				{
					if (!e.MoveNext()) break;
					ary[i] = new KeyValuePair<TKey, TValue>(e.Current.Key, e.Current.Value);
				}

				return ary;
			}
		}

		/// <summary>
		/// Returns first element or default value of <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="col">The col.</param>
		/// <returns></returns>
		public static T GetFirstOrDefault<T>(this IEnumerable<T> col)
		{
			ArgumentValidator.AssertNotNull(col, "col");

			var list = col as IList<T>;
			if (list != null)
			{
				if (list.Count > 0)
					return list[0];
			}
			else
			{
				using (var enumerator = col.GetEnumerator())
				{
					if (enumerator.MoveNext())
						return enumerator.Current;
				}
			}
			return default(T);
		}

		/// <summary>
		/// Returns the maximum value in a sequence of <see cref="T:System.Int32"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The maximum value in the sequence.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Int32"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static int GetMaxValue(this IEnumerable<int> source)
		{
			ArgumentValidator.AssertNotNull(source, "source");

			var num1 = 0;
			var flag = false;
			foreach (var num2 in source)
			{
				if (flag)
				{
					if (num2 > num1)
						num1 = num2;
				}
				else
				{
					num1 = num2;
					flag = true;
				}
			}
			if (flag) return num1;

			throw new OperationCanceledException(string.Format("No Elements in {0}", source));
		}

		/// <summary>
		/// Returns the element at a specified index in a sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The element at the specified position in the source sequence.
		/// </returns>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="index">The zero-based index of the element to retrieve.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to the number of elements in <paramref name="source"/>.</exception>
		public static TSource GetElementAt<TSource>(this IEnumerable<TSource> source, int index)
		{
			ArgumentValidator.AssertNotNull(source, "source");

			var list = source as IList<TSource>;
			if (list != null) return list[index];

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			using (var enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (index == 0)
					{
						return enumerator.Current;
					}
					--index;
				}

				throw new ArgumentOutOfRangeException("index");
			}
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Single"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Single"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static float GetAverage(this IEnumerable<float> source)
		{
			ArgumentValidator.AssertNotNull(source, "source");

			var num1 = 0.0;
			var num2 = 0L;
			foreach (var num3 in source)
			{
				num1 += num3;
				checked { ++num2; }
			}
			if (num2 > 0L)
				return (float)num1 / num2;

			throw new OperationCanceledException(string.Format("No elements in {0}", source));
		}

		/// <summary>
		/// Generates a sequence that contains one repeated value.
		/// </summary>
		/// 
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains a repeated value.
		/// </returns>
		/// <param name="element">The value to be repeated.</param><param name="count">The number of times to repeat the value in the generated sequence.</param><typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.</exception>
		public static IEnumerable<TResult> CreateSequence<TResult>(TResult element, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException("count");
			
			var res = RepeatIterator(element, count);
			return res;
		}

		private static IEnumerable<TResult> RepeatIterator<TResult>(TResult element, int count)
		{
			for (var i = 0; i < count; ++i)
				yield return element;
		}

		/// <summary>
		/// Creates the list from given source.
		/// </summary>
		/// <typeparam name="TSource">The type of the source.</typeparam>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static List<TSource> CreateList<TSource>(this IEnumerable<TSource> source)
		{
			ArgumentValidator.AssertNotNull(source, "source");

			var res = new List<TSource>(source);
			return res;
		}

		/// <summary>
		/// Gets the value or default for the given key.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dic">The src dic.</param>
		/// <param name="key">The key.</param>
		/// <param name="recursive">if set to <c>true</c> [recursive].</param>
		/// <returns></returns>
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, bool recursive = false)
		{
			ArgumentValidator.AssertNotNull(dic, "dic");

			if (!dic.ContainsKey(key))
			{
				if(!recursive) return default(TValue);

				foreach (var item in dic)
				{
					var innerDic = item.Value as IDictionary<TKey, TValue>;
					if(innerDic == null) continue;

					var res = innerDic.GetValueOrDefault(key, true);
					if (Equals(res, default(TValue))) continue;

					return res;
				}
			}

			var value = dic[key];
			return value;
		}

		/// <summary>
		/// Gets the value or null for the given key.
		/// </summary>
		/// <param name="dic">The src dic.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static object GetValueOrNull(this IDictionary dic, object key)
		{
			ArgumentValidator.AssertNotNull(dic, "dic");

			if (!dic.Contains(key)) return null;

			var value = dic[key];
			return value;
		}

		public static bool TryGetValueOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> dic, TKey key, 
			out TValue value, TValue fallbackValue = default(TValue))
		{
			value = fallbackValue;
			if (!dic.ContainsKey(key)) return false;
			value = dic[key];
			return true;
		}
	}
}