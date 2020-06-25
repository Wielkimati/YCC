using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace YeelightControlCenter.Twitch
{
	class TwitchBot : MainWindow
	{
		TwitchClient _client;
		ConnectionCredentials _credentials;
		private string _username;
		private string _channelName;
		private string _oAuthKey;

		public TwitchBot(string nickname, string channelName, string key)
		{
			_username = nickname;
			_channelName = channelName;
			_oAuthKey = key;

			Bot();
		}

		private void Bot()
		{
			_credentials = new ConnectionCredentials(_username, _oAuthKey);
			_client = new TwitchClient();
			_client.Initialize(_credentials, _channelName);

			_client.OnConnected += _client_OnConnected;
		}

		#region event Methods
		
		private void _client_OnConnected(object sender, OnConnectedArgs e)
		{
			DevTwitchOutputTextBlock.Text = "Successfully conneted, lmao";
		}

		#endregion
	}
}
