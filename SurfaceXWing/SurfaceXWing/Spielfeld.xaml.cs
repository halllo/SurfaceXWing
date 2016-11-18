using System;

namespace SurfaceXWing
{
	public partial class Spielfeld
	{
		Game _Spiel;
		RemoteGame _Remote;

		public Spielfeld()
		{
			InitializeComponent();
			
			SizeChanged += (s, e) => Setup();
			Loaded += (s, e) =>
			{
				Setup();
				_Remote = new RemoteGame();
				_Remote.ConnectToMBus();
			};
		}

		private void Setup()
		{
			centerText.Text = "(" + ActualWidth + ", " + ActualHeight + ")";


			environmentBuilder.ViewModel.Setup(this, fieldsContainer);

			_Spiel = new Game(this, fieldsContainer);
			_Spiel.Start();

			TagManagement.Instance.Value.TagRegistered += tag => _Spiel.TagIntroduce(tag.Visual);
			TagManagement.Instance.Value.TagUnregistered += tag => _Spiel.TagDismiss(tag.Visual);

			var h = GameStarted;
			if (h != null) h(_Spiel);
		}

		public event Action<Game> GameStarted;
	}










	public class RemoteGame
	{
		MBusClient _MBus = new MBusClient("SurfaceXWing");

		public void ConnectToMBus(string url = "http://mbusrelay.azurewebsites.net/signalr")
		{
			_MBus.OnDisconnect += Mbus_OnDisconnect;
			_MBus.On += Mbus_On;
			try
			{
				Log("connecting to " + url);
				_MBus.Connect(url).Wait();
				Log("connected");
				_MBus.Emit("hallo");
			}
			catch (AggregateException e)
			{
				Log("cannot connect: " + e.InnerException.Message);
			}
		}

		private void Log(string log)
		{
			System.Diagnostics.Debug.WriteLine(log);
		}

		private void Mbus_OnDisconnect()
		{
			Log("disconnect");
		}

		private void Mbus_On(string clientname, string message)
		{
			if (clientname.StartsWith("SurfaceXWing.CompanionApp"))
			{
				var messageItems = message.Split(new[] { ";" }, StringSplitOptions.None);
				if (messageItems[0] == "refresh")
				{
					Log(clientname + ": " + message);
				}
				else if (messageItems[0] == "move")
				{
					Log(clientname + ": " + message);
				}
			}
		}
	}
}
