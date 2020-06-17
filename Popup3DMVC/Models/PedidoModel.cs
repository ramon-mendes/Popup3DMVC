using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LiteDB;
using Popup3DMVC.Classes;
using Popup3DMVC.DAL;

namespace Popup3DMVC.Models
{
	public enum EPedidoStatus
	{
		AGUARDANDO_PGTO,
		EXPIRADO,
		PAGO
	}

    public class PedidoModel
    {
		[BsonId]
		public int Id { get; set; }
		public int num { get; set; }
		public DateTime dt { get; set; }
		public EPedidoStatus status { get; set; }

		[BsonRef(nameof(P3DContext.Users))]
		public UserModel User { get; set; }

		[BsonRef(nameof(P3DContext.Uploads))]
		public UploadModel Upload { get; set; }

		// Serialized, not refed.. what if records are deleted?
		public MaterialModel Material { get; set; }
		public ImpressoraModel Impressora { get; set; }
		public EmpresaModel Empresa { get; set; }
		public CheckoutModel Checkout { get; set; }

		public string cor { get; set; }
		public int prazo_dias_envio { get; set; }
		public int prazo_dias_impressao { get; set; }
		public double valor_frete { get; set; }
		public double valor_impressao { get; set; }
		public double valor_total { get; set; }

		public bool pago { get; set; }

		public string UIvalor_frete => $"R$ {Math.Round(valor_frete, 2).ToString("F", Consts.COMMA_NFI)}";
		public string UIvalor_impressao => $"R$ {Math.Round(valor_impressao, 2).ToString("F", Consts.COMMA_NFI)}";
		public string UIvalor_total => $"R$ {Math.Round(valor_total, 2).ToString("F", Consts.COMMA_NFI)}";
	}
}