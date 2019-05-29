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
            int origem = int.Parse(lsbOrigem.SelectedItem.ToString().Substring(1, 2));

            int destino = int.Parse(lsbDestino.SelectedItem.ToString().Substring(1, 2));

            PilhaLista<Caminho> pilhaCaminho = new PilhaLista<Caminho>();

            if (AcharCaminhos(origem, destino, ref pilhaCaminho))
            {
                PilhaLista<Caminho> caminhoCerto = new PilhaLista<Caminho>();

                while (!pilhaCaminho.EstaVazia())
                    caminhoCerto.Empilhar(pilhaCaminho.Desempilhar());


            }
            else
                MessageBox.Show("Não existe caminho entre essas cidades!!!");
        }

        private bool AcharCaminhos(int entrada, int saida, ref PilhaLista<Caminho> pilhaCaminho)
        {
            int quantas = arvore.QuantosDados;

            bool[] passouCidade = new bool[quantas];

            for (int i = 0; i < passouCidade.Length; i++)
                passouCidade[i] = false;

            bool achou = false;
            int entrada_atual = entrada;
            int saida_atual = 0;

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();

            while (!achou && !(entrada_atual == entrada && saida_atual == quantas && pilhaCaminho.EstaVazia()))
            {
                while (!achou && saida_atual < quantas)
                {
                    if (adjacencia[entrada_atual, saida_atual] == 0)
                        saida_atual++;
                    else
                        if (passouCidade[saida_atual])
                        saida_atual++;
                    else
                         if (saida_atual == saida)
                         {
                            //int distancia = adjacencia[entrada_atual, saida_atual];
                            //dgvCaminhoEncontrado[entrada_atual, saida_atual].Value = distancia;
                            pilhaCaminho.Empilhar(new Caminho(entrada_atual, saida_atual, adjacencia[entrada_atual, saida_atual]));

                            while (++saida_atual < quantas)
                                if (adjacencia[entrada_atual, saida_atual] != 0)
                                    aux.Empilhar(new Caminho(entrada_atual, saida_atual, adjacencia[entrada_atual, saida_atual]));

                            if (aux.EstaVazia())
                                achou = true;
                            else
                            {
                                Caminho anterior = aux.Desempilhar();
                                entrada_atual = anterior.IdOrigem;
                                saida_atual = anterior.IdDestino;
                            }
                                
                         }
                    else
                    {
                        //int distancia = adjacencia[entrada_atual, saida_atual];
                        //dgvCaminhoEncontrado[entrada_atual, saida_atual].Value = distancia;

                        pilhaCaminho.Empilhar(new Caminho(entrada_atual, saida_atual, adjacencia[entrada_atual, saida_atual]));
                        passouCidade[entrada_atual] = true;
                        entrada_atual = saida_atual;

                        while (++saida_atual < quantas)
                            if (adjacencia[entrada_atual, saida_atual] != 0)
                                aux.Empilhar(new Caminho(entrada_atual, saida_atual, adjacencia[entrada_atual, saida_atual]));

                        saida_atual = 0;
                    }
                }

                if (!achou)
                    if (!pilhaCaminho.EstaVazia())
                    {
                        Caminho cam = pilhaCaminho.Desempilhar();
                        saida_atual = cam.IdDestino + 1;
                        entrada_atual = cam.IdOrigem;
                    }
            }

            return achou;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore = new Arvore<Cidade>();
            if (dlgArquivo.ShowDialog() == DialogResult.OK)
            {
                StreamReader arq = new StreamReader(dlgArquivo.FileName, Encoding.Default, true);
                LerArquivo(arq);
            }

            int tamanhoMatriz = arvore.QuantosDados;
            adjacencia = new int[tamanhoMatriz, tamanhoMatriz];

            dgvCaminhoEncontrado.RowCount = tamanhoMatriz;
            dgvCaminhoEncontrado.ColumnCount = tamanhoMatriz;

            CarregarCidades();

            LerCaminhos(new StreamReader("CaminhosEntreCidadesMarte.txt"));
        }

        private void CarregarCidades()
        {
            arvore.Escrever(lsbOrigem);

            for (int item = 0; item < lsbOrigem.Items.Count; item++)
            {
                lsbDestino.Items.Add(lsbOrigem.Items[item]);
                dgvCaminhoEncontrado.Columns[item].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvCaminhoEncontrado.Columns[item].HeaderText = item + "";
                dgvCaminhoEncontrado.Rows[item].HeaderCell.Value = item + "";
            }

        }

        private void LerCaminhos(StreamReader arq)
        {
            while (!arq.EndOfStream)
            {
                Caminho caminho = Caminho.LerRegistro(arq);
                adjacencia[caminho.IdOrigem, caminho.IdDestino] = caminho.Distancia;
            }
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
            arvore.DesenharPontos(e.Graphics, pbMapa);
        }
    }
}