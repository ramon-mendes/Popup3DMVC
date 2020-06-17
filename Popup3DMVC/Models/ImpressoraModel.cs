using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Popup3DMVC.DAL;

namespace Popup3DMVC.Models
{
    public class ImpressoraModel
    {
		public ObjectId Id { get; set; }

		[BsonRef(nameof(P3DContext.Empresas))]
		public EmpresaModel Empresa { get; set; }

		public string apelido { get; set; }
		public string marca { get; set; }
		public string modelo { get; set; }
		public string descricao { get; set; }
		public double print_x { get; set; }
		public double print_y { get; set; }
		public double print_z { get; set; }
		public double custo_grama { get; set; }
		public double custo_minimo { get; set; }
		public double material { get; set; }
		public string[] cores { get; set; }
	}
}