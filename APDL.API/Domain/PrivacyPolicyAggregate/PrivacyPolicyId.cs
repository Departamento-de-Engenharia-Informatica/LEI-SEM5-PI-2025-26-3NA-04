using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.PrivacyPolicyAggregate
{
    public class PrivacyPolicyId : EntityId
    {
        public PrivacyPolicyId(Guid value) : base(value) { }

        public PrivacyPolicyId(string value) : base(value) { }

        protected override object createFromString(string text)
        {
            return Guid.Parse(text);
        }

        public override string AsString()
        {
            return ((Guid)ObjValue).ToString();
        }
    }
}

