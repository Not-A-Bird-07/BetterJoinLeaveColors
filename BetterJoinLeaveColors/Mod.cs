using GDWeave;

namespace BetterJoinLeaveColors;

public class Mod : IMod {
    public Config Config;
    public static Mod Instance;

    public Mod(IModInterface modInterface) {
        this.Config = modInterface.ReadConfig<Config>();
        modInterface.RegisterScriptMod(new Join_Leave_Patch());
        Instance = this;
    }

    public void Dispose() {
    }
}
