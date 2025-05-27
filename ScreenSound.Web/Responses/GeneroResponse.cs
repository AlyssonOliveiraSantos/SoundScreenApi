namespace ScreenSound.Web.Responses
{
    public record GeneroResponse(int id, string Nome, string Descricao)
    {
        public override string ToString()
        {
            return $"{this.Nome}";
        }
    }
}
