using System.ComponentModel.DataAnnotations;

public class ReservationModifyModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public string Status { get; set; }

    [Display(Name = "RegisterDate")]
    [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime RegisterDate { get; set; }

    [Display(Name = "ExpireDate")]
    [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime ExpireDate { get; set; }

    public override string ToString()
    {
        return $"ReservationModifyModel: " +
               $"Id = {Id}, " +
               $"Email = {Email}, " +
               $"Title = {Title}, " +
               $"ISBN = {ISBN}, " +
               $"Status = {Status}, " +
               $"RegisterDate = {RegisterDate:dd/MM/yyyy}, " +
               $"ExpireDate = {ExpireDate:dd/MM/yyyy}";
    }
}
