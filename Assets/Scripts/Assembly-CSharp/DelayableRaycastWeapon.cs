using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class DelayableRaycastWeapon : RaycastWeapon
{
	public float delayTime;

	[SerializeField]
	private Transform _wallCheckStart;

	[SerializeField]
	private LayerMask _wallCheckMask;

	[SerializeField]
	private float _wallCheckDistance;

	[SerializeField]
	private GameObject _attackSpawnPointEffect;

	public override bool OnAttack()
	{
		SpawnAttackOnSpawnPoint();
		StartCoroutine("DelayedAttackRoutine");
		if (base.NetSyncReporter != null && !dontSendNetworkMessages)
		{
			base.NetSyncReporter.SpawnProjectile(base.gameObject.transform.position, base.gameObject.transform.forward);
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 dir, int delay)
	{
		SpawnAttackOnSpawnPoint();
		StartCoroutine("DelayedAttackRoutine");
	}

	public void DoRemoteFire(Vector3 hitPosition)
	{
		DoRaycastAttack(hitPosition);
	}

	protected override Vector3 DoRaycastAttack(Vector3 hitPosition)
	{
		Vector3 vector = base.DoRaycastAttack(hitPosition);
		if (!isRemote)
		{
			SendDelayedFireMessage(vector);
		}
		return vector;
	}

	private void SendDelayedFireMessage(Vector3 hitPos)
	{
		if (base.NetSyncReporter != null && !dontSendNetworkMessages)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = base.OwnerID;
			hashtable[(byte)1] = hitPos.x;
			hashtable[(byte)2] = hitPos.y;
			hashtable[(byte)3] = hitPos.z;
			base.NetSyncReporter.SetAction(56, hashtable);
		}
	}

	private IEnumerator DelayedAttackRoutine()
	{
		PlayAttackAnimation(0f, 1f);
		base.playerController.BodyAnimator.OnAttack();
		if (!isRemote)
		{
			base.playerController.canSwitchWeapons = false;
		}
		yield return new WaitForSeconds(delayTime);
		if (!base.playerController.IsDead && base.playerController.WeaponManager.CurrentWeapon == this && !isRemote)
		{
			Vector3 spawnPoint = Vector3.zero;
			RaycastHit hit;
			if (Physics.Raycast(_wallCheckStart.position, aimer.forward, out hit, _wallCheckDistance, _wallCheckMask))
			{
				spawnPoint = hit.point;
			}
			if (myAudio != null && fireSounds.Length > 0)
			{
				myAudio.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length)], SoundManager.Instance.getEffectsVolume());
			}
			DoRaycastAttack(spawnPoint);
		}
		if (!isRemote)
		{
			yield return new WaitForSeconds(firingTime - delayTime);
			base.playerController.canSwitchWeapons = true;
		}
	}

	private void SpawnAttackOnSpawnPoint()
	{
		Transform transform = base.transform.Find("spawn");
		if (transform != null)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(_attackSpawnPointEffect, transform.position, transform.rotation);
			gameObject.transform.parent = base.transform;
		}
	}
}
