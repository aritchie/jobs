## FAQ

Q. How long does the background sync let me have on iOS

> 30 seconds and not a penny more

Q. How long does a task run on iOS

> 3 minutes on iOS 10+, 10 mins on iOS 8+

Q. How do I schedule periodic jobs?

> All jobs are considered periodic with or without criteria

Q. Why no job triggers? (ie. geofence, bluetooth, specific time)

> I am considering some triggers in the future. The current limitations on the time factored jobs is that iOS is in complete control of how/when things are run

Q. How many jobs can I run?

> Technically as many as you want... BUT this was built with mobile timeslicing in mind (ie. iOS).  Your job set needs to complete within that timeslice as we don't set job ordering currently

Q. Is there job priorization?

> Not yet - debating this one for the future