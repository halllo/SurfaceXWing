using System;

namespace SurfaceXWing
{
	public partial class Spielfeld
	{
		Game _Spiel;

		public Spielfeld()
		{
			InitializeComponent();

			Loaded += Setup;
			SizeChanged += Setup;
		}

		private void Setup(object sender, System.Windows.RoutedEventArgs e)
		{
			centerText.Text = "(" + ActualWidth + ", " + ActualHeight + ")";

			_Spiel = new Game(this, fieldsContainer);
			_Spiel.Start();

			TagManagement.Instance.Value.TagRegistered += tag => _Spiel.TagIntroduce(tag.Visual);
			TagManagement.Instance.Value.TagUnregistered += tag => _Spiel.TagDismiss(tag.Visual);

			var h = GameStarted;
			if (h != null) h(_Spiel);
		}

		public event Action<Game> GameStarted;
	}
}
