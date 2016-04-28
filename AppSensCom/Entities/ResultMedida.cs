using System;

namespace AppSensCom.Entities
{
	[Serializable]
	public class ResultMedida
	{
		public decimal Altura { get; set; }

		public decimal PerimetroCabeza { get; set; }

		public decimal PerimetroCuello { get; set; }

		public decimal PerimetroPecho { get; set; }

		public decimal PerimetroCintura { get; set; }

		public decimal PerimetroCadera { get; set; }

		public decimal IndiceCaderaCintura { get; set; }
	}
}
