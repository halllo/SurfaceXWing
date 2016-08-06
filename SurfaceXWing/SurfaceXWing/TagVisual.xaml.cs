using Microsoft.Surface.Presentation.Input;
using SurfaceGameBasics;
using System;
using System.Windows;
using System.Windows.Media;

namespace SurfaceXWing
{
	public partial class TagVisual : IFieldOccupant
	{
		public virtual Point Position { get { return Center; } }
		public virtual double OrientationAngle { get { return Orientation; } }

		string IFieldOccupant.Id
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
			NotifyChanged("TacticleColor");

			TagManagement.Instance.Value.Register(Id, this);
		}

		internal void TagUnavailable()
		{
			TagManagement.Instance.Value.Unregister(Id, this);
		}


		public long Id { get; private set; }
		public TagVisual Visual { get; set; }

		Command _NewPosition;
		public Command NewPosition
		{
			get { return _NewPosition; }
			set { _NewPosition = value; NotifyChanged("NewPosition"); }
		}

		public Brush TacticleColor
		{
			get
			{
				if (Id.Between(50, 100)) return Brushes.Green;
				if (Id.Between(100, 150)) return Brushes.Red;
				return Brushes.Blue;
			}
		}
	}
}