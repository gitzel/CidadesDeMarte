using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ISABELA PAULINO DE SOUZA 18189 GUSTAVO FERRREIRA GITZEL 18194

namespace apCaminhosMarte
{
    class Grafo
    {
        /*
         Matriz de inteiros que armazena a distância entre dois vértices do grafo.
        */
        protected int[,] adjacencia;

        /*
         Propriedade que retorna e altera a matriz de adjacência.
        */
        public int[,] Adjacencia
        {
            get => adjacencia;
            set => adjacencia = value;
        }

        /*
         Propriedade que retorna e altera uma posição da matriz de adjacência.
        */
        public int this[int i, int j]
        {
            get => adjacencia[i, j];
            set
            {
                if (value > 0)
                    adjacencia[i, j] = value;
            }
        }

        /*
         Construtor que recebe como parâmetro a quantidade máxima de linhas e colunas que a matriz de adjacência deverá ter.
        */
        public Grafo(int tamanhoLinhas, int tamanhoColunas)
        {
            adjacencia = new int[tamanhoLinhas, tamanhoColunas];
        }
        
    }
}
