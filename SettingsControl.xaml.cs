// Decompiled with JetBrains decompiler
// Type: sierses.Haptics.SettingsControl
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
using SimHub;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;



namespace sierses.Sim
{
	// see also ToneControl.cs

	public partial class SettingsControl : UserControl //, IComponentConnector
	{
		public readonly Haptics Plugin;
		readonly ResourceDictionary Tres;
		public SettingsControl()
		{
			Tres = new ResourceDictionary();
			// VS generates InitializeComponent() from settingscontrol.xaml
			InitializeComponent();
		}


		public SettingsControl(Haptics plugin) : this()
		{
			Plugin = plugin;
			DataContext = Plugin;
			Version.Text = Plugin.PluginVersion;
            SetTheme();
		}

		internal string Theme = "AmberTheme.xaml";
		internal void ChangeTheme(string theme)
		{
			Theme = theme;
			SetTheme();
		}

		internal void SetTheme()
		{
			Tres.Source = new Uri("/sierses.Sim;component/Themes/" + Theme, UriKind.Relative);
			//			Resources.MergedDictionaries.Clear();
			//			Resources.MergedDictionaries.Add(Tres);
			Resources.MergedDictionaries[0] = Tres;
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			Plugin.S.Id = "";		   // Refresh_Click() force a mismatch
			Logging.Current.Info($"Haptics.Refresh_Click()");
			Haptics.LoadFailCount = 1;
		}

		private void Lock_Click(object sender, RoutedEventArgs e)
		{
			Plugin.D.Unlocked = !Plugin.D.Unlocked;
			Plugin.D.LockedText = Plugin.D.Unlocked ? "Lock" : "Unlock";
		}

		private void MenuTheme(object sender, RoutedEventArgs e)
		{
			MenuItem m = sender as MenuItem;
			ChangeTheme(m.Header.ToString() + "Theme.xaml");
		}

		private void EngineView_Switch_Checked(object sender, RoutedEventArgs e)
		{
			EngineView_Switch.Content = "Expand";
			EngView1.Height = new GridLength(0);
			EngView2.Height = new GridLength(0);
			EngView3.Height = new GridLength(0);
			EngView4.Height = new GridLength(0);
		}

		private void EngineView_Switch_Unchecked(object sender, RoutedEventArgs e)
		{
			EngineView_Switch.Content = "Collapse";
			EngView1.Height = GridLength.Auto;
			EngView2.Height = GridLength.Auto;
			EngView3.Height = GridLength.Auto;
			EngView4.Height = GridLength.Auto;
		}

		private void ButtonResetSuspension_Click(object sender, RoutedEventArgs e)
		{
			SuspensionMultAll.Value = 1.5;
			SuspensionMult.Value = 1;
			SuspensionGammaAll.Value = 1.7;
			SuspensionGamma.Value = 1;
		}

		private void SuspensionMultAll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void SuspensionGammaAll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void SuspensionGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}


		private void ExpanderEngine_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

			if (e.ClickCount == 2)
			{
				ExpanderEngine.IsExpanded = !ExpanderEngine.IsExpanded;
			}
		}

		private void ExpanderSuspension_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				ExpanderSuspension.IsExpanded = !ExpanderSuspension.IsExpanded;
			}
		}

		private void CarName_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngNameLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngNameLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void CarName_LostFocus(object sender, RoutedEventArgs e)
		{
			EngNameLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngNameLabel.FontWeight = FontWeights.Regular;
		}

		private void CarID_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngIDLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngIDLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void CarID_LostFocus(object sender, RoutedEventArgs e)
		{
			EngIDLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngIDLabel.FontWeight = FontWeights.Regular;
		}

		private void CarClass_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngCategoryLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngCategoryLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void CarClass_LostFocus(object sender, RoutedEventArgs e)
		{
			EngCategoryLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngCategoryLabel.FontWeight = FontWeights.Regular;
		}

		private void IdleRPM_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngIdleLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngIdleLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void IdleRPM_LostFocus(object sender, RoutedEventArgs e)
		{
			EngIdleLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngIdleLabel.FontWeight = FontWeights.Regular;
		}

		private void Redline_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngRedlineLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngRedlineLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void Redline_LostFocus(object sender, RoutedEventArgs e)
		{
			EngRedlineLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngRedlineLabel.FontWeight = FontWeights.Regular;
		}

		private void MaxRPM_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngMaxLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngMaxLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void MaxRPM_LostFocus(object sender, RoutedEventArgs e)
		{
			EngMaxLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngMaxLabel.FontWeight = FontWeights.Regular;
		}

		private void MaxPowerHP_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngHPLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngHPLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void MaxPowerHP_LostFocus(object sender, RoutedEventArgs e)
		{
			EngHPLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngHPLabel.FontWeight = FontWeights.Regular;
		}

		private void EMaxPowerHP_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngEHPLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngEHPLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void EMaxPowerHP_LostFocus(object sender, RoutedEventArgs e)
		{
			EngEHPLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngEHPLabel.FontWeight = FontWeights.Regular;
		}

		private void MaxTorqueNM_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngNMLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngNMLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void MaxTorqueNM_LostFocus(object sender, RoutedEventArgs e)
		{
			EngNMLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngNMLabel.FontWeight = FontWeights.Regular;
		}

		private void Displacement_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngCCLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngCCLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void Displacement_LostFocus(object sender, RoutedEventArgs e)
		{
			EngCCLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngCCLabel.FontWeight = FontWeights.Regular;
		}

		private void EngineConfig_GotFocus(object sender, RoutedEventArgs e)
		{
			EngConfigLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngConfigLabel.FontWeight = FontWeights.Bold;
		}

		private void EngineConfig_LostFocus(object sender, RoutedEventArgs e)
		{
			EngConfigLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngConfigLabel.FontWeight = FontWeights.Regular;
		}

		private void EngineCylinders_GotFocus(object sender, RoutedEventArgs e)
		{
			EngCylindersLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngCylindersLabel.FontWeight = FontWeights.Bold;
		}

		private void EngineCylinders_LostFocus(object sender, RoutedEventArgs e)
		{
			EngCylindersLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngCylindersLabel.FontWeight = FontWeights.Regular;
		}

		private void Drivetrain_GotFocus(object sender, RoutedEventArgs e)
		{
			EngDrivetrainLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngDrivetrainLabel.FontWeight = FontWeights.Bold;
		}

		private void Drivetrain_LostFocus(object sender, RoutedEventArgs e)
		{
			EngDrivetrainLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngDrivetrainLabel.FontWeight = FontWeights.Regular;
		}

		private void Englocation_GotFocus(object sender, RoutedEventArgs e)
		{
			EngLocationLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngLocationLabel.FontWeight = FontWeights.Bold;
		}

		private void Englocation_LostFocus(object sender, RoutedEventArgs e)
		{
			EngLocationLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngLocationLabel.FontWeight = FontWeights.Regular;
		}

		private void FiringOrder_GotFocus(object sender, RoutedEventArgs e)
		{
			FiringOrderLabel1.Foreground = (SolidColorBrush)this.FindResource("Light");
			FiringOrderLabel1.FontWeight = FontWeights.Bold;
            FiringOrderLabel2.Foreground = (SolidColorBrush)this.FindResource("Light");
            FiringOrderLabel2.FontWeight = FontWeights.Bold;
            FiringOrderLabel3.Foreground = (SolidColorBrush)this.FindResource("Light");
            FiringOrderLabel3.FontWeight = FontWeights.Bold;
        }

		private void FiringOrder_LostFocus(object sender, RoutedEventArgs e)
		{
			FiringOrderLabel1.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			FiringOrderLabel1.FontWeight = FontWeights.Regular;
            FiringOrderLabel2.Foreground = (SolidColorBrush)this.FindResource("Foreground");
            FiringOrderLabel2.FontWeight = FontWeights.Regular;
            FiringOrderLabel3.Foreground = (SolidColorBrush)this.FindResource("Foreground");
            FiringOrderLabel3.FontWeight = FontWeights.Regular;
        }

		private void SuspensionMultAll_GotFocus(object sender, RoutedEventArgs e)
		{
			SusMultAllLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusMultAllLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionMultAll_LostFocus(object sender, RoutedEventArgs e)
		{
			SusMultAllLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusMultAllLabel.FontWeight = FontWeights.Regular;
		}

		private void SuspensionMult_GotFocus(object sender, RoutedEventArgs e)
		{
			SusMultLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusMultLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionMult_LostFocus(object sender, RoutedEventArgs e)
		{
			SusMultLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusMultLabel.FontWeight = FontWeights.Regular;
		}

		private void SuspensionGammaAll_GotFocus(object sender, RoutedEventArgs e)
		{
			SusGammaAllLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusGammaAllLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionGammaAll_LostFocus(object sender, RoutedEventArgs e)
		{
			SusGammaAllLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusGammaAllLabel.FontWeight = FontWeights.Regular;
		}

		private void SuspensionGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			SusGammaLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusGammaLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			SusGammaLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusGammaLabel.FontWeight = FontWeights.Regular;
        }
    }
}
