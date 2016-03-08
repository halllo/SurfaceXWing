using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SurfaceXWing
{
	public class ForwardMove : Move
	{
		List<Path> _Fluglinien = new List<Path>();

		protected override void CreatePotenzielleZiele(Schiffsposition von, Action<Schiffsposition> enable)
		{
			var angle = von.OrientationAngle;
			var position = von.Position.AsVector();

			var gradeaus = angle.AsVector();
			var links = (angle - 90).AsVector();
			var rechts = (angle + 90).AsVector();


			enable(SchiffspositionFabrik.Neu(//1gradeaus
				position: position + (gradeaus * 172),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(SchiffspositionFabrik.Neu(//1scharflinks
				position: position + (gradeaus * 120) + (links * 120),
				orientation: angle - 90,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(SchiffspositionFabrik.Neu(//1leichtlinks
				position: position + (gradeaus * 202) + (links * 82),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(SchiffspositionFabrik.Neu(//1scharfrechts
				position: position + (gradeaus * 120) + (rechts * 120),
				orientation: angle + 90,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(SchiffspositionFabrik.Neu(//1leichtrechts
				position: position + (gradeaus * 202) + (rechts * 82),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));


			enable(SchiffspositionFabrik.Neu(//2gradeaus
				position: position + (gradeaus * 259),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(SchiffspositionFabrik.Neu(//2scharflinks
				position: position + (gradeaus * 180) + (links * 180),
				orientation: angle - 90,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(SchiffspositionFabrik.Neu(//2leichtlinks
				position: position + (gradeaus * 275) + (links * 119),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(SchiffspositionFabrik.Neu(//2scharfrechts
				position: position + (gradeaus * 180) + (rechts * 180),
				orientation: angle + 90,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(SchiffspositionFabrik.Neu(//2leichtrechts
				position: position + (gradeaus * 275) + (rechts * 119),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));


			enable(SchiffspositionFabrik.Neu(//3gradeaus
				position: position + (gradeaus * 346),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));

			enable(SchiffspositionFabrik.Neu(//3scharflinks
				position: position + (gradeaus * 240) + (links * 240),
				orientation: angle - 90,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));

			enable(SchiffspositionFabrik.Neu(//3leichtlinks
				position: position + (gradeaus * 355) + (links * 145),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));

			enable(SchiffspositionFabrik.Neu(//3scharfrechts
				position: position + (gradeaus * 240) + (rechts * 240),
				orientation: angle + 90,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));

			enable(SchiffspositionFabrik.Neu(//3lechtrechts
				position: position + (gradeaus * 355) + (rechts * 145),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));


			enable(SchiffspositionFabrik.Neu(//4gradeaus
				position: position + (gradeaus * 433),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "4"));

			enable(SchiffspositionFabrik.Neu(//5gradeaus
				position: position + (gradeaus * 520),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "5"));


			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 77,77 0 0 0 -34,-77") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 77,77 0 0 1 120,-77") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 168,168 0 0 0 -6,-131") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 168,168 0 0 1 92,-131") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 137,137 0 0 0 -94,-137") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 137,137 0 0 1 180,-137") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 300,300 0 0 0 -46,-201") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 300,300 0 0 1 132,-201") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 196,196 0 0 0 -154,-197") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 196,196 0 0 1 240,-197") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 390,390 0 0 0 -72,-281") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 390,390 0 0 1 158,-281") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 L 43,-434") });
			_Fluglinien.ForEach(fl => von.Canvas.Children.Add(fl));
		}

		protected override void MovedOrCanceled(Schiffsposition von)
		{
			_Fluglinien.ForEach(fl => von.Canvas.Children.Remove(fl));
			_Fluglinien.Clear();
		}
	}

	public class BarrelRollMove : Move
	{
		Schiffsposition _Links1;
		Schiffsposition _Links1SchrägVorwärtz;
		Schiffsposition _Links1SchrägRückwärtz;
		Schiffsposition _Rechts1;
		Schiffsposition _Rechts1SchrägVorwärtz;
		Schiffsposition _Rechts1SchrägRückwärtz;

		Schiffsposition _Links2;
		Schiffsposition _Links2SchrägVorwärtz;
		Schiffsposition _Links2SchrägRückwärtz;
		Schiffsposition _Rechts2;
		Schiffsposition _Rechts2SchrägVorwärtz;
		Schiffsposition _Rechts2SchrägRückwärtz;

		Path _LinksGrade;
		Path _RechtsGrade;

		protected override void CreatePotenzielleZiele(Schiffsposition von, Action<Schiffsposition> enable)
		{
			var angle = von.OrientationAngle;
			var position = von.Position.AsVector();

			var gradeaus = angle.AsVector();
			var nachhinten = (angle + 180).AsVector();
			var links = (angle - 90).AsVector();
			var rechts = (angle + 90).AsVector();


			enable(_Links1 = SchiffspositionFabrik.Neu(
				position: position + (links * 172),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Links1SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: position + (links * 202) + (gradeaus * 82),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Links1SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: position + (links * 202) + (nachhinten * 82),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Rechts1 = SchiffspositionFabrik.Neu(
				position: position + (rechts * 172),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Rechts1SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: position + (rechts * 202) + (gradeaus * 82),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Rechts1SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: position + (rechts * 202) + (nachhinten * 82),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));


			enable(_Links2 = SchiffspositionFabrik.Neu(
				position: position + (links * 259),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Links2SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: position + (links * 275) + (gradeaus * 119),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Links2SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: position + (links * 275) + (nachhinten * 119),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Rechts2 = SchiffspositionFabrik.Neu(
				position: position + (rechts * 259),
				orientation: angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Rechts2SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: position + (rechts * 275) + (gradeaus * 119),
				orientation: angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Rechts2SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: position + (rechts * 275) + (nachhinten * 119),
				orientation: angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));


			von.Canvas.Children.Add(_LinksGrade = new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 0,43 L -172,43") });
			von.Canvas.Children.Add(_RechtsGrade = new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 86,43 L 258,43") });


			//TODO: slider2
			von.ViewModel.Slider1Visible = true;
			von.Slider1.Value = 0;
			von.Slider1.ValueChanged += slider1ValueChanged = new RoutedPropertyChangedEventHandler<double>((object o, RoutedPropertyChangedEventArgs<double> e) =>
			{
				var value = e.NewValue;
				_Links1.PositionAt(position + (links * 172) + (gradeaus * value * 4.3));
				_Links1SchrägVorwärtz.PositionAt(position + (links * 202) + (gradeaus * 82) + (gradeaus * value * 4.3));
				_Links1SchrägRückwärtz.PositionAt(position + (links * 202) + (nachhinten * 82) + (gradeaus * value * 4.3));
				_Rechts1.PositionAt(position + (rechts * 172) + (gradeaus * value * -4.3));
				_Rechts1SchrägVorwärtz.PositionAt(position + (rechts * 202) + (gradeaus * 82) + (gradeaus * value * -4.3));
				_Rechts1SchrägRückwärtz.PositionAt(position + (rechts * 202) + (nachhinten * 82) + (gradeaus * value * -4.3));
				_Links2.PositionAt(position + (links * 259) + (gradeaus * value * 4.3));
				_Links2SchrägVorwärtz.PositionAt(position + (links * 275) + (gradeaus * 119) + (gradeaus * value * 4.3));
				_Links2SchrägRückwärtz.PositionAt(position + (links * 275) + (nachhinten * 119) + (gradeaus * value * 4.3));
				_Rechts2.PositionAt(position + (rechts * 259) + (gradeaus * value * -4.3));
				_Rechts2SchrägVorwärtz.PositionAt(position + (rechts * 275) + (gradeaus * 119) + (gradeaus * value * -4.3));
				_Rechts2SchrägRückwärtz.PositionAt(position + (rechts * 275) + (nachhinten * 119) + (gradeaus * value * -4.3));
			});

			von.ViewModel.Slider2Visible = true;
			von.Slider2.Value = 0;
			von.Slider2.ValueChanged += slider2ValueChanged = new RoutedPropertyChangedEventHandler<double>((object o, RoutedPropertyChangedEventArgs<double> e) =>
			{
				var value = e.NewValue;
				_Links1SchrägVorwärtz.ViewModel.Label = value.ToString();
				_Links1SchrägRückwärtz.ViewModel.Label = value.ToString();
			});
		}

		protected override void MovedOrCanceled(Schiffsposition von)
		{
			von.Canvas.Children.Remove(_LinksGrade);
			von.Canvas.Children.Remove(_RechtsGrade);

			von.ViewModel.Slider1Visible = false;
			von.Slider1.ValueChanged -= slider1ValueChanged;

			von.ViewModel.Slider2Visible = false;
			von.Slider2.ValueChanged -= slider2ValueChanged;
		}

		RoutedPropertyChangedEventHandler<double> slider1ValueChanged;
		RoutedPropertyChangedEventHandler<double> slider2ValueChanged;
	}
}
