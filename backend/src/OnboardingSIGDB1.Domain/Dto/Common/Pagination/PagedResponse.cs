namespace OnboardingSIGDB1.Domain.Dto.Common.Pagination;

/// <summary>
/// Generic paginated response payload.
/// </summary>
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
    
    /// <summary>
    /// Page data items.
    /// </summary>
    public IEnumerable<T> Data { get; set; }

    /// <summary>
    /// Total number of items found.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Current page size.
    /// </summary>
    public int PageSize { get; set; }
}