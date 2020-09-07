using System.Collections.Generic;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
    public class NetworkConfig
    {
        [JsonDataMember(Name = "services")]
        public Dictionary<string, ServiceConfig> Services { get; set; }

        [JsonDataMember(Name = "defaultConfig")]
        public ServiceConfig DefaultConfig
        {
            get { return DefaultConfigSetting; }
            set { DefaultConfigSetting = value; }
        }       

        [JsonDataMemberIgnore]
        public static ServiceConfig DefaultConfigSetting { get; set; }
    }

    [JsonDataContract]
    public class ServiceConfig
    {
        [JsonDataMember(Name = "name")]
        public string Name
        {
            get
            {
                return _name ?? NetworkConfig.DefaultConfigSetting.Name;
            }
            set
            {
                _name = value;
            }
        }
        private string _name;

        [JsonDataMember(Name = "server")]
        public string Server
        {
            get
            {
                return _server ?? NetworkConfig.DefaultConfigSetting.Server;
            }
            set
            {
                _server = value;
            }
        }
        private string _server;

        [JsonDataMember(Name = "service_path")]
        public string ServicePath 
        {
            get
            {
                return _servicePath ?? NetworkConfig.DefaultConfigSetting.ServicePath;
            }
            set
            {
                _servicePath = value;
            }
        }
        private string _servicePath;

        [JsonDataMember(Name = "timeout")]
        public float? Timeout
        {
            get
            {
                return  _timeout ?? NetworkConfig.DefaultConfigSetting.Timeout;
            }
            set
            {
                _timeout = value;
            }
        }
        private float? _timeout;

        [JsonDataMember(Name = "max_retries")]
        public int? MaxRetries
        {
            get
            {
                return _maxRetries ?? NetworkConfig.DefaultConfigSetting.MaxRetries;
            }
            set
            {
                _maxRetries = value;
            }
        }
        private int? _maxRetries;

        [JsonDataMember(Name = "is_post")]
        public bool? IsPost
        {
            get
            {
                return _isPost ?? NetworkConfig.DefaultConfigSetting.IsPost;
            }
            set
            {
                _isPost = value;
            }
        }
        private bool? _isPost;

        [JsonDataMember(Name = "suppress_errors")]
        public int[] SuppressErrors
        {
            get
            {
                return _suppressErrors ?? NetworkConfig.DefaultConfigSetting.SuppressErrors;
            }
            set
            {
                _suppressErrors = value;
            }
        }
        private int[] _suppressErrors;

        [JsonDataMember(Name = "suppress_all")]
        public bool? SuppressAll
        {
            get
            {
                return _suppressAll ?? NetworkConfig.DefaultConfigSetting.SuppressAll;
            }
            set
            {
                _suppressAll = value;
            }
        }
        private bool? _suppressAll;

        [JsonDataMember(Name = "lock_on_error")]
        public bool? LockOnError
        {
            get
            {
                return _lockOnError ?? NetworkConfig.DefaultConfigSetting.LockOnError;
            }
            set
            {
                _lockOnError = value;
            }
        }
        private bool? _lockOnError;
    }
}
