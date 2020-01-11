using System;
using System.Windows;
using System.Windows.Media;
using YeelightAPI;

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

			Button1.Click += Button1OnClick;
			Button2.Click += Button2OnClick;
			BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
			ColorPicker.SelectedColorChanged += ColorPickerOnSelectedColorChanged;
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

		private void ColorPickerOnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			if (ColorPicker.SelectedColor != null)
			{
				var color = ColorPicker.SelectedColor.Value;
				_device.SetRGBColor(color.R, color.G, color.B);
			}
		}

		private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_device.SetBrightness((int)BrightnessSlider.Value);
		}

		private void Button2OnClick(object sender, RoutedEventArgs e)
		{
			ConnectDevices();
		}

		private async void ConnectDevices()
		{
			Text1.Text = string.Empty;
			var connectionAddress = string.Join("", "192.168.0.", TextBoxIp.Text);
			_device = new Device(connectionAddress);

			await _device.Connect();

			var x = _device.IsConnected ? "Connected" : "Connection Failed";
			Text1.Text += $"Status: {x} == {_device.Hostname}";

			BrightnessSlider.Value = GetCurrentDeviceBrightness(_device);
		}

		private void Button1OnClick(object sender, RoutedEventArgs e)
		{
			Text1.Text = $"{_device.IsConnected.ToString()} Weszło!";

			_device.Toggle();
		}
	}
}