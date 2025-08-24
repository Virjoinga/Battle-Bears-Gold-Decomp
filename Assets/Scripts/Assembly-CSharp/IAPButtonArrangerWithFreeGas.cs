using UnityEngine;

public class IAPButtonArrangerWithFreeGas : IAPButtonArranger
{
	[SerializeField]
	private GameObject _freeGasButton;

	public bool FreeGasEnabled
	{
		get
		{
			return _freeGasButton.activeSelf;
		}
		set
		{
			_freeGasButton.SetActive(value);
		}
	}

	protected override void CreateButtonsInGrid()
	{
		base.CreateButtonsInGrid();
		IIAPButton[] array = new IIAPButton[base.Buttons.Length - 1];
		for (int i = 0; i < base.Buttons.Length - 1; i++)
		{
			array[i] = base.Buttons[i];
		}
		_freeGasButton = Object.Instantiate(_freeGasButton, ((MonoBehaviour)base.Buttons[base.Buttons.Length - 1]).gameObject.transform.position, ((MonoBehaviour)base.Buttons[base.Buttons.Length - 1]).gameObject.transform.rotation) as GameObject;
		_freeGasButton.transform.parent = _upperLeftPosition;
		_freeGasButton.name = _freeGasButton.name.Replace("(Clone)", string.Empty);
		Object.Destroy(((MonoBehaviour)base.Buttons[base.Buttons.Length - 1]).gameObject);
		int val = -1;
		ServiceManager.Instance.UpdateProperty("offerwall_level", ref val);
		if (val != -1 && ServiceManager.Instance.GetStats().level >= (double)val)
		{
			FreeGasEnabled = true;
		}
		else
		{
			FreeGasEnabled = false;
		}
	}
}
