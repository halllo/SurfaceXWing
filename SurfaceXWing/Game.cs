using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfaceXWing
{
	public class Game
	{
		FieldsView _Spielfeld;
		Canvas _FieldsContainer;

		public Game(FieldsView spielfeld, Canvas fieldsContainer)
		{
			_Spielfeld = spielfeld;
			_FieldsContainer = fieldsContainer;
		}

		public void TagIntroduce(TagVisual visual)
		{
			_Spielfeld.Track(visual);
			visual.ViewModel.NewPosition = new Command(() => NewField(
				position: TopRight(visual),
				orientation: visual.OrientationAngle,
				color: visual.ViewModel.TacticleColor
			));
		}

		public void TagDismiss(TagVisual visual)
		{
			_Spielfeld.Untrack(visual);
			visual.ViewModel.NewPosition = null;
		}

		public void Start()
		{
			var alteFelder = _FieldsContainer.Children.OfType<Schiffsposition>().ToList();
			foreach (var altesFeld in alteFelder)
			{
				_FieldsContainer.Children.Remove(altesFeld);
				_Spielfeld.Unregister(altesFeld);
			}

			_Spielfeld.Register(_FieldsContainer);
		}

		private void PrepareToMove(IField field, IFieldOccupant occupant)
		{
			var schiffsposition = (Schiffsposition)field;
			if (schiffsposition.Move == null)
			{
				var move = new Move(_Spielfeld, _FieldsContainer, schiffsposition, occupant, moved: (von, nach) =>
				{
					von.Move = null;
					von.Yielded -= PrepareToMove;
					von.Occupied -= CancelMove;
					nach.Yielded += PrepareToMove;
					nach.Occupied += CancelMove;
					nach.Activate(onForget: RemoveField);
				});
				move.CreatePotenzielleZiele();
				schiffsposition.Move = move;
				schiffsposition.ViewModel.AllowCancel(() => CancelMove(schiffsposition, occupant));
			}
		}

		private void CancelMove(IField field, IFieldOccupant occupant = null)
		{
			var schiffsposition = (Schiffsposition)field;
			if (schiffsposition.Move != null)
			{
				if (occupant == null || schiffsposition.Move.Mover.Id == occupant.Id)
				{
					schiffsposition.Move.Dispose();
					schiffsposition.Move = null;
					schiffsposition.ViewModel.CancelCancel();
				}
			}
		}

		private void NewField(Vector position, double orientation, Brush color)
		{
			var schiffsposition = SchiffspositionFabrik.Neu(position, orientation, color);
			schiffsposition.Yielded += PrepareToMove;
			schiffsposition.Occupied += CancelMove;

			_FieldsContainer.Children.Add(schiffsposition);
			_Spielfeld.Register((IField)schiffsposition);

			schiffsposition.Activate(onForget: RemoveField);
		}

		private Vector TopRight(IFieldOccupant occupant)
		{
			return occupant.Position.AsVector() - new Vector(45.5, 45);
		}

		private void RemoveField(Schiffsposition field)
		{
			CancelMove(field);

			field.Yielded -= PrepareToMove;
			field.Occupied -= CancelMove;
			_FieldsContainer.Children.Remove(field);
			_Spielfeld.Unregister(field);
		}
	}

	public class Move : IDisposable
	{
		FieldsView _Spielfeld;
		Canvas _FieldsContainer;
		Schiffsposition _Von;
		IFieldOccupant _Mover;

		List<Schiffsposition> _Ziele;
		Action<Schiffsposition, Schiffsposition> _Moved;

		public Move(FieldsView spielfeld, Canvas fieldsContainer, Schiffsposition von, IFieldOccupant mover, Action<Schiffsposition, Schiffsposition> moved)
		{
			_Spielfeld = spielfeld;
			_FieldsContainer = fieldsContainer;
			_Von = von;
			_Mover = mover;

			_Ziele = new List<Schiffsposition>();
			_Moved = moved;
		}

		public IFieldOccupant Mover { get { return _Mover; } }

		public void CreatePotenzielleZiele()
		{
			var angle = _Von.OrientationAngle;
			var position = _Von.Position.AsVector();

			var gradeaus = angle.AsVector();
			var links = (angle - 90).AsVector();
			var rechts = (angle + 90).AsVector();


			Enable(SchiffspositionFabrik.Neu(//1gradeaus
				position: position + (gradeaus * 180),
				orientation: angle,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "1"));

			Enable(SchiffspositionFabrik.Neu(//1scharflinks
				position: position + (gradeaus * 135) + (links * 135),
				orientation: angle - 90,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "1"));

			Enable(SchiffspositionFabrik.Neu(//1leichtlinks
				position: position + (gradeaus * 200) + (links * 80),
				orientation: angle - 45,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "1"));

			Enable(SchiffspositionFabrik.Neu(//1scharfrechts
				position: position + (gradeaus * 135) + (rechts * 135),
				orientation: angle + 90,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "1"));

			Enable(SchiffspositionFabrik.Neu(//1leichtrechts
				position: position + (gradeaus * 200) + (rechts * 80),
				orientation: angle + 45,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "1"));


			Enable(SchiffspositionFabrik.Neu(//2gradeaus
				position: position + (gradeaus * 270),
				orientation: angle,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "2"));

			Enable(SchiffspositionFabrik.Neu(//2scharflinks
				position: position + (gradeaus * 200) + (links * 200),
				orientation: angle - 90,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "2"));

			Enable(SchiffspositionFabrik.Neu(//2leichtlinks
				position: position + (gradeaus * 290) + (links * 120),
				orientation: angle - 45,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "2"));

			Enable(SchiffspositionFabrik.Neu(//2scharfrechts
				position: position + (gradeaus * 200) + (rechts * 200),
				orientation: angle + 90,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "2"));

			Enable(SchiffspositionFabrik.Neu(//2leichtrechts
				position: position + (gradeaus * 290) + (rechts * 120),
				orientation: angle + 45,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "2"));


			Enable(SchiffspositionFabrik.Neu(//3gradeaus
				position: position + (gradeaus * 360),
				orientation: angle,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "3"));

			Enable(SchiffspositionFabrik.Neu(//3scharflinks
				position: position + (gradeaus * 265) + (links * 265),
				orientation: angle - 90,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "3"));

			Enable(SchiffspositionFabrik.Neu(//3leichtlinks
				position: position + (gradeaus * 365) + (links * 150),
				orientation: angle - 45,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "3"));

			Enable(SchiffspositionFabrik.Neu(//3scharfrechts
				position: position + (gradeaus * 265) + (rechts * 265),
				orientation: angle + 90,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "3"));

			Enable(SchiffspositionFabrik.Neu(//3lechtrechts
				position: position + (gradeaus * 365) + (rechts * 150),
				orientation: angle + 45,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "3"));


			Enable(SchiffspositionFabrik.Neu(//4gradeaus
				position: position + (gradeaus * 450),
				orientation: angle,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "4"));

			Enable(SchiffspositionFabrik.Neu(//5gradeaus
				position: position + (gradeaus * 540),
				orientation: angle,
				color: _Von.ViewModel.Color, opacity: 0.4, label: "5"));


			_Von.ViewModel.FluglinienVisible = true;
		}

		private void Enable(Schiffsposition potenziellesZiel)
		{
			_Ziele.Add(potenziellesZiel);
			_FieldsContainer.Children.Add(potenziellesZiel);
			_Spielfeld.Register((IField)potenziellesZiel);

			potenziellesZiel.Occupied += ZielOccupied;
		}

		private void ZielOccupied(IField field, IFieldOccupant occupant)
		{
			if (_Mover.Id == occupant.Id)
			{
				_FieldsContainer.Children.Remove(_Von);
				_Spielfeld.Unregister(_Von);

				var neueSchiffsposition = (Schiffsposition)field;

				var von = _Von;
				var moved = _Moved;

				Dispose(except: neueSchiffsposition);

				if (occupant.OrientatesBottom(neueSchiffsposition))
					neueSchiffsposition.OrientationAngle = (neueSchiffsposition.OrientationAngle + 180) % 360;

				if (occupant.OrientatesLeft(neueSchiffsposition))
					neueSchiffsposition.OrientationAngle = (neueSchiffsposition.OrientationAngle + 270) % 360;

				if (occupant.OrientatesRight(neueSchiffsposition))
					neueSchiffsposition.OrientationAngle = (neueSchiffsposition.OrientationAngle + 90) % 360;

				neueSchiffsposition.ViewModel.LetztePosition = new SchiffspositionModel.Position(von.Position, von.OrientationAngle);
				neueSchiffsposition.ViewModel.Label = null;

				moved(von, neueSchiffsposition);
			}
		}

		public void Dispose()
		{
			Dispose(null);
		}

		bool disposed;
		private void Dispose(Schiffsposition except)
		{
			if (disposed) throw new ObjectDisposedException("Move");
			disposed = true;

			foreach (var ziel in _Ziele)
			{
				ziel.Occupied -= ZielOccupied;
				if (ziel != except)
				{
					_FieldsContainer.Children.Remove(ziel);
					_Spielfeld.Unregister(ziel);
				}
			}
			_Ziele.Clear();

			_Von.ViewModel.FluglinienVisible = false;

			_Spielfeld = null;
			_FieldsContainer = null;
			_Von = null;
			_Mover = null;
			_Ziele = null;
			_Moved = null;
		}
	}

	public static class SchiffspositionFabrik
	{
		public static Schiffsposition Neu(Vector position, double orientation, Brush color, double opacity = 1.0, string label = null)
		{
			var schiffsposition = new Schiffsposition { ViewModel = { Color = color, Label = label }, Opacity = opacity };
			var schiffspositionHeightHalbe = schiffsposition.Height / 2;
			schiffsposition.RenderTransform = new RotateTransform { CenterX = schiffspositionHeightHalbe, CenterY = schiffspositionHeightHalbe, Angle = orientation };
			schiffsposition.SetValue(Canvas.LeftProperty, position.X);
			schiffsposition.SetValue(Canvas.TopProperty, position.Y);

			return schiffsposition;
		}
	}
}
