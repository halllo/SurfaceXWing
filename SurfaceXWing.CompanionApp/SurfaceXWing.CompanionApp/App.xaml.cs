using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using JustObjectsPrototype.Universal;
using JustObjectsPrototype.Universal.JOP;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SurfaceXWing.CompanionApp
{
	sealed partial class App : Application
	{
		public App()
		{
			this.InitializeComponent();
			Suspending += App_Suspending;
		}

		private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
		{
			var ships = Prototype.Repository.OfType<Ship>().ToList();
			Schiffspeicher.File<List<Ship>>("alle").SaveOrUpdateSync(ships);
		}

		protected async override void OnLaunched(LaunchActivatedEventArgs e)
		{
			List<Ship> schiffe = new List<Ship>();
			try { schiffe = await Schiffspeicher.File<List<Ship>>("alle").Read(); }
			catch (Exception) { await Schiffspeicher.DeleteAll(); }


			Prototype = Show.Prototype(With.These(await Squadronspeicher.All(), schiffe, await Einstellungen.Alle())
				.AndViewOf<Squadron>()
				.AndViewOf<Pilot>()
				.AndViewOf<Ship>()
				.AndViewOf<Einstellungen>()
				.AndOpen<Squadron>());
		}

		public static Prototype Prototype;
		public static Squadronspeicher Squadronspeicher = new Squadronspeicher();
		public static Store Schiffspeicher = new Store("ships");
	}





























	[Title("Squadrons"), Icon(Symbol.Favorite)]
	public class Squadron
	{
		private const string XWingBuilderBaseUrl = "http://xwing-builder.co.uk";

		[Editor(@readonly: true)]
		public string Name { get; set; }
		[Editor(@readonly: true)]
		public string Url { get; set; }
		[Editor(hide: true)]
		public string Downloaded { get; set; }

		public override string ToString()
		{
			return Name;
		}

		[Icon(Symbol.Download), JumpsToResult, WithProgressBar]
		public static async Task<Squadron> Download([Title("URL eingeben (http://xwing-builder.co.uk/view/...)")]string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				try
				{
					var name = url.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
					var downloaded = await Download(new Uri(url));
					var newSquadron = new Squadron { Name = name, Url = url, Downloaded = downloaded };

					await App.Squadronspeicher.Store(newSquadron);
					return newSquadron;
				}
				catch (Exception e)
				{
					await new MessageDialog(e.Message, "Fehler").ShowAsync();
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		async static Task<string> Download(Uri url)
		{
			var httpClient = new Windows.Web.Http.HttpClient();
			return await httpClient.GetStringAsync(url);
		}

		[Icon(Symbol.Play), JumpsToResult, WithProgressBar]
		public async Task<List<Pilot>> Aktivieren(ObservableCollection<Pilot> piloten)
		{
			try
			{
				var parsedPilots = Parse();
				if (!parsedPilots.Any())
				{
					throw new ArgumentException("Keine Piloten in diesem Squadron.");
				}

				piloten.Clear();

				return parsedPilots;
			}
			catch (Exception e)
			{
				await new MessageDialog(e.Message, "Fehler").ShowAsync();
				return null;
			}
		}

		private List<Pilot> Parse()
		{
			var matches = Regex.Matches(Downloaded, "pilot_card_container pilot_body_cell((.|\n|\r)*?)<\\/table>");
			var pilotMatches = matches.OfType<Match>().Select(m => new
			{
				image = Regex.Matches(m.Value, "data-card-src=\"(.*?)\"").OfType<Match>().First(),
				title = Regex.Matches(m.Value, "title=\"(.*?)\"").OfType<Match>().First(),
				description = Regex.Matches(m.Value, "data-card-description=\"(.*?)\"").OfType<Match>().First(),
				manoeuvers = Regex.Matches(m.Value, "speed\">((.|\n|\r)*?)<\\/tr>").OfType<Match>(),
				upgrades = Regex.Matches(m.Value, "card upgrade_card((.|\n|\r)*?)>").OfType<Match>().Select(upgrade => new
				{
					image = Regex.Matches(upgrade.Value, "data-card-src=\"(.*?)\"").OfType<Match>().First(),
					title = Regex.Matches(upgrade.Value, "title=\"(.*?)\"").OfType<Match>().First(),
					description = Regex.Matches(upgrade.Value, "data-card-description=\"(.*?)\"").OfType<Match>().First(),
				}),
			});

			var pilots = pilotMatches.Select(p => new Pilot
			{
				Name = p.title.Groups[1].Value,
				Bild = XWingBuilderBaseUrl + p.image.Groups[1].Value,
				Upgrades = p.upgrades
					.Select(upgrade => new Upgrade
					{
						Bild = XWingBuilderBaseUrl + upgrade.image.Groups[1].Value,
						Name = upgrade.title.Groups[1].Value,
						Beschreibung = Regex.Replace(upgrade.description.Groups[1].Value, "&lt;(.*?)&gt;", new MatchEvaluator(m => "@"))
					})
					.ToList(),
				Manoeuvers = new Manoeuvers
				{
					Grid = p.manoeuvers.Select(m => new Manoeuvers.Row
					{
						Speed = int.Parse(Regex.Matches(m.Value, "speed\">(.*?)<\\/td>").OfType<Match>().FirstOrDefault()?.Groups[1].Value ?? "-1"),
						Columns = Regex.Matches(m.Value, "<td class=\"manoeuvre\">((.|\n|\r)*?)<\\/td>").OfType<Match>()
							.Select(column =>
							{
								var match = Regex.Match(column.Value, "<img src=\"((.|\n|\r)*?)\">");
								return match.Success ? XWingBuilderBaseUrl + match.Groups[1].Value : null;
							}).ToArray()
					}).ToArray()
				},
			}).ToList();
			return pilots;
		}

		[Icon(Symbol.Delete), RequiresConfirmation]
		public async Task Löschen(ObservableCollection<Squadron> squadrons)
		{
			await App.Squadronspeicher.Remove(this);
			squadrons.Remove(this);
		}
	}


















	[Title("Schiffe"), Icon(Symbol.Send), CustomView("ShipListItem")]
	public class Ship
	{
		[Editor(@readonly: true)]
		public string Pilot { get; set; }
		[Title("Schiff-ID")]
		public int SchiffId { get; set; }

		[Icon(Symbol.Add)]
		public static Ship Neu(Pilot pilot, int id)
		{
			return new Ship { Pilot = pilot.Name, SchiffId = id };
		}

		[Icon(Symbol.Delete), RequiresConfirmation]
		public void Löschen(ObservableCollection<Ship> ships)
		{
			ships.Remove(this);
		}
	}


















	[Title("aktive Piloten"), Icon(Symbol.People), CustomView("PilotListItem")]
	public class Pilot
	{
		[Editor(hide: true)]
		public string Name { get; set; }

		[Editor(hide: true)]
		public string Id { get { return App.Prototype.Repository.OfType<Ship>().Where(s => s.Pilot == Name).FirstOrDefault()?.SchiffId.ToString(); } }
		[Editor(hide: true)]
		public string IdDesc { get { return Id != null ? "in  Schiff " + Id : ""; } }

		[CustomView("ImageDisplay")]
		public string Bild { get; set; }

		[CustomView("NumericUpDownDisplay")]
		public int Schilde { get; set; }
		[CustomView("NumericUpDownDisplay")]
		public int Hülle { get; set; }
		[CustomView("NumericUpDownDisplay")]
		public int Schaden { get; set; }
		[CustomView("NumericUpDownDisplay")]
		public int Ausweichen { get; set; }
		[CustomView("NumericUpDownDisplay")]
		public int Fokus { get; set; }
		[CustomView("NumericUpDownDisplay")]
		public int Stress { get; set; }

		public List<int> Zielerfassungen { get; set; }

		[CustomView("ManoeuversDisplay")]
		public Manoeuvers Manoeuvers { get; set; }

		[CustomView("ImagesDisplay")]
		public List<Upgrade> Upgrades { get; set; }

		public override string ToString()
		{
			return Name;
		}

		[Icon(Symbol.Sync), WithProgressBar]
		public async void Aktualisieren()
		{
			await EmitOnMBus($"refresh;{Id};{Schilde};{Hülle};{Schaden};{Ausweichen};{Fokus};{Stress};{string.Join(",", Zielerfassungen ?? Enumerable.Empty<int>())}");
		}

		[Icon(Symbol.Send), WithProgressBar]
		public async void Fliegen()
		{
			if (Manoeuvers.SelectedMove != null)
			{
				await EmitOnMBus($"move;{Id};{Manoeuvers.SelectedSpeed};{Manoeuvers.AusgewähltesManöver}");
			}
		}

		[Icon(Symbol.Rotate), WithProgressBar]
		public async void Rollen()
		{
			await EmitOnMBus($"move;{Id};0;rollen");
		}

		private static async Task EmitOnMBus(string content)
		{
			var sender = "SurfaceXWing.CompanionApp[" + App.Prototype.Repository.OfType<Einstellungen.AllgemeineEinstellungen>().Single().Username + "]";

			var httpClient = new HttpClient();
			var response = await httpClient.PostAsync(
				new Uri("https://mbusrelay.azurewebsites.net/api/mbus"),
				new StringContent("{sender:'" + sender + "',content:'" + content + "'}", Encoding.UTF8, "application/json"));

			if (!response.IsSuccessStatusCode)
			{
				await new MessageDialog(response.StatusCode.ToString() + "\n" + await response.Content.ReadAsStringAsync()).ShowAsync();
			}
		}
	}

	public class Upgrade
	{
		public string Bild { get; set; }
		public string Name { get; set; }
		public string Beschreibung { get; set; }
	}









	public class Manoeuvers : JustObjectsPrototype.Universal.Shell.ViewModel
	{
		public class Row
		{
			public int Speed { get; set; }
			public string[] Columns { get; set; }
		}

		public Row[] Grid
		{
			set
			{
				GridVM = value.Select(r => r.Columns.Select(c =>
				{
					return new MoveViewModel
					{
						Url = c,
						Choose = new Command(
							execute: p =>
							{
								SelectedMove = c;
								SelectedSpeed = r.Speed;
							},
							canExecute: p => !string.IsNullOrEmpty(c))
					};
				}).ToArray()).ToArray();
			}
		}

		public MoveViewModel[][] GridVM { get; private set; }

		string selectedMove; public string SelectedMove { get { return selectedMove; } set { selectedMove = value; Changed(); } }
		int selectedSpeed; public int SelectedSpeed { get { return selectedSpeed; } set { selectedSpeed = value; Changed(); } }

		public string AusgewähltesManöver
		{
			get
			{
				var move = SelectedMove.Split(new[] { '/' }).Last();
				if (move.Contains("forward")) return "gradeaus";
				if (move.Contains("koiogram")) return "wende";
				if (move.Contains("bank_left")) return "schräglinks";
				if (move.Contains("turn_left")) return "scharflinks";
				if (move.Contains("segnors_loop_left")) return "schrägewendelinks";
				if (move.Contains("bank_right")) return "schrägrechts";
				if (move.Contains("turn_right")) return "scharfrechts";
				if (move.Contains("segnors_loop_right")) return "schrägewenderechts";
				return string.Empty;
			}
		}

		public class MoveViewModel
		{
			public string Url { get; set; }
			public ICommand Choose { get; set; }
		}

		public class Command : ICommand
		{
			Action<object> execute;
			Func<object, bool> canExecute;

			public Command(Action<object> execute, Func<object, bool> canExecute)
			{
				this.canExecute = canExecute;
				this.execute = execute;
			}

			public event EventHandler CanExecuteChanged;

			public bool CanExecute(object parameter)
			{
				return canExecute(parameter);
			}

			public void Execute(object parameter)
			{
				execute(parameter);
			}
		}
	}


















	[Icon(Symbol.Setting)]
	public abstract class Einstellungen
	{
		internal static async Task<Einstellungen[]> Alle()
		{
			var users = await User.FindAllAsync();
			var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated && p.Type == UserType.LocalUser).FirstOrDefault();

			var data = await current.GetPropertyAsync(KnownUserProperties.AccountName);
			string displayName = (string)data;

			if (string.IsNullOrEmpty(displayName))
			{
				string a = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
				string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);
				displayName = string.Format("{0} {1}", a, b);
			}

			return new Einstellungen[]
			{
				new AllgemeineEinstellungen { Username = displayName },
				new SpeicherEinstellungen()
			};
		}

		class SpeicherEinstellungen : Einstellungen
		{
			public override string ToString() => "Speicher";

			public IEnumerable<object> Gespeicherte_Objekte => App.Prototype.Repository.Where(o => !(o is Einstellungen));

			[Icon(Symbol.Delete)]
			public async Task Alles_Löschen()
			{
				await App.Squadronspeicher.RemoveAll();

				var allesAußerEinstellungen = App.Prototype.Repository.Where(o => !(o is Einstellungen)).ToList();
				allesAußerEinstellungen.ForEach(o => App.Prototype.Repository.Remove(o));
			}
		}

		public class AllgemeineEinstellungen : Einstellungen
		{
			public override string ToString() => "Allgemein";

			[Editor(@readonly: true)]
			public string Username { get; set; }

			[CustomView("ShipColors")]
			public int Farben { get; set; }
		}
	}





































	public class Squadronspeicher : Store
	{
		public Squadronspeicher() : base("squadrons")
		{
		}

		public async Task Store(Squadron squadron)
		{
			var filecontent = new StringBuilder();
			using (var writer = new StringWriter(filecontent))
			{
				writer.WriteLine(squadron.Name);
				writer.WriteLine(squadron.Url);
				writer.Write(squadron.Downloaded);
				writer.Flush();
			}
			await SaveOrUpdate(squadron.Name, filecontent.ToString(), Windows.Storage.CreationCollisionOption.FailIfExists);
		}

		public async Task Remove(Squadron squadron)
		{
			await Delete(squadron.Name);
		}

		public async Task RemoveAll()
		{
			await DeleteAll();
		}

		public async Task<List<Squadron>> All()
		{
			var result = new List<Squadron>();
			var filenames = await Browse();
			foreach (var filename in filenames)
			{
				var file = await Get(filename);
				using (var reader = new StringReader(file))
				{
					result.Add(new Squadron
					{
						Name = reader.ReadLine(),
						Url = reader.ReadLine(),
						Downloaded = reader.ReadToEnd(),
					});
				}
			}
			return result;
		}
	}
}
