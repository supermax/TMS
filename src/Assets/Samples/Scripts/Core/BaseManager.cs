using System.Collections;
using System.Collections.Generic;
using TMS.Common.Core;
using UnityEngine;

public class BaseManager : ViewModel
{
	public virtual int InstanceId
	{
		get
		{
			return GetHashCode();
		}
	}
}
