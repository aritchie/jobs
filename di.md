# Dependency Injection

## TODO

Take a look at the Samples below to show how this works.


Android makes DI more difficult - you need to set
CrossJob.ResolveJob = (jobInfo) => return IJob in the MainApplication

for iOS, you can set it the AppDelegate or Xamarin Forms initialization like you normally do


Autofac is also a little more difficult because it is an immutable container.  Take a look at AutofacRegistrationSource in the sample to see how to forward types