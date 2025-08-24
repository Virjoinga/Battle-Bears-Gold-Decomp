using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargettingSystem : MonoBehaviour
{
	public enum LockStart
	{
		BEHIND = 0,
		NORMAL = 1,
		AHEAD = 2,
		HALF = 3
	}

	public Transform lockedTarget;

	public LayerMask targetMask;

	public LayerMask lineOfSightMask;

	public float targettingTime = 3f;

	public float lockRadius = 100f;

	public float trackingVisualSpeed = 20f;

	public GameObject targettingSystemPrefab;

	protected GameObject targettingSystem;

	protected Transform targettingSystemTransform;

	private Vector3 targetHUDPos;

	private float targettingTimeLeft;

	public Transform currentTarget;

	private Transform aimer;

	private Transform myTransform;

	public bool isLocking;

	private bool isLosingLock;

	public float loseLockDelay = 1f;

	public LockStart lockStartLocation;

	protected virtual void Awake()
	{
		myTransform = base.transform;
	}

	protected virtual void Start()
	{
		aimer = myTransform.root.Find("aimer");
		targettingSystem = Object.Instantiate(targettingSystemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		targettingSystemTransform = targettingSystem.transform;
		targettingSystem.SetActiveRecursively(false);
		if (aimer != null)
		{
			StartCoroutine("trackTarget");
		}
	}

	private IEnumerator trackTarget()
	{
		targettingSystem.SetActiveRecursively(false);
		while (aimer != null)
		{
			if (HUD.Instance != null && HUD.Instance.PlayerController != null && HUD.Instance.PlayerController.IsDead)
			{
				yield return new WaitForSeconds(0.5f);
				continue;
			}
			bool foundTarget = false;
			RaycastHit[] hits = ((lockStartLocation == LockStart.BEHIND) ? Physics.SphereCastAll(aimer.position - aimer.forward * lockRadius, lockRadius, aimer.forward, 5000f, targetMask) : ((lockStartLocation == LockStart.AHEAD) ? Physics.SphereCastAll(aimer.position + aimer.forward * lockRadius, lockRadius, aimer.forward, 5000f, targetMask) : ((lockStartLocation != LockStart.HALF) ? Physics.SphereCastAll(aimer.position, lockRadius, aimer.forward, 5000f, targetMask) : Physics.SphereCastAll(aimer.position + aimer.forward * lockRadius / 2f, lockRadius, aimer.forward, 5000f, targetMask))));
			PlayerController myChar = null;
			if (hits.Length > 0)
			{
				myChar = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
				if (myChar == null)
				{
					yield return new WaitForSeconds(0.1f);
					continue;
				}
				SortedDictionary<float, TargetableObject> potentialTargets = new SortedDictionary<float, TargetableObject>();
				for (int i = 0; i < hits.Length; i++)
				{
					TargetableObject otherTarget = hits[i].transform.root.GetComponentInChildren<TargetableObject>();
					if (otherTarget != null && otherTarget.Team != myChar.Team)
					{
						float distance = Vector3.Distance(myChar.transform.position, otherTarget.transform.position);
						if (!potentialTargets.ContainsKey(distance))
						{
							potentialTargets.Add(distance, otherTarget);
						}
					}
				}
				SortedDictionary<float, TargetableObject> visiblePotentialTargets = new SortedDictionary<float, TargetableObject>();
				myChar.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
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
						if (Physics.Raycast(aimer.position, (pointsToCheck[j] - aimer.position).normalized, out hit, 5000f, lineOfSightMask) && hit.transform.gameObject.layer == LayerMask.NameToLayer("GUI"))
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
					Bounds targetBounds2 = actualTarget.GetComponent<Collider>().bounds;
					if (HUD.Instance.PlayerCamera != null && HUD.Instance.hudCamera != null)
					{
						Vector3 screenPoint2 = HUD.Instance.PlayerCamera.GetComponent<Camera>().WorldToScreenPoint(targetBounds2.center);
						Vector3 hudPos2 = HUD.Instance.hudCamera.ScreenToWorldPoint(screenPoint2);
						targetHUDPos = hudPos2;
						if (currentTarget == null)
						{
							targettingSystemTransform.position = targetHUDPos;
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
				}
				myChar.gameObject.layer = LayerMask.NameToLayer("Player");
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
			if (isLosingLock && lockedTarget != null && HUD.Instance.PlayerCamera != null && HUD.Instance.hudCamera != null)
			{
				Bounds targetBounds = lockedTarget.transform.GetComponent<Collider>().bounds;
				Vector3 screenPoint = HUD.Instance.PlayerCamera.GetComponent<Camera>().WorldToScreenPoint(targetBounds.center);
				Vector3 hudPos = HUD.Instance.hudCamera.ScreenToWorldPoint(screenPoint);
				targetHUDPos = hudPos;
			}
			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator losingLock()
	{
		isLosingLock = true;
		yield return new WaitForSeconds(loseLockDelay);
		loseLock();
	}

	private void OnEnterMGSBox()
	{
		loseLock();
		StopCoroutine("trackTarget");
	}

	private void OnExitMGSBox()
	{
		if (targettingSystem != null)
		{
			targettingSystem.SetActive(false);
		}
		if (aimer != null)
		{
			StartCoroutine("trackTarget");
		}
	}

	private void loseLock()
	{
		StopCoroutine("losingLock");
		StopCoroutine("beginLock");
		if (targettingSystem != null)
		{
			targettingSystem.SetActiveRecursively(false);
		}
		lockedTarget = null;
		currentTarget = null;
		isLosingLock = false;
		isLocking = false;
	}

	protected abstract IEnumerator beginLock();

	private void LateUpdate()
	{
		if (targettingSystem != null && currentTarget != null)
		{
			targettingSystemTransform.position = Vector3.Lerp(targettingSystemTransform.position, targetHUDPos, Time.deltaTime * trackingVisualSpeed);
			Vector3 position = targettingSystemTransform.position;
			position.z = 0f;
			targettingSystemTransform.position = position;
		}
	}

	private void OnDisable()
	{
		if (targettingSystem != null)
		{
			Object.Destroy(targettingSystem);
		}
	}

	private void OnOwnerDead()
	{
		if (targettingSystem != null)
		{
			Object.Destroy(targettingSystem);
		}
	}
}
