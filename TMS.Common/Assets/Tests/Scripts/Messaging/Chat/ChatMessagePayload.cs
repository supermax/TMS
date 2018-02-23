public class ChatMessagePayload : ChatMessageBase, IChatMessage
{
	public string Text { get; set; }
}

public interface IChatMessage
{
	string SenderId { get; }

	string Text { get; set; }
}

public abstract class ChatMessageBase
{
	public string SenderId { get; set; }
}