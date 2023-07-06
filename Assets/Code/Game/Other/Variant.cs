using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public interface IVariantHolder
    {
        bool Is<T>();
    }

    sealed class VariantHolder<T> : IVariantHolder
    {
        public T Item { get; }
        public bool Is<U>() => typeof(U) == typeof(T);
        public VariantHolder(T item) => Item = item;
    }

    public class Variant
    {
        protected IVariantHolder variant;
        public bool Is<T>() => variant.Is<T>();
        public T Get<T>() => ((VariantHolder<T>)variant).Item;
        protected Variant(IVariantHolder item) => variant = item;
    }

    public class Variant<T1, T2> : Variant
    {
        public Variant(T1 item) : base(new VariantHolder<T1>(item)) { }
        public Variant(T2 item) : base(new VariantHolder<T2>(item)) { }
    }

    public class Variant<T1, T2, T3> : Variant
    {
        public Variant(T1 item) : base(new VariantHolder<T1>(item)) { }
        public Variant(T2 item) : base(new VariantHolder<T2>(item)) { }
        public Variant(T3 item) : base(new VariantHolder<T3>(item)) { }
    }

    public class Variant<T1, T2, T3, T4> : Variant
    {
        public Variant(T1 item) : base(new VariantHolder<T1>(item)) { }
        public Variant(T2 item) : base(new VariantHolder<T2>(item)) { }
        public Variant(T3 item) : base(new VariantHolder<T3>(item)) { }
        public Variant(T4 item) : base(new VariantHolder<T4>(item)) { }
    }
    
    // ...
    // Add more variants as needed.
    //

    public class CallbackVariant<T1, T2, T3, T4> : Variant<T1, T2, T3, T4>
    {
        private List<IVariantHolder> callbacks;

        public CallbackVariant(T1 item) : base(item) => callbacks = new();
        public CallbackVariant(T2 item) : base(item) => callbacks = new();
        public CallbackVariant(T3 item) : base(item) => callbacks = new();
        public CallbackVariant(T4 item) : base(item) => callbacks = new();

        public virtual void Set<T>(T val)
        {
            if (!Is<T>()) throw new TypeAccessException();
            variant = new VariantHolder<T>(val);
            Invoke();
        }

        public void AddCallback<T>(Action<T> callback)
        {
            if (!Is<T>()) throw new TypeAccessException();
            callbacks.Add(new VariantHolder<Action<T>>(callback));
        }

        public void Invoke()
        {
            if      (Is<T1>()) foreach (var callback in callbacks) { ((VariantHolder<Action<T1>>)callback).Item(Get<T1>()); }
            else if (Is<T2>()) foreach (var callback in callbacks) { ((VariantHolder<Action<T2>>)callback).Item(Get<T2>()); }
            else if (Is<T3>()) foreach (var callback in callbacks) { ((VariantHolder<Action<T3>>)callback).Item(Get<T3>()); }
            else if (Is<T4>()) foreach (var callback in callbacks) { ((VariantHolder<Action<T4>>)callback).Item(Get<T4>()); }
        }
    }

    public class SettingsVariant : CallbackVariant<bool, int, float, string>
    {
        private readonly string name;

        public SettingsVariant(string name, bool def) : base(PlayerPrefs.GetInt(name, def ? 1 : 0) == 1) => this.name = name;
        public SettingsVariant(string name, int def) : base(PlayerPrefs.GetInt(name, def)) => this.name = name;
        public SettingsVariant(string name, float def) : base(PlayerPrefs.GetFloat(name, def)) => this.name = name;
        public SettingsVariant(string name, string def) : base(PlayerPrefs.GetString(name, def)) => this.name = name;

        public override void Set<T>(T val)
        {
            base.Set(val);
            if      (Is<bool>())   PlayerPrefs.SetInt(name, Get<bool>() ? 1 : 0);
            else if (Is<int>())    PlayerPrefs.SetInt(name, Get<int>());
            else if (Is<float>())  PlayerPrefs.SetFloat(name, Get<float>());
            else if (Is<string>()) PlayerPrefs.SetString(name, Get<string>());
        }

        public static SettingsVariant Create<T>(string name, T val)
        {
            if (typeof(T) == typeof(bool))   return new SettingsVariant(name, (bool)(object)val);
            if (typeof(T) == typeof(int))    return new SettingsVariant(name, (int)(object)val);
            if (typeof(T) == typeof(float))  return new SettingsVariant(name, (float)(object)val);
            if (typeof(T) == typeof(string)) return new SettingsVariant(name, (string)(object)val);
            throw new NotSupportedException();
        }
    }
}
