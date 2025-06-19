using System;

namespace ExtraHours.API.Model
{
    public class CompensationRequest
    {
        public int Id { get; set; }
        public long EmployeeId { get; set; }
        public DateTime WorkDate { get; set; } // Día trabajado (domingo/festivo)
        public DateTime RequestedCompensationDate { get; set; } // Día solicitado como compensación
        public string Status { get; set; } // Pending, Approved, Rejected
        public string? Justification { get; set; } // Motivo de rechazo o comentario de aprobación
        public long? ApprovedById { get; set; } // Manager o Superusuario que decide
        public DateTime RequestedAt { get; set; }
        public DateTime? DecidedAt { get; set; }

        // Relaciones de navegación (opcional, para EF Core)
        public Employee? Employee { get; set; }
        public User? ApprovedBy { get; set; }
    }
}
