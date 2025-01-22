namespace GetTeacher.Server.Models.Chats;

public class SendMessageResponseModel
{
	public int Id { get; set; }

	public int SenderId { get; set; }

	public string SenderName { get; set; } = string.Empty;

	public string Content { get; set; } = string.Empty;

	public DateTime SendTime { get; set; }
}