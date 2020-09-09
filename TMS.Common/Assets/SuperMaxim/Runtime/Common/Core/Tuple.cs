namespace TMS.Common.Core
{
	public class Tuple<T1, T2>
	{
		public T1 Object1 { get; set; }

		public T2 Object2 { get; set; }

		public Tuple()
		{

		}

		public Tuple(T1 obj1, T2 obj2)
		{
			Object1 = obj1;
			Object2 = obj2;
		}
	}

	public class Tuple<T> : Tuple<T, T>
	{
		public Tuple()
		{

		}

		public Tuple(T obj1, T obj2) : base(obj1, obj2)
		{

		}
	}
}