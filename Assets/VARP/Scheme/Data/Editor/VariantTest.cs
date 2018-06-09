/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using NUnit.Framework;
using System.Collections.Generic;

namespace VARP.Scheme.Data.Test
{
    using DataStructures;

    /// <summary>
    /// Some tests for Variant class
    /// </summary>
    public class VariantTest
    {

        [Test]
        public void BooleanTest()
        {
            Variant t = (Variant)true;
            Variant f = (Variant)false;
            Variant t1 = (Variant)true;
            Variant f1 = (Variant)false;
            Assert.AreEqual(true, (bool)t);
            Assert.AreEqual(false, (bool)f);
            Assert.AreEqual(t, t1);
            Assert.AreEqual(f, f1);
            Assert.AreNotEqual(t, f);
            Assert.AreNotEqual(t1, f1);
            Assert.AreNotEqual(true, (bool)f);
            Assert.AreNotEqual(false, (bool)t);
        }


        [Test]
        public void IntTest()
        {
            Variant a1 = (Variant)( 1);
            Variant a2 = (Variant)( 2);
            Variant b1 = (Variant)( 1);
            Variant b2 = (Variant)( 2);
            Assert.AreEqual(1, (int)a1);
            Assert.AreEqual(2, (int)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(1, (int)a2);
            Assert.AreNotEqual(2, (int)a1);
        }

        [Test]
        public void FloatTest()
        {
            Variant a1 = (Variant)(1.1f);
            Variant a2 = (Variant)(2.1f);
            Variant b1 = (Variant)(1.1f);
            Variant b2 = (Variant)( 2.1f);
            Assert.AreEqual(1.1f, (float)a1);
            Assert.AreEqual(2.1f, (float)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(1.1f, (float)a2);
            Assert.AreNotEqual(2.1f, (float)a1);
        }

        [Test]
        public void NameTest()
        {
			Name.Init();
            Variant a1 = (Variant)(Name.Intern("x1"));
            Variant a2 = (Variant)(Name.Intern("x2"));
            Variant b1 = (Variant)(Name.Intern("x1"));
            Variant b2 = (Variant)(Name.Intern("x2"));
            Assert.AreEqual(Name.Intern("x1"), (Name)a1);
            Assert.AreEqual(Name.Intern("x2"), (Name)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(Name.Intern("x1"), (Name)a2);
            Assert.AreNotEqual(Name.Intern("x2"), (Name)a1);
			Name.DeInit();
        }

    }
}
