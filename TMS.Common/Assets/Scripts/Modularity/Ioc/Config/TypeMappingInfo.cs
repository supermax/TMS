using System;
using System.Collections.Generic;
using TMS.Common.Extensions;

namespace TMS.Common.Modularity
{
	internal class TypeMappingInfo : Dictionary<Type, MappingInfo>, IDisposable
	{
		public void Dispose()
		{
			Values.ForEach(map => map.Dispose());
			Clear();
		}
	}
}