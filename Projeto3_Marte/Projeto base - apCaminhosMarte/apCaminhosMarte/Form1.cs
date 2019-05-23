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
        public Form1()
        {
            InitializeComponent();
        }

        private void TxtCaminhos_DoubleClick(object sender, EventArgs e)
        {
           
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Buscar caminhos entre cidades selecionadas");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore = new Arvore<Cidade>();

            if(dlgArquivo.ShowDialog() == DialogResult.OK)
            {
                StreamReader arq = new StreamReader(dlgArquivo.FileName, Encoding.Default, true);
                LerArquivo(arq);
            }

            pnlArvore.Invalidate();
        }

        

        private void DesenharArvore(bool primeiro, NoArvore<Cidade> raiz, int x, int y, double angulo, double i, double compr, Graphics g)
        {
            int xf, yf;
            if (raiz != null)
            {
                Pen caneta = new Pen(Color.FromArgb(39, 60, 117));
                xf = (int)Math.Round(x + Math.Cos(angulo) * compr);
                yf = (int)Math.Round(y + Math.Sin(angulo) * compr);
                if (primeiro)
                    yf = 25;

                g.DrawLine(caneta, x, y, xf, yf);

                DesenharArvore(false, raiz.Esq, xf, yf, Math.PI / 2 + i,
                                                 i * 0.60, compr * 0.8, g);
                DesenharArvore(false, raiz.Dir, xf, yf, Math.PI / 2 - i,
                                                  i * 0.60, compr * 0.8, g);
                // sleep(100);
                SolidBrush preenchimento = new SolidBrush(Color.FromArgb(32, 32, 32));
                g.FillRectangle(preenchimento, xf - 15, yf - 15, 40, 40);
                g.DrawString(raiz.Info.Id +"\n\n" + raiz.Info.Nome, new Font("Courier New", 12),
                              new SolidBrush(Color.FromArgb(232, 65, 24)), xf - 7, yf - 5);
            }
        }

        private void LerArquivo(StreamReader arquivo)
        {
            while(!arquivo.EndOfStream)
            {
                arvore.Incluir(Cidade.LerRegistro(arquivo));
            }
            arquivo.Close();
        }

        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DesenharArvore(true, arvore.Raiz, (int)pnlArvore.Width / 2, 0, Math.PI / 2, Math.PI / 2.5, 350, g);
        }
    }
}