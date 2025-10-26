using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Vessels
{
    public class VesselId : EntityId
    {
        public VesselId(Guid value) : base(value) { }

        public VesselId(string value) : base(value) { }

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
}