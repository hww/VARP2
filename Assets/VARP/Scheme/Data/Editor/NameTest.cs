/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using UnityEditor;
using UnityEngine;
using NUnit.Framework;
using System.Collections;


namespace VARP.Scheme.Data.Test
{
    public class SchemeSymbolTest
    {

        [Test]
        public void NameTest()
        {
			Name.Init();
            int quantity = 100;

            Name[] names = new Name[quantity];
            for (int i = 0; i < quantity; i++)
            {
                names[i] = Name.Intern(i.ToString());
            }

            Name abcdName = Name.Intern("ABCDEFGH");

            for (int i = 0; i < quantity; i++)
            {
                Name name = Name.Intern( i.ToString ( ) );
                Assert.AreEqual(name, names[i]);
                Assert.AreNotEqual(name, abcdName);
            }
			Name.DeInit();
        }
    }
}