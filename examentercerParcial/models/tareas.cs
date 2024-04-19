using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace examentercerParcial.models
{
	public class tareas
	{
		public int Id {  get; set; }
		public string descripcion  { get; set; }
		public  DateTime fecha { get; set; }
		public byte[] Photo_Record { get; set; }
		public byte[] Audio_Record { get; set; }


	}
}
