using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCaminhosMarte
{
    public partial class Form1 : Form
    {

        PilhaLista<Caminho> melhorCaminho;
        Arvore<Cidade> arvore;
        int[,] adjacencia;
        public Form1()
        {
            InitializeComponent();
        }

        private void TxtCaminhos_DoubleClick(object sender, EventArgs e)
        {

        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            int origem = int.Parse(lsbOrigem.SelectedItem.ToString().Substring(0, 2));

            int destino = int.Parse(lsbDestino.SelectedItem.ToString().Substring(0, 2));

            PilhaLista<Caminho> pilhaCaminho = new PilhaLista<Caminho>();

            dgvCaminhoEncontrado.RowCount = 0;
            dgvCaminhoEncontrado.ColumnCount = 0;

            if (AcharCaminhos(origem, destino, pilhaCaminho))
            {
                int qtdMaxima = arvore.QuantosDados;

                int[] cidade = new int[qtdMaxima];
                PilhaLista<Caminho>[] caminhos = new PilhaLista<Caminho>[qtdMaxima];

                for (int i = 0; i < cidade.Length; i++)
                {
                    cidade[i] = 0;
                    caminhos[i] = new PilhaLista<Caminho>();
                }

                while (!pilhaCaminho.EstaVazia())
                {
                    Caminho c = pilhaCaminho.Desempilhar();

                    ExibirDgv(dgvCaminhoEncontrado, c);

                    if (c.IdDestino == destino)
                    {
                        cidade[c.IdOrigem] += c.Distancia;
                        caminhos[c.IdOrigem].Empilhar(c);
                    }
                    else
                    {
                        cidade[c.IdDestino] += c.Distancia + cidade[c.IdOrigem];

                        caminhos[c.IdDestino] = caminhos[c.IdOrigem].Clone();
                    
                        caminhos[c.IdDestino].Empilhar(c);
                    }
                }
                dgvCaminhoEncontrado.ColumnCount--;

                int menor = int.MaxValue;
                int qualMenor = -1;

                for (int i = 0; i < cidade.Length; i++)
                    if (menor > cidade[i] && cidade[i] != 0)
                    {
                        menor = cidade[i];
                        qualMenor = i;
                    }

                melhorCaminho = caminhos[qualMenor].Clone();

                pbMapa.Invalidate();
            }
            else
                MessageBox.Show("Não existe caminho entre essas cidades!!!");
        }

        private void ExibirDgv(DataGridView qual, Caminho insercao)
        {
            bool p = true;

            int qualLinha = -1, qualColuna = -1;

            for (int i = 0; i < qual.RowCount; i++)
                if (qual.Rows[i].HeaderCell.Value.ToString() == insercao.IdOrigem + "")
                {
                    p = false;
                    qualLinha = i;
                }
                    

            if (p)
            {
                qualLinha = qual.RowCount;
                qual.Rows[qual.RowCount++].HeaderCell.Value = insercao.IdOrigem + "";
            }
                

            p = true;

            for (int i = 0; i < qual.ColumnCount; i++)
                if (qual.Columns[i].HeaderText == insercao.IdDestino + "")
                {
                    qualColuna = i;
                    p = false;
                }
                    

            if (p)
            {
                qualColuna = qual.ColumnCount - 1;
                qual.Columns[qualColuna].HeaderText = insercao.IdDestino + "";
                qual.Columns[qualColuna].SortMode = DataGridViewColumnSortMode.NotSortable;
                qual.ColumnCount++;
            }

            qual[qualColuna, qualLinha].Value = insercao.Distancia + "";
        }

        private bool AcharCaminhos(int entrada, int saida, PilhaLista<Caminho> pilhaCaminho)
        {
            int quantasCidades = arvore.QuantosDados;

            bool[] saidas = new bool[quantasCidades];
            bool[] passouCidade = new bool[quantasCidades];

            for (int i = 0; i < passouCidade.Length; i++)
            {
                passouCidade[i] = false;
                saidas[i] = false;
            }

            bool acabou = false;

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();
            PilhaLista<Caminho> possibilidades = new PilhaLista<Caminho>();

            int entradaAtual = entrada;
            int dist = 0;
            passouCidade[entrada] = true;

            while(!acabou)
            {
                for(int saidaAtual = 0; saidaAtual < quantasCidades; saidaAtual++)
                    if ((dist = adjacencia[entradaAtual, saidaAtual]) != 0  && !passouCidade[saidaAtual])
                        aux.Empilhar(new Caminho(entradaAtual, saidaAtual, dist));

                if (aux.EstaVazia())
                    acabou = true;
                else
                {
                    Caminho possivel = aux.Desempilhar();

                    if (possivel.IdDestino == saida)
                    {
                        pilhaCaminho.Empilhar(possivel);
                        saidas[possivel.IdOrigem] = true;
                    }
                       
                    else
                    {
                        passouCidade[possivel.IdDestino] = true;

                        possibilidades.Empilhar(possivel);
                        entradaAtual = possivel.IdDestino;
                    }
                }
            }

            while (!possibilidades.EstaVazia())
            {
                Caminho a = possibilidades.Desempilhar();

                if (saidas[a.IdDestino])
                    pilhaCaminho.Empilhar(a);
            }

            return !pilhaCaminho.EstaVazia();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore = new Arvore<Cidade>();
            melhorCaminho = null;
            if (dlgArquivo.ShowDialog() == DialogResult.OK)
            {
                StreamReader arq = new StreamReader(dlgArquivo.FileName, Encoding.Default, true);
                LerArquivo(arq);
            }

            int tamanhoMatriz = arvore.QuantosDados;
            adjacencia = new int[tamanhoMatriz, tamanhoMatriz];

            CarregarCidades();

            LerCaminhos(new StreamReader("CaminhosEntreCidadesMarte.txt"));
        }

        private void CarregarCidades()
        {
            arvore.InOrdem((Cidade c) => {
                lsbOrigem.Items.Add($"{c.Id:00} - {c.Nome}");
                lsbDestino.Items.Add($"{c.Id:00} - {c.Nome}");            
            });
        }

        private void LerCaminhos(StreamReader arq)
        {
            while (!arq.EndOfStream)
            {
                Caminho caminho = Caminho.LerRegistro(arq);
                adjacencia[caminho.IdOrigem, caminho.IdDestino] = caminho.Distancia;
            }
            arq.Close();
        }
        private void LerArquivo(StreamReader arquivo)
        {
            while (!arquivo.EndOfStream)
                arvore.Incluir(Cidade.LerRegistro(arquivo));

            arquivo.Close();
        }

        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int t = (int)pnlArvore.Width / 2;
            arvore.DesenharArvore(g, t);
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics grafo = e.Graphics;
            arvore.PreOrdem((Cidade c)=> {
                float coordenadaX = c.CoordenadaX * pbMapa.Width / 4096;
                float coordenadaY = c.CoordenadaY * pbMapa.Height / 2048;
                grafo.FillEllipse(
                 new SolidBrush(Color.Black),
                 coordenadaX, coordenadaY, 10f, 10f
               );
                grafo.DrawString(c.Nome, new Font("Courier New", 8, FontStyle.Bold),
                             new SolidBrush(Color.FromArgb(32, 32, 32)), coordenadaX + 12, coordenadaY - 10);
            });

            if(melhorCaminho != null)
            {
                while (!melhorCaminho.EstaVazia())
                {
                    Caminho possivelCaminho = melhorCaminho.Desempilhar();
                   //                    grafo.DrawLine()
                    ExibirDgv(dgvMelhorCaminho, possivelCaminho);
                }

                dgvMelhorCaminho.ColumnCount--;
            }
        }
    }
}