using System;
using System.Linq;
using System.Windows;
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
			_Remote.Spielfeld = this;
		}


		public void ZieleErfassen()
		{
			var andereSchiffspositionen = Fields.Cast<Schiffsposition>().Where(s => s.Opacity == 1.0).ToLookup(s => s.AllowedOccupantId);
			foreach (Schiffsposition schiffsposition in Fields)
			{
				schiffsposition.tokens.Canvas.Children.Clear();
				foreach (var zielerfassung in schiffsposition.ViewModel.Tokens.Zielerfassungen)
				{
					if (andereSchiffspositionen[zielerfassung].Any())
					{
						var ziel = andereSchiffspositionen[zielerfassung].First();
						var entfernungsVektor = ziel.Position.AsVector() - schiffsposition.Position.AsVector();
						var entfernungsUndRichtungsVektor = entfernungsVektor.Rotate(-schiffsposition.OrientationAngle);
						var zielerfassungsVektor = entfernungsUndRichtungsVektor.Enlarge(-61);

						var zielerfassungslinie = new ArrowLine
						{
							X1 = 43,
							Y1 = 43,
							X2 = zielerfassungsVektor.X + 43,
							Y2 = zielerfassungsVektor.Y + 43,
							Stroke = schiffsposition.ViewModel.Color,
							StrokeThickness = 1
						};
						schiffsposition.tokens.Canvas.Children.Add(zielerfassungslinie);
					}
				}
			}
		}
	}
}
