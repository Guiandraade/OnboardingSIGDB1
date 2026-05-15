namespace OnboardingSIGDB1.Domain.Dto.Common.Pagination;

public class PagedResponse<T> where T : class
{
    public PagedResponse() 
    { 
        Data = Enumerable.Empty<T>();
    }
    
    public PagedResponse(IEnumerable<T> data, int total, int pageNumber, int pageSize)
    {
        Data = data;
        Total = total;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
    
    public IEnumerable<T> Data { get; set; }
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}