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
        Arvore<Cidade> arvore;                    // arvore binária para armazenar as cidades
        Grafo grafo;

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

                    int qtdCidade = arvore.QuantosDados;                // guardamos numa variavel a quantidade de cidades
                    grafo  = new Grafo(qtdCidade, qtdCidade);     // instancia o grafo
                    LerCaminhos(new StreamReader("CaminhosEntreCidadesMarte.txt"));         // inicia a leitura do arquivo dos caminhos

                    CarregarListBox();                     

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
                grafo[caminho.IdOrigem, caminho.IdDestino] = caminho.Distancia;
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
                    List<PilhaLista<Caminho>> listaCaminhos = new List<PilhaLista<Caminho>>();

                    dgvMelhorCaminho.RowCount = dgvMelhorCaminho.ColumnCount = dgvCaminhoEncontrado.RowCount = dgvCaminhoEncontrado.ColumnCount = 0;

                    if (AcharCaminhos(origem, destino, listaCaminhos))
                    {
                        foreach(PilhaLista<Caminho> caminho in listaCaminhos)
                        {
                            bool primeiro = true;
                            PilhaLista<Caminho> aux = caminho.Clone();
                            dgvCaminhoEncontrado.RowCount++;
                            while(!aux.EstaVazia())
                            {
                                ExibirDgv(dgvCaminhoEncontrado, aux.Desempilhar(), primeiro);
                                primeiro = false;
                            }
                               
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

        private bool AcharCaminhos(int origem, int destino, List<PilhaLista<Caminho>> pilhaCaminho)
        {
            List<PilhaLista<Caminho>> caminhos = new List<PilhaLista<Caminho>>();

            int menor = int.MaxValue, disAtual = 0;
            PilhaLista<Caminho> caminhoAtual = new PilhaLista<Caminho>();

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();

            int atual = origem;

            bool acabou = false;

            while (!acabou)
            {
                int tamanhoAnterior = aux.Tamanho();
                for (int i = 0; i < 23; i++)
                    if (grafo[atual, i] != 0)
                        aux.Empilhar(new Caminho(atual, i, grafo[atual, i]));

                if (tamanhoAnterior == aux.Tamanho())
                    disAtual -= caminhoAtual.Desempilhar().Distancia;

                if (aux.EstaVazia())
                    acabou = true;
                else
                {
                    Caminho c = aux.Desempilhar();

                    while (!caminhoAtual.EstaVazia() && caminhoAtual.OTopo().IdDestino != c.IdOrigem)
                    {
                        disAtual -= caminhoAtual.Desempilhar().Distancia;
                    }
                        

                    caminhoAtual.Empilhar(c);
                    disAtual += c.Distancia;

                    if (c.IdDestino != destino)
                        atual = c.IdDestino;
                    else
                    {
                        caminhos.Add(caminhoAtual.Clone());
                        if(disAtual < menor)
                        {
                            menor = disAtual;
                            melhorCaminho = caminhoAtual.Clone();
                            disAtual = 0;
                        }
                           
                        if (aux.EstaVazia())
                            acabou = true;
                        else
                        {
                            Caminho retorno = aux.Desempilhar();

                            while (!caminhoAtual.EstaVazia() && caminhoAtual.OTopo().IdDestino != retorno.IdOrigem)
                                disAtual -= caminhoAtual.Desempilhar().Distancia;

                            caminhoAtual.Empilhar(retorno);
                            disAtual += retorno.Distancia;

                            if (retorno.IdDestino == destino)
                            {
                                caminhos.Add(caminhoAtual.Clone());
                                acabou = true;

                                if (disAtual < menor)
                                {
                                    menor = disAtual;
                                    melhorCaminho = caminhoAtual.Clone();
                                    disAtual = 0;
                                }
                            }

                            atual = retorno.IdDestino;
                        }
                    }
                }
            }

            return caminhos.Count != 0;
        }

        private void ExibirDgv(DataGridView qualDgv, Caminho insercao, bool primeiro)
        {
            if(!primeiro)
            {
                qualDgv[, qualDgv.RowCount - 1].Value = insercao.IdDestino;
            }
            else
            {
                qualDgv[0, qualDgv.RowCount - 1].Value = insercao.IdOrigem;
               

            }
            
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