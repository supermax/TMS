using System;
using System.Collections.Generic;
using TMS.Common.Extensions;
using TMS.Common.Modularity.Ioc.Config;

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