using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage
{
    public class FacilityId : EntityId
    {
        public FacilityId(Guid value) : base(value) { }

        public FacilityId(string value) : base(value) { }

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
