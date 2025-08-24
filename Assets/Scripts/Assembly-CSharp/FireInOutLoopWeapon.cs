using System.Collections;
using UnityEngine;

public class FireInOutLoopWeapon : WeaponBase
{
	protected static readonly string FIRE_IN = "_fireIn";

	protected static readonly string FIRE_LOOP = "_fireLoop";

	protected static readonly string FIRE_OUT = "_fireOut";

	protected AnimationState _fireIn;

	protected AnimationState _fireLoop;

	protected AnimationState _fireOut;

	protected string _fireInAnimName;

	protected string _fireLoopAnimName;

	protected string _fireOutAnimName;

	protected override void Start()
	{
		base.Start();
		_fireInAnimName = base.name + FIRE_IN;
		_fireLoopAnimName = base.name + FIRE_LOOP;
		_fireOutAnimName = base.name + FIRE_OUT;
	}

	public override bool OnAttack()
	{
		StartCoroutine(FireInOutLoopRoutine());
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		StartCoroutine(FireInOutLoopRoutine());
	}

	protected virtual IEnumerator FireInOutLoopRoutine()
	{
		_fireIn = base.playerController.BodyAnimator.Animator[_fireInAnimName];
		_fireLoop = base.playerController.BodyAnimator.Animator[_fireLoopAnimName];
		_fireOut = base.playerController.BodyAnimator.Animator[_fireOutAnimName];
		yield break;
	}
}
