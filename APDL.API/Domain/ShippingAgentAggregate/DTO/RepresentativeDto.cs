using System;

namespace APDL.API.Domain.ShippingAgentAggregate.DTO
{
    public class RepresentativeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CitizenId { get; set; }
        public string Nationality { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public RepresentativeDto() { }

        public RepresentativeDto(
            Guid id,
            string name,
            string citizenId,
            string nationality,
            string email,
            string phone,
            bool isActive)
        {
            Id = id;
            Name = name;
            CitizenId = citizenId;
            Nationality = nationality;
            Email = email;
            Phone = phone;
            IsActive = isActive;
        }
    }
}
