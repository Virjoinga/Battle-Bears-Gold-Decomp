using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageAnimator : MonoBehaviour
{
	[Serializable]
	public class ImageAnimation
	{
		public string name;

		public int startIndex;

		public int endIndex;

		public float fps = 20f;

		public bool reverse;

		public bool loop;

		public bool pingpong;

		public int[] skipFrames;
	}

	public ImageAnimation[] imageAnimations;

	public Dictionary<string, ImageAnimation> _animations = new Dictionary<string, ImageAnimation>();

	public int totalAnimations = 59;

	public float xTiles = 8f;

	public float yTiles = 8f;

	private float currentAnimationFrame;

	public string currentAnimationName;

	private ImageAnimation currentAnimation;

	public bool isPlaying;

	private Renderer ourRenderer;

	private float lastUpdateTime;

	private void OnStartAtRandomIndex()
	{
		currentAnimationFrame = UnityEngine.Random.Range(0, totalAnimations);
	}

	private void Start()
	{
		ourRenderer = base.renderer;
		ourRenderer.material.mainTextureScale = new Vector2(1f / xTiles, 1f / yTiles);
		for (int i = 0; i < imageAnimations.Length; i++)
		{
			_animations.Add(imageAnimations[i].name, imageAnimations[i]);
		}
		if (currentAnimationName != string.Empty)
		{
			OnPlayAnimation(currentAnimationName);
		}
	}

	private void OnDisableLoopMode(string animationName)
	{
		if (_animations.ContainsKey(animationName))
		{
			_animations[animationName].loop = false;
		}
	}

	public void OnPlayAnimation(string animationName)
	{
		if (_animations.ContainsKey(animationName))
		{
			currentAnimation = _animations[animationName];
			if (currentAnimation.reverse)
			{
				currentAnimationFrame = currentAnimation.endIndex;
			}
			else
			{
				currentAnimationFrame = currentAnimation.startIndex;
			}
			isPlaying = true;
			ourRenderer.material.SetTextureOffset("_MainTex", new Vector2(currentAnimationFrame / xTiles, 1f - (float)((int)(currentAnimationFrame / xTiles) + 1) * (1f / yTiles)));
		}
	}

	private void LateUpdate()
	{
		if (currentAnimation == null || (!isPlaying && currentAnimation.fps > 0f) || !(Time.time > lastUpdateTime + 1f / currentAnimation.fps))
		{
			return;
		}
		lastUpdateTime = Time.time;
		if (currentAnimation.reverse)
		{
			currentAnimationFrame -= 1f;
			while (ShouldSkipFrame(currentAnimationFrame))
			{
				currentAnimationFrame -= 1f;
			}
			if (currentAnimationFrame < (float)currentAnimation.startIndex)
			{
				if (currentAnimation.loop)
				{
					currentAnimationFrame = currentAnimation.endIndex;
				}
				else if (currentAnimation.pingpong)
				{
					currentAnimationFrame = 0f;
					currentAnimation.reverse = !currentAnimation.reverse;
				}
				else
				{
					isPlaying = false;
				}
			}
		}
		else
		{
			currentAnimationFrame += 1f;
			while (ShouldSkipFrame(currentAnimationFrame))
			{
				currentAnimationFrame += 1f;
			}
			if (currentAnimationFrame > (float)currentAnimation.endIndex)
			{
				if (currentAnimation.loop)
				{
					currentAnimationFrame = currentAnimation.startIndex;
				}
				else if (currentAnimation.pingpong)
				{
					currentAnimationFrame = currentAnimation.endIndex;
					currentAnimation.reverse = !currentAnimation.reverse;
				}
				else
				{
					isPlaying = false;
				}
			}
		}
		float num = currentAnimationFrame / xTiles;
		float num2 = 1f - (float)((int)(currentAnimationFrame / xTiles) + 1) * (1f / yTiles);
		ourRenderer.material.SetTextureOffset("_MainTex", new Vector2(num % 1f, num2 % 1f));
	}

	public bool ShouldSkipFrame(float currentFrame)
	{
		for (int i = 0; i < currentAnimation.skipFrames.Length; i++)
		{
			if (currentFrame == (float)currentAnimation.skipFrames[i])
			{
				return true;
			}
		}
		return false;
	}

	public float GetCurrentAnimationLength()
	{
		float result = 0f;
		if (currentAnimation != null)
		{
			int num = currentAnimation.endIndex - currentAnimation.startIndex;
			result = (float)num / currentAnimation.fps;
		}
		return result;
	}
}
