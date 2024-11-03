using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace BetterJoinLeaveColors;

public class Join_Leave_Patch : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Singletons/SteamNetwork.gdc";
    bool test;

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        // username = username.replace("]", "")
        // _update_chat(
        MultiTokenWaiter usernamecheck = new MultiTokenWaiter([
            t => t is IdentifierToken{Name:"username"}, // username
            t => t.Type is TokenType.OpAssign, // =
            t => t is IdentifierToken{Name:"username"}, // username
            t => t.Type is TokenType.Period, // .
            t => t is IdentifierToken {Name: "replace"}, // replace
            t => t.Type is TokenType.ParenthesisOpen, // (
            t => t is ConstantToken{Value: StringVariant {Value:"]"}}, // "]"
            t => t.Type is TokenType.Comma, // ,
            t => t is ConstantToken{Value: StringVariant {Value:""}}, // ""
            t => t.Type is TokenType.ParenthesisClose, // )
            t => t.Type is TokenType.Newline,
            t => t is IdentifierToken{Name:"_update_chat"}, // _update_chat
            t => t.Type is TokenType.ParenthesisOpen, // (
        ]);
        // _delayed_join_message(making_change_id, " joined the game."
        MultiTokenWaiter join = new MultiTokenWaiter([
            t => t is IdentifierToken{Name:"_delayed_join_message"}, // message
            t => t.Type is TokenType.ParenthesisOpen, // (
            t => t is IdentifierToken{Name:"making_change_id"}, // making_change_id
            t => t.Type is TokenType.Comma,
            t => t is ConstantToken{Value: StringVariant{Value:" joined the game."}}
        ]);
        // " left the game."
        TokenWaiter leave = new TokenWaiter(t => t is ConstantToken{Value: StringVariant{Value:" left the game."}});
        
        foreach (var token in tokens)
        {
            if (usernamecheck.Check(token))
            {
                yield return token;
                if (!test)// this is very cursed, but it works because it patches the leaving message first then the joining one
                {
                    test = true;
                    yield return new ConstantToken(new StringVariant($"[color={Mod.Instance.Config.Leave}]")); //leave
                }
                else
                {
                    test = false;
                    yield return new ConstantToken(new StringVariant($"[color={Mod.Instance.Config.Join}]")); //join
                }
                
                yield return new Token(TokenType.OpAdd);
                usernamecheck.Reset();
            }
            else if (join.Check(token))
            {
                yield return new ConstantToken(new StringVariant(" joined the game.[/color]"));
            }
            else if (leave.Check(token))
            {
                yield return new ConstantToken(new StringVariant(" left the game.[/color]"));
            }
            else
            {
                yield return token;
            }
        }
    }
}