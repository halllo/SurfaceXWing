using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Windows;
using System.Windows.Controls;

namespace SurfaceXWing.Test
{
	public class TestTagVisual : TagVisual
	{
		public TestTagVisual()
		{
			PlacedCheckBox = new SurfaceCheckBox
			{
				Content = "place",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			PlacedCheckBox.Checked += (s, e) =>
			{
				ViewModel.TagAvailable(new TagData(0, 0, 0, ViewModel.Id));
				elMenu.Visibility = Visibility.Visible;
			};
			PlacedCheckBox.Unchecked += (s, e) =>
			{
				ViewModel.TagUnavailable();
				elMenu.Visibility = Visibility.Collapsed;
			};

			elMenu.Visibility = Visibility.Collapsed;

			var grid = Content as Grid;
			grid.Children.Add(PlacedCheckBox);
		}

		public CheckBox PlacedCheckBox { get; private set; }

		public override Point Position { get { return ((ScatterViewItem)Parent).Center; } }
		public override double OrientationAngle { get { return ((ScatterViewItem)Parent).Orientation; } }

	}
}
