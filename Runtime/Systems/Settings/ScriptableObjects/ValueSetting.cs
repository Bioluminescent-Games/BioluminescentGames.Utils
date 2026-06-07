using System;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    public abstract class ValueSetting<T> : SavableSetting
    {
        [field: SerializeField] public T DefaultValue { get; private set; }
        [field: SerializeField] public bool AutoApply { get; private set; }

        /// <summary>
        /// Called when the value of the setting changes, or when it is first loaded.
        /// </summary>
        public event Action<T> OnChanged;

        protected T InternalValue;
        private T _currentValue;

        public T Value
        {
            set
            {
                InternalValue = value;
                if (AutoApply)
                    ApplyValues();
            }
            get => _currentValue;
        }

        public override void OnApply()
        {
            ApplyValues();

            base.OnApply();
        }

        protected virtual void ApplyValues()
        {
            _currentValue = InternalValue;
            OnChanged?.Invoke(InternalValue);
        }

        public override void Init()
        {
            base.Init();

            ApplyValues();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OnChanged = null;
        }
    }
}
