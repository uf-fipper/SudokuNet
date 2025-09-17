using System.Collections.Generic;
using DanceLinkX.DlxDefault;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanceLinkX.Default.Tests;

[TestClass]
public class DlxIntTests
{
    [TestMethod]
    public void TestAddNode()
    {
        var dlx = new Dlx<int, int>();
        Assert.IsTrue(dlx.AddNode(1, 1));
        Assert.IsFalse(dlx.AddNode(1, 1)); // 重复添加应返回 false
    }

    [TestMethod]
    public void TestAddNodes()
    {
        var dlx = new Dlx<int, int>();
        var nodes = new List<(int, int)> { (1, 1), (2, 2), (3, 3) };
        var results = dlx.AddNodes(nodes);
        Assert.AreEqual(3, results.Length);
        Assert.IsTrue(results.All(r => r));
    }

    [TestMethod]
    public void TestTryGetNode()
    {
        var dlx = new Dlx<int, int>();
        dlx.AddNode(1, 1);
        Assert.IsTrue(dlx.TryGetNode(1, 1, out var node));
        Assert.IsNotNull(node);
        Assert.IsFalse(dlx.TryGetNode(2, 2, out _)); // 不存在的节点应返回 false
    }

    [TestMethod]
    public void TestClone()
    {
        var dlx = new Dlx<int, int>();
        dlx.AddNode(1, 1);
        var clone = dlx.Clone();
        Assert.IsTrue(clone.TryGetNode(1, 1, out _)); // 克隆应包含原节点
    }

    [TestMethod]
    public void TestDance()
    {
        var dlx = new Dlx<int, int>();
        dlx.AddNode(1, 1);
        dlx.AddNode(2, 2);
        var results = dlx.Dance().ToList();
        Assert.AreEqual(1, results.Count); // 预期结果数量
    }
}
