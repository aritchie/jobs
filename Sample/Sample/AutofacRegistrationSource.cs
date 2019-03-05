using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;


namespace Sample
{
    public class AutofacRegistrationSource : IRegistrationSource
    {
        readonly IComponentContext context;


        public AutofacRegistrationSource(IComponentContext context)
            => this.context = context;


        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
            => this.context.ComponentRegistry.RegistrationsFor(service);

        public bool IsAdapterForIndividualComponents => false;
    }
}
