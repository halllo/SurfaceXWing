using Microsoft.Surface.Presentation.Controls;
using System.Linq;
using System.Windows.Media;

namespace SurfaceXWing.Test
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			Loaded += (s, e) =>
			{
				var scatterView = spielfeld.Children.OfType<ScatterView>().Single();

				scatterView.Items.Add(NewTestTag(tag: 10, color: Brushes.Red));
				scatterView.Items.Add(NewTestTag(tag: 11, color: Brushes.Green));
			};
		}

		private static ScatterViewItem NewTestTag(long tag, Brush color)
		{
			var testTag = new ScatterViewItem
			{
				Width = 100,
				Height = 100,
				Background = Brushes.Transparent,
				Tag = tag,
				Content = new TestTagVisual { Color = color },
			};

			var tagVisual = testTag.Content as TestTagVisual;
			tagVisual.Loaded += (s2, e2) =>
			{
				tagVisual.ViewModel.TagAvailable(new Microsoft.Surface.Presentation.Input.TagData(0, 0, 0, long.Parse(testTag.Tag.ToString())));
				tagVisual.ViewModel.TagUnavailable();
			};

			return testTag;
		}
	}
}
