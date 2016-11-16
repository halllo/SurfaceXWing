using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SurfaceXWing
{
	public partial class TokenCircle
	{
		public TokenCircle()
		{
			InitializeComponent();
		}
	}


	public class SchiffTokens
	{
		public abstract class Token : ViewModel { }

		public class IdToken : Token { string _Value; public string Value { get { return _Value; } set { _Value = value; NotifyChanged("Value"); } } }
		public class HuelleToken : Token { }
		public class SchildToken : Token { }
		public class FokusToken : Token { }
		public class AusweichenToken : Token { }
		public class StressToken : Token { }
		public class SchadenToken : Token { }

		public SchiffTokens()
		{
			All = new ObservableCollection<Token>(new List<Token>
			{
				new IdToken { Value = string.Empty }
			});
		}

		public ObservableCollection<Token> All { get; private set; }

		public string Id { get { return All.OfType<IdToken>().Single().Value; } set { All.OfType<IdToken>().Single().Value = value; } }
		public int Huelle { get { return All.OfType<HuelleToken>().Count(); } set { Set<HuelleToken>(value); } }
		public int Schild { get { return All.OfType<SchildToken>().Count(); } set { Set<SchildToken>(value); } }
		public int Fokus { get { return All.OfType<FokusToken>().Count(); } set { Set<FokusToken>(value); } }
		public int Ausweichen { get { return All.OfType<AusweichenToken>().Count(); } set { Set<AusweichenToken>(value); } }
		public int Stress { get { return All.OfType<StressToken>().Count(); } set { Set<StressToken>(value); } }
		public int Schaden { get { return All.OfType<SchadenToken>().Count(); } set { Set<SchadenToken>(value); } }

		private void Set<T>(int times) where T : Token, new()
		{
			var tokens = All.OfType<T>().ToList();
			foreach (var token in tokens) All.Remove(token);
			for (int i = 0; i < times; i++) All.Add(new T());
		}
	}
}
