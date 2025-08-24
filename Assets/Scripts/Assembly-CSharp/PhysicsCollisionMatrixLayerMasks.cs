using System.Collections.Generic;
using UnityEngine;

public static class PhysicsCollisionMatrixLayerMasks
{
	private static Dictionary<int, int> _masksByLayer;

	public static void Init()
	{
		_masksByLayer = new Dictionary<int, int>();
		for (int i = 0; i < 32; i++)
		{
			int num = 0;
			for (int j = 0; j < 32; j++)
			{
				if (!Physics.GetIgnoreLayerCollision(i, j))
				{
					num |= 1 << (j & 0x1F);
				}
			}
			_masksByLayer.Add(i, num);
		}
	}

	public static int MaskForLayer(int layer)
	{
		if (_masksByLayer == null)
		{
			Init();
		}
		return _masksByLayer[layer];
	}
}
