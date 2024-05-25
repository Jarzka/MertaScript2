using MertaScript.Network;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MertaScript.Tests.Network;

[TestFixture]
public class MessageParserTests {
  [Test]
  public void ParseCompleteNetworkMessage_WhenGivenEmptyString_ReturnsEmptyList() {
    const string receivedString = "";
    var result = NetworkManager.ParseCompleteNetworkMessages(receivedString);
    Assert.AreEqual(result.Count, 0);
  }

  [Test]
  public void ParseCompleteNetworkMessage_WhenGivenSingleMessage_ReturnsListWithSingleMessage() {
    const string receivedString = "<message>";

    var result = NetworkManager.ParseCompleteNetworkMessages(receivedString);

    Assert.AreEqual(1, result.Count);
    Assert.AreEqual("<message>", result[0]);
  }

  [Test]
  public void ParseCompleteNetworkMessage_WhenGivenMultipleMessages_ReturnsListWithAllMessages() {
    const string receivedString = "<msg1><msg2><msg3>";

    var result = NetworkManager.ParseCompleteNetworkMessages(receivedString);

    Assert.AreEqual(3, result.Count);
    Assert.AreEqual("<msg1>", result[0]);
    Assert.AreEqual("<msg2>", result[1]);
    Assert.AreEqual("<msg3>", result[2]);
  }

  [Test]
  public void ParseCompleteNetworkMessage_WhenGivenSomePartialMessage_ReturnsListWithCompleteAllMessages() {
    const string receivedString = "<msg1><msg2><msg3><ms";

    var result = NetworkManager.ParseCompleteNetworkMessages(receivedString);

    Assert.AreEqual(3, result.Count);
    Assert.AreEqual("<msg1>", result[0]);
    Assert.AreEqual("<msg2>", result[1]);
    Assert.AreEqual("<msg3>", result[2]);
  }

  [Test]
  public void ParsePartialEndMessage_InputContainsEmptyString_ReturnsEmptyString() {
    const string input = "";

    var result = NetworkManager.ParsePartialEndMessage(input);

    Assert.AreEqual("", result);
  }

  [Test]
  public void ParsePartialEndMessage_InputContainsOnlyCompleteMessages_ReturnsEmptyString() {
    const string input = "<Some message><Another message>";

    var result = NetworkManager.ParsePartialEndMessage(input);

    Assert.AreEqual("", result);
  }

  [Test]
  public void ParsePartialEndMessage_InputContainsPartialMessage_ReturnsPartialMessage() {
    const string input = "<Some message><Another message><Partial Mes";

    var result = NetworkManager.ParsePartialEndMessage(input);

    Assert.AreEqual("<Partial Mes", result);
  }
}