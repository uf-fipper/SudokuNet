namespace Sudoku;

public interface ISudoku : IEquatable<ISudoku>
{
    /// <summary>
    /// 一个宫格的边长
    /// </summary>
    int MinSize { get; }

    /// <summary>
    /// 整个迷宫的边长
    /// </summary>
    int Size { get; }

    /// <summary>
    /// 数独的原始值列表（无法重写的值）
    /// </summary>
    ICollection<(int i, int j)> BaseIndexs { get; }

    /// <summary>
    /// 求解，返回false时不改变原数独
    /// </summary>
    /// <returns>如果有解，返回true，否则返回false</returns>
    bool Solve();

    /// <summary>
    /// 求解，返回一个新的数独，不改变原数独
    /// </summary>
    /// <returns>如果有解，返回新的数独，否则返回null</returns>
    ISudoku? SolveNew();

    /// <summary>
    /// 判断在(i, j)位置能否添加value，不判断是否有效
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool IsValidAdd(int i, int j, int value);

    /// <summary>
    /// 判断在(i, j)位置添加value是否有效，不改变原数独，注意不是判断整个数独是否有解
    /// </summary>
    /// <param name="i">i</param>
    /// <param name="j">j</param>
    /// <param name="value">value</param>
    /// <returns>是否有效</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    bool IsValidAfterAdd(int i, int j, int value);

    /// <summary>
    /// 获取(i, j)位置的值
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    int GetValue(int i, int j);

    /// <summary>
    /// 在(i, j)位置添加value，不判断是否有效
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    void SetValue(int i, int j, int value);

    /// <summary>
    /// 在(i, j)位置移除value
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    void RemoveValue(int i, int j);

    /// <summary>
    /// 判断数独是否有效，即满足每行，每列和每个九宫格中都没有重复的数字，注意不是判断数独是否有解
    /// </summary>
    /// <returns>数独是否有效</returns>
    bool IsValid();

    /// <summary>
    /// 返回第i行中value的数量
    /// </summary>
    /// <param name="i">行</param>
    /// <param name="value">数字</param>
    /// <returns>数量</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    int RowNumberCount(int i, int value);

    /// <summary>
    /// 返回第j列中value的数量
    /// </summary>
    /// <param name="j">列</param>
    /// <param name="value">数字</param>
    /// <returns>数量</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    int ColumnNumberCount(int j, int value);

    /// <summary>
    /// 返回第k个宫格中value的数量
    /// </summary>
    /// <param name="k">宫格</param>
    /// <param name="value">数字</param>
    /// <returns>数量</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    int BlockNumberCount(int k, int value);

    /// <summary>
    /// 返回(i, j)所在的宫格的索引，从0开始
    /// </summary>
    /// <param name="i">行</param>
    /// <param name="j">列</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超过范围</exception>
    int GetBlockIdx(int i, int j);

    /// <summary>
    /// 克隆数独
    /// </summary>
    /// <returns></returns>
    ISudoku Clone();

    /// <summary>
    /// 在(i, j)位置添加或移除value，不判断是否有效
    /// 当value为0时，移除value，否则添加value
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    /// <exception cref="ChangeBaseIndexException">是数独原来的索引，不可修改</exception>
    int this[int i, int j] { get; set; }

    /// <summary>
    /// 判断(i, j)是否是数独的原始值，即是否是数独的初始值
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    bool IsBaseIndex(int i, int j);

    /// <summary>
    /// 数独是否已完成
    /// </summary>
    /// <returns></returns>
    bool IsWin();

    /// <summary>
    /// 获取二维数组棋盘
    /// </summary>
    /// <returns></returns>
    int[][] GetBoard();
}

public interface ISudokuAsync : ISudoku
{
    /// <summary>
    /// 求解，返回false时不改变原数独
    /// </summary>
    /// <returns>如果有解，返回true，否则返回false</returns>
    Task<bool> SolveAsync();

    /// <summary>
    /// 求解，返回一个新的数独，不改变原数独
    /// </summary>
    /// <returns>如果有解，返回新的数独，否则返回null</returns>
    Task<ISudoku?> SolveNewAsync();
}
