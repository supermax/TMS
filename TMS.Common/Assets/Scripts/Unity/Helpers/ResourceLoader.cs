using System;
using System.Collections;
using TMS.Common.Extensions;
using TMS.Common.Logging;
using TMS.Common.Serialization.Json;
using TMS.Common.Tasks.Threading;
using UnityEngine;

namespace TMS.Runtime.Unity.Helpers
{
    public static class ResourceLoader
    {
        public static ResourceLoadResult<T> Load<T>(string path)
        {
            UnityEngine.Object rss;
            try
            {
                rss = Resources.Load(path);
            }
            catch (Exception e)
            {
                Loggers.Default.ConsoleLogger.Write(e);
                return new ResourceLoadResult<T>(default(T), ResourceLoadResultStatus.Error, e);
            }

            var res = GetAsset<T>(path, rss);
            return res;
        }

        private static ResourceLoadResult<T> GetAsset<T>(string path, UnityEngine.Object rss) 
        {
            if (rss == null)
            {
                return new ResourceLoadResult<T>(default(T), ResourceLoadResultStatus.Error, 
                    new NullReferenceException(string.Format("Cannot load asset from path: {0}", path)));
            }
            
            if (rss.GetTypeWrapper().IsAssignableFrom(typeof(T)))
            {
                var res = (T)Convert.ChangeType(rss, typeof(T));
                return new ResourceLoadResult<T>(res, ResourceLoadResultStatus.Success);
            }

            var txt = rss as TextAsset;
            if (txt == null)
            {
                return new ResourceLoadResult<T>(default(T), ResourceLoadResultStatus.Error, 
                    new NullReferenceException(string.Format("Cannot load text asset from path: {0}", path)));
            }

            if (txt.text.IsNullOrEmpty())
            {
                return new ResourceLoadResult<T>(default(T), ResourceLoadResultStatus.Error, 
                    new NullReferenceException(string.Format("Text asset is empty from path: {0}", path)));    
            }

            try
            {
                var obj = JsonMapper.Default.ToObject<T>(txt.text);
                return new ResourceLoadResult<T>(obj, ResourceLoadResultStatus.Success);
            }
            catch (Exception e)
            {
                Loggers.Default.ConsoleLogger.Write(e);
                return new ResourceLoadResult<T>(default(T), ResourceLoadResultStatus.Error, e);
            }
        }

        public static void LoadAsync<T>(string path, Action<ResourceLoadResult<T>> callback) 
        {
            ThreadHelper.Default.StartCoroutine(LoadAsyncInternal(path, callback));
        }

        private static IEnumerator LoadAsyncInternal<T>(string path, Action<ResourceLoadResult<T>> callback) 
        {
            var rss = Resources.LoadAsync(path);
            yield return rss;

            while (!rss.isDone)
            {
                yield return null;
            }
            
            var res = GetAsset<T>(path, rss.asset);
            InvokeCallback(res, callback);
        }

        private static void InvokeCallback<T>(ResourceLoadResult<T> result, Action<ResourceLoadResult<T>> callback)
        {
            if (callback == null)
            {
                return;
            }
            callback(result);
        }
    }

    public class ResourceLoadResult<T>
    {
        public ResourceLoadResultStatus Status { get; private set; }
        
        public Exception Error { get; private set; }
        
        public T Result { get; private set; }

        public ResourceLoadResult(T result, ResourceLoadResultStatus status, Exception error = null)
        {
            Result = result;
            Status = status;
            Error = error;
        }
    }

    public enum ResourceLoadResultStatus
    {
        Success,
        Error
    }
}