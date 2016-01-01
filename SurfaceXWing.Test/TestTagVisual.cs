using Microsoft.Surface.Presentation.Input;
using System.Windows;
using System.Windows.Controls;

namespace SurfaceXWing.Test
{
	public class TestTagVisual : TagVisual
	{
		public TestTagVisual()
		{
			PlacedCheckBox = new CheckBox
			{
				Content = "place",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			PlacedCheckBox.Checked += (s, e) => ViewModel.TagAvailable(new TagData(0, 0, 0, ViewModel.Id));
			PlacedCheckBox.Unchecked += (s, e) => ViewModel.TagUnavailable();

			var grid = Content as Grid;
			grid.Children.Add(PlacedCheckBox);
		}

		public CheckBox PlacedCheckBox { get; private set; }

		public override Point Position { get { return ((Microsoft.Surface.Presentation.Controls.ScatterViewItem)Parent).Center; } }
		public override double OrientationAngle { get { return ((Microsoft.Surface.Presentation.Controls.ScatterViewItem)Parent).Orientation; } }

	}
}
