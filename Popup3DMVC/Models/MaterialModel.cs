using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Popup3DMVC.Classes;
using Popup3DMVC.DAL;

namespace Popup3DMVC.Models
{
    public class MaterialModel
    {
		public ObjectId Id { get; set; }

		[BsonRef(nameof(P3DContext.Impressoras))]
		public ImpressoraModel Impressora { get; set; }

		public string nome { get; set; }
		public EQualidade qualidade { get; set; }
		public string[] cores { get; set; }
		public double preco_minimo { get; set; }
		public double valor_grama { get; set; }

		public double PrecoPeca(UploadModel peca)
		{
			const double TAXA_SERVICO = 0.02;// 20%

			double preco = peca.volume * valor_grama;
			preco *= 1 + TAXA_SERVICO;
			preco = Math.Max(preco, preco_minimo);
			preco = Math.Floor(preco);
			return preco;
		}

		/*public double PrecoTotal(UploadModel peca, double frete)
		{
			double total = PrecoPeca(peca);
			total += frete;
			total = Math.Floor(total);
			return total;
		}*/
	}
}