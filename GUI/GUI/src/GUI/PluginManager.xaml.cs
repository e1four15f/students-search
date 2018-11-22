/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 22.11.2018
 * Time: 22:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI.GUI
{
	/// <summary>
	/// Interaction logic for PluginManager.xaml
	/// </summary>
	public partial class PluginManager : Window
	{
		public PluginManager()
		{
			InitializeComponent();
		}
		void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}