using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  bool[] saidasDoDestinoFinal = new bool[qtd];
            bool[] jaPassouPelaCidade = new bool[qtd];

            for (int indice = 0; indice < saidasDoDestinoFinal.Length; indice++)
            {
                jaPassouPelaCidade[indice] = false;
                saidasDoDestinoFinal[indice] = false;
            }

            bool acabou = false;

            PilhaLista<Caminho> ret = new PilhaLista<Caminho>();
            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();

            int distanciaAtual = 0;
            int entradaAtual = origem;

            PilhaLista<Caminho> possiveis = new PilhaLista<Caminho>();

            while (!acabou)
            {
                if(!jaPassouPelaCidade[entradaAtual])
                {
                    for (int saidaAtual = 0; saidaAtual < qtd; saidaAtual++)
                        if ((distanciaAtual = adjacencia[entradaAtual, saidaAtual]) != 0 && !saidasDoDestinoFinal[entradaAtual])
                            aux.Empilhar(new Caminho(entradaAtual, saidaAtual, distanciaAtual));
                }

                if (aux.EstaVazia())
                    acabou = true;
                else
                {
                    Caminho umPossivel = aux.Desempilhar();

                    if (umPossivel.IdDestino == destino)
                    {
                        ret.Empilhar(umPossivel);
                        saidasDoDestinoFinal[umPossivel.IdOrigem] = true;
                        int i = umPossivel.IdOrigem;
                        while (i != origem)
                        {
                            for (int a = 0; a < qtd; a++)
                                if (jaPassouPelaCidade[a])
                                {
                                    saidasDoDestinoFinal[a] = true;
                                    i = a;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        possiveis.Empilhar(umPossivel);
                        jaPassouPelaCidade[entradaAtual] = true;
                        entradaAtual = umPossivel.IdDestino;                  
                    }
                }
            }

            while (!possiveis.EstaVazia())
            {
                Caminho umPossivel = possiveis.Desempilhar();

                if (saidasDoDestinoFinal[umPossivel.IdDestino])
                {
                    ret.Empilhar(umPossivel);
                    saidasDoDestinoFinal[umPossivel.IdOrigem] = true;
                }
            }

            return ret;
 */




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

        public PilhaLista<Caminho> ObterCaminhos(int origem, int destino, int qtd)
        {
            /*
             *   private Cidade[][] ProcurarCaminhosRecursivo(Cidade origem, Cidade destino, Cidade[] percorridos)
        {
            List<Cidade[]> caminhos = new List<Cidade[]>();

            List<Cidade> percorrido = new List<Cidade>(percorridos);
            Aresta<Cidade>[] saidas = grafo.Saidas(origem);

            percorrido.Add(origem);

            foreach (Aresta<Cidade> aresta in saidas)
            {
                if (aresta.Destino.Equals(destino))
                {
                    caminhos.Add(new Cidade[] { origem, destino });
                }
                else if (!percorrido.Contains(aresta.Destino))
                {
                    Cidade[][] proximos = ProcurarCaminhosRecursivo(aresta.Destino, destino, percorrido.ToArray());

                    if (proximos != null)
                        foreach (Cidade[] proximo in proximos)
                        {
                            List<Cidade> caminho = new List<Cidade>();
                            caminho.Add(origem);
                            caminho.AddRange(proximo);
                            caminhos.Add(caminho.ToArray());
                        }
                }
            }

            return caminhos.Count == 0 ? null : caminhos.ToArray();
        }
             */
            bool[] passouCidade = new bool[qtd];

            for (int i = 0; i < passouCidade.Length; i++)
                passouCidade[i] = false;

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();
            PilhaLista<Caminho> pilha = new PilhaLista<Caminho>();
            PilhaLista<Caminho> ret = new PilhaLista<Caminho>();

            passouCidade[origem] = true;

            int dis = 0;

            int entradaAtual = origem;

            bool acabou = false;

            while (!acabou)
            {
                for (int saidaAtual = 0; saidaAtual < qtd; saidaAtual++)
                    if ((dis = adjacencia[entradaAtual, saidaAtual]) != 0 && !passouCidade[saidaAtual])
                        aux.Empilhar(new Caminho(entradaAtual, saidaAtual, dis));

                if (aux.EstaVazia())
                    acabou = true;
                else
                {
                    Caminho p = aux.Desempilhar();
                    if (p.IdDestino == destino)
                    {
                        ret.Empilhar(p);
                    }
                    else
                    {
                        pilha.Empilhar(p);
                        passouCidade[p.IdOrigem] = true;
                        entradaAtual = p.IdDestino;
                    }
                }
            }

            return ret;
        }
    }
}
