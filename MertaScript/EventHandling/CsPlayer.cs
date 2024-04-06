namespace MertaScript.EventHandling;

// NOTE: This class cannot be abstract because an instance is created via JSON serialization.
public record CsPlayer(string Name, string[] Aliases) {
  public List<string> PlayerNameWithAliases() {
    var aliasesWithPlayerName = new List<string> { Name };
    aliasesWithPlayerName.AddRange(Aliases);
    return aliasesWithPlayerName;
  }
}