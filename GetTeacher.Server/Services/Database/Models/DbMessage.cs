using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;


public class DbMessage
{

    public int Id { get; set; }

    public DateTime DateTime{ get; set; }

}