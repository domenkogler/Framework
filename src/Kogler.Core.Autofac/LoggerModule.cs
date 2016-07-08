using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Core;

namespace Kogler.Framework.Autofac
{
    public abstract class LoggerModule : global::Autofac.Module
    {
        protected LoggerModule() { }

        protected LoggerModule(params Type[] resolvedTypes) : this()
        {
            foreach (var type in resolvedTypes)
            {
                ResolvedLoggerTypes.Add(type);
            }
        }

        protected abstract object GetLogger(string loggerName);

        public List<Type> ResolvedLoggerTypes { get; protected set; }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += OnPreparing;
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        private void OnPreparing(object sender, PreparingEventArgs e)
        {
            var rp = new ResolvedParameter(
                (p, i) => ResolvedLoggerTypes.Any(t => p.ParameterType.GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())),
                (p, i) => GetLogger(p, e.Component.Activator.LimitType));

            var list = e.Parameters.ToList();
            list.Add(rp);
            e.Parameters = list;
        }

        private object GetLogger(ParameterInfo p, Type type)
        {
            type = p.Member.DeclaringType ?? type;
            var loggerNames = new List<string>(3) { type.Name };
            var ta = GetLA(type.GetTypeInfo().CustomAttributes);
            var pa = GetLA(p.CustomAttributes);
            ProccessLA(ta, loggerNames, type.Name);
            ProccessLA(pa, loggerNames, type.Name);
            return GetLogger(string.Join("+", loggerNames));
        }
        private LoggerAttribute GetLA(IEnumerable<CustomAttributeData> attributes)
        {
            return attributes.GetAttributes<LoggerAttribute>().FirstOrDefault();
        }

        private void ProccessLA(LoggerAttribute a, ICollection<string> loggerNames, string typeName)
        {
            if (a == null) return;
            if (!a.StackNames) loggerNames.Clear();
            loggerNames.Add(string.IsNullOrEmpty(a.Name) ? typeName : a.Name);
        }
    }
}