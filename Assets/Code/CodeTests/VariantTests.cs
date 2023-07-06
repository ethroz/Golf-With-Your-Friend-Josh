using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class VariantTests
{
    [Test]
    public void VariantBasic()
    {
        Variant<bool, int> vnt = new(true);
        Assert.True(vnt.Is<bool>());
        Assert.True(vnt.Get<bool>());

        vnt = new(1);
        Assert.True(vnt.Is<int>());
        Assert.AreEqual(1, vnt.Get<int>());

        Assert.False(vnt.Is<bool>());
        Assert.Throws<InvalidCastException>(() => vnt.Get<bool>());
    }

    class A { public int Value; public A() { Value = 0; } }

    class B : A { public B() { Value = 1; } }

    class C : A { public C() { Value = 2; } }

    [Test]
    public void VariantInherited()
    {
        Variant<A, B, C> vnt = new(new A());
        Assert.True(vnt.Is<A>());
        Assert.False(vnt.Is<B>());
        Assert.False(vnt.Is<C>());
        Assert.AreEqual(0, vnt.Get<A>().Value);
        Assert.Throws<InvalidCastException>(() => vnt.Get<C>());

        vnt = new(new B());
        Assert.False(vnt.Is<A>());
        Assert.True(vnt.Is<B>());
        Assert.False(vnt.Is<C>());
        Assert.AreEqual(1, vnt.Get<B>().Value);
        Assert.Throws<InvalidCastException>(() => vnt.Get<A>());

        vnt = new(new C());
        Assert.False(vnt.Is<A>());
        Assert.False(vnt.Is<B>());
        Assert.True(vnt.Is<C>());
        Assert.AreEqual(2, vnt.Get<C>().Value);
        Assert.Throws<InvalidCastException>(() => vnt.Get<B>());
    }

    [Test]
    public void VariantStringObject()
    {
        Variant<object, string> vnt = new(1);
        Assert.True(vnt.Is<object>());
        Assert.False(vnt.Is<string>());
        Assert.False(vnt.Is<int>());
        Assert.AreEqual(1, (int)vnt.Get<object>());
        Assert.Throws<InvalidCastException>(() => vnt.Get<int>());

        vnt = new(new B());
        Assert.True(vnt.Is<object>());
        Assert.False(vnt.Is<string>());
        Assert.False(vnt.Is<B>());
        Assert.AreEqual(1, ((B)vnt.Get<object>()).Value);
        Assert.Throws<InvalidCastException>(() => vnt.Get<B>());

        vnt = new("Hello");
        Assert.True(vnt.Is<string>());
        Assert.False(vnt.Is<object>());
        Assert.AreEqual("Hello", vnt.Get<string>());
        Assert.Throws<InvalidCastException>(() => vnt.Get<object>());
    }

    [Test]
    public void CallbackVariantBasic()
    {
        CallbackVariant<byte, short, int, long> vnt = new((byte)255);
        Assert.True(vnt.Is<byte>());
        Assert.False(vnt.Is<short>());
        Assert.False(vnt.Is<int>());
        Assert.False(vnt.Is<long>());

        Assert.AreEqual(255, vnt.Get<byte>());
        Assert.Throws<InvalidCastException>(() => vnt.Get<short>());
        Assert.Throws<InvalidCastException>(() => vnt.Get<int>());
        Assert.Throws<InvalidCastException>(() => vnt.Get<long>());

        vnt.Set<byte>(128);
        Assert.AreEqual(128, vnt.Get<byte>());
        Assert.Throws<TypeAccessException>(() => vnt.Set(0L));

        List<byte> values = new(); 
        vnt.AddCallback((byte val) => values.Add(val));
        Assert.AreEqual(128, vnt.Get<byte>());
        Assert.AreEqual(0, values.Count);

        vnt.Invoke();
        Assert.AreEqual(1, values.Count);
        Assert.AreEqual(128, values[0]);

        Assert.Throws<TypeAccessException>(() => vnt.AddCallback((short val) => {}));

        vnt.AddCallback((byte val) => values.Add(val));

        vnt.Set<byte>(125);
        Assert.AreEqual(125, vnt.Get<byte>());
        Assert.AreEqual(3, values.Count);
        Assert.AreEqual(125, values[1]);
        Assert.AreEqual(125, values[2]);
    }

    const string key = "_VARIANT_TESTS";

    [Test]
    public void SettingsVariantBool()
    {
        PlayerPrefs.DeleteKey(key);
        
        SettingsVariant vnt = new(key, true);
        Assert.True(vnt.Get<bool>());

        vnt.Set(false);
        Assert.False(vnt.Get<bool>());

        vnt = new(key, true);
        Assert.False(vnt.Get<bool>());

        PlayerPrefs.DeleteKey(key);
    }

    [Test]
    public void SettingsVariantInt()
    {
        PlayerPrefs.DeleteKey(key);
        
        SettingsVariant vnt = new(key, 1);
        Assert.AreEqual(1, vnt.Get<int>());

        vnt.Set(2);
        Assert.AreEqual(2, vnt.Get<int>());

        vnt = new(key, 1);
        Assert.AreEqual(2, vnt.Get<int>());

        PlayerPrefs.DeleteKey(key);
    }

    [Test]
    public void SettingsVariantFloat()
    {
        PlayerPrefs.DeleteKey(key);
        
        SettingsVariant vnt = new(key, 1.5f);
        Assert.AreEqual(1.5f, vnt.Get<float>());

        vnt.Set(2.5f);
        Assert.AreEqual(2.5f, vnt.Get<float>());

        vnt = new(key, 1.5f);
        Assert.AreEqual(2.5f, vnt.Get<float>());

        PlayerPrefs.DeleteKey(key);
    }

    [Test]
    public void SettingsVariantString()
    {
        PlayerPrefs.DeleteKey(key);
        
        SettingsVariant vnt = new(key, "hi");
        Assert.AreEqual("hi", vnt.Get<string>());

        vnt.Set("bye");
        Assert.AreEqual("bye", vnt.Get<string>());

        vnt = new(key, "hi");
        Assert.AreEqual("bye", vnt.Get<string>());

        PlayerPrefs.DeleteKey(key);
    }

    [Test]
    public void SettingsVariantCallback()
    {
        PlayerPrefs.DeleteKey(key);
        
        SettingsVariant vnt = new(key, "hi");
        Assert.AreEqual("hi", vnt.Get<string>());

        List<string> values = new();
        vnt.AddCallback((string val) => values.Add(val));
        Assert.AreEqual(0, values.Count);

        vnt.Invoke();
        Assert.AreEqual(1, values.Count);
        Assert.AreEqual("hi", values[0]);

        vnt.Set("bye");
        Assert.AreEqual("bye", vnt.Get<string>());
        Assert.AreEqual(2, values.Count);
        Assert.AreEqual("bye", values[1]);

        PlayerPrefs.DeleteKey(key);
    }
}
