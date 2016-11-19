using System;
using System.Linq;
using System.Windows.Controls;

namespace SurfaceXWing
{
	public class RemoteGame
	{
		MBusClient _MBus = new MBusClient("SurfaceXWing");

		public Game Spiel { get; set; }
		public Canvas FieldsContainer { get; set; }

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
					try
					{
						var id = long.Parse(messageItems[1]);
						var schilde = int.Parse(messageItems[2]);
						var huelle = int.Parse(messageItems[3]);
						var schaden = int.Parse(messageItems[4]);
						var ausweichen = int.Parse(messageItems[5]);
						var fokus = int.Parse(messageItems[6]);
						var stress = int.Parse(messageItems[7]);

						if (TagManagement.Instance.Value.Tags.ContainsKey(id))
						{
							FieldsContainer.Dispatcher.BeginInvoke(new Action(() =>
							{
								var tagData = TagManagement.Instance.Value.Tags[id];
								tagData.Tokens.Schild = schilde;
								tagData.Tokens.Huelle = huelle;
								tagData.Tokens.Schaden = schaden;
								tagData.Tokens.Ausweichen = ausweichen;
								tagData.Tokens.Fokus = fokus;
								tagData.Tokens.Stress = stress;

								var schiffsposition = FieldsContainer.Children.OfType<Schiffsposition>().Where(p => p.Opacity == 1.0 && p.AllowedOccupantId == id.ToString()).FirstOrDefault();
								if (schiffsposition != null && schiffsposition.ViewModel.Cancel != null)
								{
									schiffsposition.ViewModel.Cancel.Execute(null);
								}
							}));
						}
					}
					catch (Exception)
					{ }
				}
				else if (messageItems[0] == "move")
				{
					Log(clientname + ": " + message);
					try
					{
						var id = long.Parse(messageItems[1]);
						var speed = int.Parse(messageItems[2]);
						var move = messageItems[3];

						if (TagManagement.Instance.Value.Tags.ContainsKey(id))
						{
							FieldsContainer.Dispatcher.BeginInvoke(new Action(() =>
							{
								var schiffsposition = FieldsContainer.Children.OfType<Schiffsposition>().Where(p => p.Opacity == 1.0 && p.AllowedOccupantId == id.ToString()).FirstOrDefault();
								if (schiffsposition != null)
								{
									if (move.Contains("schräglinks")
										|| move.Contains("schrägrechts")
										|| move.Contains("scharfrechts")
										|| move.Contains("scharflinks")
										|| move.Contains("gradeaus")
										|| move.Contains("wende"))
										schiffsposition.ViewModel.Forward.Execute(speed + move);
									else if (move.Contains("rollen"))
										schiffsposition.ViewModel.BarrelRoll.Execute(speed + move);
									else if (move.Contains("TODO: besondere 3er wende"))
										schiffsposition.ViewModel.Slide3.Execute(speed + move);
								}
							}));
						}
					}
					catch (Exception)
					{ }
				}
			}
		}
	}
}
