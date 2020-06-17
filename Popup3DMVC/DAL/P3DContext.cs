using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Popup3DMVC.Classes;
using Popup3DMVC.Models;

namespace Popup3DMVC.DAL
{
    public class P3DContext : LiteDatabase
	{
#if DEBUG
		public static readonly string DBPATH = Startup.MapPath("/App_Data/db_dev.db");
#else
		public static readonly string DBPATH = Startup.MapPath("/App_Data/db_prod.db");
#endif

		private static readonly string CONN_STR;

		static P3DContext()
		{
			if(Environment.OSVersion.Platform == PlatformID.Win32NT)
				CONN_STR = $"Filename={DBPATH}";
			else
				CONN_STR = $"Filename={DBPATH};mode=Exclusive";
		}

		public P3DContext()
			: base(CONN_STR)
		{
			lock(DBPATH)
			{
				//GetCollectionNames().ToList().ForEach(c => DropCollection(c));
				bool isnew = GetCollectionNames().Count() == 0;

				Uploads = GetCollection<UploadModel>(nameof(Uploads));
				Users = GetCollection<UserModel>(nameof(Users));
				Empresas = GetCollection<EmpresaModel>(nameof(Empresas));
				Impressoras = GetCollection<ImpressoraModel>(nameof(Impressoras));
				Material = GetCollection<MaterialModel>(nameof(Material));
				Pedidos = GetCollection<PedidoModel>(nameof(Pedidos));

				if(isnew)
					Initialize();
			}
		}

		public void Initialize()
		{
			Users.Insert(new UserModel()
			{
				dt = DateTime.Now,
				nome = "Ramon Mendes",
				email = "ramon@misoftware.com.br",
				pwd_hash = Auth.ComputeHash("pcdcss"),
			});

			InsertFeaturedUpload("Magic the Gathering card box", "MTG/mtg_white_black_box.stl", "MTG/preview.jpg");
			InsertFeaturedUpload("Plantygon: vaso para suculentas", "Plantygon/v2_plantygon.stl", "Plantygon/preview.jpg", 30);
			InsertFeaturedUpload("Pote para flores", "Pote/GARDEN_placebed.stl", "Pote/preview.jpg");
			// SW
			InsertFeaturedUpload("Apoio de livros Star Wars", "SW1/Storm_Trooper_Right.stl", "SW1/preview.jpg", 28, EAnimation.SW);
			InsertFeaturedUpload("TIE fighter Star Wars", "SW2/tie_fighter.stl", "SW2/preview.jpg", 13, EAnimation.SW);
			InsertFeaturedUpload("Mestre Yoda low-poly", "SWlow-poly-yoda/yoda.stl", "SWlow-poly-yoda/preview.jpg", 0, EAnimation.SW);
			//InsertFeaturedUpload("Lápis Lightsaber", "SWPencil/Lightsaber_Pencil_Top.stl", "SWPencil/preview.jpg", 0, EAnimation.SW);
			InsertFeaturedUpload("Case Start Wars para iPhone 6", "SWPhoneCase/Iphone6StarWars.stl", "SWPhoneCase/preview.jpg", 0, EAnimation.SW);
			InsertFeaturedUpload("Chaveiro Star Wars", "SWKeychain/starwarskeychain_v_1.1.stl", "SWKeychain/preview.png", 0, EAnimation.SW);

			InsertEmpresa("Prototec", "95082-200");
			InsertEmpresa("Protosul", "88260-000");
			InsertEmpresa("3D Build", "18120-970");
			InsertEmpresa("Freedom 3D", "78587-970");

			for(int i = 0; i < 1000; i++)
				Pedidos.Delete(Pedidos.Insert(new PedidoModel()));
		}

		public void InsertEmpresa(string nome, string cep)
		{
			var end = Utils.BuscaCEP(cep);
			var geo = Utils.BuscaCoordEnd(end);

			var e = new EmpresaModel
			{
				nome = nome,
				email = "hey@meow.com",
				end = end,
				lat = geo.Latitude,
				lng = geo.Longitude,
			};
			Empresas.Insert(e);

			var i = new ImpressoraModel
			{
				Empresa = e,
				marca = "Sethi3D",
				modelo = "S3"
			};
			Impressoras.Insert(i);

			Material.Insert(new MaterialModel
			{
				Impressora = i,
				nome = "Plástico PLA",
				cores = new string[] { "Azul", "Verde" },
				qualidade = EQualidade.ALTA,
				preco_minimo = 10,
				valor_grama = 0.3,
			});

			Material.Insert(new MaterialModel
			{
				Impressora = i,
				nome = "Plástico PLA",
				cores = new string[] { "Azul", "Verde" },
				qualidade = EQualidade.BAIXA,
				preco_minimo = 10,
				valor_grama = 0.2,
			});
		}

		public void InsertFeaturedUpload(string name, string featured_stl, string featured_img, int adjust = 0, EAnimation anim = EAnimation.NONE)
		{
			var model = new UploadModel()
			{
				dt = DateTime.Now,
				subpath = "/stl_featured/" + featured_stl,
				thumbs = new string[] { "/stl_featured/" + featured_img },
				filename = Path.GetFileName(featured_stl),
				featured = true,
				featured_name = name,
				featured_adjust_y = adjust,
				featured_anim = anim
			};
			model.FillStlInfo(Startup._env.WebRootPath + model.subpath);

			Uploads.Insert(model);
		}

		public LiteCollection<UploadModel> Uploads { get; set; }
		public LiteCollection<UserModel> Users { get; set; }
		public LiteCollection<EmpresaModel> Empresas { get; set; }
		public LiteCollection<ImpressoraModel> Impressoras { get; set; }
		public LiteCollection<MaterialModel> Material { get; set; }
		public LiteCollection<PedidoModel> Pedidos { get; set; }
	}
}