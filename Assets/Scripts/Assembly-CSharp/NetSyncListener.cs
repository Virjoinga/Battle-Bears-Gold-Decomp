using System.Collections.Generic;
using UnityEngine;

public class NetSyncListener : MonoBehaviour
{
	private GameObject target;

	private NetSyncObject nsoTarget;

	private Queue<TransformPackage> transformQueue = new Queue<TransformPackage>();

	private Queue<StatePackage> stateQueue = new Queue<StatePackage>();

	private Queue<ActionPackage> actionQueue = new Queue<ActionPackage>();

	private Queue<FireProjectilePackage> fireProjectileQueue = new Queue<FireProjectilePackage>();

	private TransformPackage currentPackage;

	private TransformPackage nextPackage;

	private TransformPackage lastTransformPackage;

	private Transform angleTransform;

	private Transform targetTransform;

	public void SetTarget(GameObject gameObject, NetSyncObject nso)
	{
		target = gameObject;
		targetTransform = target.transform;
		nsoTarget = nso;
		angleTransform = (target.GetComponentInChildren(typeof(AngleControllerPlaceholder)) as AngleControllerPlaceholder).transform;
	}

	public void Update()
	{
		int serverTimeInMilliseconds = PhotonManager.Instance.ServerTimeInMilliseconds;
		UpdateTransform(serverTimeInMilliseconds);
		UpdateState(serverTimeInMilliseconds);
		UpdateAction(serverTimeInMilliseconds);
		UpdateFireProjectile(serverTimeInMilliseconds);
	}

	private void UpdateTransform(int delayedTime)
	{
		if (!nsoTarget.IsStatic)
		{
			if (currentPackage == null && transformQueue.Count > 0)
			{
				currentPackage = transformQueue.Dequeue();
			}
			if (currentPackage != null && nextPackage == null && transformQueue.Count > 0)
			{
				nextPackage = transformQueue.Dequeue();
			}
			if (currentPackage != null && nextPackage != null)
			{
				while (delayedTime - nextPackage.timestamp >= 0 && transformQueue.Count > 0)
				{
					currentPackage = nextPackage;
					nextPackage = transformQueue.Dequeue();
				}
				float num = nextPackage.timestamp - currentPackage.timestamp;
				float num2 = (float)(delayedTime - currentPackage.timestamp) / num;
				if (num == 0f)
				{
					num2 = 1f;
				}
				if (num2 < 0f)
				{
					Debug.LogError("ERROR: Negative interpolation");
					targetTransform.position = currentPackage.position;
					Vector3 eulerAngles = targetTransform.eulerAngles;
					eulerAngles.y = currentPackage.baseYrotation;
					targetTransform.eulerAngles = eulerAngles;
					Vector3 localEulerAngles = angleTransform.localEulerAngles;
					localEulerAngles.x = currentPackage.angleXRotation;
					angleTransform.localEulerAngles = localEulerAngles;
				}
				else if (num2 <= 1f)
				{
					targetTransform.position = Vector3.Slerp(currentPackage.position, nextPackage.position, num2);
					Vector3 eulerAngles2 = targetTransform.eulerAngles;
					eulerAngles2.y = Mathf.LerpAngle(currentPackage.baseYrotation, nextPackage.baseYrotation, num2);
					targetTransform.eulerAngles = eulerAngles2;
					Vector3 localEulerAngles2 = angleTransform.localEulerAngles;
					localEulerAngles2.x = Mathf.LerpAngle(currentPackage.angleXRotation, nextPackage.angleXRotation, num2);
					angleTransform.localEulerAngles = localEulerAngles2;
				}
				else
				{
					targetTransform.position = Vector3.Slerp(targetTransform.position, ExtrapolatePosition(Mathf.Min(num2, 2f)), Time.deltaTime * 5f);
					Vector3 eulerAngles3 = targetTransform.eulerAngles;
					eulerAngles3.y = nextPackage.baseYrotation;
					targetTransform.eulerAngles = eulerAngles3;
					Vector3 localEulerAngles3 = angleTransform.localEulerAngles;
					localEulerAngles3.x = nextPackage.angleXRotation;
					angleTransform.localEulerAngles = localEulerAngles3;
				}
			}
			else if (currentPackage != null)
			{
				targetTransform.position = currentPackage.position;
				Vector3 eulerAngles4 = targetTransform.eulerAngles;
				eulerAngles4.y = currentPackage.baseYrotation;
				targetTransform.eulerAngles = eulerAngles4;
				Vector3 localEulerAngles4 = angleTransform.localEulerAngles;
				localEulerAngles4.x = currentPackage.angleXRotation;
				angleTransform.localEulerAngles = localEulerAngles4;
			}
		}
		else if (transformQueue.Count > 0)
		{
			TransformPackage transformPackage = null;
			while (transformQueue.Count > 0)
			{
				transformPackage = transformQueue.Dequeue();
			}
			targetTransform.position = transformPackage.position;
			Vector3 eulerAngles5 = targetTransform.eulerAngles;
			eulerAngles5.y = transformPackage.baseYrotation;
			targetTransform.eulerAngles = eulerAngles5;
			Vector3 localEulerAngles5 = angleTransform.localEulerAngles;
			localEulerAngles5.x = transformPackage.angleXRotation;
			angleTransform.localEulerAngles = localEulerAngles5;
		}
	}

	private void UpdateState(int delayedTime)
	{
		while (stateQueue.Count > 0 && delayedTime - stateQueue.Peek().timestamp >= 0)
		{
			StatePackage statePackage = stateQueue.Dequeue();
			nsoTarget.HandleStateChange(statePackage.state);
		}
	}

	private void UpdateAction(int delayedTime)
	{
		while (actionQueue.Count > 0 && delayedTime - actionQueue.Peek().timestamp >= 0)
		{
			ActionPackage actionPackage = actionQueue.Dequeue();
			nsoTarget.HandleActionChange(actionPackage.action, actionPackage.parameters, Mathf.Abs(PhotonManager.Instance.ServerTimeInMilliseconds - actionPackage.timestamp));
		}
	}

	private void UpdateFireProjectile(int delayedTime)
	{
		while (fireProjectileQueue.Count > 0 && delayedTime - fireProjectileQueue.Peek().timestamp >= 0)
		{
			FireProjectilePackage fireProjectilePackage = fireProjectileQueue.Dequeue();
			nsoTarget.HandleFireProjectile(fireProjectilePackage.position, fireProjectilePackage.velocity, Mathf.Abs(PhotonManager.Instance.ServerTimeInMilliseconds - fireProjectilePackage.timestamp));
		}
	}

	private Vector3 ExtrapolatePosition(float interpFactor)
	{
		Vector3 vector = nextPackage.position - currentPackage.position;
		return (interpFactor - 1f) * vector + nextPackage.position;
	}

	public void AddTransformPackage(TransformPackage package)
	{
		int num = 5;
		if (transformQueue.Count > 0 && lastTransformPackage != null)
		{
			Vector3 vector = package.position - lastTransformPackage.position;
			int num2 = package.timestamp - lastTransformPackage.timestamp;
			float num3 = package.angleXRotation - lastTransformPackage.angleXRotation;
			float num4 = package.baseYrotation - lastTransformPackage.baseYrotation;
			vector /= (float)num;
			num2 /= num;
			num3 /= (float)num;
			num4 /= (float)num;
			for (int i = 1; i < num; i++)
			{
				TransformPackage transformPackage = new TransformPackage();
				transformPackage.angleXRotation = num3 * (float)i + lastTransformPackage.angleXRotation;
				transformPackage.baseYrotation = num4 * (float)i + lastTransformPackage.baseYrotation;
				transformPackage.timestamp = num2 * i + lastTransformPackage.timestamp;
				transformPackage.position = vector * i + lastTransformPackage.position;
				transformQueue.Enqueue(transformPackage);
			}
			transformQueue.Enqueue(package);
		}
		else
		{
			transformQueue.Enqueue(package);
		}
		lastTransformPackage = package;
	}

	public void AddStatePackage(StatePackage package)
	{
		stateQueue.Enqueue(package);
	}

	public void AddActionPackage(ActionPackage package)
	{
		actionQueue.Enqueue(package);
	}

	public void AddFireProjectilePackage(FireProjectilePackage package)
	{
		fireProjectileQueue.Enqueue(package);
	}
}
