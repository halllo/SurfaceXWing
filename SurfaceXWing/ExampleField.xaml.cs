using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfaceXWing
{
	public partial class ExampleField : UserControl, IField
	{
		ConcurrentDictionary<IFieldOccupant, byte> _fieldOccupants = new ConcurrentDictionary<IFieldOccupant, byte>();

		public ExampleField()
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




		string IField.Tag
		{
			get { return Tag.ToString(); }
			set { Tag = value; }
		}
		public Point Position { get { return new Point((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty)); } }
		public double OrientationAngle { get { return RenderTransform is RotateTransform ? ((RotateTransform)RenderTransform).Angle : 0; } }
		public Vector Size { get { return new Vector(ActualWidth, ActualHeight); } }
		public ReadOnlyCollection<IFieldOccupant> Occupants { get { return _fieldOccupants.Keys.ToList().AsReadOnly(); } }


		public void Occupy(IFieldOccupant occupant)
		{
			_fieldOccupants.TryAdd(occupant, byte.MinValue);
			UpdateState();
			RaiseOccupied(occupant);
		}
		public event Action<IFieldOccupant> Occupied;
		private void RaiseOccupied(IFieldOccupant occupant)
		{
			var h = Occupied;
			if (h != null) h(occupant);
		}

		public void Yield(IFieldOccupant occupant)
		{
			var value = byte.MinValue;
			_fieldOccupants.TryRemove(occupant, out value);
			UpdateState();
			RaiseYielded(occupant);
		}
		public event Action<IFieldOccupant> Yielded;
		private void RaiseYielded(IFieldOccupant occupant)
		{
			var h = Yielded;
			if (h != null) h(occupant);
		}


		private void UpdateState()
		{
			HideArrows();

			if (_fieldOccupants.Any())
			{
				Border.BorderBrush = Brushes.Green;

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
				Border.BorderBrush = Brushes.Red;
			}

			StateText = _fieldOccupants.Count + " occupants";
		}
	}
}
