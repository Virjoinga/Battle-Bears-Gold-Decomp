using System.Collections.Generic;
using UnityEngine;

public class AnimatedScroller : MonoBehaviour
{
	public Collider myCollider;

	public GUIButton mainButton;

	public List<GUIButton> buttons = new List<GUIButton>();

	public GUIButton linkedButton;

	public float scrollSpeed = 7f;

	private float targetAnimationTime;

	private float currentAnimationTime;

	public Camera guiCamera;

	public bool isHorizontal = true;

	public float scrollInputMultiplier = 0.1f;

	private float timeInterval;

	public string scrollerName = "chooseGame";

	public int currentIndex;

	public bool didScroll;

	protected float defaultWidth = 320f;

	protected float defaultHeight = 480f;

	protected List<GUIButton> buttonList = new List<GUIButton>();

	public List<Transform> slots = new List<Transform>();

	public Animation myAnimation;

	private GameObject animatorObject;

	public bool buttonsCanSelect = true;

	public int numberShowing = -1;

	public int[] linkButtonDisables;

	private float _epsilon = 0.05f;

	private float _startingTime = -1f;

	public virtual void Awake()
	{
		if (myAnimation == null)
		{
			myAnimation = base.GetComponent<Animation>();
		}
		animatorObject = myAnimation.gameObject;
		if (myCollider == null)
		{
			myCollider = base.GetComponent<Collider>();
		}
		for (int i = 0; i < buttons.Count; i++)
		{
			buttonList.Add(buttons[i]);
			buttons[i].scroller = this;
		}
		recalculate();
		if (linkedButton != null)
		{
			linkedButton.name = scrollerName + currentIndex;
		}
		if (mainButton != null)
		{
			mainButton.gameObject.SetActive(true);
		}
	}

	public virtual void addButton(GUIButton b)
	{
		b.transform.parent = slots[buttonList.Count];
		b.transform.localPosition = Vector3.zero;
		b.transform.localEulerAngles = Vector3.zero;
		if (!buttons.Contains(b))
		{
			buttons.Add(b);
		}
		buttonList.Add(b);
		b.scroller = this;
		recalculate();
	}

	public void OnSetIndex(int index)
	{
		currentIndex = index;
		timeInterval = myAnimation["slide"].length / (float)(slots.Count + 1);
		targetAnimationTime = timeInterval * (float)(currentIndex + 1);
		if (linkedButton != null)
		{
			linkedButton.name = scrollerName + currentIndex;
		}
	}

	protected virtual void recalculate()
	{
		currentIndex = 0;
		timeInterval = myAnimation["slide"].length / (float)(slots.Count + 1);
		targetAnimationTime = timeInterval * (float)(currentIndex + 1);
	}

	public virtual void OnReset()
	{
		buttonList.Clear();
		recalculate();
	}

	public void clearButtons()
	{
		for (int i = 0; i < buttonList.Count; i++)
		{
			buttonList[i].OnButtonDepressed();
		}
	}

	public virtual void OnGUIButtonClicked(GUIButton b)
	{
		if (Mathf.Abs(targetAnimationTime - timeInterval * (float)(currentIndex + 1)) > 0.05f)
		{
			return;
		}
		if (b.name == "upArrow")
		{
			if (currentIndex >= 0)
			{
				currentIndex--;
				targetAnimationTime = timeInterval * (float)(currentIndex + 1);
			}
			return;
		}
		if (b.name == "downArrow")
		{
			if (currentIndex < buttonList.Count)
			{
				currentIndex++;
				targetAnimationTime = timeInterval * (float)(currentIndex + 1);
			}
			return;
		}
		if (b.name == "mainButton")
		{
			b.name = scrollerName + currentIndex;
			SendMessageUpwards("OnGUIButtonClicked", b, SendMessageOptions.DontRequireReceiver);
			b.name = "mainButton";
			return;
		}
		int num = -1;
		for (int i = 0; i < buttonList.Count; i++)
		{
			if (b == buttonList[i])
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			if (buttonsCanSelect)
			{
				currentIndex = num;
				targetAnimationTime = timeInterval * (float)(num + 1);
			}
			if (linkedButton != null)
			{
				linkedButton.name = scrollerName + currentIndex;
			}
			if (mainButton != null)
			{
				mainButton.gameObject.SetActive(true);
			}
		}
	}

	private void LateUpdate()
	{
		if (Mathf.Abs(currentAnimationTime - targetAnimationTime) > float.Epsilon)
		{
			currentAnimationTime = Mathf.Lerp(currentAnimationTime, targetAnimationTime, scrollSpeed * Time.deltaTime);
			myAnimation["slide"].clip.SampleAnimation(animatorObject, currentAnimationTime);
		}
	}

	public virtual void Update()
	{
		if (Input.touchCount > 0)
		{
			if (_startingTime == -1f)
			{
				_startingTime = targetAnimationTime;
			}
			for (int i = 0; i < Input.touchCount; i++)
			{
				Ray ray = guiCamera.ScreenPointToRay(Input.GetTouch(i).position);
				RaycastHit hitInfo;
				if (myCollider.Raycast(ray, out hitInfo, 500f))
				{
					float deltaTime = Input.GetTouch(i).deltaTime;
					if (deltaTime <= 0f)
					{
						deltaTime = 1f;
					}
					if (isHorizontal)
					{
						if (Mathf.Abs(Input.GetTouch(i).deltaPosition.x) > 5f && mainButton != null)
						{
							mainButton.gameObject.SetActive(false);
						}
						targetAnimationTime -= FixTouchDelta(Input.GetTouch(i)).x * Time.deltaTime * (defaultWidth / (float)Screen.currentResolution.width) * 0.25f;
					}
					else
					{
						if (Mathf.Abs(Input.GetTouch(i).deltaPosition.y) > 5f && mainButton != null)
						{
							mainButton.gameObject.SetActive(false);
						}
						float num = FixTouchDelta(Input.GetTouch(i)).y * Time.deltaTime * (defaultWidth / (float)Screen.currentResolution.height) * 0.25f;
						targetAnimationTime += num;
						if (Mathf.Abs(_startingTime - targetAnimationTime) > _epsilon)
						{
							didScroll = true;
						}
					}
					if (targetAnimationTime < 0f)
					{
						targetAnimationTime = 0f;
					}
					if (targetAnimationTime > timeInterval * (float)(buttonList.Count + 1))
					{
						targetAnimationTime = timeInterval * (float)(buttonList.Count + 1);
					}
				}
				else
				{
					snapToButton();
				}
			}
		}
		else
		{
			didScroll = false;
			_startingTime = -1f;
			snapToButton();
		}
	}

	protected virtual void snapToButton()
	{
		int num = 0;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < buttonList.Count && (numberShowing == -1 || buttonList.Count - numberShowing >= i); i++)
		{
			float num3 = Mathf.Abs(targetAnimationTime - timeInterval * (float)(i + 1));
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		currentIndex = num;
		targetAnimationTime = (float)(currentIndex + 1) * timeInterval;
		if (linkedButton != null)
		{
			linkedButton.name = scrollerName + currentIndex;
			linkedButton.enable();
			for (int j = 0; j < linkButtonDisables.Length; j++)
			{
				if (currentIndex == linkButtonDisables[j])
				{
					linkedButton.disable();
					break;
				}
			}
		}
		if (mainButton != null)
		{
			mainButton.gameObject.SetActive(true);
		}
	}

	public static Vector2 FixTouchDelta(Touch aT)
	{
		float num = Time.deltaTime / aT.deltaTime;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		return aT.deltaPosition * num;
	}
}
