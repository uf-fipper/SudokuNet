using DanceLinkX.DlxDefault;

namespace DanceLinkX.Default.Tests;

using Dlx = Dlx<int, int>;
using Node = DlxNode<int, int>;

[TestClass]
public class DlxDanceTests
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

    [TestMethod]
    public void TestEmptyInput()
    {
        var dlx = new Dlx();
        var result = dlx.Dance().ToList();
        Assert.AreEqual(0, result.Count);

        dlx = new Dlx([]);
        result = dlx.Dance().ToList();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void TestSimple()
    {
        var dlx = new Dlx([(0, 0), (0, 1)]);
        var result = dlx.Dance().ToList();
        Assert.AreEqual(result.Count, 1);
        Assert.IsTrue(result[0].SequenceEqual([0]));
    }

    [TestMethod]
    public void TestSimple2()
    {
        var dlx = new Dlx([(0, 0), (0, 1), (1, 0), (1, 2), (2, 2)]);
        var result = dlx.Dance().ToList();
        Assert.AreEqual(result.Count, 1);
        AssertEqual(result[0], [0, 2]);
    }

    [TestMethod]
    public void TestMultipleResult()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)]);
        var result = dlx.Dance().ToList();
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

    [TestMethod]
    public void DanceUseHeap()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)])
        {
            UseHeap = true,
        };
        var result = dlx.Dance().ToList();
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

    [TestMethod]
    public void DanceInDance()
    {
        var dlx = new Dlx<int, int>([(0, 0), (0, 1)]);

        dlx.Dance().GetEnumerator().MoveNext();
        Assert.ThrowsException<InvalidOperationException>(() =>
            dlx.Dance().GetEnumerator().MoveNext()
        );
        Assert.ThrowsException<InvalidOperationException>(() => dlx.AddNode(2, 2));
        Assert.ThrowsException<InvalidOperationException>(() => dlx.AddNode((2, 2)));
        Assert.ThrowsException<InvalidOperationException>(() => dlx.AddNodes([(2, 2), (3, 3)]));
    }

    [TestMethod]
    public void DanceTwice()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)]);
        List<List<int>> result;
        List<List<int>> rightResult =
        [
            [0, 1, 2],
            [0, 4],
            [1, 5],
            [2, 3],
        ];
        //
        testRaw();
        //
        testRaw();
        //
        foreach (var one in dlx.Dance())
        {
            AssertEqual(one, [0, 1, 2]);
            break;
        }
        //
        testRaw();
        //
        result = dlx.Dance().Take(2).ToList();
        Assert.AreEqual(result.Count, 2);
        AssertIn(result, rightResult);
        //
        testRaw();
        //
        result = dlx.Dance().Skip(1).Take(2).ToList();
        Assert.AreEqual(result.Count, 2);
        AssertIn(result, rightResult);
        //
        testRaw();
        //
        result = dlx.Dance().Take(2).Skip(1).ToList();
        Assert.AreEqual(result.Count, 1);
        AssertIn(result, rightResult);
        //
        using (var enumerator = dlx.Dance().GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var x = enumerator.Current;
                AssertIn([x], rightResult);
            }
        }
        //
        testRaw();

        void testRaw()
        {
            var result = dlx.Dance().ToList();
            AssertEqual(result, rightResult);
        }
    }

    [TestMethod]
    public void DanceNoSolve()
    {
        var dlx = new Dlx([(0, 0), (0, 1), (1, 0), (1, 2), (2, 1), (2, 2)]);
        var result = dlx.Dance().ToList();
        Assert.AreEqual(result.Count, 0);
    }

    [TestMethod]
    public void TestIsDancing()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)]);
        Assert.IsFalse(dlx.IsDancing);
        foreach (var _ in dlx.Dance())
        {
            Assert.IsTrue(dlx.IsDancing);
        }
        Assert.IsFalse(dlx.IsDancing);

        Assert.IsFalse(dlx.IsDancing);
        foreach (var _ in dlx.Dance())
        {
            Assert.IsTrue(dlx.IsDancing);
            break;
        }
        Assert.IsFalse(dlx.IsDancing);
    }

    [TestMethod]
    public void TestIsDancing2()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)]);
        Assert.IsFalse(dlx.IsDancing);
        using (var enumerator = dlx.Dance().GetEnumerator())
        {
            Assert.IsFalse(dlx.IsDancing);
            enumerator.MoveNext();
            Assert.IsTrue(dlx.IsDancing);
        }
        Assert.IsFalse(dlx.IsDancing);
    }

    [TestMethod]
    public void TestDanceWithString()
    {
        List<(int, int)> nodes =
        [
            (0, 0),
            (1, 1),
            (2, 2),
            (3, 0),
            (3, 1),
            (4, 1),
            (4, 2),
            (5, 0),
            (5, 2),
        ];
        List<List<int>> rightResult =
        [
            [0, 1, 2],
            [0, 4],
            [1, 5],
            [2, 3],
        ];
        var dlx = new Dlx<string, string>(
            nodes.Select(x => (x.Item1.ToString(), x.Item2.ToString()))
        );
        AssertEqual(
            dlx.Dance().ToList(),
            rightResult.Select(x => x.Select(y => y.ToString()).ToList()).ToList()
        );
    }

    [TestMethod]
    public void TestDanceWithObject()
    {
        object r1 = new();
        object r2 = new();
        object r3 = new();
        object r4 = new();
        object r5 = new();
        object r6 = new();
        object c1 = new();
        object c2 = new();
        object c3 = new();
        var dlx = new Dlx<object, object>(
            [
                (r1, c1),
                (r2, c2),
                (r3, c3),
                (r4, c1),
                (r4, c2),
                (r5, c1),
                (r5, c3),
                (r6, c2),
                (r6, c3),
            ]
        );
        var result = dlx.Dance().ToList();
        Assert.IsTrue(result[0].SequenceEqual([r1, r2, r3]));
        Assert.IsTrue(result[1].SequenceEqual([r1, r6]));
        Assert.IsTrue(result[2].SequenceEqual([r4, r3]));
        Assert.IsTrue(result[3].SequenceEqual([r5, r2]));
    }

    [TestMethod]
    public void TestDanceAndAddNodes()
    {
        var dlx = new Dlx([(0, 0), (1, 1), (2, 2), (3, 0), (3, 1), (4, 1), (4, 2), (5, 0), (5, 2)]);
        AssertEqual(
            dlx.Dance().ToList(),
            [
                [0, 1, 2],
                [0, 4],
                [1, 5],
                [2, 3],
            ]
        );
        dlx.AddNode(4, 0);
        AssertEqual(
            dlx.Dance().ToList(),
            [
                [0, 1, 2],
                [4],
                [1, 5],
                [2, 3],
            ]
        );
        dlx.AddNodes([(6, 1), (6, 2)]);
        AssertEqual(
            dlx.Dance().ToList(),
            [
                [0, 1, 2],
                [0, 6],
                [4],
                [1, 5],
                [2, 3],
            ]
        );
        dlx.AddNode(7, 3);
        AssertEqual(
            dlx.Dance().ToList(),
            [
                [0, 1, 2, 7],
                [0, 6, 7],
                [4, 7],
                [1, 5, 7],
                [2, 3, 7],
            ]
        );
    }
}
