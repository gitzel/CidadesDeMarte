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
        const int TAMANHOMAPAX = 4096, TAMANHOMAPAY = 2048;

        PilhaLista<Caminho> melhorCaminho;
        Arvore<Cidade> arvore;
        int[,] adjacencia;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore = new Arvore<Cidade>();
            melhorCaminho = null;
            LeituraDosArquivos();
        }

        private void LeituraDosArquivos()
        {
            if (dlgArquivo.ShowDialog() == DialogResult.OK)
            {
                IniciarArvore(new StreamReader(dlgArquivo.FileName, Encoding.Default, true));

                int tamanhoMatriz = arvore.QuantosDados;
                adjacencia = new int[tamanhoMatriz, tamanhoMatriz];

                CarregarListBox();

                LerCaminhos(new StreamReader("CaminhosEntreCidadesMarte.txt"));

                pbMapa.Invalidate();
                pnlArvore.Invalidate();
            }
        }

        private void IniciarArvore(StreamReader arquivo)
        {
            while (!arquivo.EndOfStream)
                arvore.Incluir(Cidade.LerRegistro(arquivo));

            arquivo.Close();
        }

        private void CarregarListBox()
        {
            arvore.InOrdem((Cidade c) =>
            {
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

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (lsbOrigem.SelectedIndex >= 0 && lsbDestino.SelectedIndex >= 0)
            {
                int origem = int.Parse(lsbOrigem.SelectedItem.ToString().Substring(0, 2));

                int destino = int.Parse(lsbDestino.SelectedItem.ToString().Substring(0, 2));

                if (destino == origem)
                    MessageBox.Show("Selecione cidades diferentes!", "Viagem inválida", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                {
                    PilhaLista<Caminho> pilhaCaminho = new PilhaLista<Caminho>();

                    dgvMelhorCaminho.RowCount = dgvMelhorCaminho.ColumnCount = dgvCaminhoEncontrado.RowCount = dgvCaminhoEncontrado.ColumnCount = 0;

                    if (AcharCaminhos(origem, destino, pilhaCaminho))
                    {
                        int qtdMaxima = arvore.QuantosDados;

                        int[] distancia = new int[qtdMaxima];

                        PilhaLista<Caminho>[] caminhosPossiveis = new PilhaLista<Caminho>[qtdMaxima];

                        for (int indice = 0; indice < distancia.Length; indice++)
                        {
                            distancia[indice] = 0;
                            caminhosPossiveis[indice] = new PilhaLista<Caminho>();
                        }

                        while (!pilhaCaminho.EstaVazia())
                        {
                            Caminho umCaminho = pilhaCaminho.Desempilhar();

                            ExibirDgv(dgvCaminhoEncontrado, umCaminho);

                            if (umCaminho.IdDestino == destino)
                            {
                                distancia[umCaminho.IdOrigem] += umCaminho.Distancia;
                                caminhosPossiveis[umCaminho.IdOrigem].Empilhar(umCaminho);
                            }
                            else
                            {
                                distancia[umCaminho.IdDestino] += umCaminho.Distancia + distancia[umCaminho.IdOrigem];

                                caminhosPossiveis[umCaminho.IdDestino] = caminhosPossiveis[umCaminho.IdOrigem].Clone();

                                caminhosPossiveis[umCaminho.IdDestino].Empilhar(umCaminho);
                            }
                        }
                        dgvCaminhoEncontrado.ColumnCount--;

                        int menor = int.MaxValue;
                        int qualMenor = -1;

                        for (int indice = 0; indice < distancia.Length; indice++)
                            if (menor > distancia[indice] && distancia[indice] != 0 && caminhosPossiveis[indice].OTopo().IdDestino == destino)
                            {
                                menor = distancia[indice];
                                qualMenor = indice;
                            }

                        caminhosPossiveis[qualMenor].Inverter();
                        melhorCaminho = caminhosPossiveis[qualMenor].Clone();

                        pbMapa.Invalidate();
                    }
                    else
                    {
                        melhorCaminho = null;
                        pbMapa.Invalidate();
                        dgvMelhorCaminho.RowCount = dgvMelhorCaminho.ColumnCount = 0;
                        MessageBox.Show("Não existe caminho entre essas cidades!", "Viagem inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool AcharCaminhos(int origem, int destino, PilhaLista<Caminho> pilhaCaminho)
        {
            int quantasCidades = arvore.QuantosDados;

            bool[] saidasDoDestinoFinal = new bool[quantasCidades];
            bool[] jaPassouPelaCidade = new bool[quantasCidades];

            for (int indice = 0; indice < jaPassouPelaCidade.Length; indice++)
            {
                jaPassouPelaCidade[indice] = false;
                saidasDoDestinoFinal[indice] = false;
            }

            bool acabouOsCaminhos = false;

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();
            PilhaLista<Caminho> possiveis = new PilhaLista<Caminho>();

            int entradaAtual = origem;
            int distanciaAtual = 0;
            jaPassouPelaCidade[origem] = true;

            while (!acabouOsCaminhos)
            {
                for (int saidaAtual = 0; saidaAtual < quantasCidades; saidaAtual++)
                    if ((distanciaAtual = adjacencia[entradaAtual, saidaAtual]) != 0 && !jaPassouPelaCidade[saidaAtual] && !saidasDoDestinoFinal[entradaAtual])
                        aux.Empilhar(new Caminho(entradaAtual, saidaAtual, distanciaAtual));

                if (aux.EstaVazia())
                    acabouOsCaminhos = true;
                else
                {
                    Caminho umPossivel = aux.Desempilhar();

                    if (umPossivel.IdDestino == destino)
                    {
                        pilhaCaminho.Empilhar(umPossivel);
                        saidasDoDestinoFinal[umPossivel.IdOrigem] = true;
                    }
                    else
                    {
                        jaPassouPelaCidade[umPossivel.IdDestino] = true;
                        possiveis.Empilhar(umPossivel);
                        entradaAtual = umPossivel.IdDestino;
                    }
                }
            }

            while (!possiveis.EstaVazia())
            {
                Caminho umPossivel = possiveis.Desempilhar();

                if (saidasDoDestinoFinal[umPossivel.IdDestino])
                {
                    pilhaCaminho.Empilhar(umPossivel);
                    saidasDoDestinoFinal[umPossivel.IdOrigem] = true;
                }
            }

            return !pilhaCaminho.EstaVazia();
        }

        private void ExibirDgv(DataGridView qualDgv, Caminho insercao)
        {
            bool podeEscrever = true;

            int qualLinha = -1, qualColuna = -1;

            for (int indice = 0; indice < qualDgv.RowCount; indice++)
                if (qualDgv.Rows[indice].HeaderCell.Value.ToString() == insercao.IdOrigem + "")
                {
                    podeEscrever = false;
                    qualLinha = indice;
                }

            if (podeEscrever)
            {
                qualLinha = qualDgv.RowCount;
                qualDgv.Rows[qualDgv.RowCount++].HeaderCell.Value = insercao.IdOrigem + "";
            }

            podeEscrever = true;

            for (int indice = 0; indice < qualDgv.ColumnCount; indice++)
                if (qualDgv.Columns[indice].HeaderText == insercao.IdDestino + "")
                {
                    qualColuna = indice;
                    podeEscrever = false;
                }


            if (podeEscrever)
            {
                qualColuna = qualDgv.ColumnCount - 1;
                qualDgv.Columns[qualColuna].HeaderText = insercao.IdDestino + "";
                qualDgv.Columns[qualColuna].SortMode = DataGridViewColumnSortMode.NotSortable;
                qualDgv.ColumnCount++;
            }

            qualDgv[qualColuna, qualLinha].Value = insercao.Distancia.ToString();
        }
        
        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int tamanho = (int)pnlArvore.Width / 2;
            arvore.DesenharArvore(g, tamanho);
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            arvore.PreOrdem((Cidade c) =>
            {
                float coordenadaX = c.CoordenadaX * pbMapa.Width / TAMANHOMAPAX;
                float coordenadaY = c.CoordenadaY * pbMapa.Height / TAMANHOMAPAY;
                g.FillEllipse(
                 new SolidBrush(Color.Black),
                 coordenadaX, coordenadaY, 10f, 10f
               );
                g.DrawString(c.Nome, new Font("Courier New", 8, FontStyle.Bold),
                             new SolidBrush(Color.FromArgb(32, 32, 32)), coordenadaX + 12, coordenadaY - 10);
            });

            if (melhorCaminho != null)
            {
                dgvMelhorCaminho.RowCount = dgvMelhorCaminho.ColumnCount = 0;
                PilhaLista<Caminho> aux = melhorCaminho.Clone();
                while (!aux.EstaVazia())
                {
                    Caminho possivelCaminho = aux.Desempilhar();

                    Cidade origem = arvore.ExisteDado(new Cidade(possivelCaminho.IdOrigem));
                    Cidade destino = arvore.ExisteDado(new Cidade(possivelCaminho.IdDestino));
                    using (var pen = new Pen(Color.FromArgb(211, 47, 47), 4))
                    {
                        pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                        g.DrawLine(pen, origem.CoordenadaX * pbMapa.Width / TAMANHOMAPAX + 3, origem.CoordenadaY * pbMapa.Height / TAMANHOMAPAY + 3, destino.CoordenadaX * pbMapa.Width / TAMANHOMAPAX + 3, destino.CoordenadaY * pbMapa.Height / TAMANHOMAPAY + 3);
                    }
                    ExibirDgv(dgvMelhorCaminho, possivelCaminho);
                }

                dgvMelhorCaminho.ColumnCount--;
            }
        }

        private void cidadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LeituraDosArquivos();
        }
    }
}