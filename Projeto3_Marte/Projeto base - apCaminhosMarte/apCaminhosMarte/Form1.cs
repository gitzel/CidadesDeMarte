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

        /*
         Constantes inteiras que armazenam a largura e a altura real do mapa de marte.
        */
        const int TAMANHOX = 4096, TAMANHOY = 2048;   
        
        /*
         Variáveis inteiras que indicam qual posição da lista de caminhos está o menor caminho e qual caminho está selecionado. 
        */
        int selecionado, menor;

        /*
          Lista que irá armazenar todos os caminhos entre as cidades.
        */
        List<PilhaLista<Caminho>> listaCaminhos;

        /*
          Árvore que armazena todas cidades do mapa.
        */
        Arvore<Cidade> arvore;

        /*
          Grafo usado para guardar as adjacência das cidades.
        */
        Grafo grafo;

        public Form1()
        {
            InitializeComponent();
        }

        /*
         Quando o formulário carrega inicia a lista de caminhos vazio, instância a arvore de cidades e atribui um valor 
         negativo para selecionado e menor. Chama o método para ler os arquivos.
        */
        private void Form1_Load(object sender, EventArgs e) 
        {
            listaCaminhos = null;
            arvore = new Arvore<Cidade>();            
            menor = selecionado = -1;         
            LeituraDosArquivos();          
        }

        /*
          Método que lê um arquivo de cidades escolhido pelo usuário e inclui cada uma delas na árvore. Instanciamos o grafo
          com uma matriz de dimensões iguais a quantidade de cidades. Chamamo os métodos para ler os caminhos, carregar os dados no 
          listBox e chama o evento paint do mapa para desenhar pontos nas cidades. 
          @throws caso não consiga ler o arquivo.
        */
        private void LeituraDosArquivos()       
        {
            try
            {
                if (dlgArquivo.ShowDialog() == DialogResult.OK)     
                {
                    ConstruirArvore(new StreamReader(dlgArquivo.FileName, Encoding.Default, true));      

                    int qtdCidade = arvore.QuantosDados;                
                    grafo  = new Grafo(qtdCidade, qtdCidade);   
                    LerCaminhos(new StreamReader("CaminhosEntreCidadesMarte.txt"));         // inicia a leitura do arquivo dos caminhos

                    CarregarListBox();                     

                    pbMapa.Invalidate();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao ler arquivos!", "Arquivo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
          Constrói uma árvore com as cidades baseado em um arquivo recebido com parâmetro.
          @params um StreamReader que contém o arquivo que será lido.
        */
        private void ConstruirArvore(StreamReader arquivo)
        {
            while (!arquivo.EndOfStream)
                arvore.Incluir(Cidade.LerRegistro(arquivo));

            arquivo.Close();
        }

        /*
             Adiciona aos dois listBox os dados de cidade contidos na árvore em uma sequência InOrdem de forma que os
             dados acabam ordenados.
        */
        private void CarregarListBox()
        {
            arvore.InOrdem((Cidade c) =>
            {
                lsbOrigem.Items.Add($"{c.Id:00} - {c.Nome}");
                lsbDestino.Items.Add($"{c.Id:00} - {c.Nome}");
            });
        }

        /*
           Lê os caminhos de um arquivo e adiciona na matriz do grafo. 
        */
        private void LerCaminhos(StreamReader arq)
        {
            while (!arq.EndOfStream)
            {
                Caminho caminho = Caminho.LerRegistro(arq);
                grafo[caminho.IdOrigem, caminho.IdDestino] = caminho.Distancia;
            }
            arq.Close();
        }

        /*
         Evento click do botão de buscar caminhos que chama os métodos para encontrar um caminho e exibí-lo.
         Avisa o usuário se ele selecionar como origem e destino a mesma cidade e se não houver um caminho possível entre 
         duas cidades por meio de uma MessageBox.
        */
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
                    AcharCaminhos(origem, destino);

                    dgvMelhorCaminho.RowCount = dgvMelhorCaminho.ColumnCount = dgvCaminhoEncontrado.RowCount = dgvCaminhoEncontrado.ColumnCount = 0;

                    if (listaCaminhos.Count != 0)
                        MostrarCaminhos();
                    else
                    {
                        selecionado = -1;
                        pbMapa.Invalidate();
                        dgvMelhorCaminho.RowCount = dgvMelhorCaminho.ColumnCount = 0;
                        MessageBox.Show("Não existe caminho entre essas cidades!", "Viagem inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /*
         Método que utiliza backtraking e pilhas para encontrar todos os caminhos entre dois pontos. Armazena todos os caminhos
         em uma lista e determina qual posição guarda o menor.
         @params dois inteiros sendo que a origem a cidade de onde estamos saindo e o destino a cidade onde queremos chegar.
        */
        private void AcharCaminhos(int origem, int destino)
        {
            listaCaminhos = new List<PilhaLista<Caminho>>();

            int menorDistancia = int.MaxValue, disAtual = 0;
            PilhaLista<Caminho> caminhoAtual = new PilhaLista<Caminho>();

            PilhaLista<Caminho> aux = new PilhaLista<Caminho>();

            bool[] jaPassou = new bool[23];
            for (int i = 0; i < 23; i++)
                jaPassou[i] = false;

            int atual = origem;

            bool acabou = false;

            while (!acabou)
            {
                int tamanhoAnterior = aux.Tamanho();
                for (int i = 0; i < 23; i++)
                    if (grafo[atual, i] != 0 && !jaPassou[i])
                        aux.Empilhar(new Caminho(atual, i, grafo[atual, i]));

                if (!aux.EstaVazia() && tamanhoAnterior == aux.Tamanho())
                {
                    Caminho cam = caminhoAtual.Desempilhar();
                    disAtual -= cam.Distancia;
                    jaPassou[cam.IdDestino] = true;
                }

                if (aux.EstaVazia())
                    acabou = true;
                else
                {
                    Caminho c = aux.Desempilhar();

                    while (!caminhoAtual.EstaVazia() && caminhoAtual.OTopo().IdDestino != c.IdOrigem)
                    {
                        Caminho cam = caminhoAtual.Desempilhar();
                        disAtual -= cam.Distancia;
                        jaPassou[cam.IdDestino] = false;
                    }

                    caminhoAtual.Empilhar(c);
                    disAtual += c.Distancia;

                    if (c.IdDestino != destino)
                    {
                        jaPassou[c.IdOrigem] = true;
                        atual = c.IdDestino;
                    }
                    else
                    {
                        listaCaminhos.Add(caminhoAtual.Clone());
                        if (disAtual < menorDistancia)
                        {
                            menor = listaCaminhos.Count - 1;
                            menorDistancia = disAtual;
                        }

                        if (aux.EstaVazia())
                            acabou = true;
                        else
                        {
                            Caminho retorno = aux.Desempilhar();
                     
                            while (!caminhoAtual.EstaVazia() && caminhoAtual.OTopo().IdDestino != retorno.IdOrigem)
                            {
                                Caminho cam = caminhoAtual.Desempilhar();
                                disAtual -= cam.Distancia;
                                jaPassou[cam.IdDestino] = false;
                            }

                            caminhoAtual.Empilhar(retorno);
                            jaPassou[retorno.IdDestino] = true;
                            disAtual += retorno.Distancia;

                            while(retorno.IdDestino == destino && !acabou)
                            {
                                listaCaminhos.Add(caminhoAtual.Clone());

                                if (disAtual < menorDistancia)
                                {
                                    menor = listaCaminhos.Count - 1;
                                    menorDistancia = disAtual;
                                }

                                if (!aux.EstaVazia())
                                {
                                    retorno = aux.Desempilhar();
                                    while (!caminhoAtual.EstaVazia() && caminhoAtual.OTopo().IdDestino != retorno.IdOrigem)
                                    {
                                        Caminho cam = caminhoAtual.Desempilhar();
                                        disAtual -= cam.Distancia;
                                        jaPassou[cam.IdDestino] = false;
                                    }

                                    caminhoAtual.Empilhar(retorno);
                                    disAtual += retorno.Distancia;
                                }
                                else
                                    acabou = true;
                            }

                            atual = retorno.IdDestino;
                        }
                    }
                }
            }
        }

        /*
          Exibe todos os caminhos em um dataGridView de caminhos e o menor caminho em um outro.
        */
        private void MostrarCaminhos()
        {
            foreach (PilhaLista<Caminho> caminho in listaCaminhos)
            {
                int posicao = 0;
                PilhaLista<Caminho> aux = caminho.Clone();
                aux.Inverter();

                if (dgvCaminhoEncontrado.RowCount == menor)
                {
                    dgvMelhorCaminho.RowCount++;
                    dgvMelhorCaminho.ColumnCount = aux.Tamanho() + 1;
                }

                dgvCaminhoEncontrado.RowCount++;


                if (dgvCaminhoEncontrado.ColumnCount <= aux.Tamanho())
                    dgvCaminhoEncontrado.ColumnCount = aux.Tamanho() + 1;

                while (!aux.EstaVazia())
                {
                    Caminho c = aux.Desempilhar();
                    if (dgvCaminhoEncontrado.RowCount - 1 == menor)
                        ExibirDgv(dgvMelhorCaminho, c, posicao);

                    ExibirDgv(dgvCaminhoEncontrado, c, posicao);
                    posicao++;
                }
            }

            selecionado = menor;
            dgvCaminhoEncontrado.Rows[selecionado].Selected = true;
            pbMapa.Invalidate();
        }

        /*
          Adiciona um caminho a um indice determinado no dataGridView.
          @params o DataGridView que iremos modificar, o caminho que iremos inserir e um inteiro chamado indice que determina
          que posição ou coluna será incluido.
        */
        private void ExibirDgv(DataGridView qualDgv, Caminho insercao, int indice)
        {
            if(indice == 0 )
               qualDgv[indice, qualDgv.RowCount - 1].Value = insercao.IdOrigem;

            qualDgv[++indice, qualDgv.RowCount - 1].Value = insercao.IdDestino;
        }

        /*
         Evento paint do panel que exibirá a árvore. Chama o método público de desenhar árvore e passa como parâmetro o Graphics
         do panel e metade da largura dele para começar desenhando no meio.
        */
        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int tamanho = (int)pnlArvore.Width / 2;
            arvore.DesenharArvore(g, tamanho);
        }

        /*
         Evento paint do pictureBox mapa que exibirá as cidades e os caminhos. Desenha os pontos em cada cidade, seguindo
         PreOrdem da Árvore. Também exibe um caminho selecionado pelo usuário caso ele não seja negativo. 
        */
        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            arvore.PreOrdem((Cidade c) =>
            {
                float coordenadaX = c.CoordenadaX * pbMapa.Width / TAMANHOX;
                float coordenadaY = c.CoordenadaY * pbMapa.Height / TAMANHOY;
                g.FillEllipse(
                 new SolidBrush(Color.Black),
                 coordenadaX, coordenadaY, 10f, 10f
               );
                g.DrawString(c.Nome, new Font("Courier New", 8, FontStyle.Bold),
                             new SolidBrush(Color.FromArgb(32, 32, 32)), coordenadaX + 12, coordenadaY - 10);
            });

            if (selecionado >= 0)
            {
                PilhaLista<Caminho> aux = listaCaminhos[selecionado].Clone();
               
                while (!aux.EstaVazia())
                {
                    Caminho possivelCaminho = aux.Desempilhar();

                    Cidade origem = arvore.ExisteDado(new Cidade(possivelCaminho.IdOrigem));
                    Cidade destino = arvore.ExisteDado(new Cidade(possivelCaminho.IdDestino));
                    using (var pen = new Pen(Color.FromArgb(211, 47, 47), 4))
                    {
                        
                        int origemX = origem.CoordenadaX * pbMapa.Width / TAMANHOX + 5;
                        int origemY = origem.CoordenadaY * pbMapa.Height / TAMANHOY + 3;
                        int destinoX = destino.CoordenadaX * pbMapa.Width / TAMANHOX +3;
                        int destinoY = destino.CoordenadaY * pbMapa.Height / TAMANHOY +5;


                        
                        if (destinoX - origemX > 2 * pbMapa.Width / 4)
                        {
                            g.DrawLine(pen, origemX, origemY, 0, origemY);
                            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                            g.DrawLine(pen, pbMapa.Width, origemY, destinoX, destinoY);
                        }
                        else
                        {
                            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                            g.DrawLine(pen, origemX,origemY, destinoX,  destinoY);
                        }
                    }
                }
            }
        }

        /*
           Evento que muda o valor do selecionado dependo da linha do dataGridView que foi selecionado. Chama o evento paint
           do pictureBox mapa para desenhar o caminho selecionado.
        */
        private void dgvCaminhoEncontrado_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCaminhoEncontrado.SelectedRows.Count != 0)
            {
                selecionado = dgvCaminhoEncontrado.Rows.IndexOf(dgvCaminhoEncontrado.SelectedRows[0]);
                pbMapa.Invalidate();
            }
        }

        /*
          Evento click do TollStripMenu que chama o método de ler arquivos.
        */
        private void cidadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LeituraDosArquivos();
        }
    }
}