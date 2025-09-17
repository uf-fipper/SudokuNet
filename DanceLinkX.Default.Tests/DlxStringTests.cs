using System.Collections.Generic;
using DanceLinkX.DlxDefault;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanceLinkX.Default.Tests;

[TestClass]
public class DlxStringTests
{
    [TestMethod]
    public void TestAddNode()
    {
        var dlx = new Dlx<string, string>();
        Assert.IsTrue(dlx.AddNode("row1", "col1"));
        Assert.IsFalse(dlx.AddNode("row1", "col1")); // 重复添加应返回 false
    }

    [TestMethod]
    public void TestAddNodes()
    {
        var dlx = new Dlx<string, string>();
        var nodes = new List<(string, string)>
        {
            ("row1", "col1"),
            ("row2", "col2"),
            ("row3", "col3"),
        };
        var results = dlx.AddNodes(nodes);
        Assert.AreEqual(3, results.Length);
        Assert.IsTrue(results.All(r => r));
    }

    [TestMethod]
    public void TestTryGetNode()
    {
        var dlx = new Dlx<string, string>();
        dlx.AddNode("row1", "col1");
        Assert.IsTrue(dlx.TryGetNode("row1", "col1", out var node));
        Assert.IsNotNull(node);
        Assert.IsFalse(dlx.TryGetNode("row2", "col2", out _)); // 不存在的节点应返回 false
    }

    [TestMethod]
    public void TestClone()
    {
        var dlx = new Dlx<string, string>();
        dlx.AddNode("row1", "col1");
        var clone = dlx.Clone();
        Assert.IsTrue(clone.TryGetNode("row1", "col1", out _)); // 克隆应包含原节点
    }

    [TestMethod]
    public void TestDance()
    {
        var dlx = new Dlx<string, string>();
        dlx.AddNode("row1", "col1");
        dlx.AddNode("row2", "col2");
        var results = dlx.Dance().ToList();
        Assert.AreEqual(1, results.Count); // 预期结果数量
    }
}
