using System;

namespace Prime31.MetroEssentials
{
	public static class Snap
	{
		public static void registerForSnapChanges(Action<bool> sizeChangedAction)
		{
		}

		public static void registerForSnapChanges(Action<bool> sizeChangedAction, bool attemptToAutomaticallyUnsnap)
		{
		}

		public static void deregisterAllSnapListeners()
		{
		}
	}
}
