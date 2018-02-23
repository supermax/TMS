using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Core;
using TMS.Common.Tasks.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMS.Common.Extensions
{ 
	public static class StringConvertionExtensions
	{
		public static string ToString(this Scene scene)
		{
			return scene == null ? null : scene.name;
		}
	}
}