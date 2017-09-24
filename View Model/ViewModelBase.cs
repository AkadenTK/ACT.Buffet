using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Buffet
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>(ref T backingField, T Value, [CallerMemberName] string propertyName = "")
        {
            var changed = !EqualityComparer<T>.Default.Equals(backingField, Value);

            if (changed)
            {
                backingField = Value;
                this.RaisePropertyChanged(propertyName);

                if (_reflections.ContainsKey(propertyName))
                    foreach (var reflectedPropertyName in _reflections[propertyName])
                        RaisePropertyChanged(reflectedPropertyName);
            }

            return changed;
        }

        #region Reflection

        private Dictionary<string, List<string>> _reflections = new Dictionary<string, List<string>>();

        protected void RegisterReflection(string sourcePropertyName, params string[] targetPropertyNames)
        {
            if (!_reflections.ContainsKey(sourcePropertyName)) _reflections.Add(sourcePropertyName, new List<string>());

            foreach(var propertyName in targetPropertyNames)
            {
                if (!_reflections[sourcePropertyName].Contains(propertyName)) _reflections[sourcePropertyName].Add(propertyName);
            }
        }

        protected void RegisterReflectionSources(string targetPropertyName, params string[] sourcePropertyNames)
        {
            foreach (string sourcePropertyName in sourcePropertyNames)
                RegisterReflection(sourcePropertyName, targetPropertyName);
        }

        protected void UnregisterReflection(string sourcePropertyName, params string[] targetPropertyNames)
        {
            if (!_reflections.ContainsKey(sourcePropertyName)) _reflections.Add(sourcePropertyName, new List<string>());

            foreach(var propertyName in targetPropertyNames)
            {
                if (_reflections[sourcePropertyName].Contains(propertyName)) _reflections[sourcePropertyName].Remove(propertyName);
            }
        }

        #endregion
    }
}
