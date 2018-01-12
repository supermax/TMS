#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Unity.IO.Compression
{
#if !NETFX_CORE
	[Serializable]
#endif // !FEATURE_NETCORE
	public sealed class InvalidDataException
#if NETFX_CORE
        : Exception
#else
		: SystemException
#endif
	{
		public InvalidDataException()
			: base(SR.GetString(SR.GenericInvalidData))
		{
		}

		public InvalidDataException(string message)
			: base(message)
		{
		}

		public InvalidDataException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

#if !NETFX_CORE
		internal InvalidDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif // !NETFX_CORE
	}
}