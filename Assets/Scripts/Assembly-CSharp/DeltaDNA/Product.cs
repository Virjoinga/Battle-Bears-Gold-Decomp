using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace DeltaDNA
{
	public class Product<T> : Params where T : Product<T>
	{
		private List<Dictionary<string, object>> virtualCurrencies;

		private List<Dictionary<string, object>> items;

		private static readonly IDictionary<string, int> ISO4217;

		static Product()
		{
			ISO4217 = new Dictionary<string, int>();
			using (XmlReader xmlReader = XmlReader.Create(new StringReader((Resources.Load("iso_4217", typeof(TextAsset)) as TextAsset).text)))
			{
				bool flag = false;
				bool flag2 = false;
				string text = null;
				string text2 = null;
				while (xmlReader.Read())
				{
					switch (xmlReader.NodeType)
					{
					case XmlNodeType.Element:
						if (xmlReader.Name.Equals("Ccy"))
						{
							flag = true;
						}
						else if (xmlReader.Name.Equals("CcyMnrUnts"))
						{
							flag2 = true;
						}
						break;
					case XmlNodeType.Text:
						if (flag)
						{
							text = xmlReader.Value;
						}
						else if (flag2)
						{
							text2 = xmlReader.Value;
						}
						break;
					case XmlNodeType.EndElement:
						if (xmlReader.Name.Equals("Ccy"))
						{
							flag = false;
						}
						else if (xmlReader.Name.Equals("CcyMnrUnts"))
						{
							flag2 = false;
						}
						else
						{
							if (!xmlReader.Name.Equals("CcyNtry"))
							{
								break;
							}
							if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
							{
								int value;
								try
								{
									value = int.Parse(text2);
								}
								catch (FormatException)
								{
									value = 0;
								}
								ISO4217[text] = value;
							}
							flag = false;
							flag2 = false;
							text = null;
							text2 = null;
						}
						break;
					}
				}
			}
		}

		public T SetRealCurrency(string type, int amount)
		{
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentException("Type cannot be null or empty");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("realCurrencyType", type);
			dictionary.Add("realCurrencyAmount", amount);
			Dictionary<string, object> value = dictionary;
			AddParam("realCurrency", value);
			return (T)this;
		}

		public T AddVirtualCurrency(string name, string type, long amount)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty");
			}
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentException("Type cannot be null or empty");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("virtualCurrency", new Dictionary<string, object>
			{
				{ "virtualCurrencyName", name },
				{ "virtualCurrencyType", type },
				{ "virtualCurrencyAmount", amount }
			});
			Dictionary<string, object> item = dictionary;
			if (GetParam("virtualCurrencies") == null)
			{
				virtualCurrencies = new List<Dictionary<string, object>>();
				AddParam("virtualCurrencies", virtualCurrencies);
			}
			virtualCurrencies.Add(item);
			return (T)this;
		}

		public T AddItem(string name, string type, int amount)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty");
			}
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentException("Type cannot be null or empty");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("item", new Dictionary<string, object>
			{
				{ "itemName", name },
				{ "itemType", type },
				{ "itemAmount", amount }
			});
			Dictionary<string, object> item = dictionary;
			if (GetParam("items") == null)
			{
				items = new List<Dictionary<string, object>>();
				AddParam("items", items);
			}
			items.Add(item);
			return (T)this;
		}

		public static int ConvertCurrency(string code, decimal value)
		{
			if (ISO4217.ContainsKey(code))
			{
				return decimal.ToInt32(value * (decimal)Math.Pow(10.0, ISO4217[code]));
			}
			Debug.LogWarning("Failed to find currency for: " + code);
			return 0;
		}
	}
	public class Product : Product<Product>
	{
	}
}
