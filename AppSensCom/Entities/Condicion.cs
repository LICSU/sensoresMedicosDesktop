using System;
using System.Collections.Generic;
using System.Data;

namespace AppSensCom.Entities
{
	[Serializable]
	public class Condicion
	{
		public int IdCondicion { set; get; }

		public int IdSensor { set; get; }

		public string Nombre { set; get; }

		public string Formula { set; get; }

		public string Descripcion { set; get; }

		public string Diagnostico { set; get; }

		public List<ItemCondicion> Items { set; get; }

		/// <summary>Método que evalua una condición.</summary>
		/// <param name="paciente">Paciente a evaluar.</param>
		/// <returns>Listado de respuestas coincidentes.</returns>
		public List<CondicionResp> Evaluate(Paciente paciente)
		{
			List<CondicionResp> Respuestas = new List<CondicionResp>();
			List<string> items = new List<string>();
			List<bool> itemsValue = new List<bool>();
			List<string> item_resp = new List<string>();
			CondicionResp respuesta = new CondicionResp { IdCondicion = this.IdCondicion, Match = false, Descripcion = this.Descripcion, Diagnostico = this.Diagnostico };
			if (respuesta.Diagnostico.Length > 0)
				item_resp.Add(respuesta.Diagnostico);
			//Se evaluan cada item y se almacena el resultado.
			for (int iItems = 0; iItems < Items.Count; iItems++)
			{
				try
				{
					items.Add(Items[iItems].Nombre);
					//En teoría se podria filtrar primero los productos y luego evaluarlos:
					//bool resp = Items[iItems].Evaluate(RuleItemFilter.Evaluate(Items[iItems].Filters, productos));
					bool? resp = Items[iItems].Evaluate(paciente);
					if (resp == null)
						return null;
					itemsValue.Add((bool)resp);
					//Cada item que hace match agrega su mensaje al resultado final.
					//Esto es una prueba, no necesarimente es lo deseable.
					if ((bool)resp)
					{
						if (Items[iItems].Descripcion.Length > 0 && !item_resp.Contains(Items[iItems].Descripcion))
							item_resp.Add(Items[iItems].Descripcion);
					}
				}
				catch (Exception ex)
				{
					//WinService.generalWriter.WriteEntry(ex.ToString());
				}
			}
			//Corregir mensaje evitando que se repitan separaciones.
			respuesta.Descripcion = string.Join(Environment.NewLine, item_resp.ToArray());
			try
			{
				//Se evalua la formula, creando una tabla en memoria con los datos.
				using (DataTable dt = new DataTable(@"Be"))
				{
					//Se crean los campos booleanos con los nombres de los ítems.
					for (int i = 0; i < items.Count; i++)
						dt.Columns.Add(items[i], typeof(bool));
					//Se agrega un campo calculado con la formula.
					dt.Columns.Add(new DataColumn(@"Eval", typeof(bool), Formula));
					//Se agrega una fila.
					DataRow dr = dt.NewRow();
					//Se agrega la data.
					for (int i = 0; i < items.Count; i++)
						dr[items[i]] = itemsValue[i];
					//Se agrega a la tabla.
					dt.Rows.Add(dr);
					//Se lee el campo calculado que tendrá la evaluación de los campos almacenados.
					respuesta.Match = (bool)dt.Rows[0][@"Eval"];
					Respuestas.Add(respuesta);
				}
			}
			catch (Exception ex)
			{
				//WinService.generalWriter.WriteEntry(ex.ToString());
			}
			//El resultado de la evaluación de formula es el valor final de la regla.
			return Respuestas;
		}
	}
}
