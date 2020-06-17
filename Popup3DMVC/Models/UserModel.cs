using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace Popup3DMVC.Models
{
    public class UserModel
    {
		public ObjectId Id { get; set; }
		public DateTime dt { get; set; }
		public string nome { get; set; }
		public string email { get; set; }
		public string pwd_hash { get; set; }
    }
}