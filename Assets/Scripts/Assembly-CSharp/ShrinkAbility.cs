using System.Collections;
using UnityEngine;

public class ShrinkAbility : SpecialAbility
{
	private const float SECONDS_BETWEEN_RESIZE_ITERATIONS = 0.02f;

	public float minScale = 0.2f;

	protected float _maxScale = 1f;

	private Transform myTransform;

	public float growSpeed = 0.05f;

	public float shrinkSpeed = 0.05f;

	protected CharacterController character;

	protected DamageReceiver _damageReceiver;

	protected CapsuleCollider capsuleCollider;

	private float originalLegSpeed;

	protected IEnumerator _beginningResizeRoutine;

	protected IEnumerator _endingResizeRoutine;

	protected override void Awake()
	{
		base.Awake();
		myTransform = base.transform;
		SetupBeginningAndEndingRoutines();
	}

	protected virtual void SetupBeginningAndEndingRoutines()
	{
		_beginningResizeRoutine = shrink();
		_endingResizeRoutine = grow();
	}

	protected override void Start()
	{
		base.Start();
		SetupPlayerComponents();
		StartCoroutine(BeginningResizeRoutine());
	}

	protected void SetupPlayerComponents()
	{
		character = myTransform.GetComponent(typeof(CharacterController)) as CharacterController;
		if (character == null)
		{
			capsuleCollider = myTransform.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
		}
		_damageReceiver = myTransform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
	}

	public void OnDisable()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			StartCoroutine(EndingResizeRoutine());
		}
	}

	protected void OnDestroy()
	{
		StopAllCoroutines();
		Vector3 position = myTransform.position;
		if (character != null)
		{
			position.y += character.height;
		}
		else
		{
			position.y += capsuleCollider.height;
		}
		myTransform.localScale = Vector3.one;
		if (_damageReceiver != null)
		{
			_damageReceiver.isShrunk = false;
		}
	}

	private IEnumerator BeginningResizeRoutine()
	{
		while (_beginningResizeRoutine.MoveNext())
		{
			yield return _beginningResizeRoutine.Current;
		}
	}

	private IEnumerator EndingResizeRoutine()
	{
		while (_endingResizeRoutine.MoveNext())
		{
			yield return _endingResizeRoutine.Current;
		}
		Object.Destroy(this);
	}

	protected IEnumerator grow()
	{
		float targetScale = myTransform.localScale.x;
		while (targetScale < _maxScale)
		{
			targetScale += growSpeed;
			myTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
			Vector3 pos = myTransform.position;
			float colliderWidth2 = 0f;
			if (character != null)
			{
				pos.y += character.height * growSpeed;
				colliderWidth2 = character.radius;
			}
			else
			{
				pos.y += capsuleCollider.height * growSpeed;
				colliderWidth2 = capsuleCollider.radius;
			}
			int numRays = 16;
			Vector3 originalRotation = myTransform.eulerAngles;
			LayerMask layer = 1 << LayerMask.NameToLayer("Wall");
			for (int i = 0; i < numRays; i++)
			{
				Ray ray = new Ray(myTransform.position, myTransform.forward);
				if (Physics.Raycast(ray, colliderWidth2, layer))
				{
					pos -= myTransform.forward * colliderWidth2 * (targetScale + 0.2f);
				}
				myTransform.RotateAround(Vector3.up, 360f / (float)numRays);
			}
			myTransform.eulerAngles = originalRotation;
			myTransform.position = pos;
			yield return new WaitForSeconds(0.02f);
		}
		myTransform.localScale = new Vector3(_maxScale, _maxScale, _maxScale);
		if (_damageReceiver != null)
		{
			_damageReceiver.isShrunk = _maxScale < 1f;
		}
	}

	protected IEnumerator shrink()
	{
		if (_damageReceiver != null)
		{
			_damageReceiver.isShrunk = minScale < 1f;
		}
		float targetScale = myTransform.localScale.x;
		while (targetScale > minScale)
		{
			targetScale -= shrinkSpeed;
			myTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
			KeepCharacterFromFallingThroughFloorIfStationary();
			yield return new WaitForSeconds(0.02f);
		}
		KeepCharacterFromFallingThroughFloorIfStationary();
		myTransform.localScale = new Vector3(minScale, minScale, minScale);
	}

	private void KeepCharacterFromFallingThroughFloorIfStationary()
	{
		myTransform.position += new Vector3(0f, 5f, 0f);
	}
}
