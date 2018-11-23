using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;

using RuntimePlugin_ns;
using DB;
namespace GUI
{
	/// <summary>
	/// Interaction logic for PluginManager.xaml
	/// </summary>
	public partial class PluginManager : Window
	{
		ObservableCollection<RuntimePlugin> plugins;
		List<Human> humans2pass;
		
		public PluginManager(ObservableCollection<RuntimePlugin> plugins, List<Human> humans_list)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();
			
			this.humans2pass = humans_list;
			this.plugins = plugins;
			PluginsList.ItemsSource = plugins;
			
		}
		void Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((RuntimePlugin)PluginsList.SelectedItem).Call(humans2pass);
		}
	}
}