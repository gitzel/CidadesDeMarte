using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//ISABELA PAULINO DE SOUZA 18189 GUSTAVO FERRREIRA GITZEL 18194

namespace apCaminhosMarte
{
    public class Arvore<Dado> where Dado : IComparable<Dado>
    {
        private NoArvore<Dado> raiz, atual, antecessor;

        int quantosNos = 0;

        public NoArvore<Dado> Raiz
        {
            get { return raiz; }
            set { raiz = value; }
        }
        public NoArvore<Dado> Atual
        {
            get { return atual; }
            set { atual = value; }
        }
        public NoArvore<Dado> Antecessor
        {
            get { return antecessor; }
            set { antecessor = value; }
        }

        public bool Existe(Dado procurado)  // pesquisa binária não recursiva
        {
            antecessor = null;
            atual = Raiz;
            while (atual != null)
            {
                if (atual.Info.CompareTo(procurado) == 0)
                    return true;
                else
                {
                    antecessor = atual;
                    if (procurado.CompareTo(atual.Info) < 0)
                        atual = atual.Esq; // Desloca à esquerda
                    else
                        atual = atual.Dir; // Desloca à direita
                }
            }
            return false; // Se atual == null, a chave não existe mas antecessor aponta o pai 
        }
     
        public void Incluir(Dado incluido)    // inclusão usando o método de pesquisa binária
        {
            if (raiz == null)
            {
                raiz = new NoArvore<Dado>(incluido);
                return;
            }

            if (Existe(incluido))
                throw new Exception("Informação repetida");
            else
            {
                var novoNo = new NoArvore<Dado>(incluido);

                if (incluido.CompareTo(antecessor.Info) < 0)
                    antecessor.Esq = novoNo;
                else
                    antecessor.Dir = novoNo;
                quantosNos++;
            }
        }

        private int QtosNos(NoArvore<Dado> noAtual)
        {
            if (noAtual == null)
                return 0;
            return 1 +                    // conta o nó atual
                   QtosNos(noAtual.Esq) + // conta nós da subárvore esquerda
                   QtosNos(noAtual.Dir);  // conta nós da subárvore direita
        }
        public int QuantosDados { get => this.QtosNos(this.Raiz); }

        private int QtasFolhas(NoArvore<Dado> noAtual)
        {
            if (noAtual == null)
                return 0;
            if (noAtual.Esq == null && noAtual.Dir == null) // noAtual é folha
                return 1;
            // noAtual não é folha, portanto procuramos as folhas de cada ramo e as contamos
            return QtasFolhas(noAtual.Esq) + // conta folhas da subárvore esquerda
                   QtasFolhas(noAtual.Dir);  // conta folhas da subárvore direita
        }
        public int QuantasFolhas { get => this.QtasFolhas(this.Raiz); }

        private int Altura(NoArvore<Dado> noAtual)
        {
            int alturaEsquerda,
                alturaDireita;

            if (noAtual == null)
                return 0;

            alturaEsquerda = Altura(noAtual.Esq);
            alturaDireita = Altura(noAtual.Dir);

            if (alturaEsquerda >= alturaDireita)
                return 1 + alturaEsquerda;
            return 1 + alturaDireita;
        }

        public int Altura()
        {
            return Altura(Raiz);
        }


        /*
          Método privado que desenha um nó da árvore em um panel. É recursivo e ocorre enquanto o nó que será desenhado não for nulo.
          Por ser recursivo acaba desenhando a árvore inteira.
          @params bool que determina se aquele é o primeiro nó a ser desenhado, um NoArvore<Dado> que será desenhado dois inteiros
          x e y que são coordenadas de onde será desenhado, três double que determinam respectivamente a angulação o incremento e o 
          comprimento do círculo e Graphics que desenha a árvore. 
        */
        private void DesenharArvore(bool primeiro, NoArvore<Dado> atual, int x, int y, double angulo, double i, double compr, Graphics g)
        {
            int xf, yf;

            if (atual != null)
            {
                Pen caneta = new Pen(Color.FromArgb(39, 60, 117));

                xf = (int)Math.Round(x + Math.Cos(angulo) * compr);
                yf = (int)Math.Round(y + Math.Sin(angulo) * compr);

                if (primeiro)
                    yf = 25;

                g.DrawLine(caneta, x, y, xf, yf);

                DesenharArvore(false, atual.Esq, xf, yf, Math.PI / 2 + i,
                                                 i * 0.5, compr * 0.8, g);
                DesenharArvore(false, atual.Dir, xf, yf, Math.PI / 2 - i,
                                                  i * 0.5, compr * 0.8, g);

                SolidBrush preenchimento = new SolidBrush(Color.FromArgb(32, 32, 32));
                g.FillRectangle(preenchimento, xf - 15, yf - 15, 40, 40);
                g.DrawString(atual.Info.ToString(), new Font("Courier New", 12),
                              new SolidBrush(Color.FromArgb(232, 65, 24)), xf - 7, yf - 5);
            }
        }


        /*
          Método público que incia o método recursivo que desenha a árvore.
          @params o Graphics do panel que desenhará a árvore e o tamanho deste panel.
        */
        public void DesenharArvore(Graphics g, int tamanho)
        {
            DesenharArvore(true, raiz, tamanho, 0, Math.PI / 2, Math.PI / 2.5, 350, g);
        } 

        /*
         Método privado que percorre, de forma recursiva, a árvore em PreOrdem e realiza uma action passada como parâmetro.
         @params a Action que será feita em cima do Dado durante o percurso e o nó atual. 
       */
        private void Percorrer(Action<Dado>action, NoArvore<Dado> atual)
        {
            if (atual != null)
            {
                action(atual.Info);
                Percorrer(action, atual.Esq);
                Percorrer(action, atual.Dir);
            }
        }

        /*
         Método público que incia o método recursivo que percorre a árvore em PreOrdem.
         @params a Action que será feita em cima do Dado durante o percurso. 
       */
        public void PreOrdem(Action<Dado> action)
        {
            Percorrer(action, raiz);
        }

        /*
        Método privado que percorre, de forma recursiva, a árvore em InOrdem e realiza uma action passada como parâmetro.
        @params a Action que será feita em cima do Dado durante o percurso e o nó atual. 
        */
        private void PercorrerInOrdem(Action<Dado>action, NoArvore<Dado> atual)
        {
            if(atual != null)
            {
                PercorrerInOrdem(action, atual.Esq);
                action(atual.Info);
                PercorrerInOrdem(action, atual.Dir); 
            }
        }

        /*
         Método público que incia o método recursivo que percorre a árvore em InOrdem.
         @params a Action que será feita em cima do Dado durante o percurso. 
       */
        public void InOrdem(Action<Dado> action)
        {
            PercorrerInOrdem(action, raiz);
        }


        public Dado ExisteDado(Dado procurado)
        {
            antecessor = null;
            atual = Raiz;
            while (atual != null)
            {
                if (atual.Info.CompareTo(procurado) == 0)
                    return atual.Info;
                else
                {
                    antecessor = atual;
                    if (procurado.CompareTo(atual.Info) < 0)
                        atual = atual.Esq; // Desloca à esquerda
                    else
                        atual = atual.Dir; // Desloca à direita
                }
            }
            return default(Dado); // Se atual == null, a chave não existe mas antecessor aponta o pai 
        }
        
    }
}
