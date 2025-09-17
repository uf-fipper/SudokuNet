using DanceLinkX.DlxDefault;

namespace DanceLinkX.Default.Tests;

using Dlx = Dlx<int, int>;
using Node = DlxNode<int, int>;

[TestClass]
public class DlxStructTests
{
    public void AssertNode(Node node, Node? left, Node? right, Node? up, Node? down)
    {
        Assert.AreEqual(node.Left, left);
        Assert.AreEqual(node.Right, right);
        Assert.AreEqual(node.Up, up);
        Assert.AreEqual(node.Down, down);
    }

    public void AssertDlx(Dlx<int, int> dlx)
    {
        var rowRoot = dlx.RowHeadRoot;
        var columnRoot = dlx.ColumnHeadRoot;
        var row0 = rowRoot.Next;
        var row1 = row0.Next;
        var row2 = row1.Next;
        var row3 = row2.Next;
        var row4 = row3.Next;
        var row5 = row4.Next;
        var column0 = columnRoot.Next;
        var column3 = column0.Next;
        var column6 = column3.Next;
        var column4 = column6.Next;
        var column2 = column4.Next;
        var column5 = column2.Next;
        var column1 = column5.Next;
        var node00 = dlx.GetNode(0, 0);
        var node03 = dlx.GetNode(0, 3);
        var node06 = dlx.GetNode(0, 6);
        var node10 = dlx.GetNode(1, 0);
        var node13 = dlx.GetNode(1, 3);
        var node23 = dlx.GetNode(2, 3);
        var node24 = dlx.GetNode(2, 4);
        var node32 = dlx.GetNode(3, 2);
        var node34 = dlx.GetNode(3, 4);
        var node35 = dlx.GetNode(3, 5);
        var node41 = dlx.GetNode(4, 1);
        var node42 = dlx.GetNode(4, 2);
        var node45 = dlx.GetNode(4, 5);
        var node46 = dlx.GetNode(4, 6);
        var node51 = dlx.GetNode(5, 1);
        var node56 = dlx.GetNode(5, 6);
        // row node
        Assert.AreEqual(row0.FirstNode, node00);
        Assert.AreEqual(row0.LastNode, node06);
        Assert.AreEqual(row1.FirstNode, node10);
        Assert.AreEqual(row1.LastNode, node13);
        Assert.AreEqual(row2.FirstNode, node23);
        Assert.AreEqual(row2.LastNode, node24);
        Assert.AreEqual(row3.FirstNode, node32);
        Assert.AreEqual(row3.LastNode, node35);
        Assert.AreEqual(row4.FirstNode, node41);
        Assert.AreEqual(row4.LastNode, node46);
        Assert.AreEqual(row5.FirstNode, node51);
        Assert.AreEqual(row5.LastNode, node56);
        // column node
        Assert.AreEqual(column0.FirstNode, node00);
        Assert.AreEqual(column0.LastNode, node10);
        Assert.AreEqual(column3.FirstNode, node03);
        Assert.AreEqual(column3.LastNode, node23);
        Assert.AreEqual(column6.FirstNode, node06);
        Assert.AreEqual(column6.LastNode, node56);
        Assert.AreEqual(column4.FirstNode, node24);
        Assert.AreEqual(column4.LastNode, node34);
        Assert.AreEqual(column2.FirstNode, node32);
        Assert.AreEqual(column2.LastNode, node42);
        Assert.AreEqual(column1.FirstNode, node41);
        Assert.AreEqual(column1.LastNode, node51);
        Assert.AreEqual(column5.FirstNode, node35);
        Assert.AreEqual(column5.LastNode, node45);
        // nodes row and column
        Assert.AreEqual(node00.Row, row0);
        Assert.AreEqual(node00.Column, column0);
        Assert.AreEqual(node03.Row, row0);
        Assert.AreEqual(node03.Column, column3);
        Assert.AreEqual(node06.Row, row0);
        Assert.AreEqual(node06.Column, column6);
        Assert.AreEqual(node10.Row, row1);
        Assert.AreEqual(node10.Column, column0);
        Assert.AreEqual(node13.Row, row1);
        Assert.AreEqual(node13.Column, column3);
        Assert.AreEqual(node23.Row, row2);
        Assert.AreEqual(node23.Column, column3);
        Assert.AreEqual(node24.Row, row2);
        Assert.AreEqual(node24.Column, column4);
        Assert.AreEqual(node32.Row, row3);
        Assert.AreEqual(node32.Column, column2);
        Assert.AreEqual(node34.Row, row3);
        Assert.AreEqual(node34.Column, column4);
        Assert.AreEqual(node35.Row, row3);
        Assert.AreEqual(node35.Column, column5);
        Assert.AreEqual(node41.Row, row4);
        Assert.AreEqual(node41.Column, column1);
        Assert.AreEqual(node42.Row, row4);
        Assert.AreEqual(node42.Column, column2);
        Assert.AreEqual(node45.Row, row4);
        Assert.AreEqual(node45.Column, column5);
        Assert.AreEqual(node46.Row, row4);
        Assert.AreEqual(node46.Column, column6);
        Assert.AreEqual(node51.Row, row5);
        Assert.AreEqual(node51.Column, column1);
        Assert.AreEqual(node56.Row, row5);
        Assert.AreEqual(node56.Column, column6);
        // nodes left right up down
        // node00
        AssertNode(node00, null, node03, null, node10);
        // node03
        AssertNode(node03, node00, node06, null, node13);
        // node06
        AssertNode(node06, node03, null, null, node46);
        // node10
        AssertNode(node10, null, node13, node00, null);
        // node13
        AssertNode(node13, node10, null, node03, node23);
        // node23
        AssertNode(node23, null, node24, node13, null);
        // node24
        AssertNode(node24, node23, null, null, node34);
        // node32
        AssertNode(node32, null, node34, null, node42);
        // node34
        AssertNode(node34, node32, node35, node24, null);
        // node35
        AssertNode(node35, node34, null, null, node45);
        // node41
        AssertNode(node41, null, node42, null, node51);
        // node42
        AssertNode(node42, node41, node45, node32, null);
        // node45
        AssertNode(node45, node42, node46, node35, null);
        // node46
        AssertNode(node46, node45, null, node06, node56);
        // node51
        AssertNode(node51, null, node56, node41, null);
        // node56
        AssertNode(node56, node51, null, node46, null);
    }

    [TestMethod]
    public void TestCreate()
    {
        //   0 3 6 4 2 5 1
        // 0 1 1 1 0 0 0 0
        // 1 1 1 0 0 0 0 0
        // 2 0 1 0 1 0 0 0
        // 3 0 0 0 1 1 1 0
        // 4 0 0 1 0 1 1 1
        // 5 0 0 1 0 0 0 1
        List<(int, int)> nodesList =
        [
            (0, 0),
            (0, 3),
            (0, 6),
            (1, 0),
            (1, 3),
            (2, 3),
            (2, 4),
            (3, 2),
            (3, 4),
            (3, 5),
            (4, 1),
            (4, 2),
            (4, 5),
            (4, 6),
            (5, 1),
            (5, 6),
        ];
        var dlx = new Dlx(nodesList);
        AssertDlx(dlx);
    }

    [TestMethod]
    public void TestClone()
    {
        //   0 3 6 4 2 5 1
        // 0 1 1 1 0 0 0 0
        // 1 1 1 0 0 0 0 0
        // 2 0 1 0 1 0 0 0
        // 3 0 0 0 1 1 1 0
        // 4 0 0 1 0 1 1 1
        // 5 0 0 1 0 0 0 1
        List<(int, int)> nodesList =
        [
            (0, 0),
            (0, 3),
            (0, 6),
            (1, 0),
            (1, 3),
            (2, 3),
            (2, 4),
            (3, 2),
            (3, 4),
            (3, 5),
            (4, 1),
            (4, 2),
            (4, 5),
            (4, 6),
            (5, 1),
            (5, 6),
        ];
        var dlx = new Dlx(nodesList);
        var dlxClone = dlx.Clone();
        AssertDlx(dlxClone);
        dlx.AddNode(1, 2);
        AssertDlx(dlxClone);
    }
}
