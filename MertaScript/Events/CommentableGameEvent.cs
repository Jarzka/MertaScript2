using MertaScript.Events;

namespace MertaScript;

public record CommentableGameEvent {
  public GameEventId Id { get; init; }
  public int CommentProbability { get; init; }
  public int Priority { get; init; }
}