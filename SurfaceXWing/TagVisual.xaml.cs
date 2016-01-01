using Microsoft.Surface.Presentation.Input;
using System;
using System.Windows;

namespace SurfaceXWing
{
	public partial class TagVisual : IFieldOccupant
	{
		public virtual Point Position { get { return Center; } }
		public virtual double OrientationAngle { get { return Orientation; } }

		string IFieldOccupant.Tag
		{
			get { return ViewModel.Id.ToString(); }
			set { throw new NotSupportedException(); }
		}

		public TagVisualModel ViewModel { get; private set; }


		public TagVisual()
		{
			InitializeComponent();

			DataContext = ViewModel = new TagVisualModel { Visual = this };

			Loaded += (s, e) => ViewModel.TagAvailable(VisualizedTag);
			Unloaded += (s, e) => ViewModel.TagUnavailable();
		}
	}

	public class TagVisualModel : ViewModel
	{
		public void TagAvailable(TagData tag)
		{
			Id = tag.Value;
			NotifyChanged("Id");

			TagManagement.Instance.Value.Register(Id, this);
		}

		internal void TagUnavailable()
		{
			TagManagement.Instance.Value.Unregister(Id, this);
		}


		public long Id { get; private set; }
		public TagVisual Visual { get; set; }
	}
}
