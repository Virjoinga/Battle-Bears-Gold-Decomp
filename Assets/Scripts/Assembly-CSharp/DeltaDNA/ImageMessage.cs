using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace DeltaDNA
{
	public class ImageMessage
	{
		public class EventArgs : System.EventArgs
		{
			public string ID { get; set; }

			public string ActionType { get; set; }

			public string ActionValue { get; set; }

			public EventArgs(string id, string type, string value)
			{
				ID = id;
				ActionType = type;
				ActionValue = value;
			}
		}

		private class SpriteMap : MonoBehaviour
		{
			private Dictionary<string, object> configuration;

			private Texture2D texture;

			public string URL { get; private set; }

			public int Width { get; private set; }

			public int Height { get; private set; }

			public Texture Texture
			{
				get
				{
					return texture;
				}
			}

			public Texture Background
			{
				get
				{
					try
					{
						Dictionary<string, object> dictionary = configuration["background"] as Dictionary<string, object>;
						int x = (int)(long)dictionary["x"];
						int y = (int)(long)dictionary["y"];
						int width = (int)(long)dictionary["width"];
						int height = (int)(long)dictionary["height"];
						return GetSubRegion(x, y, width, height);
					}
					catch (KeyNotFoundException ex)
					{
						Logger.LogError("Invalid format, background not found: " + ex.Message);
					}
					return null;
				}
			}

			public List<Texture> Buttons
			{
				get
				{
					List<Texture> list = new List<Texture>();
					if (configuration.ContainsKey("buttons"))
					{
						try
						{
							List<object> list2 = configuration["buttons"] as List<object>;
							foreach (object item in list2)
							{
								int x = (int)(long)((Dictionary<string, object>)item)["x"];
								int y = (int)(long)((Dictionary<string, object>)item)["y"];
								int width = (int)(long)((Dictionary<string, object>)item)["width"];
								int height = (int)(long)((Dictionary<string, object>)item)["height"];
								list.Add(GetSubRegion(x, y, width, height));
							}
						}
						catch (KeyNotFoundException ex)
						{
							Logger.LogError("Invalid format, button not found: " + ex.Message);
						}
					}
					return list;
				}
			}

			public void Build(Dictionary<string, object> configuration)
			{
				try
				{
					URL = configuration["url"] as string;
					Width = (int)(long)configuration["width"];
					Height = (int)(long)configuration["height"];
					this.configuration = configuration["spritemap"] as Dictionary<string, object>;
				}
				catch (KeyNotFoundException ex)
				{
					Logger.LogError("Invalid format: " + ex.Message);
				}
			}

			public void LoadResource(Action<string> callback)
			{
				texture = new Texture2D(Width, Height);
				StartCoroutine(LoadResourceCoroutine(URL, callback));
			}

			public Texture2D GetSubRegion(int x, int y, int width, int height)
			{
				Color[] pixels = texture.GetPixels(x, texture.height - y - height, width, height);
				Texture2D texture2D = new Texture2D(width, height, texture.format, false);
				texture2D.SetPixels(pixels);
				texture2D.Apply();
				return texture2D;
			}

			public Texture2D GetSubRegion(Rect rect)
			{
				return GetSubRegion(Mathf.FloorToInt(rect.x), Mathf.FloorToInt(rect.y), Mathf.FloorToInt(rect.width), Mathf.FloorToInt(rect.height));
			}

			private IEnumerator LoadResourceCoroutine(string url, Action<string> callback)
			{
				WWW www = new WWW(url);
				yield return www;
				if (www.error == null)
				{
					www.LoadImageIntoTexture(texture);
				}
				else
				{
					Logger.LogWarning("Failed to load resource " + url + " " + www.error);
				}
				callback(www.error);
			}
		}

		private class Layer : MonoBehaviour
		{
			protected GameObject parent;

			protected ImageMessage imageMessage;

			protected List<Action> actions = new List<Action>();

			protected int depth;

			protected void RegisterAction()
			{
				actions.Add(delegate
				{
				});
			}

			protected void RegisterAction(Dictionary<string, object> action, string id)
			{
				object valueObj;
				action.TryGetValue("value", out valueObj);
				object value;
				if (!action.TryGetValue("type", out value))
				{
					return;
				}
				EventArgs eventArgs = new EventArgs(id, (string)value, (string)valueObj);
				GameEvent actionEvent = new GameEvent("imageMessageAction");
				if (imageMessage.engagement.JSON.ContainsKey("eventParams"))
				{
					Dictionary<string, object> dictionary = imageMessage.engagement.JSON["eventParams"] as Dictionary<string, object>;
					actionEvent.AddParam("responseDecisionpointName", dictionary["responseDecisionpointName"]);
					actionEvent.AddParam("responseEngagementID", dictionary["responseEngagementID"]);
					actionEvent.AddParam("responseEngagementName", dictionary["responseEngagementName"]);
					actionEvent.AddParam("responseEngagementType", dictionary["responseEngagementType"]);
					actionEvent.AddParam("responseMessageSequence", dictionary["responseMessageSequence"]);
					actionEvent.AddParam("responseVariantName", dictionary["responseVariantName"]);
					actionEvent.AddParam("responseTransactionID", dictionary["responseTransactionID"]);
				}
				actionEvent.AddParam("imActionName", id);
				actionEvent.AddParam("imActionType", (string)value);
				if (!string.IsNullOrEmpty((string)valueObj) && (string)value != "dismiss")
				{
					actionEvent.AddParam("imActionValue", (string)valueObj);
				}
				switch ((string)value)
				{
				case "none":
					actions.Add(delegate
					{
					});
					return;
				case "action":
					actions.Add(delegate
					{
						if (valueObj != null && imageMessage.OnAction != null)
						{
							imageMessage.OnAction(eventArgs);
						}
						Singleton<DDNA>.Instance.RecordEvent(actionEvent);
						imageMessage.Close();
					});
					return;
				case "link":
					actions.Add(delegate
					{
						if (imageMessage.OnAction != null)
						{
							imageMessage.OnAction(eventArgs);
						}
						if (valueObj != null)
						{
							Application.OpenURL((string)valueObj);
						}
						Singleton<DDNA>.Instance.RecordEvent(actionEvent);
						imageMessage.Close();
					});
					return;
				}
				actions.Add(delegate
				{
					if (imageMessage.OnDismiss != null)
					{
						imageMessage.OnDismiss(eventArgs);
					}
					Singleton<DDNA>.Instance.RecordEvent(actionEvent);
					imageMessage.Close();
				});
			}

			protected void PositionObject(GameObject obj, Rect position)
			{
				obj.transform.SetParent(parent.transform);
				float num = 1f / (float)Screen.width;
				float num2 = 1f / (float)Screen.height;
				float num3 = position.x * num;
				float num4 = position.y * num2;
				float num5 = position.width * num;
				float num6 = position.height * num2;
				obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, 0f);
				obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
				obj.GetComponent<RectTransform>().anchorMin = new Vector2(num3, 1f - num4 - num6);
				obj.GetComponent<RectTransform>().anchorMax = new Vector2(num3 + num5, 1f - num4);
			}
		}

		private class ShimLayer : Layer
		{
			private Texture2D texture;

			private readonly byte dimmedMaskAlpha = 128;

			public void Build(GameObject parent, ImageMessage imageMessage, Dictionary<string, object> config, int depth)
			{
				base.parent = parent;
				base.imageMessage = imageMessage;
				base.depth = depth;
				object value;
				if (config.TryGetValue("mask", out value))
				{
					bool flag = true;
					Color32[] array = new Color32[1];
					switch ((string)value)
					{
					default:
					{
						int num;
						if (num == 1)
						{
							array[0] = new Color32(0, 0, 0, 0);
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "dimmed":
						array[0] = new Color32(0, 0, 0, dimmedMaskAlpha);
						break;
					}
					if (flag)
					{
						texture = new Texture2D(1, 1);
						texture.SetPixels32(array);
						texture.Apply();
					}
				}
				object value2;
				if (config.TryGetValue("action", out value2))
				{
					RegisterAction((Dictionary<string, object>)value2, "shim");
				}
				else
				{
					RegisterAction();
				}
			}

			private void Start()
			{
				if (!texture)
				{
					return;
				}
				GameObject gameObject = new GameObject("Shim", typeof(RectTransform));
				PositionObject(gameObject, new Rect(0f, 0f, Screen.width, Screen.height));
				gameObject.AddComponent<Button>();
				gameObject.AddComponent<Image>();
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					if (actions.Count > 0)
					{
						actions[0]();
					}
				});
				gameObject.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			}
		}

		private class BackgroundLayer : Layer
		{
			private Texture texture;

			private Rect position;

			private float scale;

			public Rect Position
			{
				get
				{
					return position;
				}
			}

			public float Scale
			{
				get
				{
					return scale;
				}
			}

			public void Build(GameObject parent, ImageMessage imageMessage, Dictionary<string, object> layout, Texture texture, int depth)
			{
				base.parent = parent;
				base.imageMessage = imageMessage;
				this.texture = texture;
				base.depth = depth;
				object value;
				if (layout.TryGetValue("background", out value))
				{
					Dictionary<string, object> dictionary = value as Dictionary<string, object>;
					object value2;
					if (dictionary.TryGetValue("action", out value2))
					{
						RegisterAction((Dictionary<string, object>)value2, "background");
					}
					else
					{
						RegisterAction();
					}
					object value3;
					if (dictionary.TryGetValue("cover", out value3))
					{
						position = RenderAsCover((Dictionary<string, object>)value3);
					}
					else if (dictionary.TryGetValue("contain", out value3))
					{
						position = RenderAsContain((Dictionary<string, object>)value3);
					}
					else
					{
						Logger.LogError("Invalid layout");
					}
				}
				else
				{
					RegisterAction();
				}
			}

			private void Start()
			{
				if (!texture)
				{
					return;
				}
				GameObject gameObject = new GameObject("Background", typeof(RectTransform));
				PositionObject(gameObject, position);
				gameObject.AddComponent<Button>();
				gameObject.AddComponent<Image>();
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					if (actions.Count > 0)
					{
						actions[0]();
					}
				});
				gameObject.GetComponent<Image>().sprite = Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			}

			private Rect RenderAsCover(Dictionary<string, object> rules)
			{
				scale = Math.Max((float)Screen.width / (float)texture.width, (float)Screen.height / (float)texture.height);
				float num = (float)texture.width * scale;
				float num2 = (float)texture.height * scale;
				float top = (float)Screen.height / 2f - num2 / 2f;
				float left = (float)Screen.width / 2f - num / 2f;
				object value;
				if (rules.TryGetValue("valign", out value))
				{
					switch ((string)value)
					{
					case "top":
						top = 0f;
						break;
					case "bottom":
						top = (float)Screen.height - num2;
						break;
					}
				}
				object value2;
				if (rules.TryGetValue("halign", out value2))
				{
					switch ((string)value2)
					{
					case "left":
						left = 0f;
						break;
					case "right":
						left = (float)Screen.width - num;
						break;
					}
				}
				return new Rect(left, top, num, num2);
			}

			private Rect RenderAsContain(Dictionary<string, object> rules)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				object value;
				if (rules.TryGetValue("left", out value))
				{
					num = GetConstraintPixels((string)value, Screen.width);
				}
				object value2;
				if (rules.TryGetValue("right", out value2))
				{
					num2 = GetConstraintPixels((string)value2, Screen.width);
				}
				float val = ((float)Screen.width - num - num2) / (float)texture.width;
				object value3;
				if (rules.TryGetValue("top", out value3))
				{
					num3 = GetConstraintPixels((string)value3, Screen.height);
				}
				object value4;
				if (rules.TryGetValue("bottom", out value4))
				{
					num4 = GetConstraintPixels((string)value4, Screen.height);
				}
				float val2 = ((float)Screen.height - num3 - num4) / (float)texture.height;
				scale = Math.Min(val, val2);
				float num5 = (float)texture.width * scale;
				float num6 = (float)texture.height * scale;
				float top = ((float)Screen.height - num3 - num4) / 2f - num6 / 2f + num3;
				float left = ((float)Screen.width - num - num2) / 2f - num5 / 2f + num;
				object value5;
				if (rules.TryGetValue("valign", out value5))
				{
					switch ((string)value5)
					{
					case "top":
						top = num3;
						break;
					case "bottom":
						top = (float)Screen.height - num6 - num4;
						break;
					}
				}
				object value6;
				if (rules.TryGetValue("halign", out value6))
				{
					switch ((string)value6)
					{
					case "left":
						left = num;
						break;
					case "right":
						left = (float)Screen.width - num5 - num2;
						break;
					}
				}
				return new Rect(left, top, num5, num6);
			}

			private float GetConstraintPixels(string constraint, float edge)
			{
				float result = 0f;
				Regex regex = new Regex("(\\d+)(px|%)", RegexOptions.IgnoreCase);
				Match match = regex.Match(constraint);
				if (match != null && match.Success)
				{
					GroupCollection groups = match.Groups;
					if (float.TryParse(groups[1].Value, out result))
					{
						if (groups[2].Value == "%")
						{
							return edge * result / 100f;
						}
						return result;
					}
				}
				return result;
			}
		}

		private class ButtonsLayer : Layer
		{
			private List<Texture> textures = new List<Texture>();

			private List<Rect> positions = new List<Rect>();

			public void Build(GameObject parent, ImageMessage imageMessage, Dictionary<string, object> orientation, List<Texture> textures, BackgroundLayer content, int depth)
			{
				base.parent = parent;
				base.imageMessage = imageMessage;
				base.depth = depth;
				object value;
				if (!orientation.TryGetValue("buttons", out value))
				{
					return;
				}
				List<object> list = value as List<object>;
				for (int i = 0; i < list.Count; i++)
				{
					Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
					float left = 0f;
					float top = 0f;
					object value2;
					if (dictionary.TryGetValue("x", out value2))
					{
						left = (float)(int)(long)value2 * content.Scale + content.Position.xMin;
					}
					object value3;
					if (dictionary.TryGetValue("y", out value3))
					{
						top = (float)(int)(long)value3 * content.Scale + content.Position.yMin;
					}
					positions.Add(new Rect(left, top, (float)textures[i].width * content.Scale, (float)textures[i].height * content.Scale));
					object value4;
					if (dictionary.TryGetValue("action", out value4))
					{
						RegisterAction((Dictionary<string, object>)value4, "button" + (i + 1));
					}
					else
					{
						RegisterAction();
					}
				}
				this.textures = textures;
			}

			private void Start()
			{
				for (int i = 0; i < textures.Count; i++)
				{
					GameObject gameObject = new GameObject("Button", typeof(RectTransform));
					PositionObject(gameObject, positions[i]);
					gameObject.AddComponent<Button>();
					gameObject.AddComponent<Image>();
					Action action = actions[i];
					gameObject.GetComponent<Button>().onClick.AddListener(delegate
					{
						action();
					});
					gameObject.GetComponent<Image>().sprite = Sprite.Create(textures[i] as Texture2D, new Rect(0f, 0f, textures[i].width, textures[i].height), new Vector2(0.5f, 0.5f));
				}
			}
		}

		private Dictionary<string, object> configuration;

		private GameObject gameObject;

		private SpriteMap spriteMap;

		private int depth;

		private bool resourcesLoaded;

		private bool showing;

		private Engagement engagement;

		public Dictionary<string, object> Parameters { get; private set; }

		public event Action OnDidReceiveResources;

		public event Action<string> OnDidFailToReceiveResources;

		public event Action<EventArgs> OnDismiss;

		public event Action<EventArgs> OnAction;

		private ImageMessage(Dictionary<string, object> configuration, string name, int depth, Engagement engagement)
		{
			gameObject = new GameObject(name, typeof(RectTransform));
			SpriteMap spriteMap = gameObject.AddComponent<SpriteMap>();
			spriteMap.Build(configuration);
			this.configuration = configuration;
			this.spriteMap = spriteMap;
			this.depth = depth;
			this.engagement = engagement;
		}

		public static ImageMessage Create(Engagement engagement)
		{
			return Create(engagement, null);
		}

		public static ImageMessage Create(Engagement engagement, Dictionary<string, object> options)
		{
			if (engagement == null || engagement.JSON == null || !engagement.JSON.ContainsKey("image"))
			{
				return null;
			}
			string name = "DeltaDNA Image Message";
			int num = 0;
			if (options != null)
			{
				if (options.ContainsKey("name"))
				{
					name = options["name"] as string;
				}
				if (options.ContainsKey("depth"))
				{
					num = (int)options["depth"];
				}
			}
			ImageMessage imageMessage = null;
			try
			{
				Dictionary<string, object> c = engagement.JSON["image"] as Dictionary<string, object>;
				if (ValidConfiguration(c))
				{
					imageMessage = new ImageMessage(c, name, num, engagement);
					if (engagement.JSON.ContainsKey("parameters"))
					{
						imageMessage.Parameters = engagement.JSON["parameters"] as Dictionary<string, object>;
					}
				}
				else
				{
					Logger.LogWarning("Invalid image message configuration.");
				}
			}
			catch (Exception ex)
			{
				Logger.LogWarning("Failed to create image message: " + ex.Message);
			}
			return imageMessage;
		}

		private static bool ValidConfiguration(Dictionary<string, object> c)
		{
			if (!c.ContainsKey("url") || !c.ContainsKey("height") || !c.ContainsKey("width") || !c.ContainsKey("spritemap") || !c.ContainsKey("layout"))
			{
				return false;
			}
			Dictionary<string, object> dictionary = c["layout"] as Dictionary<string, object>;
			if (!dictionary.ContainsKey("landscape") && !dictionary.ContainsKey("portrait"))
			{
				return false;
			}
			Dictionary<string, object> dictionary2 = c["spritemap"] as Dictionary<string, object>;
			if (!dictionary2.ContainsKey("background"))
			{
				return false;
			}
			return true;
		}

		public void FetchResources()
		{
			spriteMap.LoadResource(delegate(string error)
			{
				if (error == null)
				{
					resourcesLoaded = true;
					if (this.OnDidReceiveResources != null)
					{
						this.OnDidReceiveResources();
					}
				}
				else if (this.OnDidFailToReceiveResources != null)
				{
					this.OnDidFailToReceiveResources(error);
				}
			});
		}

		public bool IsReady()
		{
			return resourcesLoaded;
		}

		public void Show()
		{
			if (!resourcesLoaded)
			{
				return;
			}
			try
			{
				gameObject.AddComponent<Canvas>();
				gameObject.AddComponent<GraphicRaycaster>();
				gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
				if (configuration.ContainsKey("shim"))
				{
					ShimLayer shimLayer = gameObject.AddComponent<ShimLayer>();
					shimLayer.Build(gameObject, this, configuration["shim"] as Dictionary<string, object>, depth);
				}
				Dictionary<string, object> dictionary = configuration["layout"] as Dictionary<string, object>;
				object value;
				if (!dictionary.TryGetValue("landscape", out value) && !dictionary.TryGetValue("portrait", out value))
				{
					throw new KeyNotFoundException("Layout missing orientation key.");
				}
				BackgroundLayer backgroundLayer = gameObject.AddComponent<BackgroundLayer>();
				backgroundLayer.Build(gameObject, this, value as Dictionary<string, object>, spriteMap.Background, depth - 1);
				ButtonsLayer buttonsLayer = gameObject.AddComponent<ButtonsLayer>();
				buttonsLayer.Build(gameObject, this, value as Dictionary<string, object>, spriteMap.Buttons, backgroundLayer, depth - 2);
				showing = true;
			}
			catch (KeyNotFoundException ex)
			{
				Logger.LogWarning("Failed to show image message, invalid format: " + ex.Message);
			}
			catch (Exception ex2)
			{
				Logger.LogWarning("Failed to show image message: " + ex2.Message);
			}
		}

		public bool IsShowing()
		{
			return showing;
		}

		public void Close()
		{
			if (showing)
			{
				UnityEngine.Object.Destroy(gameObject);
				showing = false;
			}
		}
	}
}
