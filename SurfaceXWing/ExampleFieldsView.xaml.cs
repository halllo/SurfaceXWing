namespace SurfaceXWing
{
	public partial class ExampleFieldsView
	{
		public ExampleFieldsView()
		{
			InitializeComponent();

			Loaded += Setup;
			SizeChanged += Setup;
		}

		private void Setup(object sender, System.Windows.RoutedEventArgs e)
		{
			centerText.Text = "(" + ActualWidth + ", " + ActualHeight + ")";

			Register(fieldsContainer);
			Register(field1, field2, field3, field4);

			Track(scatterViewItem1, scatterViewItem2);

			TagManagement.Instance.Value.TagRegistered += tag => Track(tag.Visual);
			TagManagement.Instance.Value.TagUnregistered += tag => Untrack(tag.Visual);
		}
	}
}
