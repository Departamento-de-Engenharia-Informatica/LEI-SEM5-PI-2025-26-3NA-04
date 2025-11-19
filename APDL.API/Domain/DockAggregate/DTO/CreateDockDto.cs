using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.DockAggregate.DTO
{
    public class CreateDockDto
    {
        public string DockName { get; set; }
        public int DockLength { get; set; }
        public int DockDraft { get; set; }
    }
}
