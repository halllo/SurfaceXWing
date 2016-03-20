using SurfaceGameBasics;
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
		double _Angle;
		Vector _Position;
		Vector _Gradeaus;
		Vector _Nachhinten;
		Vector _Links;
		Vector _Rechts;

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

		List<Path> _FluglinienLinks = new List<Path>();
		List<Path> _FluglinienRechts = new List<Path>();

		protected override void CreatePotenzielleZiele(Schiffsposition von, Action<Schiffsposition> enable)
		{
			_Angle = von.OrientationAngle;
			_Position = von.Position.AsVector();

			_Gradeaus = _Angle.AsVector();
			_Nachhinten = (_Angle + 180).AsVector();
			_Links = (_Angle - 90).AsVector();
			_Rechts = (_Angle + 90).AsVector();


			enable(_Links1 = SchiffspositionFabrik.Neu(
				position: _Position + (_Links * 172),
				orientation: _Angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Links1SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Links * 202) + (_Gradeaus * 82),
				orientation: _Angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Links1SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Links * 202) + (_Nachhinten * 82),
				orientation: _Angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Links2 = SchiffspositionFabrik.Neu(
				position: _Position + (_Links * 259),
				orientation: _Angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Links2SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Links * 275) + (_Gradeaus * 119),
				orientation: _Angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Links2SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Links * 275) + (_Nachhinten * 119),
				orientation: _Angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));


			enable(_Rechts1 = SchiffspositionFabrik.Neu(
				position: _Position + (_Rechts * 172),
				orientation: _Angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Rechts1SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Rechts * 202) + (_Gradeaus * 82),
				orientation: _Angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Rechts1SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Rechts * 202) + (_Nachhinten * 82),
				orientation: _Angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "1"));

			enable(_Rechts2 = SchiffspositionFabrik.Neu(
				position: _Position + (_Rechts * 259),
				orientation: _Angle,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Rechts2SchrägVorwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Rechts * 275) + (_Gradeaus * 119),
				orientation: _Angle - 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));

			enable(_Rechts2SchrägRückwärtz = SchiffspositionFabrik.Neu(
				position: _Position + (_Rechts * 275) + (_Nachhinten * 119),
				orientation: _Angle + 45,
				color: von.ViewModel.Color, opacity: 0.4, label: "2"));


			_FluglinienLinks.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 0,43 L -172,43") });
			_FluglinienLinks.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 0,43 A 168,168 0 0 1 -131,-6") });
			_FluglinienLinks.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 0,43 A 168,168 0 0 0 -131,92") });
			_FluglinienLinks.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 0,43 A 300,300 0 0 1 -201,-46") });
			_FluglinienLinks.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 0,43 A 300,300 0 0 0 -201,132") });
			_FluglinienLinks.ForEach(fl => von.Canvas.Children.Add(fl));

			_FluglinienRechts.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 86,43 L 258,43") });
			_FluglinienRechts.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 86,43 A 168,168 0 0 0 217,-6") });
			_FluglinienRechts.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 86,43 A 168,168 0 0 1 217,92") });
			_FluglinienRechts.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 86,43 A 300,300 0 0 0 287,-46") });
			_FluglinienRechts.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 86,43 A 300,300 0 0 1 287,132") });
			_FluglinienRechts.ForEach(fl => von.Canvas.Children.Add(fl));


			von.ViewModel.Slider1Visible = true;
			von.Slider1.Value = 0;
			von.Slider1.ValueChanged += slider1ValueChanged = new RoutedPropertyChangedEventHandler<double>((object o, RoutedPropertyChangedEventArgs<double> e) =>
			{
				Slide(von.Slider1.Value, von.Slider2.Value);
			});

			von.ViewModel.Slider2Visible = true;
			von.Slider2.Value = 0;
			von.Slider2.ValueChanged += slider2ValueChanged = new RoutedPropertyChangedEventHandler<double>((object o, RoutedPropertyChangedEventArgs<double> e) =>
			{
				Slide(von.Slider1.Value, von.Slider2.Value);
			});
		}

		private void Slide(double slide1, double slide2)
		{
			var sourceSlide = slide1 * 2.2;
			var targetSlide = slide2 * 2.2;

			_FluglinienLinks.ForEach(fl => fl.SetValue(System.Windows.Controls.Canvas.TopProperty, sourceSlide * -1));
			_FluglinienRechts.ForEach(fl => fl.SetValue(System.Windows.Controls.Canvas.TopProperty, sourceSlide));

			_Links1.PositionAt(_Position + (_Links * 172) + (_Gradeaus * sourceSlide) + (_Gradeaus * targetSlide));
			_Links1SchrägVorwärtz.PositionAt(_Position + (_Links * 202) + (_Gradeaus * 82) + (_Gradeaus * sourceSlide) + ((_Angle + 45).AsVector() * targetSlide));
			_Links1SchrägRückwärtz.PositionAt(_Position + (_Links * 202) + (_Nachhinten * 82) + (_Gradeaus * sourceSlide) + ((_Angle - 45).AsVector() * targetSlide));
			_Links2.PositionAt(_Position + (_Links * 259) + (_Gradeaus * sourceSlide) + (_Gradeaus * targetSlide));
			_Links2SchrägVorwärtz.PositionAt(_Position + (_Links * 275) + (_Gradeaus * 119) + (_Gradeaus * sourceSlide) + ((_Angle + 45).AsVector() * targetSlide));
			_Links2SchrägRückwärtz.PositionAt(_Position + (_Links * 275) + (_Nachhinten * 119) + (_Gradeaus * sourceSlide) + ((_Angle - 45).AsVector() * targetSlide));

			_Rechts1.PositionAt(_Position + (_Rechts * 172) + (_Gradeaus * sourceSlide * -1) + (_Gradeaus * targetSlide * -1));
			_Rechts1SchrägVorwärtz.PositionAt(_Position + (_Rechts * 202) + (_Gradeaus * 82) + (_Gradeaus * sourceSlide * -1) + ((_Angle - 45).AsVector() * targetSlide * -1));
			_Rechts1SchrägRückwärtz.PositionAt(_Position + (_Rechts * 202) + (_Nachhinten * 82) + (_Gradeaus * sourceSlide * -1) + ((_Angle + 45).AsVector() * targetSlide * -1));
			_Rechts2.PositionAt(_Position + (_Rechts * 259) + (_Gradeaus * sourceSlide * -1) + (_Gradeaus * targetSlide * -1));
			_Rechts2SchrägVorwärtz.PositionAt(_Position + (_Rechts * 275) + (_Gradeaus * 119) + (_Gradeaus * sourceSlide * -1) + ((_Angle - 45).AsVector() * targetSlide * -1));
			_Rechts2SchrägRückwärtz.PositionAt(_Position + (_Rechts * 275) + (_Nachhinten * 119) + (_Gradeaus * sourceSlide * -1) + ((_Angle + 45).AsVector() * targetSlide * -1));
		}

		protected override void MovedOrCanceled(Schiffsposition von)
		{
			_FluglinienLinks.ForEach(fl => von.Canvas.Children.Remove(fl));
			_FluglinienLinks.Clear();
			_FluglinienRechts.ForEach(fl => von.Canvas.Children.Remove(fl));
			_FluglinienRechts.Clear();

			von.ViewModel.Slider1Visible = false;
			von.Slider1.ValueChanged -= slider1ValueChanged;

			von.ViewModel.Slider2Visible = false;
			von.Slider2.ValueChanged -= slider2ValueChanged;
		}

		RoutedPropertyChangedEventHandler<double> slider1ValueChanged;
		RoutedPropertyChangedEventHandler<double> slider2ValueChanged;
	}

	public class Slide3Move : Move
	{
		double _Angle;
		Vector _Position;
		Vector _Gradeaus;
		Vector _Links;
		Vector _Rechts;

		Schiffsposition _Links3;
		Schiffsposition _Rechts3;

		List<Path> _Fluglinien = new List<Path>();

		protected override void CreatePotenzielleZiele(Schiffsposition von, Action<Schiffsposition> enable)
		{
			_Angle = von.OrientationAngle;
			_Position = von.Position.AsVector();

			_Gradeaus = _Angle.AsVector();
			_Links = (_Angle - 90).AsVector();
			_Rechts = (_Angle + 90).AsVector();


			enable(_Links3 = SchiffspositionFabrik.Neu(
				position: _Position + (_Gradeaus * 240) + (_Links * 240),
				orientation: _Angle - 180,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));

			enable(_Rechts3 = SchiffspositionFabrik.Neu(
				position: _Position + (_Gradeaus * 240) + (_Rechts * 240),
				orientation: _Angle + 180,
				color: von.ViewModel.Color, opacity: 0.4, label: "3"));


			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 196,196 0 0 0 -154,-197") });
			_Fluglinien.Add(new Path { Opacity = 0.2, Stroke = Brushes.White, StrokeThickness = 43, Data = Geometry.Parse("M 43,0 A 196,196 0 0 1 240,-197") });
			_Fluglinien.ForEach(fl => von.Canvas.Children.Add(fl));


			von.ViewModel.Slider1Visible = true;
			von.Slider1.Value = 0;
			von.Slider1.ValueChanged += slider1ValueChanged = new RoutedPropertyChangedEventHandler<double>((object o, RoutedPropertyChangedEventArgs<double> e) =>
			{
				var targetSlide = e.NewValue * 2.2;

				_Links3.PositionAt(_Position + (_Gradeaus * 240) + (_Links * 240) + (_Gradeaus * targetSlide));
				_Rechts3.PositionAt(_Position + (_Gradeaus * 240) + (_Rechts * 240) + (_Gradeaus * targetSlide * -1));
			});
		}

		protected override void MovedOrCanceled(Schiffsposition von)
		{
			_Fluglinien.ForEach(fl => von.Canvas.Children.Remove(fl));
			_Fluglinien.Clear();

			von.ViewModel.Slider1Visible = false;
			von.Slider1.ValueChanged -= slider1ValueChanged;
		}

		RoutedPropertyChangedEventHandler<double> slider1ValueChanged;
	}
}
