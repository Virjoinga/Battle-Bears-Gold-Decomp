using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public interface ISkyVuNetworkProxy
	{
		void CallService(Services service, Action<string> successCallback, Action<string> failureCallback);

		void CallService<T>(Services service, T entity, Action<string> successCallback, Action<string> failureCallback) where T : BaseEntity;

		T GetEntity<T>(string json) where T : BaseEntity, new();

		List<T> GetEntities<T>(string json) where T : BaseEntity, new();
	}
}
