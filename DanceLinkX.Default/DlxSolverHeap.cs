namespace DanceLinkX.DlxDefault;

public class DlxSolverHeapValue<TElement, TCompare>(TElement element, TCompare compare)
    where TElement : notnull
    where TCompare : IComparable<TCompare>
{
    public TElement Element = element;

    public TCompare Compare = compare;

    public override string ToString()
    {
        return Compare.ToString()!;
    }
}

public class DlxSolverHeap<TElement, TCompare>
    where TElement : notnull
    where TCompare : IComparable<TCompare>
{
    public DlxSolverHeap(IEnumerable<(TElement element, TCompare compare)> values)
    {
        Values = values
            .Select(x => new DlxSolverHeapValue<TElement, TCompare>(x.element, x.compare))
            .ToArray();
        // 调整堆
        for (int i = Values.Length / 2 - 1; i >= 0; i--)
        {
            AdjectDown(i);
        }
        for (int i = 0; i < Values.Length; i++)
        {
            IndexMap[Values[i].Element] = i;
        }
    }

    public DlxSolverHeapValue<TElement, TCompare>[] Values { get; }

    public Dictionary<TElement, int> IndexMap = [];

    public TElement Select() => Values[0].Element;

    public void SetValue(TElement element, TCompare value)
    {
        int i = IndexMap[element];
        var self = Values[i];
        var rawValue = self.Compare;
        self.Compare = value;
        switch (rawValue.CompareTo(value))
        {
            case > 0:
                AdjectUp(i);
                break;
            case < 0:
                AdjectDown(i);
                break;
        }
    }

    public void AdjectUp(TElement element) => AdjectUp(IndexMap[element]);

    public void AdjectUp(int i)
    {
        if (i == 0)
            return;
        ref var self = ref Values[i];
        // compare parent
        int parentIndex = (i - 1) / 2;
        ref var parent = ref Values[parentIndex];
        if (parent.Compare.CompareTo(self.Compare) > 0)
        {
            (self, parent) = (parent, self);
            IndexMap[self.Element] = i;
            IndexMap[parent.Element] = parentIndex;
            AdjectUp(parentIndex);
        }
    }

    public void AdjectDown(TElement element) => AdjectDown(IndexMap[element]);

    public void AdjectDown(int i)
    {
        if (Values.Length <= i * 2 + 1)
            return;
        ref var self = ref Values[i];
        ref var leftChild = ref Values[i * 2 + 1];
        ref var child = ref leftChild;
        int childIndex = i * 2 + 1;
        if (Values.Length > i * 2 + 2)
        {
            ref var rightChild = ref Values[i * 2 + 2];
            if (leftChild.Compare.CompareTo(rightChild.Compare) > 0)
            {
                child = ref rightChild;
                childIndex = i * 2 + 2;
            }
        }
        if (child.Compare.CompareTo(self.Compare) < 0)
        {
            (self, child) = (child, self);
            IndexMap[self.Element] = i;
            IndexMap[child.Element] = childIndex;
            AdjectDown(childIndex);
        }
    }
}
