using UnityEngine;

public class PlayerAnimationTester : MonoBehaviour
{
	public string[] walkAnimations;

	public string[] armAnimations;

	private Animation myAnimator;

	public string walkAnimation;

	public string armAnimation;

	private int walkAnimationIndex;

	private int armAnimationIndex;

	private void Awake()
	{
		myAnimator = base.animation;
		for (int i = 0; i < walkAnimations.Length; i++)
		{
			myAnimator[walkAnimations[i]].layer = 1;
			myAnimator[walkAnimations[i]].wrapMode = WrapMode.Loop;
		}
		for (int j = 0; j < armAnimations.Length; j++)
		{
			myAnimator[armAnimations[j]].layer = 0;
			myAnimator[armAnimations[j]].wrapMode = WrapMode.Loop;
		}
		if (walkAnimation != string.Empty)
		{
			myAnimator[walkAnimation].layer = 1;
			myAnimator[walkAnimation].wrapMode = WrapMode.Loop;
			myAnimator.CrossFade(walkAnimation);
		}
		if (armAnimation != string.Empty)
		{
			myAnimator[armAnimation].layer = 0;
			myAnimator[armAnimation].wrapMode = WrapMode.Loop;
			myAnimator.CrossFade(armAnimation);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			walkAnimationIndex++;
			if (walkAnimationIndex >= walkAnimations.Length)
			{
				walkAnimationIndex = 0;
			}
			walkAnimation = walkAnimations[walkAnimationIndex];
			myAnimator.CrossFade(walkAnimations[walkAnimationIndex]);
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			armAnimationIndex++;
			if (armAnimationIndex >= armAnimations.Length)
			{
				armAnimationIndex = 0;
			}
			armAnimation = armAnimations[armAnimationIndex];
			myAnimator.CrossFade(armAnimations[armAnimationIndex]);
		}
	}
}
