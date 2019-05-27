using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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



        // Exercício 5 – Altura de uma árvore binária
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

        public void DesenharArvore(Graphics g, int tamanho)
        {
            DesenharArvore(true, raiz, tamanho, 0, Math.PI / 2, Math.PI / 2.5, 350, g);
        }

        public void DesenharPontos(Graphics g, PictureBox pbMapa)
        {
            DesenharPontos(raiz, g, pbMapa);
        }

        private void DesenharPontos(NoArvore<Dado> atual, Graphics grafo, PictureBox mapa)
        {
            if(atual != null)
            {
                float coordenadaX = (atual.Info as Cidade).CoordenadaX * mapa.Width / 4096;
                float coordenadaY = (atual.Info as Cidade).CoordenadaY * mapa.Height / 2048;

                 //grafo.DrawLine(
                 //new Pen(Color.Red, 2f),
                 // new Point(0, 0),
                 // new Point(pbMapa.Size.Width, pbMapa.Size.Height)
                 //);
                grafo.FillEllipse(
                  new SolidBrush(Color.Black),
                  coordenadaX, coordenadaY, 10f, 10f
                );

                grafo.DrawString((atual.Info as Cidade).Nome, new Font("Courier New", 8, FontStyle.Bold),
                             new SolidBrush(Color.FromArgb(32, 32, 32)), coordenadaX + 12, coordenadaY - 10);

                DesenharPontos(atual.Esq, grafo, mapa);
                DesenharPontos(atual.Dir, grafo, mapa);
            } 
        }
    }
}
