using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Popup3DMVC.Classes;

namespace Popup3DMVC.Models
{
	public class PrinterUIModel
	{
		public PrinterUIModel(MaterialModel m, double valor)
		{
			material = m;
			impressora = m.Impressora;
			empresa = m.Impressora.Empresa;
			valor_peca = valor.ToString("F", Consts.COMMA_NFI);
			prazo_dias = 6;
		}

		public MaterialModel material { get; set; }
		public ImpressoraModel impressora { get; set; }
		public EmpresaModel empresa { get; set; }
		public string valor_peca { get; set; }
		public int prazo_dias { get; set; }
	}

	public class EmpresaModel
    {
		public ObjectId Id { get; set; }
		public string nome { get; set; }
		public string email { get; set; }

		public Utils.EnderecoCEP end { get; set; }
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class GeoDistComparer : IComparer<EmpresaModel>
	{
		private GeoCoordinate _from;

		public GeoDistComparer(GeoCoordinate from)
		{
			_from = from;
		}

		public int Compare(EmpresaModel x, EmpresaModel y)
		{
			var dist1 = new GeoCoordinate(x.lat, x.lng).GetDistanceTo(_from);
			var dist2 = new GeoCoordinate(y.lat, y.lng).GetDistanceTo(_from);
			return dist1 > dist2 ? 1 : -1;
		}
	}
}