using JustObjectsPrototype.Universal;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SurfaceXWing.CompanionApp
{
	sealed partial class App : Application
	{
		public App()
		{
			this.InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			var objects = new ObservableCollection<object>
			{
			};

			Show.Prototype(With.These(objects)
				.AndViewOf<Squadron>());
		}
	}

	public class Squadron
	{
		public string Name { get; private set; }
		public string Content { get; private set; }

		public override string ToString()
		{
			return Name;
		}

		public static Squadron Download(string url)//todo: http://xwing-builder.co.uk/view/541496/first-try
		{
			try
			{
				var content = AsyncHelpers.RunSync(() => Download(new Uri(url)));
				//todo: parse images (data-card-src=".*?"), manovers

				//todo: remember downloaded squadron
				return new Squadron { Name = url, Content = content.Length.ToString() };
			}
			catch (Exception e)
			{
				//todo: message box
				return null;
			}
		}

		private async static Task<string> Download(Uri url)
		{
			var httpClient = new Windows.Web.Http.HttpClient();
			return await httpClient.GetStringAsync(url);
		}
	}
}
