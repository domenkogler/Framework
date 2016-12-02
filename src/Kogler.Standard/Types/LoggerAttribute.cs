using System;

namespace Kogler.Standard
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class)]
    public class LoggerAttribute : Attribute
    {
        public string Name { get; }
        public bool StackNames { get; set; } = false;

        public LoggerAttribute(string name)
        {
            Name = name;
        }
    }
}