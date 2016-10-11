using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SurfaceXWing.CompanionApp
{
	public class ImageOrText : ContentControl
	{
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			"ImageSource",
			typeof(ImageSource),
			typeof(ImageOrText),
			new PropertyMetadata(null, new PropertyChangedCallback(ImageSourcePropertyChanged))
		);
		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		private async static void ImageSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			(o as ImageOrText)?.TryShowImage(e.NewValue as BitmapImage);
		}


		public static readonly DependencyProperty ImagePlaceholderHeaderProperty = DependencyProperty.Register(
			"ImagePlaceholderHeader",
			typeof(string),
			typeof(ImageOrText),
			new PropertyMetadata(null, new PropertyChangedCallback(ImagePlaceholderHeaderPropertyChanged))
		);
		public string ImagePlaceholderHeader
		{
			get { return (string)GetValue(ImagePlaceholderHeaderProperty); }
			set { SetValue(ImagePlaceholderHeaderProperty, value); }
		}
		private async static void ImagePlaceholderHeaderPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{ }


		public static readonly DependencyProperty ImagePlaceholderProperty = DependencyProperty.Register(
			"ImagePlaceholder",
			typeof(string),
			typeof(ImageOrText),
			new PropertyMetadata(null, new PropertyChangedCallback(ImagePlaceholderPropertyChanged))
		);
		public string ImagePlaceholder
		{
			get { return (string)GetValue(ImagePlaceholderProperty); }
			set { SetValue(ImagePlaceholderProperty, value); }
		}
		private async static void ImagePlaceholderPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{ }







		private void TryShowImage(BitmapImage imageSource)
		{
			var image = new Image { Stretch = Stretch.None, Source = imageSource };
			image.ImageFailed += (s, a) => ShowPlaceholder();

			Content = image;
		}

		private void ShowPlaceholder()
		{
			var grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition());
			grid.RowDefinitions.Add(new RowDefinition());

			var header = new TextBlock { FontSize = 20, TextWrapping = TextWrapping.Wrap, Text = ImagePlaceholderHeader };
			Grid.SetRow(header, 0);
			grid.Children.Add(header);

			var placeholder = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = ImagePlaceholder };
			Grid.SetRow(placeholder, 1);
			grid.Children.Add(placeholder);

			Content = grid;
		}
	}
}
