using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfaceXWing
{
	public partial class Schiffsposition : IField
	{
		ConcurrentDictionary<IFieldOccupant, byte> _fieldOccupants = new ConcurrentDictionary<IFieldOccupant, byte>();

		public Schiffsposition()
		{
			InitializeComponent();

			HideArrows();
		}

		private void HideArrows()
		{
			topArrow.Visibility = Visibility.Collapsed;
			bottomArrow.Visibility = Visibility.Collapsed;
			leftArrow.Visibility = Visibility.Collapsed;
			rightArrow.Visibility = Visibility.Collapsed;
		}

		public string Text
		{
			get { return FieldText.Text; }
			set { FieldText.Text = value; }
		}

		public string StateText
		{
			get { return FieldStateText.Text; }
			set { FieldStateText.Text = value; }
		}

		public Brush Color
		{
			get { return Background; }
			set
			{
				Background = value;
				rangeIndicator.Stroke = value;
			}
		}




		string IField.Tag
		{
			get { return Tag.ToString(); }
			set { Tag = value; }
		}
		public Point Position { get { return new Point((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty)); } }
		public double OrientationAngle { get { return RenderTransform is RotateTransform ? ((RotateTransform)RenderTransform).Angle : 0; } }
		public Vector Size { get { return new Vector(Width, Height); } }

		public bool IsOccupiedBy(IFieldOccupant occupant)
		{
			return _fieldOccupants.ContainsKey(occupant);
		}

		public void Occupy(IFieldOccupant occupant)
		{
			_fieldOccupants.TryAdd(occupant, byte.MinValue);
			UpdateState();

			var h = Occupied;
			if (h != null) h(occupant);
		}
		public event Action<IFieldOccupant> Occupied;
		public void Stays(IFieldOccupant occupant)
		{
			UpdateState();
		}

		public void Yield(IFieldOccupant occupant)
		{
			var value = byte.MinValue;
			_fieldOccupants.TryRemove(occupant, out value);
			UpdateState();

			var h = Yielded;
			if (h != null) h(occupant);
		}
		public event Action<IFieldOccupant> Yielded;


		private void UpdateState()
		{
			HideArrows();

			if (_fieldOccupants.Any())
			{
				foreach (var occupant in _fieldOccupants.Keys)
				{
					if (occupant.OrientatesTop(this)) topArrow.Visibility = Visibility.Visible;
					if (occupant.OrientatesBottom(this)) bottomArrow.Visibility = Visibility.Visible;
					if (occupant.OrientatesRight(this)) rightArrow.Visibility = Visibility.Visible;
					if (occupant.OrientatesLeft(this)) leftArrow.Visibility = Visibility.Visible;
				}
			}
			else
			{
			}

			StateText = _fieldOccupants.Count + " occupants";
		}
	}
}
