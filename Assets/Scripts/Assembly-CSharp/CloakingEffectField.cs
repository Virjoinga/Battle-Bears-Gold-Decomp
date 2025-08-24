using System.Collections.Generic;
using UnityEngine;

public class CloakingEffectField : EffectField
{
	private Dictionary<string, CamouflageCloak> _cloaks = new Dictionary<string, CamouflageCloak>();

	protected override void Update()
	{
		base.Update();
		RemoveDeletedOrNullCloaks();
	}

	private void RemoveDeletedOrNullCloaks()
	{
		List<KeyValuePair<string, CamouflageCloak>> list = new List<KeyValuePair<string, CamouflageCloak>>();
		foreach (KeyValuePair<string, CamouflageCloak> cloak in _cloaks)
		{
			if (cloak.Value == null)
			{
				list.Add(cloak);
			}
		}
		foreach (KeyValuePair<string, CamouflageCloak> item in list)
		{
			_cloaks.Remove(item.Key);
		}
	}

	protected override void ApplyEffect(PlayerController pc)
	{
		base.ApplyEffect(pc);
		GameObject gameObject = new GameObject("CloakField");
		CamouflageCloak camouflageCloak = gameObject.AddComponent<CamouflageCloak>();
		camouflageCloak.Activate(pc, pc.isRemote, 0f);
		pc.DamageReceiver.Cloak = camouflageCloak;
		if (!_cloaks.ContainsKey(pc.name))
		{
			_cloaks.Add(pc.name, camouflageCloak);
		}
	}

	protected override void RemoveEffect(PlayerController pc)
	{
		base.RemoveEffect(pc);
		if (_cloaks.ContainsKey(pc.name) && _cloaks[pc.name] != null)
		{
			_cloaks[pc.name].OnDeactivate(0f);
			_cloaks.Remove(pc.name);
		}
	}
}
