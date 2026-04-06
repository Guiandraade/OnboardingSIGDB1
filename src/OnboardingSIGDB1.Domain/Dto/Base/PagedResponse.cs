namespace OnboardingSIGDB1.Domain.Dto.Base;

public class PagedResponse<T> where T : class
{
    public IEnumerable<T> Data { get; set; }
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PagedResponse(IEnumerable<T> data, int total, int pageNumber, int pageSize)
    {
        Data = data;
        Total = total;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}