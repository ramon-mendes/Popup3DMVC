using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Popup3DMVC.Classes
{
	public enum EMaterial
	{
		BAIXA, ALTA
	}

	public enum EQualidade
	{
		BAIXA, MÉDIA, ALTA, ULTRA
	}

	public static class Consts
    {
		public static readonly NumberFormatInfo DOT_NFI = new NumberFormatInfo();
		public static readonly NumberFormatInfo COMMA_NFI = new NumberFormatInfo();

		static Consts()
		{
			DOT_NFI.NumberDecimalSeparator = ".";
			COMMA_NFI.NumberDecimalSeparator = ",";
			ESTADOS_LIST = new SelectList(ESTADOS, "Key", "Value");
		}

		public static string Name(this EMaterial material)
		{
			return "";
		}

		public static readonly string[] MATERIAIS =
		{
			"Aço Inoxidável",
			"Alumide",
			"Bambu PLA",
			"Chocolate",
			"Fibra de Carbono",
			"Gesso",
			"Madeira PLA",
			"Nylon",
			"Plástico ABS",
			"Plástico ASA",
			"Plástico FLEX",
			"Plástico PC",
			"Plástico PC-ABS",
			"Plástico PE",
			"Plástico Pet",
			"Plástico PLA",
			"Plástico PVA",
			"Plástico PVC",
			"Platina",
			"Resina",
			"Resina Visijet",
			"Titânio",
		};

		public static readonly Dictionary<string, string> ESTADOS = new Dictionary<string, string>()
		{
			[""] = "",
			["AC"] = "Acre",
			["AL"] = "Alagoas",
			["AP"] = "Amapá",
			["AM"] = "Amazonas",
			["BA"] = "Bahia",
			["CE"] = "Ceará",
			["DF"] = "Distrito Federal",
			["ES"] = "Espírito Santo",
			["GO"] = "Goiás",
			["MA"] = "Maranhão",
			["MT"] = "Mato Grosso",
			["MS"] = "Mato Grosso do Sul",
			["MG"] = "Minas Gerais",
			["PA"] = "Pará",
			["PB"] = "Paraíba",
			["PR"] = "Paraná",
			["PE"] = "Pernambuco",
			["PI"] = "Piauí",
			["RJ"] = "Rio de Janeiro",
			["RN"] = "Rio Grande do Norte",
			["RS"] = "Rio Grande do Sul",
			["RO"] = "Rondônia",
			["RR"] = "Roraima",
			["SC"] = "Santa Catarina",
			["SP"] = "São Paulo",
			["SE"] = "Sergipe",
			["TO"] = "Tocantins"
		};

		public static readonly SelectList ESTADOS_LIST;
	}
}