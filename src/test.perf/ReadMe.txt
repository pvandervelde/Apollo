The goal for the Test.Perf project is to provide performance related numbers. This assembly will not provide comparisons that indicate if 
performance has improved or deteriorated. The final comparisons will probably have to be done manually. However by just providing a way to 
measure performance (CPU time, memory consumption etc. etc.) it will be possible to aggregate the performance data and thus make the right
comparisons (e.g. on the build server or in other locations).

A suggestion to make this work is to use the performance counters. We'll need some external application for that though. This assembly
can then indicate what actions should be invoked and what should be measured.