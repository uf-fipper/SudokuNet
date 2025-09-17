using System.Collections.Generic;
using DanceLinkX.DlxDefault;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanceLinkX.Default.Tests;

public class CustomRow
{
    public int Id { get; set; }
}

public class CustomColumn
{
    public int Id { get; set; }
}

[TestClass]
public class DlxCustomClassTests
{
    [TestMethod]
    public void TestAddNode()
    {
        var dlx = new Dlx<CustomRow, CustomColumn>();
        var row = new CustomRow { Id = 1 };
        var col = new CustomColumn { Id = 1 };
        Assert.IsTrue(dlx.AddNode(row, col));
        Assert.IsFalse(dlx.AddNode(row, col)); // 重复添加应返回 false
    }

    [TestMethod]
    public void TestAddNodes()
    {
        var dlx = new Dlx<CustomRow, CustomColumn>();
        var row1 = new CustomRow { Id = 1 };
        var row2 = new CustomRow { Id = 2 };
        var col1 = new CustomColumn { Id = 1 };
        var col2 = new CustomColumn { Id = 2 };
        var nodes = new List<(CustomRow, CustomColumn)> { (row1, col1), (row2, col2) };
        var results = dlx.AddNodes(nodes);
        Assert.AreEqual(2, results.Length);
        Assert.IsTrue(results.All(r => r));
    }

    [TestMethod]
    public void TestTryGetNode()
    {
        var dlx = new Dlx<CustomRow, CustomColumn>();
        var row = new CustomRow { Id = 1 };
        var col = new CustomColumn { Id = 1 };
        dlx.AddNode(row, col);
        Assert.IsTrue(dlx.TryGetNode(row, col, out var node));
        Assert.IsNotNull(node);
        Assert.IsFalse(
            dlx.TryGetNode(new CustomRow { Id = 2 }, new CustomColumn { Id = 2 }, out _)
        ); // 不存在的节点应返回 false
    }

    [TestMethod]
    public void TestClone()
    {
        var dlx = new Dlx<CustomRow, CustomColumn>();
        var row = new CustomRow { Id = 1 };
        var col = new CustomColumn { Id = 1 };
        dlx.AddNode(row, col);
        var clone = dlx.Clone();
        Assert.IsTrue(clone.TryGetNode(row, col, out _)); // 克隆应包含原节点
    }

    [TestMethod]
    public void TestDance()
    {
        var dlx = new Dlx<CustomRow, CustomColumn>();
        var row1 = new CustomRow { Id = 1 };
        var row2 = new CustomRow { Id = 2 };
        var col1 = new CustomColumn { Id = 1 };
        var col2 = new CustomColumn { Id = 2 };
        dlx.AddNode(row1, col1);
        dlx.AddNode(row2, col2);
        var results = dlx.Dance().ToList();
        Assert.AreEqual(1, results.Count); // 预期结果数量
    }
}
