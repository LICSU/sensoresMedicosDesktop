using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppSensCom.Entities
{
	/// <summary>Respuesta asociada a una evaluación de una condición.</summary>
	[Serializable]
	[KnownType(typeof(CondicionResp))]
	[DataContract(Name = @"CondicionResp", IsReference = false)]
	public class CondicionResp
	{
		/// <summary>Condicion que se cumplio.</summary>
		[DataMember(EmitDefaultValue = false, IsRequired = true, Name = @"IdCondicion", Order = 0)]
		public int IdCondicion { get; set; }

		/// <summary>Si esta condicion se cumplio.</summary>
		[DataMember(EmitDefaultValue = false, IsRequired = true, Name = @"Match", Order = 1)]
		public bool Match { get; set; }

		/// <summary>Descripción de la condición.</summary>
		[DataMember(EmitDefaultValue = false, IsRequired = true, Name = @"Descripcion", Order = 2)]
		public string Descripcion { get; set; }

		/// <summary>Diagnostico asociado a la condición.</summary>
		[DataMember(EmitDefaultValue = false, IsRequired = true, Name = @"Diagnostico", Order = 3)]
		public string Diagnostico { get; set; }
	}

	/// <summary>Clase que representa un conjunto de respuesta de las condiciones.</summary>
	[Serializable]
	[KnownType(typeof(RestCondResp))]
	[DataContract(Name = @"RestCondResp", IsReference = false)]
	public class RestCondResp
	{
		/// <summary>Reglas que se cumplieron.</summary>
		[DataMember(EmitDefaultValue = false, IsRequired = true, Name = @"Respuestas", Order = 0)]
		public List<CondicionResp> Respuestas { get; set; }
	}
}
