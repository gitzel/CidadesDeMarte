using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Cidade : IComparable<Cidade>
{
    const int inicioId = 0,
              tamanhoId = 3,
              inicioNome = inicioId + tamanhoId,
              tamanhoNome = 15,
              inicioCoordenadaX = inicioNome + tamanhoNome,
              tamanhoCoordenadaX = 5,
              inicioCoordenadaY = inicioCoordenadaX + tamanhoCoordenadaX,
              tamanhoCoordenadaY = 5;

    int id, coordenadaX, coordenadaY;
    string nome;

    public int Id
    {
        get => id;
        set
        {
            if (value >= 0)
                id = value;
        }
    }

    public string Nome
    {
        get => nome;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                nome = value;
        }
    }

    public int CoordenadaX
    {
        get => coordenadaX;
        set
        {
            if (value >= 0)
                coordenadaX = value;
        }
    }
    public int CoordenadaY
    {
        get => coordenadaY;
        set
        {
            if (value >= 0)
                coordenadaY = value;
        }
    }

    public Cidade()
    {
        id = CoordenadaX = coordenadaY = -1;
        nome = null;
    }

    public Cidade(int id, string nome, int coordenadaX, int coordenadaY)
    {
        Id = id;
        Nome = nome;
        CoordenadaX = coordenadaX;
        CoordenadaY = coordenadaY;
    }
    
    public static Cidade LerRegistro(StreamReader arq)
    {
        Cidade ret = null;
        try
        {
            if (!arq.EndOfStream)
            {
                ret = new Cidade();
                string linha = arq.ReadLine();
                ret.Id = int.Parse(linha.Substring(inicioId, tamanhoId));
                ret.Nome = linha.Substring(inicioNome, tamanhoNome);
                ret.CoordenadaX = int.Parse(linha.Substring(inicioCoordenadaX, tamanhoCoordenadaX));
                ret.CoordenadaY = int.Parse(linha.Substring(inicioCoordenadaY, tamanhoCoordenadaY));
            }
        }
        catch (Exception erro)
        {
            throw new Exception(erro.Message);
        }
        return ret;
    }

    public int CompareTo(Cidade outra)
    {
        return this.id - outra.Id;
    }
}

