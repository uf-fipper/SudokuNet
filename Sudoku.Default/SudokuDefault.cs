using System.Numerics;
using DanceLinkX.Services;

namespace Sudoku.Default;

/// <summary>
/// 数独类，相当于一个边长为size的二维数组，其中size只能是平方数
/// </summary>
public class SudokuDefault
    : ISudoku,
        ISudokuAsync,
        IEquatable<SudokuDefault>,
        IEqualityOperators<SudokuDefault, SudokuDefault, bool>
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

    public ICollection<(int i, int j)> BaseIndexs { get; private set; }

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

    public SudokuDefault(IList<List<int>> board)
        : this(board.Select(x => x.ToArray()).ToArray()) { }

    public SudokuDefault(IList<IList<int>> board)
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
        BaseIndexs = new HashSet<(int, int)>();
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
                    BaseIndexs.Add((i, j));
                }
            }
        }
    }

    private static IDlxAsync<(int, int, int), SolveNode> CreateDefaultDlx(
        IList<((int, int, int) idx, SolveNode node)> nodes
    )
    {
        return new DanceLinkX.DlxDefault.Dlx<(int, int, int), SolveNode>(nodes);
    }

    public SudokuDefault? SolveNew() => SolveNew(CreateDefaultDlx);

    /// <summary>
    /// 求解并返回一个新的数独，不会改变原数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>null则无解</returns>
    public SudokuDefault? SolveNew(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            IDlx<(int, int, int), SolveNode>
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

    public bool Solve() => Solve(CreateDefaultDlx);

    /// <summary>
    /// 求解数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>返回值代表是否有解</returns>
    public bool Solve(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            IDlx<(int, int, int), SolveNode>
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

    public async Task<SudokuDefault?> SolveNewAsync() =>
        await SolveNewAsync(n => Task.FromResult(CreateDefaultDlx(n)));

    /// <summary>
    /// 求解并返回一个新的数独，不会改变原数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>null则无解</returns>
    public async Task<SudokuDefault?> SolveNewAsync(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            Task<IDlxAsync<(int, int, int), SolveNode>>
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

    public async Task<bool> SolveAsync() =>
        await SolveAsync(n => Task.FromResult(CreateDefaultDlx(n)));

    /// <summary>
    /// 求解数独
    /// </summary>
    /// <param name="createDlx">舞蹈链算法的创建函数</param>
    /// <returns>返回值代表是否有解</returns>
    public async Task<bool> SolveAsync(
        Func<
            List<((int, int, int) idx, SolveNode node)>,
            Task<IDlxAsync<(int, int, int), SolveNode>>
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

    public bool IsValidAdd(int i, int j, int value)
    {
        if (CheckOverflow(i, j))
            return false;
        if (value <= 0 || value > Size)
            return false;
        if (IsBaseIndex(i, j))
            return false;
        return true;
    }

    /// <summary>
    /// 判断在(i, j)位置添加value是否有效，不改变原数独，注意不是判断整个数独是否有解
    /// </summary>
    /// <param name="i">i</param>
    /// <param name="j">j</param>
    /// <param name="value">value</param>
    /// <returns>是否有效</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    public bool IsValidAfterAdd(int i, int j, int value)
    {
        CheckOverflowAndThrow(i, j);
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        if (IsBaseIndex(i, j))
            throw new ChangeBaseIndexException(i, j);
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
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    public int GetValue(int i, int j)
    {
        CheckOverflowAndThrow(i, j);
        return _board[i, j];
    }

    /// <summary>
    /// 在(i, j)位置添加value，不判断是否有效
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    public void SetValue(int i, int j, int value)
    {
        CheckOverflowAndThrow(i, j);
        if (value <= 0 || value > Size)
            throw new ArgumentOutOfRangeException(nameof(value));
        if (IsBaseIndex(i, j))
            throw new ChangeBaseIndexException(i, j);
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
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    public void RemoveValue(int i, int j)
    {
        CheckOverflowAndThrow(i, j);
        if (IsBaseIndex(i, j))
            throw new ChangeBaseIndexException(i, j);
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
    /// 在(i, j)位置添加或移除value，不判断是否有效
    /// 当value为0时，移除value，否则添加value
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    public int this[int i, int j]
    {
        get => GetValue(i, j);
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
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    public int GetBlockIdx(int i, int j)
    {
        CheckOverflowAndThrow(i, j);
        return GetBlockIdxInternal(i, j);
    }

    private int GetBlockIdxInternal(int i, int j) => i / MinSize * MinSize + j / MinSize;

    public bool CheckOverflow(int i, int j)
    {
        if (i < 0 || i >= Size)
            return true;
        if (j < 0 || j >= Size)
            return true;
        return false;
    }

    public void CheckOverflowAndThrow(int i, int j)
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
    public SudokuDefault Clone()
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
        var sudoku = new SudokuDefault(arr);
        return sudoku;
    }

    /// <summary>
    /// 求数独的所有解
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SudokuDefault> SolveAll()
    {
        return SolveAllAsync().ToBlockingEnumerable();
    }

    /// <summary>
    /// 异步求数独的所有解
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<SudokuDefault> SolveAllAsync()
    {
        var nodes = SolveGetNodes();
        var dlx = CreateDefaultDlx(nodes);
        await foreach (var one in dlx.DanceAsync())
        {
            var sudoku = Clone();
            sudoku.SolveSetValues(one);
            yield return sudoku;
        }
    }

    /// <summary>
    /// 创建一个唯一解的随机数独
    /// </summary>
    /// <param name="size"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static SudokuDefault NewSudoku(int size, Random? random)
    {
        random ??= new Random();
        // 先创建一个完全体数独
        int[][] sudokuValues = new int[size][];
        for (int i = 0; i < size; i++)
        {
            sudokuValues[i] = new int[size];
        }
        var sudoku = new SudokuDefault(sudokuValues);
        var nodes = sudoku.SolveGetNodes().ToArray();
        random.Shuffle(nodes);
        var dlx = CreateDefaultDlx(nodes);
        using var solver = dlx.Dance().GetEnumerator();
        solver.MoveNext();
        var values = solver.Current;
        sudoku.SolveSetValues(values);

        // 现在开始挖洞
        var idxList = Enumerable
            .Range(0, size)
            .SelectMany(i => Enumerable.Range(0, size).Select(j => (i, j)))
            .ToArray();
        random.Shuffle(idxList);
        foreach (var (i, j) in idxList)
        {
            int value = sudoku[i, j];
            sudoku.RemoveValueInternal(i, j);
            using var solver1 = sudoku.SolveAll().GetEnumerator();
            solver1.MoveNext();
            if (solver1.MoveNext())
            {
                sudoku.SetValueInternal(i, j, value);
            }
        }
        // 重新赋值 BaseIndexs
        sudoku.ResetBaseIndexs();
        return sudoku;
    }

    /// <summary>
    /// 异步创建一个唯一解的随机数独
    /// </summary>
    /// <param name="size"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static async Task<SudokuDefault> NewSudokuAsync(int size, Random? random)
    {
        random ??= new Random();
        // 先创建一个完全体数独
        int[][] sudokuValues = new int[size][];
        for (int i = 0; i < size; i++)
        {
            sudokuValues[i] = new int[size];
        }
        var sudoku = new SudokuDefault(sudokuValues);
        var nodes = sudoku.SolveGetNodes().ToArray();
        random.Shuffle(nodes);
        var dlx = CreateDefaultDlx(nodes);
        await using var solver = dlx.DanceAsync().GetAsyncEnumerator();
        await solver.MoveNextAsync();
        var values = solver.Current;
        sudoku.SolveSetValues(values);

        // 现在开始挖洞
        var idxList = Enumerable
            .Range(0, size)
            .SelectMany(i => Enumerable.Range(0, size).Select(j => (i, j)))
            .ToArray();
        random.Shuffle(idxList);
        foreach (var (i, j) in idxList)
        {
            int value = sudoku[i, j];
            sudoku.RemoveValueInternal(i, j);
            await using var solver1 = sudoku.SolveAllAsync().GetAsyncEnumerator();
            await solver1.MoveNextAsync();
            if (await solver1.MoveNextAsync())
            {
                sudoku.SetValueInternal(i, j, value);
            }
        }
        // 重新赋值 BaseIndexs
        sudoku.ResetBaseIndexs();
        return sudoku;
    }

    private void ResetBaseIndexs()
    {
        int size = Size;
        BaseIndexs = [];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (this[i, j] != 0)
                {
                    BaseIndexs.Add((i, j));
                }
            }
        }
    }

    public bool Equals(SudokuDefault? other)
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

    public static bool operator ==(SudokuDefault? left, SudokuDefault? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SudokuDefault? left, SudokuDefault? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SudokuDefault);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    ISudoku? ISudoku.SolveNew()
    {
        return SolveNew();
    }

    ISudoku ISudoku.Clone()
    {
        return Clone();
    }

    bool IEquatable<ISudoku>.Equals(ISudoku? other)
    {
        return Equals(other);
    }

    Task<bool> ISudokuAsync.SolveAsync()
    {
        return SolveAsync();
    }

    async Task<ISudoku?> ISudokuAsync.SolveNewAsync()
    {
        return await SolveNewAsync();
    }

    public bool IsBaseIndex(int i, int j)
    {
        return BaseIndexs.Contains((i, j));
    }
}
