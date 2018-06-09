/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.VM
{
    using Data;

    public class Binding : SObject
    {
        public Environment environment;
        public object value;
    }
}
