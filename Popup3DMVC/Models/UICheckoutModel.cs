using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Html;

namespace Popup3DMVC.Models
{
	// serializado dentro de PedidoModel
	public class CheckoutModel
	{
		// Nomes em Maiuscula para as msg de erro da validação
		public string Nome { get; set; }
		public string Email { get; set; }

		public string CEP { get; set; }
		public string Rua { get; set; }
		public string Número { get; set; }
		public string Complemento { get; set; }
		public string Bairro { get; set; }
		public string Cidade { get; set; }
		public string UF { get; set; }

		public bool EULA { get; set; }

		public void FillDemoAddrData()
		{
			CEP = "95082-200";
			Rua = "Rua Pistóia";
			Número = "83";
			Complemento = "ap. 52";
			Bairro = "Panazzolo";
			Cidade = "Caxias do Sul";
			UF = "DF";
		}

		public HtmlString ToAddrHtml()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Rua + " " + Número);
			if(Complemento != null)
				sb.Append(" / " + Complemento);
			sb.Append("<br/>");
			sb.Append(Bairro + ", " + Cidade + ", " + UF);
			sb.Append("<br/>");
			sb.Append(CEP);
			return new HtmlString(sb.ToString());
		}
	}

	public class CheckoutValidation : AbstractValidator<CheckoutModel>
	{
		public CheckoutValidation()
		{
			RuleFor(cad => cad.Email).NotEmpty().EmailAddress();
			RuleFor(cad => cad.Nome).NotEmpty().MinimumLength(4).Must(n => n.Contains(' ')).WithMessage("Por favor, insira o nome completo (nome e sobrenome).");
			RuleFor(cad => cad.CEP).NotEmpty().Matches("^\\d{5}[-]\\d{3}$").WithMessage("Informe um CEP válido");
			RuleFor(cad => cad.Rua).NotEmpty();
			RuleFor(cad => cad.Número).NotEmpty();
			RuleFor(cad => cad.Bairro).NotEmpty();
			RuleFor(cad => cad.Cidade).NotEmpty();
			RuleFor(cad => cad.UF).NotEmpty();
			RuleFor(cad => cad.EULA).Equal(true).WithMessage("Aceitar os termos de uso é obrigatório.");
		}
	}
}