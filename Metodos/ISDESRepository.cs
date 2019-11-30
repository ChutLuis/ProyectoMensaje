using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metodos
{
    public interface ISDESRepository
    {
        void ObtenerLlave(string ClaveInicial);
        string ObtenerPathDescargaCifrado();
        string ObtenerPathDescargaDescifrado();
        void EliminarEnCarpeta();
        byte CifrarTexto(byte CifrarByte);
        byte Descifrado(byte nas);
    }
}
