﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SurfaceGameBasics;

namespace SurfaceXWing
{
	public partial class Schiffsposition : IField
	{
		public SchiffspositionModel ViewModel { get; private set; }

		public Schiffsposition()
		{
			InitializeComponent();

			DataContext = ViewModel = new SchiffspositionModel();
		}

		public Move Move { get; set; }

		string id;
		string IField.Id
		{
			get { return id; }
			set { id = value; }
		}
		public Point Position
		{
			get { return new Point((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty)); }
			set { SetValue(Canvas.LeftProperty, value.X); SetValue(Canvas.TopProperty, value.Y); }
		}
		public event Action<IField> PositionChanged;
		public double OrientationAngle
		{
			get { return RenderTransform is RotateTransform ? ((RotateTransform)RenderTransform).Angle : 0; }
			set { if (RenderTransform is RotateTransform) ((RotateTransform)RenderTransform).Angle = value; }
		}
		public Vector Size { get { return new Vector(Width, Height); } }

		public bool IsOccupiedBy(IFieldOccupant occupant)
		{
			return ViewModel.FieldOccupants.ContainsKey(occupant);
		}

		public void AllowOccupant(IFieldOccupant occupant)
		{
			ViewModel.Tokens = ((TagVisual)occupant).ViewModel.Tokens;
		}
		public string AllowedOccupantId { get { return ViewModel.Tokens.Id; } }
		public bool CanOccupy(IFieldOccupant occupant)
		{
			return occupant.Id == AllowedOccupantId;
		}
		public void Occupy(IFieldOccupant occupant)
		{
			ViewModel.FieldOccupants.TryAdd(occupant, byte.MinValue);
			ViewModel.UpdateState(this);
			LastOccupant = occupant;

			var h = Occupied;
			if (h != null) h(this, occupant);
		}
		public event Action<IField, IFieldOccupant> Occupied;
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
			if (h != null) h(this, occupant);
		}
		public event Action<IField, IFieldOccupant> Yielded;

		public IFieldOccupant LastOccupant { get; private set; }
		public FieldsView FieldsView { get; set; }

		public void Activate(
			Action<Schiffsposition, object> onForget = null,
			Action<Schiffsposition, object> onForward = null,
			Action<Schiffsposition, object> onBarrelRoll = null,
			Action<Schiffsposition, object> onSlide3 = null)
		{
			Opacity = 1.0;
			menu1.Visibility = Visibility.Visible;
			menu2.Visibility = Visibility.Visible;
			menu3.Visibility = Visibility.Visible;
			tokens.Visibility = Visibility.Visible;
			ViewModel.GoBack = new Command(GoBackMethod);

			if (onForget != null) ViewModel.Forget = new Command(o => onForget(this, o));
			if (onForward != null) ViewModel.Forward = new Command(o => onForward(this, o));
			if (onBarrelRoll != null) ViewModel.BarrelRoll = new Command(o => onBarrelRoll(this, o));
			if (onSlide3 != null) ViewModel.Slide3 = new Command(o => onSlide3(this, o));
		}

		public void PositionAt(Vector position)
		{
			Position = position.AsPoint();

			var h = PositionChanged;
			if (h != null) h(this);
		}

		public void PositionAt(Point position)
		{
			Position = position;

			var h = PositionChanged;
			if (h != null) h(this);
		}

		private void GoBackMethod()
		{
			var letztePosition = ViewModel.LetztePosition;
			if (letztePosition != null)
			{
				ViewModel.LetztePosition = null;

				if (ViewModel.Cancel != null)
					ViewModel.Cancel.Execute(null);

				OrientationAngle = letztePosition.Orientation;
				PositionAt(letztePosition.Point);
				((Spielfeld)FieldsView).ZieleErfassen();
			}
		}
	}

	public class SchiffspositionModel : ViewModel
	{
		public class Position
		{
			public Position(Point point, double orientation)
			{
				Point = point;
				Orientation = orientation;
			}

			public Point Point { get; set; }
			public double Orientation { get; set; }
		}

		ConcurrentDictionary<IFieldOccupant, byte> _FieldOccupants = new ConcurrentDictionary<IFieldOccupant, byte>();
		public ConcurrentDictionary<IFieldOccupant, byte> FieldOccupants { get { return _FieldOccupants; } }

		public Position LetztePosition { get; set; }

		public SchiffspositionModel()
		{
			Range = new Command(() => RangeIndicatorVisible = !RangeIndicatorVisible);
		}

		public SchiffTokens Tokens { get; set; }

		Brush _Color;
		public Brush Color
		{
			get { return _Color; }
			set { _Color = value; NotifyChanged("Color"); }
		}

		Command _Forget;
		public Command Forget
		{
			get { return _Forget; }
			set { _Forget = value; NotifyChanged("Forget"); }
		}

		Command _GoBack;
		public Command GoBack
		{
			get { return _GoBack; }
			set { _GoBack = value; NotifyChanged("GoBack"); }
		}

		Command _Range;
		public Command Range
		{
			get { return _Range; }
			set { _Range = value; NotifyChanged("Range"); }
		}

		Command _BarrelRoll;
		public Command BarrelRoll
		{
			get { return _BarrelRoll; }
			set { _BarrelRoll = value; NotifyChanged("BarrelRoll"); }
		}

		Command _Slide3;
		public Command Slide3
		{
			get { return _Slide3; }
			set { _Slide3 = value; NotifyChanged("Slide3"); }
		}

		Command _Forward;
		public Command Forward
		{
			get { return _Forward; }
			set { _Forward = value; NotifyChanged("Forward"); }
		}

		Command _Cancel;
		public Command Cancel
		{
			get { return _Cancel; }
			set { _Cancel = value; NotifyChanged("Cancel"); }
		}

		bool _Cancelable;
		public bool Cancelable
		{
			get { return _Cancelable; }
			private set { _Cancelable = value; NotifyChanged("Cancelable"); }
		}

		public void AllowCancel(Action cancel)
		{
			Cancelable = true;
			Cancel = new Command(cancel);
		}
		public void CancelCancel()
		{
			Cancelable = false;
			Cancel = null;
		}

		bool _RangeIndicatorVisible;
		public bool RangeIndicatorVisible
		{
			get { return _RangeIndicatorVisible; }
			set
			{
				_RangeIndicatorVisible = value; NotifyChanged("RangeIndicatorVisible");
			}
		}

		bool _Slider1Visible;
		public bool Slider1Visible
		{
			get { return _Slider1Visible; }
			set { _Slider1Visible = value; NotifyChanged("Slider1Visible"); }
		}

		bool _Slider2Visible;
		public bool Slider2Visible
		{
			get { return _Slider2Visible; }
			set { _Slider2Visible = value; NotifyChanged("Slider2Visible"); }
		}

		double _BackgroundOpacity = 0.1;
		public double BackgroundOpacity
		{
			get { return _BackgroundOpacity; }
			set { _BackgroundOpacity = value; NotifyChanged("BackgroundOpacity"); }
		}

		string _Label;
		public string Label
		{
			get { return _Label; }
			set { _Label = value; NotifyChanged("Label"); }
		}

		public void UpdateState(IField field)
		{
			if (FieldOccupants.Any())
			{
				BackgroundOpacity = 1;
			}
			else
			{
				BackgroundOpacity = 0.5;
			}
		}
	}
}
