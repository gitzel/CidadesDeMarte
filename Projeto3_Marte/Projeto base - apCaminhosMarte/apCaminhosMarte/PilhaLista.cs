using System;

public class PilhaLista<Dado> : IStack<Dado> where Dado : IComparable<Dado>
{
    private NoLista<Dado> topo;
    private int tamanho;
    public PilhaLista()
    { 
        topo = null;
        tamanho = 0;
    }
    public int Tamanho()
    {
        return tamanho;
    }
    public bool EstaVazia()
    {
        return (topo == null);
    }
    public void Empilhar(Dado o)
    {
        NoLista<Dado> novoNo = new NoLista<Dado>(o, topo);
        topo = novoNo; 
        tamanho++;
    }
    public Dado OTopo()
    {
        Dado o;
        if (EstaVazia())
            throw new PilhaVaziaException("Underflow da pilha");
        o = topo.Info;
        return o;
    }
    public Dado Desempilhar()
    {
        if (EstaVazia())
            throw new PilhaVaziaException("Underflow da pilha");
        Dado o = topo.Info;
        topo = topo.Prox;
        tamanho--; 
        return o;
    }

    //public void Inverter()
    //{
    //    PilhaLista<Dado> pilhaInversa = new PilhaLista<Dado>();

    //    while (!EstaVazia())
    //        pilhaInversa.Empilhar(Desempilhar());

    //    topo = pilhaInversa.topo;
    //}
}