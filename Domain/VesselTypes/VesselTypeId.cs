using System;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.VesselTypes
{
    public class VesselTypeId : EntityId
    {
        public VesselTypeId(Guid value) : base(value) { }
        public VesselTypeId(string value) : base(Guid.Parse(value)) { }

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
