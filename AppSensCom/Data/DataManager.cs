using AppSensCom.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSensCom.Data
{
	public static class DataManager
	{
		/// <summary>Obtiene una lista de pacientes asociados a una empresa.</summary>
		/// <param name="IdEmpresa">Identificador de la empresa.</param>
		/// <returns>Lista de pacientes asociados a una empresa.</returns>
		public static List<Paciente> GetListPacienteByIdEmpresa(int IdEmpresa)
		{
			List<Paciente> LstPaciente;
			try
			{
				DataTable data = SqlManager.GetDataSet(string.Format(Properties.Settings.Default.SpGetListPacienteByIdEmpresa, IdEmpresa), SqlManager.GetConnetion()).Tables[0];
				if (data == null)
					return null;
				if (data.Rows.Count <= 0)
					return null;
				LstPaciente = new List<Paciente>(data.Rows.Count);
				for (int i = 0; i < data.Rows.Count; i++)
				{
					LstPaciente.Add(new Paciente
					{
						IdPaciente = Convert.ToInt32(data.Rows[i][@"IdUsuario"]),
						Cedula = string.Empty + data.Rows[i][@"Identificacion"],
						Nombre = string.Empty + data.Rows[i][@"Paciente"],
						StatusPaciente = StatusPaciente.Disponible,
						LstSensor = GetListSensorByIdPaciente(Convert.ToInt32(data.Rows[i][@"IdUsuario"]))
					});
				}
				return LstPaciente;
			}
			catch(Exception)
			{
				return null;
			}
		}

		/// <summary>Obtiene una lista de sensores habilitados para un paciente.</summary>
		/// <param name="IdUsuario">Identificador del paciente.</param>
		/// <returns>Lista de sensores habilitados para un paciente</returns>
		public static List<SensorEntity> GetListSensorByIdPaciente(int IdUsuario)
		{
			List<SensorEntity> LstSensor;
			DataTable data = SqlManager.GetDataSet(String.Format(Properties.Settings.Default.SpGetListSensorByIdUsario, IdUsuario), SqlManager.GetConnetion()).Tables[0];
			if (data == null)
				return null;
			if (data.Rows.Count <= 0)
				return null;
			LstSensor = new List<SensorEntity>(data.Rows.Count);
			for (int i = 0; i < data.Rows.Count; i++)
			{
				LstSensor.Add(new SensorEntity
				{
					IdSensor = Convert.ToInt32(data.Rows[i][@"IdSensor"]),
					NombreSensor = string.Empty + data.Rows[i][@"Nombre"]
				});
			}
			return LstSensor;
		}

		/// <summary>Obtiene una lista de sensores activos.</summary>
		/// <returns>Lista de sensores activos.</returns>
		public static List<SensorEntity> GetListSensor()
		{
			List<SensorEntity> LstSensor;
			try
			{
				DataTable data = SqlManager.GetDataSet(Properties.Settings.Default.spGetListSensor, SqlManager.GetConnetion()).Tables[0];
				if (data == null)
					return null;
				if (data.Rows.Count <= 0)
					return null;
				LstSensor = new List<SensorEntity>(data.Rows.Count);
				for (int i = 0; i < data.Rows.Count; i++)
				{
					LstSensor.Add(new SensorEntity
					{
						IdSensor = Convert.ToInt32(data.Rows[i][@"IdSensor"]),
						NombreSensor = string.Empty + data.Rows[i][@"Nombre"]
					});
				}
				return LstSensor;
			}
			catch(Exception)
			{
				return null;
			}
		}

		public static bool AddNewResultPaciente(int idPaciente, int idSensor, DateTime fecha, string valor)
		{
			try
			{
				if (SqlManager.ExecuteNonQuery(string.Format(Properties.Settings.Default.SpAddResultPaciente, idPaciente, idSensor, fecha.ToString(@"yyyy/MM/dd HH:mm:ss"), valor), SqlManager.GetConnetion()) == -1)
					return false;
			}
			catch 
			{
				return false;
			}
			return true;
		}
	}
}
