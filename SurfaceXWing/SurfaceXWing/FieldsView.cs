﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SurfaceGameBasics
{
	public class FieldsView : Grid
	{
		FrameworkElement _fieldsContainer;
		ConcurrentDictionary<IField, FieldPosition> _fields = new ConcurrentDictionary<IField, FieldPosition>();
		ConcurrentDictionary<IFieldOccupant, byte> _occupants = new ConcurrentDictionary<IFieldOccupant, byte>();
		ConcurrentDictionary<IFieldOccupant, byte> _untrackedOccupants = new ConcurrentDictionary<IFieldOccupant, byte>();
		
		protected IEnumerable<IField> Fields { get { return _fields.Keys; } }
		protected IEnumerable<IFieldOccupant> Occupants { get { return _occupants.Keys; } }

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
				if (!_fields.ContainsKey(field))
				{
					field.PositionChanged += FieldPositionChanged;
					field.FieldsView = this;
				}

				_fields.TryAdd(field, new FieldPosition());

				FieldPositionChanged(field);
			}
		}

		public void Unregister(params IField[] fields)
		{
			foreach (var field in fields)
			{
				if (!_fields.ContainsKey(field))
				{
					field.PositionChanged -= FieldPositionChanged;
				}

				FieldPosition value;
				_fields.TryRemove(field, out value);
			}
		}

		private void FieldPositionChanged(IField field)
		{
			var globalPosition = GetCenter(field);
			var globalPositionDifferenceTolerance = field.Size.X / 2.0;

			var fieldPositioning = _fields[field];
			fieldPositioning.GlobalPosition = globalPosition;
			fieldPositioning.GlobalPositionDifferenceToleranceSquared = globalPositionDifferenceTolerance * globalPositionDifferenceTolerance;
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
								if (field.Key.CanOccupy(occupant))
								{
									field.Key.Occupy(occupant);
								}
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
					}
					byte value;
					_untrackedOccupants.TryRemove(untrackedOccupant, out value);
				}
				timer.Start();
			};
			timer.Start();
		}

		private Vector GetCenter(IField field)
		{
			var containerSize = new Vector(ActualWidth, ActualHeight);
			var fieldsContainerSize = new Vector(_fieldsContainer.ActualWidth, _fieldsContainer.ActualHeight);
			var fieldsContainerTopLeft = (containerSize - fieldsContainerSize) / 2.0;

			var fieldTopLeft = fieldsContainerTopLeft + field.Position.AsVector();
			var fieldCenter = fieldTopLeft + field.Size / 2.0;

			return fieldCenter;
		}
	}

	public static class VectorConverter
	{
		public static Vector AsVector(this Point point)
		{
			return new Vector(point.X, point.Y);
		}

		public static Point AsPoint(this Vector vector)
		{
			return new Point(vector.X, vector.Y);
		}

		public static Vector AsVector(this double angle)
		{
			var degrees = (angle + 360 - 90) % 360;
			var radians = degrees * Math.PI / 180.0;
			return new Vector(Math.Cos(radians), Math.Sin(radians));
		}

		public static Vector Rotate(this Vector vector, double angle)
		{
			var matrix = new Matrix();
			matrix.Rotate(angle);
			return matrix.Transform(vector);
		}

		public static Vector Enlarge(this Vector vector, double lengthAddition)
		{
			var newLength = vector.Length + lengthAddition;
			var copiedVector = new Vector(vector.X, vector.Y);
			copiedVector.Normalize();
			var newVector = copiedVector * newLength;
			return newVector;
		}
	}

	public static class AngleCalculator
	{
		public static bool OrientatesTop(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.BetweenAngle(leftBound: 360 - 45, rightBound: 360)
				|| occupantAngle.BetweenAngle(leftBound: 0, rightBound: 0 + 45);
		}
		public static bool OrientatesBottom(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.BetweenAngle(leftBound: 180 - 45, rightBound: 180 + 45);
		}
		public static bool OrientatesRight(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.BetweenAngle(leftBound: 90 - 45, rightBound: 90 + 45);
		}
		public static bool OrientatesLeft(this IFieldOccupant occupant, IField field = null)
		{
			var occupantAngle = occupant.OrientationAngle - (field != null ? field.OrientationAngle : 0);
			return occupantAngle.BetweenAngle(leftBound: 270 - 45, rightBound: 270 + 45);
		}

		private static bool BetweenAngle(this double angle, double leftBound, double rightBound)
		{
			angle = (angle + 360) % 360;
			return angle.Between(leftBound, rightBound);
		}

		public static bool Between(this double value, double leftBound, double rightBound)
		{
			return leftBound <= value && value < rightBound;
		}
		public static bool Between(this int value, int leftBound, int rightBound)
		{
			return leftBound <= value && value < rightBound;
		}
		public static bool Between(this long value, long leftBound, long rightBound)
		{
			return leftBound <= value && value < rightBound;
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
		string Id { get; set; }
		Point Position { get; }
		double OrientationAngle { get; }
		Vector Size { get; }

		event Action<IField> PositionChanged;

		bool IsOccupiedBy(IFieldOccupant occupant);
		bool CanOccupy(IFieldOccupant occupant);

		void Occupy(IFieldOccupant occupant);
		event Action<IField, IFieldOccupant> Occupied;
		void Stays(IFieldOccupant occupant);

		void Yield(IFieldOccupant occupant);
		event Action<IField, IFieldOccupant> Yielded;

		IFieldOccupant LastOccupant { get; }
		FieldsView FieldsView { set; }
	}

	public interface IFieldOccupant
	{
		string Id { get; set; }
		Point Position { get; }
		double OrientationAngle { get; }
	}
}
