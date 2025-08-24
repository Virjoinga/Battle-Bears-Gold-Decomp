using System;

namespace TextFilter
{
	public interface ICommunityTextFilter
	{
		void FilterText(string text, Action<TextFilterResult> callback);
	}
}
