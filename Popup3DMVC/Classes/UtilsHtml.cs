using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Popup3DMVC.Models;

namespace Popup3DMVC.Classes
{
    public static class UtilsHtml
    {
    }

	public static class HtmlExtensions
	{
		public static object PrintIf(this IHtmlHelper helper, bool cond, string html, string elsehtml = null)
		{
			if(cond)
				return helper.Raw(html);
			if(elsehtml != null)
				return helper.Raw(elsehtml);
			return null;
		}

		public static string StatusString(this EPedidoStatus pedido)
		{
			switch(pedido)
			{
				case EPedidoStatus.AGUARDANDO_PGTO:
					return "Aguardando pagamento";
				case EPedidoStatus.EXPIRADO:
					return "Expirado";
				case EPedidoStatus.PAGO:
					return "Pago";
				default:
					Debug.Assert(false);
					return "";
			}
		}
	}
}