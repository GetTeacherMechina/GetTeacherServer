namespace GetTeacher.Server.Models.Chats;

public class ChatCreateModelRequest
{
	public required ICollection<int> Users { get; set; }
}