using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SurfaceXWing
{
	public class FieldsView : Grid
	{
		FrameworkElement _fieldsContainer;
		ConcurrentDictionary<IField, FieldPosition> _fields = new ConcurrentDictionary<IField, FieldPosition>();
		ConcurrentDictionary<IFieldOccupant, byte> _occupants = new ConcurrentDictionary<IFieldOccupant, byte>();
		ConcurrentDictionary<IFieldOccupant, byte> _untrackedOccupants = new ConcurrentDictionary<IFieldOccupant, byte>();

		public FieldsView()
		{
			Loaded += StartBackgroundPositioning;
		}

		public void Register(FrameworkElement fieldsContainer)
		{
			_fieldsContainer = fieldsContainer;
		}

		public void Register(params IField[] fields)
		{
			foreach (var field in fields)
			{
				var globalPosition = GetCenter(field);
				var globalPositionDifferenceTolerance = field.Size.X / 2.0;

				_fields.TryAdd(field, new FieldPosition());

				var fieldPositioning = _fields[field];
				fieldPositioning.GlobalPosition = globalPosition;
				fieldPositioning.GlobalPositionDifferenceToleranceSquared = globalPositionDifferenceTolerance * globalPositionDifferenceTolerance;
			}
		}

		public void Unregister(params IField[] fields)
		{
			foreach (var field in fields)
			{
				FieldPosition value;
				_fields.TryRemove(field, out value);
			}
		}

		public void Track(params IFieldOccupant[] occupants)
		{
			foreach (var occupant in occupants)
			{
				_occupants.TryAdd(occupant, byte.MinValue);
			}
		}

		public void Untrack(params IFieldOccupant[] occupants)
		{
			foreach (var occupant in occupants)
			{
				var value = byte.MinValue;
				_occupants.TryRemove(occupant, out value);
				_untrackedOccupants.TryAdd(occupant, byte.MinValue);
			}
		}

		private void StartBackgroundPositioning(object sender, RoutedEventArgs e)
		{
			var timer = new DispatcherTimer(DispatcherPriority.Background);
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += (s, e2) =>
			{
				timer.Stop();
				foreach (var occupant in _occupants.Keys)
				{
					foreach (var field in _fields)
					{
						if (field.Value.Contains(occupant.Position.AsVector()))
						{
							if (!field.Key.IsOccupiedBy(occupant))
							{
								field.Key.Occupy(occupant);
							}
							else
							{
								field.Key.Stays(occupant);
							}
						}
						else
						{
							if (field.Key.IsOccupiedBy(occupant))
							{
								field.Key.Yield(occupant);
							}
						}
					}
				}
				foreach (var untrackedOccupant in _untrackedOccupants.Keys)
				{
					foreach (var field in _fields.Keys)
					{
						if (field.IsOccupiedBy(untrackedOccupant))
						{
							field.Yield(untrackedOccupant);
						}
						byte value;
						_untrackedOccupants.TryRemove(untrackedOccupant, out value);
					}
				}
				timer.Start();
			};
			timer.Start();
		}

		private Vector GetCenter(IField field)
		{
			var containerSize = new Vector(ActualWidth, ActualHeight);
			var fieldsContainerSize = new Vector(_fieldsContainer.ActualWidth, _fieldsContainer.ActualHeight);
			var fieldsContainerTopRight = (containerSize - fieldsContainerSize) / 2.0;

			var fieldTopRight = fieldsContainerTopRight + field.Position.AsVector();
			var fieldCenter = fieldTopRight + field.Size / 2.0;

			return fieldCenter;
		}
	}

	public static class VectorConverter
	{
		public static Vector AsVector(this Point point)
		{
			return new Vector(point.X, point.Y);
		}
	}

	public static class AngleCalculator
	{
		public static bool OrientatesTop(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.Between(leftBound: 360 - 45, rightBound: 360)
				|| occupantAngle.Between(leftBound: 0, rightBound: 0 + 45);
		}
		public static bool OrientatesBottom(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.Between(leftBound: 180 - 45, rightBound: 180 + 45);
		}
		public static bool OrientatesRight(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.Between(leftBound: 90 - 45, rightBound: 90 + 45);
		}
		public static bool OrientatesLeft(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.Between(leftBound: 270 - 45, rightBound: 270 + 45);
		}

		private static bool Between(this double angle, double leftBound, double rightBound)
		{
			angle = (angle + 360) % 360;
			return leftBound <= angle && angle < rightBound;
		}
	}

	public class FieldPosition
	{
		public Vector GlobalPosition { get; set; }
		public double GlobalPositionDifferenceToleranceSquared { get; set; }

		public bool Contains(Vector position)
		{
			var centerDifference = GlobalPosition - position;
			return centerDifference.LengthSquared < GlobalPositionDifferenceToleranceSquared;
		}
	}

	public interface IField
	{
		string Tag { get; set; }
		Point Position { get; }
		double OrientationAngle { get; }
		Vector Size { get; }

		bool IsOccupiedBy(IFieldOccupant occupant);

		void Occupy(IFieldOccupant occupant);
		event Action<IFieldOccupant> Occupied;
		void Stays(IFieldOccupant occupant);

		void Yield(IFieldOccupant occupant);
		event Action<IFieldOccupant> Yielded;
	}

	public interface IFieldOccupant
	{
		string Tag { get; set; }
		Point Position { get; }
		double OrientationAngle { get; }
	}
}
