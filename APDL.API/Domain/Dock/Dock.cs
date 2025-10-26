using System;
using System.Collections.Generic;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Dock;

public class Dock
{
    public DockId Id { get; private set; }
    public string Name { get; private set; }

    public Dock(string name)
    {
        Id = new DockId(Guid.NewGuid());
        Name = name;
    }

    protected Dock() { }
}
