using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI
{
	private float _smallestRatio;

	private float _defaultGUISizeX = 1280f;

	private float _defaultGUISizeY = 720f;

	private List<GUIComponent> _components = new List<GUIComponent>();

	private List<UpdatedGUIComponent> _updated = new List<UpdatedGUIComponent>();

	private List<KeyValuePair<float, RenderedGUIComponent>> _rendered = new List<KeyValuePair<float, RenderedGUIComponent>>();

	private List<KeyValuePair<float, InputGUIComponent>> _input = new List<KeyValuePair<float, InputGUIComponent>>();

	public static PlayerGUI Instance { get; private set; }

	public float HorizontalRatio { get; private set; }

	public float VerticalRatio { get; private set; }

	public float SmallestRatio
	{
		get
		{
			return _smallestRatio;
		}
	}

	public PlayerGUI()
	{
		Instance = this;
		HorizontalRatio = (float)Screen.width / _defaultGUISizeX;
		VerticalRatio = (float)Screen.height / _defaultGUISizeY;
		_smallestRatio = ((!(HorizontalRatio < VerticalRatio)) ? VerticalRatio : HorizontalRatio);
	}

	public void AddComponent(GUIComponent component)
	{
		if (!_components.Contains(component))
		{
			_components.Add(component);
			component.AddTo(this);
		}
	}

	public void RemoveComponent(GUIComponent component)
	{
		if (_components.Remove(component))
		{
			component.RemoveFrom(this);
		}
	}

	public void RemoveAll()
	{
		foreach (GUIComponent component in _components)
		{
			component.RemoveFrom(this);
		}
		_components.RemoveRange(0, _components.Count);
	}

	public void AddUpdatedComponent(UpdatedGUIComponent component)
	{
		_updated.Add(component);
	}

	public void RemoveUpdatedComponent(UpdatedGUIComponent component)
	{
		_updated.Remove(component);
	}

	public void AddRenderedComponent(float depth, RenderedGUIComponent component)
	{
		_rendered.Add(new KeyValuePair<float, RenderedGUIComponent>(depth, component));
		_rendered.Sort((KeyValuePair<float, RenderedGUIComponent> a, KeyValuePair<float, RenderedGUIComponent> b) => a.Key.CompareTo(b.Key));
	}

	public void RemoveRenderedComponent(RenderedGUIComponent component)
	{
		foreach (KeyValuePair<float, RenderedGUIComponent> item in _rendered)
		{
			if (item.Value == component)
			{
				_rendered.Remove(item);
				break;
			}
		}
	}

	public void AddInputComponent(float depth, InputGUIComponent component)
	{
		_input.Add(new KeyValuePair<float, InputGUIComponent>(depth, component));
		_input.Sort((KeyValuePair<float, InputGUIComponent> a, KeyValuePair<float, InputGUIComponent> b) => -a.Key.CompareTo(b.Key));
	}

	public void RemoveInputComponent(InputGUIComponent component)
	{
		foreach (KeyValuePair<float, InputGUIComponent> item in _input)
		{
			if (item.Value == component)
			{
				_input.Remove(item);
				break;
			}
		}
	}

	public void UpdateGUI(float delta)
	{
		SVTouchInput.UpdateTouches();
		foreach (KeyValuePair<float, InputGUIComponent> item in _input)
		{
			item.Value.WipeInput();
		}
		SVTouch[] touches = SVTouchInput.Touches;
		SVTouch[] array = touches;
		foreach (SVTouch touch in array)
		{
			bool flag = false;
			foreach (KeyValuePair<float, InputGUIComponent> item2 in _input)
			{
				if (item2.Value.ClaimsInput(touch))
				{
					item2.Value.ConsumeInput(touch);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			using (List<KeyValuePair<float, InputGUIComponent>>.Enumerator enumerator3 = _input.GetEnumerator())
			{
				while (enumerator3.MoveNext() && !enumerator3.Current.Value.ConsumeInput(touch))
				{
				}
			}
		}
		foreach (KeyValuePair<float, InputGUIComponent> item3 in _input)
		{
			item3.Value.FinalizeInput();
		}
		foreach (UpdatedGUIComponent item4 in _updated)
		{
			item4.UpdateGUI(delta);
		}
	}

	public void RenderGUI()
	{
		foreach (KeyValuePair<float, RenderedGUIComponent> item in _rendered)
		{
			item.Value.RenderGUI();
		}
	}
}
