using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Caminho
{
   const int inicioOrigem = 0,
              tamanhoOrigem = 3,
              inicioDestino = inicioOrigem + tamanhoOrigem,
              tamanhoDestino = 3,
              inicioDistancia = inicioDestino + tamanhoDestino,
              tamanhoDistancia = 5,
              inicioTempo = inicioDestino + tamanhoDistancia,
              tamanhoTempo = 4,
              inicioCusto = inicioTempo + tamanhoTempo,
              tamanhoCusto = 5;

    protected int idOrigem, idDestino, distancia, tempo, custo;

    public Caminho()
    {

    }

    public Caminho(int idOrigem, int idDestino, int distancia, int tempo, int custo)
    {
        IdOrigem = idOrigem;
        IdDestino = idDestino;
        Distancia = distancia;
        Tempo = tempo;
        Custo = custo;
    }

    public int IdOrigem
    {
        get => idOrigem;
        set
        {
            if (value >= 0)
                idOrigem = value;
        }
    }

    public int IdDestino
    {
        get => idDestino;
        set
        {
            if (value >= 0)
                idDestino = value;
        }
    }

    public int Distancia
    {
        get => distancia;
        set
        {
            if (value > 0)
                distancia = 0;
        }
    }

    public int Tempo
    {
        get => tempo;
        set
        {
            if (value >= 0)
                tempo = value;
        }
    }

    public int Custo
    {
        get => custo;
        set
        {
            if (value >= 0)
                custo = value;
        }
    }

    public static Caminho LerRegistro(StreamReader arq)
    {
        Caminho ret = null;
        try
        {
            if (!arq.EndOfStream)
            {
                string linha = arq.ReadLine();

                ret = new Caminho(int.Parse(linha.Substring(inicioOrigem, tamanhoOrigem)),
                                  int.Parse(linha.Substring(inicioDestino, tamanhoDestino)),
                                  int.Parse(linha.Substring(inicioDistancia,tamanhoDistancia)),
                                  int.Parse(linha.Substring(inicioTempo, tamanhoTempo)),
                                  int.Parse(linha.Substring(inicioCusto, tamanhoCusto)));
            }
        }
        catch (Exception erro)
        {
            throw new Exception(erro.Message);
        }
        return ret;
    }

}

