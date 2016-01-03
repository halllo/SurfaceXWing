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
		public SchiffspositionModel ViewModel { get; private set; }

		public Schiffsposition()
		{
			InitializeComponent();

			DataContext = ViewModel = new SchiffspositionModel();

			ViewModel.HideArrows();
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
			return ViewModel.FieldOccupants.ContainsKey(occupant);
		}

		public void Occupy(IFieldOccupant occupant)
		{
			ViewModel.FieldOccupants.TryAdd(occupant, byte.MinValue);
			ViewModel.UpdateState(this);

			var h = Occupied;
			if (h != null) h(occupant);
		}
		public event Action<IFieldOccupant> Occupied;
		public void Stays(IFieldOccupant occupant)
		{
			ViewModel.UpdateState(this);
		}

		public void Yield(IFieldOccupant occupant)
		{
			var value = byte.MinValue;
			ViewModel.FieldOccupants.TryRemove(occupant, out value);
			ViewModel.UpdateState(this);

			var h = Yielded;
			if (h != null) h(occupant);
		}
		public event Action<IFieldOccupant> Yielded;
	}

	public class SchiffspositionModel : ViewModel
	{
		ConcurrentDictionary<IFieldOccupant, byte> _FieldOccupants = new ConcurrentDictionary<IFieldOccupant, byte>();
		public ConcurrentDictionary<IFieldOccupant, byte> FieldOccupants { get { return _FieldOccupants; } }

		string _Text = "##";
		public string Text
		{
			get { return _Text; }
			set { _Text = value; NotifyChanged("Text"); }
		}

		string _StateText = "0 occupants";
		public string StateText
		{
			get { return _StateText; }
			set { _StateText = value; NotifyChanged("StateText"); }
		}

		Brush _Color;
		public Brush Color
		{
			get { return _Color; }
			set { _Color = value; NotifyChanged("Color"); }
		}

		Visibility _TopArrowVisibility;
		public Visibility TopArrowVisibility
		{
			get { return _TopArrowVisibility; }
			set { _TopArrowVisibility = value; NotifyChanged("TopArrowVisibility"); }
		}

		Visibility _BottomArrowVisibility;
		public Visibility BottomArrowVisibility
		{
			get { return _BottomArrowVisibility; }
			set { _BottomArrowVisibility = value; NotifyChanged("BottomArrowVisibility"); }
		}

		Visibility _LeftArrowVisibility;
		public Visibility LeftArrowVisibility
		{
			get { return _LeftArrowVisibility; }
			set { _LeftArrowVisibility = value; NotifyChanged("LeftArrowVisibility"); }
		}

		Visibility _RightArrowVisibility;
		public Visibility RightArrowVisibility
		{
			get { return _RightArrowVisibility; }
			set { _RightArrowVisibility = value; NotifyChanged("RightArrowVisibility"); }
		}

		bool _RangeIndicatorVisible;
		public bool RangeIndicatorVisible
		{
			get { return _RangeIndicatorVisible; }
			set { _RangeIndicatorVisible = value; NotifyChanged("RangeIndicatorVisible"); }
		}

		public void UpdateState(IField field)
		{
			HideArrows();

			if (FieldOccupants.Any())
			{
				foreach (var occupant in FieldOccupants.Keys)
				{
					if (occupant.OrientatesTop(field)) TopArrowVisibility = Visibility.Visible;
					if (occupant.OrientatesBottom(field)) BottomArrowVisibility = Visibility.Visible;
					if (occupant.OrientatesRight(field)) RightArrowVisibility = Visibility.Visible;
					if (occupant.OrientatesLeft(field)) LeftArrowVisibility = Visibility.Visible;
				}
			}

			StateText = FieldOccupants.Count + " occupants";
		}

		public void HideArrows()
		{
			TopArrowVisibility = Visibility.Collapsed;
			BottomArrowVisibility = Visibility.Collapsed;
			LeftArrowVisibility = Visibility.Collapsed;
			RightArrowVisibility = Visibility.Collapsed;
		}
	}
}
