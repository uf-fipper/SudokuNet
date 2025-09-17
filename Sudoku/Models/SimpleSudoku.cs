using System.Numerics;
using DanceLinkX.Services;

namespace Sudoku.Models;

/// <summary>
/// 数独类，相当于一个边长为size的二维数组，其中size只能是平方数
/// </summary>
public class SimpleSudoku
    : IEquatable<SimpleSudoku>,
        IEqualityOperators<SimpleSudoku, SimpleSudoku, bool>
{
    public enum SolveType
    {
        HasValue,
        RowHasNumber,
        ColumnHasNumber,
        BlockHasNumber,
    }

    public record SolveNode(SolveType Type, int I, int J);

    /// <summary>
    /// 一个宫格的边长
    /// </summary>
    public int MinSize { get; }

    /// <summary>
    /// 整个迷宫的边长
    /// </summary>
    public int Size { get; }

    private readonly int[,] _board;

    /// <summary>
    /// _rowNumberCount[i, value]代表第i行存在value的数量
    /// </summary>
    private readonly int[,] _rowNumberCount;

    /// <summary>
    /// _columnNumberCount[j, value]代表第j行存在value的数量
    /// </summary>
    private readonly int[,] _columnNumberCount;

    /// <summary>
    /// _blockNumberCount[k, value]代表第k个宫格中存在value的数量
    /// </summary>
    private readonly int[,] _blockNumberCount;

    private int _hashCode = 0;

    private bool GetMinSize(int size, out int minSize)
    {
        minSize = (int)Math.Sqrt(size);
        if (minSize * minSize == size)
        {
            return true;
        }
        return false;
    }

    public SimpleSudoku(IList<List<int>> board)
        : this(board.Select(x => x.ToArray()).ToArray()) { }

    public SimpleSudoku(IList<IList<int>> board)
    {
        int size = board.Count;
        if (size > 5 * 5)
        {
            throw new ArgumentException($"size too large: {size}");
        }
        if (!GetMinSize(size, out int minSize))
        {
            throw new ArgumentException($"size not valid: {size}");
        }
        MinSize = minSize;
        Size = minSize * minSize;
        if (board.Count != size)
        {
            throw new ArgumentException($"board.Count != {size}");
        }
        _board = new int[size, size];
        _rowNumberCount = new int[size, size + 1];
        _columnNumberCount = new int[size, size + 1];
        _blockNumberCount = new int[size, size + 1];
        _hashCode = 0;
        for (int i = 0; i < size; i++)
        {
            var row = board[i];
            if (row.Count != size)
            {
                throw new ArgumentException($"board[{i}].Count != {size}");
            }
            for (int j = 0; j < size; j++)
            {
                int value = row[j];
                if (value < 0 || value > size)
                {
                    throw new ArgumentException($"board[{i}][{j}] < 1 || board[{i}][{j}] > {size}");
                }
                if (value != 0)
                {
                    SetValueInternal(i, j, value);
                }
            }
        }
    }

    /// <summary>
    /// 求解并返回一个新的数独，不会改变原数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>null则无解</returns>
    public SimpleSudoku? SolveNew(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            IDlx<(int, int, int), SolveType>
        > createDlx
    )
    {
        var result = Clone();
        if (result.Solve(createDlx))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// 求解数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>返回值代表是否有解</returns>
    public bool Solve(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            IDlx<(int, int, int), SolveType>
        > createDlx
    )
    {
        var nodes = SolveGetNodes();
        var dlx = createDlx(nodes);
        using var solver = dlx.Dance().GetEnumerator();
        if (!solver.MoveNext())
        {
            return false;
        }
        var values = solver.Current;
        SolveSetValues(values);
        return true;
    }

    /// <summary>
    /// 求解并返回一个新的数独，不会改变原数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>null则无解</returns>
    public async Task<SimpleSudoku?> SolveNewAsync(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            Task<IDlxAsync<(int, int, int), SolveType>>
        > createDlx
    )
    {
        var result = Clone();
        if (await result.SolveAsync(createDlx))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// 求解数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>返回值代表是否有解</returns>
    public async Task<bool> SolveAsync(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            Task<IDlxAsync<(int, int, int), SolveType>>
        > createDlx
    )
    {
        var nodes = SolveGetNodes();
        var dlx = await createDlx(nodes);
        await using var solver = dlx.DanceAsync().GetAsyncEnumerator();
        if (!await solver.MoveNextAsync())
        {
            return false;
        }
        var values = solver.Current;
        SolveSetValues(values);
        return true;
    }

    /// <summary>
    /// 判断在(i, j)位置添加value是否合法，不改变原数独，注意不是判断整个数独是否有解
    /// </summary>
    /// <param name="i">i</param>
    /// <param name="j">j</param>
    /// <param name="value">value</param>
    /// <returns>是否合法</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    public bool IsValidAdd(int i, int j, int value)
    {
        CheckOverflow(i, j);
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        if (
            _board[i, j] != 0
            || _rowNumberCount[i, value] > 0
            || _columnNumberCount[j, value] > 0
            || _blockNumberCount[GetBlockIdxInternal(i, j), value] > 0
        )
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取(i, j)位置的值
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public int GetValue(int i, int j)
    {
        CheckOverflow(i, j);
        return _board[i, j];
    }

    /// <summary>
    /// 在(i, j)位置添加value，不判断是否合法
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    public void SetValue(int i, int j, int value)
    {
        CheckOverflow(i, j);
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        if (_board[i, j] != 0)
        {
            RemoveValueInternal(i, j);
        }
        SetValueInternal(i, j, value);
    }

    private void SetValueInternal(int i, int j, int value)
    {
        _board[i, j] = value;
        _rowNumberCount[i, value]++;
        _columnNumberCount[j, value]++;
        _blockNumberCount[GetBlockIdxInternal(i, j), value]++;
        _hashCode += value * (i * Size + j);
    }

    /// <summary>
    /// 在(i, j)位置移除value
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    public void RemoveValue(int i, int j)
    {
        CheckOverflow(i, j);
        int value = _board[i, j];
        if (value == 0)
            return;
        RemoveValueInternal(i, j);
        _hashCode -= value * (i * Size + j);
    }

    private void RemoveValueInternal(int i, int j)
    {
        int value = _board[i, j];
        _board[i, j] = 0;
        _rowNumberCount[i, value]--;
        _columnNumberCount[j, value]--;
        _blockNumberCount[GetBlockIdx(i, j), value]--;
    }

    /// <summary>
    /// 判断数独是否有效，即满足每行，每列和每个九宫格中都没有重复的数字，注意不是判断数独是否有解
    /// </summary>
    /// <returns>数独是否有效</returns>
    public bool IsValid()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                for (int v = 1; v <= Size; v++)
                {
                    if (_rowNumberCount[i, v] > 1)
                        return false;
                    if (_columnNumberCount[j, v] > 1)
                        return false;
                    if (_blockNumberCount[GetBlockIdx(i, j), v] > 1)
                        return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 返回第i行中value的数量
    /// </summary>
    /// <param name="i">行</param>
    /// <param name="value">数字</param>
    /// <returns>数量</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    public int RowNumberCount(int i, int value)
    {
        if (i < 0 || i >= Size)
            throw new ArgumentOutOfRangeException(nameof(i));
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        return _rowNumberCount[i, value];
    }

    /// <summary>
    /// 返回第j列中value的数量
    /// </summary>
    /// <param name="j">列</param>
    /// <param name="value">数字</param>
    /// <returns>数量</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    public int ColumnNumberCount(int j, int value)
    {
        if (j < 0 || j >= Size)
            throw new ArgumentOutOfRangeException(nameof(j));
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        return _columnNumberCount[j, value];
    }

    /// <summary>
    /// 返回第k个宫格中value的数量
    /// </summary>
    /// <param name="k">宫格</param>
    /// <param name="value">数字</param>
    /// <returns>数量</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    public int BlockNumberCount(int k, int value)
    {
        if (k < 0 || k >= Size)
            throw new ArgumentOutOfRangeException(nameof(k));
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        return _blockNumberCount[k, value];
    }

    /// <summary>
    /// 返回(i, j)所在的宫格的索引，从0开始
    /// </summary>
    /// <param name="i">行</param>
    /// <param name="j">列</param>
    /// <returns></returns>
    public int GetBlockIdx(int i, int j)
    {
        CheckOverflow(i, j);
        return GetBlockIdxInternal(i, j);
    }

    private int GetBlockIdxInternal(int i, int j) => i / MinSize * MinSize + j / MinSize;

    public void CheckOverflow(int i, int j)
    {
        if (i < 0 || i >= Size)
            throw new ArgumentOutOfRangeException(nameof(i));
        if (j < 0 || j >= Size)
            throw new ArgumentOutOfRangeException(nameof(j));
    }

    /// <summary>
    /// 求解时获取所有舞蹈链节点
    /// </summary>
    /// <returns></returns>
    private List<((int i, int j, int k) idx, SolveNode node)> SolveGetNodes()
    {
        List<((int, int, int) idx, SolveNode node)> nodes = [];
        HashSet<SolveNode> hasValues = [];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                int k = _board[i, j];
                if (k != 0)
                {
                    int blockIdx = i / MinSize * MinSize + j / MinSize;
                    hasValues.Add(new SolveNode(SolveType.RowHasNumber, i, k));
                    hasValues.Add(new SolveNode(SolveType.ColumnHasNumber, j, k));
                    hasValues.Add(new SolveNode(SolveType.BlockHasNumber, blockIdx, k));
                }
            }
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (_board[i, j] != 0)
                {
                    continue;
                }
                int blockIdx = i / MinSize * MinSize + j / MinSize;
                for (int k = 1; k <= Size; k++)
                {
                    SolveNode[] nowNodes =
                    [
                        new SolveNode(SolveType.RowHasNumber, i, k),
                        new SolveNode(SolveType.ColumnHasNumber, j, k),
                        new SolveNode(SolveType.BlockHasNumber, blockIdx, k),
                    ];
                    if (nowNodes.Any(hasValues.Contains))
                    {
                        continue;
                    }
                    var idx = (i, j, k);
                    nodes.Add((idx, new SolveNode(SolveType.HasValue, i, j)));
                    nodes.AddRange(nowNodes.Select(n => (idx, n)));
                }
            }
        }
        return nodes;
    }

    /// <summary>
    /// 反解时设置数独的值
    /// </summary>
    /// <param name="values"></param>
    private void SolveSetValues(List<(int, int, int)> values)
    {
        foreach ((int i, int j, int k) in values)
        {
            _board[i, j] = k;
        }
    }

    /// <summary>
    /// 克隆数独
    /// </summary>
    /// <returns></returns>
    public SimpleSudoku Clone()
    {
        int[][] arr = new int[Size][];
        for (int i = 0; i < Size; i++)
        {
            var inner = new int[Size];
            for (int j = 0; j < Size; j++)
            {
                inner[j] = _board[i, j];
            }
            arr[i] = inner;
        }
        var sudoku = new SimpleSudoku(arr);
        return sudoku;
    }

    public bool Equals(SimpleSudoku? other)
    {
        if (other is null)
            return false;
        if (Size != other.Size)
            return false;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (_board[i, j] != other._board[i, j])
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 在(i, j)位置添加或移除value，不判断是否合法
    /// 当value为0时，移除value，否则添加value
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="NotSupportedException">添加时该位置已经有值，或移除时该位置没有值</exception>
    public int this[int i, int j]
    {
        get => _board[i, j];
        set
        {
            if (value == 0)
            {
                RemoveValue(i, j);
            }
            else
            {
                SetValue(i, j, value);
            }
        }
    }

    public static bool operator ==(SimpleSudoku? left, SimpleSudoku? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SimpleSudoku? left, SimpleSudoku? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SimpleSudoku);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }
}
