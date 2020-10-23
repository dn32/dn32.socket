using System;

internal class DnContratoDeMensagem
{
    public string Conteudo { get; set; }
    public bool Retorno { get; set; }
    public Guid IdDaRequisicao { get; set; }
    protected DnContratoDeMensagem() { }

    public DnContratoDeMensagem(string conteudo, bool retorno, Guid idDaRequisicao)
    {
        Conteudo = conteudo;
        Retorno = retorno;
        IdDaRequisicao = idDaRequisicao;
    }
}
