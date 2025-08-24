using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
	private static readonly string _shieldTag = "Shield";

	[SerializeField]
	private Material _shieldMaterialOriginal;

	[SerializeField]
	private Color _teamBlueColor = Color.blue;

	[SerializeField]
	private Color _teamRedColor = Color.red;

	[SerializeField]
	private float _damageShieldAmount;

	[SerializeField]
	private float _shieldDamageMultiplier = 1f;

	[SerializeField]
	private string _itemOverride;

	private Material _shieldMaterial;

	private float _defaultDuration = 10f;

	private PlayerDamageReceiver _receiver;

	private PlayerController _controller;

	private List<Renderer> _renderers = new List<Renderer>();

	private void Awake()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName(_itemOverride);
		Object.Destroy(t: (!itemByName.properties.ContainsKey("duration")) ? _defaultDuration : ((float)itemByName.properties["duration"]), obj: base.gameObject);
	}

	private void Start()
	{
		_receiver = base.transform.root.GetComponent<PlayerDamageReceiver>();
		_controller = base.transform.root.GetComponent<PlayerController>();
		if (_receiver != null)
		{
			_receiver.DamageShieldValue += _damageShieldAmount;
			_receiver.DamageMultipliers.Add(_shieldDamageMultiplier);
		}
		if (_shieldMaterialOriginal != null)
		{
			_shieldMaterial = new Material(_shieldMaterialOriginal);
		}
		if (!(_shieldMaterial != null))
		{
			return;
		}
		PlayerController component = base.transform.root.GetComponent<PlayerController>();
		if (component != null)
		{
			_shieldMaterial.color = ((component.Team != Team.BLUE) ? _teamRedColor : _teamBlueColor);
		}
		AddCharacterRenderers();
		AddWeaponRenderers();
		foreach (Renderer renderer in _renderers)
		{
			if (renderer != null && renderer.tag == _shieldTag)
			{
				Material[] array = new Material[renderer.materials.Length + 1];
				for (int i = 0; i < renderer.materials.Length; i++)
				{
					array[i] = renderer.materials[i];
				}
				array[array.Length - 1] = _shieldMaterial;
				renderer.materials = array;
			}
		}
	}

	private void Update()
	{
		if (_controller != null && _controller.IsDead)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (_receiver != null)
		{
			_receiver.DamageMultipliers.Remove(_shieldDamageMultiplier);
		}
		if (!(_shieldMaterial != null))
		{
			return;
		}
		foreach (Renderer renderer in _renderers)
		{
			if (!(renderer != null) || !(renderer.tag == _shieldTag))
			{
				continue;
			}
			Material[] materials = renderer.materials;
			int num = 0;
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i].name.Contains(_shieldMaterial.name))
				{
					num++;
					break;
				}
			}
			Material[] array = new Material[materials.Length - num];
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j].name.Contains(_shieldMaterial.name))
				{
					if (num3 < num)
					{
						num3++;
					}
					else
					{
						array[num2++] = materials[j];
					}
				}
				else
				{
					array[num2++] = materials[j];
				}
			}
			renderer.materials = array;
		}
	}

	private void AddCharacterRenderers()
	{
		CharacterHandle component = base.transform.root.GetComponent<CharacterHandle>();
		if (component != null)
		{
			Renderer[] renderers = component.Renderers;
			foreach (Renderer item in renderers)
			{
				_renderers.Add(item);
			}
		}
	}

	private void AddWeaponRenderers()
	{
		WeaponBase[] componentsInChildren = base.transform.root.GetComponentsInChildren<WeaponBase>();
		WeaponBase[] array = componentsInChildren;
		foreach (WeaponBase weaponBase in array)
		{
			Renderer component = weaponBase.GetComponent<Renderer>();
			Renderer[] componentsInChildren2 = weaponBase.GetComponentsInChildren<Renderer>();
			_renderers.Add(component);
			Renderer[] array2 = componentsInChildren2;
			foreach (Renderer item in array2)
			{
				_renderers.Add(item);
			}
		}
	}
}
