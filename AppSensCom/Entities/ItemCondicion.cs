using System;

namespace AppSensCom.Entities
{
	[Serializable]
	public class ItemCondicion
	{
		public string Nombre { set; get; }

		public TipoItem TipoItem { set; get; }

		public string AccionItem { set; get; }

		public string ValorItem { set; get; }

		public string Descripcion { set; get; }

		public bool? Evaluate(Paciente paciente)
		{
			if (paciente == null)
				return null;
			switch (this.TipoItem)
			{
				case TipoItem.Edad:
					switch (AccionItem.Trim())
					{
						case @"<":
							return paciente.Edad < Convert.ToInt32(this.ValorItem);
						case @">":
							return paciente.Edad > Convert.ToInt32(this.ValorItem);
						case @"=":
							return paciente.Edad == Convert.ToInt32(this.ValorItem);
						default:
							return null;
					}
				case TipoItem.MasaCorporal:
					switch (AccionItem.Trim())
					{
						case @"<":
							return paciente.MasaCorporal < Convert.ToDecimal(this.ValorItem);
						case @">":
							return paciente.MasaCorporal > Convert.ToDecimal(this.ValorItem);
						case @"=":
							return paciente.MasaCorporal == Convert.ToDecimal(this.ValorItem);
						default:
							return null;
					}
				default:
					return null;
			}
		}
	}

	public enum TipoItem
	{
		Edad = 1,
		MasaCorporal = 2
	}
}
