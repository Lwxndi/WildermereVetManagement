namespace IntergratedProject.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public string PDFUrl { get; set; }
    }
}
