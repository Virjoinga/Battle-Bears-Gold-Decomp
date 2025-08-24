using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FontMetrics
{
	private TextMesh textMesh;

	private static Dictionary<string, Dictionary<char, Vector3>> metrics = new Dictionary<string, Dictionary<char, Vector3>>();

	private Font textMeshFont;

	private int fontSize;

	private string fontIdentifier;

	public int numLines;

	public FontMetrics(TextMesh tm, float size)
	{
		textMesh = tm;
		Renderer renderer = textMesh.renderer;
		textMeshFont = textMesh.font;
		fontSize = (int)size;
		fontIdentifier = textMeshFont.name + fontSize;
		if (metrics.ContainsKey(fontIdentifier))
		{
			return;
		}
		Dictionary<char, Vector3> dictionary = new Dictionary<char, Vector3>();
		string text = textMesh.text;
		for (ushort num = 0; num < 2000; num++)
		{
			char c = (char)num;
			if (textMeshFont.HasCharacter(c))
			{
				textMesh.text = c.ToString();
				dictionary.Add(c, renderer.bounds.size);
			}
		}
		metrics.Add(fontIdentifier, dictionary);
		textMesh.text = "A A";
		Vector3 value = dictionary[' '];
		value.x = renderer.bounds.size.x - GetWidth(textMesh.text);
		dictionary[' '] = value;
		textMesh.text = text;
	}

	private static string CapText(Match m)
	{
		string text = m.ToString();
		if (char.IsLower(text[0]))
		{
			return char.ToUpper(text[0]) + text.Substring(1, text.Length - 1);
		}
		return text;
	}

	public int GetNumberLines(Bounds boundary)
	{
		return (int)Math.Floor(Mathf.Max(boundary.size.y, boundary.size.z) / Mathf.Max(metrics[fontIdentifier][' '].y, metrics[fontIdentifier][' '].z));
	}

	public float GetWidth(string str)
	{
		float num = 0f;
		foreach (char key in str)
		{
			if (metrics[fontIdentifier].ContainsKey(key))
			{
				num += metrics[fontIdentifier][key].x;
			}
		}
		return num;
	}

	public string Format(string str, Bounds boundary, int newLineSpaces, Transform parent, bool heightClamp)
	{
		float num = boundary.size.x;
		float num2 = boundary.size.y;
		string text = string.Empty;
		int num3 = 0;
		string text2 = string.Empty;
		for (int i = 0; i < newLineSpaces; i++)
		{
			text2 += " ";
		}
		float num4 = 0f;
		Vector3 vector = metrics[fontIdentifier][' '];
		float num5 = Mathf.Max(vector.y, vector.z);
		for (int j = 0; j < str.Length; j++)
		{
			char c = str[j];
			char c2 = c;
			if (c2 == '\n')
			{
				if (!heightClamp || num2 > 2f * num5)
				{
					num3 = j;
					text += c;
					num = boundary.size.x;
					num4 += vector.y;
					num2 -= vector.y;
					numLines++;
					continue;
				}
				return addTrailingDots(text);
			}
			Vector3 vector2 = Vector3.zero;
			if (metrics[fontIdentifier].ContainsKey(c))
			{
				vector2 = metrics[fontIdentifier][c];
			}
			bool flag = false;
			if (num <= vector2.x)
			{
				numLines++;
				if (heightClamp && !(num2 > 2f * Mathf.Max(vector2.y, vector2.z)))
				{
					return addTrailingDots(text);
				}
				int num6 = text.LastIndexOfAny(" \n".ToCharArray());
				if (num6 == -1)
				{
					text = text + "\n" + text2;
					flag = true;
					num = boundary.size.x - (float)newLineSpaces * vector.x;
					num3 = j;
				}
				else
				{
					int num7 = str.IndexOfAny(" \n".ToCharArray(), num3 + 1);
					string str2 = str.Substring(num3);
					if (num7 != -1)
					{
						str2 = str.Substring(num3, num7 - num3);
					}
					if (GetWidth(str2) < boundary.size.x - (float)newLineSpaces * vector.x)
					{
						text = text.Remove(num6, 1).Insert(num6, "\n" + text2);
						num = boundary.size.x - (float)newLineSpaces * vector.x;
						flag = true;
						if (j >= num3)
						{
							num -= GetWidth(str.Substring(num3, j - num3));
						}
						num3 = j;
					}
					else
					{
						text = text + "\n" + text2;
						num = boundary.size.x - (float)newLineSpaces * vector.x;
						num3 = j;
						flag = true;
					}
				}
				num4 += vector.y;
			}
			if (flag)
			{
				num2 -= vector.y;
			}
			if (c == ' ')
			{
				num3 = j + 1;
			}
			text += c;
			if (!flag)
			{
				num -= vector2.x;
			}
		}
		return text;
	}

	private string addTrailingDots(string s)
	{
		int num = 3;
		string text = s.Substring(0, s.Length - num);
		for (int i = 0; i < num; i++)
		{
			text += '.';
		}
		return text;
	}
}
