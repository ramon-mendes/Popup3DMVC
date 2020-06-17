using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Popup3DMVC.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Popup3DMVC.Models
{
	public enum EAnimation
	{
		NONE,
		SW,
	}

    public class UploadModel
    {
		[JsonConverter(typeof(ToStringJsonConverter))]
		public ObjectId Id { get; set; }
		public DateTime dt { get; set; }
		public string subpath { get; set; }// path/GUID of file saved locally
		public string filename { get; set; }// uploaded filename
		public string[] thumbs { get; set; }
		public bool featured { get; set; }
		public string featured_name { get; set; }
		public int featured_adjust_y { get; set; }
		public EAnimation featured_anim { get; set; }

		public double volume { get; set; }// cm³ from node-stl
		public double x { get; set; }// mm
		public double y { get; set; }
		public double z { get; set; }

		[BsonIgnore]
		public string UIdimensao => $"{Math.Round(x/10, 1).ToString(Consts.COMMA_NFI)} x {Math.Round(y / 10, 1).ToString(Consts.COMMA_NFI)} x {Math.Round(z / 10, 1).ToString(Consts.COMMA_NFI)} cm";
		[BsonIgnore]
		public string UIvolume => $"{Math.Round(volume, 1).ToString(Consts.COMMA_NFI)} cm³";

		public void FillStlInfo(string filePath)
		{
			if(!File.Exists(filePath))
				throw new Exception("File doesn't exists");

			var file = Startup.MapPath("App_Data/node_scripts/stl_info.js") + " " + filePath;
			var p = Process.Start(new ProcessStartInfo
			{
				FileName = "node",
				Arguments = file,
				UseShellExecute = false,
				RedirectStandardOutput = true,
			});
			p.WaitForExit();

			if(p.ExitCode != 0)
			{
				throw new Exception("Error processing STL file - " + file);
			}

			var json = p.StandardOutput.ReadToEnd();
			dynamic info = JsonConvert.DeserializeObject(json);

			volume = info.volume;
			x = info.x;
			y = info.y;
			z = info.z;
		}
	}
}