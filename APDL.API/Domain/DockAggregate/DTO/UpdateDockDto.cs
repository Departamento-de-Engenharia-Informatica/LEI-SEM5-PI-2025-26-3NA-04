using System;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.DockAggregate.DTO
{
    public class UpdateDockDto
    {
        public Guid Id { get; set; }

        public string DockName { get; set; }

        public int DockLength { get; set; }

        public int DockDraft { get; set; }
    }
}
