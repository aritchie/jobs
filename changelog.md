# CHANGE LOG

## 1.2.1
* [fix] Support complex objects & lists in job parameters

## 1.2.0
* [feature] Built-in log trimming job - Use JobManager.ScheduleLogTrimmingJob(TimeSpan MaxAge)

## 1.1.1
* [ios][fix] RunAll is running inside RunTask producing 2 background tasks /w is also producing bad reporting
* [ios][fix] RunTask now supports cancellation (only iOS has this functionality)

## 1.1.0
* Better internal parameter serializer
* JobParameters was an unnecessary interface and has been replaced by plain-jane IDictionary /w some beautiful extension methods

## 1.0.0
* Initial Release