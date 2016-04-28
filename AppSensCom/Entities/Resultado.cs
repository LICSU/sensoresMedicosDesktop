using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSensCom.Entities
{
	[Serializable]
	public class Resultado
	{
		public DateTime Fecha { set; get; }

		public string Valor { set; get; }

		public Resultado() { }

		public Resultado(DateTime Fecha, string Valor) 
		{
			this.Fecha = Fecha;
			this.Valor = Valor;
		}
	}
}
