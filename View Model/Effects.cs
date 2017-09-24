using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Buffet
{
    public struct Effect
    {
        public Regex activationRegex;
        public Regex duractionDetectorRegex;
        public Regex deactivationRegex;
        public EffectExtender[] extensions;
        public EffectChangeByTime physicalChangeByTime;
        public EffectChangeByTime magicChangeByTime;
        public EffectChangeByTime criticalChangeByTime;
        public string id;
        public string name;
        public int? duration;
        public bool stackable;
        public int physicalPower;
        public int magicPower;
        public int criticalPower;
    }
    public class EffectExtender
    {
        public int extension;
        public Regex activationRegex;
    }
    public class EffectChangeByTime
    {
        public int interval;
        public int change;
    }

    public class ActiveEffect
    {
        public string source;
        public string target;
        public int? duration;
        public Effect effect;
        public int physicalPower;
        public int magicPower;
        public int criticalPower;
        public DateTime activationTime;
    }

    public class Effects
    {
        private static string characterNameRegex = "[a-zA-z'\\-]{2,15} [a-zA-z'\\-]{2,15}";
        private static EffectExtender[] ASTEffectExtensions = new EffectExtender[]
        {
            new EffectExtender() {extension = 10, activationRegex = new Regex(":(?<source>"+characterNameRegex+"):.*:Celestial Opposition:.*:(?<target>"+characterNameRegex+")")},
            new EffectExtender() {extension = 15, activationRegex = new Regex(":(?<source>"+characterNameRegex+"):.*:Time Dilation:.*:(?<target>"+characterNameRegex+")")},
        };

        public static Effect TrickAttack = new Effect()
        {
            id = "Trick Attack",
            name = "Trick Attack",
            stackable = false,
            duration = 10,
            physicalPower = 10,
            magicPower = 10,
            activationRegex = new Regex(" ..:(?<target>" + characterNameRegex + ") gains the effect of Vulnerability Up from (?<source>" + characterNameRegex + ") for 10.00 Seconds.*"),
            deactivationRegex = new Regex(" ..:(?<target>" + characterNameRegex + ") loses.*Vulnerability Up.*from (?<source>" + characterNameRegex + ").")
        };

        public static Effect Hypercharge = new Effect()
        {
            id = "Hypercharge",
            name = "Hypercharge",
            stackable = false,
            duration = 10,
            physicalPower = 5,
            magicPower = 5,
            activationRegex = new Regex(" ..:(?<target>" + characterNameRegex + ") gains the effect of Vulnerability Up from (?<source>(Rook|Bishop) Autoturret) for 10.00 Seconds.*"),
            deactivationRegex = new Regex(" ..:(?<target>" + characterNameRegex + ") loses.*Vulnerability Up.*from (?<source>" + characterNameRegex + ").")
        };

        public static Effect BattleLitany = new Effect()
        {
            id = "Battle Litany",
            name = "Battle Litany",
            stackable = false,
            duration = 20,
            criticalPower = 15,
            activationRegex = new Regex(".*(?<target>You) gain the effect of.*Battle Litany"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Battle Litany")
        };
        public static Effect DragonSight = new Effect()
        {
            id = "Dragon Sight",
            name = "Dragon Sight",
            stackable = false,
            duration = 20,
            physicalPower = 5,
            activationRegex = new Regex(".*(?<target>You) gain the effect of.*Left Eye"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Left Eye")
        };

        public static Effect SelfEmbolden = new Effect()
        {
            id = "Embolden",
            name = "Self Embolden",
            stackable = false,
            duration = 20,
            magicPower = 10,
            magicChangeByTime = new EffectChangeByTime() { interval = 4, change = -2 },
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*:Embolden:.*:(?<target>\\1)"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Embolden")
        };

        public static Effect Embolden = new Effect()
        {
            id = "Embolden",
            name = "Ally Embolden",
            stackable = false,
            duration = 20,
            physicalPower = 10,
            physicalChangeByTime = new EffectChangeByTime() { interval = 4, change = -2 },
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*:Embolden:.*:(?!<target>\\1)"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Embolden")
        };

        public static Effect Brotherhood = new Effect()
        {
            id = "Brotherhood",
            name = "Brotherhood",
            stackable = false,
            duration = 15,
            physicalPower = 5,
            activationRegex = new Regex(".*(?<target>You) gain the effect of.*Brotherhood"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Brotherhood")
        };

        public static Effect ChainStrategem = new Effect()
        {
            id = "Chain Strategem",
            name = "Chain Strategem",
            stackable = false,
            duration = 15,
            criticalPower = 15,
            activationRegex = new Regex(" ..:(?<target>" + characterNameRegex + ") gains the effect of Chain Strategem from (?<source>" + characterNameRegex + ") for 10.00 Seconds.*"),
            deactivationRegex = new Regex(" ..:(?<target>" + characterNameRegex + ") loses.*Chain Strategem.*from (?<source>" + characterNameRegex + ").")
        };

        public static Effect BattleVoice = new Effect()
        {
            id = "Battle Voice",
            name = "Battle Voice",
            stackable = false,
            duration = 20,
            criticalPower = 8,
            activationRegex = new Regex(".*(?<target>You) gain the effect of.*Battle Voice"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Battle Voice")
        };

        public static Effect BRDSong = new Effect()
        {
            id = "BRD Song",
            name = "Bard Song",
            stackable = false,
            criticalPower = 2,
            activationRegex = new Regex(".*(?<target>You) gain the effect of.*Critical Up"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Critical Up")
        };

        public static Effect Devotion = new Effect()
        {
            id = "Devotion",
            name = "Devotion",
            stackable = false,
            duration = 15,
            physicalPower = 5,
            magicPower = 5,
            activationRegex = new Regex(".*(?<target>You) gain the effect of.*Devotion"),
            deactivationRegex = new Regex(".*(?<target>You) lose the effect of.*Devotion")
        };

        public static Effect HighBalance = new Effect()
        {
            id = "AST Buff",
            name = "Enhanced Balance",
            stackable = false,
            physicalPower = 15,
            magicPower = 15,
            duractionDetectorRegex = new Regex(":(?<target>" + characterNameRegex + ") gains the effect of (?<card>The Balance) from (?<source>" + characterNameRegex + ") for (?<duration>[0-9]+\\.[0-9]+) Seconds\\."),
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*The Balance:.*:(?<target>" + characterNameRegex + "):F0F:33D"),
            deactivationRegex = new Regex(".*You (gain the effect of.*(The (Ewer|Arrow|Spire|Bole|Spear)))|(lose the effect of .*The Balance)"),
            extensions = ASTEffectExtensions
        };
        public static Effect HighSpear = new Effect()
        {
            id = "AST Buff",
            name = "Enhanced Spear",
            stackable = false,
            criticalPower = 15,
            duractionDetectorRegex = new Regex(":(?<target>" + characterNameRegex + ") gains the effect of (?<card>The Spear) from (?<source>" + characterNameRegex + ") for (?<duration>[0-9]+\\.[0-9]+) Seconds\\."),
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*The Spear:.*:(?<target>" + characterNameRegex + "):F0F:340"),
            deactivationRegex = new Regex(".*You (gain the effect of.*(The (Ewer|Arrow|Spire|Bole|Balance)))|(lose the effect of .*The Spear)"),
            extensions = ASTEffectExtensions
        };
        public static Effect Balance = new Effect()
        {
            id = "AST Buff",
            name = "Balance",
            stackable = false,
            physicalPower = 10,
            magicPower = 10,
            duractionDetectorRegex = new Regex(":(?<target>" + characterNameRegex + ") gains the effect of (?<card>The Balance) from (?<source>" + characterNameRegex + ") for (?<duration>[0-9]+\\.[0-9]+) Seconds\\."),
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*(?<card>The Balance):.*:(?<target>" + characterNameRegex + "):A0F:33D"),
            deactivationRegex = new Regex(".*You (gain the effect of.*(The (Ewer|Arrow|Spire|Bole|Spear)))|(lose the effect of .*The Balance)"),
            extensions = ASTEffectExtensions
        };
        public static Effect Spear = new Effect()
        {
            id = "AST Buff",
            name = "Spear",
            stackable = false,
            criticalPower = 10,
            duractionDetectorRegex = new Regex(":(?<target>" + characterNameRegex + ") gains the effect of (?<card>The Spear) from (?<source>" + characterNameRegex + ") for (?<duration>[0-9]+\\.[0-9]+) Seconds\\."),
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*The Spear:.*:(?<target>" + characterNameRegex + "):A0F:340"),
            deactivationRegex = new Regex(".*You (gain the effect of.*(The (Ewer|Arrow|Spire|Bole|Balance)))|(lose the effect of .*The Spear)"),
            extensions = ASTEffectExtensions
        };
        public static Effect LowBalance = new Effect()
        {
            id = "AST Buff",
            name = "Extended Balance",
            stackable = false,
            physicalPower = 5,
            magicPower = 5,
            duractionDetectorRegex = new Regex(":(?<target>" + characterNameRegex + ") gains the effect of (?<card>The Balance) from (?<source>" + characterNameRegex + ") for (?<duration>[0-9]+\\.[0-9]+) Seconds\\."),
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*The Balance:.*:(?<target>" + characterNameRegex + "):50F:33D"),
            deactivationRegex = new Regex(".*You (gain the effect of.*(The (Ewer|Arrow|Spire|Bole|Spear)))|(lose the effect of .*The Balance)"),
            extensions = ASTEffectExtensions
        };
        public static Effect LowSpear = new Effect()
        {
            id = "AST Buff",
            name = "Extended Spear",
            stackable = false,
            criticalPower = 5,
            duractionDetectorRegex = new Regex(":(?<target>" + characterNameRegex + ") gains the effect of (?<card>The Spear) from (?<source>" + characterNameRegex + ") for (?<duration>[0-9]+\\.[0-9]+) Seconds\\."),
            activationRegex = new Regex(":(?<source>" + characterNameRegex + "):.*The Spear:.*:(?<target>" + characterNameRegex + "):50F:340"),
            deactivationRegex = new Regex(".*You (gain the effect of.*(The (Ewer|Arrow|Spire|Bole|Balance)))|(lose the effect of .*The Spear)"),
            extensions = ASTEffectExtensions
        };

        public static IEnumerable<Effect> AllEffects()
        {
            yield return Hypercharge;
            yield return TrickAttack;
            yield return BattleLitany;
            yield return DragonSight;
            yield return SelfEmbolden;
            yield return Embolden;
            yield return Brotherhood;
            yield return ChainStrategem;
            yield return BattleVoice;
            yield return BRDSong;
            yield return Devotion;
            yield return HighBalance;
            yield return Balance;
            yield return LowBalance;
            yield return HighSpear;
            yield return Spear;
            yield return LowSpear;
        }
    }
}
