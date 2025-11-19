using System;
using APDL.API.Domain.Shared;
using Newtonsoft.Json;

namespace APDL.API.Domain.QualificationsAggregate
{
    public class StaffQualificationId : EntityId
    {
        [JsonConstructor]
        public StaffQualificationId(Guid value)
            : base(value) { }

        public StaffQualificationId(string value)
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
