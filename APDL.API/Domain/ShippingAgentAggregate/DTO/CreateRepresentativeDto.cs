namespace APDL.API.Domain.ShippingAgentAggregate.DTO
{
    public class CreateRepresentativeDto
    {
        public string Name { get; set; }
        public string CitizenId { get; set; }
        public string Nationality { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public CreateRepresentativeDto() { }

        public CreateRepresentativeDto(
            string name,
            string citizenId,
            string nationality,
            string email,
            string phone)
        {
            Name = name;
            CitizenId = citizenId;
            Nationality = nationality;
            Email = email;
            Phone = phone;
        }
    }
}
