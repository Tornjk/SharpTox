using System;
using System.Collections.Generic;
using System.Text;

namespace SharpTox
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property)]
    class NotNullAttribute : Attribute
    {
    }
}
