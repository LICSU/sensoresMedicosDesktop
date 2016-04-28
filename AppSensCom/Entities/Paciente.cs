using System;
using System.Collections.Generic;

namespace AppSensCom.Entities
{
	[Serializable]
	public class Paciente
	{
		/// <summary>Identificador del paciente.</summary>
		public int IdPaciente { set; get; }

		/// <summary>Cédula del paciente.</summary>
		public string Cedula { set; get; }

		/// <summary>Nombre del paciente.</summary>
		public string Nombre { set; get; }

		/// <summary>Edad del paciente.</summary>
		public int Edad { set; get; }

		/// <summary>Masa corporal del paciente.</summary>
		public decimal MasaCorporal { set; get; }


		/// <summary>Lista de sensores.</summary>
		public List<SensorEntity> LstSensor { set; get; }

		/// <summary>Representa en que estatus se encuentra el paciente dentro del proceso de obtener los valores de los sensores.</summary>
		public StatusPaciente StatusPaciente { set; get; }

		/// <summary>Constructor vacio de la clase.</summary>
		public Paciente() { }

		/// <summary>Constructor de la clase.</summary>
		/// <param name="IdPaciente">Identificador del paciente.</param>
		/// <param name="Cedula">Cedula del paciente.</param>
		/// <param name="Nombre">Nombre del paciente.</param>
		/// <param name="LstSensor">Lista de sensores por los que el paciente debe someterse.</param>
		public Paciente(int IdPaciente, string Cedula, string Nombre, List<SensorEntity> LstSensor)
		{
			this.IdPaciente = IdPaciente;
			this.Cedula = Cedula;
			this.Nombre = Nombre;
			this.LstSensor = LstSensor;
		}
	}

	public enum StatusPaciente
	{
		Disponible = 0,
		Asignado = 1
	}
}
