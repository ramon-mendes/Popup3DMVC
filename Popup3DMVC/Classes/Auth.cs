using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Popup3DMVC.DAL;
using Popup3DMVC.Models;

namespace Popup3DMVC.Classes
{
    public static class Auth
    {
		static readonly string COOKIE_NAME = "LID";
		static readonly TimeSpan LOGIN_TIMEOUT = TimeSpan.FromDays(2);


		private static Dictionary<string, Tuple<ObjectId, DateTime>> _lid2user = new Dictionary<string, Tuple<ObjectId, DateTime>>();

		public static bool IsLogged(this HttpContext ctx)
		{
			var guid = ctx.Request.Cookies[COOKIE_NAME];
			if(guid == null)
				return false;

			if(!_lid2user.ContainsKey(guid))
				return false;
			var login = _lid2user[guid];
			if(login.Item2 < DateTime.Now)
				return false;
			return true;
		}

		public static UserModel LoggedUser(this HttpContext ctx, P3DContext db)
		{
			var guid = ctx.Request.Cookies[COOKIE_NAME];
			//using(P3DContext db = new P3DContext())
			{
				var user_id = _lid2user[guid].Item1;
				var user = db.Users.FindById(new ObjectId(user_id));
				return user;
			}
		}

		public static bool Login(this HttpContext ctx, string email, string pwd, P3DContext db)
		{
			if(email == null || pwd == null)
				return false;
			email = email.Trim();

			//using(P3DContext db = new P3DContext())
			{
				string pwd_hash = ComputeHash(pwd);
				var user = db.Users.FindOne(u => u.email == email && u.pwd_hash == pwd_hash);
				if(user == null)
					return false;

				var lid = new Guid().ToString();
				var dt_expires = DateTime.Now.Add(LOGIN_TIMEOUT);
				_lid2user[lid] = new Tuple<ObjectId, DateTime>(user.Id, dt_expires);
				ctx.Response.Cookies.Append(COOKIE_NAME, lid, new CookieOptions() { Expires = new DateTimeOffset(dt_expires) });
				return true;
			}
		}

		public static void Logout(this HttpContext ctx)
		{
			var guid = ctx.Request.Cookies[COOKIE_NAME];
			ctx.Response.Cookies.Delete(COOKIE_NAME);
			if(guid != null)
				_lid2user.Remove(guid);
		}


		public static string ComputeHash(string input)
		{
			HashAlgorithm algorithm = new MD5CryptoServiceProvider();
			string salt = "MeowMidiPopup3D";
			Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			Byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

			// Combine salt and input bytes
			Byte[] saltedInput = new Byte[saltBytes.Length + inputBytes.Length];
			saltBytes.CopyTo(saltedInput, 0);
			inputBytes.CopyTo(saltedInput, salt.Length);

			Byte[] hashedBytes = algorithm.ComputeHash(saltedInput);
			return BitConverter.ToString(hashedBytes);
		}
	}
}