namespace Personal.Common.Model;

public class PagedList<TEntity> where TEntity : class
{
    /// <summary>
    /// 数据集
    /// </summary>
    public List<TEntity> DataList { get; }

    /// <summary>
    /// 当前页
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// 当前页数据条数
    /// </summary>
    public int CurrentCount => DataList.Count;

    /// <summary>
    /// 是否有前一页
    /// </summary>
    public bool HasPrev => CurrentPage > 1;

    /// <summary>
    /// 是否有后一页
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<TEntity> dataList, int currentPage, int pageSize, int count)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(count * 1.0 / pageSize);
        DataList = dataList;
    }
}