namespace CardCore.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetProducts(string sTerm = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();
    }
}
