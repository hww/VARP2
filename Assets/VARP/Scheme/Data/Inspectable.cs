/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Scheme.Data
{
    public interface Inspectable
    {

        /** 
         * Any object with this interface has it own method to render 
         * debug information.
         */
        string Inspect ( InspectOptions options = InspectOptions.Default );

    }
}
