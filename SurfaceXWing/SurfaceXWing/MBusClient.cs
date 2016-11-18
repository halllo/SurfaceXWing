using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SurfaceXWing
{
	public class MBusClient : IDisposable
	{
		string _clientname;

		HubConnection _connection;
		IHubProxy _hubProxy;

		public MBusClient(string clientname)
		{
			_clientname = clientname;
		}

		public Task Connect(string uri)
		{
			var connection = new HubConnection(uri);
			connection.Closed += () =>
			{
				var eh = OnDisconnect;
				if (eh != null) eh();
			};
			var hubProxy = connection.CreateHubProxy("MyHub");
			hubProxy.On<string, string>("AddMessage", (userName, message) =>
			{
				var eh = On;
				if (eh != null) eh(userName, message);
			});

			_connection = connection;
			_hubProxy = hubProxy;

			var task = connection.Start();
			return task;
		}

		public string ConnectionId
		{
			get { return _connection.ConnectionId; }
		}

		public void Dispose()
		{
			_connection.Dispose();
		}

		public void Emit(string message)
		{
			_hubProxy.Invoke("Send", _clientname, message);
		}

		public event Action<string, string> On;
		public event Action OnDisconnect;
	}
}
