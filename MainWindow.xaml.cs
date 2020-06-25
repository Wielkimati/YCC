using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using YeelightAPI;
using YeelightControlCenter.Properties;

namespace YeelightControlCenter
{
	public partial class MainWindow : Window
	{
		private Device _device;
		private TaskbarIcon taskbarIcon;

		public MainWindow()
		{
			InitializeComponent();

			taskbarIcon = (TaskbarIcon)FindResource("YCCTaskbarIcon");
			taskbarIcon.ContextMenu = new ContextMenu()
			{
				Items = { 
					new MenuItem() { Header = "Open YCC", Name = "MenuItem_OpenYCC"},
					new MenuItem() { Background = Brushes.Black, IsEnabled = false, Height = 5},
					new MenuItem() { Header = "Day Mode", Name = "MenuItem_DayMode" },
					new MenuItem() { Header = "Night Mode", Name = "MenuItem_NightMode" },
					new MenuItem() { Background = Brushes.Black, IsEnabled = false, Height = 5},
					new MenuItem() { Header = "Exit YCC", Name = "MenuItem_ExitYCC" }
				}
			};

			taskbarIcon.TrayMouseDoubleClick += TaskbarIconOnTray_MouseDoubleClick;

			PowerButton.Click += TogglePower;
			ConnectionButton.Click += ConnectDevices;
			DayModeButton.Click += DayModeShortcut;
			NightModeButton.Click += NightModeShortcut;

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

		private void MainWindow_OnStateChanged(object sender, EventArgs e)
		{
			ShowInTaskbar = WindowState != WindowState.Minimized;
		}
		#endregion
		
		#region Taskbar Related Events
		private void TaskbarIconOnTray_MouseDoubleClick(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
				WindowState = WindowState.Normal;
			}
		}

		private void TaskBarIconOnTray_OpenYCC(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
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
		private void DayModeShortcut(object sender, EventArgs eventArgs)
		{
			_device.SetRGBColor(255, 255, 255);
			_device.SetBrightness(100);
			_device.SetColorTemperature(3500);
		}

		private void NightModeShortcut(object sender, EventArgs eventArgs)
		{
			_device.SetRGBColor(167, 508, 48);
			_device.SetBrightness(1);
			_device.SetColorTemperature(3500);
		}

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

			var isDeviceConnected = _device.IsConnected ? "Connected" : "Connection Failed";
			DevOutputTextBlock.Text += $"Status: {isDeviceConnected} == {_device.Hostname}";

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