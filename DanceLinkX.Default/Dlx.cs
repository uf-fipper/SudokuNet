using System.Diagnostics.CodeAnalysis;
using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public class Dlx<TRow, TColumn> : IDlx<TRow, TColumn>, IDlxAsync<TRow, TColumn>
    where TRow : notnull
    where TColumn : notnull
{
    public Dlx()
    {
        RowHeadRoot = new DlxRowRoot<TRow, TColumn>();
        ColumnHeadRoot = new DlxColumnRoot<TRow, TColumn>();
    }

    public Dlx(IEnumerable<(TRow row, TColumn rolumn)> nodes)
        : this()
    {
        AddNodes(nodes);
    }

    public DlxRowRoot<TRow, TColumn> RowHeadRoot { get; }

    public DlxColumnRoot<TRow, TColumn> ColumnHeadRoot { get; }

    /// <summary>
    /// 使用堆反而会慢很多
    /// </summary>
    internal DlxSolverHeap<DlxHead<TRow, TColumn, TColumn>, int> ColumnSelecter { get; set; } =
        null!;

    public bool UseHeap { get; init; } = false;
    private readonly Dictionary<(TRow, TColumn), DlxNode<TRow, TColumn>> NodesMap = [];

    public bool IsDancing { get; internal set; } = false;

    IDlxHeadRoot<TRow, TColumn, TRow> IDlx<TRow, TColumn>.RowHead => RowHeadRoot;

    IDlxHeadRoot<TRow, TColumn, TColumn> IDlx<TRow, TColumn>.ColumnHead => ColumnHeadRoot;

    public IEnumerable<DlxResultNode<TRow>> DanceRaw()
    {
        return DanceRawAsync().ToBlockingEnumerable();
    }

    public async IAsyncEnumerable<DlxResultNode<TRow>> DanceRawAsync()
    {
        await Task.CompletedTask;
        ThrowIfDancing();
        IsDancing = true;
        if (UseHeap)
        {
            ColumnSelecter = new(ColumnHeadRoot.EnumerateHeads().Select(x => (x, x.NodeCount)));
        }
        try
        {
            if (ColumnHeadRoot.IsEmpty())
                yield break;
            var resultNode = new DlxResultNode<TRow>(default);
            foreach (var row in SelectRows())
            {
                using var solver = new DlxSolver<TRow, TColumn>(this, row, resultNode);
                await foreach (var result in solver.SolveAsync())
                {
                    yield return result;
                }
            }
        }
        finally
        {
            IsDancing = false;
        }
    }

    public IEnumerable<List<TRow>> Dance()
    {
        return DanceRawAsync().ToBlockingEnumerable().Select(x => x.GetRowList());
    }

    public async IAsyncEnumerable<List<TRow>> DanceAsync()
    {
        await foreach (var result in DanceRawAsync())
        {
            yield return result.GetRowList();
        }
    }

    internal IEnumerable<DlxHead<TRow, TColumn, TRow>> SelectRows()
    {
        DlxHead<TRow, TColumn, TColumn> column;
        if (UseHeap)
        {
            column = ColumnSelecter.Select();
        }
        else
        {
            column = ColumnHeadRoot.EnumerateHeads().MinBy(x => x.NodeCount)!;
        }
        // var column = ColumnHeadRoot.Next;
        return column.EnumerateNodes().Select(n => n.Row).Where(x => !x.IsCover);
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="rowValue"></param>
    /// <param name="columnValue"></param>
    /// <returns>返回true成功添加，false则代表node已存在</returns>
    /// <exception cref="InvalidOperationException">正在搜索</exception>
    public bool AddNode(TRow rowValue, TColumn columnValue)
    {
        ThrowIfDancing();
        return AddNodeInternal(rowValue, columnValue);
    }

    public bool AddNode((TRow row, TColumn column) node)
    {
        ThrowIfDancing();
        return AddNodeInternal(node.row, node.column);
    }

    /// <summary>
    /// 批量添加节点
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">正在搜索</exception>
    public bool[] AddNodes(IEnumerable<(TRow row, TColumn column)> nodes)
    {
        ThrowIfDancing();
        return nodes.Select(n => AddNodeInternal(n.row, n.column)).ToArray();
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="rowValue"></param>
    /// <param name="columnValue"></param>
    /// <returns>返回true成功添加，false则代表node已存在</returns>
    public bool AddNodeInternal(TRow rowValue, TColumn columnValue)
    {
        if (NodesMap.ContainsKey((rowValue, columnValue)))
        {
            return false;
        }
        RowHeadRoot.TryAddHead(rowValue, out var row);
        ColumnHeadRoot.TryAddHead(columnValue, out var column);
        var node = new DlxNode<TRow, TColumn>(row, column);
        row.AddNode(node);
        column.AddNode(node);
        NodesMap[(rowValue, columnValue)] = node;
        return true;
    }

    public DlxNode<TRow, TColumn> GetNode(TRow row, TColumn column)
    {
        return NodesMap[(row, column)];
    }

    public bool TryGetNode(
        TRow row,
        TColumn column,
        [MaybeNullWhen(false)] out DlxNode<TRow, TColumn>? node
    )
    {
        return NodesMap.TryGetValue((row, column), out node);
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="InvalidOperationException">正在搜索</exception>
    public void ThrowIfDancing()
    {
        if (IsDancing)
            throw new InvalidOperationException();
    }

    public Dlx<TRow, TColumn> Clone()
    {
        var dlx = new Dlx<TRow, TColumn>();
        foreach (var (row, column) in NodesMap.Keys)
        {
            dlx.AddNodeInternal(row, column);
        }
        return dlx;
    }

    IEnumerable<List<TRow>> IDlx<TRow, TColumn>.Dance() => Dance();
}
