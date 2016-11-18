using System;

namespace SurfaceXWing
{
	public partial class Spielfeld
	{
		Game _Spiel;
		RemoteGame _Remote;

		public Spielfeld()
		{
			InitializeComponent();

			SizeChanged += (s, e) => Setup();
			Loaded += (s, e) => Setup();

			_Remote = new RemoteGame();
			_Remote.ConnectToMBus();
		}

		private void Setup()
		{
			centerText.Text = "(" + ActualWidth + ", " + ActualHeight + ")";
			

			environmentBuilder.ViewModel.Setup(this, fieldsContainer);

			_Spiel = new Game(this, fieldsContainer);
			_Spiel.Start();

			TagManagement.Instance.Value.TagRegistered += tag => _Spiel.TagIntroduce(tag.Visual);
			TagManagement.Instance.Value.TagUnregistered += tag => _Spiel.TagDismiss(tag.Visual);

			var h = GameStarted;
			if (h != null) h(_Spiel);


			_Remote.Spiel = _Spiel;
			_Remote.FieldsContainer = fieldsContainer;
		}

		public event Action<Game> GameStarted;
	}
}
