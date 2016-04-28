using AppSensCom.Data;
using AppSensCom.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace AppSensCom
{
	public partial class MainWindow : Window
	{
		SerialPort puerto;
		public List<Paciente> lstPaciente;
		List<SensorEntity> lstSensor;
		int idSensorActivo = 0;
		string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		public string textPort;
		public bool BdConnection;
		DispatcherTimer timerWeight = new DispatcherTimer();
		DispatcherTimer timerRotation = new DispatcherTimer();
		DispatcherTimer timerClock = new DispatcherTimer();
		int clock = 0;
		int timeRotation = 0;

		public MainWindow()
		{
			InitializeComponent();
			ValidateConnection();
			cargarVelocidadesTransmision();
			listarPuertos();
			BtnSalir.Click += BtnSalir_Click;
			BtnElectro.Click += BtnElectro_Click;
			BtnDetener.Click += BtnDetener_Click;
			BtnGlucometro.Click += BtnGlucometro_Click;
			BtnPulsiometro.Click += BtnPulsiometro_Click;
			BtnMedidas.Click += BtnMedidas_Click;
			BtnNextElectro.Click += BtnNextElectro_Click;
			BtnNextGlucometro.Click += BtnNextGlucometro_Click;
			BtnNextPulsiometro.Click += BtnNextPulsiometro_Click;
			BtnNextMedida.Click += BtnNextMedida_Click;
			BtnGuardar.Click += BtnGuardar_Click;
			BtnPeso.Click += BtnPeso_Click;
			BtnNextPeso.Click += BtnNextPeso_Click;
			Loaded += MainWindow_Loaded;
			path += @"\Log.txt";
			if (File.Exists(path))
				File.Delete(path);
		}

		private void BtnPeso_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(LblMedida.Text))
				return;
			Process.Start(Properties.Settings.Default.PathSpeech);
		}

		private void BtnMedidas_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(LblMedida.Text))
				return;
			Process.Start(Properties.Settings.Default.PathCloudTool);
			timerWeight.Interval = TimeSpan.FromMilliseconds(3000);
			timerWeight.Tick += TimerWeight_Tick;
			timerWeight.IsEnabled = true;
		}

		private void TimerWeight_Tick(object sender, EventArgs e)
		{
			try
			{
				if (File.Exists(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Properties.Settings.Default.NameFileResultMedidas))
				{
					Process[] processActive = Process.GetProcessesByName(Properties.Settings.Default.NameAppCloud);
					foreach(Process prg in processActive)
					{
						prg.CloseMainWindow();
					}
					timerWeight.IsEnabled = false;
					if (lstPaciente == null || lstSensor == null)
						return;
					int sensIndex = lstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Medida));
					lstSensor[sensIndex].StatusSensor = StatusSensor.Disponible;
					Paciente paciSearch = lstPaciente.Find(x => x.Nombre.IndexOf(LblMedida.Text, StringComparison.InvariantCultureIgnoreCase) != -1);
					//Obtener Resultados del archivo
					XmlSerializer serializer = new XmlSerializer(typeof(ResultMedida));
					string filename = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Properties.Settings.Default.NameFileResultMedidas;
					StreamReader reader = new StreamReader(filename);
					ResultMedida resultMedida = (ResultMedida)serializer.Deserialize(reader);
					reader.Close();
					int paciSearchIndex = paciSearch.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Medida));
					paciSearch.LstSensor[paciSearchIndex].ResultadoMedida = resultMedida;
					//Asignar nuevo paciente al sensor
					LblMedida.Text = string.Empty;
					foreach (Paciente paci in lstPaciente)
					{
						int sensPaciIndex = paci.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Medida));
						if (sensPaciIndex < 0)
							continue;
						if (paci.StatusPaciente == StatusPaciente.Disponible && (paci.LstSensor[sensPaciIndex].ResultadoMedida == null) && paci.Nombre != LblMedida.Text && lstSensor[sensIndex].StatusSensor == StatusSensor.Disponible)
						{
							if (paciSearch != null)
								paciSearch.StatusPaciente = StatusPaciente.Disponible;
							paci.StatusPaciente = StatusPaciente.Asignado;
							lstSensor[sensIndex].StatusSensor = StatusSensor.Ocupado;
							LblMedida.Text = paci.Nombre;
						}
					}
					File.Delete(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Properties.Settings.Default.NameFileResultMedidas);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(string.Format(@"Error: {0}", ex.Message), @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			LoadObject();
			if (lstPaciente != null && lstSensor != null)
				NextPaciente();
			else
				ActivarBotones(false);
			InitializeTimers();
		}

		public void InitializeTimers()
		{
			int totalSens = 0;
			foreach (SensorType suit in Enum.GetValues(typeof(SensorType)))
			{
				totalSens++;
			}
			timeRotation = (Properties.Settings.Default.SecondsBySensor * totalSens) * 1000;
			clock = timeRotation;
			timerRotation.Tick += new EventHandler(TimerRotation_Tick);
			timerClock.Tick += new EventHandler(TimerClock_Tick);
			timerRotation.Interval = TimeSpan.FromMilliseconds(timeRotation);
			timerClock.Interval = TimeSpan.FromMilliseconds(1000);
			timerRotation.Start();
			timerClock.Start();
		}

		private void TimerClock_Tick(object sender, EventArgs e)
		{
			clock = clock - 1000;
			LblNextRotation.Content = string.Format(@"Proxima Rotación en: {0} min {1} seg.", (clock / 1000) / 60, (clock / 1000 % 60));
		}

		private void TimerRotation_Tick(object sender, EventArgs e)
		{
			clock = timeRotation;
			//MessageBox.Show("Debe Cambia los valores");
		}

		public void ValidateConnection()
		{
			BdConnection = SqlManager.ValidateConnection();
		}

		public void LoadObject()
		{
			if (!BdConnection)
			{
				LoadObjectFromXml();
				return;
			}
			lstPaciente = DataManager.GetListPacienteByIdEmpresa(1);
			lstSensor = DataManager.GetListSensor();
		}

		public void LoadObjectFromXml()
		{
			try
			{
				//Carga de los pacientes.
				XmlSerializer serializer = new XmlSerializer(typeof(List<Paciente>));
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.DefaultExt = @".xml";
				dlg.Filter = @"XML Files (*.xml)|*.xml";
				dlg.Title = @"Carga de Pacientes.";
				bool? result = dlg.ShowDialog();
				if (result == true)
				{
					string filename = dlg.FileName;
					StreamReader reader = new StreamReader(filename);
					lstPaciente = (List<Paciente>)serializer.Deserialize(reader);
					reader.Close();
				}
				//Carga de los sensores.
				serializer = new XmlSerializer(typeof(List<SensorEntity>));
				dlg = new OpenFileDialog();
				dlg.DefaultExt = @".xml";
				dlg.Filter = @"XML Files (*.xml)|*.xml";
				dlg.Title = @"Carga de Sensores.";
				result = dlg.ShowDialog();
				if (result == true)
				{
					string filename = dlg.FileName;
					StreamReader reader = new StreamReader(filename);
					lstSensor = (List<SensorEntity>)serializer.Deserialize(reader);
					reader.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(@"No se pudo realizar la carga de la información. {0}", ex.Message), @"Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void BtnNextPeso_Click(object sender, RoutedEventArgs e)
		{
			if (lstPaciente == null || lstSensor == null)
				return;
			int sensIndex = lstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Peso));
			lstSensor[sensIndex].StatusSensor = StatusSensor.Disponible;
			Paciente paciSearch = lstPaciente.Find(x => x.Nombre.IndexOf(LblPeso.Text, StringComparison.OrdinalIgnoreCase) != -1);
			foreach (Paciente paci in lstPaciente)
			{
				int sensPaciIndex = paci.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Peso));
				if (sensPaciIndex < 0)
					continue;
				if (paci.StatusPaciente == StatusPaciente.Disponible && (paci.LstSensor[sensPaciIndex].LstResultado == null || paci.LstSensor[sensPaciIndex].LstResultado.Count <= 0) && paci.Nombre != LblPeso.Text && lstSensor[sensIndex].StatusSensor == StatusSensor.Disponible)
				{
					if (paciSearch != null)
						paciSearch.StatusPaciente = StatusPaciente.Disponible;
					paci.StatusPaciente = StatusPaciente.Asignado;
					lstSensor[sensIndex].StatusSensor = StatusSensor.Ocupado;
					LblPeso.Text = paci.Nombre;
				}
			}
		}

		private void BtnNextPulsiometro_Click(object sender, RoutedEventArgs e)
		{
			if (lstPaciente == null || lstSensor == null)
				return;
			int sensIndex = lstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Pulsiometro));
			lstSensor[sensIndex].StatusSensor = StatusSensor.Disponible;
			Paciente paciSearch = lstPaciente.Find(x => x.Nombre.IndexOf(LblPulsiometro.Text, StringComparison.OrdinalIgnoreCase) != -1);
			foreach (Paciente paci in lstPaciente)
			{
				int sensPaciIndex = paci.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Pulsiometro));
				if (sensPaciIndex < 0)
					continue;
				if (paci.StatusPaciente == StatusPaciente.Disponible && (paci.LstSensor[sensPaciIndex].LstResultado == null || paci.LstSensor[sensPaciIndex].LstResultado.Count <= 0) && paci.Nombre != LblPulsiometro.Text && lstSensor[sensIndex].StatusSensor == StatusSensor.Disponible)
				{
					if (paciSearch != null)
						paciSearch.StatusPaciente = StatusPaciente.Disponible;
					paci.StatusPaciente = StatusPaciente.Asignado;
					lstSensor[sensIndex].StatusSensor = StatusSensor.Ocupado;
					LblPulsiometro.Text = paci.Nombre;
				}
			}
		}

		private void BtnNextGlucometro_Click(object sender, RoutedEventArgs e)
		{
			if (lstPaciente == null || lstSensor == null)
				return;
			int sensIndex = lstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Glucometro));
			lstSensor[sensIndex].StatusSensor = StatusSensor.Disponible;
			Paciente paciSearch = lstPaciente.Find(x => x.Nombre.IndexOf(LblGlucometro.Text, StringComparison.OrdinalIgnoreCase) != -1);
			foreach (Paciente paci in lstPaciente)
			{
				int sensPaciIndex = paci.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Glucometro));
				if (sensPaciIndex < 0)
					continue;
				if (paci.StatusPaciente == StatusPaciente.Disponible && (paci.LstSensor[sensPaciIndex].LstResultado == null || paci.LstSensor[sensPaciIndex].LstResultado.Count <= 0) && paci.Nombre != LblGlucometro.Text && lstSensor[sensIndex].StatusSensor == StatusSensor.Disponible)
				{
					if (paciSearch != null)
						paciSearch.StatusPaciente = StatusPaciente.Disponible;
					paci.StatusPaciente = StatusPaciente.Asignado;
					lstSensor[sensIndex].StatusSensor = StatusSensor.Ocupado;
					LblGlucometro.Text = paci.Nombre;
				}
			}
		}

		private void BtnNextElectro_Click(object sender, RoutedEventArgs e)
		{
			if (lstPaciente == null || lstSensor == null)
				return;
			int sensIndex = lstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Electrocardiograma));
			lstSensor[sensIndex].StatusSensor = StatusSensor.Disponible;
			Paciente paciSearch = lstPaciente.Find(x => x.Nombre.IndexOf(LblElectro.Text, StringComparison.OrdinalIgnoreCase) != -1);
			foreach (Paciente paci in lstPaciente)
			{
				int sensPaciIndex = paci.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Electrocardiograma));
				if (sensPaciIndex < 0)
					continue;
				if (paci.StatusPaciente == StatusPaciente.Disponible && (paci.LstSensor[sensPaciIndex].LstResultado == null || paci.LstSensor[sensPaciIndex].LstResultado.Count <= 0) && paci.Nombre != LblElectro.Text && lstSensor[sensIndex].StatusSensor == StatusSensor.Disponible)
				{
					if (paciSearch != null)
						paciSearch.StatusPaciente = StatusPaciente.Disponible;
					paci.StatusPaciente = StatusPaciente.Asignado;
					lstSensor[sensIndex].StatusSensor = StatusSensor.Ocupado;
					LblElectro.Text = paci.Nombre;
				}
			}
		}

		private void BtnNextMedida_Click(object sender, RoutedEventArgs e)
		{
			if (lstPaciente == null || lstSensor == null)
				return;
			int sensIndex = lstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Medida));
			lstSensor[sensIndex].StatusSensor = StatusSensor.Disponible;
			Paciente paciSearch = lstPaciente.Find(x => x.Nombre.IndexOf(LblMedida.Text, StringComparison.OrdinalIgnoreCase) != -1);
			foreach (Paciente paci in lstPaciente)
			{
				int sensPaciIndex = paci.LstSensor.FindIndex(x => x.IdSensor.Equals((int)SensorType.Medida));
				if (sensPaciIndex < 0)
					continue;
				if (paci.StatusPaciente == StatusPaciente.Disponible && (paci.LstSensor[sensPaciIndex].ResultadoMedida == null) && paci.Nombre != LblMedida.Text && lstSensor[sensIndex].StatusSensor == StatusSensor.Disponible)
				{
					if (paciSearch != null)
						paciSearch.StatusPaciente = StatusPaciente.Disponible;
					paci.StatusPaciente = StatusPaciente.Asignado;
					lstSensor[sensIndex].StatusSensor = StatusSensor.Ocupado;
					LblMedida.Text = paci.Nombre;
				}
			}
		}

		void BtnDetener_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				puerto.Close();
				MostrarTexto(@"Estado: Desconectado");
			}
			catch (Exception ex)
			{
				TxtMensaje.Text = ex.Message;
			}
		}

		bool iniciar()
		{
			try
			{
				if (puerto != null)
					if (puerto.IsOpen)
						puerto.Close();
				string strport = CmbPort.Visibility == Visibility.Visible ? CmbPort.Text : TxtPort.Text;
				if (string.IsNullOrEmpty(strport) || string.IsNullOrEmpty(CmbVelTrans.Text))
					return false;
				puerto = new SerialPort(strport, int.Parse(CmbVelTrans.Text));
				if (!puerto.IsOpen)
				{
					puerto.Open();
					//MostrarTexto(@"Estado: Conectado en el puerto " + strport + System.Environment.NewLine);
					puerto.DataReceived += new SerialDataReceivedEventHandler(puerto_DataReceived);
				}
				return true;
			}
			catch (Exception ex)
			{
				TxtMensaje.Text = ex.Message;
				return false;
			}
		}

		void BtnPulsiometro_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (iniciar() && !string.IsNullOrEmpty(LblPulsiometro.Text))
				{
					puerto.Write(@"p");
					SensorEntity sens = lstSensor.Find(x => x.NombreSensor.IndexOf(BtnPulsiometro.Content.ToString(), StringComparison.OrdinalIgnoreCase) != -1);
					if (sens != null)
						idSensorActivo = sens.IdSensor;
					ActivarBotones(false);
				}
			}
			catch (Exception ex)
			{
				TxtMensaje.Text = ex.Message;
			}
		}

		void BtnGlucometro_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (iniciar() && !string.IsNullOrEmpty(LblGlucometro.Text))
				{
					puerto.Write(@"g");
					SensorEntity sens = lstSensor.Find(x => x.NombreSensor.IndexOf(BtnGlucometro.Content.ToString(), StringComparison.OrdinalIgnoreCase) != -1);
					if (sens != null)
						idSensorActivo = sens.IdSensor;
					ActivarBotones(false);
				}
			}
			catch (Exception ex)
			{
				TxtMensaje.Text = ex.Message;
			}
		}

		void BtnElectro_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (puerto == null)
				{
					MessageBox.Show(@"No hay conexión con el puerto serial, por favor verifique.", @"Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
				if (puerto.IsOpen)
					puerto.Close();
				string strport = CmbPort.Visibility == Visibility.Visible ? CmbPort.Text : TxtPort.Text;
				if (!string.IsNullOrEmpty(strport) && !string.IsNullOrEmpty(CmbVelTrans.Text) && !string.IsNullOrEmpty(LblElectro.Text))
				{
					App.Port = strport;
					App.Speed = int.Parse(CmbVelTrans.Text);
					App.Paciente = lstPaciente.Find(x => x.Nombre.IndexOf(LblElectro.Text, StringComparison.OrdinalIgnoreCase) != -1);
					lstPaciente.Remove(App.Paciente);
					Electro pp = new Electro();
					pp.Closed += pp_Closed;
					pp.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				TxtMensaje.Text = ex.Message;
			}
		}

		void pp_Closed(object sender, EventArgs e)
		{
			List<Paciente> lstTmpPaci = new List<Paciente>(lstPaciente.Count);
			lstTmpPaci.Add(App.Paciente);
			lstTmpPaci.AddRange(lstPaciente);
			lstPaciente.Clear();
			lstPaciente.AddRange(lstTmpPaci);
			lstTmpPaci = null;
			lstSensor[0].StatusSensor = StatusSensor.Disponible;
			NextPaciente();
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
				MostrarTexto(ex.ToString());
			}
		}

		private void ReadString(string texto)
		{
			foreach (char strchar in texto)
			{
				if (strchar == ';')
				{
					MostrarTexto(string.Format("String recibido: {0} - Longitud: {1}\r", textPort, textPort.Length));
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
			if (this.Txtlog.Dispatcher.CheckAccess())
			{
				Paciente paciActive = null;
				switch (idSensorActivo)
				{
					case (int)SensorType.Electrocardiograma:
						paciActive = lstPaciente.Find(x => x.Nombre == LblElectro.Text);
						break;
					case (int)SensorType.Glucometro:
						paciActive = lstPaciente.Find(x => x.Nombre == LblGlucometro.Text);
						break;
					case (int)SensorType.Pulsiometro:
						paciActive = lstPaciente.Find(x => x.Nombre == LblPulsiometro.Text);
						break;
					case (int)SensorType.Medida:
						LblMedida.Text = string.Empty;
						break;
				}
				Txtlog.AppendText(texto);
				using (StreamWriter file = new StreamWriter(path, true))
				{
					file.WriteLine(string.Format(@"{0} {1}", texto, paciActive.Nombre));
				}
				textPort = string.Empty;
				Resultado res = new Resultado();
				res.Fecha = DateTime.Now;
				res.Valor = texto;
				int sensIndex = paciActive.LstSensor.FindIndex(x => x.IdSensor.Equals(idSensorActivo));
				if (paciActive.LstSensor[sensIndex].LstResultado == null)
					paciActive.LstSensor[sensIndex].LstResultado = new List<Resultado>();
				paciActive.LstSensor[sensIndex].LstResultado.Add(res);
				paciActive.StatusPaciente = StatusPaciente.Disponible;
				if (paciActive.LstSensor[sensIndex].LstResultado.Count >= Properties.Settings.Default.QuantityResult)
				{
					switch (idSensorActivo)
					{
						case (int)SensorType.Electrocardiograma:
							LblElectro.Text = string.Empty;
							break;
						case (int)SensorType.Glucometro:
							LblGlucometro.Text = string.Empty;
							break;
						case (int)SensorType.Pulsiometro:
							LblPulsiometro.Text = string.Empty;
							break;
						case (int)SensorType.Medida:
							LblMedida.Text = string.Empty;
							break;
					}
					if (puerto != null)
						if (puerto.IsOpen)
							puerto.Close();
					ActivarBotones(true);
					lstSensor[lstSensor.FindIndex(x => x.IdSensor.Equals(idSensorActivo))].StatusSensor = StatusSensor.Disponible;
					NextPaciente();
				}
			}
			else
			{
				MostrarTextoCallback d = new MostrarTextoCallback(MostrarTexto);
				this.Dispatcher.BeginInvoke(d, new object[] { texto });
			}
		}

		void BtnSalir_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		/// <summary>Cargar las velocidades de transmisión en el combo.</summary>
		private void cargarVelocidadesTransmision()
		{
			CmbVelTrans.Items.Clear();
			CmbVelTrans.Items.Add(2400);
			CmbVelTrans.Items.Add(4800);
			CmbVelTrans.Items.Add(9600);
			CmbVelTrans.Items.Add(14400);
			CmbVelTrans.Items.Add(19200);
			CmbVelTrans.Items.Add(28800);
			CmbVelTrans.Items.Add(38400);
			CmbVelTrans.Items.Add(115200);
		}

		/// <summary>Cargar los puertos seriales disponibles del pc.</summary>
		private void listarPuertos()
		{
			CmbPort.Items.Clear();
			string[] puertos = SerialPort.GetPortNames();
			foreach (string puerto in puertos)
			{
				CmbPort.Items.Add(puerto);
			}
			if (CmbPort.Items.Count == 0)
			{
				TxtMensaje.Text = @"El pc no posee puertos seriales disponibles.";
				TxtPort.Visibility = Visibility.Visible;
				CmbPort.Visibility = Visibility.Hidden;
			}
		}

		public void NextPaciente()
		{
			foreach (SensorEntity item in lstSensor)
			{
				foreach (Paciente paci in lstPaciente)
				{
					SensorEntity senPaci = paci.LstSensor.Find(x => x.NombreSensor.IndexOf(item.NombreSensor, StringComparison.OrdinalIgnoreCase) != -1);
					if (senPaci == null)
						continue;
					if (paci.StatusPaciente == StatusPaciente.Disponible && item.StatusSensor == StatusSensor.Disponible && (senPaci.LstResultado == null || senPaci.LstResultado.Count <= 0))
					{
						paci.StatusPaciente = StatusPaciente.Asignado;
						item.StatusSensor = StatusSensor.Ocupado;
						switch (item.IdSensor)
						{
							case (int)SensorType.Electrocardiograma:
								LblElectro.Text = paci.Nombre;
								break;
							case (int)SensorType.Glucometro:
								LblGlucometro.Text = paci.Nombre;
								break;
							case (int)SensorType.Pulsiometro:
								LblPulsiometro.Text = paci.Nombre;
								break;
							case (int)SensorType.Medida:
								LblMedida.Text = paci.Nombre;
								break;
							case (int)SensorType.Peso:
								LblPeso.Text = paci.Nombre;
								break;
						}
					}
				}
			}
		}

		void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			if (lstPaciente == null)
				return;
			if (!BdConnection)
			{
				SaveResultXmlFile();
				return;
			}
			foreach (Paciente paci in lstPaciente)
			{
				bool flag = true;
				foreach (SensorEntity sens in paci.LstSensor)
				{
					if (sens.LstResultado == null || sens.LstResultado.Count <= 0)
					{
						flag = false;
						break;
					}
				}
				if (flag == true)
				{
					foreach (SensorEntity sens in paci.LstSensor)
					{
						foreach (Resultado result in sens.LstResultado)
						{
							if (!DataManager.AddNewResultPaciente(paci.IdPaciente, sens.IdSensor, result.Fecha, result.Valor))
							{
								MessageBox.Show(string.Format(@"Fallo el intento de guardar datos del paciente {0}",
							paci.Nombre));
								break;
							}
						}
					}
				}
			}
		}

		public void SaveResultXmlFile()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Paciente>));
			SaveFileDialog savefile = new SaveFileDialog();
			savefile.FileName = @"Resultados";
			savefile.DefaultExt = @".xml";
			savefile.Filter = @"XML Files (*.xml)|*.xml";
			savefile.RestoreDirectory = true;
			bool? result = savefile.ShowDialog();
			if (result == true)
			{
				TextWriter writer = new StreamWriter(savefile.FileName, false, System.Text.Encoding.UTF8);
				serializer.Serialize(writer, lstPaciente);
				writer.Close();
			}
		}

		public void ActivarBotones(bool value)
		{
			BtnElectro.IsEnabled = value;
			BtnGlucometro.IsEnabled = value;
			BtnPulsiometro.IsEnabled = value;
			BtnMedidas.IsEnabled = value;
		}
	}
}
