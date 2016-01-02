using System.Windows.Controls;
using System.Windows.Media;

namespace SurfaceXWing
{
	public class Game
	{
		FieldsView _Spielfeld;
		Canvas _FieldsContainer;

		public Game(FieldsView spielfeld, Canvas fieldsContainer)
		{
			_Spielfeld = spielfeld;
			_FieldsContainer = fieldsContainer;
		}

		public void Start()
		{
			_Spielfeld.Register(_FieldsContainer);


			var schiffsposition1 = new Schiffsposition { Text = "links", Color = Brushes.Green };
			var schiffsposition1HeightHalbe = schiffsposition1.Height / 2;
			schiffsposition1.RenderTransform = new RotateTransform { CenterX = schiffsposition1HeightHalbe, CenterY = schiffsposition1HeightHalbe, Angle = 90 };
			schiffsposition1.SetValue(Canvas.LeftProperty, 0.0);
			schiffsposition1.SetValue(Canvas.TopProperty, _FieldsContainer.ActualHeight / 2.0 - schiffsposition1HeightHalbe);

			_FieldsContainer.Children.Add(schiffsposition1);
			_Spielfeld.Register((IField)schiffsposition1);


			var schiffsposition2 = new Schiffsposition { Text = "rechts", Color = Brushes.Red };
			var schiffsposition2HeightHalbe = schiffsposition2.Height / 2;
			schiffsposition2.RenderTransform = new RotateTransform { CenterX = schiffsposition2HeightHalbe, CenterY = schiffsposition2HeightHalbe, Angle = 270 };
			schiffsposition2.SetValue(Canvas.LeftProperty, _FieldsContainer.ActualWidth - schiffsposition2.Height);
			schiffsposition2.SetValue(Canvas.TopProperty, _FieldsContainer.ActualHeight / 2.0 - schiffsposition2HeightHalbe);

			_FieldsContainer.Children.Add(schiffsposition2);
			_Spielfeld.Register((IField)schiffsposition2);
		}

		internal void TagIntroduce(TagVisual visual)
		{
			_Spielfeld.Track(visual);
		}

		internal void TagDismiss(TagVisual visual)
		{
			_Spielfeld.Untrack(visual);
		}
	}
}
