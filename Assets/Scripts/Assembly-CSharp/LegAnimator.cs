using System.Collections;
using UnityEngine;

public class LegAnimator : MonoBehaviour
{
	private Animation myAnimator;

	private Transform playerTransform;

	private Vector3 lastPos;

	public float animateCheckInterval = 0.1f;

	private string lastAnimation = string.Empty;

	private BodyAnimatorBase bodyAnimator;

	public float animationSpeedDivider = 12f;

	public bool isDisabled;

	private CharacterMotor motor;

	private void Awake()
	{
		myAnimator = base.GetComponent<Animation>();
		playerTransform = base.transform.parent;
		myAnimator["legs_left"].layer = 1;
		myAnimator["legs_right"].layer = 1;
		myAnimator["legs_forward"].layer = 1;
		myAnimator["legs_back"].layer = 1;
		myAnimator["legs_idle"].layer = 1;
		myAnimator["legs_left"].wrapMode = WrapMode.Loop;
		myAnimator["legs_right"].wrapMode = WrapMode.Loop;
		myAnimator["legs_forward"].wrapMode = WrapMode.Loop;
		myAnimator["legs_back"].wrapMode = WrapMode.Loop;
		myAnimator["legs_idle"].wrapMode = WrapMode.Loop;
		bodyAnimator = GetComponent(typeof(BodyAnimatorBase)) as BodyAnimatorBase;
		motor = playerTransform.GetComponent(typeof(CharacterMotor)) as CharacterMotor;
	}

	public void Start()
	{
		StartCoroutine(animateLegsChecker());
	}

	public void OnReset()
	{
		lastAnimation = string.Empty;
		StopAllCoroutines();
		myAnimator.CrossFade("legs_idle");
		StartCoroutine(animateLegsChecker());
	}

	public IEnumerator DisableLegAnimationsForDuration(float duration)
	{
		StopAllCoroutines();
		myAnimator.Stop();
		yield return new WaitForSeconds(duration);
		OnReset();
	}

	private IEnumerator animateLegsChecker()
	{
		lastPos = playerTransform.position;
		while (true)
		{
			if (!isDisabled)
			{
				animateLegs(playerTransform.position - lastPos);
			}
			lastPos = playerTransform.position;
			yield return new WaitForSeconds(animateCheckInterval);
		}
	}

	public void animateLegs(Vector3 movement)
	{
		movement.y = 0f;
		float magnitude = movement.magnitude;
		if (magnitude < 0.25f)
		{
			movement = Vector3.zero;
		}
		if (!motor.IsGrounded())
		{
			if (lastAnimation != "legs_idle")
			{
				myAnimator.CrossFade("legs_idle");
				lastAnimation = "legs_idle";
			}
		}
		else if (Mathf.Abs(movement.x) > 0.01f || Mathf.Abs(movement.z) > 0.01f)
		{
			if (lastAnimation == "legs_idle")
			{
				bodyAnimator.updateIdleState(true);
			}
			float num = Vector3.Angle(movement, playerTransform.forward);
			string empty = string.Empty;
			if (num < 45f)
			{
				empty = "legs_forward";
			}
			else if (num > 135f)
			{
				empty = "legs_back";
			}
			else
			{
				float num2 = Vector3.Angle(movement, playerTransform.right);
				empty = ((!(num2 < 90f)) ? "legs_left" : "legs_right");
			}
			myAnimator[empty].speed = magnitude / animationSpeedDivider;
			myAnimator.CrossFade(empty);
			lastAnimation = empty;
		}
		else if (lastAnimation != "legs_idle")
		{
			bodyAnimator.updateIdleState(false);
			myAnimator.CrossFade("legs_idle");
			lastAnimation = "legs_idle";
		}
	}
}
