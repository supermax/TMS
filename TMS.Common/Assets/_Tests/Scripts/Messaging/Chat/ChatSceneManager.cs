using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMS.Common.Core;
using TMS.Common.Messaging;
using UnityEngine;

[MessengerConsumer(typeof(ISceneManager), true, InstantiateOnRegistration = true)]
public class ChatSceneManager : Singleton<ISceneManager, ChatSceneManager>, ISceneManager, IMessengerConsumer
{
	private readonly IDictionary<string, Action<string>> _actions;

    public ChatSceneManager()
	{
		_actions = new Dictionary<string, Action<string>> 
		{
			{ "#load_scene", LoadScene },
			{ "#unload_scene", UnloadScene }
		};
	}

	public void Subscribe()
	{
		Messenger.Default.Subscribe<IChatMessage>(OnChatMessageReceived, CanHandleChatMessage);

		Messenger.Default.Subscribe<LoadScenePayload>(payload => LoadScene(payload.SceneName));
		Messenger.Default.Subscribe<UnloadScenePayload>(payload => UnloadScene(payload.SceneName));
	}

	private bool CanHandleChatMessage(IChatMessage payload)
	{
		var canHandle = _actions.Count(item => payload.Text.StartsWith(item.Key)) > 0;
		return canHandle;
	}

	private void OnChatMessageReceived(IChatMessage payload)
	{
		var res = _actions.First(item => payload.Text.StartsWith(item.Key));
		var sceneName = Regex.Split(payload.Text, ":").Last().Trim();
		res.Value(sceneName);
	}

	public void LoadScene(string name)
	{
		var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(name);
		if (scene.isLoaded)
		{
			Debug.LogFormat("The scene \"{0}\" is loaded already.", name);
			return;
		}

		UnityEngine.SceneManagement.SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
		Debug.LogFormat("Loaded scene \"{0}\".", name);
	}

	public void UnloadScene(string name)
	{
		var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(name);
		if (!scene.isLoaded)
		{
			Debug.LogFormat("The scene \"{0}\" is not loaded.", name);
			return;
		}

		UnityEngine.SceneManagement.SceneManager.UnloadScene(name);
		Debug.LogFormat("Unloaded scene \"{0}\".", name);
	}
}

public interface ISceneManager
{
	void LoadScene(string name);

	void UnloadScene(string name);
}

public abstract class SceneManagerPayloadBase
{
	public string SceneName { get; set; }
}

public class LoadScenePayload : SceneManagerPayloadBase
{
	
}

public class UnloadScenePayload : SceneManagerPayloadBase
{
	
}
