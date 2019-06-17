using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosMarte
{
    class Grafo
    {
        protected int[,] adjacencia;                  // matriz de adjacência para guardas as distancias 

        public int[,] Adjacencia { get => adjacencia; set => adjacencia = value; }

        public int this[int i, int j]
        {
            get => adjacencia[i, j];
            set
            {
                if (value > 0)
                    adjacencia[i, j] = value;
            }
        }

        public Grafo(int tamanhoLinhas, int tamanhoColunas)
        {
            adjacencia = new int[tamanhoLinhas, tamanhoColunas];
        }
        
    }
}
