using System;

namespace IntergratedProject.ViewModels
{
    public class MedicalRecordFileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedByName { get; set; }
        public string UploadedByRole { get; set; }
    }
}
