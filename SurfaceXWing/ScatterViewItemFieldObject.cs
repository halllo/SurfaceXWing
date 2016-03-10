using Microsoft.Surface.Presentation.Controls;
using SurfaceGameBasics;
using System.Windows;

namespace SurfaceXWing
{
	public class ScatterViewItemFieldObject : ScatterViewItem, IFieldOccupant
	{
		public Point Position { get { return Center; } }
		public double OrientationAngle { get { return Orientation; } }

		string IFieldOccupant.Id
		{
			get { return Tag.ToString(); }
			set { Tag = value; }
		}
	}
}
