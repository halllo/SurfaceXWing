using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using JustObjectsPrototype.Universal;
using JustObjectsPrototype.Universal.JOP;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
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
		}

		protected async override void OnLaunched(LaunchActivatedEventArgs e)
		{
			Prototype = Show.Prototype(With.These(await Squadronspeicher.All(), await Einstellungen.Alle())
				.AndViewOf<Squadron>()
				.AndViewOf<Pilot>()
				.AndViewOf<Einstellungen>()
				.AndOpen<Squadron>());
		}

		public static Prototype Prototype;
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

		[Icon(Symbol.Download), JumpsToResult]
		public static async Task<Squadron> Download([Title("URL eingeben (http://xwing-builder.co.uk/view/...)")]string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				var name = url.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
				var downloaded = await Download(new Uri(url));
				var newSquadron = new Squadron { Name = name, Url = url, Downloaded = downloaded };

				try
				{
					await Squadronspeicher.Store(newSquadron);
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

		[Icon(Symbol.Play), JumpsToResult]
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
				images = Regex.Matches(m.Value, "data-card-src=\"(.*?)\"").OfType<Match>(),
				titles = Regex.Matches(m.Value, "title=\"(.*?)\"").OfType<Match>(),
				manoeuvers = Regex.Matches(m.Value, "speed\">((.|\n|\r)*?)<\\/tr>").OfType<Match>(),
			});

			var pilots = pilotMatches.Select(p => new Pilot
			{
				Name = p.titles.First().Groups[1].Value,
				Bild = XWingBuilderBaseUrl + p.images.First().Groups[1].Value,
				Upgrades = p.titles.Skip(1).Zip(p.images.Skip(1), (t, i) => new { title = t, image = i })
						.Select(u => new Upgrade { Bild = XWingBuilderBaseUrl + u.image.Groups[1].Value, Name = u.title.Groups[1].Value })
						.ToList(),
				Manoeuvers = new Manoeuvers
				{
					Grid = p.manoeuvers.Select(m => Regex.Matches(m.Value, "<td class=\"manoeuvre\">((.|\n|\r)*?)<\\/td>").OfType<Match>()
						.Select(row =>
						{
							var match = Regex.Match(row.Value, "<img src=\"((.|\n|\r)*?)\">");
							return match.Success ? XWingBuilderBaseUrl + match.Groups[1].Value : null;
						}).ToArray()
						).ToArray()
				},
			}).ToList();
			return pilots;
		}

		[Icon(Symbol.Delete)]
		public async Task Löschen(ObservableCollection<Squadron> squadrons)
		{
			var löschDialog = new MessageDialog($"Suqadron \"{Name}\" löschen?");
			löschDialog.Options = MessageDialogOptions.None;
			löschDialog.Commands.Add(new UICommand("Ja", async cmd =>
			{
				await Squadronspeicher.Remove(this);
				squadrons.Remove(this);
			}));
			löschDialog.Commands.Add(new UICommand("Nein", cmd => { }));
			löschDialog.CancelCommandIndex = 1;
			löschDialog.DefaultCommandIndex = 0;

			await löschDialog.ShowAsync();
		}
	}

















	[Title("aktive Piloten"), Icon(Symbol.People)]
	public class Pilot
	{
		[Editor(hide: true)]
		public string Name { get; set; }

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

		[Icon(Symbol.Sync)]
		public async void Aktualisieren()
		{
			await new MessageDialog("Aktualisieren").ShowAsync();
		}

		[Icon(Symbol.Send)]
		public async void Fliegen()
		{
			await new MessageDialog("Fliegen").ShowAsync();
		}
	}

	public class Upgrade
	{
		public string Bild { get; set; }
		public string Name { get; set; }
	}
















	public class Manoeuvers : JustObjectsPrototype.Universal.Shell.ViewModel
	{
		public string[][] Grid
		{
			set
			{
				GridVM = value.Select(r => r.Select(c =>
				{
					return new MoveViewModel
					{
						Url = c,
						Choose = new Command(
							execute: p => SelectedMove = c,
							canExecute: p => !string.IsNullOrEmpty(c))
					};
				}).ToArray()).ToArray();
			}
		}

		public MoveViewModel[][] GridVM { get; private set; }

		string selectedMove;
		public string SelectedMove
		{
			get { return selectedMove; }
			set { selectedMove = value; Changed(); }
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
				await Squadronspeicher.RemoveAll();

				var allesAußerEinstellungen = App.Prototype.Repository.Where(o => !(o is Einstellungen)).ToList();
				allesAußerEinstellungen.ForEach(o => App.Prototype.Repository.Remove(o));
			}
		}

		class AllgemeineEinstellungen : Einstellungen
		{
			public override string ToString() => "Allgemein";

			public bool Tisch_verbunden { get; } = false;

			[Editor(@readonly: true)]
			public string Username { get; set; }
		}
	}







































	public static class Squadronspeicher
	{
		public static async Task Store(Squadron squadron)
		{
			var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(squadron.Name, CreationCollisionOption.FailIfExists);
			using (var openedFile = await file.OpenAsync(FileAccessMode.ReadWrite))
			using (var openedFileWriter = new StreamWriter(openedFile.AsStreamForWrite()))
			{
				await openedFileWriter.WriteLineAsync(squadron.Url);
				await openedFileWriter.WriteAsync(squadron.Downloaded);
				await openedFileWriter.FlushAsync();
			}
		}

		public static async Task Remove(Squadron squadron)
		{
			var file = await ApplicationData.Current.LocalFolder.GetFileAsync(squadron.Name);
			await file.DeleteAsync();
		}

		public static async Task RemoveAll()
		{
			var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
			foreach (var file in files)
			{
				await file.DeleteAsync();
			}
		}

		public static async Task<List<Squadron>> All()
		{
			var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
			var squadrons = new List<Squadron>();
			foreach (var file in files)
			{
				using (var openedFile = await file.OpenAsync(FileAccessMode.Read))
				using (var openedFileReader = new StreamReader(openedFile.AsStreamForRead()))
				{
					var fileFirstLine = await openedFileReader.ReadLineAsync();
					var fileContent = await openedFileReader.ReadToEndAsync();
					squadrons.Add(new Squadron
					{
						Name = file.Name,
						Url = fileFirstLine,
						Downloaded = fileContent,
					});
				}
			}
			return squadrons;
		}
	}
}
