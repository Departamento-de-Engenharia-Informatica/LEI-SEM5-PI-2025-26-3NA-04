using System;
using System.Collections.Generic;

namespace APDL.API.Domain.ShippingAgentAggregate.DTO
{
    public class CreateShippingAgentDto
    {
        public string LegalName { get; set; }
        public string? AlternativeName { get; set; }
        public string Address { get; set; }
        public string TaxNumber { get; set; }

        public List<CreateRepresentativeDto> Representatives { get; set; }
    }
}
