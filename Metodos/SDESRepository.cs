using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metodos
{
    public class SDESRepository : ISDESRepository
    {
        string K1, K2;
        int bufferEscritura = 1024;        
        string[,] Sbox1 = new string[4, 4];
        string[,] Sbox2 = new string[4, 4];
        string pathDescargaCifrado = "";
        string pathDescargaDescifrado = "";

        public void ObtenerLlave(string ClaveInicial)
        {
            if (ClaveInicial.Length==10 && string.IsNullOrWhiteSpace(ClaveInicial.Trim('0','1')))//Validar si la clave es de 10 bits y que sea binario
            {
                //P10
                char[] aux = P10(ClaveInicial);
                //Separar Bits
                char[] ls1 = new char[5];
                Array.Copy(aux, ls1, 5);
                char[] ls2 = new char[5];
                Array.ConstrainedCopy(aux, 5, ls2, 0, 5);
                string sls1 = new string(ls1);
                string sls2 = new string(ls2);
                //LeftShift
                sls1 = LeftShift(sls1);
                sls2 = LeftShift(sls2);
                string aftershift = sls1 + sls2;//Volver a Juntar los 10 bits
                //P8            
                //Primera Key 
                K1 = P8(aftershift);

                //Segundo Left Shift
                sls1 = LeftShift(sls1);
                sls2 = LeftShift(sls2);
                string aftershift2 = sls1 + sls2;//Volver a juntar los 10 bits      
                //Segunda Key 
                K2 = P8(aftershift2);

                LlenarSboxes();                          
            }
            else
            {
                throw new Exception("La llave es menor o mayor a 10 bits por favor ingrese una llave valida");
            }

        }

        public byte CifrarTexto(byte CifrarByte)
        {
            //Obtener Binario
            string Binario = Convert.ToString(CifrarByte, 2).PadLeft(8, '0');
            //Hacer IP 
            string Bits = IP(Binario);            
            //Separar en 2 de 4             
            string BitsIzquierda = Bits.Substring(0,4);
            string BitsDerecha = Bits.Substring(4,4);
            string AcarreoBitsDerecha = BitsDerecha;
            //Hacer EP
            BitsDerecha = EP(BitsDerecha);
            //Hacer XOR
            BitsDerecha = XOR(K1, BitsDerecha);
            //Hacer SBoxes 
            //Obtener Filas Y Columnas Para el Primer Sbox
            int Fila1 = Convert.ToInt32(BitsDerecha.ElementAt(0) + "" + BitsDerecha.ElementAt(3), 2);
            int Columna1 = Convert.ToInt32(BitsDerecha.ElementAt(1) + "" + BitsDerecha.ElementAt(2), 2);
            //Obtener Filas Y Columnas Para el Segundo Sbox
            int Fila2 = Convert.ToInt32(BitsDerecha.ElementAt(4) + "" + BitsDerecha.ElementAt(7), 2);
            int Columna2 = Convert.ToInt32(BitsDerecha.ElementAt(5) + "" + BitsDerecha.ElementAt(6), 2);
            //Juntar los 2 Resultados de la Sbox
            string ResultSbox = Sbox1[Fila1, Columna1] + Sbox2[Fila2, Columna2];
            //Hacer P4
            BitsDerecha = P4(ResultSbox);
            BitsDerecha = XOR(BitsIzquierda,BitsDerecha);
            string FinalPrimerBloque = BitsDerecha + "" + AcarreoBitsDerecha;
            //Empezar Segundo Bloque
            //Hacer Swap
            Bits = Swap(FinalPrimerBloque);
            //Separar De 8 a 4 Bits 
            string BitsIzquierda2 = Bits.Substring(0, 4);
            string BitsDerecha2 = Bits.Substring(4, 4);
            string AcarreoSegundoBloque = BitsDerecha2;
            //Hacer EP
            BitsDerecha2 = EP(BitsDerecha2);
            //Hacer XOR con K2 
            BitsDerecha2 = XOR(K2, BitsDerecha2);
            //Hacer Sboxes            
            //Obtener Filas Y Columnas Para el Primer Sbox
            int FilaSegundoBloque1 = Convert.ToInt32(BitsDerecha2.ElementAt(0) + "" + BitsDerecha2.ElementAt(3), 2);
            int ColumnaSegundoBloque1 = Convert.ToInt32(BitsDerecha2.ElementAt(1) + "" + BitsDerecha2.ElementAt(2), 2);
            //Obtener Filas Y Columnas Para el Segundo Sbox
            int FilaSegundoBloque2 = Convert.ToInt32(BitsDerecha2.ElementAt(4) + "" + BitsDerecha2.ElementAt(7), 2);
            int ColumnaSegundoBloque2 = Convert.ToInt32(BitsDerecha2.ElementAt(5) + "" + BitsDerecha2.ElementAt(6), 2);
            //Juntar los 2 Resultados de la Sbox
            string ResultSboxSegundoBloque = Sbox1[FilaSegundoBloque1, ColumnaSegundoBloque1] + Sbox2[FilaSegundoBloque2, ColumnaSegundoBloque2];
            //HacerP4
            BitsDerecha2 = P4(ResultSboxSegundoBloque);
            //Hacer XOR Con los Bits Izquierdos
            BitsDerecha2 = XOR(BitsIzquierda2, BitsDerecha2);
            string CifradoFinal = BitsDerecha2 + AcarreoSegundoBloque;
            CifradoFinal = IP1(CifradoFinal);
            byte nas = Convert.ToByte(CifradoFinal,2);

            return nas;
        }

        public List<byte> CifrarPass(string Password)
        {
            byte[] bytesTexto = Encoding.ASCII.GetBytes(Password);
            for (int i = 0; i < bytesTexto.Length; i++)
            {
                bytesTexto[i] = CifrarTexto(bytesTexto[i]);
            }
            List<byte> rtn = bytesTexto.ToList();
            return rtn;
        }

        public string DescifrarPass(List<byte> ListaB)
        {
            byte[] Password = ListaB.ToArray();
            for (int i = 0; i < Password.Length; i++)
            {
                Password[i] = Descifrado(Password[i]);
            }
            string Cifrado = Encoding.ASCII.GetString(Password);
            return Cifrado;
        }

        public byte Descifrado(byte nas)
        {
            //Obtener Binario
            string Binario = Convert.ToString(nas, 2).PadLeft(8, '0');
            //Hacer IP 
            string Bits = IP(Binario);
            //Separar en 2 de 4             
            string BitsIzquierda = Bits.Substring(0, 4);
            string BitsDerecha = Bits.Substring(4, 4);
            string AcarreoBitsDerecha = BitsDerecha;
            //Hacer EP
            BitsDerecha = EP(BitsDerecha);
            //Hacer XOR
            BitsDerecha = XOR(K2, BitsDerecha);
            //Hacer SBoxes 
            //Obtener Filas Y Columnas Para el Primer Sbox
            int Fila1 = Convert.ToInt32(BitsDerecha.ElementAt(0) + "" + BitsDerecha.ElementAt(3), 2);
            int Columna1 = Convert.ToInt32(BitsDerecha.ElementAt(1) + "" + BitsDerecha.ElementAt(2), 2);
            //Obtener Filas Y Columnas Para el Segundo Sbox
            int Fila2 = Convert.ToInt32(BitsDerecha.ElementAt(4) + "" + BitsDerecha.ElementAt(7), 2);
            int Columna2 = Convert.ToInt32(BitsDerecha.ElementAt(5) + "" + BitsDerecha.ElementAt(6), 2);
            //Juntar los 2 Resultados de la Sbox
            string ResultSbox = Sbox1[Fila1, Columna1] + Sbox2[Fila2, Columna2];
            //Hacer P4
            BitsDerecha = P4(ResultSbox);
            BitsDerecha = XOR(BitsIzquierda, BitsDerecha);
            string FinalPrimerBloque = BitsDerecha + "" + AcarreoBitsDerecha;
            //Empezar Segundo Bloque
            //Hacer Swap
            Bits = Swap(FinalPrimerBloque);
            //Separar De 8 a 4 Bits 
            string BitsIzquierda2 = Bits.Substring(0, 4);
            string BitsDerecha2 = Bits.Substring(4, 4);
            string AcarreoSegundoBloque = BitsDerecha2;
            //Hacer EP
            BitsDerecha2 = EP(BitsDerecha2);
            //Hacer XOR con K2 
            BitsDerecha2 = XOR(K1, BitsDerecha2);
            //Hacer Sboxes            
            //Obtener Filas Y Columnas Para el Primer Sbox
            int FilaSegundoBloque1 = Convert.ToInt32(BitsDerecha2.ElementAt(0) + "" + BitsDerecha2.ElementAt(3), 2);
            int ColumnaSegundoBloque1 = Convert.ToInt32(BitsDerecha2.ElementAt(1) + "" + BitsDerecha2.ElementAt(2), 2);
            //Obtener Filas Y Columnas Para el Segundo Sbox
            int FilaSegundoBloque2 = Convert.ToInt32(BitsDerecha2.ElementAt(4) + "" + BitsDerecha2.ElementAt(7), 2);
            int ColumnaSegundoBloque2 = Convert.ToInt32(BitsDerecha2.ElementAt(5) + "" + BitsDerecha2.ElementAt(6), 2);
            //Juntar los 2 Resultados de la Sbox
            string ResultSboxSegundoBloque = Sbox1[FilaSegundoBloque1, ColumnaSegundoBloque1] + Sbox2[FilaSegundoBloque2, ColumnaSegundoBloque2];
            //HacerP4
            BitsDerecha2 = P4(ResultSboxSegundoBloque);
            //Hacer XOR Con los Bits Izquierdos
            BitsDerecha2 = XOR(BitsIzquierda2, BitsDerecha2);
            string CifradoFinal = BitsDerecha2 + AcarreoSegundoBloque;
            CifradoFinal = IP1(CifradoFinal);            
            byte asd = Convert.ToByte(CifradoFinal,2);
            return asd;
        }

        private void Escribir(byte item)
        {            
            string _path = Path.Combine(Directory.GetCurrentDirectory(), "CifradoSDES.scif");
            pathDescargaCifrado = _path;
            using (var file2 = new FileStream(_path, FileMode.Append))
            {
                using (var writer = new BinaryWriter(file2))
                {                    
                        writer.Write(item);
                    
                }
            }
        }

        private void EscribirDescifrado(byte item)
        {
            string _path = Path.Combine(Directory.GetCurrentDirectory(), "DescifradoSDES.txt");
            pathDescargaDescifrado = _path;
            using (var file2 = new FileStream(_path, FileMode.Append))
            {
                using (var writer = new BinaryWriter(file2))
                {
                    writer.Write(item);

                }
            }
        }

        public string ObtenerPathDescargaCifrado()
        {
            return pathDescargaCifrado;
        }

        public string ObtenerPathDescargaDescifrado()
        {
            return pathDescargaDescifrado;
        }

        public void EliminarEnCarpeta()
        {
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = d.GetFiles("*.txt");

            foreach (var item in files)
            {
                if (item.Name=="SBox1.txt"|| item.Name == "SBox2.txt")
                {

                }
                else
                {
                    File.Delete(item.FullName);
                }
            }

            FileInfo[] files2 = d.GetFiles("*.scif");
            foreach (var item in files2)
            {                
                    File.Delete(item.FullName);
                
            }

        }

        private string IP(string CifrarByte)
        {
            int[] IP = { 0, 2, 4, 6, 1, 3, 5, 7 };
            char[] vs = CifrarByte.ToCharArray();
            char[] aux = new char[vs.Length];
            int count = 0;
            //IP
            foreach (var item in IP)
            {
                aux[count] = vs[item];
                count++;
            }
            string rtn = new string(aux);
            return rtn;
        }

        private string EP(string CifrarByte)
        {
            int[] EP = { 1, 3, 0, 2, 3, 2, 0, 1 };
            char[] vs = CifrarByte.ToCharArray();
            char[] aux = new char[8];
            int count = 0;
            //EP
            foreach (var item in EP)
            {
                aux[count] = vs[item];
                count++;
            }
            string rtn = new string(aux);
            return rtn;
        }

        private string XOR(string Key, string CifrarByte)
        {
            char[] Llave = Key.ToCharArray();
            char[] Bits = CifrarByte.ToCharArray();
            string Aux = "";
            //Compara 2 Arreglos en la misma posicion y si son iguales Agrega 0 y si son diferentes agrega 1 
            for (int i = 0; i < Llave.Length; i++)
            {
                if (Llave[i].Equals(Bits[i]))
                {
                    Aux = Aux + "0";
                }
                else
                {
                    Aux = Aux + "1";
                }
            }
            return Aux;
        }
        private void LlenarSboxes()
        {
            string _path = Path.Combine(Directory.GetCurrentDirectory(), "SBox1.txt");
            string _path2 = Path.Combine(Directory.GetCurrentDirectory(), "SBox2.txt");
            StreamReader n1 = new StreamReader(_path);
            string Linea = n1.ReadLine();
            for (int i = 0; i < 4; i++)
            {
                for (int b = 0; b < 4; b++)
                {
                    if (Linea != "")
                    {
                        Sbox1[i, b] = Linea;
                        Linea = n1.ReadLine();
                    }
                }
            }
            n1.Close();
            n1.Dispose();
            StreamReader n2 = new StreamReader(_path2);
            string Linea2 = n2.ReadLine();
            for (int i = 0; i < 4; i++)
            {
                for (int b = 0; b < 4; b++)
                {
                    if (Linea2 != "")
                    {
                        Sbox2[i, b] = Linea2;
                        Linea2 = n2.ReadLine();
                    }
                }
            }
            n2.Close();
            n2.Dispose();


        }
        private string LeftShift(string texto)
        {
            texto = texto.Substring(4, 1) + texto.Substring(0, 4);
            return texto;
        }
        private char[] P10(string Binario)
        {
            int[] P10 = { 7, 1, 4, 3, 8, 2, 9, 6, 5, 0 };
            char[] vs = Binario.ToCharArray();
            char[] aux = new char[vs.Length];
            int count = 0;
            //P10 
            foreach (var item in P10)
            {
                aux[count] = vs[item];
                count++;

            }
            return aux;
        }
        private string P8(string Binario)
        {
            int count = 0;
            char[] chars = Binario.ToCharArray();
            char[] aux2 = new char[8];
            int[] P8 = { 3, 9, 8, 6, 0, 1, 2, 7 };
            count = 0;
            foreach (var item in P8)
            {
                aux2[count] = chars[item];
                count++;
            }
            string aux = new string(aux2);
            return aux;
        }

        private string P4(string Txt)
        {
            int count = 0;
            char[] chars = Txt.ToCharArray();
            char[] aux2 = new char[4];
            int[] P4 = { 1,3,2,0};
            count = 0;
            foreach (var item in P4)
            {
                aux2[count] = chars[item];
                count++;
            }
            string aux = new string(aux2);
            return aux;
        }

        private string Swap(string Txt)
        {
            string aux = Txt.Substring(4,4)+Txt.Substring(0,4);
            return aux;
            
        }

        private string IP1(string Txt)
        {
            int[] IP1 = {0,4,1,5,2,6,3,7};
            char[] vs = Txt.ToCharArray();
            char[] aux = new char[vs.Length];
            int count = 0;
            //P10 
            foreach (var item in IP1)
            {
                aux[count] = vs[item];
                count++;

            }

            string aux2 = new string(aux);

            return aux2;

        }

    }
}
