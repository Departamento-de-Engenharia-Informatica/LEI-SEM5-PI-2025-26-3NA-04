using System;
using APDL.API.Domain.Shared;
using Newtonsoft.Json;

namespace APDL.API.Domain.DockAggregate
{
    public class DockId : EntityId
    {
        [JsonConstructor]
        public DockId(Guid value)
            : base(value) { }

        public DockId(string value)
            : base(value) { }

        protected override object createFromString(string text)
        {
            return new Guid(text);
        }

        public override string AsString()
        {
            Guid obj = (Guid)base.ObjValue;
            return obj.ToString();
        }

        public Guid AsGuid()
        {
            return (Guid)base.ObjValue;
        }
    }
}
