using SurfaceGameBasics;
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

		public void TagIntroduce(TagVisual visual)
		{
			_Spielfeld.Track(visual);
			visual.ViewModel.NewPosition = new Command(() =>
			{
				var neueSchiffsposition = NewField(
					position: TopRight(visual),
					orientation: visual.OrientationAngle,
					color: visual.ViewModel.TacticleColor
				);
				neueSchiffsposition.AllowedOccupantId = ((IFieldOccupant)visual).Id;
			});
		}

		public void TagDismiss(TagVisual visual)
		{
			_Spielfeld.Untrack(visual);
			visual.ViewModel.NewPosition = null;
		}

		private Vector TopRight(IFieldOccupant occupant)
		{
			return occupant.Position.AsVector() - new Vector(43, 43);
		}

		private Schiffsposition NewField(Vector position, double orientation, Brush color)
		{
			var schiffsposition = SchiffspositionFabrik.Neu(position, orientation, color);
			schiffsposition.Yielded += PrepareToMove;
			schiffsposition.Occupied += CancelMove;
			schiffsposition.SetValue(Canvas.ZIndexProperty, 1);

			_FieldsContainer.Children.Add(schiffsposition);
			_Spielfeld.Register((IField)schiffsposition);

			schiffsposition.Activate(
				onForget: RemoveField,
				onForward: Forward,
				onBarrelRoll: BarrelRoll,
				onSlide3: Slide3);

			return schiffsposition;
		}

		private void PrepareToMove(IField field, IFieldOccupant occupant)
		{
			Move<ForwardMove>((Schiffsposition)field, occupant);
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

		private void RemoveField(Schiffsposition schiffsposition)
		{
			CancelMove(schiffsposition);

			schiffsposition.Yielded -= PrepareToMove;
			schiffsposition.Occupied -= CancelMove;
			_FieldsContainer.Children.Remove(schiffsposition);
			_Spielfeld.Unregister(schiffsposition);
		}

		private void Forward(Schiffsposition schiffsposition)
		{
			var occupant = schiffsposition.LastOccupant;
			if (schiffsposition.Move != null)
			{
				CancelMove(schiffsposition, occupant);
			}
			Move<ForwardMove>(schiffsposition, occupant);
		}

		private void BarrelRoll(Schiffsposition schiffsposition)
		{
			var occupant = schiffsposition.LastOccupant;
			if (schiffsposition.Move != null)
			{
				CancelMove(schiffsposition, occupant);
			}
			Move<BarrelRollMove>(schiffsposition, occupant);
		}

		private void Slide3(Schiffsposition schiffsposition)
		{
			var occupant = schiffsposition.LastOccupant;
			if (schiffsposition.Move != null)
			{
				CancelMove(schiffsposition, occupant);
			}
			Move<Slide3Move>(schiffsposition, occupant);
		}

		private void Move<TMove>(Schiffsposition schiffsposition, IFieldOccupant occupant) where TMove : Move, new()
		{
			if (schiffsposition.Move == null)
			{
				var move = new TMove();
				move.Init(_Spielfeld, _FieldsContainer, schiffsposition, occupant, (von, nach) =>
				{
					von.Move = null;
					von.Yielded -= PrepareToMove;
					von.Occupied -= CancelMove;
					nach.Yielded += PrepareToMove;
					nach.Occupied += CancelMove;
					nach.Activate(
						onForget: RemoveField,
						onForward: Forward,
						onBarrelRoll: BarrelRoll,
						onSlide3: Slide3);

				});
				move.CreatePotenzielleZiele();
				schiffsposition.Move = move;
				schiffsposition.ViewModel.AllowCancel(() => CancelMove(schiffsposition, occupant));
			}
		}
	}

	public abstract class Move : IDisposable
	{
		FieldsView _Spielfeld;
		Canvas _FieldsContainer;
		Schiffsposition _Von;
		IFieldOccupant _Mover;

		List<Schiffsposition> _Ziele;
		Action<Schiffsposition, Schiffsposition> _Moved;

		public void Init(FieldsView spielfeld, Canvas fieldsContainer, Schiffsposition von, IFieldOccupant mover, Action<Schiffsposition, Schiffsposition> moved)
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
			CreatePotenzielleZiele(_Von, Enable);
		}

		protected abstract void CreatePotenzielleZiele(Schiffsposition von, Action<Schiffsposition> enable);

		protected abstract void MovedOrCanceled(Schiffsposition von);

		private void Enable(Schiffsposition potenziellesZiel)
		{
			potenziellesZiel.SetValue(Canvas.ZIndexProperty, 2);
			_Ziele.Add(potenziellesZiel);
			_FieldsContainer.Children.Add(potenziellesZiel);
			_Spielfeld.Register((IField)potenziellesZiel);

			potenziellesZiel.Occupied += ZielOccupied;
			potenziellesZiel.AllowedOccupantId = _Mover.Id;
		}

		private void ZielOccupied(IField field, IFieldOccupant occupant)
		{
			if (_Mover.Id == occupant.Id)
			{
				_FieldsContainer.Children.Remove(_Von);
				_Spielfeld.Unregister(_Von);

				var neueSchiffsposition = (Schiffsposition)field;
				neueSchiffsposition.SetValue(Canvas.ZIndexProperty, 1);

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

			MovedOrCanceled(_Von);

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
			schiffsposition.PositionAt(position);

			return schiffsposition;
		}
	}
}
