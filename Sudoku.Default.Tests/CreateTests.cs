using DanceLinkX.DlxDefault;
using Sudoku.Default;

namespace Sudoku.Default.Tests;

[TestClass]
public class CreateTests
{
    public SudokuDefault CreateDefaultSudoku()
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
        var sudoku = new SudokuDefault(raw);
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
        Assert.IsFalse(sudoku.IsValidAdd(0, 0, 1));
        Assert.IsFalse(sudoku.IsValidAdd(0, 0, 5));
        Assert.IsTrue(sudoku.IsValidAdd(0, 2, 3));
        Assert.IsTrue(sudoku.IsValidAdd(0, 2, 5));
        Assert.IsFalse(sudoku.IsValidAfterAdd(0, 2, 3));
        Assert.IsFalse(sudoku.IsValidAfterAdd(0, 2, 8));
        Assert.IsFalse(sudoku.IsValidAfterAdd(0, 2, 6));
        Assert.IsTrue(sudoku.IsValidAfterAdd(0, 2, 4));
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

    [TestMethod]
    public void CloneTest()
    {
        var raw = CreateDefaultSudoku();
        var sudoku = raw.Clone();
        Assert.AreEqual(sudoku, raw);
        sudoku.SetValue(0, 2, 4);
        Assert.AreNotEqual(sudoku, raw);
        Assert.AreEqual(raw.RowNumberCount(0, 4), 0);
        Assert.AreEqual(raw.ColumnNumberCount(2, 4), 0);
        Assert.AreEqual(raw.BlockNumberCount(0, 4), 0);
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 1);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 1);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 1);
        sudoku.RemoveValue(0, 2);
        Assert.AreEqual(sudoku, raw);
        Assert.AreEqual(sudoku.RowNumberCount(0, 4), 0);
        Assert.AreEqual(sudoku.ColumnNumberCount(2, 4), 0);
        Assert.AreEqual(sudoku.BlockNumberCount(0, 4), 0);
    }

    [TestMethod]
    public void SolveTest()
    {
        int[][] expected =
        [
            [5, 3, 4, 6, 7, 8, 9, 1, 2],
            [6, 7, 2, 1, 9, 5, 3, 4, 8],
            [1, 9, 8, 3, 4, 2, 5, 6, 7],
            [8, 5, 9, 7, 6, 1, 4, 2, 3],
            [4, 2, 6, 8, 5, 3, 7, 9, 1],
            [7, 1, 3, 9, 2, 4, 8, 5, 6],
            [9, 6, 1, 5, 3, 7, 2, 8, 4],
            [2, 8, 7, 4, 1, 9, 6, 3, 5],
            [3, 4, 5, 2, 8, 6, 1, 7, 9],
        ];
        var sudoku = CreateDefaultSudoku();
        sudoku.Solve();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Assert.AreEqual(sudoku[i, j], expected[i][j]);
            }
        }
    }

    [TestMethod]
    public void SolveNewTest()
    {
        int[][] expected =
        [
            [5, 3, 4, 6, 7, 8, 9, 1, 2],
            [6, 7, 2, 1, 9, 5, 3, 4, 8],
            [1, 9, 8, 3, 4, 2, 5, 6, 7],
            [8, 5, 9, 7, 6, 1, 4, 2, 3],
            [4, 2, 6, 8, 5, 3, 7, 9, 1],
            [7, 1, 3, 9, 2, 4, 8, 5, 6],
            [9, 6, 1, 5, 3, 7, 2, 8, 4],
            [2, 8, 7, 4, 1, 9, 6, 3, 5],
            [3, 4, 5, 2, 8, 6, 1, 7, 9],
        ];
        var raw = CreateDefaultSudoku();
        var sudoku = raw.SolveNew();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Assert.AreEqual(sudoku?[i, j], expected[i][j]);
            }
        }
        Assert.AreEqual(raw[0, 2], 0);
    }

    [TestMethod]
    public async Task SolveAsyncTest()
    {
        int[][] expected =
        [
            [5, 3, 4, 6, 7, 8, 9, 1, 2],
            [6, 7, 2, 1, 9, 5, 3, 4, 8],
            [1, 9, 8, 3, 4, 2, 5, 6, 7],
            [8, 5, 9, 7, 6, 1, 4, 2, 3],
            [4, 2, 6, 8, 5, 3, 7, 9, 1],
            [7, 1, 3, 9, 2, 4, 8, 5, 6],
            [9, 6, 1, 5, 3, 7, 2, 8, 4],
            [2, 8, 7, 4, 1, 9, 6, 3, 5],
            [3, 4, 5, 2, 8, 6, 1, 7, 9],
        ];
        var sudoku = CreateDefaultSudoku();
        await sudoku.SolveAsync();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Assert.AreEqual(sudoku[i, j], expected[i][j]);
            }
        }
    }

    [TestMethod]
    public async Task SolveNewAsyncTest()
    {
        int[][] expected =
        [
            [5, 3, 4, 6, 7, 8, 9, 1, 2],
            [6, 7, 2, 1, 9, 5, 3, 4, 8],
            [1, 9, 8, 3, 4, 2, 5, 6, 7],
            [8, 5, 9, 7, 6, 1, 4, 2, 3],
            [4, 2, 6, 8, 5, 3, 7, 9, 1],
            [7, 1, 3, 9, 2, 4, 8, 5, 6],
            [9, 6, 1, 5, 3, 7, 2, 8, 4],
            [2, 8, 7, 4, 1, 9, 6, 3, 5],
            [3, 4, 5, 2, 8, 6, 1, 7, 9],
        ];
        var raw = CreateDefaultSudoku();
        var sudoku = await raw.SolveNewAsync();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Assert.AreEqual(sudoku?[i, j], expected[i][j]);
            }
        }
        Assert.AreEqual(raw[0, 2], 0);
    }

    [TestMethod]
    public void RandomNewTest()
    {
        var random = new Random(1);
        var sudoku = SudokuDefault.NewSudoku(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        random = new Random(2);
        sudoku = SudokuDefault.NewSudoku(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        random = new Random(3);
        sudoku = SudokuDefault.NewSudoku(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        random = new Random(12345678);
        sudoku = SudokuDefault.NewSudoku(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        var random1 = new Random(12345678);
        var sudoku1 = SudokuDefault.NewSudoku(9, random1);
        Assert.AreEqual(sudoku, sudoku1);
    }

    [TestMethod]
    public async Task RandomNewAsyncTest()
    {
        var random = new Random(1);
        var sudoku = await SudokuDefault.NewSudokuAsync(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        random = new Random(2);
        sudoku = await SudokuDefault.NewSudokuAsync(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        random = new Random(3);
        sudoku = await SudokuDefault.NewSudokuAsync(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        random = new Random(12345678);
        sudoku = await SudokuDefault.NewSudokuAsync(9, random);
        Assert.AreEqual(sudoku.SolveAll().Count(), 1);

        var random1 = new Random(12345678);
        var sudoku1 = SudokuDefault.NewSudoku(9, random1);
        Assert.AreEqual(sudoku, sudoku1);
    }

    [TestMethod]
    public void BaseIndexTest()
    {
        var sudoku = CreateDefaultSudoku();
        var left = sudoku.BaseIndexs.OrderBy(x => x.i).ThenBy(x => x.j).ToArray();
        (int, int)[] right =
        [
            (0, 0),
            (0, 1),
            (0, 4),
            (1, 0),
            (1, 3),
            (1, 4),
            (1, 5),
            (2, 1),
            (2, 2),
            (2, 7),
            (3, 0),
            (3, 4),
            (3, 8),
            (4, 0),
            (4, 3),
            (4, 5),
            (4, 8),
            (5, 0),
            (5, 4),
            (5, 8),
            (6, 1),
            (6, 6),
            (6, 7),
            (7, 3),
            (7, 4),
            (7, 5),
            (7, 8),
            (8, 4),
            (8, 7),
            (8, 8),
        ];
        Assert.IsTrue(left.SequenceEqual(right));

        Assert.ThrowsException<ChangeBaseIndexException>(() => sudoku.SetValue(0, 0, 1));
        Assert.ThrowsException<ChangeBaseIndexException>(() => sudoku.RemoveValue(0, 0));
        Assert.ThrowsException<ChangeBaseIndexException>(() => sudoku[0, 0] = 1);
        Assert.ThrowsException<ChangeBaseIndexException>(() => sudoku[0, 0] = 0);
        Assert.ThrowsException<ChangeBaseIndexException>(() => sudoku.IsValidAfterAdd(0, 0, 1));
        Assert.IsFalse(sudoku.IsValidAdd(0, 0, 1));
        Assert.IsTrue(left.SequenceEqual(right));
        Assert.IsTrue(sudoku[0, 0] == 5);

        sudoku[1, 1] = 3;
        Assert.IsTrue(left.SequenceEqual(right));
    }
}
