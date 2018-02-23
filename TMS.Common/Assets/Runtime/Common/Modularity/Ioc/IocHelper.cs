using System.Collections.Generic;
using System.Reflection;
using TMS.Common.Extensions;

namespace TMS.Common.Modularity
{
	internal static class IocHelper
	{
		internal static ConstructorInfo GetMatchingConstructor(IEnumerable<ConstructorInfo> ctors, object[] args)
		{
			if (ctors.IsNullOrEmpty()) return null;

			// no arguments were passed, will use default constructor
			if (args.IsNullOrEmpty())
			{
				foreach (var ctor in ctors)
				{
					if (!ctor.IsPublic || ctor.IsStatic) continue;

					var paramsInfo = ctor.GetParameters();
					if (!paramsInfo.IsNullOrEmpty()) continue;

					return ctor;
				}
				return null;
			}

			// arguments were passed, will try to find matching constructor
			foreach (var ctor in ctors)
			{
				if (!ctor.IsPublic || ctor.IsStatic) continue;

				var paramsInfo = ctor.GetParameters();
				if (paramsInfo.IsNullOrEmpty() || paramsInfo.Length != args.Length) continue;

				var match = true;
				for (var i = 0; i < paramsInfo.Length; i++)
				{
					var paramInfo = paramsInfo[i];
					if (paramInfo == null) break;

					var arg = args[i];
					var argType = arg != null ? arg.GetType() : paramInfo.ParameterType;
					var isAssignable = paramInfo.ParameterType.IsAssignableFrom(argType);
					if (isAssignable) continue;

					match = false;
					break;
				}
				if (!match) continue;

				return ctor;
			}
			return null;
		}
	}
}