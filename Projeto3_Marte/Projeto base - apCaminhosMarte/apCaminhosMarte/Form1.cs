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
            int t = (int)pnlArvore.Width / 2;
            arvore.DesenharArvore(g, t);
        }
    }
}