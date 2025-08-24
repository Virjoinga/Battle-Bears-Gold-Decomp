using System;
using System.Collections.Generic;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.Core.Powerups
{
	public class Powerup
	{
		public static Dictionary<string, Powerup> allPowerups;

		public static Dictionary<string, float> appliedMods;

		public static List<string> selectedPowerups;

		public string title;

		public string description;

		public string key;

		public Dictionary<string, float> mods;

		public float stackingValue = 1f;

		public Powerup()
		{
		}

		public Powerup(string title, string description, string key, Dictionary<string, float> mods, float stackingValue)
		{
			this.title = title;
			this.description = description;
			this.key = key;
			this.mods = mods;
			this.stackingValue = stackingValue;
			AddToPowerupsDictionary();
		}

		public static void RemoveAllApplied()
		{
			appliedMods = null;
			selectedPowerups = null;
		}

		public static void LoadPowerupsFromJSON(string json)
		{
			JsonReader jsonReader = new JsonReader(json);
			jsonReader.Read();
			if (jsonReader.Token != JsonToken.ObjectStart)
			{
				throw new JsonException("Json string has no opening brace");
			}
			allPowerups = new Dictionary<string, Powerup>();
			while (jsonReader.Read() && jsonReader.Token != JsonToken.ObjectEnd)
			{
				string text = (string)jsonReader.Value;
				Powerup value = CreatePowerupFromJSONReader(jsonReader);
				allPowerups[text] = value;
			}
		}

		private static Powerup CreatePowerupFromJSONReader(JsonReader reader)
		{
			Powerup powerup = new Powerup();
			reader.Read();
			if (reader.Token != JsonToken.ObjectStart)
			{
				throw new JsonException("Json powerup has no opening brace");
			}
			while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
			{
				switch ((string)reader.Value)
				{
				case "title":
					reader.Read();
					powerup.title = (string)reader.Value;
					break;
				case "description":
					reader.Read();
					powerup.description = (string)reader.Value;
					break;
				case "key":
					reader.Read();
					powerup.key = (string)reader.Value;
					break;
				case "mods":
					powerup.mods = CreateModsDictionaryFromJSONReader(reader);
					break;
				case "stackingValue":
					powerup.stackingValue = (float)reader.Value;
					break;
				default:
					throw new JsonException("Unrecognized property name: " + (string)reader.Value);
				}
			}
			return powerup;
		}

		private static Dictionary<string, float> CreateModsDictionaryFromJSONReader(JsonReader reader)
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			reader.Read();
			if (reader.Token != JsonToken.ObjectStart)
			{
				throw new JsonException("Mods object has no opening brace");
			}
			while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
			{
				string text = (string)reader.Value;
				reader.Read();
				dictionary[text] = Convert.ToSingle(reader.Value);
			}
			return dictionary;
		}

		public void AddToSelected()
		{
			if (selectedPowerups == null)
			{
				selectedPowerups = new List<string>();
			}
			if (appliedMods == null)
			{
				appliedMods = new Dictionary<string, float>();
			}
			float num = 1f;
			foreach (string selectedPowerup in selectedPowerups)
			{
				if (selectedPowerup == key)
				{
					num *= stackingValue;
				}
			}
			selectedPowerups.Add(key);
			foreach (KeyValuePair<string, float> mod in mods)
			{
				if (appliedMods.ContainsKey(mod.Key))
				{
					if (mod.Value >= 1f)
					{
						appliedMods[mod.Key] += (mod.Value - 1f) * num;
					}
					else
					{
						appliedMods[mod.Key] -= (1f - mod.Value) * num;
					}
				}
				else
				{
					appliedMods[mod.Key] = mod.Value;
				}
			}
		}

		public void RemoveFromSelected()
		{
			appliedMods = new Dictionary<string, float>();
			if (selectedPowerups.Contains(key))
			{
				selectedPowerups.Remove(key);
			}
			foreach (string selectedPowerup in selectedPowerups)
			{
				allPowerups[selectedPowerup].AddToSelected();
			}
		}

		protected void AddToPowerupsDictionary()
		{
			if (allPowerups == null)
			{
				allPowerups = new Dictionary<string, Powerup>();
			}
			allPowerups[key] = this;
		}
	}
}
