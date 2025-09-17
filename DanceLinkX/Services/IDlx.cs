namespace DanceLinkX.Services;

public interface IDlx<TRow, TColumn>
    where TRow : notnull
    where TColumn : notnull
{
    IDlxHeadRoot<TRow, TColumn, TRow> RowHead { get; }

    IDlxHeadRoot<TRow, TColumn, TColumn> ColumnHead { get; }

    /// <summary>
    /// 是否正在搜索
    /// </summary>
    bool IsDancing { get; }

    /// <summary>
    /// 开始搜索
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">正在搜索</exception>
    IEnumerable<List<TRow>> Dance();
}

public interface IDlxAsync<TRow, TColumn> : IDlx<TRow, TColumn>
    where TRow : notnull
    where TColumn : notnull
{
    /// <summary>
    /// 开始搜索
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">正在搜索</exception>
    IAsyncEnumerable<List<TRow>> DanceAsync();

    IEnumerable<List<TRow>> IDlx<TRow, TColumn>.Dance() => DanceAsync().ToBlockingEnumerable();
}
