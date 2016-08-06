using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SurfaceXWing
{
	[ContentProperty("Text")]
	public class TextOnAPath : Control
	{
		static TextOnAPath()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TextOnAPath), new FrameworkPropertyMetadata(typeof(TextOnAPath)));

			Control.FontSizeProperty.OverrideMetadata(typeof(TextOnAPath), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontPropertyChanged)));

			Control.FontFamilyProperty.OverrideMetadata(typeof(TextOnAPath), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontPropertyChanged)));

			Control.FontStretchProperty.OverrideMetadata(typeof(TextOnAPath), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontPropertyChanged)));

			Control.FontStyleProperty.OverrideMetadata(typeof(TextOnAPath), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontPropertyChanged)));

			Control.FontWeightProperty.OverrideMetadata(typeof(TextOnAPath), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontPropertyChanged)));
		}

		static void OnFontPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextOnAPath textOnAPath = d as TextOnAPath;

			if (textOnAPath == null)
				return;

			if (e.NewValue == null || e.NewValue == e.OldValue)
				return;

			textOnAPath.UpdateText();
			textOnAPath.Update();
		}

		double[] _segmentLengths;
		TextBlock[] _textBlocks;

		Panel _layoutPanel;
		bool _layoutHasValidSize = false;

		#region Text DP
		public String Text
		{
			get { return (String)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(String), typeof(TextOnAPath),
			new PropertyMetadata(null, new PropertyChangedCallback(OnStringPropertyChanged),
				new CoerceValueCallback(CoerceTextValue)));

		static void OnStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextOnAPath textOnAPath = d as TextOnAPath;

			if (textOnAPath == null)
				return;

			if (e.NewValue == e.OldValue || e.NewValue == null)
			{
				if (textOnAPath._layoutPanel != null)
					textOnAPath._layoutPanel.Children.Clear();
				return;
			}

			textOnAPath.UpdateText();
			textOnAPath.Update();
		}

		static object CoerceTextValue(DependencyObject d, object baseValue)
		{
			if ((String)baseValue == "")
				return null;

			return baseValue;
		}

		#endregion

		#region TextPath DP
		public Geometry TextPath
		{
			get { return (Geometry)GetValue(TextPathProperty); }
			set { SetValue(TextPathProperty, value); }
		}

		public static readonly DependencyProperty TextPathProperty =
			DependencyProperty.Register("TextPath", typeof(Geometry), typeof(TextOnAPath),
			new FrameworkPropertyMetadata(null,

										  new PropertyChangedCallback(OnTextPathPropertyChanged)));

		static void OnTextPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextOnAPath textOnAPath = d as TextOnAPath;

			if (textOnAPath == null)
				return;

			if (e.NewValue == e.OldValue || e.NewValue == null)
				return;

			textOnAPath.TextPath.Transform = null;

			textOnAPath.UpdateSize();
			textOnAPath.Update();
		}

		#endregion

		#region DrawPath DP

		/// <summary>
		/// Set this property to True to display the TextPath geometry in the control
		/// </summary>
		public bool DrawPath
		{
			get { return (bool)GetValue(DrawPathProperty); }
			set { SetValue(DrawPathProperty, value); }
		}

		public static readonly DependencyProperty DrawPathProperty =
			DependencyProperty.Register("DrawPath", typeof(bool), typeof(TextOnAPath),
			new PropertyMetadata(false, new PropertyChangedCallback(OnDrawPathPropertyChanged)));

		static void OnDrawPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextOnAPath textOnAPath = d as TextOnAPath;

			if (textOnAPath == null)
				return;

			if (e.NewValue == e.OldValue || e.NewValue == null)
				return;

			textOnAPath.Update();
		}

		#endregion

		#region DrawLinePath DP
		/// <summary>
		/// Set this property to True to display the line segments under the text (flattened path)
		/// </summary>
		public bool DrawLinePath
		{
			get { return (bool)GetValue(DrawLinePathProperty); }
			set { SetValue(DrawLinePathProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DrawFlattendPath.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DrawLinePathProperty =
			DependencyProperty.Register("DrawLinePath", typeof(bool), typeof(TextOnAPath),
			new PropertyMetadata(false, new PropertyChangedCallback(OnDrawLinePathPropertyChanged)));

		static void OnDrawLinePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextOnAPath textOnAPath = d as TextOnAPath;

			if (textOnAPath == null)
				return;

			if (e.NewValue == e.OldValue || e.NewValue == null)
				return;

			textOnAPath.Update();
		}

		#endregion

		#region ScaleTextPath DP
		/// <summary>
		/// If set to True (default) then the geometry defined by TextPath automatically gets scaled to fit the width/height of the control
		/// </summary>
		public bool ScaleTextPath
		{
			get { return (bool)GetValue(ScaleTextPathProperty); }
			set { SetValue(ScaleTextPathProperty, value); }
		}

		public static readonly DependencyProperty ScaleTextPathProperty =
			DependencyProperty.Register("ScaleTextPath", typeof(bool), typeof(TextOnAPath),
					new PropertyMetadata(false, new PropertyChangedCallback(OnScaleTextPathPropertyChanged)));

		static void OnScaleTextPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextOnAPath textOnAPath = d as TextOnAPath;

			if (textOnAPath == null)
				return;

			if (e.NewValue == e.OldValue)
				return;

			bool value = (Boolean)e.NewValue;

			if (value == false && textOnAPath.TextPath != null)
				textOnAPath.TextPath.Transform = null;

			textOnAPath.UpdateSize();
			textOnAPath.Update();

		}

		#endregion

		public bool Inverted { get; set; }

		void UpdateText()
		{
			if (Text == null || FontFamily == null || FontWeight == null || FontStyle == null)
				return;

			_textBlocks = new TextBlock[Text.Length];
			_segmentLengths = new double[Text.Length];

			for (int i = 0; i < Text.Length; i++)
			{
				TextBlock t = new TextBlock();
				t.FontSize = this.FontSize;
				t.FontFamily = this.FontFamily;
				t.FontStretch = this.FontStretch;
				t.FontWeight = this.FontWeight;
				t.FontStyle = this.FontStyle;

				if (Inverted)
				{
					t.Text = new String(Text[i], 1);
					t.LayoutTransform = new RotateTransform(angle: 0);
				}
				else
				{
					t.Text = new String(Text[(Text.Length - 1) - i], 1);
					t.LayoutTransform = new RotateTransform(angle: 180);
				}

				t.RenderTransformOrigin = new Point(0.0, 1.0);
				t.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

				_textBlocks[i] = t;
				_segmentLengths[i] = t.DesiredSize.Width;


			}
		}

		void Update()
		{
			if (Text == null || TextPath == null || _layoutPanel == null || !_layoutHasValidSize)
				return;

			List<Point> intersectionPoints;

			intersectionPoints = GeometryHelper.GetIntersectionPoints(TextPath.GetFlattenedPathGeometry(), _segmentLengths);

			_layoutPanel.Children.Clear();

			_layoutPanel.Margin = new Thickness(FontSize);

			for (int i = 0; i < intersectionPoints.Count - 1; i++)
			{
				double oppositeLen = Math.Sqrt(Math.Pow(intersectionPoints[i].X + _segmentLengths[i] - intersectionPoints[i + 1].X, 2.0) + Math.Pow(intersectionPoints[i].Y - intersectionPoints[i + 1].Y, 2.0)) / 2.0;
				double hypLen = Math.Sqrt(Math.Pow(intersectionPoints[i].X - intersectionPoints[i + 1].X, 2.0) + Math.Pow(intersectionPoints[i].Y - intersectionPoints[i + 1].Y, 2.0));

				double ratio = oppositeLen / hypLen;

				if (ratio > 1.0)
					ratio = 1.0;
				else if (ratio < -1.0)
					ratio = -1.0;

				//double angle = 0.0;

				double angle = 2.0 * Math.Asin(ratio) * 180.0 / Math.PI;

				// adjust sign on angle
				if ((intersectionPoints[i].X + _segmentLengths[i]) > intersectionPoints[i].X)
				{
					if (intersectionPoints[i + 1].Y < intersectionPoints[i].Y)
						angle = -angle;
				}
				else
				{
					if (intersectionPoints[i + 1].Y > intersectionPoints[i].Y)
						angle = -angle;
				}

				TextBlock currTextBlock = _textBlocks[i];

				RotateTransform rotate = new RotateTransform(angle);
				TranslateTransform translate = new TranslateTransform(intersectionPoints[i].X, intersectionPoints[i].Y - currTextBlock.DesiredSize.Height);
				TransformGroup transformGrp = new TransformGroup();
				transformGrp.Children.Add(rotate);
				transformGrp.Children.Add(translate);
				currTextBlock.RenderTransform = transformGrp;

				_layoutPanel.Children.Add(currTextBlock);

				if (DrawLinePath == true)
				{
					Line line = new Line();
					line.X1 = intersectionPoints[i].X;
					line.Y1 = intersectionPoints[i].Y;
					line.X2 = intersectionPoints[i + 1].X;
					line.Y2 = intersectionPoints[i + 1].Y;
					line.Stroke = Brushes.Black;
					_layoutPanel.Children.Add(line);
				}
			}

			// don't draw path if already drawing line path
			if (DrawPath == true && DrawLinePath == false)
			{
				Path path = new Path();
				path.Data = TextPath;
				path.Stroke = Brushes.Black;
				_layoutPanel.Children.Add(path);
			}
		}

		public TextOnAPath()
		{
			var canvas = new FrameworkElementFactory(typeof(Canvas));
			canvas.Name = "LayoutPanel";

			var border = new FrameworkElementFactory(typeof(Border));
			border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(BorderBrushProperty));
			border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(BorderThicknessProperty));
			border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));
			border.AppendChild(canvas);

			var controlTemplate = new ControlTemplate(typeof(TextOnAPath));
			controlTemplate.VisualTree = border;

			Template = controlTemplate;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_layoutPanel = GetTemplateChild("LayoutPanel") as Panel;
			if (_layoutPanel == null)
				throw new Exception("Could not find template part: LayoutPanel");

			_layoutPanel.SizeChanged += new SizeChangedEventHandler(_layoutPanel_SizeChanged);
		}

		Size _newSize;

		void _layoutPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_newSize = e.NewSize;

			UpdateSize();
			Update();
		}

		void UpdateSize()
		{
			if (_newSize == null || TextPath == null)
				return;

			_layoutHasValidSize = true;

			double xScale = _newSize.Width / TextPath.Bounds.Width;
			double yScale = _newSize.Height / TextPath.Bounds.Height;

			if (TextPath.Bounds.Width <= 0)
				xScale = 1.0;

			if (TextPath.Bounds.Height <= 0)
				xScale = 1.0;

			if (xScale <= 0 || yScale <= 0)
				return;

			if (TextPath.Transform is TransformGroup)
			{
				TransformGroup grp = TextPath.Transform as TransformGroup;
				if (grp.Children[0] is ScaleTransform && grp.Children[1] is TranslateTransform)
				{
					if (ScaleTextPath)
					{
						ScaleTransform scale = grp.Children[0] as ScaleTransform;
						scale.ScaleX *= xScale;
						scale.ScaleY *= yScale;
					}

					TranslateTransform translate = grp.Children[1] as TranslateTransform;
					translate.X += -TextPath.Bounds.X;
					translate.Y += -TextPath.Bounds.Y;
				}
			}
			else
			{
				ScaleTransform scale;
				TranslateTransform translate;

				if (ScaleTextPath)
				{
					scale = new ScaleTransform(xScale, yScale);
					translate = new TranslateTransform(-TextPath.Bounds.X * xScale, -TextPath.Bounds.Y * yScale);
				}
				else
				{
					scale = new ScaleTransform(1.0, 1.0);
					translate = new TranslateTransform(-TextPath.Bounds.X, -TextPath.Bounds.Y);
				}

				TransformGroup grp = new TransformGroup();
				grp.Children.Add(scale);
				grp.Children.Add(translate);
				TextPath.Transform = grp;
			}
		}
	}

	public static class GeometryHelper
	{
		public static List<Point> GetIntersectionPoints(PathGeometry FlattenedPath, double[] SegmentLengths)
		{
			List<Point> intersectionPoints = new List<Point>();

			List<Point> pointsOnFlattenedPath = GetPointsOnFlattenedPath(FlattenedPath);

			if (pointsOnFlattenedPath == null || pointsOnFlattenedPath.Count < 2)
				return intersectionPoints;

			Point currPoint = pointsOnFlattenedPath[0];
			intersectionPoints.Add(currPoint);

			// find point on flattened path that is segment length away from current point

			int flattedPathIndex = 0;

			int segmentIndex = 1;

			while (flattedPathIndex < pointsOnFlattenedPath.Count - 1 &&
				segmentIndex < SegmentLengths.Length + 1)
			{
				Point? intersectionPoint = GetIntersectionOfSegmentAndCircle(
					pointsOnFlattenedPath[flattedPathIndex],
					pointsOnFlattenedPath[flattedPathIndex + 1], currPoint, SegmentLengths[segmentIndex - 1]);

				if (intersectionPoint == null)
					flattedPathIndex++;
				else
				{
					intersectionPoints.Add((Point)intersectionPoint);
					currPoint = (Point)intersectionPoint;
					pointsOnFlattenedPath[flattedPathIndex] = currPoint;
					segmentIndex++;
				}
			}

			return intersectionPoints;
		}

		static List<Point> GetPointsOnFlattenedPath(PathGeometry FlattenedPath)
		{
			List<Point> flattenedPathPoints = new List<Point>();

			// for flattened geometry there should be just one PathFigure in the Figures
			if (FlattenedPath.Figures.Count != 1)
				return null;

			PathFigure pathFigure = FlattenedPath.Figures[0];

			flattenedPathPoints.Add(pathFigure.StartPoint);

			// SegmentsCollection should contain PolyLineSegment and LineSegment
			foreach (PathSegment pathSegment in pathFigure.Segments)
			{
				if (pathSegment is PolyLineSegment)
				{
					PolyLineSegment seg = pathSegment as PolyLineSegment;

					foreach (Point point in seg.Points)
						flattenedPathPoints.Add(point);
				}
				else if (pathSegment is LineSegment)
				{
					LineSegment seg = pathSegment as LineSegment;

					flattenedPathPoints.Add(seg.Point);
				}
				else
					throw new Exception("GetIntersectionPoint - unexpected path segment type: " + pathSegment.ToString());

			}

			return (flattenedPathPoints);
		}

		static Point? GetIntersectionOfSegmentAndCircle(Point SegmentPoint1, Point SegmentPoint2,
			Point CircleCenter, double CircleRadius)
		{
			// linear equation for segment: y = mx + b
			double slope = (SegmentPoint2.Y - SegmentPoint1.Y) / (SegmentPoint2.X - SegmentPoint1.X);
			double intercept = SegmentPoint1.Y - (slope * SegmentPoint1.X);

			// special case when segment is vertically oriented
			if (double.IsInfinity(slope))
			{
				double root = Math.Pow(CircleRadius, 2.0) - Math.Pow(SegmentPoint1.X - CircleCenter.X, 2.0);

				if (root < 0)
					return null;

				// soln 1
				double SolnX1 = SegmentPoint1.X;
				double SolnY1 = CircleCenter.Y - Math.Sqrt(root);
				Point Soln1 = new Point(SolnX1, SolnY1);

				// have valid result if point is between two segment points
				if (IsBetween(SolnX1, SegmentPoint1.X, SegmentPoint2.X) &&
					IsBetween(SolnY1, SegmentPoint1.Y, SegmentPoint2.Y))
				//if (ValidSoln(Soln1, SegmentPoint1, SegmentPoint2, CircleCenter))
				{
					// found solution
					return (Soln1);
				}

				// soln 2
				double SolnX2 = SegmentPoint1.X;
				double SolnY2 = CircleCenter.Y + Math.Sqrt(root);
				Point Soln2 = new Point(SolnX2, SolnY2);

				// have valid result if point is between two segment points
				if (IsBetween(SolnX2, SegmentPoint1.X, SegmentPoint2.X) &&
					IsBetween(SolnY2, SegmentPoint1.Y, SegmentPoint2.Y))
				//if (ValidSoln(Soln2, SegmentPoint1, SegmentPoint2, CircleCenter))
				{
					// found solution
					return (Soln2);
				}
			}
			else
			{
				// use soln to quadradratic equation to solve intersection of segment and circle:
				// x = (-b +/ sqrt(b^2-4ac))/(2a)
				double a = 1 + Math.Pow(slope, 2.0);
				double b = (-2 * CircleCenter.X) + (2 * (intercept - CircleCenter.Y) * slope);
				double c = Math.Pow(CircleCenter.X, 2.0) + Math.Pow(intercept - CircleCenter.Y, 2.0) - Math.Pow(CircleRadius, 2.0);

				// check for no solutions, is sqrt negative?
				double root = Math.Pow(b, 2.0) - (4 * a * c);

				if (root < 0)
					return null;

				// we might have two solns...

				// soln 1
				double SolnX1 = (-b + Math.Sqrt(root)) / (2 * a);
				double SolnY1 = slope * SolnX1 + intercept;
				Point Soln1 = new Point(SolnX1, SolnY1);

				// have valid result if point is between two segment points
				if (IsBetween(SolnX1, SegmentPoint1.X, SegmentPoint2.X) &&
					IsBetween(SolnY1, SegmentPoint1.Y, SegmentPoint2.Y))
				//if (ValidSoln(Soln1, SegmentPoint1, SegmentPoint2, CircleCenter))
				{
					// found solution
					return (Soln1);
				}

				// soln 2
				double SolnX2 = (-b - Math.Sqrt(root)) / (2 * a);
				double SolnY2 = slope * SolnX2 + intercept;
				Point Soln2 = new Point(SolnX2, SolnY2);

				// have valid result if point is between two segment points
				if (IsBetween(SolnX2, SegmentPoint1.X, SegmentPoint2.X) &&
					IsBetween(SolnY2, SegmentPoint1.Y, SegmentPoint2.Y))
				//if (ValidSoln(Soln2, SegmentPoint1, SegmentPoint2, CircleCenter))
				{
					// found solution
					return (Soln2);
				}
			}

			// shouldn't get here...but in case
			return null;
		}

		static bool IsBetween(double X, double X1, double X2)
		{
			if (X1 >= X2 && X <= X1 && X >= X2)
				return true;

			if (X1 <= X2 && X >= X1 && X <= X2)
				return true;

			return false;
		}
	}
}
