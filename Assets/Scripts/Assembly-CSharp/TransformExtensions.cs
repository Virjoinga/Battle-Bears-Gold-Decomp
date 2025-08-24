using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
	public static void ResetLocally(this Transform trans)
	{
		trans.localPosition = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = Vector3.one;
	}

	public static void Empty(this Transform trans)
	{
		foreach (Transform tran in trans)
		{
			Object.Destroy(tran.gameObject);
		}
	}

	public static void CopyLocal(this Transform copyTo, Transform copyFrom)
	{
		copyTo.localPosition = copyFrom.localPosition;
		copyTo.localRotation = copyFrom.localRotation;
		copyTo.localScale = copyFrom.localScale;
	}

	public static Vector3 InheritedScale(this Transform trans)
	{
		Vector3 vector = new Vector3(1f, 1f, 1f);
		while (trans != null)
		{
			vector = Vector3.Scale(vector, trans.localScale);
			trans = trans.parent;
		}
		return vector;
	}

	public static Transform[] Children(this Transform trans)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < trans.childCount; i++)
		{
			list.Add(trans.GetChild(i));
		}
		return list.ToArray();
	}
}
