using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AppSensCom.Entities;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace AppSensCom
{
	public partial class Electro : Window
	{
		SerialPort puerto;
		Paciente paci;
		DispatcherTimer dispacher = new DispatcherTimer();
		string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
		public string textPort;

		private int _maxValue;
		public int MaxValue
		{
			get { return _maxValue; }
			set { _maxValue = value; this.OnPropertyChanged(@"MaxValue"); }
		}

		private int _minValue;
		public int MinValue
		{
			get { return _minValue; }
			set { _minValue = value; this.OnPropertyChanged(@"MinValue"); }
		}

		public VoltagePointCollection voltagePointCollection;

		public Electro()
		{
			InitializeComponent();
			voltagePointCollection = new VoltagePointCollection();
			Closing +=Electro_Closing;
			var ds = new EnumerableDataSource<VoltagePoint>(voltagePointCollection);
			ds.SetXMapping(x => dateAxis.ConvertToDouble(x.Date));
			ds.SetYMapping(y => y.Voltage);
			plotter.AddLineGraph(ds, Colors.Green, 2, @"Electrocardiograma");
			MaxValue = 5;
			MinValue = 0;
			paci = App.Paciente;
			path += @"\Log.txt";
			if (File.Exists(path))
				File.Delete(path);
			Loaded += Electro_Loaded;
		}

		private void Electro_Loaded(object sender, RoutedEventArgs e)
		{
			if (iniciar())
				puerto.Write(@"e");
			else
				Close();
		}

		void Electro_Closing(object sender, CancelEventArgs e)
		{
			if (puerto != null)
				if (puerto.IsOpen)
					puerto.Close();
		}

		void dispacher_Tick(object sender, EventArgs e)
		{
			Close();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		bool iniciar()
		{
			try
			{
				puerto = new SerialPort(App.Port, App.Speed);
				if (!puerto.IsOpen)
				{
					puerto.Open();
					puerto.DataReceived += new SerialDataReceivedEventHandler(puerto_DataReceived);
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, @"Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
				App.Paciente.StatusPaciente = StatusPaciente.Disponible;
				return false;
			}
		}

		private void puerto_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				string recivedText = puerto.ReadExisting();
				ReadString(recivedText);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void ReadString(string texto)
		{
			foreach (char strchar in texto)
			{
				if (strchar == ';')
				{
					MostrarTexto(textPort);
				}
				else
				{
					textPort += strchar;
				}
			}
		}

		delegate void MostrarTextoCallback(string texto);
		private void MostrarTexto(string texto)
		{
			if (Txtlog.Dispatcher.CheckAccess())
			{
				Txtlog.AppendText(texto);
				using (StreamWriter file = new StreamWriter(path, true))
				{
					file.WriteLine(string.Format(@"{0} {1}", texto, paci.Nombre));
				}
				try
				{
					double voltaje = 0d;
					string value = texto;
					value = value.Replace(@".", @",").Trim();
					if (value.Length > 3)
						voltaje = double.TryParse(value.Substring(0, 4), out voltaje) ? double.Parse(value.Substring(0, 4)) : voltaje;
					if (voltaje != 0)
					{
						Dispatcher.Invoke(() =>
						{
							voltagePointCollection.Add(new VoltagePoint(voltaje, DateTime.Now));
						});
					}
					textPort = string.Empty;
					//Modificación de resultados del paciente
					Resultado res = new Resultado();
					res.Fecha = DateTime.Now;
					res.Valor = texto;
					if (paci.LstSensor[0].LstResultado == null)
						paci.LstSensor[0].LstResultado = new List<Resultado>();
					paci.LstSensor[0].LstResultado.Add(res);
					paci.StatusPaciente = StatusPaciente.Disponible;
					if (paci.LstSensor[0].LstResultado.Count >= Properties.Settings.Default.QuantityResult)
					{
						App.Paciente = paci;
						Close();
					}
				}
				catch (InvalidOperationException) { }
				catch (Exception)
				{
					//TxtMensaje.Text = ex.Message;
				}
			}
			else
			{
				MostrarTextoCallback d = new MostrarTextoCallback(MostrarTexto);
				Dispatcher.BeginInvoke(d, new object[] { texto });
			}
		}

		delegate void SetDataGraphicCallback(string data);
		public void SetDataGraphic(string data)
		{
			try
			{
				double voltaje = 0d;
				string value = data;
				value = value.Replace(".", ",").Trim();
				if (value.Length > 3)
					voltaje = double.TryParse(value.Substring(0, 4), out voltaje) ? double.Parse(value.Substring(0, 4)) : voltaje;
				if (voltaje != 0)
				{
					Dispatcher.Invoke(() =>
					{
						voltagePointCollection.Add(new VoltagePoint(voltaje, DateTime.Now));
					});
				}
			}
			catch (InvalidOperationException) { }
			catch (Exception)
			{
				//TxtMensaje.Text = ex.Message;
			}
		}
	}
}
