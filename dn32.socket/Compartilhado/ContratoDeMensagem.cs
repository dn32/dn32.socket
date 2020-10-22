using System;

public class ContratoDeMensagem
{
    public string Conteudo { get; set; }
    public bool Retorno { get; set; }
    public Guid IdDaRequisicao { get; set; }
    protected ContratoDeMensagem() { }

    public ContratoDeMensagem(string conteudo, bool retorno, Guid idDaRequisicao)
    {
        Conteudo = conteudo;
        Retorno = retorno;
        IdDaRequisicao = idDaRequisicao;
    }
}
