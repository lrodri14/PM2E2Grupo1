using System;
using System.Collections.Generic;

namespace PM2E2Grupo1.Models
{
    public class LugarResponse
    {
        public List<Lugar> datos { get; set; }
    }

    public class Lugar
    {
        public int Id { get; set; }
        public byte[] Audio { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public byte[] Imagen { get; set; }
        public string Descripcion { get; set; }
    }
    
}
