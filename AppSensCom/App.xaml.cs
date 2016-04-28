using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AppSensCom.Entities;

namespace AppSensCom
{
	/// <summary>
	/// Lógica de interacción para App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static string Port;
		public static int Speed;
		public static Paciente Paciente;
	}
}
