using System;
using System.Windows.Media;

namespace SurfaceXWing
{
	public class TagManagement
	{
		public class Data
		{
			public string Name { get; set; }
			public Brush Color { get; set; }
		}


		public readonly static Lazy<TagManagement> Instance = new Lazy<TagManagement>(() => new TagManagement());


		public event Action<TagVisualModel> TagRegistered;
		private void RaiseTagRegistered(TagVisualModel tag)
		{
			var h = TagRegistered;
			if (h != null) h(tag);
		}
		public void Register(long tag, TagVisualModel viewModel)
		{
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
