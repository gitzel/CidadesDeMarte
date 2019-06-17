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


        private Caminho[] Saidas(int origem, bool[] jaPassou)
        {
            var saidas = new List<Caminho>();
            for (int i = 0; i < 23; i++)
                if (adjacencia[origem, i] != 0 && !jaPassou[i])
                    saidas.Add(new Caminho(origem, i, adjacencia[origem, i]));

            return (saidas.Count == 0) ? null : saidas.ToArray();
        }


        public void Teste2(int origem, int destino)
        {
            PilhaLista<Caminho>[] possiveis = new PilhaLista<Caminho>[23];
            List<PilhaLista<Caminho>> caminhos = new List<PilhaLista<Caminho>>();
            PilhaLista<Caminho> caminhoAtual = new PilhaLista<Caminho>();
            PilhaLista<Caminho> possibilidades = new PilhaLista<Caminho>();
            int atual = origem;
            bool[] jaPassou = new bool[23];
            int pos = 0;

            for (int i = 0; i < 23; i++)
            {
                possiveis[i] = new PilhaLista<Caminho>();
                jaPassou[i] = false;
            }


            caminhos[pos] = new PilhaLista<Caminho>();
            jaPassou[origem] = true;

            while (atual != destino)
            {
                var saidas = Saidas(atual, jaPassou);
                if (saidas != null)
                {
                    foreach (Caminho c in saidas)
                        possibilidades.Empilhar(c);
                }
                else
                {
                    jaPassou[atual] = true;
                    caminhoAtual.Desempilhar();
                }

                if (!possibilidades.EstaVazia()) 
                {
                    Caminho c = possibilidades.Desempilhar();
                    if (c.IdDestino == destino)
                    {
                        caminhoAtual.Empilhar(c);
                        caminhos.Add(caminhoAtual.Clone());
                        if (!possibilidades.EstaVazia())
                        {
                            Caminho retorno = possibilidades.Desempilhar();
                            while (caminhoAtual.OTopo().IdDestino != retorno.IdOrigem)
                                caminhoAtual.Desempilhar();

                            caminhoAtual.Empilhar(retorno);

                            atual = retorno.IdDestino;
                        }
                    }
                    else
                    {
                        if (caminhoAtual.EstaVazia() || caminhoAtual.OTopo().IdDestino == c.IdOrigem)
                            caminhoAtual.Empilhar(c);
                        else
                            possiveis[c.IdDestino].Empilhar(c);
                        atual = c.IdDestino;
                        jaPassou[c.IdOrigem] = true;
                    }
                }
                else
                {
                    break;
                }
            }
        }



        public void Teste(int origem, int destino)
        {
            PilhaLista<Caminho>[] possiveis = new PilhaLista<Caminho>[23];
            PilhaLista<Caminho>[] caminhos = new PilhaLista<Caminho>[23];
            bool[] jaPassou = new bool[23];
            bool[] ehSaida = new bool[23];
            int pos = 0;
            for (int i = 0; i < 23; i++)
            {
                possiveis[i] = new PilhaLista<Caminho>();
                caminhos[i] = new PilhaLista<Caminho>();
                jaPassou[i] = false;
                ehSaida[i] = false;
            }

            int atual = origem;

            bool acabou = false;
            while (!acabou)
            {
                PilhaLista<Caminho> aux = Saidas(atual, jaPassou);
                if (aux == null)
                    acabou = true;
                else
                {
                    Caminho c = aux.Desempilhar();
                    if (c.IdDestino == destino)
                    {
                        if (!caminhos[pos].EstaVazia())
                            caminhos[pos].Empilhar(c);
                        else
                        {

                        }
                        pos++;
                    }
                    else
                    {
                        possiveis[c.IdDestino].Empilhar(c);
                    }
                }

            }
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
