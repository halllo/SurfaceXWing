using SurfaceGameBasics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfaceXWing
{
	public partial class EnvironmentBuilder
	{
		public EnvironmentBuilderModel ViewModel { get; private set; }

		public EnvironmentBuilder()
		{
			InitializeComponent();

			DataContext = ViewModel = new EnvironmentBuilderModel(this);
		}
	}

	public class EnvironmentBuilderModel : ViewModel
	{
		ScatterViewItemFieldObject _View;
		FieldsView _FieldsView;
		Canvas _Spielfeld;

		public EnvironmentBuilderModel(ScatterViewItemFieldObject view)
		{
			_View = view;
			Asteroid = new Command(() => PlaceObstacle().Asteroids.Visibility = Visibility.Visible);
			Debris = new Command(() => PlaceObstacle().Debris.Visibility = Visibility.Visible);
			Remove = new Command(RemovePlaced);
		}

		public void Setup(FieldsView fieldsView, Canvas spielfeld)
		{
			_FieldsView = fieldsView;
			_Spielfeld = spielfeld;
		}

		Command _Asteroid;
		public Command Asteroid
		{
			get { return _Asteroid; }
			set { _Asteroid = value; NotifyChanged("Asteroid"); }
		}

		Command _Debris;
		public Command Debris
		{
			get { return _Debris; }
			set { _Debris = value; NotifyChanged("Debris"); }
		}

		Command _Remove;
		public Command Remove
		{
			get { return _Remove; }
			set { _Remove = value; NotifyChanged("Remove"); }
		}

		private Obstacle PlaceObstacle()
		{
			var builderPosition = _View.Position.AsVector();
			var builderSize = new Vector(_View.ActualWidth, _View.ActualHeight);
			var builderSizeHalbe = builderSize / 2.0;
			var obstaclePosition = builderPosition - builderSizeHalbe;

			var obstacle = new Obstacle
			{
				Width = _View.ActualWidth,
				Height = _View.ActualHeight
			};
			obstacle.Position = obstaclePosition.AsPoint();
			obstacle.RenderTransform = new RotateTransform
			{
				CenterX = builderSizeHalbe.X,
				CenterY = builderSizeHalbe.Y,
				Angle = _View.OrientationAngle
			};

			_Spielfeld.Children.Add(obstacle);

			return obstacle;
		}

		private void RemovePlaced()
		{
			var builderPosition = _View.Position.AsVector();
			var builderSize = new Vector(_View.ActualWidth, _View.ActualHeight);
			var builderSizeHalbeLengthSquared = (builderSize / 2.0).LengthSquared;

			var obstacles = _Spielfeld.Children.OfType<Obstacle>();

			var obstaclesInRange = obstacles.Where(o =>
			{
				var obstaclePosition = o.Position.AsVector() + o.Size / 2.0;
				return (builderPosition - obstaclePosition).LengthSquared < builderSizeHalbeLengthSquared;
			}).ToList();

			obstaclesInRange.ForEach(_Spielfeld.Children.Remove);
		}
	}
}
