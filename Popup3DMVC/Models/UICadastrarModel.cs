using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Popup3DMVC.DAL;

namespace Popup3DMVC.Models
{
    public class UICadastrarModel
    {
		public string Nome { get; set; }
		public string Email { get; set; }
		public string Senha { get; set; }
		public string Senha2 { get; set; }
		public bool Termos { get; set; }
	}

	public class CadastroValidation : AbstractValidator<UICadastrarModel>
	{
		public CadastroValidation(P3DContext db)
		{
			RuleFor(cad => cad.Nome).MinimumLength(4).Must(n => n.Contains(' ')).WithMessage("Por favor, insira o nome completo (nome e sobrenome).");
			RuleFor(cad => cad.Email)
				.Must(email => db.Users.Count(u => u.email == email.Trim()) == 0)
				.WithMessage("E-mail já está sendo usado.");
			RuleFor(cad => cad.Email).EmailAddress().NotEmpty().WithMessage("Informe um e-mail válido.");
			RuleFor(cad => cad.Senha).MinimumLength(6).NotEmpty().WithMessage("A senha deve possuir ao menos 6 caracteres.");
			RuleFor(cad => cad.Senha2).Equal(cad => cad.Senha).WithMessage("As senhas deves ser iguais.");
			RuleFor(cad => cad.Termos).Equal(true).WithMessage("Você deve aceitar os Termos de Uso.");
		}
	}
}