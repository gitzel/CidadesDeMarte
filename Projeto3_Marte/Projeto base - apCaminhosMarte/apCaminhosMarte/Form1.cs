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
            int quantasCidades = arvore.QuantosDados;

            bool[] passouCidade = new bool[quantasCidades];

            for (int i = 0; i < passouCidade.Length; i++)
                passouCidade[i] = false;

            bool acabou = false;
            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();
            int entradaAtual = entrada;
            int saidaAtual = 0;

            while(!acabou)
            {
                int indice = 0;
                while (indice < quantasCidades)
                {
                    if (adjacencia[entradaAtual, indice] != 0)        // tem caminho 
                    {
                        Caminho umCaminho = new Caminho(entradaAtual, indice, adjacencia[entradaAtual, indice]);
                        dgvCaminhoEncontrado[indice, entradaAtual].Value = umCaminho.Distancia;
                        entradaAtual = indice;
                        indice = 0;
                        aux.Empilhar(umCaminho);
                    }
                }
                
                acabou = true;
            }

            //bool achou = false;
            ////PilhaLista<Caminho> aux = new PilhaLista<Caminho>();

            //while (!achou && !(entradaAtual == entrada && saidaAtual == quantasCidades && pilhaCaminho.EstaVazia()))
            //{
            //    while (!achou && saidaAtual < quantasCidades)
            //    {
            //        if (adjacencia[entradaAtual, saidaAtual] == 0)
            //            saidaAtual++;
            //        else
            //            if (passouCidade[saidaAtual])
            //            saidaAtual++;
            //        else
            //             if (saidaAtual == saida)
            //             {
            //                //int distancia = adjacencia[entradaAtual, saidaAtual];
            //                //dgvCaminhoEncontrado[entradaAtual, saidaAtual].Value = distancia;
            //                pilhaCaminho.Empilhar(new Caminho(entradaAtual, saidaAtual, adjacencia[entradaAtual, saidaAtual]));

            //                while (++saidaAtual < quantasCidades)
            //                    if (adjacencia[entradaAtual, saidaAtual] != 0)
            //                        aux.Empilhar(new Caminho(entradaAtual, saidaAtual, adjacencia[entradaAtual, saidaAtual]));

            //                if (aux.EstaVazia())
            //                    achou = true;
            //                else
            //                {
            //                    Caminho anterior = aux.Desempilhar();
            //                    entradaAtual = anterior.IdOrigem;
            //                    saidaAtual = anterior.IdDestino;
            //                }
                                
            //             }
            //        else
            //        {
            //            //int distancia = adjacencia[entradaAtual, saidaAtual];
            //            //dgvCaminhoEncontrado[entradaAtual, saidaAtual].Value = distancia;

            //            pilhaCaminho.Empilhar(new Caminho(entradaAtual, saidaAtual, adjacencia[entradaAtual, saidaAtual]));
            //            passouCidade[entradaAtual] = true;
            //            entradaAtual = saidaAtual;

            //            while (++saidaAtual < quantasCidades)
            //                if (adjacencia[entradaAtual, saidaAtual] != 0)
            //                    aux.Empilhar(new Caminho(entradaAtual, saidaAtual, adjacencia[entradaAtual, saidaAtual]));

            //            saidaAtual = 0;
            //        }
            //    }

            //    if (!achou)
            //        if (!pilhaCaminho.EstaVazia())
            //        {
            //            Caminho cam = pilhaCaminho.Desempilhar();
            //            saidaAtual = cam.IdDestino + 1;
            //            entradaAtual = cam.IdOrigem;
            //        }
            //}

            return acabou;
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