#region Code Editor

// Maxim

#endregion

#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace TMS.Common.Core
{
	//public class U3DList<T>
	//{
	//	public T[] Buffer;
	//	public int Size;
    //
	//	public T this[int i]
	//	{
	//		get { return Buffer[i]; }
	//		set { Buffer[i] = value; }
	//	}
    //
	//	public IEnumerator<T> GetEnumerator()
	//	{
	//		if (Buffer == null)
	//		{
	//			yield break;
	//		}
    //
	//		for (var i = 0; i < Size; ++i)
	//		{
	//			yield return Buffer[i];
	//		}
	//	}
    //
	//	private void AllocateMore()
	//	{
	//	    Debug.Log(Buffer.Length);
	//		var newSize = Mathf.Max(Buffer.Length << 1, 32);
	//		var objArray = Buffer != null ? new T[newSize] : new T[32];
	//		if (Buffer != null && Size > 0)
	//		{
	//			Buffer.CopyTo(objArray, 0);
	//		}
    //
	//		Buffer = objArray;
	//	}
    //
	//	private void Trim()
	//	{
	//		if (Size > 0)
	//		{
	//			if (Size >= Buffer.Length)
	//			{
	//				return;
	//			}
	//			var objArray = new T[Size];
	//			for (var index = 0; index < Size; ++index)
	//			{
	//				objArray[index] = Buffer[index];
	//			}
	//			Buffer = objArray;
	//		}
	//		else
	//		{
	//			Buffer = null;
	//		}
	//	}
    //
	//	public void Clear()
	//	{
	//		Size = 0;
	//	}
    //
	//	public void Release()
	//	{
	//		Size = 0;
	//		Buffer = null;
	//	}
    //
	//	public void Add(T item)
	//	{
    //        Debug.Log(Buffer.Length);
	//		if (Buffer == null || Size == Buffer.Length)
	//			AllocateMore();
	//		Buffer[Size++] = item;
	//	}
    //
	//	public void Remove(T item)
	//	{
	//		if (Buffer == null)
	//		{
	//			return;
	//		}
    //
	//		var @default = EqualityComparer<T>.Default;
	//		for (var index1 = 0; index1 < Size; ++index1)
	//		{
	//			if (!@default.Equals(Buffer[index1], item))
	//			{
	//				continue;
	//			}
    //
	//			--Size;
	//			Buffer[index1] = default(T);
    //
	//			for (var index2 = index1; index2 < Size; ++index2)
	//			{
	//				Buffer[index2] = Buffer[index2 + 1];
	//			}
	//			break;
	//		}
	//	}
    //
	//	public void RemoveAt(int index)
	//	{
	//		if (Buffer == null || index >= Size)
	//		{
	//			return;
	//		}
    //
	//		--Size;
	//		Buffer[index] = default(T);
	//		for (var index1 = index; index1 < Size; ++index1)
	//		{
	//			Buffer[index1] = Buffer[index1 + 1];
	//		}
	//	}
    //
	//	public T[] ToArray()
	//	{
	//		Trim();
	//		return Buffer;
	//	}
	//}
}