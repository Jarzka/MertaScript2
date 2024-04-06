namespace MertaScript.EventHandling;

public record CsPlayer(string Name, string[] Aliases) {
  public List<string> PlayerNameWithAliases() {
    var aliasesWithPlayerName = new List<string> { Name };
    aliasesWithPlayerName.AddRange(Aliases);
    return aliasesWithPlayerName;
  }
}