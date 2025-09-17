namespace DanceLinkX.DlxDefault;

public class DlxSolver<TRow, TColumn>(
    Dlx<TRow, TColumn> dlx,
    DlxHead<TRow, TColumn, TRow> row,
    DlxResultNode<TRow> baseResultNode
) : IDisposable, IAsyncDisposable
    where TRow : notnull
    where TColumn : notnull
{
    private readonly Stack<DlxHead<TRow, TColumn, TRow>> _rowStack = [];

    private readonly Stack<DlxHead<TRow, TColumn, TColumn>> _columnStack = [];

    private readonly Dictionary<DlxHead<TRow, TColumn, TColumn>, int> _adjectedColumns = [];

    private bool UseHeap => dlx.UseHeap;

    public void Dispose()
    {
        Recover();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Recover();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public bool CheckColumnEmpty(out DlxResultNode<TRow> dlxResultNode)
    {
        dlxResultNode = new DlxResultNode<TRow>(row.Value);
        baseResultNode.AddChild(dlxResultNode);
        if (dlx.ColumnHeadRoot.IsEmpty())
        {
            dlxResultNode.SetValid();
            dlxResultNode.SetLeaf();
            Recover();
            return true;
        }
        return false;
    }

    internal IEnumerable<DlxResultNode<TRow>> Solve()
    {
        Cover();
        if (CheckColumnEmpty(out var dlxResultNode))
        {
            yield return dlxResultNode;
            yield break;
        }
        foreach (var row in dlx.SelectRows())
        {
            using var solver = new DlxSolver<TRow, TColumn>(dlx, row, dlxResultNode);
            foreach (var resultNode in solver.Solve())
            {
                yield return resultNode;
            }
        }
        Recover();
    }

    internal async IAsyncEnumerable<DlxResultNode<TRow>> SolveAsync()
    {
        await Task.CompletedTask;
        Cover();
        if (CheckColumnEmpty(out var dlxResultNode))
        {
            yield return dlxResultNode;
            yield break;
        }
        foreach (var row in dlx.SelectRows())
        {
            using var solver = new DlxSolver<TRow, TColumn>(dlx, row, dlxResultNode);
            await foreach (var resultNode in solver.SolveAsync())
            {
                yield return resultNode;
            }
        }
        Recover();
    }

    private void Cover()
    {
        row.Cover();
        _rowStack.Push(row);
        HashSet<DlxHead<TRow, TColumn, TColumn>> willSetColumn = null!;
        if (UseHeap)
        {
            willSetColumn = [];
        }
        foreach (var node in row.EnumerateNodes())
        {
            var column = node.Column;
            column.Cover();
            _columnStack.Push(column);
            if (UseHeap)
            {
                _adjectedColumns[column] = column.NodeCount;
                dlx.ColumnSelecter.SetValue(column, int.MaxValue);
            }
            column.NodeCount -= 1;
        }
        foreach (var node in row.EnumerateNodes())
        {
            foreach (var node1 in node.Column.EnumerateNodes())
            {
                var row1 = node1.Row;
                if (row1.Cover())
                {
                    _rowStack.Push(row1);
                    foreach (var column1 in row1.EnumerateNodes().Select(n => n.Column))
                    {
                        column1.NodeCount -= 1;
                        if (UseHeap && !column1.IsCover)
                        {
                            _adjectedColumns.TryAdd(column1, column1.NodeCount);
                            willSetColumn.Add(column1);
                        }
                    }
                }
            }
        }
        if (UseHeap)
        {
            foreach (var column in willSetColumn)
            {
                dlx.ColumnSelecter.SetValue(column, column.NodeCount);
            }
        }
    }

    private void Recover()
    {
        foreach (var column in _columnStack)
        {
            column.Recover();
        }
        foreach (var row in _rowStack)
        {
            row.Recover();
            foreach (var node in row.EnumerateNodes())
            {
                var column = node.Column;
                column.NodeCount += 1;
            }
        }
        _rowStack.Clear();
        _columnStack.Clear();
        if (UseHeap)
        {
            foreach (var (column, count) in _adjectedColumns)
            {
                dlx.ColumnSelecter.SetValue(column, count);
            }
            _adjectedColumns.Clear();
        }
    }
}
