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

			NewField(
				position: new Vector(0.0, _FieldsContainer.ActualHeight / 2.0 - 45),
				orientation: 90.0,
				color: Brushes.Green);
			NewField(
				position: new Vector(_FieldsContainer.ActualWidth - 90, _FieldsContainer.ActualHeight / 2.0 - 45),
				orientation: 270.0,
				color: Brushes.Red);
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
			var schiffsposition = new Schiffsposition { ViewModel = { Color = color } };
			var schiffspositionHeightHalbe = schiffsposition.Height / 2;
			schiffsposition.RenderTransform = new RotateTransform { CenterX = schiffspositionHeightHalbe, CenterY = schiffspositionHeightHalbe, Angle = orientation };
			schiffsposition.SetValue(Canvas.LeftProperty, position.X);
			schiffsposition.SetValue(Canvas.TopProperty, position.Y);
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
			{
				var ziel1 = new Schiffsposition { Opacity = 0.5, ViewModel = { Color = _Von.ViewModel.Color } };

				var ziel1HeightHalbe = ziel1.Height / 2;
				ziel1.RenderTransform = new RotateTransform { CenterX = ziel1HeightHalbe, CenterY = ziel1HeightHalbe, Angle = 90 };

				var ziel1Position = _Von.Position.AsVector();

				ziel1.SetValue(Canvas.LeftProperty, ziel1Position.X + 150);
				ziel1.SetValue(Canvas.TopProperty, ziel1Position.Y);

				Enable(ziel1);
			}

			{
				var ziel2 = new Schiffsposition { Opacity = 0.5, ViewModel = { Color = _Von.ViewModel.Color } };

				var ziel2HeightHalbe = ziel2.Height / 2;
				ziel2.RenderTransform = new RotateTransform { CenterX = ziel2HeightHalbe, CenterY = ziel2HeightHalbe, Angle = 90 };

				var ziel2Position = _Von.Position.AsVector();

				ziel2.SetValue(Canvas.LeftProperty, ziel2Position.X + 150);
				ziel2.SetValue(Canvas.TopProperty, ziel2Position.Y + 100);

				Enable(ziel2);
			}

			{
				var ziel3 = new Schiffsposition { Opacity = 0.5, ViewModel = { Color = _Von.ViewModel.Color } };

				var ziel3HeightHalbe = ziel3.Height / 2;
				ziel3.RenderTransform = new RotateTransform { CenterX = ziel3HeightHalbe, CenterY = ziel3HeightHalbe, Angle = 90 };

				var ziel3Position = _Von.Position.AsVector();

				ziel3.SetValue(Canvas.LeftProperty, ziel3Position.X + 150);
				ziel3.SetValue(Canvas.TopProperty, ziel3Position.Y - 100);

				Enable(ziel3);
			}
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

			_Spielfeld = null;
			_FieldsContainer = null;
			_Von = null;
			_Mover = null;
			_Ziele = null;
			_Moved = null;
		}
	}
}
