using DAMA.Application.DTOs.SearchDto;

namespace DAMA.Application.Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchResponseDto>> SearchAsync(string term, int id);
    }
}
