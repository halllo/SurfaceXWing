using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using SurfaceGameBasics;

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


		public event Action<Game> GameStarted;
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


			_Remote.FieldsContainer = fieldsContainer;
		}


		private void ZielerfassungenAktualisieren(object sender, System.Windows.RoutedEventArgs e)
		{
			ZieleErfassen();
		}
		public void ZieleErfassen()
		{
			var fieldsById = Fields.Cast<Schiffsposition>().ToLookup(f => f.AllowedOccupantId);
			foreach (var field in fieldsById.Select(group => group.First()))
			{
				field.tokens.Canvas.Children.Clear();
				foreach (var zielerfassung in field.ViewModel.Tokens.Zielerfassungen)
				{
					if (fieldsById[zielerfassung].Any())
					{
						var ziel = fieldsById[zielerfassung].First();
						var entfernungsVektor = ziel.Position.AsVector() - field.Position.AsVector();
						var entfernungsUndRichtungsVektor = entfernungsVektor.Rotate(-field.OrientationAngle);
						var zielerfassungsVektor = entfernungsUndRichtungsVektor.Enlarge(-61);

						var zielerfassungslinie = new ArrowLine
						{
							X1 = 43,
							Y1 = 43,
							X2 = zielerfassungsVektor.X + 43,
							Y2 = zielerfassungsVektor.Y + 43,
							Stroke = field.ViewModel.Color,
							StrokeThickness = 1
						};
						field.tokens.Canvas.Children.Add(zielerfassungslinie);
					}
				}
			}
		}
	}
}
