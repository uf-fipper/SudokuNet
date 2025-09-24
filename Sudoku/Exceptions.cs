namespace Sudoku;

public class SudokuException : Exception { }

/// <summary>
/// 是数独原来的索引，不可修改
/// </summary>
/// <param name="i"></param>
/// <param name="j"></param>
public class ChangeBaseIndexException(int i, int j) : SudokuException
{
    public (int i, int j) Index { get; } = (i, j);
}
