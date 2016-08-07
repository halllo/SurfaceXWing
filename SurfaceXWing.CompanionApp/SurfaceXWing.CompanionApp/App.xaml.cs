using JustObjectsPrototype.Universal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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
				Squadron.Download("http://xwing-builder.co.uk/view/541496/first-try")
			};

			Show.Prototype(With.These(objects)
				.AndViewOf<Squadron>()
				.AndViewOf<Pilot>());
		}
	}

	public class Squadron
	{
		public string Name { get; private set; }
		public string Url { get; private set; }

		[JustObjectsPrototype.Universal.JOP.Editor(hide: true)]
		public string Downloaded { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public static Squadron Download(string url)
		{
			var name = url.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
			var downloaded = AsyncHelpers.RunSync(() => Download(new Uri(url)));

			return new Squadron { Name = name, Url = url, Downloaded = downloaded };
		}

		public void Aktivieren(ObservableCollection<Pilot> piloten)
		{
			try
			{
				var matches = Regex.Matches(Downloaded, "pilot_card_container pilot_body_cell((.|\n|\r)*?)<\\/table>");
				var pilotMatches = matches.OfType<Match>().Select(m => new
				{
					images = Regex.Matches(m.Value, "data-card-src=\"(.*?)\"").OfType<Match>(),
					titles = Regex.Matches(m.Value, "title=\"(.*?)\"").OfType<Match>(),
					manoeuvers = Regex.Matches(m.Value, "speed\">((.|\n|\r)*?)<\\/tr>").OfType<Match>(),
				});

				var pilots = pilotMatches.Select(p => new Pilot
				{
					Name = p.titles.First().Groups[1].Value,
					Image = p.images.First().Groups[1].Value,
					Upgrades = p.titles.Skip(1).Zip(p.images.Skip(1), (t, i) => new { title = t, image = i })
							.Select(u => new Upgrade { Image = u.image.Groups[1].Value, Name = u.title.Groups[1].Value })
							.ToList(),
					Manoeuvers = new Manoeuvers
					{
						Grid = p.manoeuvers.Select(m => Regex.Matches(m.Value, "<td class=\"manoeuvre\">((.|\n|\r)*?)<\\/td>").OfType<Match>()
							.Select(row =>
							{
								var match = Regex.Match(row.Value, "<img src=\"((.|\n|\r)*?)\">");
								return match.Success ? match.Groups[1].Value : null;
							}).ToArray()
							).ToArray()
					},
				}).ToList();


				piloten.Clear();
				pilots.ForEach(piloten.Add);
			}
			catch (Exception e)
			{
				//todo: message box
			}
		}

		private async static Task<string> Download(Uri url)
		{
			var httpClient = new Windows.Web.Http.HttpClient();
			return await httpClient.GetStringAsync(url);
		}
	}

	public class Pilot
	{
		public string Image { get; set; }
		public string Name { get; set; }
		public Manoeuvers Manoeuvers { get; set; }
		public List<Upgrade> Upgrades { get; set; }
	}

	public class Upgrade
	{
		public string Image { get; set; }
		public string Name { get; set; }
	}

	public class Manoeuvers
	{
		public string[][] Grid { get; set; }
	}
}
