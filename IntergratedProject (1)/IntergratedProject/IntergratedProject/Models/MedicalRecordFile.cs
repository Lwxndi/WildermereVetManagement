using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class MedicalRecordFile
    {
        [Key]

        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        [Required] public string FileName { get; set; }
        [Required] public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsPrescription { get; set; } = false;

        // Who uploaded it
        public int UploadedByVetId { get; set; }
        public string UploadedByName { get; set; }
    }
}
