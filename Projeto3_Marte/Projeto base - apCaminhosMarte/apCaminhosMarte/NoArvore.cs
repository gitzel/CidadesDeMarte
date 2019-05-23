using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosMarte
{
    public class NoArvore<Tipo> : IComparable<NoArvore<Tipo>>, IGravarEmArquivo
                                  where Tipo : IComparable<Tipo>
    {
        Tipo info;  // informação armazenada
        NoArvore<Tipo> esq, dir;

        public NoArvore(Tipo dado, NoArvore<Tipo> esquerda, NoArvore<Tipo> direita)
        {
            Info = dado;
            Esq = esquerda;
            Dir = direita;
        }

        public NoArvore(Tipo dado) : this(dado, null, null)
        {
        }

        public NoArvore() : this(default(Tipo), null, null)
        {
        }
        public Tipo Info { get => info; set => info = value; }
        public  NoArvore<Tipo> Esq { get => esq; set => esq = value; }
        public NoArvore<Tipo> Dir { get => dir; set => dir = value; }

        public int CompareTo(NoArvore<Tipo> outro)
        {
            return this.CompareTo(outro);
        }

        public string ParaArquivo()
        {
            throw new NotImplementedException();
        }
    }
}
