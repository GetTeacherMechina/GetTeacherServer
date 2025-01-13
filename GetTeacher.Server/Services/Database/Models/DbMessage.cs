using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;


public class DbMessage
{

    public int Id { get; set; }


    [ForeignKey(nameof(DbUser))]

    public required int DbUserId { get; set; }
    public required virtual DbUser DbUser { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public required string Content { get; set; }

}