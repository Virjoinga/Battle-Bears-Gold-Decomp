using System.Collections;
using UnityEngine;

public class SuperSizeMeAbility : ShrinkAbility
{
	private const float SECONDS_BETWEEN_STOMPS = 1f;

	private const float DEFAULT_SLOW_PERCENTAGE = 0.75f;

	private const float DEFAULT_HEAL_AMOUNT_PER_TICK = 10f;

	private const float DEFAULT_HEAL_INTERVAL = 0.5f;

	private PlayerController _playerController;

	private PlayerDamageReceiver _playerDamageReceiver;

	public float SlowPercentage = 0.75f;

	public float HealAmount = 10f;

	public float HealInterval = 0.5f;

	protected override void Awake()
	{
		base.Awake();
		_maxScale = 2.5f;
		minScale = 1f;
		_playerController = GetComponent<PlayerController>();
		_playerDamageReceiver = GetComponent<PlayerDamageReceiver>();
	}

	protected override void SetupBeginningAndEndingRoutines()
	{
		_beginningResizeRoutine = GrowSlowStomp();
		_endingResizeRoutine = shrink();
	}

	private IEnumerator GrowSlowStomp()
	{
		yield return StartCoroutine(grow());
		SlowHuggable();
		StartCoroutine(HPRegen());
		StartCoroutine(SpawnAoeStomps());
	}

	private IEnumerator SpawnAoeStomps()
	{
		while (true)
		{
			GameObject stompClone = SpawnStompClone();
			stompClone.transform.SetParent(base.transform);
			SetupStompCloneNetworkObject(stompClone);
			yield return new WaitForSeconds(1f);
		}
	}

	private GameObject SpawnStompClone()
	{
		float y = ((!(character != null)) ? (capsuleCollider.height * 0.5f) : (character.height * 0.5f));
		Vector3 position = base.transform.position - new Vector3(0f, y, 0f);
		return Object.Instantiate(effectPrefab, position, base.transform.rotation) as GameObject;
	}

	private void SetupStompCloneNetworkObject(GameObject stompClone)
	{
		ConfigurableNetworkObject component = stompClone.GetComponent<ConfigurableNetworkObject>();
		component.SetItemOverride("superSizeMe");
		component.SetEquipmentNames("superSizeMe");
		component.OwnerID = _playerController.OwnerID;
	}

	private void SlowHuggable()
	{
		float num = (_maxScale - 1f) / growSpeed * Time.deltaTime;
		_playerController.GetSlowedByRemotePlayer(duration - num, 0f, SlowPercentage);
	}

	private IEnumerator HPRegen()
	{
		while (true)
		{
			_playerDamageReceiver.addHealth(HealAmount);
			yield return new WaitForSeconds(HealInterval);
		}
	}
}
