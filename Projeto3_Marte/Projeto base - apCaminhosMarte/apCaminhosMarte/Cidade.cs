using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Cidade : IComparable<Cidade>
{
    /*
     Atributos inteiros constantes que armazenam os inicios e tamanhos de cada um dos atributos da classe quando estes
     estiverem em um arquivo texto formatado corretamente.
    */
    const int inicioId = 0,
              tamanhoId = 3,
              inicioNome = inicioId + tamanhoId,
              tamanhoNome = 15,
              inicioCoordenadaX = inicioNome + tamanhoNome,
              tamanhoCoordenadaX = 5,
              inicioCoordenadaY = inicioCoordenadaX + tamanhoCoordenadaX,
              tamanhoCoordenadaY = 5;

    /*
    Atributos inteiros que representam o id da cidade e as coordenadas de onde ela está localizada.
    */
    int id, coordenadaX, coordenadaY;

    /*
     Atributo string que representam o nome da cidade.
    */
    string nome;

    /*
     Propriedade que retorna e altera o valor do atributo inteiro que representa o id da cidade.
    */
    public int Id
    {
        get => id;
        set
        {
            if (value >= 0)
                id = value;
        }
    }

    /*
     Propriedade que retorna e altera o valor do atributo que representa o nome da cidade.
    */
    public string Nome
    {
        get => nome;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                nome = value;
        }
    }

    /*
      Propriedade que retorna e altera o valordo atributo inteiro que representa a coordenada horizontal da cidade.
    */
    public int CoordenadaX
    {
        get => coordenadaX;
        set
        {
            if (value >= 0)
                coordenadaX = value;
        }
    }

    /*
      Propriedade que retorna e altera o valordo atributo inteiro que representa a coordenada vertical da cidade.
    */
    public int CoordenadaY
    {
        get => coordenadaY;
        set
        {
            if (value >= 0)
                coordenadaY = value;
        }
    }

    /*
      Sobrecarga do construtor vazio que não recebe parâmetros e inicia os atributos com valores padrões.
    */
    public Cidade() 
    {
        id = coordenadaX = coordenadaY = -1;
        nome = null;
    }

    /*
      Sobrecarga do construtor que não recebe como parâmetro todos os atributos da classe e os instância com base nos parâmetros.
      @params os atributos inteiros id da cidade, coordenadas de onde está localizada e uma string com o nome da cidade.
    */
    public Cidade(int id, string nome, int coordenadaX, int coordenadaY)
    {
        Id = id;
        Nome = nome;
        CoordenadaX = coordenadaX;
        CoordenadaY = coordenadaY;
    }

    /*
     Sobrecarga do construtor que não recebe como parâmetro só o atributo id e inicia os outros atributos com valores padrões.
     @params os atributos inteiros id da cidade.
    */
    public Cidade(int id)
    {
        Id = id;
        coordenadaX = coordenadaY = -1;
        nome = null;
    }

    /*
     Método que lê uma linha de um StreamReader de um arquivo que contém as cidades e retorna uma Cidade com base na linha
     lida daquele arquivo.
     @params o StreamReader do arquivo que está sendo lido.
     @return uma Cidade com as informações contidas naquelas linha lida do arquivo.
    */
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
            throw new IOException(erro.Message);
        }
        return ret;
    }

    /*
      Método que compara duas cidades com base no id.
      @params outro Caminho que será usado para comparar.
      @return um int que é a diferença entre as duas distâncias e se for a mesma retorna 0.
    */
    public int CompareTo(Cidade outra)
    {
        return this.id - outra.Id;
    }

    /*
      Método que gera uma representação textual de uma cidade.
      @return uma string com a representação textual da classe.
    */
    public override string ToString()
    {
        return $"{id:00}\n\n{nome}";
    }
}

