using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Buffet
{
    public class BuffetVM : ViewModelBase
    {
        private ObservableCollection<ActiveEffect> effects = new ObservableCollection<ActiveEffect>();

        private Queue<string> backLog = new Queue<string>();
        private static int max_backlog = 20;

        public PowerDisplayVM PowerDisplay
        { get; set; }

        public SoundSettingsVM DefaultSoundSettings
        { get; set; }

        public ObservableCollection<SoundSettingsVM> SoundSettings
        { get; private set; }

        public BuffetVM()
        {
            DefaultSoundSettings = new SoundSettingsVM() { BuffName = "Default buff" };

            PowerDisplay = new PowerDisplayVM();
            SoundSettings = new ObservableCollection<SoundSettingsVM>();

            effects.CollectionChanged += effects_CollectionChanged;

            Timer t = new Timer(200);
            t.Start();
            t.Elapsed += CheckTimeRemaining;
        }

        public void InitializeWindowManager(ViewModelWindowManager windowManager)
        {
            DefaultSoundSettings.InitializeWindowManager(windowManager);
            foreach (var ss in SoundSettings)
                ss.InitializeWindowManager(windowManager);
        }

        public void HandleLog(string logLine, DateTime detectedTime)
        {
            backLog.Enqueue(logLine);
            if (backLog.Count > max_backlog) backLog.Dequeue();

            // job change
            if (Regex.IsMatch(logLine, "You change to"))
                effects.Clear();

            // Effects
            foreach (var effect in Effects.AllEffects())
            {
                var activated = effect.activationRegex.Match(logLine);
                if (activated.Success)
                {
                    AddEffect(effect, activated.Groups["source"].ToString(), activated.Groups["target"].ToString(), detectedTime);
                    return;
                }
                else if (effect.deactivationRegex != null)
                {
                    var deactivated = effect.deactivationRegex.Match(logLine);
                    if (deactivated.Success)
                    {
                        if (RemoveEffect(effect, deactivated.Groups["source"].ToString(), deactivated.Groups["target"].ToString())) return;
                    }
                }
            }

            // Effect Extensions
            foreach (var activeEffect in effects)
            {
                if (activeEffect.effect.extensions != null && activeEffect.effect.extensions.Any())
                {
                    foreach (var extension in activeEffect.effect.extensions)
                    {
                        var extensionMatch = extension.activationRegex.Match(logLine);
                        if (extensionMatch.Success)
                        {
                            var target = extensionMatch.Groups["target"].ToString();
                            var targetMatches = string.IsNullOrEmpty(target) || target == activeEffect.target;
                            var sourceMatches = extensionMatch.Groups["source"].ToString() == activeEffect.source;
                            if (targetMatches && sourceMatches)
                                activeEffect.duration += extension.extension;
                        }
                    }
                }
            }
        }

        private void AddEffect(Effect effect, string source, string target, DateTime detectedTime)
        {
            foreach (var activeEffect in effects.ToArray())
            {
                if (activeEffect.effect.id == effect.id && ((source == activeEffect.source && target == activeEffect.target) || !effect.stackable))
                {
                    effects.Remove(activeEffect);
                }
            }
            effects.Add(new ActiveEffect() { activationTime = detectedTime, effect = effect, source = source, target = target, duration = effect.duration, physicalPower = effect.physicalPower, criticalPower = effect.criticalPower, magicPower = effect.magicPower });
            Playsound(effect.name, true);
        }

        private (string path, int volume, bool playSound) GetSoundSettingsFor(string buffName, bool granted = true)
        {
            string path = null;
            int volume = 0;
            bool playSound = true;
            if (granted)
            {
                path = DefaultSoundSettings.BuffGrantedPath;
                volume = DefaultSoundSettings.BuffGrantedVolume;

                foreach (var settings in (from ss in SoundSettings where ss.BuffName == buffName select ss).Reverse())
                {
                    if (!string.IsNullOrEmpty(settings.BuffGrantedPath)) path = settings.BuffGrantedPath;
                    volume = settings.BuffGrantedVolume;
                    playSound = settings.PlayBuffGrantedSound;
                }
            }
            else
            {
                path = DefaultSoundSettings.BuffEndedPath;
                volume = DefaultSoundSettings.BuffEndedVolume;

                foreach (var settings in (from ss in SoundSettings where ss.BuffName == buffName select ss).Reverse())
                {
                    if (!string.IsNullOrEmpty(settings.BuffEndedPath)) path = settings.BuffEndedPath;
                    volume = settings.BuffEndedVolume;
                    playSound = settings.PlayBuffEndedSound;
                }
            }

            return (path, volume, playSound);
        }

        private bool RemoveEffect(Effect effect, string source, string target)
        {
            foreach (var activeEffect in effects)
            {
                var idEqual = activeEffect.effect.id == effect.id;
                var nameEqual = activeEffect.effect.name == effect.name;
                var sourceEqual = string.IsNullOrEmpty(source) || source == activeEffect.source;
                var targetEqual = string.IsNullOrEmpty(target) ||
                    target == activeEffect.target;
                if (idEqual && nameEqual && sourceEqual && targetEqual)
                {
                    effects.Remove(activeEffect);
                    Playsound(effect.name, false);
                    return true;
                }
            }
            return false;
        }

        private void effects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PowerDisplay.PhysicalPower = 0;
            PowerDisplay.MagicPower = 0;
            PowerDisplay.CriticalPower = 0;
            var timeRemaining = 0.0;
            foreach (var effect in effects)
            {
                PowerDisplay.PhysicalPower += effect.physicalPower;
                PowerDisplay.MagicPower += effect.magicPower;
                PowerDisplay.CriticalPower += effect.criticalPower;
                timeRemaining = Math.Max(timeRemaining, (effect.duration.HasValue) ? (effect.duration.Value - (DateTime.Now - effect.activationTime).TotalSeconds) : 0);
            }
            PowerDisplay.TimeRemaining = (int)Math.Round(timeRemaining);
        }

        private void Playsound(string effectName, bool granted = true)
        {
            var soundEffectSettings = GetSoundSettingsFor(effectName, granted);
            if (soundEffectSettings.playSound && !string.IsNullOrEmpty(soundEffectSettings.path))
                ActGlobals.oFormActMain.PlaySoundMethod(soundEffectSettings.path, Math.Min(100, Math.Max(0, soundEffectSettings.volume)));
        }

        private void CheckTimeRemaining(object sender, ElapsedEventArgs e)
        {
            foreach (var activeEffect in effects)
            {
                if (!activeEffect.duration.HasValue && activeEffect.effect.duractionDetectorRegex != null)
                {
                    foreach (var backlogLine in backLog)
                    {
                        var backlogMatch = activeEffect.effect.duractionDetectorRegex.Match(backlogLine);
                        if (backlogMatch.Success)
                        {
                            var targetMatches = backlogMatch.Groups["target"].ToString() == activeEffect.target;
                            var sourceMatches = backlogMatch.Groups["source"].ToString() == activeEffect.source;
                            if (targetMatches && sourceMatches)
                            {
                                double duration;
                                if (double.TryParse(backlogMatch.Groups["duration"].ToString(), out duration))
                                    activeEffect.duration = (int)Math.Round(duration);
                            }
                        }
                    }
                }
            }

            var timeRemaining = 0.0;
            var physicalPower = 0;
            var magicPower = 0;
            var criticalPower = 0;
            foreach (var effect in effects.ToArray())
            {
                var effectTimeRemaining = (effect.duration ?? 0) - (DateTime.Now - effect.activationTime).TotalSeconds;

                // Update powers.
                if (effect.effect.physicalChangeByTime != null)
                    effect.physicalPower = effect.effect.physicalPower + (int)(Math.Floor((effect.effect.duration.Value - effectTimeRemaining) / effect.effect.physicalChangeByTime.interval) * effect.effect.physicalChangeByTime.change);
                if (effect.effect.magicChangeByTime != null)
                    effect.magicPower = effect.effect.magicPower + (int)(Math.Floor((effect.effect.duration.Value - effectTimeRemaining) / effect.effect.magicChangeByTime.interval) * effect.effect.magicChangeByTime.change);
                if (effect.effect.criticalChangeByTime != null)
                    effect.criticalPower = effect.effect.criticalPower + (int)(Math.Floor((effect.effect.duration.Value - effectTimeRemaining) / effect.effect.criticalChangeByTime.interval) * effect.effect.criticalChangeByTime.change);

                physicalPower += effect.physicalPower;
                magicPower += effect.magicPower;
                criticalPower += effect.criticalPower;

                if (effect.duration.HasValue && effectTimeRemaining <= 0)
                {
                    effects.Remove(effect);
                    Playsound(effect.effect.name, false);
                    continue;
                }
                timeRemaining = Math.Max(timeRemaining, (effect.duration.HasValue) ? effectTimeRemaining : 0);
            }

            PowerDisplay.PhysicalPower = physicalPower;
            PowerDisplay.MagicPower = magicPower;
            PowerDisplay.CriticalPower = criticalPower;
            PowerDisplay.TimeRemaining = (int)Math.Round(timeRemaining);
        }

    }
}
