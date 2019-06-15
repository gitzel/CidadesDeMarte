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
        const int TAMANHOMAPACOORDENADAX = 4096, TAMANHOMAPACOORDENADAY = 2048;     // tamanho do mapa real

        PilhaLista<Caminho> melhorCaminho;          // pilha que guarda o melhor caminho entre duas cidades
        Arvore<Cidade> arvore;                  // arvore binária para armazenar as cidades
        int[,] adjacencia;                  // matriz de adjacência para guardas as distancias 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) // quando carregar o formulário
        {
            arvore = new Arvore<Cidade>();              // instanciamos a arvore
            melhorCaminho = null;           // zeramos o menor caminho
            LeituraDosArquivos();           // fazemos a leitura dos arquivos
        }

        private void LeituraDosArquivos()       // metodo para ler arquivos
        {
            try
            {
                if (dlgArquivo.ShowDialog() == DialogResult.OK)     // caso o usuario escolha o arquivo correto
                {
                    IniciarArvore(new StreamReader(dlgArquivo.FileName, Encoding.Default, true));       // preenchemos a arvore com as cidades

                    int tamanhoMatriz = arvore.QuantosDados;                // guardamos numa variavel a quantidade de cidades
                    adjacencia = new int[tamanhoMatriz, tamanhoMatriz];     // instancia a matriz
                    LerCaminhos(new StreamReader("CaminhosEntreCidadesMarte.txt"));         // inicia a leitura do arquivo dos caminhos

                    CarregarListBox();                      ///

                    pbMapa.Invalidate();
                    pnlArvore.Invalidate();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao ler arquivos!", "Arquivo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        PilhaLista<Caminho>[] caminhosPossiveis = new PilhaLista<Caminho>[qtdMaxima];

                        for (int indice = 0; indice < caminhosPossiveis.Length; indice++)
                            caminhosPossiveis[indice] = new PilhaLista<Caminho>();

                        while (!pilhaCaminho.EstaVazia())
                        {
                            Caminho umCaminho = pilhaCaminho.Desempilhar();

                            caminhosPossiveis[umCaminho.IdDestino].Empilhar(umCaminho);
                        }

                        int menor = int.MaxValue;
                        int onde = destino;

                        PilhaLista<Caminho>[] caminhos = new PilhaLista<Caminho>[qtdMaxima];

                        for(int i = 0; i< caminhos.Length; i++)
                        {
                            caminhos[i] = new PilhaLista<Caminho>();
                        }

                        PilhaLista<Caminho> caminhosAnteriores = new PilhaLista<Caminho>();


                        bool b = true;
                       while(true)
                       {
                            while(true)
                            {
                                Caminho p = caminhosPossiveis[onde].Desempilhar();
                                int entrada = p.IdOrigem;

                                if(!caminhos[onde].EstaVazia())
                                {
                                    caminhos[entrada] = caminhos[onde].Clone();
                                }

                                caminhos[entrada].Empilhar(p);
                                onde = entrada;
                                break;
                            }

                            // menor e exibir

                            bool primeiro = true;

                            while (!caminhos[onde].EstaVazia())
                            {
                                Caminho a = caminhos[onde].Desempilhar();
                                if (caminhosPossiveis[a.IdDestino].EstaVazia() && primeiro)
                                {
                                    caminhosAnteriores.Empilhar(a);
                                    primeiro = false;
                                }
                                else
                                    caminhosPossiveis[a.IdDestino].Empilhar(a);
                            }

                            while (!caminhosAnteriores.EstaVazia() && b)
                            {
                                Caminho anterior = caminhosAnteriores.Desempilhar();
                                if (caminhosPossiveis[anterior.IdDestino].Tamanho() <= 1)
                                    caminhosPossiveis[anterior.IdDestino].Empilhar(anterior);
                                else
                                    caminhosAnteriores.Empilhar(anterior);
                            }

                            b = false;

                            onde = destino;
                           
                            break;
                       }


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
            bool[,] jaPassouPelaCidade = new bool[quantasCidades, quantasCidades];

            for (int indice = 0; indice < saidasDoDestinoFinal.Length; indice++)
            {
                jaPassouPelaCidade[indice, indice] = false;
                saidasDoDestinoFinal[indice] = false;
            }

            bool acabouOsCaminhos = false;

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();
            PilhaLista<Caminho> possiveis = new PilhaLista<Caminho>();

            int entradaAtual = origem;
            int distanciaAtual = 0;

            while (!acabouOsCaminhos)
            {
                for (int saidaAtual = 0; saidaAtual < quantasCidades; saidaAtual++)
                    if ((distanciaAtual = adjacencia[entradaAtual, saidaAtual]) != 0 && !jaPassouPelaCidade[entradaAtual, saidaAtual] && !saidasDoDestinoFinal[entradaAtual])
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
                        int i = umPossivel.IdOrigem;
                        while (i != origem)
                        {
                            for (int a = 0; a < quantasCidades; a++)
                                if (jaPassouPelaCidade[a, i])
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
                        entradaAtual = umPossivel.IdDestino;
                        jaPassouPelaCidade[umPossivel.IdOrigem, umPossivel.IdDestino] = true;
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
                float coordenadaX = c.CoordenadaX * pbMapa.Width / TAMANHOMAPACOORDENADAX;
                float coordenadaY = c.CoordenadaY * pbMapa.Height / TAMANHOMAPACOORDENADAY;
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
                        g.DrawLine(pen, origem.CoordenadaX * pbMapa.Width / TAMANHOMAPACOORDENADAX + 3, origem.CoordenadaY * pbMapa.Height / TAMANHOMAPACOORDENADAY + 3, destino.CoordenadaX * pbMapa.Width / TAMANHOMAPACOORDENADAX + 3, destino.CoordenadaY * pbMapa.Height / TAMANHOMAPACOORDENADAY + 3);
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