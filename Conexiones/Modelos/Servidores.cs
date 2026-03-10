using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Servidores
    {
        public string iPServer { get; set; }
        public int idServer { get; set; }
        public string estado { get; set; }
        public List<Servidores> DatosDeServidores(List<Servidores> Server)
        {
            Server.Add(new Servidores() { idServer = 0, iPServer = "RIVANA" });
            Server.Add(new Servidores() { idServer = 1, iPServer = "ARCOS PARADA" });
            Server.Add(new Servidores() { idServer = 2, iPServer = "HOSPITALAB" });
            Server.Add(new Servidores() { idServer = 3, iPServer = "NARDO" });
            Server.Add(new Servidores() { idServer = 4, iPServer = "ARO" });
            Server.Add(new Servidores() { idServer = 5, iPServer = "ESPECIALES" });
            Server.Add(new Servidores() { idServer = 9, iPServer = "ARCOS PARADA 2" });
            return Server;
        }
    }

}
