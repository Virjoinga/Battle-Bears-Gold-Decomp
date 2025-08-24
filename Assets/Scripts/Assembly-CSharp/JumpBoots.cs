using UnityEngine;

public class JumpBoots : SpecialItem
{
	public float jumpPower = 600f;

	public AudioClip jumpSound;

	public override string IconTextureLocation
	{
		get
		{
			return "Textures/GUI/jumpboots";
		}
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("jumpPower", ref jumpPower, base.EquipmentNames);
		base.Configure(item);
	}

	protected override void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		base.OnActivate(p, isRemote, delay);
		myTransform.parent = playerController.transform;
		if (!isRemote)
		{
			Vector3 vector = new Vector3(p.Motor.movement.velocity.x, jumpPower, p.Motor.movement.velocity.z);
			playerController.OnJumpPad(vector);
			if (base.audio != null && jumpSound != null)
			{
				base.audio.PlayOneShot(jumpSound);
			}
		}
	}
}
