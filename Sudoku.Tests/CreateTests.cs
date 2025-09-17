using Sudoku.Models;

namespace Sudoku.Tests;

[TestClass]
public class CreateTests
{
    public SimpleSudoku CreateDefaultSudoku()
    {
        int[][] raw =
        [
            [5, 3, 0, 0, 7, 0, 0, 0, 0],
            [6, 0, 0, 1, 9, 5, 0, 0, 0],
            [0, 9, 8, 0, 0, 0, 0, 6, 0],
            [8, 0, 0, 0, 6, 0, 0, 0, 3],
            [4, 0, 0, 8, 0, 3, 0, 0, 1],
            [7, 0, 0, 0, 2, 0, 0, 0, 6],
            [0, 6, 0, 0, 0, 0, 2, 8, 0],
            [0, 0, 0, 4, 1, 9, 0, 0, 5],
            [0, 0, 0, 0, 8, 0, 0, 7, 9],
        ];
        var sudoku = new SimpleSudoku(raw);
        return sudoku;
    }

    [TestMethod]
    public void CreateSudokuTest()
    {
        var sudoku = CreateDefaultSudoku();
        Assert.AreEqual(9, sudoku.Size);
        Assert.AreEqual(3, sudoku.MinSize);
        int[][] raw =
        [
            [5, 3, 0, 0, 7, 0, 0, 0, 0],
            [6, 0, 0, 1, 9, 5, 0, 0, 0],
            [0, 9, 8, 0, 0, 0, 0, 6, 0],
            [8, 0, 0, 0, 6, 0, 0, 0, 3],
            [4, 0, 0, 8, 0, 3, 0, 0, 1],
            [7, 0, 0, 0, 2, 0, 0, 0, 6],
            [0, 6, 0, 0, 0, 0, 2, 8, 0],
            [0, 0, 0, 4, 1, 9, 0, 0, 5],
            [0, 0, 0, 0, 8, 0, 0, 7, 9],
        ];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Assert.AreEqual(raw[i][j], sudoku[i, j]);
            }
        }
    }

    [TestMethod]
    public void IsValidTest()
    {
        var sudoku = CreateDefaultSudoku();
        var raw = CreateDefaultSudoku();
        Assert.IsTrue(sudoku.IsValid());
        Assert.IsFalse(sudoku.IsValidAdd(0, 0, 5));
        Assert.IsFalse(sudoku.IsValidAdd(0, 2, 3));
        Assert.IsFalse(sudoku.IsValidAdd(0, 2, 8));
        Assert.IsFalse(sudoku.IsValidAdd(0, 2, 6));
        Assert.IsTrue(sudoku.IsValidAdd(0, 2, 4));
        Assert.AreEqual(sudoku, raw);
    }

    [TestMethod]
    public void SetValueTest()
    {
        var sudoku = CreateDefaultSudoku();
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 0);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 0);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 0);

        sudoku.SetValue(0, 2, 4);
        Assert.AreEqual(4, sudoku.GetValue(0, 2));
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 1);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 1);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 1);
        sudoku.RemoveValue(0, 2);
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 0);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 0);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 0);

        sudoku[0, 2] = 4;
        Assert.AreEqual(4, sudoku[0, 2]);
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 1);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 1);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 1);
        sudoku[0, 2] = 0;
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 0);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 0);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 0);
    }
}
