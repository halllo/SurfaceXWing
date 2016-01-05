using System;
using System.Collections.Generic;
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
		}

		public void TagDismiss(TagVisual visual)
		{
			_Spielfeld.Untrack(visual);
		}

		public void Start()
		{
			_Spielfeld.Register(_FieldsContainer);


			var schiffsposition1 = new Schiffsposition { ViewModel = { Text = "links", Color = Brushes.Green } };
			var schiffsposition1HeightHalbe = schiffsposition1.Height / 2;
			schiffsposition1.RenderTransform = new RotateTransform { CenterX = schiffsposition1HeightHalbe, CenterY = schiffsposition1HeightHalbe, Angle = 90 };
			schiffsposition1.SetValue(Canvas.LeftProperty, 0.0);
			schiffsposition1.SetValue(Canvas.TopProperty, _FieldsContainer.ActualHeight / 2.0 - schiffsposition1HeightHalbe);
			schiffsposition1.Yielded += YieldedToMove;
			schiffsposition1.Occupied += OccupiedToCancelMove;

			_FieldsContainer.Children.Add(schiffsposition1);
			_Spielfeld.Register((IField)schiffsposition1);


			var schiffsposition2 = new Schiffsposition { ViewModel = { Text = "rechts", Color = Brushes.Red } };
			var schiffsposition2HeightHalbe = schiffsposition2.Height / 2;
			schiffsposition2.RenderTransform = new RotateTransform { CenterX = schiffsposition2HeightHalbe, CenterY = schiffsposition2HeightHalbe, Angle = 270 };
			schiffsposition2.SetValue(Canvas.LeftProperty, _FieldsContainer.ActualWidth - schiffsposition2.Height);
			schiffsposition2.SetValue(Canvas.TopProperty, _FieldsContainer.ActualHeight / 2.0 - schiffsposition2HeightHalbe);
			schiffsposition2.Yielded += YieldedToMove;
			schiffsposition2.Occupied += OccupiedToCancelMove;

			_FieldsContainer.Children.Add(schiffsposition2);
			_Spielfeld.Register((IField)schiffsposition2);
		}

		private void YieldedToMove(IField field, IFieldOccupant occupant)
		{
			var schiffsposition = (Schiffsposition)field;
			if (schiffsposition.Move == null)
			{
				var move = new Move(_Spielfeld, _FieldsContainer, schiffsposition, occupant, moved: (von, nach) =>
				{
					von.Move = null;
					von.Yielded -= YieldedToMove;
					von.Occupied -= OccupiedToCancelMove;
					nach.Yielded += YieldedToMove;
					nach.Occupied += OccupiedToCancelMove;
				});
				move.CreatePotenzielleZiele();
				schiffsposition.Move = move;
			}
		}

		private void OccupiedToCancelMove(IField field, IFieldOccupant occupant)
		{
			var schiffsposition = (Schiffsposition)field;
			if (schiffsposition.Move != null && schiffsposition.Move.Mover.Id == occupant.Id)
			{
				schiffsposition.Move.Dispose();
				schiffsposition.Move = null;
			}
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
				var ziel1 = new Schiffsposition { Opacity = 0.3, ViewModel = { Text = _Von.ViewModel.Text, Color = _Von.ViewModel.Color } };

				var ziel1HeightHalbe = ziel1.Height / 2;
				ziel1.RenderTransform = new RotateTransform { CenterX = ziel1HeightHalbe, CenterY = ziel1HeightHalbe, Angle = 90 };

				var ziel1Position = _Von.Position.AsVector();

				ziel1.SetValue(Canvas.LeftProperty, ziel1Position.X + 150);
				ziel1.SetValue(Canvas.TopProperty, ziel1Position.Y);

				Enable(ziel1);
			}

			{
				var ziel2 = new Schiffsposition { Opacity = 0.3, ViewModel = { Text = _Von.ViewModel.Text, Color = _Von.ViewModel.Color } };

				var ziel2HeightHalbe = ziel2.Height / 2;
				ziel2.RenderTransform = new RotateTransform { CenterX = ziel2HeightHalbe, CenterY = ziel2HeightHalbe, Angle = 90 };

				var ziel2Position = _Von.Position.AsVector();

				ziel2.SetValue(Canvas.LeftProperty, ziel2Position.X + 150);
				ziel2.SetValue(Canvas.TopProperty, ziel2Position.Y + 100);

				Enable(ziel2);
			}

			{
				var ziel3 = new Schiffsposition { Opacity = 0.3, ViewModel = { Text = _Von.ViewModel.Text, Color = _Von.ViewModel.Color } };

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
				neueSchiffsposition.Opacity = 1.0;

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
