using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace SurfaceXWing
{
	public class TagManagement
	{
		public class Data
		{
			public SchiffTokens Tokens { get; set; }
		}


		public readonly static Lazy<TagManagement> Instance = new Lazy<TagManagement>(() => new TagManagement());
		public Dictionary<long, Data> Tags = new Dictionary<long, Data>();


		public event Action<TagVisualModel> TagRegistered;
		private void RaiseTagRegistered(TagVisualModel tag)
		{
			var h = TagRegistered;
			if (h != null) h(tag);
		}
		public void Register(long tag, TagVisualModel viewModel)
		{
			if (!Tags.ContainsKey(tag))
				Tags.Add(tag, new Data { Tokens = new SchiffTokens(tag.ToString()) });

			viewModel.Tokens = Tags[tag].Tokens;

			RaiseTagRegistered(viewModel);
		}


		public event Action<TagVisualModel> TagUnregistered;
		private void RaiseTagUnregistered(TagVisualModel tag)
		{
			var h = TagUnregistered;
			if (h != null) h(tag);
		}
		public void Unregister(long tag, TagVisualModel viewModel)
		{
			RaiseTagUnregistered(viewModel);
		}
	}
}
