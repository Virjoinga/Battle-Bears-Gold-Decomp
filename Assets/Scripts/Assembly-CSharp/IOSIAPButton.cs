using UnityEngine;

public class IOSIAPButton : MonoBehaviour, IIAPButton
{
	[SerializeField]
	private TextMesh _realMoneyText;

	[SerializeField]
	private TextMesh _inGameCurrencyText;

	[SerializeField]
	private MeshRenderer _gasCansMesh;

	[SerializeField]
	private MeshRenderer _joulesMesh;

	[SerializeField]
	private GameObject[] _objectsToDisableWhenButtonDisabled;

	[SerializeField]
	private GameObject _bestDealMarker;

	private IAPButtonCurrencyTypes _currencyType;

	public string RealMoneyPrice
	{
		get
		{
			return _realMoneyText.text;
		}
		set
		{
			_realMoneyText.text = value;
		}
	}

	public int InGameCurrencyValue
	{
		get
		{
			return int.Parse(_inGameCurrencyText.text);
		}
		set
		{
			_inGameCurrencyText.text = value.ToString();
		}
	}

	public IAPButtonCurrencyTypes CurrencyType
	{
		get
		{
			return _currencyType;
		}
		set
		{
			_currencyType = value;
			_gasCansMesh.enabled = false;
			_joulesMesh.enabled = false;
			switch (value)
			{
			case IAPButtonCurrencyTypes.GasCans:
				_gasCansMesh.enabled = true;
				break;
			case IAPButtonCurrencyTypes.Joules:
				_joulesMesh.enabled = true;
				break;
			}
		}
	}

	public bool Disabled
	{
		set
		{
			if (value)
			{
				_realMoneyText.gameObject.SetActiveRecursively(false);
				_inGameCurrencyText.gameObject.SetActiveRecursively(false);
				_gasCansMesh.gameObject.SetActiveRecursively(false);
				_joulesMesh.gameObject.SetActiveRecursively(false);
				if (_objectsToDisableWhenButtonDisabled != null)
				{
					GameObject[] objectsToDisableWhenButtonDisabled = _objectsToDisableWhenButtonDisabled;
					foreach (GameObject gameObject in objectsToDisableWhenButtonDisabled)
					{
						gameObject.SetActiveRecursively(false);
					}
				}
				return;
			}
			_realMoneyText.gameObject.SetActiveRecursively(false);
			_inGameCurrencyText.gameObject.SetActiveRecursively(true);
			_gasCansMesh.gameObject.SetActiveRecursively(CurrencyType == IAPButtonCurrencyTypes.GasCans);
			_joulesMesh.gameObject.SetActiveRecursively(CurrencyType == IAPButtonCurrencyTypes.Joules);
			if (_objectsToDisableWhenButtonDisabled != null)
			{
				GameObject[] objectsToDisableWhenButtonDisabled2 = _objectsToDisableWhenButtonDisabled;
				foreach (GameObject gameObject2 in objectsToDisableWhenButtonDisabled2)
				{
					gameObject2.SetActiveRecursively(true);
				}
			}
		}
	}

	public string CurrencyCode { get; set; }

	public string IAPProductID { get; set; }

	public bool BestDeal
	{
		set
		{
			_bestDealMarker.SetActive(value);
		}
	}
}
