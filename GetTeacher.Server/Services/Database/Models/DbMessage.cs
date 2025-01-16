using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;


public class DbMessage
{

    public int Id { get; set; }


    [ForeignKey(nameof(Sender))]

    public required int SenderId { get; set; }
    public required virtual DbUser Sender { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public required string Content { get; set; }

}