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
        TokenWaiter color = new TokenWaiter(t => t is ConstantToken{Value: StringVariant{Value:"ffeed5"}});
        
        foreach (var token in tokens)
        {
            if (color.Check(token))
            {
                if (!test)// this is very cursed, but it works because it patches the leaving message first then the joining one
                {
                    test = true;
                    yield return new ConstantToken(new StringVariant(Mod.Instance.Config.Leave.Replace("#", ""))); //leave
                }
                else
                {
                    test = false;
                    yield return new ConstantToken(new StringVariant(Mod.Instance.Config.Join.Replace("#", ""))); //join
                }
                color.Reset();
            }
            else
            {
                yield return token;
            }
        }
    }
}