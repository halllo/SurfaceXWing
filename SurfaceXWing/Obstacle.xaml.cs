using System.Windows;
using System.Windows.Controls;

namespace SurfaceXWing
{
	public partial class Obstacle
	{
		public Obstacle()
		{
			InitializeComponent();
		}

		public Point Position
		{
			get { return new Point((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty)); }
			set { SetValue(Canvas.LeftProperty, value.X); SetValue(Canvas.TopProperty, value.Y); }
		}

		public Vector Size
		{
			get { return new Vector(ActualWidth, ActualHeight); }
		}
	}
}
