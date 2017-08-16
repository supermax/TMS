#region Code Editor
// Maxim
#endregion

using System;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Logging
{
	[JsonDataContract]
	public class LogSystemInfo
	{
		[JsonDataMember("os")]
		public string OperatingSystem { get; set; }

		[JsonDataMember("cpu")]
		public string ProcessorType { get; set; }

		[JsonDataMember("cpuCount")]
		public int ProcessorCount { get; set; }

		[JsonDataMember("memorySize")]
		public int SystemMemorySize { get; set; }

		[JsonDataMember("gpuMemorySize")]
		public int GraphicsMemorySize { get; set; }

		[JsonDataMember("gpu")]
		public string GraphicsDeviceName { get; set; }

		[JsonDataMember("gpuDendor")]
		public string GraphicsDeviceVendor { get; set; }

		[JsonDataMember("gpuDID")]
		public int GraphicsDeviceID { get; set; }

		[JsonDataMember("gpuVDID")]
		public int GraphicsDeviceVendorID { get; set; }

		[JsonDataMember("gpuVer")]
		public string GraphicsDeviceVersion { get; set; }

		[JsonDataMember("gpuShaderLvl")]
		public int GraphicsShaderLevel { get; set; }

		[JsonDataMember]
		public int GraphicsPixelFillrate { get; set; }

		[JsonDataMember]
		public bool SupportsShadows { get; set; }

		[JsonDataMember]
		public bool SupportsRenderTextures { get; set; }

		[JsonDataMember]
		public bool SupportsImageEffects { get; set; }

		[JsonDataMember]
		public bool Supports3DTextures { get; set; }

		[JsonDataMember]
		public bool SupportsComputeShaders { get; set; }

		[JsonDataMember]
		public bool SupportsInstancing { get; set; }

		[JsonDataMember]
		public int SupportedRenderTargetCount { get; set; }

		[JsonDataMember]
		public int SupportsStencil { get; set; }

		[JsonDataMember]
		public bool SupportsVertexPrograms { get; set; }

		[JsonDataMember]
		public string DeviceUniqueIdentifier { get; set; }

		[JsonDataMember]
		public string DeviceName { get; set; }

		[JsonDataMember]
		public string DeviceModel { get; set; }

		[JsonDataMember]	
		public bool SupportsAccelerometer { get; set; }

		[JsonDataMember]
		public bool SupportsGyroscope { get; set; }

		[JsonDataMember]
		public bool SupportsLocationService { get; set; }

		[JsonDataMember]
		public bool SupportsVibration { get; set; }

		[JsonDataMember]
		public SystemDeviceType DeviceType { get; set; }

		[JsonDataMember]
		public int MaxTextureSize { get; set; }

		[JsonDataMember("user_name")]
		public string UserName { get; set; }

		[JsonDataMember]
		public DateTime StartupTime { get; set; }

		[JsonDataMember]
		public DateTime ShutdownTime { get; set; }

		[JsonDataMember]
		public string ClientVersion { get; set; }
	}

	[JsonDataContract]
	public enum SystemDeviceType
	{
		[JsonDataMember]
		Unknown = 0,

		[JsonDataMember]
		Handheld = 1,

		[JsonDataMember]
		Console = 2,

		[JsonDataMember]
		Desktop = 3,
	}
}