using TMS.Common.Extensions;
using UnityEngine.SceneManagement;

namespace TMS.Runtime.Unity.Actions
{
    public class SceneAction : GameObjectAction
    {
        public void LoadScene(string sceneName)
        {
            ArgumentValidator.AssertNotNullOrEmpty(sceneName, "sceneName");
            SceneManager.LoadScene(sceneName);
        }
		
        public void LoadSceneAdditive(string sceneName)
        {
            ArgumentValidator.AssertNotNullOrEmpty(sceneName, "sceneName");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void LoadSceneAsync(string sceneName)
        {
            ArgumentValidator.AssertNotNullOrEmpty(sceneName, "sceneName");
            SceneManager.LoadSceneAsync(sceneName);
        }
		
        public void LoadSceneAdditiveAsync(string sceneName)
        {
            ArgumentValidator.AssertNotNullOrEmpty(sceneName, "sceneName");
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}