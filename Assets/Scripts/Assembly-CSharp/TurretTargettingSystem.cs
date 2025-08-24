using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargettingSystem : MonoBehaviour
{
	public Transform aimer;

	public TargettingSystem.LockStart lockStartLocation;

	public float lockRadius = 100f;

	public float lockDistance = 100f;

	public float loseLockDelay = 2f;

	public float maxLockAngle = 45f;

	public LayerMask targetMask;

	public LayerMask lineOfSightMask;

	public Transform currentTarget;

	public Transform lockedTarget;

	public bool isLocking;

	private bool isLosingLock;

	private DeployableTurret _myTurretController;

	private Vector3 _aimerPosition;

	private Vector3 _aimerDirection;

	protected string equipmentNames = string.Empty;

	protected string configureItemName = string.Empty;

	public void SetItemOverride(string str)
	{
		configureItemName = str;
	}

	public void SetEquipmentNames(string str)
	{
		equipmentNames = str;
	}

	public void ForwardSettings(ConfigurableNetworkObject n)
	{
		n.SetItemOverride(configureItemName);
		n.SetEquipmentNames(equipmentNames);
	}

	private void Start()
	{
		ConfigureObject();
		_myTurretController = base.transform.root.GetComponentInChildren(typeof(DeployableTurret)) as DeployableTurret;
		if (_myTurretController != null && !_myTurretController.OwningPlayer.isRemote)
		{
			_aimerPosition = new Vector3(aimer.position.x, aimer.position.y, aimer.position.z);
			_aimerDirection = new Vector3(aimer.forward.x, aimer.forward.y, aimer.forward.z);
			StartCoroutine("trackTarget");
		}
	}

	public void ConfigureObject()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("lockRadius", ref lockRadius, equipmentNames);
			itemByName.UpdateProperty("lockAngle", ref maxLockAngle, equipmentNames);
			itemByName.UpdateProperty("lockDistance", ref lockDistance, equipmentNames);
		}
	}

	private IEnumerator trackTarget()
	{
		while (aimer != null)
		{
			bool foundTarget = false;
			RaycastHit[] hits = ((lockStartLocation == TargettingSystem.LockStart.BEHIND) ? Physics.SphereCastAll(_aimerPosition - _aimerDirection * lockRadius, lockRadius, _aimerDirection, lockDistance, targetMask) : ((lockStartLocation == TargettingSystem.LockStart.AHEAD) ? Physics.SphereCastAll(_aimerPosition + _aimerDirection * lockRadius, lockRadius, _aimerDirection, lockDistance, targetMask) : ((lockStartLocation != TargettingSystem.LockStart.HALF) ? Physics.SphereCastAll(_aimerPosition, lockRadius, _aimerDirection, lockDistance, targetMask) : Physics.SphereCastAll(_aimerPosition + _aimerDirection * lockRadius / 2f, lockRadius, _aimerDirection, lockDistance, targetMask))));
			if (hits.Length > 0)
			{
				if (_myTurretController == null)
				{
					yield return new WaitForSeconds(0.1f);
					continue;
				}
				SortedDictionary<float, TargetableObject> potentialTargets = new SortedDictionary<float, TargetableObject>();
				for (int i = 0; i < hits.Length; i++)
				{
					TargetableObject otherTarget = hits[i].transform.root.GetComponentInChildren<TargetableObject>();
					if (otherTarget != null && otherTarget.Team != _myTurretController.OwningPlayer.Team)
					{
						float distance = Vector3.Distance(_myTurretController.transform.position, otherTarget.transform.position);
						float angle = Vector3.Angle(_aimerDirection, otherTarget.transform.position - _myTurretController.transform.position);
						if (angle < maxLockAngle / 2f && !potentialTargets.ContainsKey(distance))
						{
							potentialTargets.Add(distance, otherTarget);
						}
					}
				}
				SortedDictionary<float, TargetableObject> visiblePotentialTargets = new SortedDictionary<float, TargetableObject>();
				_myTurretController.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				foreach (KeyValuePair<float, TargetableObject> kvp in potentialTargets)
				{
					if (kvp.Value.transform == null || kvp.Value.transform.GetComponent<Collider>() == null)
					{
						continue;
					}
					Bounds playerBounds = kvp.Value.transform.GetComponent<Collider>().bounds;
					Vector3[] pointsToCheck = new Vector3[2]
					{
						new Vector3(playerBounds.center.x, playerBounds.max.y, playerBounds.center.z),
						new Vector3(playerBounds.center.x, playerBounds.min.y, playerBounds.center.z)
					};
					kvp.Value.gameObject.layer = LayerMask.NameToLayer("GUI");
					bool canSeeTarget = false;
					for (int j = 0; j < pointsToCheck.Length; j++)
					{
						RaycastHit hit;
						if (Physics.Raycast(_aimerPosition, (pointsToCheck[j] - _aimerPosition).normalized, out hit, lockDistance, lineOfSightMask) && hit.transform.gameObject.layer == LayerMask.NameToLayer("GUI"))
						{
							Transform playerHighlight = hit.transform.Find("playerHighlight");
							if (playerHighlight != null && playerHighlight.gameObject.layer == LayerMask.NameToLayer("Player"))
							{
								canSeeTarget = true;
								break;
							}
						}
					}
					if (canSeeTarget)
					{
						visiblePotentialTargets.Add(kvp.Key, kvp.Value);
					}
					kvp.Value.gameObject.layer = LayerMask.NameToLayer("Player");
				}
				Transform actualTarget = null;
				if (currentTarget != null)
				{
					foreach (KeyValuePair<float, TargetableObject> item in visiblePotentialTargets)
					{
						if (item.Value.transform == currentTarget)
						{
							actualTarget = currentTarget;
							break;
						}
					}
				}
				if (actualTarget == null)
				{
					using (SortedDictionary<float, TargetableObject>.Enumerator enumerator3 = visiblePotentialTargets.GetEnumerator())
					{
						if (enumerator3.MoveNext())
						{
							actualTarget = enumerator3.Current.Value.transform;
						}
					}
				}
				if (actualTarget != null)
				{
					if (currentTarget == null)
					{
						StartCoroutine("beginLock");
					}
					else if (currentTarget != actualTarget)
					{
						loseLock();
						StartCoroutine("beginLock");
					}
					currentTarget = actualTarget;
					if (isLosingLock)
					{
						StopCoroutine("losingLock");
						isLosingLock = false;
					}
					foundTarget = true;
				}
				_myTurretController.gameObject.layer = LayerMask.NameToLayer("Player");
			}
			if (!foundTarget)
			{
				if (lockedTarget != null && !isLosingLock)
				{
					StartCoroutine("losingLock");
				}
				if (lockedTarget == null)
				{
					loseLock();
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void loseLock()
	{
		StopCoroutine("losingLock");
		StopCoroutine("beginLock");
		lockedTarget = null;
		currentTarget = null;
		isLosingLock = false;
		isLocking = false;
	}

	private IEnumerator losingLock()
	{
		isLosingLock = true;
		yield return new WaitForSeconds(loseLockDelay);
		loseLock();
	}

	private IEnumerator beginLock()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.2f);
			lockedTarget = currentTarget;
		}
	}
}
