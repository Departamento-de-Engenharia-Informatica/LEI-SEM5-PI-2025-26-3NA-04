using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Dock;
public class DockId : EntityId
{
    public DockId(Guid value) : base(value) { }

    public DockId(string value) : base(value) { }

    protected override object createFromString(string text)
    {
        return new Guid(text);
    }

    public override string AsString()
    {
        return ((Guid)ObjValue).ToString();
    }

    public Guid AsGuid()
    {
        return (Guid)ObjValue;
    }
}