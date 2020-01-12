using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using YeelightAPI;
using YeelightControlCenter.Properties;

namespace YeelightControlCenter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Device _device;

		public MainWindow()
		{
			InitializeComponent();

			PowerButton.Click += TogglePower;
			ConnectionButton.Click += ConnectDevices;
			BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
			ColorPicker.SelectedColorChanged += ColorPickerOnSelectedColorChanged;
		}

		#region Main Window Events
		private void MainWindow_OnInitialized(object sender, EventArgs eventArgs)
		{
			ConnectDevices(sender, null);
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			Settings.Default.Save();
		}
		#endregion

		#region Other Events
		private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_device.SetBrightness((int)BrightnessSlider.Value);
		}

		private void ColorPickerOnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			if (ColorPicker.SelectedColor != null)
			{
				var color = ColorPicker.SelectedColor.Value;
				_device.SetRGBColor(color.R, color.G, color.B);
			}
		}
		#endregion

		#region Yeelight Related Methods
		private void TogglePower(object sender, EventArgs eventArgs)
		{
			DevOutputTextBlock.Text = $"{_device.Hostname} Power Toggled!";

			_device.Toggle();
		}

		private void ConnectDevices(object sender, EventArgs eventArgs)
		{
			ConnectDevicesAsync();
		}

		private async void ConnectDevicesAsync()
		{
			DevOutputTextBlock.Text = string.Empty;
			_device = new Device(Settings.Default.LighbulbAddress);

			await _device.Connect();

			var x = _device.IsConnected ? "Connected" : "Connection Failed";
			DevOutputTextBlock.Text += $"Status: {x} == {_device.Hostname}";

			BrightnessSlider.Value = GetCurrentDeviceBrightness(_device);
		}

		private static int GetCurrentDeviceBrightness(Device device)
		{
			device.Properties.TryGetValue("bright", out var currentBrightness);

			if (currentBrightness != null)
			{
				try
				{
					return int.Parse(currentBrightness.ToString());
				}
				catch (FormatException)
				{
					return 10;
				}
			}

			return 0;
		}
		#endregion
	}
}