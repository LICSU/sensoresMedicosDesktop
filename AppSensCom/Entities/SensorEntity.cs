using System;
using System.Collections.Generic;

namespace AppSensCom.Entities
{
	[Serializable]
	public class SensorEntity
	{
		/// <summary>Identificador del sensor.</summary>
		public int IdSensor { set; get; }

		/// <summary>Nombre del sensor.</summary>
		public string NombreSensor { set; get; }

		public List<Resultado> LstResultado { set; get; }

		public ResultMedida ResultadoMedida { set; get; }

		public StatusSensor StatusSensor { set; get; }

		/// <summary>Constructor vacio de la clase.</summary>
		public SensorEntity() { }

		/// <summary>Constructor de la clase.</summary>
		/// <param name="IdSensor">Identificador del sensor.</param>
		/// <param name="NombreSensor">Nombre del sensor.</param>
		public SensorEntity(int IdSensor, string NombreSensor)
		{
			this.IdSensor = IdSensor;
			this.NombreSensor = NombreSensor;
			this.StatusSensor = StatusSensor.Disponible;
		}
	}

	public enum StatusSensor
	{
		Disponible = 0,
		Ocupado = 1
	}

	public enum SensorType
	{
		Electrocardiograma = 1,
		Glucometro = 2,
		Pulsiometro = 3,
		Medida = 4,
		Peso = 7
	}
}
