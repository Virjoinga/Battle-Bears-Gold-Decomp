using UnityEngine;

namespace DeltaDNA
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		private static object _lock = new object();

		private static bool applicationIsQuitting = false;

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					Logger.LogWarning(string.Concat("[Singleton] Instance '", typeof(T), "' already destroyed on application quit. Won't create again - returning null."));
					return (T)null;
				}
				lock (_lock)
				{
					if ((Object)_instance == (Object)null)
					{
						_instance = (T)Object.FindObjectOfType(typeof(T));
						if (Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							Logger.LogWarning("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
							return _instance;
						}
						if ((Object)_instance == (Object)null)
						{
							GameObject gameObject = new GameObject();
							_instance = gameObject.AddComponent<T>();
							gameObject.name = typeof(T).ToString();
							Object.DontDestroyOnLoad(gameObject);
						}
					}
					return _instance;
				}
			}
		}

		public virtual void OnDestroy()
		{
			applicationIsQuitting = true;
		}
	}
}
