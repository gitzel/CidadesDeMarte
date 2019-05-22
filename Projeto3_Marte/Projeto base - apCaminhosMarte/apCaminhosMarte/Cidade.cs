using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosMarte
{
    class Cidade
    {
        const int inicioId = 0;
        const int tamanhoId = 3;
        const int inicioNome = inicioId + tamanhoId;
        const int tamanhoNome = 15;
        const int inicioCoordenadaX = inicioNome + tamanhoNome;
        const int tamanhoCoordenadaX = 5;
        const int inicioCoordenadaY = inicioCoordenadaX + tamanhoCoordenadaX;
        const int tamanhoCoordenadaY = 5;

        int id;
        string nome;
        int coordenadaX;
        int coordenadaY;

        public int Id { get => id; set => id = value; }
        public string Nome { get => nome; set => nome = value; }
        public int CoordenadaX { get => coordenadaX; set => coordenadaX = value; }
        public int CoordenadaY { get => coordenadaY; set => coordenadaY = value; }
        
        public static Cidade LerRegistro(StreamReader arq)
        {
            Cidade ret = null;
            try
            {
                if (!arq.EndOfStream)
                {
                    ret = new Cidade();
                    string linha = arq.ReadLine();
                    ret.Id = int.Parse(linha.Substring(inicioId, tamanhoId));
                    ret.Nome = linha.Substring(inicioNome, tamanhoNome);
                    ret.CoordenadaX = int.Parse(linha.Substring(inicioCoordenadaX, tamanhoCoordenadaX));
                    ret.CoordenadaY = int.Parse(linha.Substring(inicioCoordenadaY, tamanhoCoordenadaY));
                }
            }
            catch(Exception erro)
            {}
            return ret;
        }
    }
}
