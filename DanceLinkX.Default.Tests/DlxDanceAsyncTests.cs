using DanceLinkX.DlxDefault;

namespace DanceLinkX.Default.Tests;

using Dlx = Dlx<int, int>;

[TestClass]
public class DlxDanceAsyncTests
{
    public void AssertEqual<T>(List<T> list1, List<T> list2)
        where T : IComparable
    {
        Assert.IsTrue(CheckEqual(list1, list2));
    }

    public bool CheckEqual<T>(List<T> list1, List<T> list2)
        where T : IComparable
    {
        var left = list1.OrderBy(x => x).ToList();
        var right = list2.OrderBy(x => x).ToList();
        return left.SequenceEqual(right);
    }

    public void AssertEqual<T>(List<List<T>> list1, List<List<T>> list2)
        where T : IComparable
    {
        var left = list1.Select(x => x.OrderBy(y => y).ToList()).ToList();
        left.Sort(
            (a, b) =>
            {
                foreach (var (x, y) in a.Zip(b))
                {
                    if (!x.Equals(y))
                        return x.CompareTo(y);
                }
                return a.Count.CompareTo(b.Count);
            }
        );
        var right = list2.Select(x => x.OrderBy(y => y).ToList()).ToList();
        right.Sort(
            (a, b) =>
            {
                foreach (var (x, y) in a.Zip(b))
                {
                    if (!x.Equals(y))
                        return x.CompareTo(y);
                }
                return a.Count.CompareTo(b.Count);
            }
        );
        Assert.AreEqual(left.Count, right.Count);
        foreach (var (a, b) in left.Zip(right))
        {
            Assert.IsTrue(a.SequenceEqual(b));
        }
    }

    public void AssertIn<T>(List<List<T>> list1, List<List<T>> list2)
        where T : IComparable
    {
        var idxs = Enumerable.Range(0, list2.Count).ToHashSet();
        foreach (var one in list1)
        {
            foreach (var (two, i) in list2.Zip(Enumerable.Range(0, list2.Count)))
            {
                if (!idxs.Contains(i))
                    continue;
                if (CheckEqual(one, two))
                {
                    idxs.Remove(i);
                    goto next;
                }
            }
            Assert.Fail();
            next:
            continue;
        }
    }

    public async Task<List<List<TRow>>> DanceToListAsync<TRow, TColumn>(Dlx<TRow, TColumn> dlx)
        where TRow : notnull
        where TColumn : notnull
    {
        List<List<TRow>> result = [];
        await foreach (var one in dlx.DanceAsync())
        {
            result.Add(one);
        }
        return result;
    }

    [TestMethod]
    public async Task TestSimple()
    {
        var dlx = new Dlx([(0, 0), (0, 1)]);
        var result = await DanceToListAsync(dlx);
        Assert.AreEqual(result.Count, 1);
        Assert.IsTrue(result[0].SequenceEqual([0]));
    }

    [TestMethod]
    public async Task TestMultipleResult()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)]);
        var result = await DanceToListAsync(dlx);
        AssertEqual(
            result,
            [
                [0, 1, 2],
                [0, 4],
                [1, 5],
                [2, 3],
            ]
        );
    }
}
