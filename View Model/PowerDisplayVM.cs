using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buffet
{
    public class PowerDisplayVM : ViewModelBase
    {
        private int _PhysicalPower;
        [YamlDotNet.Serialization.YamlIgnore]
        public int PhysicalPower
        {
            get { return _PhysicalPower; }
            set { SetProperty(ref _PhysicalPower, value); }
        }
        private int _MagicPower;
        [YamlDotNet.Serialization.YamlIgnore]
        public int MagicPower
        {
            get { return _MagicPower; }
            set { SetProperty(ref _MagicPower, value); }
        }
        private int _CriticalPower;
        [YamlDotNet.Serialization.YamlIgnore]
        public int CriticalPower
        {
            get { return _CriticalPower; }
            set { SetProperty(ref _CriticalPower, value); }
        }
        private int _TimeRemaining;
        [YamlDotNet.Serialization.YamlIgnore]
        public int TimeRemaining
        {
            get { return _TimeRemaining; }
            set { SetProperty(ref _TimeRemaining, value); }
        }

        private bool _DisplayPhysical;
        public bool DisplayPhysical
        {
            get { return _DisplayPhysical; }
            set { SetProperty(ref _DisplayPhysical, value); }
        }

        private bool _DisplayMagic;
        public bool DisplayMagic
        {
            get { return _DisplayMagic; }
            set { SetProperty(ref _DisplayMagic, value); }
        }

        private bool _DisplaySimple;
        public bool DisplaySimple
        {
            get { return _DisplaySimple; }
            set { SetProperty(ref _DisplaySimple, value); }
        }

        [YamlDotNet.Serialization.YamlIgnore]
        public int OverallPhysicalPower
        {
            get { return PhysicalPower + (int)Math.Floor(CriticalPower * 0.5f); }
        }
        [YamlDotNet.Serialization.YamlIgnore]
        public int OverallMagicPower
        {
            get { return MagicPower + (int)Math.Floor(CriticalPower * 0.5f); }
        }

        private bool _AlwaysDisplayBackground;
        public bool AlwaysDisplayBackground
        {
            get { return _AlwaysDisplayBackground; }
            set { SetProperty(ref _AlwaysDisplayBackground, value); }
        }

        private bool _HideNumbersWhenZero;
        public bool HideNumbersWhenZero
        {
            get { return _HideNumbersWhenZero; }
            set { SetProperty(ref _HideNumbersWhenZero, value); }
        }

        private bool _EnableClickThrough;
        public bool EnableClickThrough
        {
            get { return _EnableClickThrough; }
            set { SetProperty(ref _EnableClickThrough, value); }
        }

        public PowerDisplayVM()
        {
            RegisterReflectionSources(nameof(OverallPhysicalPower), nameof(PhysicalPower), nameof(CriticalPower));
            RegisterReflectionSources(nameof(OverallMagicPower), nameof(MagicPower), nameof(CriticalPower));
        }
    }
}
