using UnityEngine;

public class Minigun : RaycastWeapon
{
	public float fireRotationSpeed = 1000f;

	public float speedupSpeed = 30f;

	public float slowdownSpeedPerSec = 200f;

	private float currentRotationSpeed;

	private Transform barrelRotator;

	private AudioSource barrelLoop;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("fireRotationSpeed", ref fireRotationSpeed, base.EquipmentNames);
		item.UpdateProperty("spinDownSpeed", ref slowdownSpeedPerSec, base.EquipmentNames);
		item.UpdateProperty("spinUpSpeed", ref speedupSpeed, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Awake()
	{
		base.Awake();
		barrelRotator = myTransform.Find("barrel_grp");
		barrelLoop = barrelRotator.audio;
	}

	protected override void Start()
	{
		base.Start();
	}

	public override bool OnAttack()
	{
		if (currentRotationSpeed == 0f && !barrelLoop.isPlaying)
		{
			barrelLoop.Play();
		}
		currentRotationSpeed += speedupSpeed;
		if (currentRotationSpeed >= fireRotationSpeed)
		{
			currentRotationSpeed = fireRotationSpeed;
			base.OnAttack();
			return true;
		}
		return false;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		if (currentRotationSpeed == 0f && !barrelLoop.isPlaying)
		{
			barrelLoop.Play();
		}
		currentRotationSpeed += speedupSpeed;
		if (currentRotationSpeed >= fireRotationSpeed)
		{
			currentRotationSpeed = fireRotationSpeed;
			base.OnRemoteAttack(pos, vel, delay);
		}
	}

	public override void OnReload()
	{
		base.OnReload();
		currentRotationSpeed = 0f;
		barrelLoop.Stop();
	}

	private void LateUpdate()
	{
		Vector3 localEulerAngles = barrelRotator.transform.localEulerAngles;
		localEulerAngles.z += currentRotationSpeed * Time.deltaTime;
		barrelRotator.transform.localEulerAngles = localEulerAngles;
		currentRotationSpeed -= slowdownSpeedPerSec * Time.deltaTime;
		if (currentRotationSpeed < 0f)
		{
			currentRotationSpeed = 0f;
			barrelLoop.Stop();
		}
		else
		{
			barrelLoop.pitch = currentRotationSpeed / fireRotationSpeed * 1.25f + 0.25f;
		}
	}
}
